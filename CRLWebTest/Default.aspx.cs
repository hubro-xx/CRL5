/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL;
using CRLTest.Code;
using WebTest.Code;
namespace WebTest
{
    public partial class Default : System.Web.UI.Page
    {
        int id = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            //var result = ProductDataManage.Instance.QueryItem(1);

            //testThread();
            //return;
            var query2 = ProductDataManage.Instance.GetLambdaQuery();
            query2.Where(b => b.Id == 10);
            query2.Join<Member>((a, b) => a.SupplierId == "10" && b.Name == "123");
            //ProductDataManage.Instance.Delete(query2);

            //Response.Write(sql);
            //Response.End();
        }
        void testThread()
        {
            for (int i = 0; i < 10; i++)
            {
                var thread = new System.Threading.Thread(b =>
                {
                    ProductDataManage.Instance.QueryItemFromCache(2);
                });
                thread.Start();
            }
        }
        class testA
        {
            public string CategoryName
            {
                get;set;
            }
            public string ProductName
            {
                get;set;
            }
        }
    }
}
