using CRL;
using CRL.DBAccess;
using CRLTest.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRLTest
{
    class testClass
    {
        public string name
        {
            get; set;
        }
        public string aa;
        public DateTime time
        {
            get; set;
        }
        public decimal price
        {
            get; set;
        }
    }
    class Program
    {
        class test2
        {
            public string name
            {
                get;set;
            }
            public string name2;
        }
        static void Main(string[] args)
        {

            //var s2= ConvertObject(typeof(CRL.DBAccess.DBType), "1");
            //var s3 = ConvertObject(typeof(decimal), "");

            var test2 = new test2() { name="中广开言路上" };
            var json = CRL.Core.SerializeHelper.SerializerToJson(test2);
            //自定义定位
            CRL.Sharding.LocationManage.Register<Code.Sharding.MemberSharding>((t, a) =>
            {
                var tableName = t.TableName;
                if (a.Name == "hubro")
                {
                    tableName = "MemberSharding1";
                    return new CRL.Sharding.Location("testdb2", tableName);
                }
                //返回定位库和表名
                return new CRL.Sharding.Location("testdb", tableName);
            });
            CRL.SettingConfig.GetDbAccess = (dbLocation) =>
            {
                //定位库
                if (dbLocation.ShardingLocation != null)
                {
                    return new CRL.DBAccessBuild(DBType.MSSQL, "Data Source=.;Initial Catalog=" + dbLocation.ShardingLocation.DataBaseName + ";User ID=sa;Password=123");
                }
                return new CRL.DBAccessBuild(DBType.MSSQL, "server=.;database=testDb; uid=sa;pwd=123;");
            };


        //Code.MemberManage.Instance.QueryItem(1);
        //Code.OrderManage.Instance.QueryItem(1);
        //Code.ProductDataManage.Instance.QueryItem(1);
        label1:
            //testSharding();
            //TestAll();
            var str = "ffsf";
            //Test5(str);
            Code.TestAll.TestSelect();
            //Code.TestAll.TestUnion();
            Console.ReadLine();
            goto label1;
            Console.ReadLine();
        }
        static void testSharding()
        {
            var instance = new Code.Sharding.MemberManage();
            instance.SetLocation(new Code.Sharding.MemberSharding() { Name = "hubro" });
            var obj = instance.QueryItem(1);
            Console.WriteLine(obj?.Name);
            instance.SetLocation(new Code.Sharding.MemberSharding() { Name = "hubro2" });
            var obj2 = instance.QueryItem(1);
            Console.WriteLine(obj2?.Name);
        }
        static void TestAll()
        {
            var array = typeof(Code.TestAll).GetMethods(BindingFlags.Static | BindingFlags.Public).OrderBy(b => b.Name.Length);
            var instance = new Code.TestAll();
            foreach (var item in array)
            {
                if (item.Name == "TestUpdate")
                {
                    continue;
                }
                try
                {
                    item.Invoke(instance, null);
                    Console.WriteLine($"{item.Name} ok");
                }
                catch(Exception ero)
                {
                    Console.WriteLine($"{item.Name} error {ero.Message}");
                }
 
            }
        }

        public static void Test5(string abc = "123")
        {
            var query = ProductDataManage.Instance.GetLambdaQuery();
            query.Page(2, 1);
            //query.Where(b => b.Id > 1 && b.CategoryName.StartsWith(b.ProductName));
            //query.Join<Code.Order>((a, b) => a.Id == b.Id).Select((a, b) => new testC { id = b.Id }).OrderBy(b => b.id).ToList();
            var list = new List<int>() { 1, 2, 3 };
            //query.Where(b=>list.Contains(b.Id));
            query.Where(b => b.Id > 1 && b.CategoryName.StartsWith(abc));
            query.Join<Member>((a, b) => a.Id == b.Id)
                .Select((a, b) => b).ToList();
            Console.WriteLine(query.ToString());
        }

    }
}
