using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using Atlas.Web.Security;
using System.Configuration;
using System.ServiceModel.Security.Tokens;
using System.Security.Claims;

namespace STS.MixedAuthenticationModule
{
    public class MixedAuthenticationModule : IHttpModule
    {
        public const String HttpAuthorizationHeader = "Authorization";  // HTTP1.1 Authorization header 
        public const String HttpBearerSchemeName = "Bearer"; // HTTP1.1 Basic Challenge Scheme Name 
        //public const Char HttpCredentialSeparator = ':'; // HTTP1.1 Credential username and password separator 
        public const int HttpNotAuthorizedStatusCode = 401; // HTTP1.1 Not authorized response status code 
        public const String HttpWWWAuthenticateHeader = "WWW-Authenticate"; // HTTP1.1 Basic Challenge Scheme Name 
        public const String Realm = "demo"; // HTTP.1.1 Basic Challenge Realm 
       

        public String ModuleName
        {
            get { return "MixedAuthenticationModule"; }
        }

        public void Dispose()
        {
           
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += context_AuthenticateRequest;
            context.EndRequest += context_EndRequest;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            
        }

        static TokenValidationParameters CreateTokenValidationParameters(JwtAuthenticationOptions options)
        {
            options = options ?? JwtAuthenticationOptions.Load(ConfigurationManager.AppSettings);

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = options.IssuerName,
                IssuerSigningToken = new BinarySecretSecurityToken(options.PresharedKey),
                AudienceValidator = delegate { return true; }
            };
            return validationParameters;
        }

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            String authorizationHeader = context.Request.Headers[HttpAuthorizationHeader];

            if (context.User != null && context.User.Identity!=null && context.User.Identity.IsAuthenticated)
                return;

            if (!string.IsNullOrWhiteSpace(authorizationHeader)) {

                String verifiedAuthorizationHeader = authorizationHeader.Trim();

                if (verifiedAuthorizationHeader.IndexOf(HttpBearerSchemeName) == 0)
                {
                    var splitted = verifiedAuthorizationHeader.Split(new char[] { ' ' });

                    if (splitted.Length == 2 && !string.IsNullOrWhiteSpace(splitted[1])) {

                        var accessToken = splitted[1];

                        //var a = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;

                        //var configuredHandler = a.Where(f => f.CanValidateToken && f.TokenType == typeof(JwtSecurityToken)).FirstOrDefault();
                        //if (configuredHandler != null)
                        //{
                        //    try
                        //    {

                        //        var identities = configuredHandler.ValidateToken(new JwtSecurityToken(accessToken));
                        //        context.User = new ClaimsPrincipal(identities);
                        //        return;
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        return;
                        //    }

                        //}

                        var handler = new JwtSecurityTokenHandler();

                        SecurityToken validatedToken = null;
                        IPrincipal principal = null;

                        try
                        {
                            principal = handler.ValidateToken(accessToken, CreateTokenValidationParameters(null), out validatedToken);

                            context.User = principal;
                        }
                        catch (Exception ex)
                        {
                            var a = 1;
                            // Just don't setup the principal if token validation fails
                        }
                    }

                }

            }


            
           //if (context.User.Identity.IsAuthenticated && context.User.Identity.AuthenticationType.Equals(FederatedAuthentication.WSFederationAuthenticationModule.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            //{
             
            //}
            // 
            // Create the user principal and associate it with the request 
            // 
            //context.User = new GenericPrincipal(new GenericIdentity(userName), null);
        }
    }
}
