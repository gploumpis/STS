using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace STS.RP.WebApi1.Controllers
{
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class ClaimsController : ApiController
    {
        public const string AUTH_ROUTE_PREFIX = "api/claims";

        [Route("", Name = "ListClaims")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Claims()
        {

           var identity = User.Identity as ClaimsIdentity;

           return Ok(identity.Claims);

        } 
    }


    //public static class ClaimHelper
    //{
    //    public static Claim GetClaimFromIdentity(IIdentity identity, string claimType)
    //    {
    //        if (identity == null)
    //        {
    //            throw new ArgumentNullException("identity");
    //        }

    //        var claimsIdentity = identity as ClaimsIdentity;

    //        if (claimsIdentity == null)
    //        {
    //            throw new ArgumentException("Cannot convert identity to IClaimsIdentity", "identity");
    //        }

    //        return claimsIdentity.Claims.SingleOrDefault(c => c.Type == claimType);
    //    }

    //    public static Claim GetClaimsFromPrincipal(IPrincipal principal, string claimType)
    //    {
    //        if (principal == null)
    //        {
    //            throw new ArgumentNullException("principal");
    //        }

    //        var claimsPrincipal = principal as ClaimsPrincipal;

    //        if (claimsPrincipal == null)
    //        {
    //            throw new ArgumentException("Cannot convert principal to IClaimsPrincipal.", "principal");
    //        }

    //        return GetClaimFromIdentity(claimsPrincipal.Identity, claimType);
    //    }

    //    public static Claim GetCurrentUserClaim(string claimType)
    //    {
    //        return GetClaimsFromPrincipal(Thread.CurrentPrincipal, claimType);
    //    }

    //    public static string GetCurrentUserClaimValue(string claimType)
    //    {
    //        Claim claim = GetCurrentUserClaim(claimType);
    //        if (claim != null)
    //        {
    //            return claim.Value;
    //        }
    //        return string.Empty;
    //    }
    //}
}
