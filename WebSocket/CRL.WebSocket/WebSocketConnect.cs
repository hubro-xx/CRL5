using CRL.Core.Remoting;
using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.WebSocket
{
    public class WebSocketClientConnect : AbsClientConnect
    {
        string host;
        int port;
        public WebSocketClientConnect(string _host,int _port)
        {
            host = _host;
            port = _port;
        }
        public override T GetClient<T>()
        {
            var serviceName = typeof(T).Name;
            var key = string.Format("{0}_{1}", host, serviceName);
            var a = _services.TryGetValue(key, out object instance);
            if (a)
            {
                return instance as T;
            }
            var client = new WebSocketClient(this,host,port)
            {
                Host = host,
                Port = port,
                ServiceType = typeof(T),
                ServiceName = serviceName,
            };
            //创建代理
            instance = client.ActLike<T>();
            _services[key] = instance;
            return instance as T;
        }
    }
}
