using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.IdentityModel.Services;

namespace STS.RP.WebApi1
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FederatedAuthentication.FederationConfigurationCreated += FederatedAuthentication_FederationConfigurationCreated;
        }

        void FederatedAuthentication_FederationConfigurationCreated(object sender, System.IdentityModel.Services.Configuration.FederationConfigurationCreatedEventArgs e)
        {
           
            FederatedAuthentication.WSFederationAuthenticationModule.SignInError += WSFederationAuthenticationModule_SignInError;
            FederatedAuthentication.WSFederationAuthenticationModule.AuthorizationFailed += WSFederationAuthenticationModule_AuthorizationFailed;
            FederatedAuthentication.SessionAuthenticationModule.SessionSecurityTokenReceived += SessionAuthenticationModule_SessionSecurityTokenReceived;
        
        }

        void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        {
            var a = e.SessionToken;
        }

        void WSFederationAuthenticationModule_SignInError(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void WSFederationAuthenticationModule_AuthorizationFailed(object sender, AuthorizationFailedEventArgs e)
        {
            var a = e;
        }
    }
}
