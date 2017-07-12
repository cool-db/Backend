using System;
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
        public object GetCommentList(int taskId, String token)
        {
            return TaskBiz.GetCommentList(taskId, token);
        }
        
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
        [Route("api/task/list")]
        public object GetTaskList(int projectId, int userId)
        {
            return TaskBiz.GetTaskList(projectId, userId);
        }
        
        [HttpGet]
        [Route("api/task/item")]
        public object GetInfo(int projectId, int taskId)
        {
            return TaskBiz.GetInfo(projectId, taskId);
        }
        
        [HttpPut]
        [Route("api/task/item")]
        public object UpdateInfo(object json)
        {
            return TaskBiz.UpdateInfo(json);
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
    }
}