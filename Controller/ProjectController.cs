using System.Web.Http;
using Backend.Biz;
using Backend.Model;

namespace Backend.Controller
{
    public class ProjectController : ApiController
    {
        #region project

        [HttpPost]
        [Route("api/project")]
        public object CreateProject(object json)
        {
            return ProjectBiz.CreateProject(json);
        }

        [HttpDelete]
        [Route("api/project")]
        public object DeleteProject(object json)
        {
            return ProjectBiz.DeleteProject(json);
        }

        [HttpGet]
        [Route("api/project")]
        public object GetInfo(int projectId)
        {
            return ProjectBiz.GetInfo(projectId);
        }

        [HttpPut]
        [Route("api/project")]
        public object UpdateInfo(object json)
        {
            return ProjectBiz.UpdateInfo(json);
        }

        [HttpPut]
        [Route("api/project/owner")]
        public object ChangeOwner(object json)
        {
            return ProjectBiz.ChangeOwner(json);
        }

        [HttpGet]
        [Route("api/project/list")]
        public object GetList(int ownerId, string ownerToken)
        {
            return ProjectBiz.GetList(ownerId, ownerToken);
        }

        #endregion

        #region project member

        [HttpPost]
        [Route("api/project/member")]
        public object AddMember(object json)
        {
            return ProjectBiz.AddMember(json);
        }

        [HttpDelete]
        [Route("api/project/member")]
        public object DeleteMember(object json)
        {
            return ProjectBiz.DeleteMember(json);
        }

        [HttpGet]
        [Route("api/project/member/list")]
        public object MemberList(int projectId)
        {
            return ProjectBiz.MemberList(projectId);
        }

        #endregion

        #region permission

        [HttpGet]
        [Route("api/project/permission")]
        public object GetPermission(int userId, int projectId)
        {
            return ProjectBiz.GetPermission(userId, projectId);
        }

        [HttpPut]
        [Route("api/project/permission")]
        public object UpdatePermission(object json)
        {
            return ProjectBiz.UpdatePermission(json);
        }
        
        #endregion

        #region progress

        [HttpPost]
        [Route("api/project/progress")]
        public object CreateProgress(object json)
        {
            return ProjectBiz.CreateProgress(json);
        }
        
        [HttpDelete]
        [Route("api/project/progress")]
        public object DeleteProgress(object json)
        {
            return ProjectBiz.DeleteProgress(json);
        }
        
        [HttpPut]
        [Route("api/project/progress/name")]
        public object UpdateProgressName(object json)
        {
            return ProjectBiz.UpdateProgressName(json);
        }
        
        [HttpPut]
        [Route("api/project/progress/order")]
        public object UpdateProgressOrder(object json)
        {
            return ProjectBiz.UpdateProgressOrder(json);
        }
        
        [HttpGet]
        [Route("api/project/progress/list")]
        public object GetProgressList(int projectId)
        {
            return ProjectBiz.GetProgressList(projectId);
        }
        

        #endregion
    }
}