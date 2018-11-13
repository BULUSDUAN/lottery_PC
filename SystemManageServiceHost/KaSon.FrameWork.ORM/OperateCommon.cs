using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KaSon.FrameWork.ORM
{
    /// <summary>
    /// 链接字符串 操作类
    /// </summary>
    public static class OperateCommon
    {
        public const string AesIv = "8zXcR7dfyraBTrNUNfWC2A==";
        private static List<OrmConfigInfo> OrmConfigInfoList = null;
        public const string SqlServerDecrypt = "CONVERT(nvarchar(800),DecryptByKey([{0}])) As [{0}]";
        public const string SqlServerEncrypt = "EncryptByKey(Key_GUID('mbskey_cusdata'),{0})";
        public const string SqlServerOpenKey = "OPEN SYMMETRIC KEY mbskey_cusdata DECRYPTION BY CERTIFICATE cert_mbs; ";

        public static object AutoConvert(object value, Type type)
        {
            if (value == null)
            {
                return null;
            }
            Type type2 = value.GetType();
            if (type.IsEnum)
            {
                type = typeof(int);
            }
            if (type == type2)
            {
                return value;
            }
            string str = Convert.ToString(value);
            if (type.Name == "String")
            {
                return str;
            }
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            if (str.Length == 0)
            {
                if (type == typeof(bool))
                {
                    return false;
                }
                if (type == typeof(DateTime))
                {
                    return null;
                }
                if (type == typeof(int))
                {
                    return 0;
                }
                if (type == typeof(long))
                {
                    return 0L;
                }
                if (type == typeof(decimal))
                {
                    return 0M;
                }
                if (type == typeof(double))
                {
                    return 0.0;
                }
            }
            if (((type == typeof(int)) || (type == typeof(long))) || (type == typeof(byte)))
            {
                return type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { string.Format("{0:0}", value) });
            }
            if ((type == typeof(bool)) && (((((type2 == typeof(byte)) || (type2 == typeof(int))) || ((type2 == typeof(decimal)) || (type2 == typeof(float)))) || (type2 == typeof(float))) || (type2 == typeof(long))))
            {
                return (Convert.ToInt64(string.Format("{0:0}", value)) != 0L);
            }
            if (type == typeof(bool))
            {
                return (str.ToUpper().Equals("FALSE") ? ((object)0) : ((object)!str.Equals("0")));
            }
            return type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { str });
        }

        public static OrmConfigInfo GetConnInfo(string dbKey)
        {
            OrmConfigInfo info = null;

            //连接字符串配置信息
            List<OrmConfigInfo> ormConfigInfoList = GetOrmConfigInfoList();

            foreach (OrmConfigInfo info2 in ormConfigInfoList)
            {
                if (info2.DbKey.ToLower() == dbKey.ToLower())
                {
                    info =info2;
                    break;
                }
            }
            //if (info == null)
            //{
            //    info = GetInfo(dbKey);
            //}
            if (info == null)
            {
                throw new ApplicationException("无法找到DBKey为:" + dbKey + "的数据库连接，请检查数据库连接配置信息！");
            }
            info.DbKey = dbKey;
            return info;
        }

        /// <summary>
        /// 这只配置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool SetConfigInfo(List<OrmConfigInfo> list) {

            if (list.Count <= 0) return false;
            OrmConfigInfoList = list;
            return true;
        }
        //private static OrmConfigInfo GetInfo(string dbKey)
        //{
        //    return ServiceContainer.GetConfiger().Get<OrmConfigInfo>(dbKey);
        //}
        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <returns></returns>
        public static List<OrmConfigInfo> GetOrmConfigInfoList()
        {
          //  Console.WriteLine("OrmConfigInfo" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            if (OrmConfigInfoList == null)
            {
                string path = Directory.GetCurrentDirectory() + "KaSon.FrameWork.ORM.Config.xml";
                if (!File.Exists(path))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), @"config\KaSon.FrameWork.ORM.Config.xml");
                }
                using (FileStream stream = File.OpenRead(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<OrmConfigInfo>));
                    OrmConfigInfoList = serializer.Deserialize(stream) as List<OrmConfigInfo>;
                }
            }
           // Console.WriteLine("OrmConfigInfo2" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return OrmConfigInfoList;
        }
    }
}
