using CRL;
using CRL.Core;
using CRL.DBAccess;
using CRLTest.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Extension;
using CRL.Core.RedisProvider;
namespace CRLTest
{
    class testClass
    {
        public object name
        {
            get; set;
        }
        public DateTime? time
        {
            get; set;
        }
        public b b
        {
            get; set;
        }
        public decimal price
        {
            get; set;
        }
        public Dictionary<string, object> dic
        {
            get; set;
        }
    }
    public class b
    {
        public string name
        {
            get;set;
        }
        public string name2
        {
            get; set;
        }
    }
    public class MyGenericClass<T>
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
            Type classType = Type.GetType("CRLTest.MyGenericClass`1, CRLTest");
            //var obj = new testClass() { time = DateTime.Now };
            //var json = obj.ToJson();
            //Console.WriteLine(json);
            //Console.ReadLine();
            var configBuilder = new CRL.Core.ConfigBuilder();
            configBuilder.UseRedis("passs@127.0.0.0:27017")
                .UseRedisSession();
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
                if(dbLocation.ManageName=="mongo")
                {
                    var conn = CRL.Core.CustomSetting.GetConfigKey("mongodb");
                    return new CRL.DBAccessBuild(DBType.MongoDB, conn);

                }
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
            string str = "111";
        label1:
            MongoDBTestManage.Instance.GroupTest();
            //testFormat();
            //Code.TestAll.TestSelect();

            Console.ReadLine();
            goto label1;
            Console.ReadLine();
        }
        static void testFormat()
        {
            var obj = new testClass() { };
            obj.b = new b() { name = "b22424" };
            obj.dic = new Dictionary<string, object>() { { "tes111111111112t", 1 }, { "t2222222222122est2", 1 }, { "te22222221s3t", 1 } };
            obj.name = "test2ConvertObject";
            obj.time = DateTime.Now;
            obj.price = 1002;

            var count = 2;
            long total1 = 0;
            long total2 = 0;
            var sw = new System.Diagnostics.Stopwatch(); ;
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                total1 += testJson(obj);
            }
            sw.Stop();
            var el = sw.ElapsedMilliseconds;
            Console.WriteLine($"el:{el} t:{total1}");
            sw = new System.Diagnostics.Stopwatch(); ;
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                total2 += testBinary(obj);
            }
            sw.Stop();
            var el2 = sw.ElapsedMilliseconds;
            Console.WriteLine($"el:{el2} t:{total2}");
        }
        static int testJson(testClass obj)
        {
            var json = SerializeHelper.SerializerToJson(obj);
            var obj2 = SerializeHelper.DeserializeFromJson<testClass>(json);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            return buffer.Length;
        }
        static int testBinary(testClass obj)
        {
            var data = CRL.Core.BinaryFormat.ClassFormat.Pack(obj.GetType(), obj);
            var obj2 = CRL.Core.BinaryFormat.ClassFormat.UnPack(obj.GetType(), data);
            return data.Length;
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
