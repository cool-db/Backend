using System;
using System.Web.Http;
using System.Web.Routing;
using Backend.Sample.Biz;

namespace Backend.Sample.Controller
{
    public class SampleController : ApiController
    {
        [HttpGet]
        public object Get()
        {
            return SampleBiz.GetFromSampleDb();
        }
    }
}