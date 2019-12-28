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

namespace WebTest
{
    public partial class Delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ProductDataManage.Instance.Delete(b => b.Id == 0);//按条件删除
            ProductDataManage.Instance.Delete(1);//按主键删除
            //使用完整语法删除 goup语法不支持
            var query = ProductDataManage.Instance.GetLambdaQuery();
            query.Where(b => b.Id == 10);
            query.Join<Member>((a, b) => a.SupplierId == "10" && b.Name == "123");
            ProductDataManage.Instance.Delete(query);
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {

        }
    }
}
