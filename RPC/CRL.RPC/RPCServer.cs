using CRL.Core.Remoting;
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
    public class RPCServer: AbsServer
    {
        int port;
        
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
                    //数据包最大长度
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast(new ServerHandler(this));
                }));
        }


        public override object InvokeResult(object rq)
        {
            var request = rq as RequestMessage;
            var response = new ResponseMessage();

            try
            {
                var a = serviceHandle.TryGetValue(request.Service, out AbsService service);
                if (!a)
                {
                    return ResponseMessage.CreateError("未找到该服务", "404");
                }
                var methodKey = string.Format("{0}.{1}", request.Service, request.Method);
                a = methods.TryGetValue(methodKey, out MethodInfo method);
                var serviceType = service.GetType();
                service = System.Activator.CreateInstance(serviceType) as AbsService;
                if (!a)
                {
                    method = serviceType.GetMethod(request.Method);
                    if (method == null)
                    {
                        return ResponseMessage.CreateError("未找到该方法", "404");
                    }
                    methods.TryAdd(methodKey, method);
                }
                var checkToken = true;
                var allowAnonymous = serviceType.GetCustomAttribute<AllowAnonymousAttribute>();
                var allowAnonymous2 = method.GetCustomAttribute<AllowAnonymousAttribute>();

                var paramters = request.Args;

                var methodParamters = method.GetParameters();

                var args = new object[methodParamters.Length];
                var outIndex = new List<int>();
                int i = 0;
                foreach (var p in methodParamters)
                {
                    var value = paramters[i];
                    int offSet = 0;
                    args[i] = Core.BinaryFormat.FieldFormat.UnPack(p.ParameterType, value, ref offSet);
                    if (p.Attributes == ParameterAttributes.Out)
                    {
                        outIndex.Add(i);
                    }
                    i += 1;
                }

                if (allowAnonymous!=null || allowAnonymous2!=null)
                {
                    checkToken = false;
                }
                var loginAttr = method.GetCustomAttribute<LoginPointAttribute>();
                if (loginAttr != null)
                {
                    checkToken = false;
                }
                if (checkToken)//登录切入点不验证
                {
                    if (string.IsNullOrEmpty(request.Token))
                    {
                        return ResponseMessage.CreateError("请求token为空,请先登录", "401");
                        //throw new Exception("token为空");
                    }
                    var tokenArry = request.Token.Split('@');
                    if (tokenArry.Length < 2)
                    {
                        return ResponseMessage.CreateError("token不合法 user@token", "401");
                        //throw new Exception("token不合法 user@token");
                    }
                    var a2 = sessionManage.CheckSession(tokenArry[0], tokenArry[1], methodParamters, args.ToList(), out string error);
                    if (!a2)
                    {
                        return ResponseMessage.CreateError(error, "401");
                    }
                    service.SetUser(tokenArry[0]);
                }

 

                //var args3 = paramters?.Select(b => b.Value)?.ToArray();
                var result = method.Invoke(service, args);
                var outs = new Dictionary<int, byte[]>();
                foreach (var index in outIndex)
                {
                    var type = methodParamters[index];
                    var value = args[index];
                    outs[index] = Core.BinaryFormat.FieldFormat.Pack(type.ParameterType, value);
                }
                response.SetData(method.ReturnType, result);
                response.Success = true;
                response.Outs = outs;
                if (loginAttr != null)//登录方法后返回新TOKEN
                {
                    response.Token = service.GetToken();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Msg = ex.InnerException?.Message;
                Console.WriteLine(ex.ToString());
                return ResponseMessage.CreateError("服务端处理错误:" + ex.InnerException?.Message, "500");
            }

            return response;
        }

        public override void Start()
        {
            serverChannel = serverBootstrap.BindAsync(port).Result;
            Console.WriteLine("RPCServer start at "+ port);
        }
        public override void Dispose()
        {
            serverChannel.CloseAsync();
        }
    }
}
