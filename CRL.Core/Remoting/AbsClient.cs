using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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
            clientConnect.OnError?.Invoke(msg, code);
            throw new RemotingEx(msg) { Code = code };

            //else
            //{
            //    throw new RemotingEx(msg) { Code = code };
            //}
        }
        public virtual void Dispose()
        {

        }
        protected string GetToken(ParameterInfo[] argsName, List<object> args,string token)
        {
            if (clientConnect.__UseSign && !string.IsNullOrEmpty(token))
            {
                var arry = token.Split('@');
                var sign = SignCheck.CreateSign(arry[1], argsName, args);
                token = string.Format("{0}@{1}", arry[0], sign);
            }
            return token;
        }
    }
}
