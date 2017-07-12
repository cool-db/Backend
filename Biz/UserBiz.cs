using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Model;

namespace Backend.Biz
{
    public class UserBiz
    {
        public static object Authorize(object json)
        {
            var body = Helper.Decode(json);
            var token = body["token"];

            using (var context = new BackendContext())
            {
                var query = context.Users.Where(user => user.Token == token);
                if (!query.Any())
                    return Helper.Error(401, "token错误");
                var theUser = query.Single();

                var data = new
                {
                    id = theUser.Id,
                    name = theUser.UserInfo.Name,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object Login(object json)
        {
            //获取输入
            var body = Helper.Decode(json);
            var email = body["email"];
            var password = body["password"];

            using (var context = new BackendContext())
            {
                //查询&验证
                var query = context.Users.Where(user => user.Email == email && user.Password == password);
                if (!query.Any())
                    return Helper.Error(401, "邮箱或密码错误");

                //修改&保存
                var theUser = query.Single();
                theUser.GenerateToken();
                context.SaveChanges();

                //返回
                var data = new
                {
                    id = theUser.Id,
                    token = theUser.Token,
                    //name = theUser.UserInfo.Name,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object Logout(object json)
        {
            var body = Helper.Decode(json);
            var id = int.Parse(body["id"]);
            var token = body["token"];

            using (var context = new BackendContext())
            {
                var query = context.Users.Where(user => user.Id == id && user.Token == token);

                if (!query.Any())
                    return Helper.Error(401, "token错误");

                var theUser = query.Single();
                theUser.GenerateToken();
                context.SaveChanges();

                return Helper.BuildResult("");
            }
        }

        public static object Registrate(object json)
        {
            var body = Helper.Decode(json);
            var email = body["email"];
            var password = body["password"];

            using (var context = new BackendContext())
            {
                if (context.Users.Any(user => user.Email == email))
                    return Helper.Error(401, "该email已注册");

                var newUser = new User
                {
                    Email = email,
                    Password = password,
                    UserInfo = new UserInfo()
                    {
                        Name = (body.ContainsKey("name")) ? body["name"] : null,
                        Address = (body.ContainsKey("address")) ? body["address"] : null,
                        Gender = (body.ContainsKey("gender")) ? Helper.ParseBool(body["gender"]) : null,
                        Phonenumber = (body.ContainsKey("phonenumber")) ? body["phonenumber"] : null,
                        Job = (body.ContainsKey("job")) ? body["job"] : null,
                        Website = (body.ContainsKey("website")) ? body["website"] : null,
                        Birthday = (body.ContainsKey("birthday")) ? Helper.ParseDateTime(body["birthday"]) : null
                    }
                };
                
                context.Users.Add(newUser);
                context.SaveChanges();
                
                var data = new
                {
                    id = newUser.Id,
                    token = newUser.Token,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object GetInfo(int id, string token)
        {
            using (var context = new BackendContext())
            {
                var query = context.Users.Where(user => user.Id == id);
                if (!query.Any())
                    return Helper.Error(404, "id不存在");

                var theUser = query.Single();

                if (token == "")
                {
                    var data2 = new
                    {
                        id = theUser.Id,
                        name = theUser.UserInfo.Name,
                    };
                    return Helper.BuildResult(data2);
                }
                if (theUser.Token != token)
                    return Helper.Error(401, "token错误");

                var data = new
                {
                    id = theUser.Id,
                    name = theUser.UserInfo.Name,
                    address = theUser.UserInfo.Address,
                    gender = theUser.UserInfo.Gender,
                    phonenumber = theUser.UserInfo.Phonenumber,
                    job = theUser.UserInfo.Job,
                    website = theUser.UserInfo.Website,
                    birthday = theUser.UserInfo.Birthday,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object ChangePassword(object json)
        {
            var body = Helper.Decode(json);
            var id = int.Parse(body["userId"]);
            var passwordFrom = body["passwordFrom"];
            var passwordTo = body["passwordTo"];

            using (var context = new BackendContext())
            {
                var query = context.Users.Where(user => user.Id == id && user.Password == passwordFrom);
                if (!query.Any())
                    return Helper.Error(401, "密码错误");

                var theUser = query.Single();
                theUser.Password = passwordTo;
                theUser.GenerateToken();
                context.SaveChanges();

                var data = new
                {
                    token = theUser.Token,
                };
                return Helper.BuildResult(data);
            }
        }

        public static object UpdateInfo(object json)
        {
            var body = Helper.Decode(json);
            var id = int.Parse(body["id"]);
            var token = body["token"];

            using (var context = new BackendContext())
            {
                var query = context.Users.Where(user => user.Id == id && user.Token == token);
                if (!query.Any())
                    return Helper.Error(401, "token错误");

                var theUser = query.Single();
                var info = theUser.UserInfo;
                info.Name = (body.ContainsKey("name")) ? body["name"] : info.Name;
                info.Address = (body.ContainsKey("address")) ? body["address"] : info.Address;
                info.Gender = (body.ContainsKey("gender")) ? Helper.ParseBool(body["gender"]) : info.Gender;
                info.Phonenumber = (body.ContainsKey("phonenumber")) ? body["phonenumber"] : info.Phonenumber;
                info.Job = (body.ContainsKey("job")) ? body["job"] : info.Job;
                info.Website = (body.ContainsKey("website")) ? body["website"] : info.Website;
                info.Birthday = (body.ContainsKey("birthday")) ? Helper.ParseDateTime(body["birthday"]) : info.Birthday;
                context.SaveChanges();

                var data = new
                {
                    id = theUser.Id,
                    name = theUser.UserInfo.Name,
                    address = theUser.UserInfo.Address,
                    gender = theUser.UserInfo.Gender,
                    phonenumber = theUser.UserInfo.Phonenumber,
                    job = theUser.UserInfo.Job,
                    website = theUser.UserInfo.Website,
                    birthday = theUser.UserInfo.Birthday,
                };
                return Helper.BuildResult(data);
            }
        }
    }
}