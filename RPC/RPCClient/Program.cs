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
            var user = "user";
            string token = "123";
            //var clientConnect = new RPCClientConnect("47.105.88.113", 805);

            var clientConnect = new RPCClientConnect("127.0.0.1", 805);
            clientConnect.OnError = (ero, code) =>
              {
                  Console.WriteLine(ero + " " + code);
              };
        label1:
            try
            {
                Test2(clientConnect);

            }
            catch(Exception ero)
            {
                Console.WriteLine(ero.ToString());
            }

            Console.ReadLine();
            //clientConnect.Dispose();
            goto label1;
        }
        static void Test2(RPCClientConnect clientConnect)
        {
         
            var client = clientConnect.GetClient<ITest>();
            //client.login();
            long total = 0;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < 1; i++)
            {
                string error = "";
                var str = client.Test1("aaa",out error);
                Console.WriteLine($"return:{str} out:{error}");
            }
            sw.Stop();
            var el = sw.ElapsedMilliseconds;
          
        }
    }

    public interface ITest
    {
        bool login();
        string Test1(string msg,out string error);
    }
}
