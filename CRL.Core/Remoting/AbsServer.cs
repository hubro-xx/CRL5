using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.Remoting
{
    public abstract class AbsServer: IDisposable
    {
        protected static Dictionary<string, AbsService> serviceHandle = new Dictionary<string, AbsService>();
        protected static ConcurrentDictionary<string, MethodInfo> methods = new ConcurrentDictionary<string, MethodInfo>();
        public void Register<IService, Service>() where Service : AbsService, IService, new() where IService : class
        {
            serviceHandle.Add(typeof(IService).Name, new Service());
        }
        protected ISessionManage sessionManage
        {
            get
            {
                return Setting.SessionManage;
            }
        }
        /// <summary>
        /// 自定义session管理
        /// </summary>
        /// <param name="_sessionManage"></param>
        public void SetSessionManage(ISessionManage _sessionManage)
        {
            Setting.SessionManage = _sessionManage;
        }
        public virtual void Start()
        {

        }
        public virtual void Dispose()
        {

        }
        public abstract object InvokeResult(object rq);
    }
}
