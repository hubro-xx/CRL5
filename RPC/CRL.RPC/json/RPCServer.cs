
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CRL.RPC
{
    /// <summary>
    /// RPC服务端
    /// </summary>
    public class RPCServer: IDisposable
    {
        int port;
        Dictionary<string, object> serviceHandle = new Dictionary<string, object>();
        ConcurrentDictionary<string, MethodInfo> methods = new ConcurrentDictionary<string, MethodInfo>();
        ServerBootstrap serverBootstrap;
        IChannel serverChannel { get; set; }
        public RPCServer(int _port)
        {
            port = _port;

            serverBootstrap = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup(), new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast(new ServerHandler(this));
                }));
        }




        internal ResponseMessage InvokeResult(RequestMessage request)
        {
            var response = new ResponseMessage();

            try
            {
                var a = serviceHandle.TryGetValue(request.Service, out object service);
                if (!a)
                {
                    throw new Exception("未找到该服务");
                }
                if (tokenCheck != null)
                {
                    var tokenArry = request.Token.Split('@');
                    if (tokenArry.Length < 2)
                    {
                        throw new Exception("token不合法");
                    }
                    if (!tokenCheck(tokenArry[0], tokenArry[1]))
                    {
                        throw new Exception("token验证失败");
                    }
                }
                var methodKey = string.Format("{0}.{1}", request.Service, request.Method);
                a = methods.TryGetValue(methodKey, out MethodInfo method);
                if (!a)
                {
                    var serviceType = service.GetType();
                    method = serviceType.GetMethod(request.Method);
                    if (method == null)
                        throw new Exception("未找到该方法");
                    methods.TryAdd(methodKey, method);
                }
                var paramters = request.Args;
                var methodParamters = method.GetParameters();
                var outs = new Dictionary<string, object>();
                int i = 0;
                foreach (var p in methodParamters)
                {
                    var find = paramters.TryGetValue(p.Name, out object value);
                    if (find && value != null)
                    {
                        if (value.GetType() != p.ParameterType)
                        {
                            var value2 = value.ToJson().ToObject(p.ParameterType);
                            paramters[p.Name] = value2;
                        }
                    }
                    else
                    {
                        paramters[p.Name] = null;
                    }
                    if (p.Attributes == ParameterAttributes.Out)
                    {
                        outs.Add(p.Name, i);
                    }
                    i += 1;
                }
                var args3 = paramters?.Select(b => b.Value)?.ToArray();
                var result = method.Invoke(service, args3);
                foreach (var kv in new Dictionary<string, object>(outs))
                {
                    var value = args3[(int)kv.Value];
                    outs[kv.Key] = value;
                }
                response.SetData(result);
                response.Success = true;
                response.Outs = outs;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Msg = ex.Message;
                Console.WriteLine(ex.ToString());
            }

            return response;
        }



        public void Register<IService, Service>() where Service : class, IService, new() where IService : class
        {
            serviceHandle.Add(typeof(IService).Name, new Service());
        }
        Func<string, string, bool> tokenCheck;
        public void SetTokenCheck(Func<string, string, bool> _tokenCheck)
        {
            tokenCheck = _tokenCheck;
        }

        public void Start()
        {
            serverChannel = serverBootstrap.BindAsync(port).Result;
            Console.WriteLine("RPCServer start at "+ port);
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            serverChannel.CloseAsync();
        }
    }
}
