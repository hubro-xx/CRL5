using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Core.Session
{
    public class SessionManage
    {
        public static Func<HttpContextBase,AbsSession> __SessionCreater;
        public static AbsSession GetSessionClient(HttpContextBase context)
        {
            if (__SessionCreater == null)
            {
                return new WebSession(context);
            }
            return __SessionCreater(context);
        }
    }
}
