
using ImpromptuInterface;
using System.Collections.Concurrent;

namespace CRL.RPC
{

    public class RPCClientFactory
    {
        static ConcurrentDictionary<string, object> _services { get; } = new ConcurrentDictionary<string, object>();
        public static T GetClient<T>(string host, int port) where T : class
        {
            var serviceName = typeof(T).Name;
            var key = string.Format("{0}_{1}_{2}", host, port, serviceName);
            var a = _services.TryGetValue(key, out object instance);
            if(a)
            {
                return instance as T;
            }
            var client = new RPCClient
            {
                Host = host,
                Port = port,
                ServiceType = typeof(T),
                ServiceName = serviceName
            };
            //创建代理
            instance = client.ActLike<T>();
            _services[key] = instance;
            return instance as T;
        }
    }
}
