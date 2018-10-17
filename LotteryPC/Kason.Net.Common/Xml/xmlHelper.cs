using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Kason.Net.Common.Xml
{
    public class xmlHelper
    {


        //private static OrmConfigInfo GetInfo(string dbKey)
        //{
        //    return ServiceContainer.GetConfiger().Get<OrmConfigInfo>(dbKey);
        //}
        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <returns></returns>
        public static IList<T> SerializerList<T>(string path) where T : new()
        {
            T item = new T();
            IList<T> InfoList = new List<T>();
            //  Console.WriteLine("OrmConfigInfo" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));


            if (!File.Exists(path))
            {
                return InfoList;
            }
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                var list= serializer.Deserialize(stream);
                InfoList = list as List<T>;
            }

            // Console.WriteLine("OrmConfigInfo2" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return InfoList;
        }

    }
}
