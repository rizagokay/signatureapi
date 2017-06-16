using Sharpbrake.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Mechsoft.ESign.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {

            Exception exception = Server.GetLastError();

            var airbrake = new AirbrakeNotifier(new AirbrakeConfig
            {
                ProjectId = "146734",
                ProjectKey = "6b2293ec486cbbea517b945202e7c7fc"
            });

            airbrake.NotifyAsync(exception);

            Server.ClearError();
        }
    }
}
