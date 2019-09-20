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
            new CRL.Core.ThreadWork().Start("send", () =>
            {
                var socket = server.GetServer() as CRL.WebSocket.WebSocketServer;
                socket.SendMessage("hubro", 1000, out string error);
                Console.WriteLine("send msg");
                return true;
            }, 10);
            Console.ReadLine();
        }
    }
}
