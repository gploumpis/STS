using STS.Core;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace STS.IP.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class AuthController : ApiController
    {
        IIPManager manager;
        public AuthController(IIPManager manager)
        {
            this.manager = manager;
        }

        public const string AUTH_ROUTE_PREFIX = "api/auth";

        [Route("integrated/{id}", Name = "LoginWindowsIntegratedAuthentication")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Login(string id)
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

            SignInResponseMessage res =   manager.ProcessSignInRequest(rp, Request.RequestUri, User as ClaimsPrincipal);
            manager.ProcessSignInResponse(res, System.Web.HttpContext.Current.Response);
            
            return StatusCode(HttpStatusCode.NoContent);
            
            
            
            //string respContent = res.WriteFormPost();
            //var resp = Request.CreateResponse();
            //resp.Content = new StringContent(respContent, Encoding.UTF8, "text/html");

            //return ResponseMessage(resp);
        }

    }

   
}
