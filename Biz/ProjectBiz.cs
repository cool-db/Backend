using System.Collections.Generic;
using Backend.Model;
using System.Linq;


namespace Backend.Biz
{
    public class ProjectBiz
    {
        public static object CreateProjext(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var userId = int.Parse(body["userId"]);
                var userToken = body["userToken"];
                var projectName = body["projectName"];
                var projectDescription = body["projectDescription"];

                var query = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!query.Any())
                    return Helper.Error(401, "token错误");

                var newProject = new Project
                {
                    Name = projectName,
                    Description = projectDescription,
                    UserId = userId
                };
                context.Projects.Add(newProject);
                context.SaveChanges();

                return new
                {
                    projectId = newProject.Id,
                    projectName = newProject.Name,
                    projectDescription = newProject.Description,
                    code = 200
                };
            }
        }

        public static object DeleteProject(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            var userToken = body["userToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.UserId != userId)
                    return Helper.Error(401, "该用户未拥有该项目");

                context.Projects.Remove(theProject);
                context.SaveChanges();

                return new
                {
                    code = 200
                };
            }
        }

        public static object ChangeOwner(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            var userIdTo = int.Parse(body["userIdTo"]);
            var userToken = body["userToken"];

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();
                if (theProject.UserId != userId)
                    return Helper.Error(401, "该用户未拥有该项目");

                theProject.UserId = userIdTo;
                context.SaveChanges();

                return new
                {
                    projectId = theProject.Id,
                    userId = theProject.UserId,
                    code = 200
                };
            }
        }

        public static object GetInfo(int projectId)
        {
            using (var context = new BackendContext())
            {
                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "项目不存在");

                var theProject = queryProject.Single();

                return new
                {
                    projectId = theProject.Id,
                    projectName = theProject.Name,
                    projectDescription = theProject.Description,
                    code = 200
                };
            }
        }

        public static object GetList(int userId, string userToken)
        {
            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId && user.Token == userToken);
                if (!queryUser.Any())
                    return Helper.Error(401, "token错误");

                var theUser = queryUser.Single();
                
                var projects= theUser.Projects.ToList();
                return projects;
            }
        }

        public static object UpdateInfo(object json)
        {
            var body = Helper.Decode(json);
            var projectId = int.Parse(body["projectId"]);
            var userId = int.Parse(body["userId"]);
            var userToken = body["userToken"];
            
            using (var context = new BackendContext())
            {
                return null;
            }
        }
    }
}