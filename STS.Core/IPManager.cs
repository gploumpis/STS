using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace STS.Core
{
    public class IPManager:IIPManager
    {

        public IPManager() { 
        
        }


        public SignInResponseMessage ProcessSignInRequest(IRelyingParty rp, Uri baseUri, ClaimsPrincipal principal)
        {
            var sts = new SimpleSts(rp.GetStsConfiguration());
            var rMessage = rp.GetSignInRequestMessage(baseUri);
            
             //FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse()
            return FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(rMessage, principal, sts);
        }

        public void ProcessSignInResponse( SignInResponseMessage signInResponseMessage,HttpResponse httpResponse)
        {
            FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(signInResponseMessage, httpResponse);
        }

        
    }
}
