using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
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
        public C_Core_Config QueryCoreConfigByKey(string key)
        {
            var RedisKeyH = "CoreConfig_";
            var RedisKey = RedisKeyH + key;
            var v = RedisHelperEx.DB_Other.Get(RedisKey);
            var config = new C_Core_Config();
            if (string.IsNullOrEmpty(v))
            {
                config = DB.CreateQuery<C_Core_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
                v = config.ConfigValue;
                if (config != null)
                {
                    RedisHelperEx.DB_Other.Set(RedisKey, v, 3 * 60);
                }
                else {
                    throw new Exception(string.Format("找不到配置项：{0}", key));
                }
            }
            config.ConfigValue = v;
            config.ConfigKey = key;
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
                var db = RedisHelperEx.DB_UserBindData;
                db.Del(fullKey);
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
           
           _AppConfigList = new DataQuery().QueryAppConfigList();
            var config = _AppConfigList.FirstOrDefault(p => p.AppAgentId == appAgentId);
            if (config == null)
            {
                var entity = new DataQuery().QueryAppConfigByAgentId(appAgentId);
                if (entity == null)
                    throw new LogicException("未查询到下载地址");
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

                var satisfyFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.SatisfyLotteryGiveRedBagTofxid"));
                if (entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag && totalMoney >= satisfyFillMoney)
                {
                    //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
                    var giveFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.FirstLotteryGiveRedBagTofxid"));
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

        #region 分享中奖订单推广注册
        public void FirstOrderShareRegisterRedBag(string schemeId)
        {
            try
            {
                DB.Begin();
                var schemeInfo = new OrderQuery().QuerySportsSchemeInfo(schemeId);
                if (schemeInfo != null && schemeInfo.BonusStatus != BonusStatus.Win || schemeInfo.PreTaxBonusMoney == 0) return;
                //分享推广 购彩 送红包
                //购彩了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
                //var entityBankCard = new BankCardManager().BankCardById(userId);
                //var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
                //if (entityBankCard != null && entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag)
                //{
                //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
                var shareGiveRedBagPre = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.WinningShareGiveRedBag"));
                var business = new BlogManager();
                var oldmodel = business.QueryBlog_OrderShareRegisterRedBag(schemeId, schemeInfo.UserId);
                if (oldmodel != null)
                {
                    if (!oldmodel.IsGiveRegisterRedBag && shareGiveRedBagPre > 0)
                    {
                        var giveFillMoney = schemeInfo.PreTaxBonusMoney * shareGiveRedBagPre / 100;
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, schemeInfo.UserId, Guid.NewGuid().ToString("N"), giveFillMoney
                                              , string.Format("分享中奖订单{0}，加奖红包：{1}元", schemeId, giveFillMoney.ToString("f2")), RedBagCategory.OrderRegister);
                        oldmodel.IsGiveRegisterRedBag = true;
                    }
                    //更新条数
                    oldmodel.RegisterCount += 1;
                    oldmodel.UpdateTime = DateTime.Now;
                    business.UpdateBlog_OrderShareRegisterRedBag(oldmodel);
                }
                else
                {
                    var flag = false;
                    var giveFillMoney = 0m;
                    if (shareGiveRedBagPre > 0)
                    {
                        //发奖
                        giveFillMoney = schemeInfo.PreTaxBonusMoney * shareGiveRedBagPre / 100;
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, schemeInfo.UserId, Guid.NewGuid().ToString("N"), giveFillMoney
                                              , string.Format("分享中奖订单{0}，加奖红包：{1}元", schemeId, giveFillMoney), RedBagCategory.OrderRegister);
                        flag = true;
                    }
                    //插入数据库
                    var newmodel = new E_Blog_OrderShareRegisterRedBag()
                    {
                        CreateTime = DateTime.Now,
                        IsGiveRegisterRedBag = flag,
                        RedBagMoney = giveFillMoney,
                        RedBagPre = shareGiveRedBagPre,
                        UpdateTime = DateTime.Now,
                        RegisterCount = 1,
                        SchemeId = schemeId,
                        UserId = schemeInfo.UserId
                    };
                    business.Add_OrderShareRegisterRedBag(newmodel);
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

        public List<C_Lottery_Game> QueryLotteryAllGame()
        {
            return DB.CreateQuery<C_Lottery_Game>().ToList();
        }

        public UserLoginHistoryCollection QueryUserLoginHistoryCollection(string userId)
        {
            var result = new UserLoginHistoryCollection();
            result.AddRange(new BlogManager().QueryBlog_UserLoginHistory(userId));
            return result;
        }

        public ProfileUserInfo QueryProfileUserInfo(string userId)
        {
            var manager = new Sports_Manager();
            var info = manager.QueryProfileUserInfo(userId);
         
            return info;
        }

        public ProfileBonusLevelInfo QueryProfileBonusLevelInfo(string userId)
        {
            var result = new ProfileBonusLevelInfo();
            var pb = new BlogManager().QueryBlog_ProfileBonusLevel(userId);
            if (pb == null)
                return new ProfileBonusLevelInfo();
            return new ProfileBonusLevelInfo
            {
                UserId = pb.UserId,
                MaxLevelName = pb.MaxLevelName,
                MaxLevelValue = pb.MaxLevelValue,
                WinHundredMillionCount = pb.WinHundredMillionCount,
                WinOneHundredCount = pb.WinOneHundredCount,
                WinOneHundredThousandCount = pb.WinOneHundredThousandCount,
                WinOneMillionCount = pb.WinOneMillionCount,
                WinOneThousandCount = pb.WinOneThousandCount,
                WinTenMillionCount = pb.WinTenMillionCount,
                WinTenThousandCount = pb.WinTenThousandCount,
            };
        }

        public ProfileLastBonusCollection QueryProfileLastBonusCollection(string userId)
        {
            var result = new ProfileLastBonusCollection();
            var totalCount = 0;
            result.List.AddRange(new BlogManager().QueryProfileLastBonusList(userId, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        public ProfileDataReport QueryProfileDataReport(string userId)
        {
            var manager = new BlogManager();
            var dataReport = manager.QueryBlog_DataReport(userId);
            if (dataReport == null)
                return new ProfileDataReport();
            return new ProfileDataReport
            {
                UserId = dataReport.UserId,
                CreateSchemeCount = dataReport.CreateSchemeCount,
                JoinSchemeCount = dataReport.JoinSchemeCount,
                TotalBonusCount = dataReport.TotalBonusCount,
                TotalBonusMoney = dataReport.TotalBonusMoney,
                UpdateTime = dataReport.UpdateTime,
            };

        }
    }
}
