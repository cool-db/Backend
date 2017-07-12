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
            //老毕 我把你代码改了
            var body = Helper.DecodeToObject(json);
            var name = body["name"].ToString();
            var progressId = int.Parse(body["progressId"].ToString());
            var creatorId = int.Parse(body["creatorId"].ToString());
            var content = body["content"].ToString();

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == creatorId);
                if (!queryUser.Any())
                    return Helper.Error(401, "创建者ID不存在");
                var queryProgress = context.Progresses.Where(p => p.Id == progressId);
                if (!queryProgress.Any())
                    return Helper.Error(404, "进程不存在");

                if (queryProgress.Single().Project.Users.All(u => u.Id != creatorId))
                    return Helper.Error(403, "用户未参与该任务");

                var newTask = new Task
                {
                    Name = name,
                    Content = content,
                    OwnerId = creatorId,
                    State = false, //false代表未完成
                    EmergencyType = Emergency.Least
                };

                newTask.Users.Add(queryUser.Single());
                newTask.ProgressId = progressId;
                newTask.Progress = queryProgress.Single();

                if (!Helper.CheckPermission(newTask.Progress.ProjectId, newTask.OwnerId, true, OperationType.POST))
                {
                    return Helper.Error(401, "无权限");
                }

                context.Tasks.Add(newTask);
                context.SaveChanges();


                var data = new
                {
                    taskId = newTask.Id,
                    name = newTask.Name,
                    state = newTask.State,
                    executorId = newTask.OwnerId,
                    progressId = newTask.ProgressId,
                    emergencyType = newTask.EmergencyType
                    //projectId = newTask.Progress.ProjectId,
                    //comments = newTask.Comments,
                    //files,
                    //ddl = newTask.Ddl,
                    //memberIds,
                    //content = newTask.Content,
                };

                return Helper.BuildResult(data);
            }
        }

        public static object DeleteTask(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var taskId = int.Parse(body["taskId"]);
            var userId = int.Parse(body["userId"]);

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

        public static object GetTaskList(int projectId, int userId)
        {
            using (var context = new BackendContext())
            {
                var queryProgect = context.Projects.Where(progect => progect.Id == projectId);
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

                if (!Helper.CheckPermission(projectId, userId, flag, OperationType.GET))
                {
                    return Helper.Error(401, "无权限");
                }


                var data = new
                {
                    tasks,
                };
                return Helper.BuildResult(data);
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
            var taskId = int.Parse(body["taskId"]);
            var executorId = int.Parse(body["executorId"]);
            var userId = int.Parse(body["userId"]);
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
            var taskId = int.Parse(body["taskId"]);
            var userId = int.Parse(body["userId"]);

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
            var subtaskId = int.Parse(body["subtaskId"]);
            var subtaskExecutorId = int.Parse(body["subtaskExecutorId"]);

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

                if (!Helper.CheckPermission(theSubtask.Task.Progress.ProjectId, subtaskExecutorId, flag,
                    OperationType.DELETE))
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
            var subtaskId = int.Parse(body["subtaskId"]);
            var userId = int.Parse(body["userId"]);

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
                };
                return Helper.BuildResult(data);
            }
        }


        public static object AddComment(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var taskId = int.Parse(body["taskId"]);
                var content = body["content"];
                var time = DateTime.Parse(body["time"]);
                var userId = int.Parse(body["userId"]);

                var taskQuery = context.Users.Where(user => user.Id == userId);
                if (!taskQuery.Any())
                    return Helper.Error(404, "任务不存在");

                var userQuery = context.Users.Where(user => user.Id == userId);
                if (!userQuery.Any())
                    return Helper.Error(404, "用户不存在");

                var newComment = new Comment
                {
                    TaskId = taskId,
                    UserId = userId,
                    Content = content,
                    Time = time
                };

                context.Comments.Add(newComment);
                context.SaveChanges();

                var theTask = taskQuery.Single();
                var comments = new List<object>();
                foreach (var c in theTask.Comments)
                {
                    comments.Add(new
                    {
                        id = c.Id,
                        content = c.Content
                    });
                }

                var data = new
                {
                    comments
                };
                return Helper.BuildResult(data);
            }
        }


        public static object DeleteComment(object json)
        {
            var body = Helper.Decode(json);
            var commentId = int.Parse(body["commentId"]);
            var taskId = int.Parse(body["taskId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryComment = context.Comments.Where(comment => comment.Id == commentId);
                if (!queryComment.Any())
                    return Helper.Error(404, "评论不存在");

                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theComment = queryComment.Single();
                if (theComment.UserId != userId)
                    return Helper.Error(401, "该用户未发表该评论");

                context.Comments.Remove(theComment);
                context.SaveChanges();

                var data = new { };
                return Helper.BuildResult(data);
            }
        }

        public static object GetCommentList(int taskId)
        {
            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();

                var comments = new List<object>();
                foreach (var comment in theTask.Comments)
                {
                    comments.Add(new
                    {
                        id = comment.Id,
                        content = comment.Content
                    });
                }

                var data = new
                {
                    comments
                };
                return Helper.BuildResult(data);
            }
        }

        public static object ChangeProgress(object json)
        {
            var body = Helper.Decode(json);
            var taskId = int.Parse(body["taskId"]);

            var progressIdTo = int.Parse(body["progressIdTo"]);

            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);

                var progressId = queryTask.Single().ProgressId;

                var queryProgress = context.Progresses.Where(p => p.Id == progressId);
                var queryProgressTo = context.Progresses.Where(p => p.Id == progressIdTo);

                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                var theProgress = queryProgress.Single();
                var theProgressTo = queryProgressTo.Single();

                theTask.ProgressId = progressIdTo;
                theProgress.Tasks.Remove(theTask);
                theProgressTo.Tasks.Add(theTask);

                context.SaveChanges();

                var data = new
                {
                    name = theTask.Name,
                    content = theTask.Content,
                    ownerId = theTask.OwnerId,
                    ddl = theTask.Ddl,
                    emergencyType = theTask.EmergencyType,
                    state = theTask.State,
                    progressId = theTask.ProgressId
                };

                return Helper.BuildResult(data);
            }
        }
    }
}