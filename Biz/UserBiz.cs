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

                return new
                {
                    id = theUser.Id,
                    name = theUser.UserInfo.Name,
                    code = 200
                };
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
                return new
                {
                    id = theUser.Id,
                    token = theUser.Token,
                    name = theUser.UserInfo.Name,
                    code = 200
                };
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

                return new
                {
                    code = 200
                };
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
                };
                context.Users.Add(newUser);
                context.SaveChanges();

                return new
                {
                    id = newUser.Id,
                    token = newUser.Token,
                    code = 200
                };
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
                    return new
                    {
                        id = theUser.Id,
                        name = theUser.UserInfo.Name,
                        code = 200
                    };

                if (theUser.Token != token)
                    return Helper.Error(401, "token错误");

                return new
                {
                    id = theUser.Id,
                    name = theUser.UserInfo.Name,
                    address = theUser.UserInfo.Address,
                    gender = theUser.UserInfo.Gender,
                    phonenumber = theUser.UserInfo.Phonenumber,
                    job = theUser.UserInfo.Job,
                    website = theUser.UserInfo.Website,
                    birthday = theUser.UserInfo.Birthday,
                    code = 200
                };
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

                return new
                {
                    token = theUser.Token,
                    code = 200
                };
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

                return new
                {
                    id = theUser.Id,
                    name = theUser.UserInfo.Name,
                    address = theUser.UserInfo.Address,
                    gender = theUser.UserInfo.Gender,
                    phonenumber = theUser.UserInfo.Phonenumber,
                    job = theUser.UserInfo.Job,
                    website = theUser.UserInfo.Website,
                    birthday = theUser.UserInfo.Birthday,
                    code = 200
                };
            }
        }
    }
}