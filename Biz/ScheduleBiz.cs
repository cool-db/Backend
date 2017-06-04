using System;
using System.Collections.Generic;
using Backend.Model;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Backend.Biz
{
    public class ScheduleBiz
    {
        public static object CreateSchedule(object json)
        {
            var body = Helper.DecodeToObject(json);
            
            using (var context = new BackendContext())
            {
                var scheduleName = body["scheduleName"].ToString();
                
                var scheduleContent = body["scheduleContent"].ToString();
                
                var location = body["location"].ToString();
                var startTime = DateTime.Parse(body["startTime"].ToString());
                var endTime = DateTime.Parse(body["endTime"].ToString());
                var repeatDaily = bool.Parse(body["repeatDaily"].ToString());
                var repeatWeekly = bool.Parse(body["repeatWeekly"].ToString()); 
                var participatorIds = JArray.Parse(body["participatorIds"].ToString());  
                var creatorId = int.Parse(body["creatorId"].ToString());
                var projectId = int.Parse(body["projectId"].ToString());
                
                var users = new List<User>();
                var ids = new List<int>();
                foreach (var participatorId in participatorIds)
                { 
                    var id = int.Parse(participatorId.ToString());
                    var queryUser = context.Users.Where(user => user.Id == id);
                    
                    if (!queryUser.Any())
                        return Helper.Error(401, "用户" + participatorId + "不存在");
                    
                    users.Add(queryUser.Single());
                    
                    ids.Add(id);
                 
                }

                //var users = participatorIds.Select(participatorId => context.Users.Single(user => user.Id == int.Parse(participatorId))).ToList();
                var newSchedule = new Schedule
                {
                    Users = users,
                    Content = scheduleContent,
                    EndTime = endTime,
                    StartTime = startTime,
                    Location = location,
                    Name = scheduleName,
                    OwerId = creatorId,
                    RepeatDaily = repeatDaily,
                    RepeatWeekly = repeatWeekly,
                    ProjectId = projectId
                };
                context.Schedules.Add(newSchedule);
                context.SaveChanges();

                return new
                {
                    scheduleId = newSchedule.Id,
                    scheduleName = newSchedule.Name,
                    scheduleContent = newSchedule.Content,
                    location = newSchedule.Location,
                    startTime = newSchedule.StartTime,
                    endTime = newSchedule.EndTime,
                    repeatDaily = newSchedule.RepeatDaily,
                    repeatWeekly = newSchedule.RepeatWeekly,
                    participatorIds = ids,
                    code = 200
                };

            }
        }
        
    }
}