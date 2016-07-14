using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace STS.RP.WebApi
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
            //Anything that's needed right after succesful session and before hitting the application code goes here

            FederatedAuthentication.WSFederationAuthenticationModule.SignedIn += WSFederationAuthenticationModule_SignedIn;

            FederatedAuthentication.WSFederationAuthenticationModule.RedirectingToIdentityProvider += WSFederationAuthenticationModule_RedirectingToIdentityProvider;
        }

        
        void WSFederationAuthenticationModule_RedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e)
        {
            // keep original request deep path etc.
            //e.SignInRequestMessage = new SignInRequestMessage(new Uri(issuer), realm, Request.Url.AbsoluteUri);
            //signInMessage.Context = originalUrl;
        }

        void WSFederationAuthenticationModule_SignedIn(object sender, EventArgs e)
        {

            var signedIn = true;
            // redirect to deep link
            //var request = new HttpContextWrapper(HttpContext.Current).Request;
            //if (FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(request))
            //{
            //    var signInResponse = FederatedAuthentication.WSFederationAuthenticationModule.GetSignInResponseMessage(request);
            //    HttpContext.Current.Response.Redirect(signInResponse.Context);
            //}
        }
    }
}
