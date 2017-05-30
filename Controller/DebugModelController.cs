using System;
using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class DebugModelController : ApiController
    {
        [HttpPost]
        public object DebugModel(object json)
        {
            return DebugModelBiz.DebugModel(json);
        }
    }
}