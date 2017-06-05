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
        
    }
}