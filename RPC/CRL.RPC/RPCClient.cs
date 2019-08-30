
using CRL.Core.Remoting;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
namespace CRL.RPC
{
    class RPCClient : AbsClient
    {
        public string Host;
        public int Port;
        public string ServiceName;
        public Type ServiceType;
        static Bootstrap bootstrap;

        internal RPCClientConnect RPCClientConnect;
        IChannel channel = null;

        static ResponseWaits allWaits = new ResponseWaits();
        static RPCClient()
        {
            bootstrap = new Bootstrap()
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast(new ClientHandler(allWaits));
                }));
        }
 

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                if (channel == null || !channel.Open)
                {
                    channel = Core.AsyncInvoke.RunSync(() => bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port)));
                }
            }
            catch(Exception ero)
            {
                throw new Exception("连接服务端失败:" + ero);
            }
            var id = Guid.NewGuid().ToString();
            var method = ServiceType.GetMethod(binder.Name);
            allWaits.Add(id);
            var request = new RequestMessage
            {
                MsgId = id,
                Service = ServiceName,
                Method = binder.Name,
                Token = RPCClientConnect.Token
            };
            var dic = new Dictionary<string, byte[]>();
            var allArgs = method.GetParameters();
            var outs = new Dictionary<string, object>();
            for (int i = 0; i < allArgs.Length; i++)
            {
                var p = allArgs[i];
                //dic.Add(p.Name, args[i]);
                dic.Add(p.Name, Core.BinaryFormat.FieldFormat.Pack(p.ParameterType, args[i]));
                if (p.Attributes == ParameterAttributes.Out)
                {
                    outs.Add(p.Name, i);
                }
            }
            request.Args = dic;
            channel.WriteAndFlushAsync(request.ToBuffer());
            //等待返回
            var response = allWaits.Wait(id).Response;
            if (response == null)
            {
                ShowError("请求超时未响应", "500");
            }
            if (!response.Success)
            {
                ShowError($"服务端处理错误：{response.Msg}", response.GetData(typeof(string)) + "");
            }
            var returnType = method.ReturnType;
            if (response.Outs != null && response.Outs.Count > 0)
            {
                foreach (var kv in response.Outs)
                {
                    var find = (int)outs[kv.Key];
                    var type = allArgs[find];
                    //args[(int)find] = kv.Value;
                    int offSet = 0;
                    args[find] = Core.BinaryFormat.FieldFormat.UnPack(type.ParameterType, kv.Value, ref offSet);
                }
            }
            if (!string.IsNullOrEmpty(response.Token))
            {
                RPCClientConnect.Token = response.Token;
            }
            if (returnType == typeof(void))
            {
                result = null;
                return true;
            }
            result = response.GetData(returnType);
            return true;
        }
        protected override void ShowError(string msg, string code)
        {
            if (RPCClientConnect.OnError != null)
            {
                RPCClientConnect.OnError(msg, code);
            }
            else
            {
                throw new Exception(msg);
            }
        }
        public override void Dispose()
        {
            if (channel != null)
            {
                channel.CloseAsync();
            }
        }
    }
}
