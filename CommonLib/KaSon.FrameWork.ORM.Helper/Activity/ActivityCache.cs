﻿using EntityModel;
using KaSon.FrameWork.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 活动数据缓存
    /// </summary>
    public class ActivityCache
    {
        /// <summary>
        /// 查询活动配置
        /// </summary>
        public static string QueryActivityConfig(string key)
        {
            var v = RedisHelperEx.DB_Other.Get(key);
            if (string.IsNullOrEmpty(v))
            {
                var config = new A20150919Manager().QueryActivityConfig(key);
                v = config.ConfigValue;
                if (config != null)
                {
                    RedisHelperEx.DB_Other.Set(key, v, 3 * 60);
                }
            }
            return v;
        }

        /// <summary>
        /// 查询全部活动配置
        /// </summary>
        public static List<E_Activity_Config> QueryActivityConfig()
        {
              return new A20150919Manager().QueryActivityConfig();
        }

        /// <summary>
        /// 清空活动配置
        /// </summary>
        public static void ClearActivityConfig()
        {
            var _activityConfigCache = new A20150919Manager().QueryActivityConfig();

            foreach (var item in _activityConfigCache)
            {
                RedisHelperEx.DB_Other.Del(item.ConfigKey);
            }


        }

        /// <summary>
        /// 更新网站活动配置
        /// </summary>
        public static void UpdateActivityConfig(string key, string value)
        {
            var manager = new A20150919Manager();
            var config = manager.QueryActivityConfig(key);
            if (config == null)
                return;

            config.ConfigValue = value;
            manager.UpdateActivityConfig(config);
            //清空缓存
            ClearActivityConfig();
        }
    }
}
