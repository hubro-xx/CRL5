using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace CRL.Core.Session
{
    public class WebSession : AbsSession
    {
        public WebSession(HttpContextBase _context) : base(_context)
        {
          
        }

        public override void Clean()
        {
            context.Session.Clear();
        }

        public override T Get<T>(string name)
        {
            var obj = context.Session[name];
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }

        public override void Refresh()
        {
            
        }

        public override void Remove(string name)
        {
            context.Session.Remove(name);
        }

        public override void Set(string name, object value)
        {
            context.Session[name] = value;
        }
    }
}
