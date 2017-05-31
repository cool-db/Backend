using System.Collections.Generic;
using Backend.Model;
using System.Linq;

namespace Backend.Biz
{
    public class ScheduleBiz
    {
        public static object CreateSchedule(object json)
        {
            var body = Helper.Decode(json);
            
            using (var context = new BackendContext())
            {
               
                var scheduleName = body["scheduleName"];
                var scheduleContent = body["scheduleContent"];
                var location = body["location"];
                var startTime = body["startTime"];

                var query = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
                if (!query.Any())
                    return Helper.Error(401, "token错误");

                var newProject = new Project
                {
                    Name = projectName,
                    Description = projectDescription,
                    OwnerId = ownerId
                };
                newProject.Users.Add(query.Single());
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
        
    }
}