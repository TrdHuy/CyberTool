using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.implement.utils
{
    public class JsonHelper
    {
        public static T? DeserializeObject<T>(string json)
        {
            T? items = JsonConvert.DeserializeObject<T>(json);
            return items;
        }

        public static string SerializeObject(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return json;
        }

        public static object? Decode(string json)
        {
            var items = JsonConvert.DeserializeObject(json);
            return items;
        }
    }
}
