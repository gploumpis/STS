using STS.Simple.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;

namespace STS.CrossService.IP.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class CrossServiceIPController : ApiController
    {
        public const string AUTH_ROUTE_PREFIX = "api/auth";

     
        [Route("parties/{id}/tokens", Name = "CreateToken")]
        [HttpPost]
        public IHttpActionResult Tokens([FromUri]string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No relying party id provided");
            }

            IRelyingParty rp = STSConfiguration<RelyingParty>.Current.RelyingParties.FindByName(id);

            if (rp == null)
            {
                return BadRequest(string.Format("Relying party with id {0} was not found", id));
            }

            var sts = new SimpleSts(rp.GetStsConfiguration());
            var rMessage = rp.GetSignInRequestMessage(Request.RequestUri);


            
            //ClearAllCookies(); 

            SignInResponseMessage res = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(rMessage, GetPrincipal(), sts);


            
            //SecurityToken st = FederatedAuthentication.WSFederationAuthenticationModule.GetSecurityToken(res);

            //XmlReader reader = XmlReader.Create("addressdata.xml");
            //XmlDictionaryReader dictReader = XmlDictionaryReader.CreateDictionaryReader(reader);



            //string  s= FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(res);
            //FederatedPassiveSecurityTokenServiceOperations..ProcessSignInResponse(res, HttpContext.Current.Response);

            var response = Request.CreateResponse(HttpStatusCode.OK);

            NameValueCollection nvc = WSFederationMessage.ParseQueryString( new Uri(res.WriteQueryString()));
            
            response.Content = new FormUrlEncodedContent(nvc.AllKeys.Select(f=> new KeyValuePair<string,string>(f,nvc[f]) ));
            
            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/soap+xml");

            return ResponseMessage(response);
        }


        private ClaimsPrincipal GetPrincipal(IPrincipal user = null)
        {
            user = user ?? User;

            if(user.Identity.IsAuthenticated)
                return user as ClaimsPrincipal;

            
            ClaimsIdentity i = new ClaimsIdentity(
               new List<Claim>
                {
                    new Claim(ClaimTypes.Name,"CrossServiceUser" )
                },
               "simpleSts");

            ClaimsPrincipal p = new ClaimsPrincipal(i);
            return p;
        }


    }
}
