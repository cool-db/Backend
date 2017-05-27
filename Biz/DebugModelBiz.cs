using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Model;

namespace Backend.Biz
{
    public static class DebugModelBiz
    {
        public static Object DebugModel()
        {
            using (var context = new BackendContext())
            {
                context.Database.Log = Logger.Log; //todo

                
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
                    Description = "suck"
                };
                
                var project2 = new Project()
                {
                    Name = "fuck",
                    Description = "yea"
                };

                
                user.Projects.Add(project);
                project.Users.Add(user);
                project2.Users.Add(user);
                
                context.Projects.Add(project);
                context.Projects.Add(project2);
                context.Users.Add(user);

                context.SaveChanges();

                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                return "success";
            }
        }
    }
}