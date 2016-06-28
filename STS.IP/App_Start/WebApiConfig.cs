using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace STS.IP
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            Bootstrapper.StartDependencies(config);
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();



            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
