using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace STS.Core
{
    public class AtlasX509CertificateStoreTokenResolver : X509CertificateStoreTokenResolver
    {
        public AtlasX509CertificateStoreTokenResolver()
            : base(System.Security.Cryptography.X509Certificates.StoreName.My , StoreLocation.LocalMachine)
        {
        }

        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            return base.TryResolveSecurityKeyCore(keyIdentifierClause, out key);
        }
    }
}
