using CRL.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var rPCServer = new RPCServer(805);
            rPCServer.SetTokenCheck((u, t) =>
            {
                return u == "user" && t == "123";
            });
            rPCServer.Register<ITest, Test>();
            rPCServer.Start();
            
            Console.ReadLine();
        }
        public interface ITest
        {
            string Test1(string msg, out string error);
        }
        public class Test : ITest
        {
            public string Test1(string msg, out string error)
            {
                error = "error";
                return msg;
            }
        }
    }
    public class test1 : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Dispose");
        }
    }
}
