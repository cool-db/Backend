using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Backend.Model;
using File = Backend.Model.File;

namespace Backend.Biz
{
    public class FileBiz
    {
        public static File GetFile(int fileId)
        {
            using (var context = new BackendContext())
            {
                var queryFile = (from file in context.File
                    where file.Id == fileId
                    select file);
//                if (!queryFile.Any())
//                    return Helper.Error(404, "未找到文件");

                return queryFile.Single();
            }
        }

        public static object PostFile(object json, HttpPostedFile file)
        {
            var body = Helper.Decode(json);
            var userId = int.Parse(body["userId"]);
            var projectId = int.Parse(body["projectId"]);
            var name = body["name"];
            var stream = file.InputStream;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            using (var context = new BackendContext())
            {
                var newFile = new File()
                {
                    Name = name,
                    Content = bytes,
                    UploadTime = DateTime.Now,
                    UserId = userId,
                    ProjectId = projectId
                };
                context.File.Add(newFile);
                context.SaveChanges();
                var result = new
                {
                    id = newFile.Id,
                    name = newFile.Name
                };
                return Helper.BuildResult(result);
            }
        }

        public static object GetList(int projectId)
        {
            using (var context = new BackendContext())
            {
                var data = (from file in context.File
                    where file.ProjectId == projectId
                    select new
                    {
                        file.Id,
                        file.Name,
                        file.UploadTime,
                        file.UserId
                    }).ToArray();
                return Helper.BuildResult(data);
            }
        }

        public static object RenameFile(object json)
        {
            var body = Helper.Decode(json);
            var fileId = int.Parse(body["fileId"]);
            var userId = int.Parse(body["userId"]);
            var fileName = body["fileName"];
            using (var context = new BackendContext())
            {
                var queryFile = context.File.Where(file => file.Id == fileId);
                if (!queryFile.Any())
                    return Helper.Error(404, "未找到文件");
                var queryUser = context.Users.Where(u => u.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "未找到用户");
                var theFile = queryFile.Single();

                if (theFile.Project.Users.All(u => u.Id != userId))
                    return Helper.Error(403, "用户未参加该项目");
                if (theFile.UserId != userId &&
                    theFile.Project.UserPermissons.Single(p => p.UserId == userId).Permission == Permission.Participant)
                    return Helper.Error(401, "权限不足");
                theFile.Name = fileName;
                context.SaveChanges();
                var data = new
                {
                    name = theFile.Name,
                    userId = theFile.UserId,
                    uploadTime = theFile.UploadTime,
                    projectId = theFile.ProjectId
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteFile(object json)
        {
            var body = Helper.Decode(json);
            var fileId = int.Parse(body["fileId"]);
            var userId = int.Parse(body["userId"]);
            using (var context = new BackendContext())
            {
                var queryFile = context.File.Where(file => file.Id == fileId);
                if (!queryFile.Any())
                    return Helper.Error(404, "未找到文件");
                var queryUser = context.Users.Where(u => u.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "未找到用户");
                var theFile = queryFile.Single();

                if (theFile.Project.Users.All(u => u.Id != userId))
                    return Helper.Error(403, "用户未参加该项目");
                if (theFile.UserId != userId &&
                    theFile.Project.UserPermissons.Single(p => p.UserId == userId).Permission == Permission.Participant)
                    return Helper.Error(401, "权限不足");
                context.File.Remove(theFile);
                context.SaveChanges();
                return Helper.BuildResult(null);
            }
        }
    }
}