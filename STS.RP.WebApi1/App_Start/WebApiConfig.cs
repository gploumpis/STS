using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace STS.RP.WebApi1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            ConfigureCors(config);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static void ConfigureCors(HttpConfiguration httpConfiguration)
        {
            var attr = new EnableCorsAttribute("*", "*", "*") { SupportsCredentials = true };

            httpConfiguration.EnableCors(attr);
        }
    }
}
