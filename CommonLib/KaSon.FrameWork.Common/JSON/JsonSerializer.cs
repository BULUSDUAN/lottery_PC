using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;

namespace KaSon.FrameWork.Common.JSON
{
    public static class JsonSerializer
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string Serialize<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, t);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return jsonString;
            }
        }
        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T Deserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }

        public static T DeserializeOldDate<T>(string json)
        {
            JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            //忽略空值
            jsSettings.NullValueHandling = NullValueHandling.Ignore;
            var result = JsonConvert.DeserializeObject<T>(json, jsSettings);
            return result;
        }
    }
}
