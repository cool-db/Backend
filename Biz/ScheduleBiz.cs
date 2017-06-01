using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Model;
using System.Linq;

namespace Backend.Biz
{
    public class ScheduleBiz
    {
        public static object CreateSchedule(object json)
        {
            var body = Helper.Decode(json);
            
            using (var context = new BackendContext())
            {
                var scheduleName = body["scheduleName"];
                Console.WriteLine(scheduleName);
                var scheduleContent = body["scheduleContent"];
                var location = body["location"];
                var startTime = body["startTime"];
                var endTime = body["endTime"];
                var repeatDaily = body["repeatDaily"];
                var repeatWeekly = body["repeatWeekly"];
                var participatorIds = Helper.DecodeToList(body["participatorIds"]);
                Console.WriteLine(participatorIds);
                var creatorId = body["creatorId"];

                var users = new List<User>();
                var ids = new List<int>();
                foreach (var participatorId in participatorIds)
                {
                    var queryUser = context.Users.Where(user => user.Id == int.Parse(participatorId));
                    
                    if (!queryUser.Any())
                        return Helper.Error(401, "用户" + participatorId + "不存在");
                    
                    users.Add(queryUser.Single());
                    ids.Add(int.Parse(participatorId));
                }

                //var users = participatorIds.Select(participatorId => context.Users.Single(user => user.Id == int.Parse(participatorId))).ToList();
                var newSchedule = new Schedule
                {
                    Users = users,
                    Content = scheduleContent,
                    EndTime = DateTime.Parse(endTime),
                    StartTime = DateTime.Parse(startTime),
                    Location = location,
                    Name = scheduleName,
                    OwerId = int.Parse(creatorId),
                    RepeatDaily = bool.Parse(repeatDaily),
                    RepeatWeekly = bool.Parse(repeatWeekly)
                    
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