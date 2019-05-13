using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Core.Log
{
    [Serializable]
    public class LogCache
    {
        private static LogCache instance;
        public static LogCache Instance
        {
            get
            {
                if (instance == null)
                    instance = FromFile();
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        private DateTime updateTime;
        private long msgId = 100;

        public long MsgId
        {
            get { return msgId; }
            set { msgId = value; }
        }

        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = value; }
        }
        private Dictionary<string, List<LogItem>> caches = new Dictionary<string, List<LogItem>>();
        public Dictionary<string, List<LogItem>> Caches
        {
            get { return caches; }
            set { caches = value; }
        }
        //private List<UploadService.LogServer.LogItem> caches = new List<UploadService.LogServer.LogItem>();

        //public List<UploadService.LogServer.LogItem> Caches
        //{
        //    get { return caches; }
        //    set { caches = value; }
        //}
        public static LogCache FromFile()
        {
            string rootpath = System.Web.Hosting.HostingEnvironment.MapPath(@"\log\");
            string file = rootpath + "LogCache.config";
            LogCache cache = null;
            if (System.IO.File.Exists(file))
            {
                try
                {
                    cache = CRL.Core.SerializeHelper.BinaryDeserialize<LogCache>(file);
                    CRL.Core.EventLog.Log("读取LogCache");
                }
                catch { }
            }
            if (cache == null)
                cache = new LogCache();
            return cache;
        }
        public void Save()
        {
            string rootpath = System.Web.Hosting.HostingEnvironment.MapPath(@"\log\");
            string file = rootpath + "LogCache.config";
            CRL.Core.SerializeHelper.BinarySerialize(this, file);
            CRL.Core.EventLog.Log("保存LogCache");
        }
    }
}
