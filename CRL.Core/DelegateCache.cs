using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Core
{
    public delegate T GetDataHandler<T>();
    /// <summary>
    /// 自定义委托和过期时间,实现缓存
    /// </summary>
    public class DelegateCache
    {
        static object lockObj = new object();
        /// <summary>
        /// 初始缓存信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="minute">过期时间,单位分</param>
        /// <param name="handler">委托</param>
        public static T Init<T>(string key, double minute, GetDataHandler<T> handler)
        {
            key = "DelegateCache&" + key;
            var cache = System.Web.HttpRuntime.Cache;
            var cacheObj = cache.Get(key);
            if (cacheObj != null)
            {
                return (T)cacheObj;
            }
            lock (lockObj)
            {
                cacheObj = handler();
                cache.Insert(key, cacheObj, null, DateTime.Now.AddMinutes(minute), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return (T)cacheObj;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            key = "DelegateCache&" + key;
            var cache = System.Web.HttpRuntime.Cache;
            cache.Remove(key);
        }
    }
}
