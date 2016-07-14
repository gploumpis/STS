using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace STS.RP.WebApi1.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class HomeController : ApiController
    {
        public const string AUTH_ROUTE_PREFIX = "api";

        [Route("users/current/claims", Name = "ListCurrentUsersClaims")]
        [HttpGet]
        public IHttpActionResult GetClaims()
        {
            //var res = Request.CreateResponse(HttpStatusCode.OK, User as ClaimsPrincipal);
            //res.Headers.AddCookies(new List<CookieHeaderValue> { new CookieHeaderValue("test", "test") });
            //return ResponseMessage(res);
           
            return Ok((User as ClaimsPrincipal).Claims);
        }

        [Route("echo", Name = "Echo")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult Echo()
        {
            return Ok();
        }


    }
}
