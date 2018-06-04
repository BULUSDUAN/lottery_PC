
using Newtonsoft.Json;

using System;

namespace KaSon.FrameWork.Helper
{
 
    public static class JsonHelper
    {
        private static readonly JsonConverter[] JavaScriptConverters = new JsonConverter[] { new DateTimeConverter() };

        public static T Deserialize<T>(string json)
        {
            return (!string.IsNullOrWhiteSpace(json) ? JsonConvert.DeserializeObject<T>(json) : default(T));
        }

        public static object Deserialize(string json, Type targetType)
        {
            return JsonConvert.DeserializeObject(json, targetType);
        }

        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, JavaScriptConverters);
        }
    }
}

