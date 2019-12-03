using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.Remoting
{
    public abstract class AbsClientConnect : IDisposable
    {
        public string Token = "";
        public Action<string, string> OnError;

        internal bool __UseSign = false;
        /// <summary>
        /// 使用签名
        /// 参数都会以ToString计算,注意类型问题
        /// </summary>
        public void UseSign()
        {
            __UseSign = true;
        }
        protected Dictionary<string, object> _services = new Dictionary<string, object>();
        public virtual void Dispose()
        {

        }
        public abstract T GetClient<T>() where T : class;
        #region consul

        internal Func<HostAddress> __GetConsulAgent;
        /// <summary>
        /// 使用consul服务发现
        /// </summary>
        /// <param name="consulUrl"></param>
        /// <param name="serviceName"></param>
        public void UseConsulAgent(string consulUrl,string serviceName)
        {
            var consulClient = new ConsulClient.Consul(consulUrl);
            //发现consul服务注册,返回服务地址
            __GetConsulAgent = () =>
            {
                var serviceInfo = consulClient.GetServiceInfo(serviceName, 0.5);
                return new HostAddress() { address = serviceInfo.Address, port = serviceInfo.Port };
            };

        }
        #endregion
    }
}
