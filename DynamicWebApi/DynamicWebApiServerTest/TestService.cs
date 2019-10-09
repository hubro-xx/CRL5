using CRL.DynamicWebApi;
using CRL.Core.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebApiServerTest
{
    public class outObj
    {
        public string name
        {
            get;set;
        }
    }
    public interface ITestService
    {
        string Login(string name, string  pass);
        void SendData(string msg, int? a);
        bool CancelOrder(string orderNo, decimal refoundAmount, string passWord, string merchantSecre, out string error);
    }
    public class argsTest
    {
        public DateTime time;
        public string str;
    }
    public class TestService : AbsService, ITestService
    {
        public TestService()
        {

        }
        public bool CancelOrder(string orderNo, decimal refoundAmount, string passWord, string merchantSecre, out string error)
        {
            error = "error";
            return true;
        }

        [LoginPoint]
        public string Login(string name, string pass)
        {
            //obj = new outObj() { name= "outObj" };
            SaveSession("hubro", "7777777777", "test");
            return name;
        }

        public void SendData(string msg,int? a)
        {
            var user = CurrentUserName;
            var tag = CurrentUserTag;
            var file = GetPostFile();
        }
    }
}
