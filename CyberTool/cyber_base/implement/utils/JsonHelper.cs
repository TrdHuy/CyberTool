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
            try
            {
                T? items = JsonConvert.DeserializeObject<T>(json);
                return items;
            }
            catch
            {
                return (T?)Activator.CreateInstance(typeof(T));
            }
        }

        public static string SerializeObject(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return json;
        }

        public static object? Decode(string json)
        {
            var items = JsonConvert.DeserializeObject(json);
            return items;
        }

        public static object? DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

    }
}
