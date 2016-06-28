using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace STS.Core
{
    public static class ConfigurationExtensions
    {

        public static X509Certificate2 GetCertificate(this CertificateReferenceElement reference)
        {
            if (reference != null && reference.ElementInformation.IsPresent)
            {
                return CertificateHelper.GetCertificate(
                    reference.StoreName,
                    reference.StoreLocation,
                    reference.X509FindType,
                    reference.FindValue);
            }

            return null;
        }

        public static X509SigningCredentials GetSigningCredentials(this CertificateReferenceElement reference)
        {
            if (reference == null) return null;

            var cert = reference.GetCertificate();
            if (cert == null) return null;
            return new X509SigningCredentials(cert);
        }

        public static X509Certificate2 GetCertificate(this EmbeddedCertificate config)
        {

            return CertificateHelper.GetCertificate(config.AssemblyName, config.ResourceName, config.Password);
        }

        public static X509SigningCredentials GetSigningCredentials(this IRelyingParty rp)
        {
            var cert = rp.GetSigningCertificate();
            if (cert == null) return null;
            return new X509SigningCredentials(cert);
        }

        public static X509EncryptingCredentials GetEncryptingCredentials(this IRelyingParty rp)
        {
            var cert = rp.GetEncryptingCertificate();
            if (cert == null) return null;
            return new X509EncryptingCredentials(cert);
        }


        public static SimpleStsConfiguration GetStsConfiguration(this IRelyingParty rp)
        {
            return SimpleStsConfiguration.ForRelyingParty(rp);
        }

        public static SignInRequestMessage GetSignInRequestMessage(this IRelyingParty rp, Uri baseUri)
        {
            var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(baseUri);
            requestMessage.Realm = requestMessage.Realm ?? rp.Realm;
            return requestMessage;
        }

    }
}
