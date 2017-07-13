using System;
using System.Collections.Generic;
using Backend.Model;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Backend.Biz
{
    public class TaskBiz
    {
        public static void RecordTaskOperation(int userId, int taskId, string content)
        {
            using (var context = new BackendContext())
            {
                var user = context.Users.Single(u => u.Id == userId);

                context.TaskOperations.Add(new TaskOperation()
                {
                    Content = "用户 " + user.Email + " " + content,
                    TaskId = taskId,
                    UserId = userId,
                    Time = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public static object CreateTask(object json)
        {
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
                    EmergencyType = Emergency.Least,
                    Ddl = DateTime.Now
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


                RecordTaskOperation(creatorId, newTask.Id, "创建了该任务");

                var data = new
                {
                    taskId = newTask.Id,
                    name = newTask.Name,
                    state = newTask.State,
                    executorId = newTask.OwnerId,
                    progressId = newTask.ProgressId,
                    emergencyType = newTask.EmergencyType,
                    ddl = newTask.Ddl,
                    member = (from user in newTask.Users
                        select new
                        {
                            id = user.Id,
                            email = user.Email,
                            name = user.UserInfo.Name,
                            address = user.UserInfo.Website,
                            job = user.UserInfo.Job,
                            gender = user.UserInfo.Gender,
                            avatar = user.UserInfo.Avatar,
                            phonenumber = user.UserInfo.Phonenumber,
                            birthday = user.UserInfo.Birthday
                        }).ToArray(),
                };

                return Helper.BuildResult(data);
            }
        }

        public static object GetInfo(int taskId)
        {
            using (var contect = new BackendContext())
            {
                var queryTask = contect.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();

                var data = new
                {
                    progressName = theTask.Progress.Name,
                    taskId = theTask.Id,
                    name = theTask.Name,
                    content = theTask.Content,
                    state = theTask.State,
                    executorId = theTask.OwnerId,
                    members = (from user in theTask.Users
                        select new
                        {
                            id = user.Id,
                            email = user.Email,
                            name = user.UserInfo.Name,
                            address = user.UserInfo.Website,
                            job = user.UserInfo.Job,
                            gender = user.UserInfo.Gender,
                            avatar = user.UserInfo.Avatar,
                            phonenumber = user.UserInfo.Phonenumber,
                            birthday = user.UserInfo.Birthday
                        }).ToArray(),
                    progressId = theTask.ProgressId,
                    ddl = theTask.Ddl,
                    emergencyType = theTask.EmergencyType,
                    comments = (from comment in theTask.Comments
                        select new
                        {
                            id = comment.Id,
                            content = comment.Content,
                            time = comment.Time,
                            userId = comment.UserId
                        }).ToArray(),
                    files = (from file in theTask.Files
                        select new
                        {
                            id = file.Id,
                            name = file.Name,
                            projectId = file.ProjectId,
                            userId = file.UserId,
                            uploadTime = file.UploadTime
                        }).ToArray(),
                    subtasks = from subtask in theTask.Subtasks
                    select new
                    {
                        id = subtask.Id,
                        content = subtask.Content,
                        state = subtask.State,
                        taskId = subtask.TaskId,
                        userId = subtask.UserId
                    }
                };
                return Helper.BuildResult(data);
            }
        }

        public static object UpdateInfo(object json)
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
                if (!theTask.Progress.Project.Users.Contains(queryUser.Single()))
                    return Helper.Error(403, "用户未参与此项目");

                if (!Helper.CheckPermission(theTask.Progress.ProjectId, userId, userId == theTask.OwnerId,
                    OperationType.PUT))
                {
                    return Helper.Error(401, "无权限");
                }
                theTask.Name = (body.ContainsKey("name")) ? body["name"] : theTask.Name;
                theTask.Content = (body.ContainsKey("content"))
                    ? body["content"]
                    : theTask.Content;
                theTask.Ddl = (body.ContainsKey("ddl")) ? DateTime.Parse(body["ddl"]) : theTask.Ddl;
                theTask.State = (body.ContainsKey("state")) ? bool.Parse(body["state"]) : theTask.State;
                theTask.OwnerId = (body.ContainsKey("ownerId")) ? int.Parse(body["ownerId"]) : theTask.OwnerId;

                context.SaveChanges();

                RecordTaskOperation(userId, taskId, "更新了该任务的信息");


                var memberIds = (from user in theTask.Users
                    select new
                    {
                        id = user.Id
                    }).ToArray();

                var data = new
                {
                    taskId = theTask.Id,
                    name = theTask.Name,
                    content = theTask.Content,
                    state = theTask.State,
                    executorId = theTask.OwnerId,
                    memberId = memberIds,
                    progressId = theTask.ProgressId,
                    ddl = theTask.Ddl,
                    emergencyType = theTask.EmergencyType,
                    comments = (from comment in theTask.Comments
                        select new
                        {
                            id = comment.Id,
                            content = comment.Content,
                            time = comment.Time,
                            userId = comment.UserId
                        }).ToArray(),
                    files = (from file in theTask.Files
                        select new
                        {
                            id = file.Id,
                            name = file.Name,
                            projectId = file.ProjectId,
                            userId = file.UserId,
                            uploadTime = file.UploadTime
                        }).ToArray(),
                    subtasks = from subtask in theTask.Subtasks
                    select new
                    {
                        id = subtask.Id,
                        content = subtask.Content,
                        state = subtask.State,
                        taskId = subtask.TaskId,
                        userId = subtask.UserId
                    }
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteTask(object json)
        {
            var body = Helper.Decode(json);
            var taskId = int.Parse(body["taskId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                if (theTask.Progress.Project.Users.All(u => u.Id != userId))
                    return Helper.Error(403, "用户不在该项目中");

                var flag = userId == queryTask.Single().OwnerId;

                if (!Helper.CheckPermission(theTask.Progress.Project.Id, userId, flag, OperationType.DELETE))
                {
                    return Helper.Error(401, "无权限");
                }

                context.Tasks.Remove(theTask);
                context.SaveChanges();

                return Helper.BuildResult(null);
            }
        }


        public static object GetTaskList(int projectId)
        {
            using (var context = new BackendContext())
            {
                var queryProgect = context.Projects.Where(progect => progect.Id == projectId);
                if (!queryProgect.Any())
                    return Helper.Error(404, "项目不存在");

                var theProgect = queryProgect.Single();

                var tasks = new List<object>();
                foreach (var progress in theProgect.Progresses)
                {
                    foreach (var task in progress.Tasks)
                    {
                        tasks.Add(new
                        {
                            taskId = task.Id,
                            name = task.Name,
                            content = task.Content,
                            state = task.State,
                            executorId = task.OwnerId,
                            progressId = task.ProgressId,
                            ddl = task.Ddl,
                            emergencyType = task.EmergencyType,
                            comments = (from comment in task.Comments
                                select new
                                {
                                    id = comment.Id,
                                    content = comment.Content,
                                    time = comment.Time,
                                    userId = comment.UserId
                                }).ToArray(),
                            files = (from file in task.Files
                                select new
                                {
                                    id = file.Id,
                                    name = file.Name,
                                    projectId = file.ProjectId,
                                    userId = file.UserId,
                                    uploadTime = file.UploadTime
                                }).ToArray(),
                            subtasks = from subtask in task.Subtasks
                            select new
                            {
                                id = subtask.Id,
                                content = subtask.Content,
                                state = subtask.State,
                                taskId = subtask.TaskId,
                                userId = subtask.UserId
                            }
                        });
                    }
                }

                var data = new
                {
                    tasks
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
                if (theTask.Progress.Project.Users.All(u => u.Id != userId))
                    return Helper.Error(403, "用户不在该项目中");

                if (!Helper.CheckPermission(theTask.Progress.ProjectId, userId, userId == theTask.OwnerId,
                    OperationType.PUT))
                {
                    return Helper.Error(401, "无权限");
                }

                if ((body.ContainsKey("state")))
                    theTask.State = bool.Parse(body["state"]);

                context.SaveChanges();

                RecordTaskOperation(userId, taskId, "更新了该任务的状态");


                var data = new
                {
                    taskId = theTask.Id,
                    name = theTask.Name,
                    content = theTask.Content,
                    state = theTask.State,
                    executorId = theTask.OwnerId,
                    progressId = theTask.ProgressId,
                    ddl = theTask.Ddl,
                    emergencyType = theTask.EmergencyType,
                    comments = (from comment in theTask.Comments
                        select new
                        {
                            id = comment.Id,
                            content = comment.Content,
                            time = comment.Time,
                            userId = comment.UserId
                        }).ToArray(),
                    files = (from file in theTask.Files
                        select new
                        {
                            id = file.Id,
                            name = file.Name,
                            projectId = file.ProjectId,
                            userId = file.UserId,
                            uploadTime = file.UploadTime
                        }).ToArray(),
                    subtasks = from subtask in theTask.Subtasks
                    select new
                    {
                        id = subtask.Id,
                        content = subtask.Content,
                        state = subtask.State,
                        taskId = subtask.TaskId,
                        userId = subtask.UserId
                    }
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

                RecordTaskOperation(userId, taskId, "创建了子任务:" + newSubTask.Content);


                var data = new
                {
                    subtaskId = newSubTask.Id,
                    subtaskContent = newSubTask.Content,
                    taskId = newSubTask.TaskId,
                    state = newSubTask.State,
                    executorId = newSubTask.UserId
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

                RecordTaskOperation(userId, theSubtask.TaskId, "删除了子任务:" + theSubtask.Content);


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

                RecordTaskOperation(subtaskExecutorId, theSubtask.TaskId, "更新了了子任务:" + theSubtask.Content + "的信息");


                var data = new
                {
                    subtaskId = theSubtask.Id,
                    state = theSubtask.State,
                    taskId = theSubtask.Task.Id,
                    subtaskContent = theSubtask.Content,
                    executorId = subtaskExecutorId
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

                RecordTaskOperation(userId, theSubtask.TaskId, "更新了了子任务:" + theSubtask.Content + "的状态");


                var data = new
                {
                    subtaskState = theSubtask.State
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
                    executorId = theSubtask.UserId
                };
                return Helper.BuildResult(data);
            }
        }

        public static object AddMember(object json)
        {
            var body = JObject.Parse(json.ToString());
            var taskId = int.Parse(body["taskId"].ToString());
            var participatorId = int.Parse(body["participatorId"].ToString());

            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = queryTask.Single();

                var query = context.Users.Where(user => user.Id == participatorId);
                if (!query.Any())
                    return Helper.Error(404, "添加的用户不存在");

                var theParticipator = query.Single();

                if (!theTask.Progress.Project.Users.Contains(theParticipator))
                    return Helper.Error(417, "添加的用户未加入该项目");

                if (theTask.Users.Contains(theParticipator))
                    return Helper.Error(417, "添加的用户已存在");

                theTask.Users.Add(theParticipator);

                context.SaveChanges();

                RecordTaskOperation(participatorId, taskId, "给该任务增加了新成员：" + theParticipator.Email);


                var participators = (from user in theTask.Users
                    select new
                    {
                        id = user.Id,
                        name = user.UserInfo.Name
                    }).ToArray();

                var data = new
                {
                    participators
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteMember(object json)
        {
            var body = JObject.Parse(json.ToString());
            var taskId = int.Parse(body["taskId"].ToString());
            var participatorId = int.Parse(body["participatorId"].ToString());

            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = queryTask.Single();

                var query = context.Users.Where(user => user.Id == participatorId);
                if (!query.Any())
                    return Helper.Error(404, "删除的用户不存在");

                var theParticipator = query.Single();

                if (!theTask.Users.Contains(theParticipator))
                    return Helper.Error(417, "删除的用户不在任务参与者中");

                theTask.Users.Remove(theParticipator);
                //把subtask的拥有者为删除的成员的子任务删除
                foreach (var subtask in theTask.Subtasks)
                {
                    if (subtask.User == theParticipator)
                    {
                        theTask.Subtasks.Remove(subtask);
                    }
                }

                context.SaveChanges();

                RecordTaskOperation(participatorId, taskId, "给该任务删除了成员：" + theParticipator.Email);


                var participators = (from user in theTask.Users
                    select new
                    {
                        id = user.Id,
                        name = user.UserInfo.Name
                    }).ToArray();

                var data = new
                {
                    participators
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

                var members = (from user in theTask.Users
                    select new
                    {
                        id = user.Id,
                        name = user.UserInfo.Name
                    }).ToArray();

                var data = new
                {
                    members
                };
                return Helper.BuildResult(data);
            }
        }


        public static object AddComment(object json)
        {
            var body = Helper.Decode(json);
            var taskId = int.Parse(body["taskId"]);
            var content = body["content"];
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var taskQuery = context.Tasks.Where(t => t.Id == taskId);

                if (!taskQuery.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = taskQuery.Single();

                var userQuery = context.Users.Where(user => user.Id == userId);
                if (!userQuery.Any())
                    return Helper.Error(404, "用户不存在");

                if (!theTask.Progress.Project.Users.Contains(userQuery.Single()))
                    return Helper.Error(401, "用户未参加这个项目");

                var newComment = new Comment
                {
                    TaskId = taskId,
                    UserId = userId,
                    Content = content,
                    Time = DateTime.Now
                };

                context.Comments.Add(newComment);
                context.SaveChanges();

                RecordTaskOperation(userId, taskId, "添加了评论：" + content);


                var comments = (from comment in theTask.Comments
                    select new
                    {
                        id = comment.Id,
                        content = comment.Content,
                        userId = comment.UserId,
                        taskId = comment.TaskId
                    }).ToArray();
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
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryComment = context.Comments.Where(comment => comment.Id == commentId);
                if (!queryComment.Any())
                    return Helper.Error(404, "评论不存在");
                var theComment = queryComment.Single();
                var theTask = theComment.Task;

                if (theComment.UserId != userId)
                    return Helper.Error(401, "该用户未发表该评论");

                context.Comments.Remove(theComment);
                context.SaveChanges();

                RecordTaskOperation(userId, theTask.Id, "删除了了评论：" + theComment.Content);


                var comments = (from comment in theTask.Comments
                    select new
                    {
                        id = comment.Id,
                        content = comment.Content,
                        userId = comment.UserId,
                        taskId = comment.TaskId
                    }).ToArray();
                var data = new
                {
                    comments
                };
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

                var comments = (from comment in theTask.Comments
                    select new
                    {
                        id = comment.Id,
                        content = comment.Content,
                        userId = comment.UserId,
                        taskId = comment.TaskId
                    }).ToArray();
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
            var userId = int.Parse(body["userId"]);
            var progressIdTo = int.Parse(body["progressIdTo"]);

            using (var context = new BackendContext())
            {
                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");
                var theTask = queryTask.Single();

                var progressId = theTask.ProgressId;

                var queryProgress = context.Progresses.Where(p => p.Id == progressId);
                var queryProgressTo = context.Progresses.Where(p => p.Id == progressIdTo);
                if (!queryProgress.Any())
                    return Helper.Error(404, "进度不存在");
                if (!queryProgressTo.Any())
                    return Helper.Error(404, "进度不存在");
                var theProgress = queryProgress.Single();
                var theProgressTo = queryProgressTo.Single();

                theTask.ProgressId = progressIdTo;
                theProgress.Tasks.Remove(theTask);
                theProgressTo.Tasks.Add(theTask);

                context.SaveChanges();

                RecordTaskOperation(userId, taskId, "修改了任务到：" + theProgress.Name);


                var data = new
                {
                    name = theTask.Name,
                    content = theTask.Content,
                    executorId = theTask.OwnerId,
                    ddl = theTask.Ddl,
                    emergencyType = theTask.EmergencyType,
                    state = theTask.State,
                    progressId = theTask.ProgressId
                };

                return Helper.BuildResult(data);
            }
        }

        public static object AddAttachment(object json)
        {
            var body = Helper.Decode(json);
            var fileId = int.Parse(body["fileId"]);
            var taskId = int.Parse(body["taskId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryFile = context.File.Where(f => f.Id == fileId);
                if (!queryFile.Any())
                    return Helper.Error(404, "文件不存在");
                var theFile = queryFile.Single();

                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                if (theTask.Progress.Project.Users.All(u => u.Id != userId))
                    return Helper.Error(403, "用户不在该项目中");

                if (!theTask.Progress.Project.Files.Contains(theFile))
                    return Helper.Error(404, "文件不属于这个项目");

                var flag = userId == queryTask.Single().OwnerId;

                if (!Helper.CheckPermission(theTask.Progress.Project.Id, userId, flag, OperationType.SPECIAL))
                {
                    return Helper.Error(401, "无权限");
                }

                theFile.Tasks.Add(theTask);
                context.SaveChanges();

                RecordTaskOperation(userId, taskId, "把文件" + theFile.Name + "关联到该任务");


                var data = (from file in theTask.Files
                    select new
                    {
                        id = file.Id,
                        name = file.Name,
                        projectId = file.ProjectId,
                        userId = file.UserId,
                        uploadTime = file.UploadTime
                    }).ToArray();
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteAttachment(object json)
        {
            var body = Helper.Decode(json);
            var fileId = int.Parse(body["fileId"]);
            var taskId = int.Parse(body["taskId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryFile = context.File.Where(f => f.Id == fileId);
                if (!queryFile.Any())
                    return Helper.Error(404, "文件不存在");
                var theFile = queryFile.Single();

                var queryUser = context.Users.Where(user => user.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var queryTask = context.Tasks.Where(task => task.Id == taskId);
                if (!queryTask.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = queryTask.Single();
                if (theTask.Progress.Project.Users.All(u => u.Id != userId))
                    return Helper.Error(403, "用户不在该项目中");

                if (!theTask.Files.Contains(theFile))
                    return Helper.Error(404, "文件不属于这个任务");

                var flag = userId == queryTask.Single().OwnerId;

                if (!Helper.CheckPermission(theTask.Progress.Project.Id, userId, flag, OperationType.PUT))
                {
                    return Helper.Error(401, "无权限");
                }

                theFile.Tasks.Remove(theTask);
                context.SaveChanges();

                RecordTaskOperation(userId, taskId, "把文件" + theFile.Name + "和该任务解除关联");


                var data = (from file in theTask.Files
                    select new
                    {
                        id = file.Id,
                        name = file.Name,
                        projectId = file.ProjectId,
                        userId = file.UserId,
                        uploadTime = file.UploadTime,
                        avatar = file.User.UserInfo.Avatar,
                        userName = file.User.UserInfo.Name,
                    }).ToArray();
                return Helper.BuildResult(data);
            }
        }
    }
}