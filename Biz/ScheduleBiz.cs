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
                
                //check permission
                if (!Helper.CheckPermission(projectId, creatorId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + creatorId + "无操作权限");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(401, "项目不存在");
                

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
                queryProject.Single().Schedules.Add(newSchedule);
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

        public static object DeleteSchedule(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"].ToString());
                var operatorId = int.Parse(body["operatorId"].ToString());

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(401, "删除日程不存在");
                var theSchedule = querySchedule.Single();

                var queryProject = context.Projects.Where(project => project.Id == querySchedule.Single().Id);
                if (!queryProject.Any())
                    return Helper.Error(401, "删除日程所属项目不存在");
                
                if(!Helper.CheckPermission(queryProject.Single().Id, operatorId, true, OperationType.DELETE))
                    return Helper.Error(401, "用户无操作权限");
                
                queryProject.Single().Schedules.Remove(theSchedule);
                context.Schedules.Remove(theSchedule);

                context.SaveChanges();

                return new
                {

                };
            }
        }
        
        public static object UpdateSchedule(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"].ToString());
                var operatorId = int.Parse(body["operatorId"].ToString());
                var scheduleName = body["scheduleName"].ToString();
                var scheduleContent = body["scheduleContent"].ToString();
                var location = body["location"].ToString();
                var startTime = DateTime.Parse(body["startTime"].ToString());
                var endTime = DateTime.Parse(body["endTime"].ToString());
                var repeatDaily = bool.Parse(body["repeatDaily"].ToString());
                var repeatWeekly = bool.Parse(body["repeatWeekly"].ToString()); 
                var participatorIds = JArray.Parse(body["participatorIds"].ToString());
                var projectId = int.Parse(body["projectId"].ToString());

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(401, "更新日程不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(401, "项目不存在");
                
                //var users = new List<User>();
                var ids = new List<int>();
                foreach (var participatorId in participatorIds)
                { 
                    var id = int.Parse(participatorId.ToString());
                    var queryUser = context.Users.Where(user => user.Id == id);
                    
                    if (!queryUser.Any())
                        return Helper.Error(401, "用户" + participatorId + "不存在");
                    
                    //users.Add(queryUser.Single());
                    
                    ids.Add(id);
                 
                }
                
                if(!Helper.CheckPermission(projectId, operatorId, true, OperationType.PUT))
                    return Helper.Error(401, "用户无操作权限");
                
                var theSchedule = querySchedule.Single();
                
                var theProject = queryProject.Single();
                

                theSchedule.Content = scheduleContent;
                theSchedule.EndTime = endTime;
                theSchedule.StartTime = startTime;
                theSchedule.Location = location;
                theSchedule.Name = scheduleName;
                theSchedule.Project = theProject;
                theSchedule.ProjectId = projectId;
                theSchedule.RepeatDaily = repeatDaily;
                theSchedule.RepeatWeekly = repeatWeekly;
                //theSchedule.Users = users;

                context.SaveChanges();

                return new
                {
                    scheduleId = theSchedule.Id,
                    scheduleName = theSchedule.Name,
                    scheduleContent = theSchedule.Content,
                    location = theSchedule.Location,
                    startTime = theSchedule.StartTime,
                    endTime = theSchedule.EndTime,
                    repeatDaily = theSchedule.RepeatDaily,
                    repeatWeekly = theSchedule.RepeatWeekly,
                    participatorIds = ids,
                    code = 200
                };
            }
        }
        
        public static object GetSchedule(object json)
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
                
                //check permission
                if (!Helper.CheckPermission(projectId, creatorId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + creatorId + "无操作权限");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(401, "项目不存在");
                

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
                queryProject.Single().Schedules.Add(newSchedule);
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