/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using CRL.DBAccess;
using CRL.Core;

namespace WebTest.Code
{
    public class LocalSqlHelper
    {

        static DBHelper CreateDbHelper(string name)
        {
            string connString;
            //mssql
            //更改DBConnection目录内数据连接文件
            connString = CustomSetting.GetConnectionString(name);
            return new SqlHelper(connString);

            ////mysql
            //connString = "Server=127.0.0.1;Port=3306;Stmt=;Database=testDB; User=root;Password=123456;";
            //return new MySqlHelper(connString);

            //oracle
            //connString = "Data Source={0};User ID={1};Password={2};Integrated Security=no;";
            //connString = string.Format(connString, "orcl", "SCOTT", "a123");
            //return new OracleHelper(connString);
        }
        public static DBHelper MysqlConnection
        {
            get
            {
                var connString = CustomSetting.GetConnectionString("Mysql");
                return new MySqlHelper(connString);
            }
        }
        public static DBHelper MysqlConnection2
        {
            get
            {
                var connString = CustomSetting.GetConnectionString("Mysql2");
                return new MySqlHelper(connString);
            }
        }
        public static DBHelper TestConnection
        {
            get
            {
                return CreateDbHelper("Default");
            }
        }
        public static DBHelper MongoDB
        {
            get
            {
                var connString = CustomSetting.GetConnectionString("mongodb");
                //只是用来传连接串
                return new MongoDBHelper(connString, "DbProvince");
            }
        }
        public static DBHelper TestConnection2
        {
            get
            {
                return CreateDbHelper("db2");
            }
        }
    }
}
