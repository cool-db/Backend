using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Backend.Model;

namespace Backend.Sample.Controller
{
    public class SampleAuthController : ApiController
    {
        [HttpPost]
        public object Get(object json)
        {
            return true;
        }
    }
}


