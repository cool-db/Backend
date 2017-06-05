using System;
using Backend.Model;
using System.Linq;
using System.Collections.Generic;

namespace Backend.Biz
{
    public class TaskBiz
    {
//        public static object AddTask(object json)
//        {
//            using (var context = new BackendContext())
//            {
//                var jsonData = Helper.Decode(json);
//
//                context.Tasks.Add(new Task()
//                {
//                    Name = jsonData["name"],
//                    Content = jsonData["content"],
//                    Ddl = Convert.ToDateTime(jsonData["ddl"] ) ,
//                    State = true, //true 表示未完成
//                    ProgressId = int.Parse(jsonData["progress_id"]),
//                   // ExecutorId = int.Parse(jsonData["creator_id"])
//                    //todo
//                });
//
//                context.SaveChanges();
//
//               //todo query
//                var response = "success";
//
//                return response;
//
//            }
//        }

        public static object AddComment(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var commentContent = body["content"];
                
                var time = DateTime.Parse(body["time"]);
                
                var userId = int.Parse(body["userId"]);
                
                var taskId = int.Parse(body["taskId"]);
                

                var newComment = new Comment
                {
                    Content = commentContent,
                    Time = time,
                    UserId = userId,
                    TaskId = taskId
                };
                context.Comments.Add(newComment);
                context.SaveChanges();

                return new
                {
                    commentId = newComment.Id,
                    commentContent = newComment.Content,
                    code = 200
                };
            }
        }
        
        public static object DeleteComment(object json)
        {
            var body = Helper.Decode(json);
            var commentId = int.Parse(body["commentId"]);
            var userId = int.Parse(body["userId"]);
      
            using (var context = new BackendContext())
            {

                var query = context.Comments.Where(comment => comment.Id == commentId);
                if (!query.Any())
                    return Helper.Error(404, "评论不存在");

                var theComment = query.Single();
                if (theComment.UserId != userId)
                    return Helper.Error(401, "该用户未拥有该评论");

                context.Comments.Remove(theComment);
                context.SaveChanges();

                return new
                {
                    code = 200
                };
            }
        }
        
        public static object GetCommentList(int taskId)
        {
            using (var context = new BackendContext())
            {
                var query = context.Tasks.Where(task => task.Id == taskId);
                if (!query.Any())
                    return Helper.Error(404, "任务不存在");

                var theTask = query.Single();

                var comments = new List<object>();
                foreach (var comment in theTask.Comments)
                {
                    comments.Add(new
                    {
                        id = comment.Id,
                        content = comment.Content,
                        time = comment.Time,
                        userId = comment.UserId
                    });
                }

                return new
                {
                    comments,
                    code = 200
                };
            }
        }

    }
    
}