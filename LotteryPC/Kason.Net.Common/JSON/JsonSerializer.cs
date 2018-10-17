using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization.Json;

namespace Kason.Net.Common.JSON
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
    }
}
