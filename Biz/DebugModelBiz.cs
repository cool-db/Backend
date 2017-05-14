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

                };
                context.Users.Add(user);
                context.SaveChanges();
                return "Success";
            }
        }
    }
}