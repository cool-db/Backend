using System;
using Backend.Model;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace Backend.Biz
{
    public class ProjectBiz
    {
        public static void RecordProjectOperation(int userId, int projectId , string content)
        {
            using (var context = new BackendContext())
            {
                var user = context.Users.Single(u => u.Id == userId);

                context.ProjectOperations.Add(new ProjectOperation()
                {
                    Content = "用户 " + user.Email + " "+ content,
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
                var projectName = body["projectName"];
                var projectDescription = body["projectDescription"];

                var query = context.Users.Where(user => user.Id == ownerId);
                if (!query.Any())
                    return Helper.Error(404, "该用户不存在");

                var newProject = new Project
                {
                    Name = projectName,
                    Description = projectDescription,
                    OwnerId = ownerId
                };
                newProject.Users.Add(query.Single());
                newProject.UserPermissons.Add(new UserPermisson()
                {
                    UserId = query.Single().Id,
                    ProjectId = newProject.Id,
                    Permission = Permission.Creator
                });
                context.Projects.Add(newProject);
                context.SaveChanges();
                
                RecordProjectOperation(ownerId, newProject.Id, "创建新项目");

                var data = new
                {
                    projectId = newProject.Id,
                    projectName = newProject.Name,
                    projectDescription = newProject.Description
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteProject(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                context.Projects.Remove(theProject);
                context.SaveChanges();

                return Helper.BuildResult(null);
            }
        }

        public static object ChangeOwner(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);
            var ownerIdTo = int.Parse(body["ownerIdTo"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryUserTo = context.Users.Where(user => user.Id == ownerIdTo);
                if (!queryUserTo.Any())
                    return Helper.Error(404, "用户不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theProject.OwnerId = ownerIdTo;
                context.SaveChanges();
                
                RecordProjectOperation(ownerId, theProject.Id, "移交项目给用户 " + queryUserTo.Single().Email);


                var data = new
                {
                    projectId = theProject.Id,
                    ownerId = theProject.OwnerId
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
                    id = theProject.Id,
                    name = theProject.Name,
                    description = theProject.Description,
                    ownerId = theProject.OwnerId
                };
                return Helper.BuildResult(data);
            }
        }

        public static object UpdateInfo(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var ownerId = int.Parse(body["ownerId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == ownerId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.OwnerId != ownerId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theProject.Name = (body.ContainsKey("projectName") && body["projectName"] != "")
                    ? body["projectName"]
                    : theProject.Name;
                theProject.Description = (body.ContainsKey("projectDiscription") && body["projectDiscription"] != "")
                    ? body["projectDiscription"]
                    : theProject.Description;
                context.SaveChanges();
                
                RecordProjectOperation(ownerId, projectId, "更新了项目信息");
                
                var data = new
                {
                    projectId = theProject.Id,
                    projectName = theProject.Name,
                    projectDiscription = theProject.Description
                };
                return Helper.BuildResult(data);
            }
        }

        public static object GetList(int userId)
        {
            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(401, "不存在该用户");

                var theUser = queryUser.Single();

                var projects = (from project in theUser.Projects
                    select new
                    {
                        id = project.Id,
                        name = project.Name,
                        description = project.Description,
                        ownerId = project.OwnerId
                    }).ToArray();

                var data = new
                {
                    projects
                };
                return Helper.BuildResult(data);
            }
        }

        #endregion


        public static object AddMember(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            
            using (var context = new BackendContext())
            {
                
                int memberId;
                if (body.ContainsKey("email"))
                {
                    var email = body["email"];
                    memberId = context.Users.Where(u => u.Email == email).Single().Id;
                }
                else if(body.ContainsKey("name"))
                {
                    var name = body["name"];
                    memberId = context.Users.Where(u => u.UserInfo.Name == name).Single().Id;
                }
                else
                {
                    return Helper.Error(404, "传值不足");
                }
                
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");
                var theUser = queryUser.Single();

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");
                var theProject = queryProject.Single();

                if (!theUser.Projects.Contains(theProject))
                    return Helper.Error(401, "该用户未在这个项目");

                var queryPermission = context.UserPermissons.Where(permission =>
                    permission.ProjectId == projectId && permission.UserId == userId);
                if (!queryPermission.Any() || queryPermission.Single().Permission == Permission.Participant)
                    return Helper.Error(401, "该用户未拥有该项目管理权限");

                var queryMember = context.Users.Where(member => member.Id == memberId);
                if (!queryMember.Any())
                    return Helper.Error(404, "添加的用户不存在");

                var theMember = queryMember.Single();
                if (!theProject.Users.Contains(theMember))
                {
                    theProject.Users.Add(theMember);
                    theMember.UserPermissons.Add(new UserPermisson()
                    {
                        Permission = Permission.Participant,
                        ProjectId = projectId,
                        UserId = userId
                    });
                    context.SaveChanges();
                    
                    RecordProjectOperation(userId,projectId,"增加了新成员 " + theMember.Email);

                }
                else
                {
                    return Helper.Error(400, "用户已存在");
                }

                var members = (from theProjectUser in theProject.Users
                    select new
                    {
                        id = theProjectUser.Id,
                        name = theProjectUser.UserInfo.Name,
                        permission = theProjectUser.UserPermissons.Single(p => p.ProjectId==projectId).Permission
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
            var userId = int.Parse(body["userId"]);
            var memberId = int.Parse(body["memberId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();

                var queryMember = context.Users.Where(member => member.Id == memberId);
                if (!queryMember.Any())
                    return Helper.Error(404, "删除的用户不存在");
                var theMember = queryMember.Single();

                var queryPermission1 = context.UserPermissons.Where(permission =>
                    permission.ProjectId == projectId && permission.UserId == userId);
                var queryPermission2 = context.UserPermissons.Where(permission =>
                    permission.ProjectId == projectId && permission.UserId == memberId);
                var permission1 = (queryPermission1.Any())
                    ? queryPermission1.Single().Permission
                    : Permission.Participant;
                var permission2 = (queryPermission1.Any())
                    ? queryPermission2.Single().Permission
                    : Permission.Participant;
                if (permission1 >= permission2)
                    return Helper.Error(401, "权限不足");

                if (queryPermission2.Any())
                    context.UserPermissons.Remove(queryPermission2.Single());
                theProject.Users.Remove(theMember);
                context.SaveChanges();
                
                RecordProjectOperation(userId,projectId,"删除了成员 " + theMember.Email);

                var members = (from theProjectUser in theProject.Users
                    select new
                    {
                        id = theProjectUser.Id,
                        name = theProjectUser.UserInfo.Name,
                        permission = theProjectUser.UserPermissons.Single(p => p.ProjectId==projectId).Permission
                    }).ToArray();

                var data = new
                {
                    members
                };
                return Helper.BuildResult(data);
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

                var members = (from theProjectUser in theProject.Users
                    select new
                    {
                        id = theProjectUser.Id,
                        name = theProjectUser.UserInfo.Name,
                        permission = theProjectUser.UserPermissons.Single(p => p.ProjectId==projectId).Permission,
                        email = theProjectUser.Email,
                        avatar = theProjectUser.UserInfo.Avatar
                    }).ToArray();

                var data = new
                {
                    members
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

        public static object UpdatePermission(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            var memberId = int.Parse(body["memberId"]);
            var permission = int.Parse(body["permission"]);

            using (var context = new BackendContext())
            {
                if (permission == 0)
                    return Helper.Error(401, "不能修改至最高权限");
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();

                var queryMember = context.Users.Where(member => member.Id == memberId);
                if (!queryMember.Any())
                    return Helper.Error(404, "修改的用户不存在");
                var theMember = queryMember.Single();

                var queryPermission1 = context.UserPermissons.Where(p =>
                    p.ProjectId == projectId && p.UserId == userId);
                var queryPermission2 = context.UserPermissons.Where(p =>
                    p.ProjectId == projectId && p.UserId == memberId);
                var permission1 = (queryPermission1.Any())
                    ? queryPermission1.Single().Permission
                    : Permission.Participant;
                var permission2 = (queryPermission1.Any())
                    ? queryPermission2.Single().Permission
                    : Permission.Participant;
                if (permission1 >= permission2)
                    return Helper.Error(401, "权限不足");

                if (queryPermission2.Any())
                    queryPermission2.Single().Permission = (Permission) permission;

                RecordProjectOperation(userId,projectId,"给成员 " + theMember.Email + " 提升了权限");

                
                var members = (from theProjectUser in theProject.Users
                    select new
                    {
                        id = theProjectUser.Id,
                        name = theProjectUser.UserInfo.Name,
                        permission=theProjectUser.UserPermissons.Single(p=>p.ProjectId==projectId).Permission
                            
                    }).ToArray();

                var data = new
                {
                    members
                };
                return Helper.BuildResult(data);
            }
        }

        public static object CreateProgress(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            var progressName = body["progressName"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");
                var theUser = queryUser.Single();

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");

                var queryPermission = context.UserPermissons.Where(p =>
                    p.ProjectId == projectId && p.UserId == userId);
                if (queryPermission.Single().Permission == Permission.Participant)
                {
                    return Helper.Error(401, "权限不足");
                }
                var newProgress = new Progress()
                {
                    Name = progressName,
                    ProjectId = projectId,
                    Order = (context.Progresses.Any(p => p.ProjectId == projectId))
                        ? context.Progresses.Where(p => p.ProjectId == projectId).Max(p => p.Order) + 1
                        : 1,
                    OwnerId = userId
                };
                context.Progresses.Add(newProgress);
                context.SaveChanges();
                
                RecordProjectOperation(userId,projectId,"创建了新进程 " + progressName);


                var progressList = Progress.GetProgerssList(projectId);

                var data = new
                {
                    progressList
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteProgress(object json)
        {
            var body = Helper.Decode(json);
            var progressId = int.Parse(body["progressId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(401, "用户不存在");
                var theUser = queryUser.Single();

                var queryProgress = context.Progresses.Where(progress => progress.Id == progressId);
                if (!queryProgress.Any())
                    return Helper.Error(404, "进度不存在");

                var theProgress = queryProgress.Single();
                var theProject = context.Projects.Single(project => project.Id == theProgress.ProjectId);

                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");
                
                
                var queryPermission = context.UserPermissons.Where(p =>
                    p.ProjectId == theProject.Id && p.UserId == userId);
                if (queryPermission.Single().Permission == Permission.Participant)
                {
                    return Helper.Error(401, "权限不足");
                }
                
                foreach (var progress in context.Progresses)
                {
                    if (progress.Order > theProgress.Order)
                        --progress.Order;
                }
                context.Progresses.Remove(theProgress);
                context.SaveChanges();
                
                RecordProjectOperation(userId,theProject.Id,"创建了新进程 " + theProgress.Name);

                var progressList = Progress.GetProgerssList(theProject.Id);

                var data = new
                {
                    progressList
                };
                return Helper.BuildResult(data);
            }
        }

        public static object UpdateProgressName(object json)
        {
            var body = Helper.Decode(json);
            var progressId = int.Parse(body["progressId"]);
            var userId = int.Parse(body["userId"]);
            var progressName = body["progressName"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(401, "用户不存在");
                var theUser = queryUser.Single();

                var queryProgress = context.Progresses.Where(progress => progress.Id == progressId);
                if (!queryProgress.Any())
                    return Helper.Error(404, "进度不存在");

                var theProgress = queryProgress.Single();
                var theProject = context.Projects.Single(project => project.Id == theProgress.ProjectId);

                if (!theProject.Users.Contains(theUser))
                    return Helper.Error(401, "该用户未参与该项目");
                
                
                var queryPermission = context.UserPermissons.Where(p =>
                    p.ProjectId == theProject.Id && p.UserId == userId);
                if (queryPermission.Single().Permission == Permission.Participant)
                {
                    return Helper.Error(401, "权限不足");
                }
                
                if (string.IsNullOrWhiteSpace(progressName))
                    return Helper.Error(417, "名称为空");

                theProgress.Name = progressName;
                context.SaveChanges();
                
                RecordProjectOperation(userId,theProject.Id,"更新了进程 " + theProgress.Name + "的名字");

                var progressList = Progress.GetProgerssList(theProject.Id);

                var data = new
                {
                    progressList
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
                    
                    RecordProjectOperation(userId,projectId,"更新了进程的顺序");

                    
                    var progressList = Progress.GetProgerssList(theProject.Id);

                    var data = new
                    {
                        progressList
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
                    progressList
                };
                return Helper.BuildResult(data);
            }
        }
    }
}