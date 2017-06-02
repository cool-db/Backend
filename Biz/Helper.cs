using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Model;
using Newtonsoft.Json;

namespace Backend.Biz
{
    public enum OperationType
    {
        PUT,
        GET,
        POST,
        DELETE
    }
    
    public class Helper
    {
        public bool IsRebuild;

        public static void Log(string message)
        {
            Console.WriteLine("EF Message: {0} ", message);
        }

        public static Dictionary<string, string> Decode(object json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToString());
        }

        public static List<object> DecodeToList(object json)
        {
            return JsonConvert.DeserializeObject<List<object>>(json.ToString());
        }

        public static object Error(int code, string message)
        {
            return new
            {
                code,
                message
            };
        }

        public static bool? ParseBool(string boolString)
        {
            if (boolString == "")
                return null;
            return bool.Parse(boolString);
        }

        public static DateTime? ParseDateTime(string dateTimeString)
        {
            if (dateTimeString == "")
                return null;
            return DateTime.Parse(dateTimeString);
        }

        public static bool CheckPermission(int projectId, int userId , bool isOwner , OperationType type)
        {
            if (isOwner)
                return true;
            
            using (var context = new BackendContext())
            {
                var project = (from p in context.Projects
                    where p.Id == projectId
                    select p).FirstOrDefault();

                var permission = (from p in project.UserPermissons
                    where p.UserId == userId
                    select p).FirstOrDefault().Permission;

                switch (type)
                {
                     case OperationType.GET:
                         return true;
                     case OperationType.POST:
                         return true;
                     case OperationType.DELETE: 
                     case OperationType.PUT:
                         if (permission != Permission.Participant)
                             return true;
                         else
                             return false;
                }
            }

            return false;
        }
    }
}