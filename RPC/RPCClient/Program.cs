using CRL.Core.Extension;
using CRL.Core.Remoting;
using CRL.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var clientConnect = new RPCClientConnect("127.0.0.1", 805);
            clientConnect.UseSign();
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
