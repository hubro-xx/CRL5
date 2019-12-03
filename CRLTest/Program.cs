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
    #region obj
    class testClass
    {
        public string name
        {
            get; set;
        }
        public DateTime? time
        {
            get; set;
        }
        //public b b
        //{
        //    get; set;
        //}
        public decimal price
        {
            get; set;
        }
        //public Dictionary<string, object> dic
        //{
        //    get; set;
        //}
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
    #endregion
    class Program
    {
        static void Main(string[] args)
        {
            
 
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
            consulTest();
            Console.ReadLine();
            goto label1;
            Console.ReadLine();
        }
        static void testFormat()
        {

            var obj = new testClass() { };
            //obj.b = new b() { name = "b22424" };
            //obj.dic = new Dictionary<string, object>() { { "tes111111111112t", 1 }, { "t2222222222122est2", 1 }, { "te22222221s3t", 1 } };
            obj.name = "test2ConvertObject";
            obj.time = DateTime.Now;
            obj.price = 1002;
            int count = 1000;
            new CounterWatch().Start("json", () =>
            {
                testJson(obj);
            }, count);
            //var data = CRL.Core.BinaryFormat.ClassFormat.Pack(obj.GetType(), obj);
            new CounterWatch().Start("binary",() =>
            {
                //var obj2 = CRL.Core.BinaryFormat.ClassFormat.UnPack(obj.GetType(), data);
                testBinary(obj);
            }, count);

        }
        static int testJson(testClass obj)
        {
            var json = SerializeHelper.SerializerToJson(obj);
            var obj2 = SerializeHelper.DeserializeFromJson<testClass>(json);
            //var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            return 0;
        }
        static int testBinary(testClass obj)
        {
            var data = CRL.Core.BinaryFormat.ClassFormat.Pack(obj.GetType(), obj);
            var obj2 = CRL.Core.BinaryFormat.ClassFormat.UnPack(obj.GetType(), data);
            return 0;
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
        static void MakeGenericTypeTest()
        {
            var type = CRL.Core.Extension.Extension.MakeGenericType("CRL.LambdaQuery.Mapping.QueryInfo", "CRL", typeof(ProductData));
        }

        static void requestTest()
        {
            new CRL.Core.ThreadWork().Start("11", () =>
            {
                try
                {
                    var result = CRL.Core.Request.HttpRequest.HttpGet("http://localhost:8002");
                    Console.WriteLine(result);
                }
                catch (Exception ero)
                {
                    Console.WriteLine(ero.Message);
                }
                return true;
            }, 0.3);
        }

        static void consulTest()
        {
            var client = new CRL.Core.ConsulClient.Consul("http://118.190.157.156:8500");
            var info = new CRL.Core.ConsulClient.ServiceRegistrationInfo
            {
                Address = "http://47.105.88.113",
                Name = "payService",
                ID = "payService1",
                Port = 802,
                Tags = new[] { "v1" },
                Check = new CRL.Core.ConsulClient.CheckRegistrationInfo()
                {
                    HTTP = "http://pay.gsp2013.com/",
                    Interval = "10s",
                    DeregisterCriticalServiceAfter = "90m"
                }
            };
            var info2 = new CRL.Core.ConsulClient.ServiceRegistrationInfo
            {
                Address = "http://47.105.88.113",
                Name = "payService",
                ID = "payService2",
                Port = 802,
                Tags = new[] { "v1" },
                Check = new CRL.Core.ConsulClient.CheckRegistrationInfo()
                {
                    HTTP = "http://pay.gsp2013.com/",
                    Interval = "10s",
                    DeregisterCriticalServiceAfter = "90m"
                }
            };
            var a=client.RegisterService(info);
            //a = client.RegisterService(info2);
            Console.WriteLine($"注册{info.Name} {a}");
            Console.ReadLine();

            var services = client.GetServiceInfo(info.Name);
            a = client.DeregisterService(info.ID);
            //a = client.DeregisterService(info2.ID);
            Console.WriteLine($"卸载{info.Name} {a}");
            //Console.ReadLine();
        }
        static void test23()
        {
            using (var consul = new Consul.ConsulClient(c =>
            {
                c.Address = new Uri("http://127.0.0.1:8500");
            }))
            {
                //取在Consul注册的全部服务
                var services = consul.Agent.Services().Result.Response;
                foreach (var s in services.Values)
                {
                    Console.WriteLine($"ID={s.ID},Service={s.Service},Addr={s.Address},Port={s.Port}");
                }
            }
        }
    }
}
