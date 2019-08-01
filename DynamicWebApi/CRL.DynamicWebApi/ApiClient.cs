using CRL.Core.Request;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.DynamicWebApi
{
    class ApiClient: DynamicObject
    {
        public string Host;
        public string ServiceName;
        public Type ServiceType;
        public string Token;

        ResponseMessage SendRequest(RequestMessage msg)
        {
            var url = Host + $"/DynamicApi/{msg.Service}/{msg.Method}";
            var request = new ImitateWebRequest("orgsync", Encoding.UTF8);
            request.ContentType = "application/json";
            request.SetHead("token", Token);
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
                Token = Token
            };
            var dic = new Dictionary<string, object>();
            var allArgs = method.GetParameters();
            for (int i = 0; i < allArgs.Length; i++)
            {
                dic.Add(allArgs[i].Name, args[i]);
            }
            request.Args = dic;
            var response = SendRequest(request);
            if (response == null)
            {
                throw new Exception("请求超时未响应");
            }
            if (!response.Success)
            {
                throw new Exception($"服务端处理错误：{response.Msg}");
            }
            var returnType = method.ReturnType;
            if (response.Outs != null && response.Outs.Count > 0)
            {
                foreach (var kv in response.Outs)
                {

                }
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
