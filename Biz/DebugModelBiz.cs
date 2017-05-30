using System;
using System.Data.Entity;
using Backend.Model;

namespace Backend.Biz
{
    public static class DebugModelBiz
    {
        public static object DebugModel(object json)
        {
            using (var context = new BackendContext())
            {
                var body = Helper.Decode(json);
                var isRebuild = bool.Parse(body["isRebuild"]);
                if (isRebuild)
                    Database.SetInitializer(new DropCreateDatabaseAlways<BackendContext>());

                var user = new User()
                {
                    Email = "xt@xt.cn",
                    Password = "xt",
                    Token = "xt",
                    UserInfo = new UserInfo()
                    {
                        Address = "4-201@SHIT",
                        Name = "xt",
                        Gender = false,
                        Job = "Mogician",
                        Website = "xt.cn",
                        Birthday = DateTime.Now
                    }
                };

                var project = new Project()
                {
                    Name = "test",
                    Description = "yuck",
                    OwnerId = 1
                };

                var project2 = new Project()
                {
                    Name = "fuck",
                    Description = "yea",
                    OwnerId = 1
                };


                user.Projects.Add(project);
                project.Users.Add(user);
                project2.Users.Add(user);

                context.Projects.Add(project);
                context.Projects.Add(project2);
                context.Users.Add(user);

                context.SaveChanges();

                Database.SetInitializer(new CreateDatabaseIfNotExists<BackendContext>());
                return new
                {
                    code = 200,
                    message = "success"
                };
            }
        }
    }
}