using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientConnect = new CRL.DynamicWebApi.ApiClientConnect("http://localhost:65368");
            clientConnect.SetToken("user", "123");
            var service = clientConnect.GetClient<ITestService>();
        label1:
            //service.Test3();
            //service.Test("test", 100);
            //service.Test2(new argsTest() { str = "test", time = DateTime.Now }, 111);
            service.Test4(out string error);
            Console.WriteLine(error);
            Console.ReadLine();
            goto label1;
        }
    }
}
