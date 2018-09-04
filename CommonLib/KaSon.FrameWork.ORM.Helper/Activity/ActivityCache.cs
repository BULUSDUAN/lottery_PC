using EntityModel;
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
        public static E_Activity_Config _activityConfigCache = new E_Activity_Config();

        /// <summary>
        /// 查询活动配置
        /// </summary>
        public static string QueryActivityConfig(string key)
        {
            var v = RedisHelper.DB_Other.Get(key);
            if (string.IsNullOrEmpty(v))
            {
                var config = new A20150919Manager().QueryActivityConfig(key);
                v = config.ConfigValue;
                if (config != null)
                {
                    RedisHelper.DB_Other.Set(key, v, 3 * 60);
                }
            }
            return v;
        }

        /// <summary>
        /// 查询全部活动配置
        /// </summary>
        //public static List<E_Activity_Config> QueryActivityConfig()
        //{
        //    if (_activityConfigCache == null || _activityConfigCache.Count == 0)
        //    {
        //        _activityConfigCache = new A20150919Manager().QueryActivityConfig();
        //    }
        //    return _activityConfigCache;
        //}

        /// <summary>
        /// 清空活动配置
        /// </summary>
        public static void ClearActivityConfig()
        {
            var _activityConfigCache = new A20150919Manager().QueryActivityConfig();
            RedisHelper.DB_Other.Del(_activityConfigCache.Select(b => b.ConfigValue).ToArray());
            //foreach (var item in _activityConfigCache)
            //{
            //    var flag = RedisHelper.KeyExists(item.ConfigValue);
            //    if (flag)
            //    {
            //        RedisHelper.KeyDelete(item.ConfigValue);
            //    }
            //}


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
