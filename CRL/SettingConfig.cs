/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
  

    /// <summary>
    /// 表示CacheServer处理数据的方法委托
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public delegate CacheServer.ResultData ExpressionDealDataHandler(CacheServer.Command command);

    /// <summary>
    /// as bool TransMethod(out string error);
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public delegate bool TransMethod(out string error);

    public class DBAccessBuild
    {
        DBType _DBType;
        string _connectionString;
        public DBAccessBuild(DBType dbType, string connectionString)
        {
            _DBType = dbType;
            _connectionString = connectionString;
        }
        internal DBHelper GetDBHelper()
        {
            switch (_DBType)
            {
                case DBType.ACCESS:
                    return new AccessHelper(_connectionString);

                case DBType.MongoDB:
                    var lastIndex = _connectionString.LastIndexOf("/");
                    var DatabaseName = _connectionString.Substring(lastIndex + 1);//like mongodb://localhost:27017/db1
                    return new MongoDBHelper(_connectionString, DatabaseName);

                case DBType.MSSQL:
                    return new SqlHelper(_connectionString);

                case DBType.MSSQL2000:
                    return new Sql2000Helper(_connectionString);

                case DBType.MYSQL:
                    return new MySqlHelper(_connectionString);

                case DBType.ORACLE:
                    return new OracleHelper(_connectionString);

            }
            throw new CRLException("未知的类型" + _DBType);
        }
    }
    /// <summary>
    /// 框架部署,请实现委托
    /// </summary>
    public class SettingConfig
    {
        #region 委托
        /// <summary>
        /// 获取数据连接
        /// </summary>
        public static Func<DBLocation, DBAccessBuild> GetDbAccess;

        #endregion

        /// <summary>
        /// 清除所有内置缓存
        /// </summary>
        public static void ClearCache(string dataBase)
        {
            MemoryDataCache.CacheService.Clear(dataBase);
        }
        #region 设置
        /// <summary>
        /// 内置REDIS连接
        /// </summary>
        public static string RedisConn
        {
            set
            {
                Core.RedisProvider.RedisClient.GetRedisConn = () => value;
            }
        }
        /// <summary>
        /// 是否使用属性更改通知
        /// 如果使用了,在查询时就不设置源对象克隆
        /// 在实现了属性构造后,可设为true
        /// </summary>
        public static bool UsePropertyChange = false;

        /// <summary>
        /// string字段默认长度
        /// </summary>
        public static int StringFieldLength = 30;
        /// <summary>
        /// 是否检测表结构,生产服务器可将此值设为FALSE
        /// </summary>
        public static bool CheckModelTableMaping = true;

        /// <summary>
        /// 是否自动跟踪对象状态
        /// 为否则需要调用IMode.BeginTracking(),使更新时能识别
        /// query.__TrackingModel,同时生效,才进行跟踪
        /// </summary>
        public static bool AutoTrackingModel = true;

        /// <summary>
        /// 是否使用主从读写分离
        /// 启用后,不会自动检查表结构
        /// 在事务范围内,查询按主库
        /// </summary>
        public static bool UseReadSeparation = false;

        /// <summary>
        /// 是否记录SQL语句调用
        /// </summary>
        public static bool LogSql = false;
        /// <summary>
        /// 生成参数是否与字段名一致
        /// </summary>
        public static bool FieldParameName = false;
        /// <summary>
        /// 是否替换SQL拼接参数
        /// </summary>
        public static bool ReplaceSqlParameter = false;//生成存储过程时不能替换
        /// <summary>
        /// 默认nolock
        /// </summary>
        public static bool QueryWithNoLock = true;
        /// <summary>
        /// 分页是否编译存储过程
        /// </summary>
        public static bool CompileSp = true;
        #endregion
    }


}
