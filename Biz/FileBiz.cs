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
            var taskId = int.Parse(body["taskId"]);
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
                    TaskId = taskId
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

        public static object GetList(int taskId)
        {
            using (var context = new BackendContext())
            {
                var queryList = from file in context.File 
                    where file.TaskId==taskId
                    select new
                    {
                        file.Id,
                        file.Name,
                        file.UploadTime,
                        file.UserId,
                        file.TaskId
                    };
                return queryList.ToArray();
            }
        }

        public static object RenameFile(object json)
        {
            var body = Helper.Decode(json);
            var fileName = body["fileName"];
            var fileId = int.Parse(body["fileId"]);
            using (var context = new BackendContext())
            {
                var queryFile = context.File.Where(file => file.Id == fileId);
                if (!queryFile.Any())
                    Helper.Error(404, "未找到文件");
                var theFile = queryFile.Single();
                theFile.Name = fileName;
                context.SaveChanges();
                var result = new
                {
                    fileName = theFile.Name,
                    fileId = theFile.Id
                };
                return Helper.BuildResult(result);
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
                    Helper.Error(404, "未找到文件");
                var theFile = queryFile.Single();
                context.File.Remove(theFile);
                context.SaveChanges();
                return Helper.BuildResult("");
            }
        }
    }
}