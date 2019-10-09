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
        /// <summary>
        /// 使用签名
        /// </summary>
        public bool UseSign = false;
        protected Dictionary<string, object> _services = new Dictionary<string, object>();
        public virtual void Dispose()
        {

        }
        public abstract T GetClient<T>() where T : class;
    }
}
