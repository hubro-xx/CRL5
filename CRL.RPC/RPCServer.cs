
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
        #region 构造函数


        public RPCServer(int port)
        {
            _port = port;

            _serverBootstrap = new ServerBootstrap()
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

        #endregion

        #region 私有成员

        private int _port { get; set; }
        private Dictionary<string, object> _serviceHandle { get; set; } = new Dictionary<string, object>();
        ConcurrentDictionary<string, MethodInfo> methods = new ConcurrentDictionary<string, MethodInfo>();
        ServerBootstrap _serverBootstrap { get; }
        IChannel _serverChannel { get; set; }
        internal ResponseMessage GetResponse(RequestMessage request)
        {
            ResponseMessage response = new ResponseMessage();

            try
            {
                var a = _serviceHandle.TryGetValue(request.ServiceName, out object service);
                if (!a)
                {
                    throw new Exception("未找到该服务");
                }
                a = methods.TryGetValue(request.MethodName, out MethodInfo method);
                if (!a)
                {
                    var serviceType = service.GetType();
                    method = serviceType.GetMethod(request.MethodName);
                    if (method == null)
                        throw new Exception("未找到该方法");
                    methods.TryAdd(request.MethodName, method);
                }
                var paramters = request.Paramters.ToArray();

                var result = method.Invoke(service, paramters);
                response.SetData(result);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion

        #region 外部接口

        public void RegisterService<IService, Service>() where Service : class, IService, new() where IService : class
        {
            _serviceHandle.Add(typeof(IService).Name, new Service());
        }

        public void Start()
        {
            _serverChannel = _serverBootstrap.BindAsync(_port).Result;
        }

        public void Stop()
        {
            _serverChannel.CloseAsync();
        }

        #endregion
    }
}
