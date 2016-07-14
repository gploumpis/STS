using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace STS.BearerAuthenticationModule
{

    public class BearerAuthenticationModule : IHttpModule
    {
        public const String HttpAuthorizationHeader = "Authorization";  // HTTP1.1 Authorization header 
        public const String HttpBearerSchemeName = "Bearer"; // HTTP1.1 Basic Challenge Scheme Name 
        

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
        }

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            String authorizationHeader = context.Request.Headers[HttpAuthorizationHeader];

            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                return;

            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {

                String verifiedAuthorizationHeader = authorizationHeader.Trim();

                if (verifiedAuthorizationHeader.IndexOf(HttpBearerSchemeName) == 0)
                {
                    var splitted = verifiedAuthorizationHeader.Split(new char[] { ' ' });

                    if (splitted.Length == 2 && !string.IsNullOrWhiteSpace(splitted[1]))
                    {

                        var accessToken = Encoding.UTF8.GetString(Convert.FromBase64String(splitted[1]));
                        
                        
                        
                        var a = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
                        SecurityToken token = a.ReadToken(accessToken);
                        if (token != null) {
                            try
                            {
                                var identities = a.ValidateToken(token);
                                if (identities != null) {
                                    context.User = new ClaimsPrincipal(identities);
                                    Thread.CurrentPrincipal = context.User;
                                }
                                return;   

                            }
                            catch (Exception ex)
                            {
                                return;
                            }
                            

                        }
                                

                       

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
                    }

                }

            }



           
            // Create the user principal and associate it with the request 
            // 
            //context.User = new GenericPrincipal(new GenericIdentity(userName), null);
        }
    }
}
