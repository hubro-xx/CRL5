using CRL.Core.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using CRL.DynamicWebApi;
namespace DynamicWebApiServerTest
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Type type = typeof(TestService);
            var server = new ServerCreater().CreatetApi();
            server.SetSessionManage(new SessionManage());
            server.Register<ITestService, TestService>();
            var listener = new ServerListener();
            listener.Start("http://localhost:809/");
        }

    }
}