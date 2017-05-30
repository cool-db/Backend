using System.Collections.Generic;
using Backend.Model;
using System.Linq;


namespace Backend.Biz
{
    public class ProjectBiz
    {
        #region project

        public static object CreateProjext(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var ownerId = int.Parse(body["ownerId"]);
                var ownerToken = body["ownerToken"];
                var projectName = body["projectName"];
                var projectDescription = body["projectDescription"];

                var query = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!query.Any())
                    return Helper.Error(401, "token错误");

                var newProject = new Project
                {
                    Name = projectName,
                    Description = projectDescription,
                    OwnerId = ownerId
                };
                newProject.Users.Add(query.Single());
                context.Projects.Add(newProject);
                context.SaveChanges();

                return new
                {
                    projectId = newProject.Id,
                    projectName = newProject.Name,
                    projectDescription = newProject.Description,
                    code = 200
                };
            }
        }

        public static object DeleteProject(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            var ownerToken = body["ownerToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                context.Projects.Remove(theProject);
                context.SaveChanges();

                return new
                {
                    code = 200
                };
            }
        }

        public static object ChangeOwner(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            var ownerIdTo = int.Parse(body["ownerIdTo"]);
            var ownerToken = body["ownerToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theProject.OwnerId = ownerIdTo;
                context.SaveChanges();

                return new
                {
                    projectId = theProject.Id,
                    ownerId = theProject.OwnerId,
                    code = 200
                };
            }
        }

        public static object GetInfo(int projectId)
        {
            using (var context = new BackendContext())
            {
                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();

                return new
                {
                    projectId = theProject.Id,
                    projectName = theProject.Name,
                    projectDescription = theProject.Description,
                    code = 200
                };
            }
        }

        public static object GetList(int ownerId, string ownerToken)
        {
            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var theUser = queryUser.Single();

                var projects = new List<object>();
                foreach (var project in theUser.Projects)
                {
                    projects.Add(new
                    {
                        id = project.Id,
                        name = project.Name,
                        description = project.Description
                    });
                }

                return new
                {
                    projects,
                    code = 200
                };
            }
        }

        public static object UpdateInfo(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            var ownerToken = body["ownerToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theProject.Name = (body.ContainsKey("projectName")) ? body["projectName"] : theProject.Name;
                theProject.Description = (body.ContainsKey("projectDiscription"))
                    ? body["projectDiscription"]
                    : theProject.Description;
                context.SaveChanges();

                return new
                {
                    projectId = theProject.Id,
                    projectName = theProject.Name,
                    projectDiscription = theProject.Description,
                    code = 200
                };
            }
        }

        #endregion


        public static object AddMember(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            var ownerToken = body["ownerToken"];
            var memberId = int.Parse(body["memberId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                var queryMember = context.Users.Where(member => member.Id == memberId);
                if (!queryMember.Any())
                    return Helper.Error(404, "添加的用户不存在");

                var theMember = queryMember.Single();
                theProject.Users.Add(theMember);
                context.SaveChanges();

                var members = new List<object>();
                foreach (var member in theProject.Users)
                {
                    members.Add(new
                    {
                        id = member.Id,
                        name = member.UserInfo.Name
                    });
                }

                return new
                {
                    members,
                    code = 200
                };
            }
        }
    }
}