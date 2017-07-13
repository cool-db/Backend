using System.Web.Http;
using System.Web.Routing;
using Backend.Biz;

namespace Backend.Controller
{
    public class TaskController : ApiController
    {
        [HttpPost]
        [Route("api/task")]
        public object CreateTask(object json)
        {
            return TaskBiz.CreateTask(json);
        }

        [HttpDelete]
        [Route("api/task")]
        public object DeleteTask(object json)
        {
            return TaskBiz.DeleteTask(json);
        }

        [HttpGet]
        [Route("api/task")]
        public object GetInfo(int taskId)
        {
            return TaskBiz.GetInfo(taskId);
        }

        [HttpPut]
        [Route("api/task")]
        public object UpdateInfo(object json)
        {
            return TaskBiz.UpdateInfo(json);
        }


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


        [HttpGet]
        [Route("api/task/list")]
        public object GetTaskList(int projectId)
        {
            return TaskBiz.GetTaskList(projectId);
        }


        [HttpPut]
        [Route("api/task/state")]
        public object UpdateState(object json)
        {
            return TaskBiz.UpdateState(json);
        }

        [HttpPost]
        [Route("api/subtask")]
        public object CreateSubTask(object json)
        {
            return TaskBiz.CreateSubTask(json);
        }

        [HttpDelete]
        [Route("api/subtask")]
        public object DeleteSubTask(object json)
        {
            return TaskBiz.DeleteSubTask(json);
        }

        [HttpPut]
        [Route("api/subtask")]
        public object UpdateSubtaskInfo(object json)
        {
            return TaskBiz.UpdateSubtaskInfo(json);
        }

        [HttpPut]
        [Route("api/subtask/state")]
        public object UpdateSubtaskState(object json)
        {
            return TaskBiz.UpdateSubtaskState(json);
        }

        [HttpGet]
        [Route("api/subtask/list")]
        public object GetSubtaskList(int subtaskId)
        {
            return TaskBiz.GetSubtaskList(subtaskId);
        }

        [HttpPost]
        [Route("api/task/participator")]
        public object AddMember(object json)
        {
            return TaskBiz.AddMember(json);
        }

        [HttpDelete]
        [Route("api/task/participator")]
        public object DeleteMember(object json)
        {
            return TaskBiz.DeleteMember(json);
        }

        [HttpGet]
        [Route("api/task/participator/list")]
        public object GetMemberList(int taskId)
        {
            return TaskBiz.GetMemberList(taskId);
        }

        [HttpPut]
        [Route("api/task/order")]
        public object ChangeProgress(object json)
        {
            return TaskBiz.ChangeProgress(json);
        }
        
        [HttpPost]
        [Route("api/task/attachment")]
        public object AddAttachment(object json)
        {
            return TaskBiz.AddAttachment(json);
        }
        
        [HttpDelete]
        [Route("api/task/attachment")]
        public object DeleteAttachment(object json)
        {
            return TaskBiz.DeleteAttachment(json);
        }
    }
}