using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace CRL.DBAccess
{
    public class Sql2000Helper : SqlHelper
    {
        public override DBType CurrentDBType
        {
            get
            {
                return DBType.MSSQL2000;
            }
        }
        public Sql2000Helper(string content)
            : base(content)
        { }
    }
    public  class SqlHelper:DBHelper 
    {
        /// <summary>
        /// 根据参数类型实例化
        /// </summary>
        /// <param name="_connectionString">内容</param>
		public SqlHelper(string _connectionString)
            : base(_connectionString)
        { }
        
        static Dictionary<string, string> formatCache = new Dictionary<string, string>();
        static object lockObj = new object();
        public override DBType CurrentDBType
        {
            get
            {
                return DBType.MSSQL;
            }
        }
        public override string FormatWithNolock(string cmdText)
        {
            return cmdText;
            if (!System.Text.RegularExpressions.Regex.IsMatch(cmdText, @"^select\s", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return cmdText;
            }
            if (formatCache.ContainsKey(cmdText))
                return formatCache[cmdText];
            string pat = @"(update|insert|delete|truncate)\s+[a-z0-9._#]+";
            if (!System.Text.RegularExpressions.Regex.IsMatch(cmdText, pat, System.Text.RegularExpressions.RegexOptions.IgnoreCase) && AutoFormatWithNolock)
            {
                //换行
                string sql = Regex.Replace(cmdText, @"\r\n", " ", RegexOptions.IgnoreCase);
                //去掉WITH
                sql = Regex.Replace(sql, @" with\s*\(\s*nolock\s*\)", " ", RegexOptions.IgnoreCase).Trim();
                // like select * from table ass inner
                sql = Regex.Replace(sql, @" (from(\s+[a-z0-9._#\[\]]+)+?)\s+(where|left|right|inner|group|order)", " $1 with(nolock) $3", RegexOptions.IgnoreCase);
                //like inner join table as bb on c.a=bb.a
                sql = Regex.Replace(sql, @" (join(\s+[a-z0-9._#\[\]]+)+?)\s+on", " $1 with(nolock) on", RegexOptions.IgnoreCase);

                //like select * from (select * from table1)
                sql = Regex.Replace(sql, @" (from\s+([a-z0-9._#\[\]]+)((\s+as)*(\s+[a-z0-9._#]+))*)\s*\)", " $1 with(nolock))", RegexOptions.IgnoreCase);
                //like select * from table
                sql = Regex.Replace(sql.Trim(), @" (from\s+([a-z0-9._#\[\]]+)((\s+as){0,1}(\s+[a-z0-9._#]+)){0,1}$)", " $1 with(nolock)", RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, @"([a-z0-9._#]+)\s*,\s*([a-z0-9._#\[\]]+)", "$1 , $2", RegexOptions.IgnoreCase);
                string pattern = @"from\s+(((([a-z0-9._#]+)\s*)+(,\s*){0,1})+)";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                Match m;
                for (m = r.Match(sql); m.Success; m = m.NextMatch())
                {
                    string str1 = m.Groups[1].ToString();
                    string str2 = Regex.Replace(str1, @"([a-z0-9._#\[\]]+)\s*(,|where)", " $1 with(nolock) $2", RegexOptions.IgnoreCase);
                    sql = sql.Replace(str1, str2);
                }
                //EventLog.Log(string.Format("格式化为WithNolock:{0}\r\n处理后:{1}", cmdText, sql), "sql");
                lock(lockObj)
                {
                    if (!formatCache.ContainsKey(cmdText))
                    {
                        formatCache.Add(cmdText,sql);
                    }
                }
                return sql;
            }
            return cmdText;
        }
        protected override void fillCmdParams_(DbCommand cmd)
        {
            foreach (KeyValuePair<string, object> kv in _params)
            {
                DbParameter p = new SqlParameter(kv.Key, kv.Value);
                if (kv.Value != null)
                {
                    if (kv.Value is DBNull)
                    {
                        p.IsNullable = true;
                    }
                }
                cmd.Parameters.Add(p);
            }
            if (cmd.CommandType == CommandType.StoredProcedure)
            {
                if (OutParams != null)
                {
                    foreach (KeyValuePair<string, object> kv in OutParams)
                    {
                        //不为return ,才进行OUTPUT设置
                        if (kv.Key != "return")
                        {
                            DbParameter p = new SqlParameter(kv.Key, SqlDbType.NVarChar, 500);
                            p.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(p);
                        }
                    }
                }
                DbParameter p1 = new SqlParameter("return", SqlDbType.Int);
                p1.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(p1);
            }
        }
        protected override DbCommand createCmd_(string cmdText, DbConnection conn)
        {
            cmdText = FormatWithNolock(cmdText);
            return new SqlCommand(cmdText, (SqlConnection)conn);
        }
        protected override DbCommand createCmd_()
        {
            return new SqlCommand();
        }
        protected override DbDataAdapter createDa_(string cmdText, DbConnection conn)
        {
            cmdText = FormatWithNolock(cmdText);
            return new SqlDataAdapter(cmdText, (SqlConnection)conn);
        }
        protected override DbConnection createConn_()
        {
            return new SqlConnection(ConnectionString);
        }


        /// <summary>
        /// 新的分页存储过程，更改原来查询结果排序错误
        /// 以前传入排序参数可能不兼容，会导致语法错误
        /// </summary>
        /// <param name="tableName">要显示的表或多个表的连接</param>
        /// <param name="fields">要显示的字段列表</param>
        /// <param name="sortfield">排序字段</param>
        /// <param name="singleSortType">排序方法，false为升序，true为降序</param>
        /// <param name="pageSize">每页显示的记录个数</param>
        /// <param name="pageIndex">要显示那一页的记录</param>
        /// <param name="condition">查询条件,不需where</param>
        /// <param name="count">查询到的记录数</param>
        /// <returns></returns>
        public override DataTable TablesPage(string tableName, string fields, string sortfield, bool singleSortType, int pageSize, int pageIndex, string condition, out int count)
        {
            this.Params.Clear();
            this.Params.Add("tblName", tableName);

            this.Params.Add("fields", fields);
            this.Params.Add("sortfields", sortfield);
            this.Params.Add("singleSortType", singleSortType ? "1" : "0");
            this.Params.Add("pageSize", pageSize);
            this.Params.Add("pageIndex", pageIndex);
            this.Params.Add("strCondition", condition);
            //SqlParameter p = new SqlParameter("@Counts", SqlDbType.Int);
            //this.OutParams.Add(p);
            AddOutParam("counts");
            DataTable dt = this.RunDataTable("sp_TablesPageNew");
            count = Convert.ToInt32(GetOutParam("counts"));
            return dt;
        }

        /// <summary>
        /// 根据表插入记录,dataTable需按查询生成结构
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        /// <param name="keepIdentity"></param>
        public override void InsertFromDataTable(DataTable dataTable, string tableName, bool keepIdentity = false)
        {
            SqlBulkCopy sqlBulkCopy;

            if (_trans != null)
            {
                SqlTransaction sqlTrans = _trans as SqlTransaction;
                sqlBulkCopy = new SqlBulkCopy(currentConn as SqlConnection, keepIdentity ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.KeepNulls, sqlTrans);
            }
            else
            {
                sqlBulkCopy = new SqlBulkCopy(base.ConnectionString, keepIdentity ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.KeepNulls);
            }
            sqlBulkCopy.DestinationTableName = tableName;
            sqlBulkCopy.BatchSize = dataTable.Rows.Count;
            if (dataTable != null && dataTable.Rows.Count != 0)
            {
                sqlBulkCopy.WriteToServer(dataTable);
            }
            sqlBulkCopy.Close();
        }
    }
}
