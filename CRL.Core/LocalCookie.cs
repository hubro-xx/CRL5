using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
namespace CRL.Core
{
    /// <summary>
    /// 存取加密的COOKIE健值集合
    /// </summary>
    public class LocalCookie
    {
        /// <summary>
        /// 以默认COOKIE名构造
        /// </summary>
        public LocalCookie()
        {
            cookieName = "slocal" + DateTime.Now.Day;
        }
        /// <summary>
        /// 指定COOKIE名构造
        /// </summary>
        /// <param name="_cookieName"></param>
        public LocalCookie(string _cookieName)
        {
            cookieName = _cookieName;
        }

        string cookieName;
        
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private string GetCoookieValue()
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies[cookieName];
            string encryptStr = "";
            if (cookie != null)
            {
                encryptStr = cookie.Value;
            }
            return encryptStr;
        }
        private void GetData()
        {
            if (data.Count == 0)
            {
                string encryptStr = GetCoookieValue();
                if (encryptStr != "")
                {
                    string userData = CRL.Core.StringHelper.Decrypt(encryptStr.Trim(), "S3HSX6JA");
                    string[] arry = userData.Split('\n');
                    foreach (string str in arry)
                    {
                        if (str != "")
                        {
                            int index = str.IndexOf("=");
                            string k = str.Substring(0, index);
                            string v = str.Substring(index + 1);
                            data.Add(k, v);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAll()
        {
            GetData();
            return data;
        }
        /// <summary>
        /// 根据键名取COOKIE值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                GetData();
                if (data.ContainsKey(key))
                {
                    return data[key];
                }
                return null;
            }
            set
            {
                GetData();
                if (data.ContainsKey(key))
                {
                    data[key] = value;
                }
                else
                {
                    data.Add(key, value);
                }
                Save();
            }
        }
        void Save()
        {
            string str = "";
            foreach (KeyValuePair<string, string> entry in data)
            {
                str += entry.Key + "=" + entry.Value + "\n";
            }
            str = str.Trim();
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(cookieName);
            //cookie.Value = FormsAuthentication.Encrypt(ticket);
            cookie.Value = CRL.Core.StringHelper.Encrypt(str, "S3HSX6JA");
            context.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 设置Cookie有效期
        /// </summary>
        /// <param name="time"></param>
        public void SetExpire(DateTime time)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Cookies[cookieName].Expires = time;
        }
        /// <summary>
        /// 移除一个健值
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            data.Remove(key);
            Save();
        }
        /// <summary>
        /// 清除此COOKIE
        /// </summary>
        public void Clear()
        {
            HttpContext context = HttpContext.Current;
            data.Clear();
            context.Response.Cookies.Remove(cookieName);
        }

    }
}
