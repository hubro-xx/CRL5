using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace RedisProvider
{
    public class SessionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 每次请求都续期
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["redisRefresh"];
            DateTime lastTime;
            if (cookie == null)
            {
                filterContext.HttpContext.Response.Cookies.Add(new HttpCookie("redisRefresh", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                lastTime = DateTime.Now;
            }
            else
            {
                lastTime = Convert.ToDateTime(cookie.Value);
            }
            var ts = DateTime.Now - lastTime;
            if (ts.TotalMinutes > 5)
            {
                SessionManage.GetSessionClient(filterContext.HttpContext).Refresh();
                cookie.Value = DateTime.Now.ToString();
                filterContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }
    }
}
