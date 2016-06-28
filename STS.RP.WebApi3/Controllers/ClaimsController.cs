﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace STS.RP.WebApi3.Controllers
{

   
   
    [RoutePrefix(AUTH_ROUTE_PREFIX)]
    public class ClaimsController : ApiController
    {
         public const string AUTH_ROUTE_PREFIX = "api";

         [Route("claims", Name = "claims")]
         [HttpGet]
         [Authorize]
         public IHttpActionResult Claims()
         {

             var identity = User.Identity as ClaimsIdentity;

             return Ok(identity.Claims);

         }
    }
}