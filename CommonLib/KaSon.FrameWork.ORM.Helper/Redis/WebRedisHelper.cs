using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using EntityModel.CoreModel;
using EntityModel.Redis;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common;
using static EntityModel.CoreModel.ReportInfo;
using EntityModel.Enum;
using EntityModel;
using KaSon.FrameWork.Common.JSON;

namespace KaSon.FrameWork.ORM.Helper
{
    public class WebRedisHelper
    {
        /// <summary>
        /// 按官方结束时间查询未来期的奖期
        /// </summary>
        public static List<LotteryIssuse_QueryInfo> QueryNextIssuseListByOfficialStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            return QueryNextIssuseListByKey(key);
        }

        /// <summary>
        /// 按本地结束时间查询未来期的奖期
        /// </summary>
        public static List<LotteryIssuse_QueryInfo> QueryNextIssuseListByLocalStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            return QueryNextIssuseListByKey(key);
        }

        private static List<LotteryIssuse_QueryInfo> QueryNextIssuseListByKey(string key)
        {
            var db = RedisHelperEx.DB_CoreCacheData;
            var list = db.GetRange<LotteryIssuse_QueryInfo>(key);
            return list;
        }

        //        /// <summary>
        //        /// 从Redis中查询系统配置
        //        /// </summary>
        //        //public static string QueryCoreConfigFromRedis(string key)
        //        //{
        //        //    var db = RedisHelperEx.DB_CoreCacheData;
        //        //    var redisKey = RedisKeys.Key_CoreConfig;
        //        //    foreach (var item in db.ListRangeAsync(redisKey).Result)
        //        //    {
        //        //        if (!item.HasValue) continue;
        //        //        var array = item.ToString().Split('^');
        //        //        if (array.Length != 3)
        //        //            continue;
        //        //        if (array[0] == key)
        //        //            return array[2];
        //        //    }
        //        //    return string.Empty;
        //        //}


        /// <summary>
        /// 从Redis查询出合买订单数据
        /// </summary>
        //     public static Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherListFromRedis(string key, string issuseNumber, string gameCode, string gameType,
        //TogetherSchemeSecurity? security, SchemeBettingCategory? betCategory, TogetherSchemeProgress? progressState,
        //decimal minMoney, decimal maxMoney, decimal minProgress, decimal maxProgress, string orderBy, int pageIndex, int pageSize)
        //     {
        //         //var db = RedisHelperEx.DB_CoreCacheData;
        //var redisKey_TogetherList = RedisKeys.Key_Core_Togegher_OrderList;
        ////生成列表
        //var list = new List<Sports_TogetherSchemeQueryInfo>();
        //var redisList = db.ListRangeAsync(redisKey_TogetherList).Result;
        //foreach (var item in redisList)
        //{
        //    try
        //    {
        //        if (!item.HasValue) continue;
        //        var t = JsonSerializer.Deserialize<Sports_TogetherSchemeQueryInfo>(item.ToString());
        //        list.Add(t);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        ////查询列表
        //var seC = !security.HasValue ? -1 : (int)security.Value;
        //var betC = !betCategory.HasValue ? -1 : (int)betCategory.Value;
        //var strPro = !progressState.HasValue ? "10|20|30" : ((int)progressState.Value).ToString();
        //var arrProg = strPro.Split('|');
        //if (!string.IsNullOrEmpty(gameCode))
        //    gameCode = gameCode.ToUpper();
        //if (!string.IsNullOrEmpty(gameType))
        //    gameType = gameType.ToUpper();
        // var cache = new Sports_TogetherSchemeQueryInfoCollection();
        //var query = from s in list
        //            where arrProg.Contains(Convert.ToInt32(s.ProgressStatus).ToString())
        //              && (betC == -1 || Convert.ToInt32(s.SchemeBettingCategory) == betC)
        //              && (issuseNumber == "" || s.IssuseNumber == issuseNumber)
        //              && (s.StopTime >= DateTime.Now)
        //              && (gameCode == "" || s.GameCode == gameCode)
        //              && (gameType == "" || s.GameType == gameType)
        //              && (minMoney == -1 || s.TotalMoney >= minMoney)
        //              && (maxMoney == -1 || s.TotalMoney <= maxMoney)
        //              && (minProgress == -1 || s.Progress >= minProgress)
        //              && (maxProgress == -1 || s.Progress <= maxProgress)
        //              && (seC == -1 || Convert.ToInt32(s.Security) == seC)
        //              && (key == "" || s.CreateUserId == key || s.SchemeId == key || s.CreaterDisplayName == key)
        //            select s;
        //cache.TotalCount = query.Count();
        //cache.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        // return cache;
        // }



        //        /// <summary>
        //        /// 按钮上的广告
        //        /// </summary>
        //        public static string APP_Advertising
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("APP_Advertising").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }


        //        /// <summary>
        //        /// 网站客服电话
        //        /// </summary>
        //        public static string tel
        //        {
        //            get
        //            {
        //                try
        //                {
        //                    return WCFClients.GameClient.QueryCoreConfigByKey("Site.Service.Phone").ConfigValue;
        //                }
        //                catch
        //                {
        //                    return "400-009-6569";
        //                }
        //            }
        //        }


        //        /// <summary>
        //        /// IOS版本
        //        /// </summary>
        //        public static string IOS_PhoneCheckversionNew
        //        {
        //            get
        //            {
        //                try
        //                {
        //                    return WCFClients.GameClient.QueryCoreConfigByKey("IOS_PhoneCheckversionNew").ConfigValue;
        //                }
        //                catch
        //                {
        //                    return "[]";
        //                }
        //            }
        //        }


        //        /// <summary>
        //        /// ANDRIOD版本
        //        /// </summary>
        //        public static string ANDRIOD_PhoneCheckversionNew
        //        {
        //            get
        //            {
        //                try
        //                {
        //                    return WCFClients.GameClient.QueryCoreConfigByKey("ANDRIOD_PhoneCheckversionNew").ConfigValue;
        //                }
        //                catch
        //                {
        //                    return "[]";
        //                }
        //            }
        //        }


        //        #region "20180425新增配置接口"
        //        /// <summary>
        //        /// App首页会员中心公共信息
        //        /// </summary>
        //        public static string APP_Common
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("APP_Common").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }
        //        /// <summary>
        //        /// App会员中心logo丰富词
        //        /// </summary>
        //        public static string APP_UserCenter
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("APP_UserCenter").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }
        //        /// <summary>
        //        /// App首页
        //        /// </summary>
        //        public static string APP_Index
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("APP_Index").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }
        //        /// <summary>
        //        /// App会员中心推荐有礼
        //        /// </summary>
        //        public static string APP_tuijianyouli
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("APP_tuijianyouli").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }

        //        //--20180502新增用户协议配置接口
        //        /// <summary>
        //        /// 用户协议配置
        //        /// </summary>
        //        public static string Agreement_Config
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("Agreement_Config").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// 协议_首页配置
        //        /// </summary>
        //        public static string Index_Config
        //        {
        //            get
        //            {
        //                string defalutValue = "";
        //                try
        //                {
        //                    var v = WCFClients.GameClient.QueryConfigByKey("Index_Config").ConfigValue;
        //                    if (string.IsNullOrEmpty(v))
        //                    {
        //                        return defalutValue;
        //                    }
        //                    else
        //                    {
        //                        return v;
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    return defalutValue;
        //                }
        //            }
        //        }
        //        #endregion


        /// <summary>
        /// 从Redis中查询出合买红人数据
        /// </summary>
        public static List<TogetherHotUserInfo> QueryHotTogetherUserListFromRedis()
        {
            var db = RedisHelperEx.DB_CoreCacheData;//.DB_CoreCacheData;
            var redisKey = RedisKeys.Key_Core_Togegher_SupperUser;
            //var result = new List<TogetherHotUserInfo>();
            var list = db.GetRange<TogetherHotUserInfo>(redisKey);
            //foreach (var item in list)
            //{
            //    try
            //    {
            //        if (!item.HasValue)
            //            continue;
            //        var t = JsonSerializer.Deserialize<TogetherHotUserInfo>(item.ToString());
            //        result.Add(t);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
            return list.OrderByDescending(p => p.WeeksWinMoney).ToList();
        }

        /// <summary>
        /// 从Redis中查询过关统计数据
        /// </summary>
        public static SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool isVirtualOrder, SchemeBettingCategory? category, string key
            , string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var db = RedisHelperEx.DB_CoreCacheData;//.DB_CoreCacheData;
            var redisKey = RedisKeys.Key_Core_GGTJ_List;
            //var list = new List<SportsOrder_GuoGuanInfo>();
            //生成列表
            var list = db.GetRange<SportsOrder_GuoGuanInfo>(redisKey);
            //foreach (var item in redisList)
            //{
            //    try
            //    {
            //        if (!item.HasValue)
            //            continue;
            //        var t = JsonSerializer.Deserialize<SportsOrder_GuoGuanInfo>(item.ToString());
            //        list.Add(t);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
            //查询列表
            var cache = new SportsOrder_GuoGuanInfoCollection();
            var query = from s in list
                        where s.IsVirtualOrder == isVirtualOrder
                        && (!category.HasValue || s.SchemeBettingCategory == category.Value)
                        && (key == "" || s.UserDisplayName == key)
                        && s.GameCode == gameCode.ToUpper()
                        && (gameType == "" || s.GameType == gameType)
                        && (issuseNumber == "" || s.IssuseNumber == issuseNumber)
                        && (s.BetTime >= startTime && s.BetTime < endTime)
                        select s;
            cache.TotalCount = query.Count();
            cache.ReportItemList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return cache;
        }
        #region 查询用户个人信息
        public static ProfileUserInfo QueryProfileUserInfo(string userId)
        {
            var info = LoadProfileUserInfoFromRedis(userId);
            if (info == null)
            {
                var biz = new CacheDataBusiness();
                info = biz.QueryProfileUserInfo(userId);
                SaveProfileUserInfoToRedis(userId, info);
            }
            return info;
        }

        private static ProfileUserInfo LoadProfileUserInfoFromRedis(string userId)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;

            var key = string.Format("ProfileUserInfo_{0}", userId);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<ProfileUserInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileUserInfoToRedis(string userId, ProfileUserInfo info)
        {
            try
            {
                var json = JsonSerializer.Serialize(info);
                var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
                var key = string.Format("ProfileUserInfo_{0}", userId);
                int validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }
        #endregion
        #region 查询中奖等级

        public static ProfileBonusLevelInfo QueryProfileBonusLevelInfo(string userId)
        {
            var info = LoadProfileBonusLevelInfoFromRedis(userId);
            if (info == null)
            {
                var biz = new CacheDataBusiness();
                info = biz.QueryProfileBonusLevelInfo(userId);
                SaveProfileBonusLevelInfoToRedis(userId, info);
            }
            return info;
        }

        private static ProfileBonusLevelInfo LoadProfileBonusLevelInfoFromRedis(string userId)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("ProfileBonusLevelInfo_{0}", userId);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<ProfileBonusLevelInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileBonusLevelInfoToRedis(string userId, ProfileBonusLevelInfo info)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("ProfileBonusLevelInfo_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询最新中奖

        public static ProfileLastBonusCollection QueryProfileLastBonusCollection(string userId)
        {
            var info = LoadProfileLastBonusCollectionFromRedis(userId);
            if (info == null)
            {
                var biz = new CacheDataBusiness();
                info = biz.QueryProfileLastBonusCollection(userId);
                SaveProfileLastBonusCollectionToRedis(userId, info);
            }
            return info;
        }

        private static ProfileLastBonusCollection LoadProfileLastBonusCollectionFromRedis(string userId)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("ProfileLastBonusCollection_{0}", userId);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<ProfileLastBonusCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileLastBonusCollectionToRedis(string userId, ProfileLastBonusCollection info)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("ProfileLastBonusCollection_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询个人数据

        public static ProfileDataReport QueryProfileDataReport(string userId)
        {
            var info = LoadProfileDataReportFromRedis(userId);
            if (info == null)
            {
                var biz = new CacheDataBusiness();
                info = biz.QueryProfileDataReport(userId);
                SaveProfileDataReportToRedis(userId, info);
            }
            return info;
        }

        private static ProfileDataReport LoadProfileDataReportFromRedis(string userId)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("ProfileDataReport_{0}", userId);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<ProfileDataReport>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileDataReportToRedis(string userId, ProfileDataReport info)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("ProfileDataReport_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion

            #region 查询用户跟单人数

        public static int QueryTogetherFollowerCount(string userId)
        {
            var info = LoadTogetherFollowerCountFromRedis(userId);
            if (info == -1)
            {

                info = new SqlQueryBusiness().QueryTogetherFollowerCount(userId);
                SaveTogetherFollowerCountToRedis(userId, info);
            }
            return info;
        }

        private static int LoadTogetherFollowerCountFromRedis(string userId)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("FollowerCount_{0}", userId);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return -1;
            try
            {
                return int.Parse(v.ToString());
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static void SaveTogetherFollowerCountToRedis(string userId, int count)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("FollowerCount_{0}", userId);
            try
            {
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, count.ToString(), Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户战绩

        public static UserBeedingListInfoCollection QueryUserBeedingListInfoCollection(string userId)
        {
            var info = LoadUserBeedingListInfoCollectionFromRedis(userId);
            if (info == null)
            {
              
                info = new Sports_Business().QueryUserBeedingList("", "", userId, string.Empty, 0, 100, QueryUserBeedingListOrderByProperty.TotalBonusMoney, OrderByCategory.DESC);
                SaveUserBeedingListInfoCollectionToRedis(userId, info);
            }
            return info;
        }

        private static UserBeedingListInfoCollection LoadUserBeedingListInfoCollectionFromRedis(string userId)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("UserBeedingList_{0}", userId);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<UserBeedingListInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveUserBeedingListInfoCollectionToRedis(string userId, UserBeedingListInfoCollection info)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("UserBeedingList_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户当前的订单

        public static UserCurrentOrderInfoCollection QueryUserCurrentOrderInfoCollection(string userId, string gameCode)
        {
            var info = LoadUserCurrentOrderInfoCollectionFromRedis(userId, gameCode);
            if (info == null)
            {
               
                info = new Sports_Business().QueryUserCurrentOrderList(userId, gameCode, 0, 30);
                SaveUserCurrentOrderInfoCollectionToRedis(userId, gameCode, info);
            }
            return info;
        }

        private static UserCurrentOrderInfoCollection LoadUserCurrentOrderInfoCollectionFromRedis(string userId, string gameCode)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("UserCurrentOrderInfoCollection_{0}_{1}", userId, gameCode);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<UserCurrentOrderInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveUserCurrentOrderInfoCollectionToRedis(string userId, string gameCode, UserCurrentOrderInfoCollection info)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("UserCurrentOrderInfoCollection_{0}_{1}", userId, gameCode);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户中奖订单

        public static BonusOrderInfoCollection QueryBonusOrderInfoCollection(string userId, string gameCode, string gameType)
        {
            gameCode = (gameCode == "SZC" ? gameType : gameCode);
            gameType = (gameCode == "SZC" ? "" : gameType);
            var info = LoadBonusOrderInfoCollectionFromRedis(userId, gameCode, gameType);
            if (info == null)
            {
                info =new DataQuery().QueryBonusInfoList(userId, gameCode, gameType, string.Empty, string.Empty, string.Empty, 0, 30);
                SaveBonusOrderInfoCollectionToRedis(userId, gameCode, gameType, info);
            }
            return info;
        }

        private static BonusOrderInfoCollection LoadBonusOrderInfoCollectionFromRedis(string userId, string gameCode, string gameType)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("BonusOrderInfoCollection_{0}_{1}_{2}", userId, gameCode, gameType);
            var v = db.ExistsAsync(key).Result;
            if (!v)
                return null;
            try
            {
                return JsonSerializer.Deserialize<BonusOrderInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveBonusOrderInfoCollectionToRedis(string userId, string gameCode, string gameType, BonusOrderInfoCollection info)
        {
            var db = RedisHelperEx.DB_UserBlogData;//.DB_UserBlogData;
            var key = string.Format("BonusOrderInfoCollection_{0}_{1}_{2}", userId, gameCode, gameType);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigHelper.AllConfigInfo["BlogValidMinute"].ToString());
                db.Set(key, json, Convert.ToInt32(TimeSpan.FromMinutes(validMinute)));
            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}