using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Model;

// todo !!! return CODE
namespace Backend.Biz
{
    public class UserBiz
    {
        public static object Authority(object json)
        {
            using (var context = new BackendContext())
            {
                var body = Helper.Decode(json);
                var token = body["token"];
                var response = (from user in context.Users
                    where user.Token == token
                    select new
                    {
                        user.Id,
                        user.Name
                    }).SingleOrDefault();
                //todo Not Found Check
                return response;
            }
        }

        public static object Login(object json)
        {
            using (var context = new BackendContext())
            {
                var body = Helper.Decode(json);
                var email = body["email"];
                var password = body["password"];
                var response = (from user in context.Users
                    where user.Email == email && user.Password == password
                    select new
                    {
                        user.Id,
                        user.Name,
                        user.Token
                    }).SingleOrDefault();
                //todo Not Found Check
                return response;
            }
        }

        public static object Logout(object json)
        {
            using (var context = new BackendContext())
            {
                var body = Helper.Decode(json);
                var id = int.Parse(body["id"]);
                //todo Change Token
                //todo Not Found Check
                return true;
            }
        }

        public static object Registration(object json)
        {
            using (var context = new BackendContext())
            {
                var body = Helper.Decode(json);
                var email = body["email"];
                var password = body["password"];

                if ((from user in context.Users
                    where user.Email == email
                    select user).Any())
                {
                    return "Having registed before";
                }
                context.Users.Add(new User()
                {
                    Email = email,
                    Password = password
                });
                context.SaveChanges();
                var response = (from user in context.Users
                    where user.Email == email
                    select new
                    {
                        user.Id
                    }).SingleOrDefault();
                return response;
            }
        }

        public static object GetInfo(int senderId, int getterId)
        {
            using (var context = new BackendContext())
            {
                //todo Permission Check
                var response = (from user in context.Users
                    where user.Id == getterId
                    select user).SingleOrDefault();
                //todo Not Found Check
                return response;
            }
        }

        public static object ChangePassword(object json)
        {
            using (var context = new BackendContext())
            {
                var body = Helper.Decode(json);
                var id = int.Parse(body["user_id"]);
                var passwordFrom = body["password_from"];
                var passwordTo = body["password_to"];

                if (!(from user in context.Users
                    where user.Id == id
                    select user).Any())
                {
                    return "User not found";
                }
                
                var query = (from user in context.Users
                    where user.Id == id && user.Password == passwordFrom
                    select user);
                
                if (!query.Any())
                {
                    return "Wrong Password";
                }
                
                query.SingleOrDefault().Password = passwordTo;
                context.SaveChanges();

                //todo Change Token&return new token
                return true;
            }
        }
    }
}