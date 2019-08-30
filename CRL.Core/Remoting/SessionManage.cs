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
        void SaveSession(string user, string token);
        bool CheckSession(string user, string token, out string error);
    }
    public class SessionManage: ISessionManage
    {
        static ConcurrentDictionary<string, string> sessions = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 登录后返回新的TOKEN
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        public void SaveSession(string user, string token)
        {
            if (!sessions.TryGetValue(user, out string token2))
            {
                sessions.TryAdd(user, token);
            }
            else
            {
                sessions[user] = token;
            }
        }

        public bool CheckSession(string user, string token, out string error)
        {
            error = "";
            var exists = sessions.TryGetValue(user, out string v);
            if (!exists)
            {
                error = "API未登录";
                return false;
            }
            if (token != v)
            {
                error = "token验证失败";
                return false;
            }
            return true;
        }

    }
}
