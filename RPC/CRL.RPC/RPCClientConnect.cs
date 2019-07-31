
using ImpromptuInterface;
using System;
using System.Collections.Generic;

namespace CRL.RPC
{

    public class RPCClientConnect: IDisposable
    {
        string host;
        int port;
        public RPCClientConnect(string _host, int _port)
        {
            host = _host;
            port = _port;
        }
        string user, token;
        public void SetToken(string _user , string _token )
        {
            user = _user;
            token = _token;
        }
        Dictionary<string, object> _services  = new Dictionary<string, object>();
        public T GetClient<T>() where T : class
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
                ServiceName = serviceName,
                Token = string.Format("{0}@{1}", user, token)
            };
            //创建代理
            instance = client.ActLike<T>();
            _services[key] = instance;
            return instance as T;
        }

        public void Dispose()
        {
            foreach(var kv in _services)
            {
                var client = kv.Value.UndoActLike() as RPCClient;
                client.Dispose();
            }
            _services.Clear();
        }

    }
}
