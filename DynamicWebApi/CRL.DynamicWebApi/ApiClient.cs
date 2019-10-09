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

namespace CRL.DynamicWebApi
{
    class ApiClient: AbsClient
    {
        public ApiClient(AbsClientConnect _clientConnect) : base(_clientConnect)
        {

        }
        ResponseMessage SendRequest(ParameterInfo[] argsName, RequestMessage msg)
        {
            var url = Host + $"/DynamicApi/{msg.Service}/{msg.Method}";
            var request = new ImitateWebRequest("orgsync", Encoding.UTF8);
            request.ContentType = "application/json";
            var token = clientConnect.Token;
            token = GetToken(argsName, msg.Args, token);

            request.SetHead("token", token);
            var result = request.Post(url, msg.Args.ToJson());
            return result.ToObject<ResponseMessage>();
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var id = Guid.NewGuid().ToString();
            var method = ServiceType.GetMethod(binder.Name);
            var methodParamters = method.GetParameters();
            var returnType = method.ReturnType;
            var request = new RequestMessage
            {
                Service = ServiceName,
                Method = binder.Name,
                Token = clientConnect.Token
            };
            var dic = new List<object>();
            var allArgs = method.GetParameters();
            //var outs = new Dictionary<int, object>();
            for (int i = 0; i < allArgs.Length; i++)
            {
                var p = allArgs[i];
                dic.Add(args[i]);
                if (p.Attributes == ParameterAttributes.Out)
                {
                    //outs.Add(i, null);
                }
            }
            request.Args = dic;
            ResponseMessage response = null;
            try
            {
                response = SendRequest(methodParamters, request);
            }
            catch (Exception ero)
            {
                ThrowError(ero.Message, "500");
            }
            if (response == null)
            {
                ThrowError("请求超时未响应", "500");
            }
            if (!response.Success)
            {
                ThrowError($"服务端处理错误：{response.Msg}", response.Data);
            }
            if (response.Outs != null && response.Outs.Count > 0)
            {
                foreach (var kv in response.Outs)
                {
                    var p = allArgs[kv.Key];
                    //var obj = kv.Value.ToString().ToObject(p.ParameterType);
                    args[kv.Key] = kv.Value;
                }
            }
            if (!string.IsNullOrEmpty(response.Token))
            {
                clientConnect.Token = response.Token;
            }
            if (returnType == typeof(void))
            {
                result = null;
                return true;
            }
            result = response.GetData(returnType);

            return true;

        }
    }
}
