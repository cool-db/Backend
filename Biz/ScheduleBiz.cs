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
                var users = new List<User>();
                var ids = new List<int>();

                if (!Helper.CheckPermission(projectId, userId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + userId + "无操作权限");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "日程所属项目不存在");

                var newSchedule = new Schedule
                {
                    Users = users,
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
                    participatorIds = ids,
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
                var userId = int.Parse(body["userId"].ToString());

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
            var body = Helper.DecodeToObject(json);
            var scheduleId = int.Parse(body["scheduleId"].ToString());
            var userId = int.Parse(body["userId"].ToString());

            using (var context = new BackendContext())
            {
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
                    return Helper.Error(404, "更新日程不存在");

                var queryProject = context.Projects.Where(project => project.Id == projectId);
                if (!queryProject.Any())
                    return Helper.Error(404, "更新日程所属项目不存在");

                if (!Helper.CheckPermission(projectId, userId, true, OperationType.PUT))
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
                    code = 200
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
                var data = new
                {
                    dataList,
                    code = 200
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
                        repeatWeekly = schedule.RepeatWeekly,
                    }).ToArray();

                var data = new
                {
                    scheduleList,
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
                var operatorId = int.Parse(body["operatorId"].ToString());

                var querySchedule = context.Schedules.Where(Schedule => Schedule.Id == scheduleId);
                if (!querySchedule.Any())
                {
                    return Helper.Error(404, "日程不存在");
                }

                var schedule = querySchedule.Single();

                //check permission
                if (!Helper.CheckPermission(scheduleId, operatorId, true, OperationType.POST))
                    return Helper.Error(401, "用户" + operatorId + "无操作权限");

                foreach (var participatorId in participatorIds)
                {
                    var id = int.Parse(participatorId.ToString());
                    var queryUser = context.Users.Where(user => user.Id == id);

                    if (!queryUser.Any())
                    {
                        return Helper.Error(404, "用户" + participatorId + "不存在");
                    }

                    if (schedule.Users.Contains(queryUser.Single()))
                    {
                        return Helper.Error(417, "参与者" + participatorId + "已存在");
                    }

                    schedule.Users.Add(queryUser.Single());
                }
                context.SaveChanges();

                var ids = new List<int>();
                foreach (var participator in schedule.Users)
                {
                    ids.Add(participator.Id);
                }

                var data = new
                {
                    ids,
                    code = 200
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
                var operatorId = int.Parse(body["operatorId"].ToString());

                var querySchedule = context.Schedules.Where(schedule => schedule.Id == scheduleId);
                if (!querySchedule.Any())
                    return Helper.Error(401, "删除日程不存在");
                var theSchedule = querySchedule.Single();

                if (!Helper.CheckPermission(theSchedule.Id, operatorId, true, OperationType.DELETE))
                    return Helper.Error(401, "用户无操作权限");

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
                        return Helper.Error(404, "用户" + participatorId + "未参与该日程");
                    }

                    if (id == theSchedule.OwerId)
                    {
                        return Helper.Error(401, "无法删除日程创建者");
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
                    ids,
                    code = 200
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

                var participatorList = new List<object>();

                foreach (var participator in theSchedule.Users)
                {
                    participatorList.Add(new
                    {
                        participatorId = participator.Id,
                        participatorName = participator.UserInfo.Name
                    });
                }

                var data = new
                {
                    participatorList,
                    code = 200
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
                        repeatWeekly = schedule.RepeatWeekly,
                    }).ToArray();

                var data = new
                {
                    scheduleList,
                };

                return Helper.BuildResult(data);
            }
        }
    }
}