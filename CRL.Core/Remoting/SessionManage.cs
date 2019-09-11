using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Core.Remoting
{
    public interface ISessionManage
    {
        void SaveSession(string user, string token, object tag = null);
        bool CheckSession(string user, string token, out string error);
        Tuple<string, object> GetSession(string user);
    }
    public class SessionManage : ISessionManage
    {
        static ConcurrentDictionary<string, Tuple<string, object>> sessions = new ConcurrentDictionary<string, Tuple<string, object>>();
        /// <summary>
        /// 登录后返回新的TOKEN
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="tag"></param>
        public void SaveSession(string user, string token, object tag = null)
        {
            if (!sessions.TryGetValue(user, out Tuple<string, object> token2))
            {
                sessions.TryAdd(user, new Tuple<string, object>(token, tag));
            }
            else
            {
                sessions[user] = new Tuple<string, object>(token, tag);
            }
        }

        public bool CheckSession(string user, string token, out string error)
        {
            error = "";
            var exists = sessions.TryGetValue(user, out Tuple<string, object> v);
            if (!exists)
            {
                error = "API未登录";
                return false;
            }
            if (token != v.Item1)
            {
                error = "token验证失败";
                return false;
            }
            return true;
        }

        public Tuple<string, object> GetSession(string user)
        {
            sessions.TryGetValue(user, out Tuple<string, object> v);
            return v;
        }
    }
}
