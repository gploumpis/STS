using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace STS.Simple.Core
{
    public class SimpleSts : SecurityTokenService
    {
        public SimpleSts(SimpleStsConfiguration configuration)
            : base(configuration)
        {
            // Ignore certificate errors as we will use our selfsigned ones and it will take too long to validate 
            // the certificate path.
            //SecurityTokenServiceConfiguration.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
            //SecurityTokenServiceConfiguration.CertificateValidator = new IgnoreCertificateErrorsValidator()


            SecurityTokenServiceConfiguration.DefaultTokenType = configuration.RelyingParty.TokenType;
        }

        SimpleStsConfiguration getConfiguration()
        {
            return SecurityTokenServiceConfiguration as SimpleStsConfiguration;
        }
        IRelyingParty getRelyingParty()
        {
            var conf = getConfiguration();
            if (conf == null)
                return null;

            return conf.RelyingParty;
        }



        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            if (request == null || request.AppliesTo == null)
            {
                string name = principal == null ? null : principal.Identity == null ? null : principal.Identity.Name;
                throw new InvalidRequestException(string.Format("token request from {0} - but no realm specified.", principal.Identity.Name));
            }

            var rp = getRelyingParty();
            var realm = rp == null ? null : rp.Realm;
            //if ( !string.Equals( realm, request.AppliesTo.Uri.ToString(), StringComparison.InvariantCultureIgnoreCase) )
            //{
            //    throw new InvalidRequestException(string.Format("The AppliesTo uri {0} is not registered as a relying party.", request.AppliesTo.Uri));
            //}

            var scope = new RequestScope(request.AppliesTo.Uri, rp);

            scope.ReplyToAddress = rp.RedirectUrl ;

            request.TokenType = rp.TokenType;

            return scope;
        }

        protected override Lifetime GetTokenLifetime(Lifetime requestLifetime)
        {
            var scope = Scope as RequestScope;
            if (scope == null) throw new ApplicationException("No STS request scope found!");
            if (scope.RelyingParty.TokenLifeTime > 0)
            {
                return new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(scope.RelyingParty.TokenLifeTime));
            }
            else
            {
                return base.GetTokenLifetime(requestLifetime);
            }
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");

            ClaimsIdentity userIdentity = principal.Identities.FirstOrDefault();
            if (userIdentity == null)
                throw new Exception("User Identity not found.");

            var claimTypes = new List<string> { ClaimTypes.AuthenticationInstant, ClaimTypes.AuthenticationMethod };

            var inheritedClaims =
                userIdentity.Claims.Where(i => !claimTypes.Contains(i.Type))
                .Select(c => new Claim(c.Type, c.Value));

            var relyingParty = (scope as RequestScope).RelyingParty;

            var outputIdentity = new ClaimsIdentity(relyingParty.IssuerName);
            // We also have the ClaimTypes.AuthenticationMethod that shows which was the original authenticator
            outputIdentity.AddClaim(new Claim(ClaimTypes.AuthenticationInstant,
                DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                ClaimValueTypes.DateTime, relyingParty.IssuerName));
            outputIdentity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod,
                relyingParty.AuthenticationUrl,
                ClaimValueTypes.String, relyingParty.IssuerName));

            outputIdentity.AddClaims(inheritedClaims);

            return outputIdentity;
        }

    }

}
