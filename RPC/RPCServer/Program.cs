using CRL.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Remoting;
namespace RPCServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerCreater().CreatetRPC(805);
            server.SetSessionManage(new SessionManage());
            server.Register<ITest, Test>();
            server.Start();
            Console.ReadLine();
        }
        public interface ITest
        {
            bool login(int? a);
            string Test1(string msg, out string error);
        }
        public class Test : AbsService, ITest
        {
            [LoginPoint]
            public bool login(int? a)
            {
                SaveSession("user","token");
                return true;
            }
       
            public string Test1(string msg, out string error)
            {
                var user = GetUser();
                error = "error";
                return msg;
            }
        }
    }
}
