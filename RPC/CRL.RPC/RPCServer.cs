
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace CRL.RPC
{
    /// <summary>
    /// RPC服务端
    /// </summary>
    public class RPCServer
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
                var paramters = request.Args.ToArray();
                var methodParamters = method.GetParameters();
                //格式化参数
                for (int i = 0; i < methodParamters.Length; i++)
                {
                    var type = methodParamters[i].ParameterType;
                    if (paramters[i].GetType() != type)
                    {
                        paramters[i] = paramters[i].ToJson().ToObject(type);
                    }
                }
                var result = method.Invoke(service, paramters);
                response.SetData(result);
                response.Success = true;
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

        public void Start()
        {
            serverChannel = serverBootstrap.BindAsync(port).Result;
            Console.WriteLine("RPCServer start at "+ port);
        }

        public void Stop()
        {
            serverChannel.CloseAsync();
        }

    }
}
