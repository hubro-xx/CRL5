using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;

namespace CRL.DBAccess
{
    public class AccessHelper : DBHelper
    {
		public AccessHelper(string content)
            : base(content)
        { }
   
        public override DBType CurrentDBType
        {
            get
            {
                return DBType.ACCESS;
            }
        }
        protected override void fillCmdParams_(DbCommand cmd)
        {
            foreach (KeyValuePair<string, object> kv in _params)
            {     
                DbParameter p = new OleDbParameter(kv.Key, kv.Value);
                cmd.Parameters.Add(p);
            }
            //foreach (OleDbParameter outp in _outParams)
            //{
            //    outp.Direction = ParameterDirection.Output;
            //    cmd.Parameters.Add(outp);
            //}
            if (OutParams == null)
                return;
            foreach (KeyValuePair<string, object> kv in OutParams)
            { 
                //为空的则为默认值,才进行OUTPUT设置
                if (kv.Value == null)
                {
                    DbParameter p = new OleDbParameter(kv.Key, OleDbType.VarChar, 500);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);
                }
            }
        }
        protected override DbCommand createCmd_(string cmdText, DbConnection conn)
        {
            return new OleDbCommand(cmdText, (OleDbConnection)conn);
        }
        protected override DbCommand createCmd_()
        {
            return new OleDbCommand();
        }
        protected override DbDataAdapter createDa_(string cmdText, DbConnection conn)
        {
            return new OleDbDataAdapter(cmdText, (OleDbConnection)conn);
        }
        protected override DbConnection createConn_()
        {
            return new OleDbConnection(ConnectionString);
        }

        public override void InsertFromDataTable(DataTable dataTable, string tableName, bool keepIdentity = false)
        {
            throw new NotImplementedException();
        }

        public override DataTable TablesPage(string tableName, string fields, string sortfield, bool singleSortType, int pageSize, int pageIndex, string condition, out int count)
        {
            throw new NotImplementedException();
        }
    }
}
