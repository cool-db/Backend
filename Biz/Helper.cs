using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend.Biz
{
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

        public static List<string> DecodeToList(object json)
        {
            return JsonConvert.DeserializeObject<List<string>>(json.ToString());
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
    }
}