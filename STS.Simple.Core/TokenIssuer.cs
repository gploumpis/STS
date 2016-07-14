using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace STS.Simple.Core
{
    public class TokenIssuer
    {

        public string Issue(string relayingPartyName, string realm, string userName, string userId )
        {


            if (string.IsNullOrWhiteSpace(relayingPartyName))
                throw new ArgumentNullException("relayingPartyName");

            if (string.IsNullOrWhiteSpace(realm))
                throw new ArgumentNullException("realm");

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId");

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, userName), new Claim(ClaimTypes.Sid, userId) }));
            
            // Signin message for cross service -> issuer realm == home realm == issuer url
            SignInRequestMessage signInRequestMessage = new SignInRequestMessage(new Uri(realm), realm)
            {
                CurrentTime = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + "Z",
                HomeRealm = realm
            };

            IRelyingParty rp = STSConfiguration<RelyingParty>.Current.RelyingParties.FindByName(relayingPartyName);

            if (rp == null)
                throw new ConfigurationErrorsException(string.Format("Relying party with name {0} was not found", relayingPartyName));

            if(FederatedAuthentication.WSFederationAuthenticationModule==null)
                throw new ConfigurationErrorsException("WSFederationAuthenticationModule was not found");

            var sts = new SimpleSts(rp.GetStsConfiguration());
            
            SignInResponseMessage signInResponseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(signInRequestMessage, principal, sts);



            string tokenXml = signInResponseMessage.Result;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(tokenXml);

            string xmlSecurityToken = xml.DocumentElement.GetElementsByTagName("trust:RequestedSecurityToken").Item(0).InnerXml; // <Assertion>



            //WSFederationSerializer ser = new WSFederationSerializer();
           
            //GenericXmlSecurityToken xmlt = new GenericXmlSecurityToken()
            
            
            //string xmlSecurityToken = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);

            var xmlSecurityTokenBase64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlSecurityToken));

            return xmlSecurityTokenBase64Encoded;
           
        }



        //public SignInResponseMessage Issue(string relayingPartyName, SignInRequestMessage signInRequestMessage, ClaimsPrincipal principal)
        //{

        //    if (string.IsNullOrWhiteSpace(relayingPartyName))
        //        throw new ArgumentNullException("relayingPartyName");

        //    if(signInRequestMessage == null)
        //        throw new ArgumentNullException("signInRequestMessage");

        //    if (principal == null)
        //        throw new ArgumentNullException("principal");


        //    IRelyingParty rp = STSConfiguration<RelyingParty>.Current.RelyingParties.FindByName(relayingPartyName);

        //    if (rp == null)
        //        throw new ConfigurationErrorsException(string.Format("Relying party with name {0} was not found", relayingPartyName));

        //    if(FederatedAuthentication.WSFederationAuthenticationModule==null)
        //        throw new ConfigurationErrorsException("WSFederationAuthenticationModule was not found");

        //    var sts = new SimpleSts(rp.GetStsConfiguration());
        //    SignInResponseMessage signInResponseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(signInRequestMessage, principal, sts);
            
        //    string xmlSecurityToken = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);

        //    var xmlSecurityTokenBase64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlSecurityToken));

        //    return signInResponseMessage;
           
        //}
    }
}
