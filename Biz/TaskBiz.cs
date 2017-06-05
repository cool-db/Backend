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

                if (!Helper.CheckPermission(newTask.Progress.ProjectId, newTask.OwnerId, true, OperationType.PUT))
                {
                    return Helper.Error(401, "无权限");
                }

                context.Tasks.Add(newTask);
                context.SaveChanges(); 
                
                

                return new
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
                    code = 200
                };
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

                return new
                {
                    code = 200
                };
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
                

                return new
                {
                    tasks,
                    code = 200
                };
            }
        }

    }
}