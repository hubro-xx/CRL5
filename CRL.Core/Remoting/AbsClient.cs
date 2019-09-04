using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.Remoting
{
    public abstract class AbsClient: DynamicObject, IDisposable
    {
        public string Host;
        public string ServiceName;
        public Type ServiceType;
        public AbsClient(AbsClientConnect _clientConnect)
        {
            clientConnect = _clientConnect;
        }
        protected AbsClientConnect clientConnect;
        protected void ThrowError(string msg, string code)
        {
            throw new RemotingEx(msg) { Code = code };
            //if (clientConnect.OnError != null)
            //{
            //    clientConnect.OnError(msg, code);
            //}
            //else
            //{
            //    throw new RemotingEx(msg) { Code = code };
            //}
        }
        public virtual void Dispose()
        {

        }
    }
}
