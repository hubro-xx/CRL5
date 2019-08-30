using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DynamicWebApiServerTest
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var server = new CRL.DynamicWebApi.ApiServer();
            server.SetSessionManage(new CRL.Remoting.SessionManage());
            server.Register<ITestService, TestService>();
            var listener = new CRL.DynamicWebApi.ServerListener();
            listener.Start("http://localhost:809/");
        }

    }
}