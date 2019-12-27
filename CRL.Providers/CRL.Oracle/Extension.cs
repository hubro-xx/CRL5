using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Oracle
{
    public static class Extension
    {
        public static SettingConfigBuilder UseOracle(this SettingConfigBuilder builder)
        {
            builder.RegisterDBType<OracleHelper, ORACLEDBAdapter>(DBAccess.DBType.ORACLE);
            return builder;
        }
    }
}
