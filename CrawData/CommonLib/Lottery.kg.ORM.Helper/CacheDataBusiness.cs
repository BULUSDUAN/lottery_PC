using EntityModel;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class CacheDataBusiness:DBbase
    {
       


        public C_Core_Config QueryCoreConfigByKey(string key)
        {       
            var config = DB.CreateQuery<C_Core_Config>().FirstOrDefault(p => p.ConfigKey == key);
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config;
        }

        /// <summary>
        /// 从Redis中查询系统配置
        /// </summary>
        public string QueryCoreConfigFromRedis(string key)
        {
            var config = DB.CreateQuery<C_Core_Config>().FirstOrDefault(p => p.ConfigKey == key);
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config.ConfigValue;
        }

        /// <summary>
        /// 清理用户绑定数据缓存
        /// </summary>
        public void ClearUserBindInfoCache(string userId)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelper.DB_UserBindData;
                db.KeyDeleteAsync(fullKey);
            }
            catch (Exception)
            {
            }
           
        }
    }
}
