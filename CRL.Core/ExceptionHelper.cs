using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace CRL.Core
{
    public class ExceptionHelper
    {

		public static Exception GetInnerException(Exception exp)
		{
			if (exp.InnerException != null)
			{
				exp = exp.InnerException;
				return GetInnerException(exp);
			}
			return exp;
		}
		public static string WriteException()
		{
            string errorHtml;
            WriteException(HttpContext.Current.Server.GetLastError(), "", out errorHtml);
            return errorHtml;
		}
        static long exceptionId
        {
            get
            {
                return CoreConfig.Instance.LogMsgId;
            }
            set
            {
                CoreConfig.Instance.LogMsgId = value;
            }
        }
		static object lockObj = new object();
		static void WriteException(Exception ero, string errorCode,out string errorHtml)
		{
            errorHtml = "";
            string address = Request.RequestHelper.GetServerIp();
            //本地不作处理
            if (address.Contains("192.168."))
            {
                //return;
            }
			string html = CRL.Core.Properties.Resources.erro;
			HttpContext context = HttpContext.Current;
            html = html.Replace("[charset]",context.Request.ContentEncoding.WebName);
            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";
            context.Response.TrySkipIisCustomErrors = true;
			ero = GetInnerException(ero);
            if (ero != null)
            {
                string erroDetail = ero.StackTrace+"";
                erroDetail = erroDetail.Replace("\r\n", "<br>");
                erroDetail = HttpContext.Current.Server.HtmlEncode(erroDetail);
                html = html.Replace("[TIME]", DateTime.Now.ToString());
                html = html.Replace("[ERRO_CODE]", errorCode);
                html = html.Replace("[URL]", HttpContext.Current.Request.Url.ToString());
                html = html.Replace("[ERRO_TITLE]", HttpContext.Current.Server.HtmlEncode(ero.Message));
                html = html.Replace("[ERRO_MESSAGE]", erroDetail);
                errorHtml = html;
                context.Response.Write(html);
                context.Response.End();
            }
		}
        /// <summary>
        /// 内部记录日志
        /// </summary>
        /// <param name="ero"></param>
        /// <returns></returns>
        public static string InnerLogException(Exception ero)
        {
            var url = HttpContext.Current.Request.Url;
            string host = string.Format("{0}:{1}", url.Host.ToUpper(), url.Port);
            string errorCode = host.Replace(".", "_");
            lock (lockObj)
            {
                exceptionId += 1;
                errorCode += ":" + exceptionId;
            }

            ero = GetInnerException(ero);
            EventLog.LogItem item = new EventLog.LogItem();
            item.Title = "页面发生错误,错误代码:" + errorCode;
            if (ero != null)
            {
                item.Detail = ero.Message + ":" + ero.StackTrace;
            }
            CRL.Core.EventLog.Log(item,"Error");
            if (host == "LOCALHOST")
            {
                return errorCode;
            }
            return errorCode;
        }
        /// <summary>
        /// 页面输出并写入错误日志
        /// </summary>
        /// <param name="ero"></param>
		public static string WriteException(Exception ero)
		{
            
            bool logError = true;
            if (ero is HttpException)
            {
                HttpException he = ero as HttpException;
                int code = he.GetHttpCode();
                if (code == 404)
                {
                    logError = false;
                }
            }
            if (ero is HttpRequestValidationException)
            {
                //HttpContext.Current.Response.Write("请不要输入非法字符");
                //HttpContext.Current.Response.End();
                //return;
                logError = false;
            }

			string errorCode="";
            if (logError)
            {
                errorCode = InnerLogException(ero);
            }
            string errorHtml;
            WriteException(ero, errorCode, out errorHtml);
            return errorHtml;
		}
    }
}
