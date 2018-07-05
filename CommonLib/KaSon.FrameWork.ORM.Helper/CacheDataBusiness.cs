using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Redis;
using KaSon.FrameWork.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class CacheDataBusiness:DBbase
    {

        private static List<C_Core_Config> _coreConfigList = new List<C_Core_Config>();

        public C_Core_Config QueryCoreConfigByKey(string key)
        {
            if (_coreConfigList.Count == 0)
                _coreConfigList = QueryAllCoreConfig();
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

        private List<C_Core_Config> QueryAllCoreConfig()
        {
            return (from c in DB.CreateQuery<C_Core_Config>()
                    select c).ToList();
        }

        //public APPConfigInfo QueryAppConfigByAgentId(string appAgentId)
        //{
        //    if (string.IsNullOrEmpty(appAgentId))
        //        appAgentId = "100000";
        //    if (_AppConfigList.Count == 0)
        //        _AppConfigList = new UserIntegralManager().QueryAppConfigList();
        //    var config = _AppConfigList.FirstOrDefault(p => p.AppAgentId == appAgentId);
        //    if (config == null)
        //    {
        //        var entity = new UserIntegralManager().QueryAppConfigByAgentId(appAgentId);
        //        if (entity == null)
        //            throw new Exception("未查询到下载地址");
        //        config = new APPConfigInfo();
        //        ObjectConvert.ConverEntityToInfo(entity, ref config);
        //        _AppConfigList.Add(config);
        //    }
        //    return config;
        //}
    }
}
