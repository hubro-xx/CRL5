using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebApiClient
{
    public interface ITestService
    {
        string Test(string str, int a);
        string Test2(argsTest a, int b);
        string Test3();
        void Test4(out string error);
    }
    public class argsTest
    {
        public DateTime time;
        public string str;
    }
}
