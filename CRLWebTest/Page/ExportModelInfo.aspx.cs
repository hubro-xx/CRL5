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

namespace WebTest
{
    public partial class ExportModelInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Type[] types = new Type[] { typeof(Code.ModelTest) };
            //var xmlFiles = new List<string> { Server.MapPath(TextBox1.Text) };
            var str = CRL.SummaryAnalysis.ExportToFile(types);
            Response.Write(str);
            Response.End();
        }
    }
}
