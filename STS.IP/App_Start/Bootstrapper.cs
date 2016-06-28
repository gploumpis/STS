using Autofac;
using Autofac.Integration.WebApi;
using STS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace STS.IP
{
    public static class Bootstrapper
    {
        public static void StartDependencies(HttpConfiguration httpConfiguration){

            var builder = new Autofac.ContainerBuilder();

            builder.RegisterType<IPManager>().As<IIPManager>().InstancePerRequest();

            //controllers
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(STS.IP.Controllers.AuthController)));


            IContainer container = builder.Build();
            // Set Web API Resolver
            var resolver = new AutofacWebApiDependencyResolver(container);

            httpConfiguration.DependencyResolver = resolver;

        }
    }
}