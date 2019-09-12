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
            return GetUser(out object tag);
        }
        /// <summary>
        /// 当前用户
        /// </summary>
        /// <param name="tag">自定义数据</param>
        /// <returns></returns>
        protected string GetUser(out object tag)
        {
            var v = ServerCreater.SessionManage.GetSession(__currentUser);
            tag = v.Item2;
            return v.Item1;
        }
        /// <summary>
        /// 保存Session
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="tag"></param>
        protected void SaveSession(string user, string token, object tag = null)
        {
            ServerCreater.SessionManage.SaveSession(user, token, tag);
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
