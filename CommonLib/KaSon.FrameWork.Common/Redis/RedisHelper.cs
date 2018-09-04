using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
//using StackExchange.Redis;
using System.Net;
using System.IO;
using KaSon.FrameWork.Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace KaSon.FrameWork.Common.Redis
{

    /// <summary>
    /// Redis数据库 CsRedisCode.RedisHelper
    /// </summary>
    public static class RedisHelper
    {

        static JObject RdConfigInfo = null;
        private static readonly object redisLock = new object();
        static RedisHelper()
        {
            Init();
        }

        public static void Init()
        {
            if (RdConfigInfo == null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\AllConfig.json");
                string jsonText = FileHelper.txtReader(path);
                var alljson = (JObject)JsonConvert.DeserializeObject(jsonText);
                RdConfigInfo = (JObject)JsonConvert.DeserializeObject(alljson["RedisConfig"].ToString());
                List<CSRedis.CSRedisConfig> list = new List<CSRedis.CSRedisConfig>(){
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=0,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=1,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=2,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=3,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=4,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=5,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=6,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=7,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=8,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=9,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=10,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=11,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=12,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=13,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=14,C_PoolSize=50,c_Writebuffer=10240 },
                     new CSRedis.CSRedisConfig(){ C_IP=ServerHost,C_Post=ServerPort,C_Password=ServerPassword,C_Defaultdatabase=15,C_PoolSize=50,c_Writebuffer=10240 }
                };
                var csredis = new CSRedis.CSRedisClient(list);
                CsRedisCode.RedisHelper.Initialization(csredis,
value => Newtonsoft.Json.JsonConvert.SerializeObject(value),
deserialize: (data, type) => Newtonsoft.Json.JsonConvert.DeserializeObject(data, type));

                //DB_NoTicket_Order.Set("sss", new object(), 10);
            }

        }

        /// <summary>
        /// 是否启用Redis
        /// </summary>
        public static bool EnableRedis
        {
            get
            {
                try
                {
                    // var c =;
                    return bool.Parse(RdConfigInfo["EnableRedis"].ToString());
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 服务器ip
        /// </summary>
        public static string ServerHost
        {
            get
            {
                try
                {
                    return RdConfigInfo["RedisHost"].ToString();
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public static int ServerPort
        {
            get
            {
                try
                {
                    return int.Parse(RdConfigInfo["RedisPost"].ToString());
                }
                catch (Exception ex)
                {
                    return 6379;
                }
            }
        }

        /// <summary>
        /// 服务器密码
        /// </summary>
        public static string ServerPassword
        {
            get
            {
                try
                {
                    return RdConfigInfo["RedisPassword"].ToString();
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }


        /// <summary>
        /// 未出票的订单库
        /// </summary>
        public static CSRedis.RedisClient DB_NoTicket_Order
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/0";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 竞彩、传统、北单的比赛数据和比赛结果库
        /// </summary>
        public static CSRedis.RedisClient DB_Match
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/1";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 追号订单库
        /// </summary>
        public static CSRedis.RedisClient DB_Chase_Order
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/2";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 未结算订单的库(竞彩足球、竞彩篮球)
        /// </summary>
        public static CSRedis.RedisClient DB_Running_Order_JC
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/3";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 未结算订单的库(北京单场)
        /// </summary>
        public static CSRedis.RedisClient DB_Running_Order_BJDC
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/4";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 未结算订单的库(传统足球)
        /// </summary>
        public static CSRedis.RedisClient DB_Running_Order_CTZQ
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/5";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 未结算订单的库(低频数字彩)
        /// </summary>
        public static CSRedis.RedisClient DB_Running_Order_SCZ_DP
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/6";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 未结算订单的库(高频数字彩)
        /// </summary>
        public static CSRedis.RedisClient DB_Running_Order_SCZ_GP
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/7";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 配置、奖期、比赛、合买大厅、过关统计等
        /// </summary>
        public static CSRedis.RedisClient DB_CoreCacheData
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/8";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 用户绑定信息
        /// </summary>
        public static CSRedis.RedisClient DB_UserBindData
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/9";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 用户博客数据
        /// </summary>
        public static CSRedis.RedisClient DB_UserBlogData
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/10";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 订单详细数据
        /// </summary>
        public static CSRedis.RedisClient DB_SchemeDetail
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/11";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 用户余额
        /// </summary>
        public static CSRedis.RedisClient DB_UserBalance
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/12";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }

        /// <summary>
        /// 其它
        /// </summary>
        public static CSRedis.RedisClient DB_Other
        {
            get
            {
                string key = $"{ServerHost}:{ServerPort}/13";
                return CsRedisCode.RedisHelper.ClusterNodes[key].GetConnection().Client;
            }
        }


    }



    public static class SampleStackExchangeRedisExtensions
    {
        public static T GetObj<T>(this CSRedis.RedisClient cache, string key)
        {
            return Deserialize<T>(cache.GetBytes(key));
        }

        public static List<T> GetObjs<T>(this CSRedis.RedisClient cache, string key)
        {
            return Deserializes<T>(cache.GetBytes(key));
        }

        public static object GetObj(this CSRedis.RedisClient cache, string key)
        {
            return Deserialize<object>(cache.GetBytes(key));
        }

        public static void SetObj(this CSRedis.RedisClient cache, string key, object value)
        {
            cache.Set(key, Serialize(value));
        }

        public static void SetObj(this CSRedis.RedisClient cache, string key, object value, TimeSpan timeSpan)
        {
            cache.Set(key, Serialize(value), timeSpan);
        }

        public static List<T> GetRange<T>(this CSRedis.RedisClient cache, string key)
        {
            var index = cache.LLen(key);
            if (index == 0)
                return null;
            var array = cache.LRange(key, 0, index);
            List<T> list = new List<T>();
            foreach (var item in array)
            {
                list.Add(JsonHelper.Deserialize<T>(item.ToString()));
            }
            return list;
        }
        /// <summary>
        /// 插入到集合最后一条
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void SetRPush(this CSRedis.RedisClient cache, string key, object obj)
        {
            cache.RPush(key, obj);
        }
        /// <summary>
        /// 插入到集合第一条
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void SetLPush(this CSRedis.RedisClient cache, string key, object obj)
        {
            cache.RPush(key, obj);
        }



        static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(stream))
                {
                    T result = (T)binaryFormatter.Deserialize(memoryStream);
                    return result;
                }
            }
            catch
            {
                return default(T);
            }
        }

        static List<T> Deserializes<T>(byte[] stream)
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(stream))
                {
                    var result = binaryFormatter.Deserialize(memoryStream) as List<T>;
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
