using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class TaskController : ApiController
    {
//        [HttpPost]
//        [Route("api/task")]
//        public object AddTask(object json)
//        {
//            return TaskBiz.AddTask(json);
//        }
        [HttpPost]
        [Route("api/task")]
        public object CreateTask(object json)
        {
            return TaskBiz.CreateTask(json);
        }

    }
}