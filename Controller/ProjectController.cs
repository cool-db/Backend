using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class ProjectController : ApiController
    {
        #region project

        [HttpPost]
        [Route("api/project")]
        public object CreateProjext(object json)
        {
            return ProjectBiz.CreateProjext(json);
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

        #endregion

    }
}