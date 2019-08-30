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
        public ApiClient(AbsClientConnect _clientConnect):base(_clientConnect)
        {

        }
        ResponseMessage SendRequest(RequestMessage msg)
        {
            var url = Host + $"/DynamicApi/{msg.Service}/{msg.Method}";
            var request = new ImitateWebRequest("orgsync", Encoding.UTF8);
            request.ContentType = "application/json";
            request.SetHead("token", clientConnect.Token);
            var result = request.Post(url, msg.Args.ToJson());
            return result.ToObject<ResponseMessage>();
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var id = Guid.NewGuid().ToString();
            var method = ServiceType.GetMethod(binder.Name);
            var request = new RequestMessage
            {
                Service = ServiceName,
                Method = binder.Name,
                Token = clientConnect.Token
            };
            var dic = new Dictionary<string, object>();
            var allArgs = method.GetParameters();
            var outs = new Dictionary<string, object>();
            for (int i = 0; i < allArgs.Length; i++)
            {
                var p = allArgs[i];
                dic.Add(p.Name, args[i]);
                if (p.Attributes == ParameterAttributes.Out)
                {
                    outs.Add(p.Name, i);
                }
            }
            request.Args = dic;
            try
            {
                var response = SendRequest(request);
                if (response == null)
                {
                    ShowError("请求超时未响应", "500");
                }
                if (!response.Success)
                {
                    ShowError($"服务端处理错误：{response.Msg}", response.Data);
                }
                var returnType = method.ReturnType;
                if (response.Outs != null && response.Outs.Count > 0)
                {
                    foreach (var kv in response.Outs)
                    {
                        var find = outs[kv.Key];
                        args[(int)find] = kv.Value;
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
            catch( Exception ero)
            {
                ShowError(ero.Message, "401");
                result = null;
                return true;
            }
        }
    }
}
