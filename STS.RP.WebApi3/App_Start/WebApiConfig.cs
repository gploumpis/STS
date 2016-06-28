using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using Atlas.Web.Security;
using System.IdentityModel.Tokens;

namespace STS.RP.WebApi3
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
             var cors = new EnableCorsAttribute("*", "*", "*") { 
                SupportsCredentials = true
            };
            config.EnableCors(cors);



            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            // Web API configuration and services
            var a = new Atlas.Web.Security.JwtAuthenticationOptions("https://sts.windows.net/075af58f-3435-456a-95c4-5a4fd57e1a9d/", Encoding.ASCII.GetBytes("2450d716-7b47-4351-b47d-60fc1e67f277"));

        
            config.UseJwtBearerAuthentication(a);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}
