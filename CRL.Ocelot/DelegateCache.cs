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
        class cacheItem
        {
            public object data;
            public DateTime expTime;
        }
        static object lockObj = new object();
        static System.Collections.Concurrent.ConcurrentDictionary<string, cacheItem> cache = new System.Collections.Concurrent.ConcurrentDictionary<string, cacheItem>();
        /// <summary>
        /// 初始缓存信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="minute">过期时间,单位分</param>
        /// <param name="handler">委托</param>
        public static T Init<T>(string key, double minute, GetDataHandler<T> handler)
        {
            key = "DelegateCache&" + key;
            var a = cache.TryGetValue(key, out cacheItem cacheObj);
            if (a && cacheObj.expTime > DateTime.Now)
            {
                return (T)cacheObj.data;
            }
            lock (lockObj)
            {
                cacheObj = new cacheItem() { data = handler(), expTime = DateTime.Now.AddMinutes(minute) };
                if (cacheObj == null)
                {
                    return default(T);
                }
                cache.TryAdd(key, cacheObj);
            }
            return (T)cacheObj.data;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            key = "DelegateCache&" + key;
            cache.Remove(key, out cacheItem v);
        }
    }
}
