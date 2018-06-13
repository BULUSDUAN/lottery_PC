
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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
    //System.Web.Helper
    public static class WebHelper
    {
        public static dynamic Decode(string value)
        {
            return WrapObject(JsonConvert.DeserializeObject(value));
        }

        public static dynamic WrapObject(object value)
        {
            IDictionary<string, object> dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                return new DynamicJsonObject(dictionary);
            }
            object[] array = value as object[];
            if (array != null)
            {
                return new DynamicJsonArray(array);
            }
            return value;
        }
        private class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _values;
            public DynamicJsonObject(IDictionary<string, object> values)
            {
                _values = ((IEnumerable<KeyValuePair<string, object>>)values)
                    .ToDictionary((Func<KeyValuePair<string, object>, string>)((KeyValuePair<string, object> p) => p.Key),
                    (Func<KeyValuePair<string, object>, object>)((KeyValuePair<string, object> p) => WebHelper.WrapObject(p.Value)), 
                    (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            }
        }
        private class DynamicJsonArray : DynamicObject
        {
            private readonly object[] _arrayValues;
            public DynamicJsonArray(object[] arrayValues)
            {
                _arrayValues = ((IEnumerable<object>)arrayValues).Select((Func<object, object>)WrapObject).ToArray();
            }
        }
    } 
}

