using CRL.Core.Remoting;
using CRL.DynamicWebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerCreater().CreatetApi();
            server.Register<ITestService, TestService>();
            var listener = new ServerListener();
            listener.Start("http://localhost:809/");//启用apiService1
            //注册服务
            //var consulClient = new CRL.Core.ConsulClient.Consul("http://localhost:8500");
            var consulClient = new CRL.Core.ConsulClient.Consul("http://localhost:3400", true);
            var info = new CRL.Core.ConsulClient.ServiceRegistrationInfo
            {
                Address = "localhost",
                Name = "apiService1",
                ID = "apiService1",
                Port = 809,
                Tags = new[] { "v1" }
            };
            consulClient.DeregisterService(info.ID);
            var a = consulClient.RegisterService(info);//注册apiService1
            var clientConnect = new CRL.DynamicWebApi.ApiClientConnect("");
            clientConnect.UseConsulApiGatewayDiscover("http://localhost:3400", "apiService1");//服务发现

            var clientConnect2 = new CRL.DynamicWebApi.ApiClientConnect("");
            clientConnect2.UseConsulApiGateway("http://localhost:3400");//直接使用网关

        label1:

            var service1 = clientConnect.GetClient<ITestService>();
            service1.Login();
            Console.WriteLine("服务发现调用成功");

            var service2 = clientConnect2.GetClient<ITestService>("serviceTest");
            service2.Login();
            Console.WriteLine("服务网关调用成功");

            Console.ReadLine();

            goto label1;
        }
    }
}
