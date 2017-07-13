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
            var body = Helper.Decode(json);
            var scheduleName = body["scheduleName"];
            var scheduleContent = body["scheduleContent"];
            var userId = int.Parse(body["userId"]);
            var projectId = int.Parse(body["projectId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(u => u.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");
                var theUser = queryUser.Single();

                if (!Helper.CheckPermission(projectId, userId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + userId + "无操作权限");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "日程所属项目不存在");

                var newSchedule = new Schedule
                {
                    Users = new List<User> {theUser},
                    Content = scheduleContent,
                    EndTime = body.ContainsKey("endTime")
                        ? DateTime.Parse(body["endTime"])
                        : (body.ContainsKey("startTime") ? DateTime.Parse(body["startTime"]) : DateTime.Now),
                    StartTime = body.ContainsKey("startTime") ? DateTime.Parse(body["startTime"]) : DateTime.Now,
                    Location = body.ContainsKey("location") ? body["location"] : "",
                    Name = scheduleName,
                    OwerId = userId,
                    RepeatDaily = body.ContainsKey("repeatDaily") && bool.Parse(body["repeatDaily"]),
                    RepeatWeekly = body.ContainsKey("repeatWeekly") && bool.Parse(body["repeatWeekly"]),
                    ProjectId = projectId
                };
                context.Schedules.Add(newSchedule);
                newSchedule.Users.Add(theUser);
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
//                    participatorIds = ids
                };

                return Helper.BuildResult(data);
            }
        }

        public static object DeleteSchedule(object json)
        {
            var body = Helper.DecodeToObject(json);
            var scheduleId = int.Parse(body["scheduleId"].ToString());
            var userId = int.Parse(body["userId"].ToString());

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(u => u.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "删除日程不存在");
                var theSchedule = querySchedule.Single();

                var theProject = context.Projects.Single(project => project.Id == theSchedule.ProjectId);
                Console.WriteLine(userId);
                Console.WriteLine(theSchedule.OwerId);

                if (!Helper.CheckPermission(theProject.Id, userId, userId == theSchedule.OwerId,
                    OperationType.DELETE))
                    return Helper.Error(401, "用户无操作权限");

                theProject.Schedules.Remove(theSchedule);
                context.Schedules.Remove(theSchedule);

                context.SaveChanges();

                return Helper.BuildResult(null);
            }
        }

        public static object UpdateSchedule(object json)
        {
            var body = Helper.Decode(json);
            var scheduleId = int.Parse(body["scheduleId"]);
            var userId = int.Parse(body["userId"]);

            using (var context = new BackendContext())
            {
                var queryUser = context.Users.Where(u => u.Id == userId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户不存在");

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "修改的日程不存在");
                var theSchedule = querySchedule.Single();

                var theProject = context.Projects.Single(project => project.Id == theSchedule.ProjectId);
                Console.WriteLine(userId);
                Console.WriteLine(theSchedule.OwerId);

                if (!Helper.CheckPermission(theProject.Id, userId, userId == theSchedule.OwerId,
                    OperationType.PUT))
                    return Helper.Error(401, "用户无操作权限");

                theSchedule.Content =
                    body.ContainsKey("scheduleContent") ? body["scheduleContent"] : theSchedule.Content;
                theSchedule.EndTime =
                    body.ContainsKey("endTime") ? DateTime.Parse(body["endTime"]) : theSchedule.EndTime;
                theSchedule.StartTime = body.ContainsKey("startTime")
                    ? DateTime.Parse(body["startTime"])
                    : theSchedule.StartTime;
                theSchedule.Location =
                    body.ContainsKey("location") ? body["location"] : theSchedule.Location;
                theSchedule.Name = body.ContainsKey("scheduleName") ? body["scheduleName"] : theSchedule.Name;
                theSchedule.RepeatDaily =
                    body.ContainsKey("repeatDaily") ? bool.Parse(body["repeatDaily"]) : theSchedule.RepeatDaily;
                theSchedule.RepeatWeekly =
                    body.ContainsKey("repeatWeekly") ? bool.Parse(body["repeatWeekly"]) : theSchedule.RepeatWeekly;

                context.SaveChanges();

                var data = new
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
                    return Helper.Error(404, "日程不存在");
                }
                var theSchedule = querySchedule.Single();

                var data = new
                {
                    scheduleId = theSchedule.Id,
                    scheduleName = theSchedule.Name,
                    scheduleContent = theSchedule.Content,
                    location = theSchedule.Location,
                    startTime = theSchedule.StartTime,
                    endTime = theSchedule.EndTime,
                    repeatDaily = theSchedule.RepeatDaily,
                    repeatWeekly = theSchedule.RepeatWeekly,
                    participatorsId = (from user in theSchedule.Users
                        select new
                        {
                            id = user.Id,
                            email = user.Email,
                            name = user.UserInfo.Name,
                            address = user.UserInfo.Address,
                            gender = user.UserInfo.Gender,
                            phonenumber = user.UserInfo.Phonenumber,
                            job = user.UserInfo.Job,
                            website = user.UserInfo.Website,
                            birthday = user.UserInfo.Birthday
                        }).ToArray()
                };
                return Helper.BuildResult(data);
            }
        }

        public static object GetScheduleList(int projectId)
        {
            using (var context = new BackendContext())
            {
                if (!context.Projects.Any(p => p.Id == projectId))
                    return Helper.Error(404, "项目不存在");
                var scheduleList = (from schedule in context.Schedules
                    where schedule.ProjectId == projectId
                    select new
                    {
                        scheduleId = schedule.Id,
                        scheduleName = schedule.Name,
                        startTime = schedule.StartTime,
                        endTime = schedule.EndTime,
                        repeatDaily = schedule.RepeatDaily,
                        repeatWeekly = schedule.RepeatWeekly
                    }).ToArray();

                var data = new
                {
                    scheduleList
                };

                return Helper.BuildResult(data);
            }
        }

        public static object AddParticipator(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"]);
                var participatorId = int.Parse(body["participatorId"]);
                var userId = int.Parse(body["userId"]);


                var querySchedule = context.Schedules.Where(s => s.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "日程不存在");
                var theSchedule = querySchedule.Single();

                var queryUser = context.Users.Where(user => user.Id == participatorId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户" + participatorId + "不存在");
                var theUser = queryUser.Single();

                var queryParticipator = context.Users.Where(user => user.Id == participatorId);
                if (!queryParticipator.Any())
                    return Helper.Error(404, "用户" + participatorId + "不存在");
                var theParticipator = queryParticipator.Single();

                //check permission
                if (!Helper.CheckPermission(scheduleId, userId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + userId + "无操作权限");

                if (theSchedule.Users.Contains(queryUser.Single()))
                    return Helper.Error(417, "参与者" + participatorId + "已存在");

                theSchedule.Users.Add(theParticipator);
                context.SaveChanges();

                var data = new
                {
                    scheduleId = theSchedule.Id,
                    scheduleName = theSchedule.Name,
                    scheduleContent = theSchedule.Content,
                    location = theSchedule.Location,
                    startTime = theSchedule.StartTime,
                    endTime = theSchedule.EndTime,
                    repeatDaily = theSchedule.RepeatDaily,
                    repeatWeekly = theSchedule.RepeatWeekly,
                    participatorsId = (from user in theSchedule.Users
                        select new
                        {
                            id = user.Id,
                            email = user.Email,
                            name = user.UserInfo.Name,
                            address = user.UserInfo.Address,
                            gender = user.UserInfo.Gender,
                            phonenumber = user.UserInfo.Phonenumber,
                            job = user.UserInfo.Job,
                            website = user.UserInfo.Website,
                            birthday = user.UserInfo.Birthday
                        }).ToArray()
                };
                return Helper.BuildResult(data);
            }
        }

        public static object DeleteParticipator(object json)
        {
            var body = Helper.Decode(json);

            using (var context = new BackendContext())
            {
                var scheduleId = int.Parse(body["scheduleId"]);
                var participatorId = int.Parse(body["participatorId"]);
                var userId = int.Parse(body["userId"]);


                var querySchedule = context.Schedules.Where(s => s.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(404, "日程不存在");
                var theSchedule = querySchedule.Single();

                var queryUser = context.Users.Where(user => user.Id == participatorId);
                if (!queryUser.Any())
                    return Helper.Error(404, "用户" + participatorId + "不存在");
                var theUser = queryUser.Single();

                var queryParticipator = context.Users.Where(user => user.Id == participatorId);
                if (!queryParticipator.Any())
                    return Helper.Error(404, "用户" + participatorId + "不存在");
                var theParticipator = queryParticipator.Single();

                //check permission
                if (!Helper.CheckPermission(scheduleId, userId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + userId + "无操作权限");

                if (!theSchedule.Users.Contains(queryUser.Single()))
                    return Helper.Error(417, "参与者" + participatorId + "未参与");
                var data = new
                {
                    scheduleId = theSchedule.Id,
                    scheduleName = theSchedule.Name,
                    scheduleContent = theSchedule.Content,
                    location = theSchedule.Location,
                    startTime = theSchedule.StartTime,
                    endTime = theSchedule.EndTime,
                    repeatDaily = theSchedule.RepeatDaily,
                    repeatWeekly = theSchedule.RepeatWeekly,
                    participatorsId = (from user in theSchedule.Users
                        select new
                        {
                            id = user.Id,
                            email = user.Email,
                            name = user.UserInfo.Name,
                            address = user.UserInfo.Address,
                            gender = user.UserInfo.Gender,
                            phonenumber = user.UserInfo.Phonenumber,
                            job = user.UserInfo.Job,
                            website = user.UserInfo.Website,
                            birthday = user.UserInfo.Birthday
                        }).ToArray()
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
                    return Helper.Error(404, "日程不存在");
                }
                var theSchedule = querySchedule.Single();
                var data = new
                {
                    scheduleId = theSchedule.Id,
                    scheduleName = theSchedule.Name,
                    scheduleContent = theSchedule.Content,
                    location = theSchedule.Location,
                    startTime = theSchedule.StartTime,
                    endTime = theSchedule.EndTime,
                    repeatDaily = theSchedule.RepeatDaily,
                    repeatWeekly = theSchedule.RepeatWeekly,
                    participatorsId = (from user in theSchedule.Users
                        select new
                        {
                            id = user.Id,
                            email = user.Email,
                            name = user.UserInfo.Name,
                            address = user.UserInfo.Address,
                            gender = user.UserInfo.Gender,
                            phonenumber = user.UserInfo.Phonenumber,
                            job = user.UserInfo.Job,
                            website = user.UserInfo.Website,
                            birthday = user.UserInfo.Birthday
                        }).ToArray()
                };
                return Helper.BuildResult(data);
            }
        }

        public static object GetUserScheduleList(int userId, int projectId)
        {
            using (var context = new BackendContext())
            {
                if (!context.Projects.Any(p => p.Id == projectId))
                    return Helper.Error(404, "项目不存在");
                if (!context.Users.Any(u => u.Id == userId))
                    return Helper.Error(404, "用户不存在");
                if (!context.Projects.Any(p => p.Id == projectId && p.Users.Any(u => u.Id == userId)))
                    return Helper.Error(403, "用户未参与这个项目");
                var scheduleList = (from schedule in context.Schedules
                    where schedule.ProjectId == projectId && schedule.Users.Any(u => u.Id == userId)
                    select new
                    {
                        scheduleId = schedule.Id,
                        scheduleName = schedule.Name,
                        startTime = schedule.StartTime,
                        endTime = schedule.EndTime,
                        repeatDaily = schedule.RepeatDaily,
                        repeatWeekly = schedule.RepeatWeekly
                    }).ToArray();

                var data = new
                {
                    scheduleList
                };

                return Helper.BuildResult(data);
            }
        }
    }
}