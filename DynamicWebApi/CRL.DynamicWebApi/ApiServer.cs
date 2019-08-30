
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
                var outs = new Dictionary<string,object>();
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
                return ResponseMessage.CreateError("服务端处理错误:" + ex.Message, "500");
            }
 
            return response;
        }
    }
}
