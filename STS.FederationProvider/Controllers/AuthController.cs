using STS.Simple.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace STS.FederationProvider.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class AuthController : ApiController
    {
        public const string AUTH_ROUTE_PREFIX = "api";

        [Route("auth/login/{relyingPartyName}", Name = "Login")]
        [HttpGet]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> Login(string relyingPartyName)
        {

            if (string.IsNullOrWhiteSpace(relyingPartyName))
            {
                return BadRequest("No relying party id provided");
            }


            string action;
            NameValueCollection content = null;
            NameValueCollection qs = Request.RequestUri.ParseQueryString();
            action = qs.Get(WSFederationConstants.Parameters.Action);
            if (string.IsNullOrWhiteSpace(action))
            {
                content = await Request.Content.ReadAsFormDataAsync();
                action = content.Get(WSFederationConstants.Parameters.Action);
            }

            if (action == WSFederationConstants.Actions.SignIn)
            {
                IRelyingParty rp = STSConfiguration<RelyingParty>.Current.RelyingParties.FindByName(relyingPartyName);


                if (this.User != null && this.User.Identity.IsAuthenticated)
                {
                    if (content == null)
                        content = await Request.Content.ReadAsFormDataAsync();

                    WSFederationMessage responseMessageFromIssuer = WSFederationMessage.CreateFromNameValueCollection(Request.RequestUri, content);

                    var contextId = responseMessageFromIssuer.Context;

                    var ctxCookie = System.Web.HttpContext.Current.Request.Cookies[contextId];
                    if (ctxCookie == null)
                    {
                        throw new InvalidOperationException("Context cookie not found");
                    }

                    var originalRequestUri = new Uri(ctxCookie.Value);
                    HttpCookie cookie = DeleteContextCookie(contextId);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

                    var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(originalRequestUri);

                    var sts = new SimpleSts(rp.GetStsConfiguration());

                    SignInResponseMessage rm = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User as ClaimsPrincipal, sts);

                    //WSTrustSerializationContext context = new WSTrustSerializationContext(FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlerCollectionManager);
                    //WSFederationSerializer fedSer = new WSFederationSerializer();
                    //RequestSecurityTokenResponse token = fedSer.CreateResponse(rm, context);
                    //token.RequestedSecurityToken.SecurityToken.

                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(rm, System.Web.HttpContext.Current.Response);

                    
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    var contextId = Guid.NewGuid().ToString();
                    HttpCookie cookie = CreateContextCookie(contextId, HttpUtility.UrlDecode(this.Request.RequestUri.AbsoluteUri));
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

                    var message = new SignInRequestMessage(new Uri(rp.AuthenticationUrl), FederatedAuthentication.WSFederationAuthenticationModule.Realm)
                    {
                        CurrentTime = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + "Z",
                        HomeRealm = rp.Realm,
                        Context = contextId,
                        Reply = Url.Link("Login", new { relyingPartyName = relyingPartyName })
                    };

                    message.Parameters.Add(new KeyValuePair<string, string>("originalRequest", Request.RequestUri.ToString()));

                    return Redirect(message.RequestUrl);
                }


            }
            else {
                return BadRequest(String.Format(
                                  CultureInfo.InvariantCulture,
                                  "The action '{0}' (Request.QueryString['{1}']) is unexpected. Expected actions are: '{2}' or '{3}'.",
                                  String.IsNullOrEmpty(action) ? "<EMPTY>" : action,
                                  WSFederationConstants.Parameters.Action,
                                  WSFederationConstants.Actions.SignIn,
                                  WSFederationConstants.Actions.SignOut));
            }

           



           


        }



        [Route("users/current/claims", Name = "Claims")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult Claims()
        {
            //return Ok(User.Identity as ClaimsIdentity);

            return Ok((User as ClaimsPrincipal).Claims);
        }
       

        private HttpCookie CreateContextCookie(string contextId, string context)
        {
            var contextCookie = new HttpCookie(contextId, context)
            {
                Secure = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.RequireSsl,
                Path = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.Path,
                Domain = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.Domain,
                HttpOnly = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.HideFromClientScript
            };

            if (FederatedAuthentication.SessionAuthenticationModule.CookieHandler.PersistentSessionLifetime.HasValue &&
                (FederatedAuthentication.SessionAuthenticationModule.CookieHandler.PersistentSessionLifetime != TimeSpan.Zero))
            {
                contextCookie.Expires =
                    DateTime.UtcNow.Add(
                        FederatedAuthentication.SessionAuthenticationModule.CookieHandler.PersistentSessionLifetime.Value);
            }

            return contextCookie;
        }

        private HttpCookie DeleteContextCookie(string contextId)
        {
            var contextCookie = new HttpCookie(contextId)
            {
                Secure = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.RequireSsl,
                Path = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.Path,
                Domain = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.Domain,
                HttpOnly = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.HideFromClientScript,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            return contextCookie;

        }

    }



}
