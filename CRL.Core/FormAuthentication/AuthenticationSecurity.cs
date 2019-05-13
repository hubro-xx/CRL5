using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace CRL.Core.FormAuthentication
{

    /// <summary>
    /// Form验证，Cookie有效期通过CheckTicket进行限制
    /// </summary>
	public class AuthenticationSecurity
    {
        /// <summary>
        /// 是否启用CookieDomain
        /// </summary>
        public static bool CrossDomain = false;
        /// <summary>
        /// CookieDomain 名
        /// </summary>
        public static string CrossDomainCookieName = "__crsd";

        ///// <summary>
        ///// 获取当前用户,不存在返回空
        ///// </summary>
        ///// <param name="_user"></param>
        ///// <returns></returns>
        //public static IUser GetCurrentUser(IUser _user)
        //{
        //    string userTicket = HttpContext.Current.User.Identity.Name;
        //    if (string.IsNullOrEmpty(userTicket))
        //        return null;
        //    //数据不对会造成空
        //    IUser user = _user.ConverFromArry(userTicket);
        //    if (user == null)
        //    {
        //        LoginOut();
        //    }
        //    return user;
        //}
        /// <summary>
        /// 获取存在SESSION的自定义数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetUserData<T>() where T :class, new()
        {
            if (HttpContext.Current.Session["UserObj"] == null)
                return default(T);
            return (T)HttpContext.Current.Session["UserObj"];
        }
        /// <summary>
        /// 设置当前SESSION的自定义数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void SetUserData<T>(T obj) where T : class, new()
        {
            HttpContext.Current.Session["UserObj"] = obj;
        }
        /// <summary>
        /// Session是否验证通过
        /// 在Cookie验证通过时,验证此值,能保证是从网站上登录,并且以Session方式验证通过
        /// </summary>
        public static bool SessionVerified
        {
            get
            {
                return HttpContext.Current.Session["SessionState"] != null;
            }
        }
		/// <summary>
		/// 检测票据
		/// </summary>
		/// <param name="expires">
        /// 是否过期,过期会自动把过期时间延长20分钟,会造成COOKIE重写,需和SetTicket参数一致
		/// 如果域名为全域名则为FALSE
		/// </param>
        public static void CheckTicket(bool expires = false, string role = "")
        {
            if(CrossDomain)
            {
                CheckTicket2(expires, role);
                return;
            }
            FormsAuthenticationTicket authTicket = null;
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (authCookie == null)
                return;
            if (authCookie.Value == "")
                return;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket == null)
                    return;
                if (authTicket.IssueDate.Date != DateTime.Now.Date)
                {
                    //return;
                }
                if (!string.IsNullOrEmpty(role))//只设置指定ROLE权限
                {
                    if (!authTicket.UserData.Contains(role))
                    {
                        return;
                    }
                }
                string[] roles = authTicket.UserData.Split(new char[] { ',' });
                
                FormsIdentity id = new FormsIdentity(authTicket);

                System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, roles);
                HttpContext.Current.User = principal;

                if (expires)
                {
                    string domain = authCookie.Domain;
                    //如果不是主域COOKIE
                    if (domain == null || domain.Substring(0, 1) != ".")
                    {
                        //让20分钟后过期
                        authCookie.Expires = DateTime.Now.AddMinutes(20);
                        HttpContext.Current.Response.Cookies.Add(authCookie);
                    }
                }
            }
            catch (Exception ero)
            {
                HttpContext.Current.Response.Write("检测票据出现错误:" + ero);
            }
        }
        static string crossKey = "Hge34Lr5";
        public static void CheckTicket2(bool expires = false, string role = "")
        {
            string cookieName = CrossDomainCookieName;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (authCookie == null)
                return;
            if (authCookie.Value == "")
                return;
            var cookieValue = "";
            try
            {
                cookieValue = StringHelper.Decrypt(authCookie.Value, crossKey);
            }
            catch
            {
                return;
            }
            var array = cookieValue.Split('^');
            if (array.Length < 2)
            {
                return;
            }
            var ticketName = array[0];
            var userDate = array[1];
            //userDate = "Supplier";
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,  // 版本号
                        ticketName,  // 票据名
                        DateTime.Now,  // 票据发出时间
                        DateTime.Now.AddYears(100),  // 过期时间
                        true,  // 是否持久
                         userDate// 存储在 Cookie 中的用户定义数据,这里用来存取用户组
                        );

            FormsIdentity id = new FormsIdentity(authTicket);

            string[] roles = authTicket.UserData.Split(new char[] { ',' });

            System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, roles);
            HttpContext.Current.User = principal;

        }
        /// <summary>
        /// 设置票据
        /// </summary>
        /// <param name="user">IUser</param>
        /// <param name="rules">组</param>
        /// <param name="expires">是否过期,如果设置了域,则为false</param>
        public static void SetTicket(IUser user, string rules, bool expires)
        {
            SetTicket(user, rules, 0);
        }
		/// <summary>
        /// 设置票据
		/// </summary>
        /// <param name="user">IUser</param>
        /// <param name="rules">组</param>
		/// <param name="minute">过期分钟,0为不过期</param>
        public static void SetTicket(IUser user, string rules, int minute = 0)
        {
            if (CrossDomain)
            {
                SetTicket2(user, rules, minute);
                return;
            }
            string ticketName = user.ToArry();
            HttpContext context = HttpContext.Current;
            string userDate = rules;
            //DateTime expiration = DateTime.Now.AddMinutes(20);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,  // 版本号
                        ticketName,  // 票据名
                        DateTime.Now,  // 票据发出时间
                        DateTime.Now.AddYears(100),  // 过期时间
                        true,  // 是否持久
                         userDate// 存储在 Cookie 中的用户定义数据,这里用来存取用户组
                        );
            // Encrypt the ticket
            string cookieStr = FormsAuthentication.Encrypt(ticket);
            //context.Response.Cookies[FormsAuthentication.FormsCookieName].Value = cookieStr;
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieStr);

            if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain))
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
                cookie.Path = "/";
            }
            else
            {
                //如果过期
                if (minute > 0)
                {
                    cookie.Expires = DateTime.Now.AddMinutes(minute);
                }
            }
            context.Response.Cookies.Add(cookie);
            if (context.Session != null)
            {
                context.Session["SessionState"] = true;
            }
        }

        public static void SetTicket2(IUser user, string rules, int minute = 0)
        {
            string ticketName = user.ToArry();
            HttpContext context = HttpContext.Current;
            string userDate = rules;
            var str = ticketName + "^" + rules;//^分割
            string cookieStr = StringHelper.Encrypt(str, crossKey);
            //context.Response.Cookies[FormsAuthentication.FormsCookieName].Value = cookieStr;
            HttpCookie cookie = new HttpCookie(CrossDomainCookieName, cookieStr);

            if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain))
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
                cookie.Path = "/";
            }
            else
            {
                //如果过期
                if (minute > 0)
                {
                    cookie.Expires = DateTime.Now.AddMinutes(minute);
                }
            }
            context.Response.AppendCookie(cookie);
            if (context.Session != null)
            {
                context.Session["SessionState"] = true;
            }
        }
        /// <summary>
        /// 清空当前Cookie信息
        /// </summary>
        public static void LoginOut()
        {
            LoginOut(null);
        }
        /// <summary>
        /// 清空当前Cookie信息并跳转
        /// </summary>
        /// <param name="returnUrl"></param>
		public static void LoginOut(string returnUrl)
		{
			HttpContext context = HttpContext.Current;
			if (context == null)
			{
				return;
			}
            string cookieName = "";
            if (CrossDomain)
            {
                cookieName= CrossDomainCookieName;
            }
            FormsAuthentication.SignOut();
            HttpCookie c = context.Request.Cookies[cookieName];
            if (c == null)
            {
                return;
            }
            c.Domain = FormsAuthentication.CookieDomain;
            c.Expires = DateTime.Now.AddYears(-100);
            context.Response.Cookies.Add(c);


            context.Session.Clear();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                context.Response.Redirect(returnUrl);
            }
			return;
		}
	}
}
