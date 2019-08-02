using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.DynamicWebApi
{
    public class ApiServer
    {
        internal static Dictionary<string, object> serviceHandle = new Dictionary<string, object>();
        static ConcurrentDictionary<string, MethodInfo> methods = new ConcurrentDictionary<string, MethodInfo>();
        public void Register<IService, Service>() where Service : class, IService, new() where IService : class
        {
            serviceHandle.Add(typeof(IService).Name, new Service());
        }
        //static Func<string, string, bool> tokenCheck;
        //public void SetTokenCheck(Func<string, string, bool> _tokenCheck)
        //{
        //    tokenCheck = _tokenCheck;
        //}
        internal static ResponseMessage InvokeResult(RequestMessage request)
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
                var loginAttr = method.GetCustomAttribute<LoginPointAttribute>();
                if (loginAttr == null)//登录切入点不验证
                {
                    if (string.IsNullOrEmpty(request.Token))
                    {
                        throw new Exception("token为空");
                    }
                    var tokenArry = request.Token.Split('@');
                    if (tokenArry.Length < 2)
                    {
                        throw new Exception("token不合法 user@token");
                    }
                    SessionManage.CheckSession(tokenArry[0], tokenArry[1]);
                    Core.CallContext.SetData("currentUser", tokenArry[0]);
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
                    response.Token = Core.CallContext.GetData<string>("newToken");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Msg = ex.Message;
                Console.WriteLine(ex.ToString());
            }
 
            return response;
        }

    }
}
