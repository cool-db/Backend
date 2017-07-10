using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Backend.Model;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Backend.Biz
{
    public class TaskBiz
    {
        public static object CreateTask(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var name = body["name"].ToString();
                var content = body["content"].ToString();
                var creatorId = int.Parse(body["creator_id"].ToString());
                var memberIds = JArray.Parse(body["member_id"].ToString());
                var progressId = int.Parse(body["progress_id"].ToString());
//                var files = JArray.Parse(body["file"].ToString());
                if (body["file"] != null)
                {
                    Console.WriteLine("the problem is: {0} ");
                }
//                var files = Dic.Contains["key"]
                var ddl = DateTime.Parse(body["ddl"].ToString());

                var query = context.Users.Where(user => user.Id == creatorId);
                if (!query.Any())
                    return Helper.Error(401, "创建者ID不存在");
                
                var newTask = new Task
                {
                    Name = name,
                    Content = content,
                    OwnerId = creatorId,
                    Ddl = ddl,
                    State = false,//false代表未完成
                };

                newTask.Users.Add(query.Single());
                foreach (var memberId in memberIds)
                {
                    var memberIdI = int.Parse(memberId.ToString());
                    query = context.Users.Where(user => user.Id == memberIdI);
                    if (!query.Any())
                        return Helper.Error(417, "成员ID不存在");

                    newTask.Users.Add(query.Single());
                }
                var queryProgress = context.Progresses.Where(progress => progress.Id == progressId);
                if (!queryProgress.Any())
                    return Helper.Error(401, "progress错误");

                newTask.ProgressId = progressId;
                newTask.Progress = queryProgress.Single();
                //to do

                if (!Helper.CheckPermission(newTask.Progress.ProjectId, newTask.OwnerId, true, OperationType.POST))
                {
                    return Helper.Error(401, "无权限");
                }

                context.Tasks.Add(newTask);
                context.SaveChanges();



                var data = new
                {
                    taskId = newTask.Id,
                    projectId = newTask.Progress.ProjectId,
                    name = newTask.Name,
                    content = newTask.Content,
                    state = newTask.State,
                    executorId = newTask.OwnerId,
                    memberIds,
                    progressId = newTask.ProgressId,
                    comments = newTask.Comments,
//                    files,//to do
                    ddl = newTask.Ddl,
                };
                
                return Helper.BuildResult(data);
            }
        }
        
        public static object DeleteTask(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["project_id"]);
            var taskId = int.Parse(body["task_id"]);
            var userId = int.Parse(body["user_id"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");
                
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
//                if (theProject.OwnerId != ownerId)
//                    return Helper.Error(401, "该用户未拥有该项目");

                bool flag = userId == queryTask.Single().OwnerId;

                if (!Helper.CheckPermission(projectId, userId, flag, OperationType.DELETE))
                {
                    return Helper.Error(401, "无权限");
                }
                
                context.Tasks.Remove(theTask);
                context.SaveChanges();

                return Helper.BuildResult("");
            }
        }
        
        public static object GetTaskList(int progectId, int userId)
        {
            using (var context = new BackendContext())
            {
                var queryProgect = context.Projects.Where(progect => progect.Id == progectId);
                if (!queryProgect.Any())
                    return Helper.Error(401, "项目错误");

                var theProgect = queryProgect.Single();

                var tasks = new List<object>();
                foreach (var progress in theProgect.Progresses)
                {
                    foreach (var task in progress.Tasks)
                    {
                        tasks.Add(new
                        {
                            id = task.Id,
                            name = task.Name,
                            content = task.Content,
                            progressId = task.ProgressId,
                            ddl = task.Ddl
                        });
                    }
                }

                bool flag = userId == queryProgect.Single().OwnerId;

                if (!Helper.CheckPermission(progectId, userId, flag, OperationType.GET))
                {
                    return Helper.Error(401, "无权限");
                }
                

                var data = new
                {
                    tasks,
                    
                };return Helper.BuildResult(data);
            }
        }
        
        public static object GetInfo(int projectId, int taskId)
        {
            using (var context = new BackendContext())
            {
                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");
                
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                
                var memberIds = new List<object>();
                foreach (var user in theTask.Users)
                {
                    memberIds.Add(new
                    {
                        id = user.Id,
                    });
                }

                var data = new
                {
                    taskId = theTask.Id,
                    name = theTask.Name,
                    content = theTask.Content,
                    state = theTask.State,
                    executorId = theTask.OwnerId,
                    memberId = memberIds,
                    progressId = theTask.ProgressId,
                    comments = theTask.Comments,
                    files = theTask.Files,
                    subtasks = theTask.Subtasks,
                    ddl = theTask.Ddl,
                };
                return Helper.BuildResult(data);
            }
        }
        
        public static object UpdateInfo(object json)
        {
//            var body = JObject.Parse(json.ToString());
            var body = Helper.Decode(json);
            var taskId = int.Parse(body["taskId"].ToString());
            var executorId = int.Parse(body["executorId"].ToString());
            var userId = int.Parse(body["userId"].ToString());
//            var membersId = JArray.Parse(body["membersId"].ToString());

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == executorId);
                if (!queryUser.Any())
                    return Helper.Error(401, "executorId错误");
                
                queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(401, "userId错误");

                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                if (theTask.OwnerId != executorId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theTask.Name = (body.ContainsKey("taskName")) ? body["taskName"] : theTask.Name;
                theTask.Content = (body.ContainsKey("taskContent"))
                    ? body["content"]
                    : theTask.Content;
                
                
                bool flag = userId == theTask.OwnerId;

                if (!Helper.CheckPermission(theTask.Progress.ProjectId, userId, flag, OperationType.PUT))
                {
                    return Helper.Error(401, "无权限");
                }
                
                context.SaveChanges();
                
                var memberIds = new List<object>();
                foreach (var user in theTask.Users)
                {
                    memberIds.Add(new
                    {
                        id = user.Id,
                    });
                }

                var data = new
                {
                    taskId = theTask.Id,
                    executorId = theTask.OwnerId,
                    taskName = theTask.Name,
                    taskContent = theTask.Content,
                    memberIds,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object UpdateState(object json)
        {
            var body = Helper.Decode(json);
            var taskId = int.Parse(body["taskId"].ToString());
            var userId = int.Parse(body["userId"].ToString());

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(401, "userId错误");

                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                if ((body.ContainsKey("state")))
                {
                    switch (body["state"])
                    {
                        case "true":
                            theTask.State = true;
                            break;
                        case "false":
                            theTask.State = false;
                            break;
                    }
                }
                
                bool flag = userId == theTask.OwnerId;

                if (!Helper.CheckPermission(theTask.Progress.ProjectId, userId, flag, OperationType.PUT))
                {
                    return Helper.Error(401, "无权限");
                }
                
                context.SaveChanges();

                var data = new
                {
//                    taskId = theTask.Id,
//                    executorId = theTask.OwnerId,
//                    taskName = theTask.Name,
//                    taskContent = theTask.Content,
                    taskState = theTask.State,
                };
                return Helper.BuildResult(data);
            }
        }
        
        public static object CreateSubTask(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var content = body["subtaskContent"].ToString();
                var taskId = int.Parse(body["taskId"].ToString());
                var userId = int.Parse(body["userId"].ToString());
                
                var query = context.Users.Where(user => user.Id == userId);
                if (!query.Any())
                    return Helper.Error(401, "userId不存在");
                
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                
                
                var flag = userId == theTask.OwnerId;

//                if (!Helper.CheckPermission(theTask.Progress.ProjectId, userId, flag, OperationType.POST))
//                {
//                    return Helper.Error(401, "无权限");
//                }               
                var newSubTask = new Subtask
                {
                    Content = content,
                    State = false,
                    UserId = userId,
                    TaskId = theTask.Id
                    
                };
                newSubTask.User = query.Single();
                
                context.Subtasks.Add(newSubTask);

                theTask.Subtasks.Add(newSubTask);
                context.SaveChanges(); 
                
                var data = new
                {
                    subtaskId = newSubTask.Id,
                    subtaskContent = newSubTask.Content,
                    taskId = newSubTask.TaskId,
                    state = newSubTask.State,
                    executorId = newSubTask.UserId,
                };
                return Helper.BuildResult(data);
            }
        }
        
        public static object DeleteSubTask(object json)
        {
            var body = Helper.Decode(json);
            var subtaskId = int.Parse(body["subtaskId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");
                var querySubtask = context.Subtasks.Where(subtaskI => subtaskI.Id == subtaskId);
                if (!querySubtask.Any())
                    return Helper.Error(404, "子任务不存在");

                var theSubtask = querySubtask.Single();
                if (theSubtask.UserId != userId)
                    return Helper.Error(401, "该用户未拥有该子项目");

                bool flag = userId == querySubtask.Single().UserId;

                if (!Helper.CheckPermission(theSubtask.Task.Progress.ProjectId, userId, flag, OperationType.DELETE))
                {
                    return Helper.Error(401, "无权限");
                }
                
                context.Subtasks.Remove(theSubtask);
                context.SaveChanges();

                return Helper.BuildResult("");
            }
        }
        
        public static object UpdateSubtaskInfo(object json)
        {
            var body = Helper.Decode(json);
            var subtaskId = int.Parse(body["subtaskId"].ToString());
            var subtaskExecutorId = int.Parse(body["subtaskExecutorId"].ToString());

            using (var context = new BackendContext())
            {
                var querySubtask = context.Subtasks.Where(subtaskI => subtaskI.Id == subtaskId);
                if (!querySubtask.Any())
                    return Helper.Error(404, "子任务不存在");
                
                var theSubtask = querySubtask.Single();
                if (theSubtask.UserId != subtaskExecutorId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theSubtask.Content = (body.ContainsKey("subtaskContent")) ? body["subtaskContent"] : theSubtask.Content;
                
                bool flag = subtaskExecutorId == theSubtask.UserId;

                if (!Helper.CheckPermission(theSubtask.Task.Progress.ProjectId, subtaskExecutorId, flag, OperationType.DELETE))
                {
                    return Helper.Error(401, "无权限");
                }
                
                context.SaveChanges();

                var data = new
                {
                    subtaskId = theSubtask.Id,
                    state = theSubtask.State,
                    taskId = theSubtask.Task.Id,
                    subtaskContent = theSubtask.Content,
                    executorId = subtaskExecutorId,
                };
                return Helper.BuildResult(data);
            }
        }
        
        public static object UpdateSubtaskState(object json)
        {
            var body = Helper.Decode(json);
            var subtaskId = int.Parse(body["subtaskId"].ToString());
            var userId = int.Parse(body["userId"].ToString());
            
            using (var context = new BackendContext())
            {
                
                var querySubtask = context.Subtasks.Where(subtask => subtask.Id == subtaskId);
                if (!querySubtask.Any())
                    return Helper.Error(404, "子任务不存在");

                var theSubtask = querySubtask.Single();
//                theSubtask.State = theSubtask.State != true;
                
                if ((body.ContainsKey("state")))
                {
                    if (body["state"] == "true")
                        theSubtask.State = true;
                    else if (body["state"] == "false")
                    {
                        theSubtask.State = false;
                    }
                }
                
//                bool flag = userId == theTask.OwnerId;
//
//                if (!Helper.CheckPermission(theSubtask.Task.Progress.ProjectId, userId, flag, OperationType.PUT))
//                {
//                    return Helper.Error(401, "无权限");
//                }
                
                context.SaveChanges();

                var data = new
                {
                    subtaskState = theSubtask.State,
                };
                return Helper.BuildResult(data);
            }
        }
        
        
        public static object GetSubtaskList(int subtaskId)
        {
            using (var context = new BackendContext())
            {
                var querySubtask = context.Subtasks.Where(subtask => subtask.Id == subtaskId);
                if (!querySubtask.Any())
                    return Helper.Error(404, "子任务不存在");

                var theSubtask = querySubtask.Single();
                
                var data = new
                {
                    subtaskId = theSubtask.Id,
                    subtaskContent = theSubtask.Content,
                    state = theSubtask.State,
                    executorId = theSubtask.UserId,
                    
                };
                return Helper.BuildResult(data);
            }
        }
        
        public static object AddMember(object json)
        {
            var body = JObject.Parse(json.ToString());
            var taskId = int.Parse(body["taskId"].ToString());
            var participatorIds = JArray.Parse(body["participatorIds"].ToString());

            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = queryTask.Single();
                
                foreach (var memberId in participatorIds)
                {
                    var memberIdI = int.Parse(memberId.ToString());
                    var query = context.Users.Where(user => user.Id == memberIdI);
                    if (!query.Any())
                        return Helper.Error(404, "添加的用户不存在");

                    var theMember = query.Single();
                    
                    if (theTask.Users.Contains(theMember))
                        return Helper.Error(417, "添加的用户已存在");

                    theTask.Users.Add(theMember);
                }

                
                context.SaveChanges();

                var members = new List<object>();
                foreach (var member in theTask.Users)
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
        
        public static object DeleteMember(object json)
        {
            var body = JObject.Parse(json.ToString());
            var taskId = int.Parse(body["taskId"].ToString());
            var participatorIds = JArray.Parse(body["participatorIds"].ToString());

            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = queryTask.Single();

                foreach (var memberId in participatorIds)
                {
                    var memberIdI = int.Parse(memberId.ToString());
                    var query = context.Users.Where(user => user.Id == memberIdI);
                    if (!query.Any())
                        return Helper.Error(404, "删除的用户不存在");

                    var theMember = query.Single();
                    
                    if (!theTask.Users.Contains(theMember))
                        return Helper.Error(417, "删除的用户不在任务参与者中");

                    theTask.Users.Remove(theMember);
                    //把subtask的拥有者为删除的成员的子任务删除
                    foreach (var subtask in theTask.Subtasks)
                    {
                        if (subtask.User == theMember)
                        {
                            theTask.Subtasks.Remove(subtask);
                        }

                    }
                    
                }
                
                context.SaveChanges();

                var members = new List<object>();
                foreach (var member in theTask.Users)
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
                    code = 200
                };
                return Helper.BuildResult(data);
            }
        }
        
        public static object GetMemberList(int taskId)
        {
            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = queryTask.Single();

                var members = new List<object>();
                foreach (var member in theTask.Users)
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
                    code = 200
                };
                return Helper.BuildResult(data);
            }
        }


    }
}