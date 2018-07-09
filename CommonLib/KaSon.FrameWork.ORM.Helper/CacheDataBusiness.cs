using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.Redis;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class CacheDataBusiness : DBbase
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

        private static List<APPConfigInfo> _AppConfigList = new List<APPConfigInfo>();

        public APPConfigInfo QueryAppConfigByAgentId(string appAgentId)
        {
            if (string.IsNullOrEmpty(appAgentId))
                appAgentId = "100000";
            if (_AppConfigList.Count == 0)
                _AppConfigList = new DataQuery().QueryAppConfigList();
            var config = _AppConfigList.FirstOrDefault(p => p.AppAgentId == appAgentId);
            if (config == null)
            {
                var entity = new DataQuery().QueryAppConfigByAgentId(appAgentId);
                if (entity == null)
                    throw new Exception("未查询到下载地址");
                config = new APPConfigInfo();
                ObjectConvert.ConverEntityToInfo(entity, ref config);
                _AppConfigList.Add(config);
            }
            return config;
        }

        private static List<C_APP_NestedUrlConfig> _NestedUrlConfigList = new List<C_APP_NestedUrlConfig>();
        private static List<C_APP_NestedUrlConfig> _AllNestedUrlConfigList = new List<C_APP_NestedUrlConfig>();
        /// <summary>
        /// 根据UrlType查询所有APP嵌套配置
        /// </summary>
        /// <returns></returns>
        public NestedUrlConfig_Collection QueryNestedUrlConfigListByUrlType(int urlType)
        {
            try
            {
                NestedUrlConfig_Collection collection = new NestedUrlConfig_Collection();
                if (_AllNestedUrlConfigList == null || _AllNestedUrlConfigList.Count <= 0)
                {
                    var nestedConfigList = new DataQuery().QueryNestedUrlList();
                    _AllNestedUrlConfigList.AddRange(nestedConfigList);
                }
                var list = _AllNestedUrlConfigList.Where(s => s.UrlType == urlType || s.UrlType == (int)UrlType.All).ToList();
                if (list == null || list.Count <= 0)
                    _AllNestedUrlConfigList.AddRange(list);
                foreach (var item in list)
                {
                    NestedUrlConfigInfo info = new NestedUrlConfigInfo();
                    info.ConfigKey = item.ConfigKey;
                    info.CreateTime = item.CreateTime;
                    info.Id = item.Id;
                    info.IsEnable = item.IsEnable;
                    info.Remarks = item.Remarks;
                    info.Url = item.Url;
                    info.UrlType = (UrlType)item.UrlType;
                    collection.NestedUrlList.Add(info);
                }
                return collection;
            }
            catch
            {
                ClearNestedUrlConfig();
                return new NestedUrlConfig_Collection();
            }
        }

        /// <summary>
        /// 清空系统配置
        /// </summary>
        public void ClearNestedUrlConfig()
        {
            if (_NestedUrlConfigList != null || _NestedUrlConfigList.Count <= 0)
                _NestedUrlConfigList.Clear();
            if (_AllNestedUrlConfigList != null || _AllNestedUrlConfigList.Count <= 0)
                _AllNestedUrlConfigList.Clear();
        }
    }
}
