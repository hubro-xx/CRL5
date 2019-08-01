using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebApiTest
{
    public interface ITestService
    {
        string Test(string str, int a);
        string Test2(argsTest a, int b);
        string Test3();
    }
    public class argsTest
    {
        public DateTime time;
        public string str;
    }
    public class TestService : ITestService
    {
        public string Test(string str, int a)
        {
            return $"str:{str} a:{a}";
        }

        public string Test2(argsTest a, int b)
        {
            return "Test2";
        }
        public string Test3()
        {
            return "Test3";
        }
    }
}
