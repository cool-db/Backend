using System;
using System.Web.Http;
using Backend.Sample.Biz;

namespace Backend.Sample.Controller
{
    public class SampleController : ApiController
    {
        [HttpGet]
        public Object Get()
        {
            return SampleBiz.GetFromSampleDb();
        }
    }
}