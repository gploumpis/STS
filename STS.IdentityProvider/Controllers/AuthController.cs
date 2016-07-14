using STS.Simple;
using STS.Simple.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace STS.IdentityProvider.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class AuthController : ApiController
    {
        public const string AUTH_ROUTE_PREFIX = "api/auth";

        [Route("mock/{id}", Name = "LoginUsingMockPrincipal")]
        [HttpGet]
        public IHttpActionResult LoginMock(string id)
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

            
            ClaimsPrincipal principal = GetMockPrincipalPrincipal(GetMockUser(rMessage));

            //ClearAllCookies(); 

            SignInResponseMessage res = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(rMessage, principal, sts);

            FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(res, HttpContext.Current.Response);

            return StatusCode(HttpStatusCode.NoContent);
        }

       private string GetMockUser(SignInRequestMessage message){
           try
           {
               string context = message.Parameters[WSFederationConstants.Parameters.Context];
               NameValueCollection nvc = HttpUtility.ParseQueryString(context, System.Text.Encoding.UTF8);
               var requestUrl = nvc.Get("ru");
               Uri u = new Uri(Request.RequestUri, HttpUtility.UrlDecode(requestUrl));
               return u.ParseQueryString()["user"];



           }
           catch (Exception e) {
               return null;
           }


       }

        private ClaimsPrincipal GetMockPrincipalPrincipal(string user=null)
        {

            ClaimsIdentity i = new ClaimsIdentity(
               new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user?? "MockUser" )
                },
               "simpleSts");

            ClaimsPrincipal p = new ClaimsPrincipal(i);
            return p;
        }

        private void ClearAllCookies()
        {

            foreach (var cookieName in HttpContext.Current.Request.Cookies.Cast<string>().ToArray())
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(cookieName) { Expires = new DateTime(1990, 1, 1) });
            }
        }

    }
}
