using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace CRL.DBAccess
{
    public class MySqlHelper : DBHelper
    {
		public MySqlHelper(string content)
            : base(content)
        { }
        public override DBType CurrentDBType
        {
            get
            {
                return DBType.MYSQL;
            }
        }
        protected override void fillCmdParams_(DbCommand cmd)
        {
            foreach (KeyValuePair<string, object> kv in _params)
            {
                var key = kv.Key;
                //key = key.Replace("@","?");
                DbParameter p = new MySqlParameter(key, kv.Value);
                cmd.Parameters.Add(p);
            }
            if (OutParams != null)
            {
                foreach (KeyValuePair<string, object> kv in OutParams)
                {
                    var key = kv.Key;
                    //key = key.Replace("@", "?");
                    //不为return ,才进行OUTPUT设置
                    if (key != "return")
                    {
                        DbParameter p = new MySqlParameter(key, MySqlDbType.VarString, 500);
                        p.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(p);
                    }
                }
            }
            if (cmd.CommandType == CommandType.StoredProcedure)
            {
                DbParameter p1 = new MySqlParameter("return", MySqlDbType.Int32);
                p1.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(p1);
            }
        }
        protected override DbCommand createCmd_(string cmdText, DbConnection conn)
        {
            return new MySqlCommand(cmdText, (MySqlConnection)conn);
        }
        protected override DbCommand createCmd_()
        {
            return new MySqlCommand();
        }
        protected override DbDataAdapter createDa_(string cmdText, DbConnection conn)
        {

            return new MySqlDataAdapter(cmdText, (MySqlConnection)conn);
        }
        protected override DbConnection createConn_()
        {

            return new MySqlConnection(ConnectionString);
        }


        public override void InsertFromDataTable(DataTable dataTable, string tableName, bool keepIdentity = false)
        {
            throw new NotSupportedException("MySql不支持批量插入");
            throw new NotImplementedException();
        }

        public override DataTable TablesPage(string tableName, string fields, string sortfield, bool singleSortType, int pageSize, int pageIndex, string condition, out int count)
        {
            throw new NotImplementedException();
        }
    }

}
