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
    }
}