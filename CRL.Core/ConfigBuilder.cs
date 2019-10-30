using CRL.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Core
{
    public class ConfigBuilder
    {
        public ConfigBuilder()
        {
            current = this;
        }
        internal static ConfigBuilder current;
        public  Func<HttpContextBase, AbsSession> __SessionCreater;
    }
}
