using System.Web;
using System.Collections.Generic;
namespace CRL.Core.Session
{
    public abstract class AbsSession
    {
        public const string SessionName = "_rdssn";

        protected HttpContextBase context;
        public AbsSession(HttpContextBase _context)
        {
            context = _context;
        }
        public string SessionId { get; set; }

        public abstract T Get<T>(string name);
        public abstract void Set(string name,object value);
        public abstract void Remove(string name);
        public abstract void Refresh();
        public abstract void Clean();
    }
    public class SessionObj
    {
        public Dictionary<string,string> Data
        {
            get;set;
        }
    }
}