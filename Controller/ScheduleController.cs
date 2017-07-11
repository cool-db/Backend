using System.Web.Http;
using Backend.Biz;
using Backend.Model;

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
        
        [HttpGet]
        [Route("api/schedule/")]
        public object GetSchedule(int scheduleId)
        {
            return ScheduleBiz.GetSchedule(scheduleId);
        }
        
        [HttpGet]
        [Route("api/schedule/list/")]
        public object GetScheduleList(int userId)
        {
            return ScheduleBiz.GetScheduleList(userId);
        }
        
        [HttpPost]
        [Route("api/schedule/participator")]
        public object AddParticipator(object json)
        {
            return ScheduleBiz.AddParticipator(json);
        }
        
        [HttpDelete]
        [Route("api/schedule/participator")]
        public object DeleteParticipator(object json)
        {
            return ScheduleBiz.DeleteParticipator(json);
        }
        
        [HttpGet]
        [Route("api/schedule/participator/list")]
        public object GetParticipatorList(int scheduleId)
        {
            return ScheduleBiz.GetParticipatorList(scheduleId);
        }
    }
}