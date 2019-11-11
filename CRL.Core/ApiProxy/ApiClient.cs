using CRL.Core.Extension;
using CRL.Core.Remoting;
using CRL.Core.Request;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.ApiProxy
{
    class ApiClient: AbsClient
    {
        public ApiClient(AbsClientConnect _clientConnect) : base(_clientConnect)
        {

        }
        object SendRequest(Type returnType, ParameterInfo[] argsName, RequestJsonMessage msg, MethodAttribute methodAttribute)
        {
            var apiClientConnect = clientConnect as ApiClientConnect;
            var httpMethod = HttpMethod.POST;
            var requestPath = $"/{apiClientConnect.Apiprefix}/{msg.Service}/{msg.Method}";
            if (methodAttribute != null)
            {
                httpMethod = methodAttribute.Method;
                if(!string.IsNullOrEmpty(methodAttribute.Path))
                {
                    requestPath = methodAttribute.Path;
                    if (!requestPath.StartsWith("/"))
                    {
                        requestPath = "/" + requestPath;
                    }
                }
            }

            var url = Host + requestPath;
            var request = new ImitateWebRequest(ServiceName, apiClientConnect.Encoding);
            string result;
            var firstArgs = msg.Args.FirstOrDefault();
            var members = new Dictionary<string, object>();
            #region 提交前参数回调处理
            if (httpMethod== HttpMethod.POST&& msg.Args.Count==1)//只有一个参数的POST
            {
                var type = firstArgs.GetType();
                var pro = type.GetProperties();
                if (firstArgs is System.Collections.IDictionary)
                {
                    var dic = firstArgs as System.Collections.IDictionary;
                    foreach (string key in dic.Keys)
                    {
                        members.Add(key,dic[key]);
                    }
                }
                else
                {
                    foreach (var p in pro)
                    {
                        members.Add(p.Name, p.GetValue(firstArgs));
                    }
                }
            }
            else
            {
                var args = msg.Args;
                for (int i = 0; i < argsName.Length; i++)
                {
                    var p = argsName[i];
                    var value = args[i];
                    members.Add(p.Name, value);
                }
            }
            #endregion
            try
            {
                apiClientConnect.OnBeforRequest?.Invoke(request, members);
            }
            catch(Exception ero)
            {
                throw new Exception("设置请求头信息时发生错误:" + ero.Message);
            }
            request.ContentType = apiClientConnect.ContentType;
            if (httpMethod == HttpMethod.POST)
            {
                string data = "";
                if (firstArgs != null)
                {
                    if (apiClientConnect.ContentType == "application/json")
                    {
                        data = members.ToJson();
                    }
                    else
                    {
                        data = Core.SerializeHelper.XmlSerialize(firstArgs, apiClientConnect.Encoding);
                    }
                }
                result = request.Post(url, data);
            }
            else
            {
                var list = new List<string>();
                foreach (var kv in members)
                {
                    list.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                }
                var str = string.Join("&", list);
                result = request.Get($"{url}?{str}");
            }
            if (apiClientConnect.ContentType == "application/json")
            {
                return SerializeHelper.DeserializeFromJson(result, returnType);
            }
            else
            {
                return SerializeHelper.XmlDeserialize(returnType, result, apiClientConnect.Encoding);
            }
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var controlName = ServiceName;
            var method = ServiceType.GetMethod(binder.Name);
            var methodAttribute = method.GetCustomAttribute<MethodAttribute>();
            var methodParamters = method.GetParameters();
            var returnType = method.ReturnType;
            var request = new RequestJsonMessage
            {
                Service = controlName,
                Method = binder.Name,
                Token = clientConnect.Token
            };
            request.Args = args.ToList();
            object response = null;
            try
            {
                response = SendRequest(returnType, methodParamters, request, methodAttribute);
            }
            catch (Exception ero)
            {
                ThrowError(ero.Message, "500");
            }
            if (returnType == typeof(void))
            {
                result = null;
                return true;
            }
            result = response;

            return true;

        }
    }
}
