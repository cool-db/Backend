using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Backend.Sample.Biz;

namespace Backend.Sample.Controller
{
    public class SampleDataController : ApiController
    {
        [HttpGet]
        public object Get()
        {
            return SampleDataBiz.GetFromSampleDb();
        }
    }
}