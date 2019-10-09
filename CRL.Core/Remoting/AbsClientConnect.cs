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
    }
}
