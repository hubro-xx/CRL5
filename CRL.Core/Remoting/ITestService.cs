using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Extension;
namespace CRL.Core.Remoting
{
    public class TestObj
    {
        public string Name { get; set; }
    }
    public interface ITestService
    {
        void Login();
        bool Test1(int a,int? b,out string error);
        TestObj Test2(TestObj obj);
    }
    public class TestService : AbsService, ITestService
    {
        [LoginPoint]
        public void Login()
        {
            SaveSession("hubro", "7777777777", "test");
        }

        public bool Test1(int a, int? b, out string error)
        {
            var user = CurrentUserName;
            var tag = CurrentUserTag;

            error = "out error";
            Console.WriteLine(a);
            Console.WriteLine(b);
            return true;
        }

        public TestObj Test2(TestObj obj)
        {
            Console.WriteLine(obj.ToJson());
            return obj;
        }
    }
}
