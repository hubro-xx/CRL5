using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Mongo
{
    public static class Extension
    {
        public static SettingConfigBuilder UseMongoDB(this SettingConfigBuilder builder)
        {
            builder.RegisterDBType<MongoDBHelper, MongoDBAdapter>(DBAccess.DBType.MongoDB);
            builder.RegisterDBExtend<MongoDBEx.MongoDBExt>(DBAccess.DBType.MongoDB);
            return builder;
        }
    }
}
