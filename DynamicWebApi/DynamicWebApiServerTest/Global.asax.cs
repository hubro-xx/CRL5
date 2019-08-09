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
            //server.SetTokenCheck((u, t) =>
            //{
            //    return u == "user" && t == "123";
            //});
            server.Register<ITestService, TestService>();
        }

    }
}