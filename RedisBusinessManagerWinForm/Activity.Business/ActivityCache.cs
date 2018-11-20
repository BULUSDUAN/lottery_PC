using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activity.Domain.Entities;

namespace Activity.Business
{
    /// <summary>
    /// 活动数据缓存
    /// </summary>
    public class ActivityCache
    {
        public static List<ActivityConfig> _activityConfigCache = new List<ActivityConfig>();

        /// <summary>
        /// 查询活动配置
        /// </summary>
        public static ActivityConfig QueryActivityConfig(string key)
        {
            if (_activityConfigCache == null || _activityConfigCache.Count == 0)
            {
                _activityConfigCache = new A20150919Manager().QueryActivityConfig();
            }
            return _activityConfigCache.FirstOrDefault(p => p.ConfigKey == key);
        }

        /// <summary>
        /// 查询全部活动配置
        /// </summary>
        public static List<ActivityConfig> QueryActivityConfig()
        {
            if (_activityConfigCache == null || _activityConfigCache.Count == 0)
            {
                _activityConfigCache = new A20150919Manager().QueryActivityConfig();
            }
            return _activityConfigCache;
        }

        /// <summary>
        /// 活空活动配置
        /// </summary>
        public static void ClearActivityConfig()
        {
            if (_activityConfigCache != null)
                _activityConfigCache.Clear();
            _activityConfigCache = new A20150919Manager().QueryActivityConfig();
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
