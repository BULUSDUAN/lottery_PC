using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.JSON
{
    public class JsonHelper
    {
        public static string Serialize(object obj)
        {
           // JavaScriptSerializer serializer = new JavaScriptSerializer();
            return JsonConvert.SerializeObject(obj);
        }
        public static object Deserialize(string json)
        {
          //  JavaScriptSerializer serializer = new JavaScriptSerializer();
            return JsonConvert.DeserializeObject(json);
        }
        public static T Deserialize<T>(string json) where T : class
        {
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static dynamic Decode(string data)
        {
            return JsonConvert.DeserializeObject<dynamic>(data);
        }
        public static JObject DecodeJObject(string data)
        {
            return (JObject)JsonConvert.DeserializeObject(data);
        }
        // (JObject)JsonConvert.DeserializeObject(jsonText)
    }
}
