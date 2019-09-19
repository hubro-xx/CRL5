using CRL.Core.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.WebSocket;
namespace WebSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerCreater().CreatetWebSocket(8015);
            server.SetSessionManage(new SessionManage());
            server.Register<ITestService, TestService>();
            server.Start();
            Console.ReadLine();
        }
    }
}
