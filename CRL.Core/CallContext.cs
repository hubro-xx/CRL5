/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core
{
    /// <summary>
    /// 当前调用上下文
    /// </summary>
    public class CallContext
    {
        //rem 不能使用LogicalGetData,会造自定义线程只有一个实例
        public static T GetData<T>(string contextName)
        {
            //return default(T);
            var dbContextObj = System.Runtime.Remoting.Messaging.CallContext.GetData(contextName);
            if (dbContextObj == null)
                return default(T);
            return (T)dbContextObj;
        }
        public static void SetData(string contextName, object data)
        {
            //return;
            System.Runtime.Remoting.Messaging.CallContext.SetData(contextName, data);
        }
    }
    ///// <summary>
    ///// 自定义CallContext
    ///// </summary>
    //public class CallContext2
    //{
    //    static System.Collections.Concurrent.ConcurrentDictionary<string, callContextData> caches = new System.Collections.Concurrent.ConcurrentDictionary<string, callContextData>();
    //    static ThreadWork work = null;
    //    public static T GetData<T>(string contextName)
    //    {
    //        var threadId = System.Threading.Thread.CurrentThread.ExecutionContext.GetHashCode();
    //        var key = $"{contextName}_{threadId}";
    //        var a = caches.TryGetValue(key, out callContextData v);
    //        if (a)
    //        {
    //            return (T)v.data;
    //        }
    //        return default(T);
    //    }
    //    public static void SetData(string contextName, object data)
    //    {
    //        var threadId = System.Threading.Thread.CurrentThread.ExecutionContext.GetHashCode();
    //        var key = $"{contextName}_{threadId}";
    //        caches.TryRemove(key, out callContextData v);
    //        caches.TryAdd(key, new callContextData() { key = key, time = DateTime.Now, data = data });
    //        if (work == null)
    //        {
    //            work = new ThreadWork();
    //            work.Start("removeCallContext", () =>
    //             {
    //                 var time = DateTime.Now.AddSeconds(-10);
    //                 var find = caches.Values.ToList().FindAll(b => b.time < time);
    //                 foreach (var item in find)
    //                 {
    //                     caches.TryRemove(item.key, out callContextData d);
    //                 }
    //                 return true;
    //             }, 4);
    //        }
    //    }
    //    class callContextData
    //    {
    //        public string key;
    //        public object data;
    //        public DateTime time;
    //    }
    //}
}
