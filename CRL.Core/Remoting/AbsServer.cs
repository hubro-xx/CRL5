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
        internal void Register<IService, Service>() where Service : AbsService, IService, new() where IService : class
        {
            serviceHandle.Add(typeof(IService).Name, new Service());
        }
        protected ISessionManage sessionManage
        {
            get
            {
                return ServerCreater.SessionManage;
            }
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
