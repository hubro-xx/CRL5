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

namespace WebTest
{
    public partial class Insert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var count = ProductDataManage.Instance.Count(b => b.Id > 0);
            //Response.Write(count);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var item = new ProductData() { InterFaceUser = "2222", ProductName = "product2"+DateTime.Now.Second, BarCode = "1212122",UserId=1,Number=10 };
            item.Object2 = DateTime.Now;
            ProductDataManage.Instance.Add(item,false);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var list = new List<ProductData>();
            for (int i = 1; i < 10; i++)
            {
                list.Add(new ProductData() { InterFaceUser = "2222", ProductName = "product" + i, BarCode = "code" + i,UserId=1,Number=i });
            }
            ProductDataManage.Instance.BatchInsert(list);
        }
    }
}
