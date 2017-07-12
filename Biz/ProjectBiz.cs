using System;
using System.Collections.Generic;
using Backend.Model;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace Backend.Biz
{
    public class ProjectBiz
    {
        public static void RecordOperation(int userId, int projectId , string content)
        {
            using (var context = new BackendContext())
            {
                context.ProjectOperations.Add(new ProjectOperation()
                {
                    Content = content,
                    ProjectId = projectId,
                    UserId = userId,
                    Time = DateTime.Now
                });
                context.SaveChanges();
            }
            
        }
        
        
        #region project

        public static object CreateProject(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var ownerId = int.Parse(body["ownerId"]);
                //var ownerToken = body["ownerToken"];
                var projectName = body["projectName"];
                var projectDescription = body["projectDescription"];

                var query = context.Users.Where(user => user.Id == ownerId /*&& user.Token == ownerToken*/);
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
                
                context.UserPermissons.Add(new UserPermisson()
                {
                    UserId = ownerId,
                    ProjectId = newProject.Id,
                    Project = newProject,
                    Permission = Permission.Creator
                });
                
                context.SaveChanges();

                RecordOperation(ownerId, newProject.Id, "创建新项目");

                var data = new
                {
                    projectId = newProject.Id,
                    projectName = newProject.Name,
                    projectDescription = newProject.Description,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteProject(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            //var ownerToken = body["ownerToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId /*&& user.Token == ownerToken*/);
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

                var data = new
                {
                };
                return Helper.BuildResult(data);
            }
        }

        public static object ChangeOwner(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            var ownerIdTo = int.Parse(body["ownerIdTo"]);
            //var ownerToken = body["ownerToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId /*&& user.Token == ownerToken*/);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                //todo 组内移交
                theProject.OwnerId = ownerIdTo;
                context.SaveChanges();

                RecordOperation(ownerId, theProject.Id, "移交项目");

                var data = new
                {
                    projectId = theProject.Id,
                    ownerId = theProject.OwnerId,
                };
                return Helper.BuildResult(data);
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

                var data = new
                {
                    projectId = theProject.Id,
                    projectName = theProject.Name,
                    projectDescription = theProject.Description,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object GetList(int ownerId)
        {
            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId );
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

                var data = new
                {
                    projects,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object UpdateInfo(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            //var ownerToken = body["ownerToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId /*&& user.Token == ownerToken*/);
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

                RecordOperation(ownerId, projectId, "更新项目信息");

                var data = new
                {
                    projectId = theProject.Id,
                    projectName = theProject.Name,
                    projectDiscription = theProject.Description,
                };
                return Helper.BuildResult(data);
            }
        }

        #endregion


        public static object AddMember(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            //var ownerToken = body["ownerToken"];
            var memberId = int.Parse(body["memberId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId /*&& user.Token == ownerToken*/);
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
                if (!theProject.Users.Contains(theMember))
                {
                    theProject.Users.Add(theMember);
                    theProject.UserPermissons.Add(new UserPermisson()
                    {
                        UserId = theMember.Id,
                        ProjectId = theProject.Id,
                        Project = theProject,
                        User =  theMember,
                        Permission = Permission.Participant
                    });
                    context.SaveChanges();
                    
                    RecordOperation(ownerId,projectId,"增加新成员");
                }

                var members = (from theProjectUser in theProject.Users
                    select new
                    {
                        theProjectUser.Id,
                        theProjectUser.UserInfo.Name
                    }).ToArray();

                var data = new
                {
                    members
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteMember(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            //var ownerToken = body["ownerToken"];
            var memberId = int.Parse(body["memberId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId /*&& user.Token == ownerToken*/);
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
                //todo
                theProject.UserPermissons.Remove(theProject.UserPermissons.Where(p => p.UserId == theMember.Id).Single());
                
                context.SaveChanges();
                
                RecordOperation(ownerId,projectId,"删除新成员");


                var members = new List<object>();
                foreach (var member in theProject.Users)
                {
                    members.Add(new
                    {
                        id = member.Id,
                        name = member.UserInfo.Name
                    });
                }

                var data = new
                {
                    members,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object MemberList(int projectId)
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

                var data = new
                {
                    members,
                };
                return Helper.BuildResult(data);
            }
        }

        //todo userId 权限验证
        public static object UpdatePermission(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            //var memberId = int.Parse(body[""])
            var permission = int.Parse(body["permission"]);

            using (var context = new BackendContext())
            {
                var queryPermission = context.UserPermissons.Where(userPermisson =>
                    userPermisson.UserId == userId && userPermisson.ProjectId == projectId);
                if (!queryPermission.Any())
                    return Helper.Error(404, "未配置权限");
                queryPermission.Single().Permission = (Permission) permission;
                context.SaveChanges();
                
                RecordOperation(userId,projectId,"提升权限");

                
                var data = new
                {
                    permission
                };
                return Helper.BuildResult(data);
            }
        }

        public static object CreateProgress(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            //var userToken = body["userToken"];
            var progressName = body["progressName"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId /*&& user.Token == userToken*/);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");
                var theUser = queryUser.Single();

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");
                

                var newProgress = new Progress()
                {
                    Name = progressName,
                    ProjectId = projectId,
                    Order = theProject.Progresses.Count + 1,
                    OwnerId = userId,
                    Project = theProject
                };
                context.Progresses.Add(newProgress);
                context.SaveChanges();
                
                RecordOperation(userId,projectId,"新建进程");


                var progressList = Progress.GetProgerssList(projectId);

                var data = new
                {
                    progressList,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteProgress(object json)
        {
            var body = Helper.Decode(json);
            var progressId = int.Parse(body["progressId"]);
            var userId = int.Parse(body["userId"]);
            //var userToken = body["userToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId /*&& user.Token == userToken*/);
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

                var data = new
                {
                    progressList,
                };
                return Helper.BuildResult(data);
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

                var data = new
                {
                    progressList,
                };
                return Helper.BuildResult(data);
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

                    var data = new
                    {
                        progressList,
                    };
                    return Helper.BuildResult(data);
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

                var data = new
                {
                    progressList,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object GetPermission(int userId, int projectId)
        {
            using (var context = new BackendContext())
            {
                var queryPermission = context.UserPermissons.Where(userPermisson =>
                    userPermisson.UserId == userId && userPermisson.ProjectId == projectId);
                if (!queryPermission.Any()) return Helper.Error(404, "未配置权限");
                var data = new
                {
                    permission = queryPermission.Single().Permission
                };
                return Helper.BuildResult(data);
            }
        }
    }
}