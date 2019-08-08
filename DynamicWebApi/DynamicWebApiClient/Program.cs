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
            clientConnect.OnError = (ero, code) =>
            {
                Console.WriteLine(ero + code);
            };
                 var service = clientConnect.GetClient<ITestService>();
        label1:
            service.Login("user","123");
            //clientConnect.SetToken("user", "123");

            service.SendData("data");

            Console.ReadLine();
            goto label1;
        }
    }
}
