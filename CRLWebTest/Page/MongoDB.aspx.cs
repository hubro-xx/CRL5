/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL;
using CRLTest.Code;
using WebTest.Code;

namespace WebTest.Page
{
    public partial class MongoDB : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var instance = MongoDBTestManage.Instance;
            var count = instance.Count(b => b.Id > 0);
            Response.Write("数据总量" + count);
        }
        void TestMongoDB()
        {
            //依赖官方驱动MongoDB.Driver
            //MongoDBTest.Test();
            //return;
            var instance = MongoDBTestManage.Instance;
            //插入
            instance.Add(new MongoDBModel2() { OrderId = "1212", Status = DateTime.Now.Second });
            //函数Count
            int count = instance.Count(b => b.Status >= 0);
            var query = instance.GetLambdaQuery();
            query.Where(b => b.Status > 10);
            var result3 = query.ToList();//返回List<MongoDBModel>
            //group
            query.GroupBy(b => b.OrderId).Select(b => new { count = b.Status.SUM(), count2 = b.Status.COUNT() });
            var list = query.ToDynamic();
            foreach (var item in list)
            {
                var a = item.count;
                var key = item.OrderId;
            }
            //标准查询
            var query2 = instance.GetLambdaQuery();
            query2.Select(b => new { aa = b.Id, bb = b.Status });
            //query2.Where(b=>b.Status.In(1,2,3,4));
            var result = query2.ToDictionary<int, int>();//返回字典
            var result2 = query2.ToDynamic();//返回动态对象

            //删除
            instance.Delete(b => b.Status == 111);
            //更新
            var item2 = instance.QueryItem(b => b.Status > 0);
            item2.Status = 123;
            instance.Update(item2);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            TestMongoDB();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var instance = MongoDBTestManage.Instance;
            var max = instance.GetLambdaQuery().OrderBy(b => b.Id, true).Take(1).ToSingle();
            var list = new List<MongoDBModel2>();
            for (int i = max.Id; i < max.Id+1000; i++)
            {
                var obj = new MongoDBModel2() { OrderId = "1212", Status = DateTime.Now.Second, name = "MongoDBModel" + i };
                list.Add(obj);
                //instance.Add(obj);
            }
        
            instance.BatchInsert(list);
            Response.Write("初始数据");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var instance = MongoDBTestManage.Instance;
            var count = instance.Count(b => b.Id > 0);
            Response.Write("数据总量"+count);
            var time = DateTime.Now;
            var query = instance.QueryItem(b => b.name == "MongoDBModel123");
            var ts = DateTime.Now - time;
            Response.Write($" 查询用时{ts.TotalMilliseconds}");
        }
    }
}
