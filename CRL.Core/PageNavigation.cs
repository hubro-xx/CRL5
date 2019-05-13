using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace CRL.Core
{
    /// <summary>
    /// Get分页导航
    /// </summary>
    public class PageNavigation
    {
        public enum PageStyle
        {
            Default,
            Taobao,
            Google,
            ABL
        }
        string inputValueName = "pageNavigationValue";
        string _name;
        /// <summary>
        /// 参数编码
        /// </summary>
        public Encoding CurrentEncoding = Encoding.UTF8;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PageNavigation()
        {
            _name = "getPager";
        }
        /// <summary>
        /// 通过后辍名构造函数
        /// </summary>
        /// <param name="name"></param>
        public PageNavigation(string name)
        {
            _name = name;
            inputValueName += name;
        }
        /// <summary>
        /// 设置分页样式
        /// </summary>
        /// <param name="pageStyle"></param>
        public void SetPageStyle(PageStyle pageStyle)
        {
            if (pageStyle == PageStyle.Taobao)
            {
                ShowCurrentBeforPage = false;
                FirstPageText = "1";
                LastPageText = "{0}";
                FnFormat = "{1} {0}";
                NlFormat = "{1} {0}";
                ShowEllipsis = true;
                IndexCenter = true;
            }
            else if (pageStyle == PageStyle.Google)
            {
                IndexCenter=true;
                ShowJump = false;
                FirstPageText = "";
                LastPageText = "";
            }
            else if (pageStyle == PageStyle.ABL)
            {
                IndexCenter = true;
                ShowEllipsis = true;
                ShowJump = false;
                ShowCurrentBeforPage = true;
                FirstPageText = "|«";
                NextPageText = "»";
                PrevPageText = "«";
                LastPageText = "»|";
            }
        }
        /// <summary>
        /// 传值参数名
        /// </summary>
        public string ParameName = "page";
        /// <summary>
        /// 链接FORMAT表达式,like page_{0}
        /// 不会自动加上?连接符
        /// </summary>
        public string PageNavigationFormat = "";
        /// <summary>
        /// 首页按钮文字
        /// </summary>
        public string FirstPageText = "首页";
        /// <summary>
        /// 尾页按钮文字
        /// {0}将被替换成值
        /// </summary>
        public string LastPageText = "尾页";
        /// <summary>
        /// 下一页按钮文字
        /// </summary>
        public string NextPageText = "下一页";
        /// <summary>
        /// 上一页按钮文字
        /// </summary>
        public string PrevPageText = "上一页";

        /// <summary>
        /// 跳转到第几页文字
        /// </summary>
        public string JumpText = "跳转到第";

        /// <summary>
        /// 首页,上一页替换表达式
        /// 默认为{0} {1}
        /// </summary>
        public string FnFormat = "{0} {1}";

        /// <summary>
        /// 下一页,尾页替换表达式
        /// 默认为{0} {1}
        /// </summary>
        public string NlFormat = "{0} {1}";

        /// <summary>
        /// 在可见页范围内,是否显示首页或尾页连接
        /// </summary>
        public bool ShowCurrentBeforPage = true;

        /// <summary>
        /// 是否显示分页省略符
        /// </summary>
        public bool ShowEllipsis = false;
        /// <summary>
        /// 是否将当前页置于导航条中间
        /// </summary>
        public bool IndexCenter = false;
        /// <summary>
        /// 是否显示快速跳转提交
        /// </summary>
        public bool ShowJump = true;

        /// <summary>
        /// 获取传入的页值
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetPageIndex(HttpContext context)
        {
            string _index = context.Request[ParameName] + "";
            int index = 1;
            if (!string.IsNullOrEmpty(_index))
            {
                index = Convert.ToInt32(_index);
            }
            if (index == 0)
                index = 1;
            return index;
        }
        /// <summary>
        /// 获取分页导航代码
        /// </summary>
        /// <param name="index"></param>
        /// <param name="total"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public string GetPageNavigation(int index, int total, int pageSize)
        {
            int a = 10;
            if (ShowEllipsis)
            {
                a = 8;
            }
            int totalPage = total / pageSize;
            if (total % pageSize > 0)
                totalPage += 1;
            if (index > totalPage)
                index = totalPage;
            int zones = totalPage / a;
            if (totalPage % a > 0)
                zones += 1;
            int zoneIndex = index / a;
            if (index % a > 0)
                zoneIndex += 1;
            string str = "<div id='" + _name + "' class='paging'>";
            //str += string.Format("totalPage:{0} zones:{1} zoneIndex:{2}<br>", totalPage, zones, zoneIndex);
            if (index > 1)
            {
                string str1 = "<a class='firstPage' page='1' title='首页' href='" + GetParams(1) + "' style='margin-right:5px;'>" + FirstPageText + "</a> ";
                if (string.IsNullOrEmpty(FirstPageText))
                {
                    str1 = "";
                }
                int n = (zoneIndex - 1) * a;
                if (n == 0)
                    n = 1;
                string str2 = "<a class='prevPage' page='" +( index - 1) + "' title='上一页' href='" + GetParams(index - 1) + "' style='margin-right:5px;'>" + PrevPageText + "</a> ";
                str += string.Format(FnFormat, str1, str2);
            }
            else
            {
                if (!string.IsNullOrEmpty(FirstPageText))
                {
                    str += "<a disabled='disabled' page='" + 1 + "' class='firstPage' title='首页' style='margin-right:5px;'>" + FirstPageText + "</a> ";
                }
            }
            if (zoneIndex > 1 && ShowEllipsis)
            {
                str += "<a href='" + GetParams(2) + "' page='" +2 + "' style='margin-right:5px;'>" + 2 + "</a> ";
                str += " ... ";
            }
            int startIndex = (zoneIndex - 1) * a;
            if (startIndex == 0)
                startIndex = 1;
            if (IndexCenter&&index>5)
            {
                startIndex = index - 5;
                a = a - 1;
            }
            for (int b = startIndex; b <= startIndex+a; b++)
            {
                //int b = i + ((zoneIndex - 1) * a);
                if (b <= 0)
                {
                    continue;
                }
                if (b > totalPage)
                {
                    continue;
                }
                if (zoneIndex == 1 && b == 1 && !ShowCurrentBeforPage)
                {
                    continue;
                }
                if (zoneIndex == zones && b == totalPage && !ShowCurrentBeforPage)
                {
                    continue;
                }
                if (b != index)
                {
                    str += "<a href='" + GetParams(b) + "' page='" + b + "' style='margin-right:5px;'>" + b + "</a> ";
                }
                else
                {
                    str += "<a href='javascript:;' page='" + b + "' style='margin-right:5px;' class='current'>" + b + "</a> ";
                }
            }
            if (zoneIndex < zones && ShowEllipsis)
                str += " ... ";
 
            LastPageText = LastPageText.Replace("{0}", totalPage.ToString());
            if (index < totalPage)
            {
                int n = zoneIndex * a + 1;
                string str1 = "<a class='nextPage' page='" + (index+1) + "' title='下一页' href='" + GetParams(index + 1) + "' style='margin-right:5px;'>" + NextPageText + "</a> ";
                string str2 = "<a class='lastPage' page='" + (totalPage) + "' title='尾页' href='" + GetParams(totalPage) + "' style='margin-right:5px;'>" + LastPageText + "</a> ";
                if (string.IsNullOrEmpty(FirstPageText))
                {
                    str2 = "";
                }
                str += string.Format(NlFormat, str1, str2);
            }
            else
            {
                if (!string.IsNullOrEmpty(FirstPageText))
                {
                    str += "<a disabled='disabled' class='lastPage' title='尾页' style='margin-right:5px;'>" + LastPageText + "</a> ";
                }
            }
            if (ShowJump && index < totalPage)
            {
                string jumpUrl = GetParamsNoPage();
                string inputValue = string.Format("document.getElementById('{0}').value", inputValueName);
                str += string.Format(@" <span style='margin-right:5px;'>{0}<input type='text' value='{1}' onkeyup=""value=value.replace(/[^\d]/g,'')"" style='width:25px' id='{2}' name='" + ParameName + "'>页 ", JumpText, index + 1, inputValueName);
                str += string.Format(@"<button id='btnJump' onclick=""if({0}=='')return false;window.location='{1}'+{0}"">确定</button></span>", inputValue, jumpUrl);
            }
            str += "</div>";
            return str;
        }
        private string GetParamsNoPage()
        {
            if (!string.IsNullOrEmpty(PageNavigationFormat))
            {
                return string.Format(PageNavigationFormat, "");
            }
            NameValueCollection c = new NameValueCollection(HttpContext.Current.Request.QueryString);
            c.Remove(ParameName);
            string str = "?";
            foreach (string key in c.Keys)
            {
                str += key + "=" + c[key] + "&";
            }
            if (str.Length > 1)
            {
                str = str.Substring(0, str.Length - 1) + "&";
            }
            str += ParameName + "=";
            return str;
        }
        /// <summary>
        /// 计算分页链接
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public string GetParams(int pageIndex)
        {
            if (!string.IsNullOrEmpty(PageNavigationFormat))
            {
                return string.Format(PageNavigationFormat, pageIndex);
            }
            NameValueCollection c = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query,CurrentEncoding);
            c.Set(ParameName, pageIndex.ToString());
            string str = "?";
            foreach(string key in c.Keys)
            {
                str += key + "=" + HttpUtility.UrlEncode(c[key], CurrentEncoding) + "&";
            }
            if (str.Length > 1)
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }
    }
}
