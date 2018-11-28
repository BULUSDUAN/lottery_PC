using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using System.Configuration;

namespace RedisBusinessManager
{
    /// <summary>
    /// Redis数据库
    /// </summary>
    //public static class RedisHelper
    //{
    //    private static ConnectionMultiplexer _instance;
    //    private static string _redisConectStr = ConfigurationManager.AppSettings["RedisConnect"];
    //    /// <summary>
    //    /// Redis实例
    //    /// </summary>
    //    public static ConnectionMultiplexer Instance
    //    {
    //        get
    //        {
    //            if (_instance == null || !_instance.IsConnected)
    //            {
    //                _instance = ConnectionMultiplexer.Connect(_redisConectStr);
    //            }
    //            return _instance;
    //        }
    //    }


    //}
}
