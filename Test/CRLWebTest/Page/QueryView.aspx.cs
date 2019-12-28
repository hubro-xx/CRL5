/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRLTest.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class ViewQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var query = ProductDataManage.Instance.GetLambdaQuery();
            var query2 = query.CreateQuery<Order>();
            //返回匿名结果
            var result1 = query.Select(b => new { id = b.Id, name = b.CategoryName }).ToList();

            //关联一个子查询
            var viewJoin = query2.Where(b => b.Id > 10).Select(b => b);
            var result2 = query.Join(viewJoin, (a, b) => a.UserId == b.UserId).Select((a, b) => new { a.CategoryName, b.OrderId }).ToList();

            //联合查询
            var view1 = query.Select(b => new { a1 = b.Id, a2 = b.ProductName });
            var view2 = query2.Select(b => new { a1 = b.Id, a2 = b.Remark });
            var result3 = view1.Union(view2).OrderBy(b => b.a1).OrderBy(b => b.a2, false).ToList();

            //按IN查询
            var view3 = query2.Where(b => b.Remark == "123").Select(b => b.Id);
            query.In(view3, b => b.UserId);
        }
    }
}
