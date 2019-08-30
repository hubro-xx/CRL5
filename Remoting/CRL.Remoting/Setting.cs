using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Remoting
{
    public class Setting
    {
        public static ISessionManage SessionManage
        {
            get; set;
        } = new SessionManage();
    }
}
