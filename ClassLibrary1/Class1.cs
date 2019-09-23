using System;

namespace ClassLibrary1
{
    public class Class1
    {
        public void aa()
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
    }

    public class CallContext2
    {
        static System.Collections.Concurrent.ConcurrentDictionary<string, callContextData> caches = new System.Collections.Concurrent.ConcurrentDictionary<string, callContextData>();

        public static T GetData<T>(string contextName)
        {
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var key = $"{threadId}_{contextName}";
            var a = caches.TryGetValue(key, out callContextData v);
            if (a)
            {
                return (T)v.data;
            }
            return default(T);
        }
        public static void SetData(string contextName, object data)
        {
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var key = $"{threadId}_{contextName}";
            caches.TryRemove(key, out callContextData v);
            caches.TryAdd(key, new callContextData() { time = DateTime.Now, data = data });
        }
        class callContextData
        {
            public object data;
            public DateTime time;
        }
    }
}
