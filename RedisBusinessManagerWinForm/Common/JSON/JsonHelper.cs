using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Common.JSON
{
    public class JsonHelper
    {
        public static string Serialize(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        public static object Deserialize(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.DeserializeObject(json);
        }
        public static T Deserialize<T>(string json) where T : class
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}
