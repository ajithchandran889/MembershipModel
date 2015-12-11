using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OneConnect
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
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            System.Web.HttpContext context = HttpContext.Current;
            System.Exception exc = context.Server.GetLastError();
            var msg = exc.Message.ToString();
            var stack = exc.StackTrace.ToString();

            log4net.ILog logger= log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Error("Global Exception : Something went wrong"+msg);
            logger.Error(stack); 
        }
    }
}
