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
        public static bool UseRedis = false;
        public static AbsSession GetSessionClient(HttpContextBase context)
        {
            if (UseRedis)
            {
                return new RedisSession(context);
            }
            return new WebSession(context);
        }
    }
}
