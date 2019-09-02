using CRL.DynamicWebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebApiClient
{
    public interface ITestService
    {
        string Login(string name, string pass);
        void SendData(string msg, int? a);

    }
}
