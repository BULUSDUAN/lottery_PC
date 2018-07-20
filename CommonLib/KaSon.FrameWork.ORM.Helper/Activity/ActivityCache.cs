using EntityModel;
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
        public static List<E_Activity_Config> _activityConfigCache = new List<E_Activity_Config>();

        /// <summary>
        /// 查询活动配置
        /// </summary>
        public static E_Activity_Config QueryActivityConfig(string key)
        {
            if (_activityConfigCache == null || _activityConfigCache.Count == 0)
            {
                _activityConfigCache = new A20150919Manager().QueryActivityConfig();
            }
            return _activityConfigCache.Where(p => p.ConfigKey == key).FirstOrDefault();
        }

        /// <summary>
        /// 查询全部活动配置
        /// </summary>
        public static List<E_Activity_Config> QueryActivityConfig()
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
