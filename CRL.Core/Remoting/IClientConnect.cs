using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.Remoting
{
    public interface IClientConnect : IDisposable
    {
        T GetClient<T>() where T : class;
    }
}
