using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel;
using System.IdentityModel.Services;
using System.Globalization;
using System.Security.Claims;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using STS.Simple.Core;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Net;
using System.ServiceModel.Security;
using System.ServiceModel;

namespace STS.CrossService
{
    //public static class SecurityTokenExtensions {

    //    /// <summary>
    //    /// Converts a supported token to an XML string.
    //    /// </summary>
    //    /// <param name="token">The token.</param>
    //    /// <returns>The token XML string.</returns>
    //    public static string ToTokenXmlString(this SecurityToken token)
    //    {
    //        var genericToken = token as GenericXmlSecurityToken;
    //        if (genericToken != null)
    //        {
    //            return genericToken.ToTokenXmlString();
    //        }

    //        var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
    //        return token.ToTokenXmlString(handler);
    //    }

    //    /// <summary>
    //    /// Converts a supported token to an XML string.
    //    /// </summary>
    //    /// <param name="token">The token.</param>
    //    /// <param name="handler">The token handler.</param>
    //    /// <returns>The token XML string.</returns>
    //    public static string ToTokenXmlString(this SecurityToken token, SecurityTokenHandlerCollection handler)
    //    {
    //        if (handler.CanWriteToken(token))
    //        {
    //            var sb = new StringBuilder(128);
    //            handler.WriteToken(new XmlTextWriter(new StringWriter(sb)), token);
    //            return sb.ToString();
    //        }
    //        else
    //        {
    //            throw new InvalidOperationException("Token type not suppoted");
    //        }
    //    }
    
    //}

    public class STSInfo {

        public string Issuer { get; set; }
        public string IssuerRealm { get; set; }
        public string HomeRealm { get; set; }
    }
    
    public class STSLoginMessageHandler : DelegatingHandler
    {
        string userName;
        string password;
        ICredentials credentials;
        STSInfo info;


        public STSLoginMessageHandler(STSInfo info, ICredentials credentials)
        {
            this.InnerHandler = this.InnerHandler ?? new HttpClientHandler();
            this.info = info;
            this.credentials = credentials?? CredentialCache.DefaultCredentials;
        }

        public STSLoginMessageHandler(STSInfo info, string userName, string password, string domain = null)
            : this(info, new NetworkCredential(userName, password,domain))
        {
            this.userName = userName;
            this.password = password;
        }



        //private ClaimsPrincipal GetPrincipal(string userName, string password)
        //{
        //    IEnumerable<Claim> claims = new List<Claim>{ new Claim(ClaimTypes.Name, userName)};
            
        //    ClaimsIdentity i = new ClaimsIdentity(claims, "CrossServiceSTS");

        //    ClaimsPrincipal p = new ClaimsPrincipal(i);
        //    return p;
        //}

        private SignInRequestMessage GetSignInRequestMessage()
        {


            SignInRequestMessage signInRequestMessage = new SignInRequestMessage(new Uri(info.Issuer), info.IssuerRealm)
            {
                CurrentTime = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + "Z",
                HomeRealm = info.HomeRealm
            };

            return signInRequestMessage;
        }

        
        //private async Task<AuthenticationHeaderValue> GetAuthenticationHeaderValue(Uri baseUri, System.Threading.CancellationToken cancellationToken)
        //{

        //    SignInResponseMessage rm = await GetSignInResponseMessage(baseUri, cancellationToken);

        //    string xml = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(rm);

        //    var toEncode = Encoding.UTF8.GetBytes(xml);
        //    var encoded = Convert.ToBase64String(toEncode);

        //    return new AuthenticationHeaderValue("SAML", encoded);
        //}

        //private static string GetIdentityToken()
        //{
        //    var factory = new WSTrustChannelFactory(
        //        new WindowsWSTrustBinding(SecurityMode.Transport),new EndpointAddress()
        //        _idpEndpoint);
        //    factory.TrustVersion = TrustVersion.WSTrust13;

        //    var rst = new RequestSecurityToken
        //    {
        //        RequestType = RequestTypes.Issue,
        //        KeyType = KeyTypes.Bearer,
        //        AppliesTo = new EndpointAddress(Constants.Realm)
        //    };

        //    var token = factory.CreateChannel().Issue(rst) as GenericXmlSecurityToken;
        //    return token.TokenXml.OuterXml;
        //}
       
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
          
                        TokenIssuer ti = new TokenIssuer();
                       var token = ti.Issue("cs", "https://training01.inner.relational.gr:444/", "testUser","test Id");
                       request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                       return  await base.SendAsync(request, cancellationToken);
               /*         
            
            SignInRequestMessage signInRequestMessage = GetSignInRequestMessage();

             using (var handler = new HttpClientHandler { Credentials = credentials })
             {
                 using (var client = new HttpClient(handler))
                 {

                     var response = await client.PostAsync(signInRequestMessage.RequestUrl, new StringContent("", Encoding.UTF8), cancellationToken);

                     if (response.IsSuccessStatusCode)
                     {
                         NameValueCollection data = await response.Content.ReadAsFormDataAsync();

                         SignInResponseMessage signInResponseMessage = (SignInResponseMessage) WSFederationMessage.CreateFromNameValueCollection(request.RequestUri, data);

                         var xml = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);
                         //XmlDocument doc = new XmlDocument();
                         //doc.LoadXml(xml);
                         //var token = Encoding.UTF8.GetString(Convert.FromBase64String(doc.InnerText));
                         //var token = Encoding.UTF8.GetString(Convert.FromBase64String(xml));

                         var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));

                         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
                  
                     }
                     else
                     {
                         if (response.StatusCode == HttpStatusCode.Unauthorized)
                             return response;

                         throw new Exception(await response.Content.ReadAsStringAsync());
                     }
                 }

             }


             var resp = await base.SendAsync(request, cancellationToken);


             return resp;

             */

