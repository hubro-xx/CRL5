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
            //var clientConnect = new RPCClientConnect("47.105.88.113", 805);
            var clientConnect = new RPCClientConnect("127.0.0.1", 805);
        label1:
            Test2(clientConnect);


            Console.ReadLine();
            goto label1;
        }
        static void Test2(RPCClientConnect clientConnect)
        {
            var client = clientConnect.GetClient<ITest>();
            long total = 0;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < 10; i++)
            {
                var str = client.Test1("aaa");
                //System.Threading.Thread.Sleep(10);
            }
            sw.Stop();
            var el = sw.ElapsedMilliseconds;
            Console.WriteLine("total:"+ el.ToString());
        }
    }

    public interface ITest
    {
        string Test1(string msg);
    }
    public class Test : ITest
    {
        public string Test1(string msg)
        {
            return msg;
        }
    }
}
