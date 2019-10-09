using CRL.Core.Extension;
using CRL.Core.Remoting;
using CRL.Core.Request;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Codecs.Http.WebSockets.Extensions.Compression;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CRL.WebSocket
{
    class WebSocketClient : AbsClient
    {
        public WebSocketClient(AbsClientConnect _clientConnect) : base(_clientConnect)
        {

        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var id = Guid.NewGuid().ToString();
            var method = ServiceType.GetMethod(binder.Name);
            var returnType = method.ReturnType;
            var request = new RequestMessage
            {
                Service = ServiceName,
                Method = binder.Name,
                Token = clientConnect.Token
            };
            var dic = new List<object>();
            var allArgs = method.GetParameters();
            var token = request.Token;
            request.Token = GetToken(allArgs, args.ToList(), token);

            for (int i = 0; i < allArgs.Length; i++)
            {
                var p = allArgs[i];
                dic.Add(args[i]);
            }
            request.Args = dic;
            ResponseMessage response = null;
            try
            {
                response = ((WebSocketClientConnect)clientConnect).SendRequest(request);
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
