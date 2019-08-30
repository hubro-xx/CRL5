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
        protected abstract void ShowError(string msg, string code);
        public abstract void Dispose();
    }
}
