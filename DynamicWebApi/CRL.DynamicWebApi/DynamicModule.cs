using CRL.Core.Extension;
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
                var application = s as HttpApplication;
                var request = application.Request;
                var response = application.Response;
                var path = request.FilePath;
                if(!path.StartsWith("/DynamicApi/"))
                {
                    return;
                }
                var arry = path.Split('/');
                var service = arry[2];
                var method = arry[3];
                //var serviceHandle = ApiServer.serviceHandle;
                var token = request.Headers["token"];
                if (request.Files != null && request.Files.Count > 0)
                {
                    var file = request.Files[0];
                    Core.CallContext.SetData("postFile", file);
                }
                var requestMsg = new RequestMessage()
                {
                    Service = service,
                    Method = method,
                    Token = token,
                };
                if (request.ContentLength > 0)
                {
                    var ms = request.InputStream;
                    var data = new byte[request.ContentLength];
                    ms.Read(data, 0, data.Length);
                    var args = System.Text.Encoding.UTF8.GetString(data);
                    requestMsg.Args = args.ToObject<Dictionary<string, object>>();
                }
                var result = ApiServer.InvokeResult(requestMsg);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.ContentType = "application/json";
                response.Write(result.ToJson());
                response.End();
            };
        }
    }
}