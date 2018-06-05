using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using StackExchange.Redis;
using System.Net;

namespace Common.Lottery.Redis
{

    /// <summary>
    /// Redis数据库
    /// </summary>
    public static class RedisHelper
    {
        /// <summary>
        /// 是否启用Redis
        /// </summary>
        public static bool EnableRedis
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["EnableRedis"]);
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
                    return ConfigurationManager.AppSettings["RedisHost"];
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
                    return int.Parse(ConfigurationManager.AppSettings["RedisPost"]);
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
                    return ConfigurationManager.AppSettings["RedisPassword"];
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        private static ConnectionMultiplexer _instance;
        private static string _redisConectStr = ConfigurationManager.AppSettings["RedisConnect"];
        private static readonly object redisLock = new object();
        /// <summary>
        /// Redis实例
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                try
                {
                    lock (redisLock)
                    {
                        if (_instance == null || !_instance.IsConnected || !_instance.GetDatabase().IsConnected("testKey"))
                        {

                            var configurationOptions = new ConfigurationOptions
                            {
                                //AbortOnConnectFail = false,
                                Password = ServerPassword,
                            };
                            configurationOptions.EndPoints.Add(new DnsEndPoint(ServerHost, ServerPort));
                            _instance = ConnectionMultiplexer.Connect(configurationOptions);
                        }
                    }

                    //if (_instance == null || !_instance.IsConnected)
                    //{
                    //    _instance = ConnectionMultiplexer.Connect(_redisConectStr);
                    //}
                    return _instance;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static Dictionary<string, ConnectionMultiplexer> _instanceList = new Dictionary<string, ConnectionMultiplexer>();
        public static ConnectionMultiplexer GetInstance(string serverHost, int serverPort = 6379, string serverPassword = "123456")
        {
            try
            {
                lock (redisLock)
                {
                    ConnectionMultiplexer instance = null;
                    var key = string.Format("{0}_{1}_{2}", serverHost, serverPort, serverPassword);
                    if (_instanceList.Keys.Contains(key))
                        instance = _instanceList[key];
                    if (instance != null && instance.GetDatabase().IsConnected("testKey"))
                        return instance;

                    var configurationOptions = new ConfigurationOptions
                    {
                        //AbortOnConnectFail = false,
                        Password = serverPassword,
                    };
                    configurationOptions.EndPoints.Add(new DnsEndPoint(serverHost, serverPort));
                    instance = ConnectionMultiplexer.Connect(configurationOptions);
                    _instanceList.Add(key, instance);
                    return instance;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public static IServer CurrentServer
        //{
        //    get
        //    {
        //        return RedisHelper.Instance.GetServer(_redisConectStr);
        //    }
        //}

        /// <summary>
        /// 未出票的订单库
        /// </summary>
        public static IDatabase DB_NoTicket_Order
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(0);
            }
        }

        /// <summary>
        /// 竞彩、传统、北单的比赛数据和比赛结果库
        /// </summary>
        public static IDatabase DB_Match
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(1);
            }
        }

        /// <summary>
        /// 追号订单库
        /// </summary>
        public static IDatabase DB_Chase_Order
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(2);
            }
        }

        /// <summary>
        /// 未结算订单的库(竞彩足球、竞彩篮球)
        /// </summary>
        public static IDatabase DB_Running_Order_JC
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(3);
            }
        }

        /// <summary>
        /// 未结算订单的库(北京单场)
        /// </summary>
        public static IDatabase DB_Running_Order_BJDC
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(4);
            }
        }

        /// <summary>
        /// 未结算订单的库(传统足球)
        /// </summary>
        public static IDatabase DB_Running_Order_CTZQ
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(5);
            }
        }

        /// <summary>
        /// 未结算订单的库(低频数字彩)
        /// </summary>
        public static IDatabase DB_Running_Order_SCZ_DP
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(6);
            }
        }

        /// <summary>
        /// 未结算订单的库(高频数字彩)
        /// </summary>
        public static IDatabase DB_Running_Order_SCZ_GP
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(7);
            }
        }

        /// <summary>
        /// 配置、奖期、比赛、合买大厅、过关统计等
        /// </summary>
        public static IDatabase DB_CoreCacheData
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(8);
            }
        }

        /// <summary>
        /// 用户绑定信息
        /// </summary>
        public static IDatabase DB_UserBindData
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(9);
            }
        }

        /// <summary>
        /// 用户博客数据
        /// </summary>
        public static IDatabase DB_UserBlogData
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(10);
            }
        }

        /// <summary>
        /// 订单详细数据
        /// </summary>
        public static IDatabase DB_SchemeDetail
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(11);
            }
        }

        /// <summary>
        /// 用户余额
        /// </summary>
        public static IDatabase DB_UserBalance
        {
            get
            {
                return RedisHelper.Instance.GetDatabase(12);
            }
        }
    }

}
