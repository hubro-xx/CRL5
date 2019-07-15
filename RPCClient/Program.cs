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
            var client = RPCClientFactory.GetClient<ITest>("127.0.0.1", 8026);
            label1:
            var str = client.Test1("aaa");
            Console.WriteLine(str);
            
            Console.ReadLine();
            goto label1;
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
