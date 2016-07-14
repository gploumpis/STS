using STS.CrossService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace STS.RP.WebApi.Controllers
{

   
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class HomeController : ApiController
    {
        public const string AUTH_ROUTE_PREFIX = "api";

        [Route("users/current/claims", Name = "ListCurrentUsersClaims")]
        [HttpGet]
        public async Task<IHttpActionResult> GetClaims()
        {
            //var res = Request.CreateResponse(HttpStatusCode.OK, User as ClaimsPrincipal);
            //res.Headers.AddCookies(new List<CookieHeaderValue> { new CookieHeaderValue("test", "test") });
            //return ResponseMessage(res);



            //turn of certificate validation for dev
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            STSInfo info = new STSInfo
            {
               Issuer = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration.Issuer, 
               HomeRealm = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration.HomeRealm,
               IssuerRealm = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration.HomeRealm
            };

            ICredentials credentials = new NetworkCredential("CrossService", "polop0l0!", "training01.inner.relational.gr");
            //ICredentials credentials = CredentialCache.DefaultCredentials;

            using (var client = new HttpClient(new STSLoginMessageHandler(info, credentials)))
                {


                    //var byteArray = Encoding.ASCII.GetBytes("CrossService2:polop0l0!");
                    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiJGNzU2REFENC02RjM4LTQ5QzMtQkJGNS0zRUFCQjVGMEMyNjMiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZ2l2ZW5fbmFtZSI6Is6az47Pg8-EzrHPgiIsImZhbWlseV9uYW1lIjoizprPjs-Dz4TOsc-CIiwiY2xhaW0xIjoidmFsdWUxIiwiY2xhaW0yIjoidmFsdWUyIiwidGFza3NfYXV0aCI6IjAwMTAwMTAxMDExIiwiZGVfYXV0aCI6IjEwMDAxMDAxMDEiLCJUYXNrcyI6IjAwMDAwMDAwMDAwNDA0MDAiLCJEZWNpc2lvbiBFbmdpbmUiOiI3ZmZmZTgiLCJDb250ZW50IE1hbmFnZW1lbnQiOiIwMDAwMDAwMDAwMDAwMCIsIk9yZ2FuaXphdGlvbmFsIE1vZGVsIjoiMDAwMDAwMDAwMDgwMDAwMDAwIiwiVGVtcGxhdGVzIjoiNDAwMDAwMDAwMDAwMDAiLCJjYXNlc19hdXRoIjoiMTEwMDEwMDAxIiwiaWRfYXV0aCI6IjExMTEwMCIsInRlbXBsYXRlc19hdXRoIjoiMTAwMDEiLCJvcmdtb2RlbF9hdXRoIjoiMTAwMDEwMTAiLCJhcHBzZXR0aW5nc19hdXRoIjoiMTAwMDEwMTAxMDAxIiwiYnVzaW5lc3NfYXV0aCI6IjEwMDAxMCIsIkNhc2VTZXJ2aWNlIjoiZmZmZmZmZmZmZmZjIiwic2Rmc2QiOiJmZmZmZmZmZmZmZmMiLCJBcHBsaWNhdGlvbiBTZXR0aW5ncyI6IjQ4IiwiUkVmcmVzaFRPa2VuIjoiWkRJelpXUTJNMkV5WWpKa05HWTBZV0V6WVRjM1pHTTVOR1ppTnpVMU5EayIsImlzcyI6ImF0bGFzLmlkIiwiZXhwIjoyMTI2Njg2OTg2LCJuYmYiOjE0NjM5OTg5ODZ9.RdFQKRmtNvr6821Lnx4LyWf_775WFCRL3UGHBjGkksk");
                    var response = await client.GetAsync("https://training01.inner.relational.gr:444/STS.RP.WebApi1/api/users/current/claims");
                    return ResponseMessage(response);

                }

            

         //return Ok((User as ClaimsPrincipal).Claims);
        }

        [Route("login", Name = "PostLoginResult")]
        [HttpPost]
        public IHttpActionResult Login(string test)
        {
            //var res = Request.CreateResponse(HttpStatusCode.OK, User as ClaimsPrincipal);
            //res.Headers.AddCookies(new List<CookieHeaderValue> { new CookieHeaderValue("test", "test") });
            //return ResponseMessage(res);

            return Ok();
        }

    }
}
