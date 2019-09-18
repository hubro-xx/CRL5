
using CRL.Core.Extension;
using CRL.Core.Remoting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.DynamicWebApi
{
    public class ApiServer: AbsServer
    {
        static ApiServer()
        {
            instance = new ApiServer();
        }
        static ApiServer instance;
        internal static ApiServer Instance
        {
            get
            {
                return instance;
            }
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
                var serviceType = service.GetType();
                service = System.Activator.CreateInstance(serviceType) as AbsService;
                var methodKey = string.Format("{0}.{1}", request.Service, request.Method);
                a = methods.TryGetValue(methodKey, out MethodInfo method);
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
                if (allowAnonymous != null || allowAnonymous2 != null)
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
                    var a2 = sessionManage.CheckSession(tokenArry[0], tokenArry[1], out string error);
                    if (!a2)
                    {
                        return ResponseMessage.CreateError(error, "401");
                    }
                    //Core.CallContext.SetData("currentUser", tokenArry[0]);
                    service.SetUser(tokenArry[0]);
                }
                var paramters = request.Args;
                var methodParamters = method.GetParameters();
                if(request.Args.Count!= methodParamters.Count())
                {
                    return ResponseMessage.CreateError("参数计数不正确" + request.ToJson(), "500");
                }
                var outs = new Dictionary<int,object>();
                int i = 0;
                foreach (var p in methodParamters)
                {
                    var value = paramters[i];
                    if (value != null)
                    {
                        if (value.GetType() != p.ParameterType)
                        {
                            var value2 = value.ToJson().ToObject(p.ParameterType);
                            paramters[i] = value2;
                        }
                    }
                    else
                    {
                        paramters[i] = value;
                    }
                    if (p.Attributes == ParameterAttributes.Out)
                    {
                        outs.Add(i,null);
                    }
                    i += 1;
                }
                var args3 = paramters?.ToArray();
                var result = method.Invoke(service, args3);
                foreach (var kv in new Dictionary<int, object>(outs))
                {
                    var value = args3[kv.Key];
                    outs[kv.Key] = value;
                }
                response.SetData(result);
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
                response.Msg = ex.Message;
                Console.WriteLine(ex.ToString());
                CRL.Core.EventLog.Log(ex.ToString(), request.Service);
                return ResponseMessage.CreateError(ex.Message, "500");
            }
 
            return response;
        }
    }
}
