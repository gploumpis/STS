using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace STS.Core
{
    class AtlasClaimsTransformer : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            return base.Authenticate(resourceName, incomingPrincipal);

            var outputIdentity = new ClaimsIdentity();

            if (null == incomingPrincipal)
            {
                throw new InvalidRequestException("The caller's principal is null.");
            }

            var inputIdentity = incomingPrincipal.Identity as ClaimsIdentity;

           outputIdentity.AddClaims(inputIdentity.Claims);
           outputIdentity.AddClaim(new Claim(ClaimTypes.Surname, "Ploumpis"));


           return new ClaimsPrincipal(outputIdentity);
    
            //return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}
