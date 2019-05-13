/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SqlSugar;
using CRL.DBAccess;

namespace TestConsole
{
    class Program
    {
         [STAThread]
        static void Main()
        {
            //var fuc = CRL.Base.CreateObjectTest<Test2>();
            //var data = new object[] { "333",1};
            //var dataContainer = new DataContainer(data);
            //var obj2 = fuc(dataContainer);

            CRL.SettingConfig.AutoTrackingModel = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CRL.SettingConfig.GetDbAccess = (dbLocation) =>
            {
                return new CRL.DBAccessBuild(DBType.MSSQL, TestConsole.DbHelper.ConnectionString);
            };

            //var s2 = new CRL.ListenTestServer(1437);
            //s2.Start();

            Application.Run(new MainForm());
        }
 
    }

}
