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
            var clientConnect = new CRL.DynamicWebApi.ApiClientConnect("http://localhost:53065");
            clientConnect.OnError = (ero, code) =>
            {
                Console.WriteLine(ero + code);
            };
                 var service = clientConnect.GetClient<ITestService>();
        label1:
            var str = service.Login("user", "123");
            Console.WriteLine(str);
            int? a = 1;
            service.SendData("data",a);

            Console.ReadLine();
            goto label1;
        }
    }
}
