using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STS.Simple.Core
{
    public class RequestScope : Scope
    {
        public IRelyingParty RelyingParty { get; private set; }

        public RequestScope(Uri uri, IRelyingParty rp) :
            base(uri.ToString(), rp.GetSigningCredentials())
        {
            RelyingParty = rp;
            EncryptingCredentials = rp.GetEncryptingCredentials();
            if (EncryptingCredentials != null)
            {
                TokenEncryptionRequired = true;
                SymmetricKeyEncryptionRequired = true;
            }
            else
            {
                TokenEncryptionRequired = false;
                SymmetricKeyEncryptionRequired = false;
            }
        }

    }
}
