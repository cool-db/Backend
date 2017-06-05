using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class TaskController : ApiController
    {
        [HttpPost]
        [Route("api/task/comment/")]
        public object AddComment(object json)
        {
            return TaskBiz.AddComment(json);
        }
        
        [HttpDelete]
        [Route("api/task/comment/")]
        public object DeleteComment(object json)
        {
            return TaskBiz.DeleteComment(json);
        }
        
        [HttpGet]
        [Route("api/task/comment/list")]
        public object GetCommentList(int taskId)
        {
            return TaskBiz.GetCommentList(taskId);
        }

    }
}