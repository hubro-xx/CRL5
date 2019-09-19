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
        Bootstrap bootstrap;
        IChannel channel;
        IEventLoopGroup group;
        static ResponseWaits allWaits = new ResponseWaits();
        public int Port;
        public WebSocketClient(AbsClientConnect _clientConnect,string host,int port) : base(_clientConnect)
        {
            Host = host;
            Port = port;
            Connect();
        }
        public override void Dispose()
        {
            channel.CloseAsync().Wait();
            group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)).Wait();
        }
        void Connect()
        {
            var builder = new UriBuilder
            {
                Scheme = "ws",
                Host = Host,
                Port = Port
            };

            string path = "websocket";
            if (!string.IsNullOrEmpty(path))
            {
                builder.Path = path;
            }

            Uri uri = builder.Uri;
 
            //Console.WriteLine("Transport type : " + (useLibuv ? "Libuv" : "Socket"));

            IEventLoopGroup group;
            group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            //if (ClientSettings.IsSsl)
            //{
            //    cert = new X509Certificate2(Path.Combine(ExampleHelper.ProcessDirectory, "dotnetty.com.pfx"), "password");
            //    targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            //}
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Option(ChannelOption.TcpNodelay, true);
            bootstrap.Channel<TcpSocketChannel>();

            // Connect with V13 (RFC 6455 aka HyBi-17). You can change it to V08 or V00.
            // If you change it to V00, ping is not supported and remember to change
            // HttpResponseDecoder to WebSocketHttpResponseDecoder in the pipeline.
            var handler = new WebSocketClientHandler(
                WebSocketClientHandshakerFactory.NewHandshaker(
                        uri, WebSocketVersion.V13, null, true, new DefaultHttpHeaders()), allWaits);

            bootstrap.Handler(new ActionChannelInitializer<IChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                //if (cert != null)
                //{
                //    pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                //}

                pipeline.AddLast(
                    new HttpClientCodec(),
                    new HttpObjectAggregator(8192),
                    WebSocketClientCompressionHandler.Instance,
                    handler);
            }));

            channel = bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port)).Result;
            handler.HandshakeCompletion.Wait();

            Console.WriteLine("WebSocket handshake completed.");
        }
        ResponseMessage SendRequest(RequestMessage msg)
        {
            var id = Guid.NewGuid().ToString();
            allWaits.Add(id);
            msg.MsgId = id;
            WebSocketFrame frame = new TextWebSocketFrame(msg.ToBuffer());
            if(!channel.Active)
            {
                ThrowError("服务端已断开连接", "500");

            }
            channel.WriteAndFlushAsync(frame);
        
            //等待返回
            var response = allWaits.Wait(id).Response;
            return response;
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

            for (int i = 0; i < allArgs.Length; i++)
            {
                var p = allArgs[i];
                dic.Add(args[i]);
            }
            request.Args = dic;
            ResponseMessage response = null;
            try
            {
                response = SendRequest(request);
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
