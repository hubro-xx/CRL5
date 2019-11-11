using CRL.Core.Remoting;
using CRL.Core.Request;
using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.ApiProxy
{
    public class ApiClientConnect: AbsClientConnect
    {
        string host;
        //internal Dictionary<string, string> heads = new Dictionary<string, string>();

        internal Action<ImitateWebRequest, Dictionary<string, object>> OnBeforRequest;
        internal string Apiprefix = "api";
        internal Encoding Encoding = Encoding.UTF8;
        internal string ContentType = "application/json";
        /// <summary>
        /// 使用xml发送
        /// </summary>
        /// <returns></returns>
        public ApiClientConnect UseXmlContentType()
        {
            ContentType = "application/xml";
            return this;
        }
        public ApiClientConnect UseFormContentType()
        {
            ContentType = "application/x-www-form-urlencoded";
            return this;
        }
        /// <summary>
        /// 发送前处理
        /// </summary>
        public ApiClientConnect UseBeforRequest(Action<ImitateWebRequest, Dictionary<string, object>> action)
        {
            OnBeforRequest = action;
            return this;
        }

        public ApiClientConnect(string _host)
        {
            host = _host;
        }
        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public ApiClientConnect SetEncoding(Encoding encoding)
        {
            Encoding = encoding;
            return this;
        }
        ///// <summary>
        ///// 设置头
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void SetHead(string name, string value)
        //{
        //    if (!heads.ContainsKey(name))
        //    {
        //        heads.Add(name, value);
        //    }
        //    else
        //    {
        //        heads[name] = value;
        //    }
        //}

        public override T GetClient<T>()
        {
            var serviceName = typeof(T).Name;
            var key = string.Format("{0}_{1}", host, serviceName);
            var a = _services.TryGetValue(key, out object instance);
            if (a)
            {
                return instance as T;
            }
            var client = new ApiClient(this)
            {
                Host = host,
                ServiceType = typeof(T),
                ServiceName = serviceName,
                //Token = string.Format("{0}@{1}", user, token)
            };
            //创建代理
            instance = client.ActLike<T>();
            _services[key] = instance;
            return instance as T;
        }
    }
}
