using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL;
namespace CRLTest.Code
{
    [CRL.Attribute.Table(TableName = "MongoDBModel3")]
    public class MongoDBModel2 : CRL.IModelBase
    {
       
        public string OrderId
        {
            get;
            set;
        }
        public int Numbrer
        {
            get;
            set;
        }
        public string name
        {
            get; set;
        }
    }
    public class MongoResult
    {
        public int sum
        {
            get; set;
        }
        public int count
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public string orderId
        {
            get; set;
        }
    }
    public class MongoDBTestManage : CRL.BaseProvider<MongoDBModel2>
    {
        public override string ManageName => "mongo";
        public static MongoDBTestManage Instance
        {
            get { return new MongoDBTestManage(); }
        }
        public void  GetInitData()
        {
            var list = new List<MongoDBModel2>();
            list.Add(new MongoDBModel2() { name = "test1", Numbrer = 1, OrderId="11" });
            list.Add(new MongoDBModel2() { name = "test2", Numbrer = 2, OrderId = "12" });
            list.Add(new MongoDBModel2() { name = "test3", Numbrer = 3, OrderId = "13" });
            BatchInsert(list);
        }
        public void GroupTest()
        {
            //Delete(b=>b.Numbrer>0);
            //GetInitData();
            var query = GetLambdaQuery();
            query.Where(b => b.Numbrer > 0);
            var result = query.GroupBy(b => new { b.name, b.OrderId }).Select(b => new
            {
                count = b.Id.COUNT(),
                number=b.Numbrer.SUM(),
                //orderId = b.OrderId.MAX(),
                name = b.name,
                orderId = b.OrderId
            }).HavingCount(b => b.count > 1).ToList();
            //var result = query.ToList<MongoResult>();
        }
    }
}
