using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.DynamicWebApi
{
    public class DynamicModule : IHttpModule
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (s, e) =>
            {
                var app = s as HttpApplication;
                var path = app.Request.FilePath;
                if(!path.StartsWith("/DynamicApi/"))
                {
                    return;
                }
                var arry = path.Split('/');
                var service = arry[2];
                var method = arry[3];
                var serviceHandle = ApiServer.serviceHandle;
                var token = app.Request.Headers["token"];

                var request = new RequestMessage()
                {
                    Service = service,
                    Method = method,
                    Token = token,
                };
                if (app.Request.ContentLength > 0)
                {
                    var ms = app.Request.InputStream;
                    var data = new byte[app.Request.ContentLength];
                    ms.Read(data, 0, data.Length);
                    var args = System.Text.Encoding.UTF8.GetString(data);
                    request.Args = args.ToObject<Dictionary<string, object>>();
                }
                var result = ApiServer.InvokeResult(request);
                app.Response.ContentType = "application/json";
                app.Response.Write(result.ToJson());
                app.Response.End();
            };
        }
    }
}