            //SecurityToken securityToken = FederatedAuthentication.WSFederationAuthenticationModule.GetSecurityToken(signInResponseMessage);
           
 
            
            //string xml = securityToken.ToTokenXmlString();
            ////string xml = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);
            //var toEncode = Encoding.UTF8.GetBytes(xml);
            
            //var encoded = Convert.ToBase64String(toEncode);

            //SessionSecurityToken sst =  FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken( GetPrincipal(userName,password),"",DateTime.UtcNow,DateTime.UtcNow.AddDays(1),true);
            //FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sst);
            //var cs = System.Web.HttpContext.Current.Request.Cookies.Get(1);

            //cookies.Add(request.RequestUri, new Cookie(cs.Name, cs.Value, cs.Path, cs.Domain) { Secure= cs.Secure, HttpOnly = cs.HttpOnly, Expires = cs.Expires });

            //NameValueCollection nvc = WSFederationMessage.ParseQueryString(new Uri(signInResponseMessage.WriteQueryString()));

            
            //    using (var client = new HttpClient())
            //    {
            //        var authResp = await client.PostAsync(request.RequestUri, new FormUrlEncodedContent(nvc.AllKeys.Select(f => new KeyValuePair<string, string>(f, nvc[f]))));
            //        IEnumerable<Cookie> responseCookies = cookies.GetCookies(request.RequestUri).Cast<Cookie>();
            //    }

            
            

            


            //request.RequestUri = new Uri( signInResponseMessage.WriteQueryString());

            //NameValueCollection nvc = WSFederationMessage.ParseQueryString(new Uri(signInResponseMessage.WriteQueryString()));

            //foreach (var k in nvc.AllKeys) {
            //    request.Headers.Add(k, Convert.ToBase64String(Encoding.UTF8.GetBytes(nvc[k])));
            //}

            //var xml = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);

            //var toEncode = Encoding.UTF8.GetBytes(xml);

            //var encoded = Convert.ToBase64String(toEncode);
            //request.Headers.Authorization = new AuthenticationHeaderValue("SAML", encoded);

            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xml);

            
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", doc.InnerText);
            //request.RequestUri = new Uri(signInResponseMessage.WriteQueryString());

            
            

            //SecurityToken securityToken = FederatedAuthentication.WSFederationAuthenticationModule.GetSecurityToken(signInResponseMessage);
            //string xml = securityToken.ToTokenXmlString();
            ////string xml = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);
            //var toEncode = Encoding.UTF8.GetBytes(xml);
            
            //var encoded = Convert.ToBase64String(toEncode);
            //request.Headers.Authorization = new AuthenticationHeaderValue("SAML", encoded);
            //SessionSecurityToken sessionSecurityToken = FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(GetPrincipal(userName,password),"",DateTime.UtcNow,DateTime.UtcNow.AddDays(1),false);
            //FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionSecurityToken);
            

            //request.RequestUri = new Uri( signInResponseMessage.WriteQueryString());

            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            //using (var client = new HttpClient(handler))
            //{
            //    string xml = FederatedAuthentication.WSFederationAuthenticationModule.GetXmlTokenFromMessage(signInResponseMessage);

            //    var toEncode = Encoding.UTF8.GetBytes(xml);
            //    var encoded = Convert.ToBase64String(toEncode);


            //    cookieContainer.Add(request.RequestUri, new Cookie("CookieName", encoded));

            //    return await client.SendAsync(request, cancellationToken);
            //}

           //request.Headers.Authorization =  await  GetAuthenticationHeaderValue(request.RequestUri, cancellationToken);
            //FederatedAuthentication.WSFederationAuthenticationModule.GetSecurityToken()



            //SecurityToken securityToken = FederatedAuthentication.WSFederationAuthenticationModule.GetSecurityToken(signInResponseMessage);
            //ClaimsPrincipal principal = GetPrincipal(userName, password);
            //SessionSecurityToken st = FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(principal, "", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), false);
            //FederatedAuthentication.SessionAuthenticationModule.AuthenticateSessionSecurityToken(st,false);
            //FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(st);
            //FederatedAuthentication.WSFederationAuthenticationModule.
       

          

            //

            //var c = request.Headers.GetCookies(cookieName).FirstOrDefault();      
           
            //var c = new CookieHeaderValue(cookieName, actualToken);
            //c.Expires = DateTime.Now.AddDays(1);
            //c.Domain = request.RequestUri.Host;
            //c.Path = "/";
            //c.Secure = true;
            //c.HttpOnly = true;
            
            //request.Headers.AddCookies();
            
            //response.Headers.Location = uri.Uri;

           
        }
    }


    //using (Stream content = await response.Content.ReadAsStreamAsync())
    //{

    //    WSTrust13ResponseSerializer ser = new WSTrust13ResponseSerializer();
    //    WSTrustSerializationContext context = new WSTrustSerializationContext(FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlerCollectionManager);
    //    using (XmlReader reader = new XmlTextReader(content))
    //    {
    //        RequestSecurityTokenResponse str = ser.ReadXml(reader, context);
    //        //SecurityToken  st= FederatedAuthentication.WSFederationAuthenticationModule.GetSecurityToken(str);                

    //    }
    //}

}
