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
        /// <summary>
        /// 验证签名
        /// </summary>
        internal static bool __CheckSign = false;
        /// <summary>
        /// 验证签名
        /// 参数都会以ToString计算,注意类型问题
        /// </summary>
        /// <returns></returns>
        public ServerCreater CheckSign()
        {
            __CheckSign = true;
            return this;
        }
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
