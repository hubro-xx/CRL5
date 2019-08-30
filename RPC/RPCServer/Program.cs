﻿using CRL.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Remoting;
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
        public class Test : AbsService, ITest
        {
            [LoginPoint]
            public bool login()
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
