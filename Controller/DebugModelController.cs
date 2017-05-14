using System;
using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class DebugModelController : ApiController
    {
        [HttpGet]
        public Object Get()
        {
            return DebugModelBiz.DebugModel();
        }
    }
}