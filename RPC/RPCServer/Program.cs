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

            rPCServer.Register<ITest, Test>();
            rPCServer.Start();
            
            Console.ReadLine();
        }
        public interface ITest
        {
            bool login();
            string Test1(string msg, out string error);
        }
        public class Test : ITest
        {
            [CRL.RPC.LoginPoint]
            public bool login()
            {
                CRL.RPC.SessionManage.SaveSession("user","token");
                return true;
            }
       
            public string Test1(string msg, out string error)
            {
                var user = SessionManage.GetSession();
                error = "error";
                return msg;
            }
        }
    }
}
