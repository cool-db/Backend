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
                var ddl = DateTime.Parse(body["ddl"].ToString());

                var query = context.Users.Where(user => user.Id == creatorId);
                if (!query.Any())
                    return Helper.Error(401, "token错误");
                
                var progressQuery = context.Progresses.Where(progress => progress.Id == progressId);
                if (!progressQuery.Any())
                    return Helper.Error(401, "progress错误");
                
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
                        return Helper.Error(417, "用户不存在");

                    newTask.Users.Add(query.Single());
                }
                
                newTask.ProgressId = progressId;
                newTask.Progress = progressQuery.Single();

              
//                //to do
// 
//
                context.Tasks.Add(newTask);
                context.SaveChanges(); 
//                
//                

                return new
                {
                    taskId = newTask.Id,
                    newTask.Progress.Id,
                    name = newTask.Name,
                    content = newTask.Content,
                    state = newTask.State,
                    executorId = newTask.OwnerId,
                    progressId = newTask.ProgressId,
                    comments = newTask.Comments,
                    memberIds,
//                    files,//to do
                    ddl = newTask.Ddl,
                    code = 200
                };
            }
        }
    }
}