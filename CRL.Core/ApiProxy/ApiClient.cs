using CRL.Core.Extension;
using CRL.Core.Remoting;
using CRL.Core.Request;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
        object SendRequest(Type returnType, ParameterInfo[] argsName, RequestJsonMessage msg, bool isPost)
        {
            var apiClientConnect = clientConnect as ApiClientConnect;
            var url = Host + $"/{apiClientConnect.Apiprefix}";
            if (!string.IsNullOrEmpty(apiClientConnect.Apiprefix))
            {
                url += "/";
            }
            url+=$"{msg.Service}/{msg.Method}";
            var request = new ImitateWebRequest(Host, apiClientConnect.Encoding);

            string result;
            var firstArgs = msg.Args.FirstOrDefault();
            var members = new SortedDictionary<string, object>();
            #region 提交前参数回调处理
            if (isPost)
            {
                if (firstArgs != null)
                {
                    var pro = firstArgs.GetType().GetProperties();
                    foreach (var p in pro)
                    {
                        members.Add(p.Name.ToLower(), p.GetValue(firstArgs));
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
            apiClientConnect.OnBeforRequest?.Invoke(request, members);
            request.ContentType = apiClientConnect.ContentType;
            if (isPost)
            {
                string data = "";
                if (firstArgs != null)
                {
                    if (request.ContentType == "application/json")
                    {
                        data = firstArgs.ToJson();
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
                var args = msg.Args;
                for (int i = 0; i < argsName.Length; i++)
                {
                    var p = argsName[i];
                    var value = args[i];
                    list.Add(string.Format("{0}={1}", p.Name, value));
                }
                var str = string.Join("&", list);
                result = request.Get($"{url}?{str}");
            }
            if (request.ContentType == "application/json")
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
            var control = ServiceType.GetCustomAttribute<ControlAttribute>();
            var controlName = ServiceName;
            if (control != null)
            {
                controlName = control.Name;
            }
            var method = ServiceType.GetMethod(binder.Name);
            var isPost = method.GetCustomAttribute<HttpGetAttribute>() == null;
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
                response = SendRequest(returnType, methodParamters, request, isPost);
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
