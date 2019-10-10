using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Extension;
using CRL.Core.Remoting;

namespace WebSocketClient
{
    class Program
    {
        class socketMsg
        {
            public string name
            {
                get;set;
            }
        }
        static void showId()
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(id);
        }
        static void Main(string[] args)
        {

            var clientConnect = new CRL.WebSocket.WebSocketClientConnect("127.0.0.1", 8015);
            clientConnect.UseSign();
            clientConnect.SubscribeMessage<socketMsg>((obj) =>
            {
                Console.WriteLine("OnMessage:" + obj.ToJson());
            });
            clientConnect.StartPing();
            var service = clientConnect.GetClient<ITestService>();
        label1:

            service.Login();
            Console.WriteLine("loginOk");
            int? a = 1;
            string error;
            service.Test1(1, a, out error);
            Console.WriteLine("error:" + error);
            var obj2 = service.Test2(new TestObj() { Name = "test" });
            Console.WriteLine("obj2:" + obj2.ToJson());
            Console.ReadLine();
            goto label1;
        }
    }
}
