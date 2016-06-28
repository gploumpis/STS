using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STS.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;

namespace STS.RP.WebApi2.Controllers
{
     [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class AuthController : ApiController
    {
         public const string AUTH_ROUTE_PREFIX = "api";


         [Route("auth", Name = "auth")]
         [HttpPost]
         [Authorize]
         public async Task<IHttpActionResult> Ahthorize()
         {

             NameValueCollection data = await Request.Content.ReadAsFormDataAsync();

             var result =  data.Get(WSFederationConstants.Parameters.Result);
              
            if (result.Contains('%'))
            {
                result = HttpUtility.UrlDecode(result);
            }


             using(XmlReader r = new XmlTextReader( new StringReader(result)))
             {
             WSTrust13ResponseSerializer	ser = new WSTrust13ResponseSerializer();
             
                RequestSecurityTokenResponse resp =  ser.ReadXml(r,new WSTrustSerializationContext(FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlerCollectionManager ));

                var bearerValue = resp.RequestedSecurityToken.SecurityTokenXml.InnerText;
             
                 var rm = Request.CreateResponse(HttpStatusCode.Found);
                 rm.Headers.Add("Authorization","Bearer "+ bearerValue);
                 rm.Headers.Location = new Uri(this.Url.Link("test", new { }));
                 return ResponseMessage(rm);
                 
                 //return Redirect(this.Url.Link("test", new { }));

                 
             }


             //JsonConvert.DeserializeObject<System.Collections.Generic.IEnumerable<RequestSecurityTokenResponse>>()
             //System.IdentityModel.
             
             
         }
        
    }
}
