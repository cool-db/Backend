﻿using System;
using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("api/authority")]
        public object Authorize(object json)
        {
            return UserBiz.Authorize(json);
        }

        [HttpPost]
        [Route("api/login")]
        public object Login(object json)
        {
            return UserBiz.Login(json);
        }

        [HttpDelete]
        [Route("api/login")]
        public object Logout(object json)
        {
            return UserBiz.Logout(json);
        }

        [HttpPost]
        [Route("api/user")]
        public object Registrate(object json)
        {
            return UserBiz.Registrate(json);
        }

        [HttpPut]
        [Route("api/user")]
        public object UpdateInfo(object json)
        {
            return UserBiz.UpdateInfo(json);
        }
        
        [HttpGet]
        [Route("api/user")]
        public object GetInfo(int id)
        {
            return UserBiz.GetInfo(id);
        }
        
        [HttpGet]
        [Route("api/user/avatar")]
        public object GetAvatar(int id)
        {
            return UserBiz.GetAvatar(id);
        }
        
        [HttpPut]
        [Route("api/user/avatar")]
        public object SetAvatar(object json)
        {
            return UserBiz.SetAvatar(json);
        }
        
        [HttpPut]
        [Route("api/user/pwd")]
        public object ChangePassword(object json)
        {
            return UserBiz.ChangePassword(json);
        }

        [HttpGet]
        [Route("api/user/recent")]
        public object GetRecent(int userId)
        {
            return UserBiz.GetRecent(userId);
        }
    }
}