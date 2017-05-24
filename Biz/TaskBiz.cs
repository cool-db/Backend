using System;
using System.Runtime.InteropServices;
using Backend.Model;

namespace Backend.Biz
{
    public class TaskBiz
    {
        public static object AddTask(object json)
        {
            using (var context = new BackendContext())
            {
                var jsonData = Helper.Decode(json);

                context.Tasks.Add(new Task()
                {
                    Name = jsonData["name"],
                    Content = jsonData["content"],
                    Ddl = Convert.ToDateTime(jsonData["ddl"] ) ,
                    State = true, //true 表示未完成
                    ProgressId = int.Parse(jsonData["progress_id"]),
                    ExecutorId = int.Parse(jsonData["creator_id"])
                    //todo
                });

               //todo query
                var response = "success";

                return response;

            }
        }

    }
}