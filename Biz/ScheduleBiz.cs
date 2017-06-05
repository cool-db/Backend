using System;
using System.Collections.Generic;
using Backend.Model;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Biz
{
    public class ScheduleBiz
    {
        public static object AddParticipator(object json)
            {
                var body = Helper.Decode(json);
//            var projectId = int.Parse(body["projectId"]);
                var ownerId = int.Parse(body["ownerId"]);
//            var ownerToken = body["ownerToken"];
                var memberIds = JArray.Parse(body["ParticipatorIds"].ToString());
                var scheduleId = int.Parse(body["scheduleId"]);
         

                using (var context = new BackendContext())
                {
//                var queryUser = context.Users.Where(user => user.Id == ownerId && user.Token == ownerToken);
//                if (!queryUser.Any())
//                    return Helper.Error(401, "token错误");

                    var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);

                    var theSchedule = querySchedule.Single();
                    if (querySchedule.Any())
                        return Helper.Error(404, "该日程不存在");

                    foreach (var m in memberIds)
                    {
                        var queryParticipator = context.Users.Where(member => member.Id == int.Parse(m["memberId"].ToString()));
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
                            id = member.Id,
                        });
                    }

                    return new
                    {
                        members,
                        code = 200
                    };
                }
            }
        
        
        
    }
}