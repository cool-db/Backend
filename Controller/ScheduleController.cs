using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class ScheduleController : ApiController
    {
        [HttpPost]
        [Route("api/schedule/")]
        public object CreateSchedule(object json)
        {
            return ScheduleBiz.CreateSchedule(json);
        }
        
        [HttpDelete]
        [Route("api/schedule/")]
        public object DeleteSchedule(object json)
        {
            return ScheduleBiz.DeleteSchedule(json);
        }
        
        [HttpPut]
        [Route("api/schedule/")]
        public object UpdateSchedule(object json)
        {
            return ScheduleBiz.UpdateSchedule(json);
        }
    }
}