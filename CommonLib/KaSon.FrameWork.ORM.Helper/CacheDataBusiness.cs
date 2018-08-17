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

        //private static List<C_Core_Config> _coreConfigList = new List<C_Core_Config>();

        public C_Core_Config QueryCoreConfigByKey(string key)
        {
            //if (_coreConfigList.Count == 0)
            //    _coreConfigList = QueryAllCoreConfig();
            var config = DB.CreateQuery<C_Core_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config;
        }

        /// <summary>
        /// 从Redis中查询系统配置
        /// </summary>
        public string QueryCoreConfigFromRedis(string key)
        {
            var config = DB.CreateQuery<C_Core_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
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
            return DB.CreateQuery<C_Core_Config>().ToList();
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


        #region 分享推广
        ///// <summary>
        ///// 分享推广 购彩 送红包
        ///// </summary>
        ///// <param name="userId"></param>
        //public void FirstLotteryGiveRedBag(string userId)
        //{
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        //分享推广 购彩 送红包
        //        //购彩了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
        //        var entityBankCard = new BankCardManager().BankCardById(userId);
        //        var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
        //        if (entityBankCard != null && entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag)
        //        {
        //            //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
        //            var giveFillMoney = decimal.Parse(Activity.Business.ActivityCache.QueryActivityConfig("ActivityConfig.FirstLotteryGiveRedBagTofxid").ConfigValue);
        //            if (giveFillMoney > 0)
        //            {
        //                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, entityShareSpread.AgentId, Guid.NewGuid().ToString("N"), giveFillMoney
        //                                  , string.Format("{1}用户购彩了赠送红包给分享推广用户{0}元", giveFillMoney, userId), RedBagCategory.FxidRegister);

        //                entityShareSpread.isGiveLotteryRedBag = true;
        //                entityShareSpread.UpdateTime = DateTime.Now;
        //                entityShareSpread.giveRedBagMoney = entityShareSpread.giveRedBagMoney + giveFillMoney;
        //                new BlogManager().UpdateBlog_UserShareSpread(entityShareSpread);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //}

        /// <summary>
        /// 分享推广 购彩 送红包(满x元送z元红包)
        /// </summary>
        /// <param name="userId"></param>
        public void FirstLotteryGiveRedBag(string userId, decimal totalMoney)
        {

            try
            {
                DB.Begin();
                //分享推广 购彩 送红包
                //购彩了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
                //var entityBankCard = new BankCardManager().BankCardById(userId);
                /*entityBankCard != null */
                var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);

                var satisfyFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.SatisfyLotteryGiveRedBagTofxid").ConfigValue);
                if (entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag && totalMoney >= satisfyFillMoney)
                {
                    //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
                    var giveFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.FirstLotteryGiveRedBagTofxid").ConfigValue);
                    if (giveFillMoney > 0)
                    {

                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, entityShareSpread.AgentId, Guid.NewGuid().ToString("N"), giveFillMoney
                                              , string.Format("{1}用户购彩超过{2}元，赠送红包给分享推广用户{0}元", giveFillMoney, userId, satisfyFillMoney), RedBagCategory.FxidRegister);

                        entityShareSpread.isGiveLotteryRedBag = true;
                        entityShareSpread.UpdateTime = DateTime.Now;
                        entityShareSpread.giveRedBagMoney = entityShareSpread.giveRedBagMoney + giveFillMoney;
                        new BlogManager().UpdateBlog_UserShareSpread(entityShareSpread);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            
        }
        #endregion
    }
}
