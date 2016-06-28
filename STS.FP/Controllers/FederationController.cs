using STS.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;

namespace STS.FP.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class FederationController : ApiController
    {
        
        public const string AUTH_ROUTE_PREFIX = "api/federation";

        
        [Route("{id}", Name = "FederatedAuthenticationAuthentication")]
        [HttpGet]
        [HttpPost]
        public async Task<IHttpActionResult> Login(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No relying party id provided");
            }


            string action ;
            NameValueCollection content=null; 
            NameValueCollection qs = Request.RequestUri.ParseQueryString();
            action = qs.Get(WSFederationConstants.Parameters.Action);
            if(string.IsNullOrWhiteSpace(action)){
                content = await Request.Content.ReadAsFormDataAsync();
                 action = content.Get(WSFederationConstants.Parameters.Action);
            }


            if (action == WSFederationConstants.Actions.SignIn)
            {
                IRelyingParty rp = STSConfiguration<RelyingParty>.Current.RelyingParties.FindByName(id);
                
                if (rp == null)
                {
                    return BadRequest(string.Format("Relying party with id {0} was not found", id));
                }

                if (this.User != null && this.User.Identity.IsAuthenticated)
                {
                    //HandleSignInResponse
                    if(content==null)
                         content = await Request.Content.ReadAsFormDataAsync();
                    //var uri = Request.RequestUri;
                    //UriBuilder ub = new UriBuilder(uri);

                    //UriBuilder b = new UriBuilder(ub.Scheme, ub.Host, ub.Port, ub.Path);

                    var responseMessage = WSFederationMessage.CreateFromNameValueCollection(Request.RequestUri, content);

                    var contextId = responseMessage.Context;

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
                  
                    //var rMessage = rp.GetSignInRequestMessage(baseUri);

                    //SecurityTokenService sts =
                    //    new FederationSecurityTokenService(FederationSecurityTokenServiceConfiguration.Current);

                    SignInResponseMessage rm =
                        FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User as ClaimsPrincipal, sts);
                    
                    
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(rm, System.Web.HttpContext.Current.Response);

                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    //HandleSignInRequest
                    

                    // always pass it to IP


                    string reply = this.Request.RequestUri.AbsoluteUri.Remove(this.Request.RequestUri.AbsoluteUri.IndexOf(this.Request.RequestUri.Query, StringComparison.OrdinalIgnoreCase));


                    var contextId = Guid.NewGuid().ToString();
                    HttpCookie cookie = CreateContextCookie(contextId, this.Request.RequestUri.AbsoluteUri);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

                    var message = new SignInRequestMessage(new Uri(ConfigurationManager.AppSettings["IssuerLocation"]+id), FederatedAuthentication.WSFederationAuthenticationModule.Realm)
                    {
                        CurrentTime = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + "Z",
                        Reply = reply,
                        Context = contextId,
                        
                        HomeRealm = rp.Realm
                    };
                    //message.WriteQueryString

                    WSTrustSerializationContext context =new WSTrustSerializationContext(FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlerCollectionManager );
                    WSFederationSerializer fedSer = new WSFederationSerializer();
                    var token = fedSer.CreateRequest(message, context);
                    WSTrust13RequestSerializer trustSer = new WSTrust13RequestSerializer();
                    
                    using(var ms = new MemoryStream()){
                    
                        using(XmlWriter w = XmlWriter.Create(ms)){
                            trustSer.WriteKnownRequestElement(token, w, context);

                            using (var client = new HttpClient()) {

                                ms.Position = 0;
                                using (var sr = new StreamReader(ms))
                                {
                                    var myStr = sr.ReadToEnd();
                                }
                                var postContent = new StreamContent(ms);
                                postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/soap+xml");
                                await client.PostAsync(message.RequestUrl, postContent);
                            }
                        }
                    }

                    
                    
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
