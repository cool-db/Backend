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
                var token = body["token"].ToString();
                var projectId = int.Parse(body["projectId"].ToString());
                
                var users = new List<User>();
                var ids = new List<int>();
                foreach (var participatorId in participatorIds)
                { 
                    var id = int.Parse(participatorId.ToString());
                    var queryUser = context.Users.Where(user => user.Id == id);

                    if (!queryUser.Any())
                        return Helper.Error(404, "用户" + participatorId + "不存在");
                    
                    users.Add(queryUser.Single());
                    
                    ids.Add(id);
                 
                }
                
                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "日程所属项目不存在");

                var queryCreator = context.Users.Where(creator => creator.Token == token);
                if (!queryCreator.Any())
                    return Helper.Error(403, "token错误");

                if (!queryProject.Single().Users.Contains(queryCreator.Single()))
                    return Helper.Error(403, "用户无权限");

                //var users = participatorIds.Select(participatorId => context.Users.Single(user => user.Id == int.Parse(participatorId))).ToList();
                var newSchedule = new Schedule
                {
                    Users = users,
                    Content = scheduleContent,
                    EndTime = endTime,
                    StartTime = startTime,
                    Location = location,
                    Name = scheduleName,
                    OwerId = queryCreator.Single().Id,
                    RepeatDaily = repeatDaily,
                    RepeatWeekly = repeatWeekly,
                    ProjectId = projectId
                };
                context.Schedules.Add(newSchedule);
                queryProject.Single().Schedules.Add(newSchedule);
                context.SaveChanges();

                var data = new
                {
                    scheduleId = newSchedule.Id,
                    scheduleName = newSchedule.Name,
                    scheduleContent = newSchedule.Content,
                    location = newSchedule.Location,
                    startTime = newSchedule.StartTime,
                    endTime = newSchedule.EndTime,
                    repeatDaily = newSchedule.RepeatDaily,
                    repeatWeekly = newSchedule.RepeatWeekly,
                    participatorIds = ids
                };

                return Helper.BuildResult(data);

            }
        }

        public static object DeleteSchedule(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"].ToString());
                var token = body["token"].ToString();

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "删除日程不存在");
                var theSchedule = querySchedule.Single();

                var queryProject = context.Projects.Where(project => project.Id == theSchedule.ProjectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "删除日程所属项目不存在");
                
                var queryOperator = context.Users.Where(Operator => Operator.Token == token);
                if (!queryOperator.Any())
                    return Helper.Error(403, "token错误");

                if (!Helper.CheckPermission(queryProject.Single().Id, queryOperator.Single().Id, queryProject.Single().OwnerId == queryOperator.Single().Id, OperationType.DELETE))
                    return Helper.Error(403, "用户无操作权限");

                queryProject.Single().Schedules.Remove(theSchedule);
                context.Schedules.Remove(theSchedule);

                context.SaveChanges();

                return Helper.BuildResult(null);
            }
        }
        
        public static object UpdateSchedule(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"].ToString());
                var token = body["token"].ToString();
                var scheduleName = body["scheduleName"].ToString();
                var scheduleContent = body["scheduleContent"].ToString();
                var location = body["location"].ToString();
                var startTime = DateTime.Parse(body["startTime"].ToString());
                var endTime = DateTime.Parse(body["endTime"].ToString());
                var repeatDaily = bool.Parse(body["repeatDaily"].ToString());
                var repeatWeekly = bool.Parse(body["repeatWeekly"].ToString()); 
                var projectId = int.Parse(body["projectId"].ToString());

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error( 404, "更新日程不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "更新日程所属项目不存在");

                var queryOperator = context.Users.Where(user => user.Token == token);
                if (!queryOperator.Any())
                    return Helper.Error(403, "token错误");
                
                if(!Helper.CheckPermission(projectId, queryOperator.Single().Id, queryProject.Single().OwnerId == queryOperator.Single().Id, OperationType.PUT))
                    return Helper.Error(403, "用户无操作权限");
                
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

                context.SaveChanges();

                var data =  new
                {
                    scheduleId = theSchedule.Id,
                    scheduleName = theSchedule.Name,
                    scheduleContent = theSchedule.Content,
                    location = theSchedule.Location,
                    startTime = theSchedule.StartTime,
                    endTime = theSchedule.EndTime,
                    repeatDaily = theSchedule.RepeatDaily,
                    repeatWeekly = theSchedule.RepeatWeekly
                };

                return Helper.BuildResult(data);
            }
        }
        
        public static object GetSchedule(int scheduleId)
        { 
            using (var context = new BackendContext())
            {
                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);

                if (!querySchedule.Any())
                {
                    return Helper.BuildResult(null, 404, "日程不存在");
                }

                var dataList = new List<object>();
                
                foreach (var theSchedule in querySchedule)
                { 
                    var ids = new List<int>();

                    foreach (var user in theSchedule.Users)
                    {
                        ids.Add(user.Id);
                    }
                    
                    dataList.Add(new
                    {
                        scheduleId = theSchedule.Id,
                        scheduleName = theSchedule.Name,
                        scheduleContent = theSchedule.Content,
                        location = theSchedule.Location,
                        startTime = theSchedule.StartTime,
                        endTime = theSchedule.EndTime,
                        repeatDaily = theSchedule.RepeatDaily,
                        repeatWeekly = theSchedule.RepeatWeekly,
                        participatorsId = ids
                        
                    });
                 
                }

                var data =  new
                {
                    dataList,
                };

                return Helper.BuildResult(data);

            }
        }
        
        public static object GetScheduleList(int userId)
        {
            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(user => user.Id == userId);
                
                if (!queryUser.Any())
                {
                    return Helper.BuildResult(null, 404, "用户不存在");
                }

                var theUser = queryUser.Single();

                var querySchedule = theUser.Schedules;

                if (!querySchedule.Any())
                {
                    return Helper.BuildResult(null, 404, "用户未参与或创建日程");
                }

                var scheduleList = new List<object>();
                
                foreach (var schedule in querySchedule)
                { 
                    
                    scheduleList.Add(new
                    {
                        scheduleId = schedule.Id,
                        scheduleName = schedule.Name,
                        startTime = schedule.StartTime,
                        endTime = schedule.EndTime,
                        repeatDaily = schedule.RepeatDaily,
                        repeatWeekly = schedule.RepeatWeekly,
                        
                    });
                 
                }

                var data = new
                {
                    scheduleList
                };

                return Helper.BuildResult(data);
            }
        }
        
        public static object AddParticipator(object json)
        {
            var body = Helper.DecodeToObject(json);
            
            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"].ToString());
                var participatorIds = JArray.Parse(body["participatorIds"].ToString());
                var token = body["token"].ToString();
                
                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                {
                    return Helper.Error( 404, "日程不存在");
                }
                var theSchedule = querySchedule.Single();

                var queryProject = context.Projects.Where(project => project.Id == theSchedule.ProjectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "日程所属项目不存在");

                var queryOperator = context.Users.Where(user => user.Token == token);
                if (!queryOperator.Any())
                    return Helper.Error(403, "token错误");
                
                //check permission
                if (!Helper.CheckPermission(theSchedule.ProjectId, queryOperator.Single().Id, queryProject.Single().OwnerId == queryOperator.Single().Id, OperationType.POST))
                    return Helper.Error(403, "用户无操作权限");
                
                foreach (var participatorId in participatorIds)
                {
                    var id = int.Parse(participatorId.ToString());
                    var queryUser = context.Users.Where(user => user.Id == id);

                    if (!queryUser.Any())
                    {
                        return Helper.Error(404, "用户" + participatorId + "不存在");
                    }

                    if (theSchedule.Users.Contains(queryUser.Single()))
                    {
                        return Helper.Error(417, "参与者" + participatorId + "已存在");
                    }
                    
                    theSchedule.Users.Add(queryUser.Single());
                    
                }
                context.SaveChanges();
                
                var ids = new List<int>();
                foreach (var participator in theSchedule.Users)
                {
                    ids.Add(participator.Id);
                }

                var data = new
                {
                    ids
                };

                return Helper.BuildResult(data);
            }
        }
        
        public static object DeleteParticipator(object json)
        {
            var body = Helper.DecodeToObject(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"].ToString());
                var participatorIds = JArray.Parse(body["participatorIds"].ToString());
                var token = body["token"].ToString();

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "删除日程不存在");
                var theSchedule = querySchedule.Single();

                var queryProject = context.Projects.Where(project => project.Id == theSchedule.ProjectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "日程所属项目不存在");
                
                var queryOperator = context.Users.Where(user => user.Token == token);
                if (!queryOperator.Any())
                    return Helper.Error(403, "token错误");
                
                //check permission
                if (!Helper.CheckPermission(theSchedule.ProjectId, queryOperator.Single().Id, queryProject.Single().OwnerId == queryOperator.Single().Id, OperationType.POST))
                    return Helper.Error(403, "用户无操作权限");

                foreach (var participatorId in participatorIds)
                {
                    var id = int.Parse(participatorId.ToString());
                    var queryUser = context.Users.Where(user => user.Id == id);

                    if (!queryUser.Any())
                    {
                        return Helper.Error(404, "用户" + participatorId + "不存在");
                    }
                    
                    var theUser = queryUser.Single();
                    if (!theSchedule.Users.Contains(theUser))
                    {
                        return Helper.Error( 404, "用户" + participatorId + "未参与该日程");
                    }
                    
                    if (id == theSchedule.OwerId)
                    {
                        return Helper.Error( 404, "无法删除日程创建者");
                    }

                    theSchedule.Users.Remove(theUser);

                }
                context.SaveChanges();
                
                var ids = new List<int>();
                foreach (var user in theSchedule.Users)
                {
                    ids.Add(user.Id);
                }

                var data = new
                {
                    ids
                };
                
                return Helper.BuildResult(data);
            }
        }
        
        public static object GetParticipatorList(int scheduleId)
        {
            using (var context = new BackendContext())
            {
                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                
                if (!querySchedule.Any())
                {
                    return Helper.Error( 404, "日程不存在");
                }

                var theSchedule = querySchedule.Single();

                var participatorList = new List<object>();
                
                foreach (var participator in theSchedule.Users)
                { 
                    
                    participatorList.Add(new
                    {
                        participatorId = participator.Id, 
                        participatorName = participator.UserInfo.Name
                        
                    });
                 
                }

                foreach (var id in participatorList)
                {
                    Console.WriteLine(id);
                }

                var data = new
                {
                    participatorList
                };

                return Helper.BuildResult(data);
            }
        }
    }
}