using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ComEx
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BundleTable.EnableOptimizations = true;

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            CultureInfo ci = new CultureInfo("es-CO");

            ci.NumberFormat.NumberDecimalSeparator = ",";
            ci.NumberFormat.NumberGroupSeparator = ".";

            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
        }
    }
}
