using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.Remoting
{
    public class ServerCreater
    {
        AbsServer Server;
        internal static ISessionManage SessionManage
        {
            get; set;
        } = new SessionManage();
        public void SetServer(AbsServer server)
        {
            Server = server;
        }
        public AbsServer GetServer()
        {
            return Server;
        }
        public ServerCreater SetSessionManage(ISessionManage _sessionManage)
        {
            SessionManage = _sessionManage;
            return this;
        }
        public ServerCreater Register<IService, Service>() where Service : AbsService, IService, new() where IService : class
        {
            Server.Register<IService, Service>();
            return this;
        }
        public void Start()
        {
            Server.Start();
        }
        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
