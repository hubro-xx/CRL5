using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
namespace CRL.Core
{
    /// <summary>
    /// 图形验证码生成和判断
    /// </summary>
    public class VerifyImage
    {
        static string defaultName = "CheckCode";
        /// <summary>
        /// 以默认名称输出验证码
        /// </summary>
        public static void DrawImage()
        {
            DrawImage(defaultName);
        }
        public static Image MakeImage(out string chkCode)
        {
            chkCode = "";
            //颜色列表，用于验证码、噪线、噪点
            Color[] color ={ Color.Black, Color.DarkGray,  Color.Green, Color.DarkOrange,
Color.Brown, Color.DarkBlue };

            //字体列表，用于验证码

            string[] font = { "Times New Roman", "MS Mincho", "Book Antiqua", "Trajan Pro", "PMingLiU", "Tekton Pro Ext" };
            FontStyle[] fontStyles = { FontStyle.Bold, FontStyle.Italic, FontStyle.Regular };
            //验证码的字符集
            string character = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //character += "123456789";
            Random rnd = new Random();
            //生成验证码字符串
            for (int i = 0; i < 4; i++)
            {
                var c = character[rnd.Next(character.Length)].ToString();
                var n = rnd.Next(20);
                if (n % 2 == 0)
                {
                    c = c.ToLower();
                }
                chkCode += c;
            }
            Bitmap bmp = new Bitmap(100, 35);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.White);
            Color clr = color[rnd.Next(color.Length)];
            ////画噪线
            //for (int i = 0; i < 5; i++)
            //{
            //    int x1 = rnd.Next(100);
            //    int y1 = rnd.Next(25);
            //    int x2 = rnd.Next(100);
            //    int y2 = rnd.Next(25);
            //    //Color clr = color[rnd.Next(color.Length)];
            //    g.DrawLine(new Pen(clr,2), x1, y1, x2, y2);
            //}
            //画验证码字符串
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, rnd.Next(18, 26), fontStyles[rnd.Next(fontStyles.Length)]);
                //Color clr = color[rnd.Next(color.Length)];
                float fX = (float)i * 22 + rnd.Next(-4, 6);
                float fY = (float)rnd.Next(-4, 1);
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), fX, fY);
            }
            ////画噪点
            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next(bmp.Width);
                int y = rnd.Next(bmp.Height);
                //Color clr = color[rnd.Next(color.Length)];
                bmp.SetPixel(x, y, clr);
            }
            g.Dispose();
            return bmp;
        }
        /// <summary>
        /// 以自定义名称输出验证码
        /// </summary>
        /// <param name="sessionName"></param>
        public static void DrawImage(string sessionName)
        {
            HttpContext context = HttpContext.Current;
            string chkCode = string.Empty;
            Image bmp = MakeImage(out chkCode);
            context.Session[sessionName] = chkCode.ToLower();
            //清除该页输出缓存，设置该页无缓存
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = System.DateTime.Now.AddMilliseconds(0);
            context.Response.Expires = 0;
            context.Response.CacheControl = "no-cache";
            context.Response.AppendHeader("Pragma", "No-Cache");
            //将验证码图片写入内存流，并将其以"image/Png" 格式输出
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                context.Response.ClearContent();
                context.Response.ContentType = "image/Png";
                context.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                //显式释放资源
                bmp.Dispose();
                ms.Dispose();
            }
        }

        /// <summary>
        /// 以默认名称对比验证码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool Check(string input, out string error)
        {
            return Check(defaultName, input, out error);
        }
        /// <summary>
        /// 以自定义名称对比验证码
        /// </summary>
        /// <param name="sessionName"></param>
        /// <param name="input"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool Check(string sessionName,string input,out string error)
        {
            error = "";
            HttpContext context = HttpContext.Current;
            if (string.IsNullOrEmpty(input))
            {
                error = "请输入验证码";
                return false;
            }
            if (context.Session[sessionName]==null)
            {
                error = "验证码未初始化,请刷新重试";
                return false;
            }
            if ((context.Session[sessionName] + "").ToLower() != input.ToLower())
            {
                error = "验证码不正确";
                return false;
            }
            return true;
        }
    }
}
