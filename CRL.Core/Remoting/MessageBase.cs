using CRL.Core.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Core.Remoting
{
    public class MessageBase
    {
        public string Token
        {
            get; set;
        }

        public string Service { get; set; }
        public string Method { get; set; }
        public List<object> Args { get; set; }
        public HttpPostedFile httpPostedFile;
    }
}
