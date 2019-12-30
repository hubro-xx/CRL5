/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    [CRL.Attribute.Table(TableName = "TestEntity")]
    public class TestEntityCRL : CRL.IModel
    {

        [Column(IsPrimaryKey = true)]
        [AutoIncrement]
        public int Id { get; set; }
        public byte? F_Byte { get; set; }
        public Int16? F_Int16 { get; set; }
        public int? F_Int32 { get; set; }
        public long? F_Int64 { get; set; }
        public double? F_Double { get; set; }
        public float? F_Float { get; set; }
        public decimal? F_Decimal { get; set; }
        public bool? F_Bool { get; set; }
        public DateTime? F_DateTime { get; set; }
        public Guid? F_Guid { get; set; }
        public string F_String { get; set; }
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<TestEntityCRL>();
            for (int i = 0; i < 5000; i++)
            {
                list.Add(new TestEntityCRL() { F_Bool = true, F_Byte = 1, F_DateTime = DateTime.Now, F_Decimal = 100.23M, F_Double = 23.22, F_Float = 1.22F, F_Guid = System.Guid.NewGuid(), F_Int16 = 22, F_Int32 = 333, F_Int64 = 333, F_String = "string" + i });
            }
            return list;
        }

    }
    class CRLManage : CRL.BaseProvider<TestEntityCRL>
    {
        public static CRLManage Instance
        {
            get
            {
                return new CRLManage();
            }
        }

        public void Test(string sql)
        {
            //string sql = "select top " + n + " * from TestEntity";
            //DbConnection conn = new SqlConnection(DbHelper.ConnectionString);
            //DbCommand CurrentDataReadCommand = null;
            //conn.Open();
            //CurrentDataReadCommand = new SqlCommand(sql, (SqlConnection)conn);
            //CurrentDataReadCommand.CommandType = System.Data.CommandType.Text;

            //DbDataReader r;

            //r = CurrentDataReadCommand.ExecuteReader(CommandBehavior.Default);

            //r.Close();
            //conn.Close();
            var list=DBExtend.ExecList<TestEntityCRL>(sql);
        }
    }
}
