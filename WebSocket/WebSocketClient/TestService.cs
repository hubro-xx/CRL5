
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient
{
    public interface ITestService
    {
        string Login(string name, string pass);
        void SendData(string msg, int? a);
        bool CancelOrder(string orderNo, decimal refoundAmount, string passWord, string merchantSecre, out string error);
    }
}
