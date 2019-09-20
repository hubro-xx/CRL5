using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Extension;
namespace WebSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientConnect = new CRL.WebSocket.WebSocketClientConnect("127.0.0.1", 8015);
            clientConnect.OnMessage += (obj) =>
            {
                Console.WriteLine("OnMessage:" + obj.ToJson());
            };
            clientConnect.StartPing();
            var service = clientConnect.GetClient<ITestService>();
            var str = service.Login("user", "123");
        label1:
 
            Console.WriteLine(str);
            int? a = 1;
            service.SendData("data", a);
            var error = "";
            service.CancelOrder("", 2, "", null, out error);
            Console.ReadLine();
            goto label1;
        }
    }
}
