using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STS.Simple.Core
{
    public class MyX509CertificateStoreTokenResolver : X509CertificateStoreTokenResolver
    {
        public MyX509CertificateStoreTokenResolver()
            : base(System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine)
        {
        }
    }
}
