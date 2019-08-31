using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Core.Remoting
{
    public abstract class AbsService
    {
        static string __currentUser;
        static string __token;
        public void SetUser(string user)
        {
            __currentUser = user;
        }
        public string GetToken()
        {
            return __token;
        }
        /// <summary>
        /// 当前用户
        /// </summary>
        protected string GetUser()
        {
            return __currentUser;
        }
        /// <summary>
        /// 保存Session
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        protected void SaveSession(string user, string token)
        {
            ServerCreater.SessionManage.SaveSession(user, token);
            __token = string.Format("{0}@{1}", user, token);
        }

        /// <summary>
        /// 获取发送的文件
        /// </summary>
        /// <returns></returns>
        protected HttpPostedFile GetPostFile()
        {
            return Core.CallContext.GetData<HttpPostedFile>("postFile");
        }
    }
}
