using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Model;
using Newtonsoft.Json.Linq;

namespace Backend.Biz
{
    public class ScheduleBiz
    {
        public static object AddParticipator(object json)
        {
            var body = Helper.DecodeToObject(json.ToString());
            var participatorIds = JArray.Parse(body["participatorIds"].ToString());
            var scheduleId = int.Parse(body["scheduleId"].ToString()); 
            using (var context = new BackendContext())
            {
//                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
//                if (!queryUser.Any())
//                    return Helper.Error(401, "token错误");


                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "该日程不存在");
                var theSchedule = querySchedule.Single();

                foreach (var m in participatorIds)
                {
                    var m1 = int.Parse(m.ToString());
                    var queryParticipator = context.Users.Where(member => member.Id == m1);
                    if (!queryParticipator.Any())
                        return Helper.Error(404, "添加的用户不存在");
                    var theParticipator = queryParticipator.Single();
                    if (theSchedule.Users.Contains(theParticipator))
                        return Helper.Error(417, "该参与者已存在");
                    
                    theSchedule.Users.Add(theParticipator);
                    context.SaveChanges();
                }              

                var members = new List<object>();
                foreach (var member in theSchedule.Users)
                {
                    members.Add(new
                    {
                        id = member.Id
                    });
                }

                return new
                {
                    members,
                    code = 200
                };
            }
        }
                
        
        public static object DeleteParticipator(object json)
        {
            var body = Helper.DecodeToObject(json.ToString());
            var participatorIds = JArray.Parse(body["participatorIds"].ToString());
            var scheduleId = int.Parse(body["scheduleId"].ToString()); 
            
            using (var context = new BackendContext())
            {
//                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
//                if (!queryUser.Any())
//                    return Helper.Error(401, "token错误");


                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "该日程不存在");
                var theSchedule = querySchedule.Single();

                foreach (var m in participatorIds)
                {
                    var m1 = int.Parse(m.ToString());
                    var queryParticipator = context.Users.Where(member => member.Id == m1);
                    if (!queryParticipator.Any())
                        return Helper.Error(404, "删除的用户不存在");
                    var theParticipator = queryParticipator.Single();
                    if (!theSchedule.Users.Contains(theParticipator))
                        return Helper.Error(417, "该用户不存在本日程中");
                    
                    theSchedule.Users.Remove(theParticipator);
                    context.SaveChanges();
                }              

                var members = new List<object>();
                foreach (var member in theSchedule.Users)
                {
                    members.Add(new
                    {
                        id = member.Id
                    });
                }

                return new
                {
                    members,
                    code = 200
                };
            }
        }


        public static object GetParticipatorList(int scheduleId)
        {
            using (var context = new BackendContext())
            {
                var querySchedule = context.Tasks.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "日程不存在");

                var theSchedule = querySchedule.Single();
                
                var participators = new List<object>();
                foreach (var p in theSchedule.Users)
                {
                    participators.Add(new
                    {
                        id = p.Id,
                        name = p.UserInfo.Name
                    });
                }

                return new
                {
                    participators,
                    code = 200
                };
            }
        }
        
    }
}