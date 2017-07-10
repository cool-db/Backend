using System;
using System.Collections.Generic;
using Backend.Model;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Backend.Biz
{
    public class ProjectBiz
    {
        #region project

        public static object CreateProject(object json)
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

                if (theProject.Users.Contains(theMember))
                    return Helper.Error(417, "该参与者已存在");

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

        public static object DeleteMember(object json)
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
                    return Helper.Error(404, "删除的用户不存在");

                var theMember = queryMember.Single();
                theProject.Users.Remove(theMember);
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

        public static object GetMemberList(int projectId)
        {
            using (var context = new BackendContext())
            {
                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");
                var theProject = queryProject.Single();

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

        public static object UpdatePermission(int userId, int projectId)
        {
            return null;
        }

        public static object CreateProgress(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            var userToken = body["userToken"];
            var progressName = body["progressName"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");
                var theUser = queryUser.Single();

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");
                Console.WriteLine(1);

                var newProgress = new Progress()
                {
                    Name = progressName,
                    ProjectId = projectId,
                    Order = (context.Progresses.Any()) ? context.Progresses.Max(progress => progress.Order) + 1 : 1,
                    OwnerId = userId
                };
                context.Progresses.Add(newProgress);
                context.SaveChanges();

                var progressList = Progress.GetProgerssList(projectId);

                return new
                {
                    progressList,
                    code = 200
                };
            }
        }

        public static object DeleteProgress(object json)
        {
            var body = Helper.Decode(json);
            var progressId = int.Parse(body["progressId"]);
            var userId = int.Parse(body["userId"]);
            var userToken = body["userToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");
                var theUser = queryUser.Single();

                var queryProgress = context.Progresses.Where(progress => progress.Id == progressId);
                if (!queryProgress.Any())
                    return Helper.Error(404, "进度不存在");

                var theProgress = queryProgress.Single();
                var theProject = context.Projects.Single(project => project.Id == theProgress.ProjectId);

                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");

                foreach (var progress in context.Progresses)
                {
                    if (progress.Order > theProgress.Order)
                        --progress.Order;
                }
                context.Progresses.Remove(theProgress);
                context.SaveChanges();

                var progressList = Progress.GetProgerssList(theProject.Id);

                return new
                {
                    progressList,
                    code = 200
                };
            }
        }

        public static object UpdateProgressName(object json)
        {
            var body = Helper.Decode(json);
            var progressId = int.Parse(body["progressId"]);
            var userId = int.Parse(body["userId"]);
            var userToken = body["userToken"];
            var progressName = body["progressName"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");
                var theUser = queryUser.Single();

                var queryProgress = context.Progresses.Where(progress => progress.Id == progressId);
                if (!queryProgress.Any())
                    return Helper.Error(404, "进度不存在");

                var theProgress = queryProgress.Single();
                var theProject = context.Projects.Single(project => project.Id == theProgress.ProjectId);

                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");

                if (string.IsNullOrWhiteSpace(progressName))
                    return Helper.Error(417, "名称为空");

                theProgress.Name = progressName;
                context.SaveChanges();

                var progressList = Progress.GetProgerssList(theProject.Id);

                return new
                {
                    progressList,
                    code = 200
                };
            }
        }

        public static object UpdateProgressOrder(object json)
        {
            var body = Helper.DecodeToObject(json.ToString());
            var projectId = int.Parse(body["projectId"].ToString());
            var userId = int.Parse(body["userId"].ToString());
            var userToken = body["userToken"].ToString();
            var progresses = JArray.Parse(body["progresses"].ToString());

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(417, "未找到该项目");

                var theUser = queryUser.Single();
                var theProject = queryProject.Single();

                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");

                if (progresses.Count == theProject.Progresses.Count)
                {
                    foreach (var inputProgress in progresses)
                    {
                        var theProgress =
                            theProject.Progresses.Single(
                                progress => progress.Id == int.Parse(inputProgress["progressId"].ToString()));
                        if (theProgress.Project.Id != projectId)
                            return Helper.Error(404, "不存在该进度");

                        theProgress.Order = int.Parse(inputProgress["order"].ToString());
                    }

                    context.SaveChanges();
                    var progressList = Progress.GetProgerssList(theProject.Id);

                    return new
                    {
                        progressList,
                        code = 200
                    };
                }
                return Helper.Error(417, "输入不合法");
            }
        }

        public static object GetProgressList(int projectId)
        {
            using (var context = new BackendContext())
            {
                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var progressList = Progress.GetProgerssList(projectId);

                return new
                {
                    progressList,
                    code = 200
                };
            }
        }
    }
}