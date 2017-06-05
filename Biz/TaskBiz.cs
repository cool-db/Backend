using System;
using Backend.Model;
using System.Linq;
using System.Collections.Generic;


namespace Backend.Biz
{
    public class TaskBiz
    {
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

                return new
                {
                    comments,
                    code = 200
                };
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

                return new
                {
                    comments,
                    code = 200
                };
            }
        }
        
        
    }
    
    
    
    
}