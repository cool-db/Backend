using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend.Biz
{
    public class Helper
    {
        public static Dictionary<string, string> Decode(object json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToString());
        }
        
        public static List<object> DecodeToList(object json)
        {
            return JsonConvert.DeserializeObject<List<object>>(json.ToString());
        }
    }
}