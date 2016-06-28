using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace STS.Core
{
    public interface IIPManager
    {
        SignInResponseMessage ProcessSignInRequest(IRelyingParty rp, Uri baseUri, ClaimsPrincipal principal);
        void ProcessSignInResponse(SignInResponseMessage signInResponseMessage, HttpResponse httpResponse);
    }
}
