using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CRL.Core
{
    /// <summary>
    /// 操作Cookie的帮助类
    /// </summary>
    public class CookieHelper
    {
        /// <summary>
        /// 添加一个新的Cookie到HttpCookes集合
        /// </summary>
        /// <param name="strCookName">Cookie的名称</param>
        /// <param name="strCookValue">Cookie的值</param>
        public static void AddCookies(string strCookName, string strCookValue)
        {
            HttpCookie Cookies = new HttpCookie(strCookName, strCookValue);
            System.Web.HttpContext.Current.Response.AppendCookie(Cookies);
        }

        /// <summary>
        /// 添加一个Cookie到HttpCookes集合并设置其过期时间
        /// </summary>
        /// <param name="strCookName">cookie名称</param>
        /// <param name="strCookValue">cookie值</param>
        /// <param name="dtExpires">过期时间</param>
        public static void AddCookies(string strCookName, string strCookValue, DateTime dtExpires)
        {
            HttpCookie myCookies = new HttpCookie(strCookName);
            myCookies.Value = strCookValue;
            myCookies.Expires = dtExpires;
            System.Web.HttpContext.Current.Response.Cookies.Add(myCookies);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCookName"></param>
        /// <param name="strCookValue"></param>
        /// <param name="dtExpires"></param>
        /// <param name="domain">Cookie域</param>
        public static void AddCookies(string strCookName, string strCookValue, DateTime dtExpires, string domain)
        {
            HttpCookie myCookies = new HttpCookie(strCookName);
            myCookies.Value = strCookValue;
            if (dtExpires != null)
            {
                myCookies.Expires = dtExpires;
            }
            if (domain != null)
            {
                myCookies.Domain = domain;
            }
            System.Web.HttpContext.Current.Response.Cookies.Add(myCookies);
        }
        /// <summary>
        /// 删除指定的Cookie
        /// </summary>
        /// <param name="strCookName">Cookie名称</param>
        public static void DelCookies(string strCookName)
        {
            if (System.Web.HttpContext.Current.Request.Cookies[strCookName] != null)
            {
                HttpCookie myCookie = new HttpCookie(strCookName);
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                System.Web.HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }

        /// <summary>
        /// 获取指定Cookie的值
        /// </summary>
        /// <param name="strCookName">Cookie名称</param>
        /// <returns></returns>
        public static string GetCookieValue(string strCookName)
        {
            if (HttpContext.Current.Request.Cookies[strCookName] != null)
            {
                return HttpContext.Current.Request.Cookies[strCookName].Value;
            }
            else
            {
                return String.Empty;
            }
        }

    }
}
