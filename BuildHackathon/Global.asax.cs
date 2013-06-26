using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

namespace BuildHackathon
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            var connectionString = "Endpoint=sb://buildhackathon.servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue=8LFP/KKPxhqrmjPUMYKiMRQnCoccHQkXD1mp4WoCF/g=";
            GlobalHost.DependencyResolver.UseServiceBus(connectionString, "BuildHackathon");
            RouteTable.Routes.MapHubs("/signalr", new HubConfiguration { EnableDetailedErrors = true });
        }
    }
}