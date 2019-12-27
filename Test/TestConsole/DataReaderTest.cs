/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class DataReaderTest:IDisposable
    {
        SqlConnection _sqlConnection;
        public DataReaderTest()
        {
            _sqlConnection = new SqlConnection(DbHelper.ConnectionString);
            _sqlConnection.Open();
        }
        public SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.Text;
            
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
           
            return sqlDataReader;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }

    class DataReaderTest2 
    {
        SqlConnection _sqlConnection;
        public DataReaderTest2()
        {
            _sqlConnection = new SqlConnection(DbHelper.ConnectionString);
            _sqlConnection.Open();
        }
        public SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.Text;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            return sqlDataReader;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }

}
