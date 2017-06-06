using System.Web.Http;
using Backend.Biz;
using Backend.Model;


namespace Backend.Controller
{
    public class ScheduleController : ApiController
    {
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