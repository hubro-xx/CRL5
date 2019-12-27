using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.MySql
{
    public static class Extension
    {
        public static SettingConfigBuilder UseMySql(this SettingConfigBuilder builder)
        {
            builder.RegisterDBType<MySqlHelper, MySQLDBAdapter>(DBAccess.DBType.MYSQL);
            return builder;
        }
    }
}
