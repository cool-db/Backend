using System;
using Backend.Model;

namespace Backend.Biz
{
    public static class DebugModelBiz
    {
        public static Object DebugModel()
        {
            using (var context = new BackendContext())
            {
                var user = new User()
                {
                    Email = "xt@xt.cn",
                    Password = "xt",
                    Address = "4-201@SHIT",
                    Name = "xt",
                    Gender = false,
                    Job = "Mogician",
                    Website = "xt.cn",
                    Birthday = DateTime.Now
                };
                context.Users.Add(user);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return "Success";
            }
        }
    }
}