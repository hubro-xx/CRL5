using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
namespace CRL.Core
{
    public class ImageHelper
    {
        /// <summary>
        /// 裁剪模式
        /// </summary>
        public enum CutMode
        {
            /// <summary>
            /// 指定宽，高按比例   
            /// </summary>
            WIDTH = 0,
            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            HEIGHT = 1,
            /// <summary>
            /// 指定高宽缩放（可能变形）   
            /// </summary>
            WIDTH_HEIGHT = 3,
            /// <summary>
            /// 指定高宽裁减（不变形）
            /// </summary>
            CUT = 4,
            /// <summary>
            /// 自动
            /// </summary>
            AUTO = 5
        }
        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        /// <param name="autoFill">是否填充到宽高</param>
        /// <param name="HightMode">是否高质量</param>
        /// <returns></returns>
        public static Image CutImg(Image originalImage, int width, int height, CutMode mode, bool autoFill = true, bool HightMode = true)
        {
            //将要达到的宽度
            int towidth = width;
            //将要达到的高度
            int toheight = height;

            int x = 0;
            int y = 0;
            //原始宽度
            int ow = originalImage.Width;
            //原始高度
            int oh = originalImage.Height;

            switch (mode)
            {
                case CutMode.WIDTH_HEIGHT://指定高宽缩放（可能变形）                
                    break;
                case CutMode.WIDTH://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case CutMode.HEIGHT://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case CutMode.CUT://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                case CutMode.AUTO://自动,在宽高内
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    else
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                    }
                    break;
                default:
                    break;
            }
            if (originalImage.Width < width)
            {
                //towidth = originalImage.Width;
                ow = originalImage.Width;
            }
            if (originalImage.Height < height)
            {
                //toheight = originalImage.Height;
                oh = originalImage.Height;
            }
            //达到的宽高大于原始图宽高
            if (towidth > ow && toheight > oh)
            {
                towidth = ow;
                toheight = oh;
            }
            if (!autoFill)
            {
                width = towidth;
                height = toheight;
            }
            Image bitmap = new System.Drawing.Bitmap(width, height);
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            if (HightMode)
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            }
            g.Clear(Color.White);
            int a = (height - toheight) / 2;
            int b = (width - towidth) / 2;
            g.DrawImage(originalImage, new Rectangle(b, a, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            // System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //originalImage.Dispose();
            //bitmap.Dispose();
            g.Dispose();
            return bitmap;
        }
        /// <summary>
        /// 生成指定个数缩略图
        /// </summary>
        /// <param name="file"></param>
        /// <param name="thumbnailMode">大于10则按值进行分割算宽高</param>
        /// <returns></returns>
        public static bool MakeThumbImage(string file, int thumbnailMode)
        {
            if (!System.IO.File.Exists(file))
                return false;
            //thumbnailMode 1,默认
            //2,一个小图,3,商品上传 总共3张
            //string[] arry = file.Split('.');
            if (!System.IO.File.Exists(file))
            {
                return false;
            }
            try
            {
                if (thumbnailMode == 2)
                {
                    Image image = Image.FromFile(file);
                    Image image1 = ImageHelper.CutImg(image, 150, 150, ImageHelper.CutMode.AUTO, false);
                    image1.Save(file + ".150150.jpg", ImageFormat.Jpeg);
                    image1.Dispose();
                    image.Dispose();
                }
                else if (thumbnailMode == 3)
                {
                    Image image = Image.FromFile(file);
                    Image image1 = ImageHelper.CutImg(image, 150, 150, ImageHelper.CutMode.AUTO, false);
                    image1.Save(file + ".150150.jpg", ImageFormat.Jpeg);
                    image1.Dispose();
                    Image image2 = ImageHelper.CutImg(image, 350, 350, ImageHelper.CutMode.AUTO, false);
                    image2.Save(file + ".350350.jpg", ImageFormat.Jpeg);
                    image2.Dispose();
                    image.Dispose();
                }
                else if (thumbnailMode > 10)
                {
                    //CRL.Core.EventLog.Log("准备生成小图");
                    //自定义长宽缩放,长宽值以字符串长度/2
                    string a = thumbnailMode.ToString();
                    int index = a.Length / 2;

                    if (a.Substring(index, 1) == "0")
                        index += 1;

                    int w = int.Parse(a.Substring(0, index));
                    int h = int.Parse(a.Substring(index));
                    if (a.Length % 2 > 0)
                    {
                        if (h / w > 5)
                        {
                            index += 1;
                            w = int.Parse(a.Substring(0, index));
                            h = int.Parse(a.Substring(index));
                        }
                    }
                    string saveFile = file + "." + thumbnailMode + ".jpg";
                    //存在就不生成了
                    if (System.IO.File.Exists(saveFile))
                        return true;
                    Image image = Image.FromFile(file);
                    Image image1 = ImageHelper.CutImg(image, w, h, ImageHelper.CutMode.AUTO, false);
                    image1.Save(file + "." + thumbnailMode + ".jpg", ImageFormat.Jpeg);
                    image1.Dispose();
                    image.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 加上水印
        /// </summary>
        /// <param name="source"></param>
        /// <param name="waterMarkImage"></param>
        /// <returns></returns>
        public static Image MakeWaterMark(Image source, Image waterMarkImage)
        {
            Graphics g = System.Drawing.Graphics.FromImage(source);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            ImageAttributes imageAttributes =
             new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            //第二个颜色处理用来改变水印的不透明性。      
            //通过应用包含提供了坐标的RGBA空间的5x5矩阵来做这个。      
            //通过设定第三行、第三列为0.3f我们就达到了一个不透明的水平。结果是水印会轻微地显示在图象底下一些。      
            imageAttributes.SetRemapTable(remapTable,
                 ColorAdjustType.Bitmap);
            float[][] colorMatrixElements = {
                                             new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                             new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                             new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                             new float[] {0.0f,  0.0f,  0.0f,  0.15f, 0.0f},
                                             new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                        };
            ColorMatrix wmColorMatrix = new
                 ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(wmColorMatrix,
                 ColorMatrixFlag.Default,
                 ColorAdjustType.Bitmap);

            int x = source.Width - waterMarkImage.Width;
            int y = source.Height - waterMarkImage.Height;
            int width = waterMarkImage.Width;
            int height = waterMarkImage.Height;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            if (width > source.Width / 2)
                width = width / 2;
            if (height > source.Height / 2)
                height = height / 2;

            g.DrawImage(waterMarkImage, new Rectangle(source.Width / 2 - width / 2, source.Height / 2 - height / 2, width, height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);

            g.Dispose();
            return source;
        }
    }
}
