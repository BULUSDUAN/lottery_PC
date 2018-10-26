using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using StackExchange.Redis;
using GameBiz.Core;
using Common.JSON;
using Common.Lottery.Redis;
using app.lottery.site.Controllers;
using Common.Utilities;
using Common.Cryptography;
using System.Threading.Tasks;
using log4net;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;
using EntityModel.RequestModel;

namespace app.lottery.site.iqucai
{
    /// <summary>
    /// Redis访问类
    /// </summary>
    public class WebRedisHelper
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private static  IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public WebRedisHelper(IServiceProxyProvider _serviceProxyProvider, ILog log, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion

        private static string _cacheRedisHost = string.Empty;
        /// <summary>
        /// 缓存Redis的ip
        /// </summary>
        public static string CacheRedisHost
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_cacheRedisHost))
                        return _cacheRedisHost;
                    _cacheRedisHost = ConfigurationManager.AppSettings["CacheRedisHost"];
                    return _cacheRedisHost;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        private static int _cacheRedisPort = 0;
        /// <summary>
        /// 缓存Redis的端口
        /// </summary>
        public static int CacheRedisPost
        {
            get
            {
                try
                {
                    if (_cacheRedisPort > 0)
                        return _cacheRedisPort;
                    _cacheRedisPort = int.Parse(ConfigurationManager.AppSettings["CacheRedisPost"]);
                    return _cacheRedisPort;
                }
                catch (Exception)
                {
                    return 6379;
                }
            }
        }

        private static string _cacheRedisPassword = string.Empty;
        /// <summary>
        /// 缓存Redis的密码
        /// </summary>
        public static string CacheRedisPassword
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_cacheRedisPassword))
                        return _cacheRedisPassword;
                    _cacheRedisPassword = ConfigurationManager.AppSettings["CacheRedisPassword"];
                    return _cacheRedisPassword;
                }
                catch (Exception)
                {
                    return "123456";
                }
            }
        }

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
            var list = new List<LotteryIssuse_QueryInfo>();
            try
            {
           
                var db = RedisHelper.DB_CoreCacheData;
                var jsonList = db.ListRangeAsync(key).Result;
                foreach (var json in jsonList)
                {
                    if (!json.HasValue) continue;
                    try
                    {
                        var issuse = JsonSerializer.Deserialize<LotteryIssuse_QueryInfo>(json);
                        list.Add(issuse);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch 
            {

              
            }
           
            return list;
        }

        /// <summary>
        /// 查询彩种红包使用配置
        /// </summary>
        public static decimal QueryRedBagUseConfigFromRedis(string gameCode)
        {
            var db = RedisHelper.DB_CoreCacheData;
            var key = RedisKeys.Key_RedBagUseConfig;
            foreach (var item in db.ListRangeAsync(key).Result)
            {
                if (!item.HasValue) continue;
                var array = item.ToString().Split('_');
                if (array.Length != 2)
                    continue;
                if (array[0].ToUpper() == gameCode.ToUpper())
                    return decimal.Parse(array[1]);
            }
            return 0M;
        }

        /// <summary>
        /// 从Redis中查询系统配置
        /// </summary>
        //public static string QueryCoreConfigFromRedis(string key)
        //{
        //    var db = RedisHelper.DB_CoreCacheData;
        //    var redisKey = RedisKeys.Key_CoreConfig;
        //    foreach (var item in db.ListRangeAsync(redisKey).Result)
        //    {
        //        if (!item.HasValue) continue;
        //        var array = item.ToString().Split('^');
        //        if (array.Length != 3)
        //            continue;
        //        if (array[0] == key)
        //            return array[2];
        //    }
        //    return string.Empty;
        //}

        /// <summary>
        /// 加载合买大厅数据到Redis
        /// </summary>
        public static void LoadTogetherDataToRedis()
        {
            var superList = app.lottery.site.Controllers.WCFClients.GameClient.QueryHotUserTogetherOrderList("");
            var db = RedisHelper.GetInstance(CacheRedisHost,CacheRedisPost,CacheRedisPassword).GetDatabase(8);//.DB_CoreCacheData;
            //加载合买红人
            var redisKey_SupperUser = RedisKeys.Key_Core_Togegher_SupperUser;
            db.KeyDeleteAsync(redisKey_SupperUser);
            foreach (var item in superList)
            {
                try
                {
                    var json = JsonSerializer.Serialize(item);
                    db.ListRightPushAsync(redisKey_SupperUser, json);
                }
                catch (Exception ex)
                {
                }
            }

            //加载合买订单
            var orderBy = "ManYuan desc,ISTOP DESC,TotalMoney desc, Progress DESC";
            var orderList = app.lottery.site.Controllers.WCFClients.GameClient.QuerySportsTogetherList("", "", "", "", null, null, null, -1, -1, -1, -1, orderBy, 0, 30000, "");
            var redisKey_TogetherList = RedisKeys.Key_Core_Togegher_OrderList;
            db.KeyDeleteAsync(redisKey_TogetherList);
            foreach (var item in orderList.List)
            {
                try
                {
                    var json = JsonSerializer.Serialize(item);
                    db.ListRightPushAsync(redisKey_TogetherList, json);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// 从Redis中查询出合买红人数据
        /// </summary>
        public static List<TogetherHotUserInfo> QueryHotTogetherUserListFromRedis()
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(8);//.DB_CoreCacheData;
            var redisKey = RedisKeys.Key_Core_Togegher_SupperUser;
            var result = new List<TogetherHotUserInfo>();
            var list = db.ListRangeAsync(redisKey).Result;
            foreach (var item in list)
            {
                try
                {
                    if (!item.HasValue)
                        continue;
                    var t = JsonSerializer.Deserialize<TogetherHotUserInfo>(item.ToString());
                    result.Add(t);
                }
                catch (Exception)
                {
                }
            }
            return result.OrderByDescending(p => p.WeeksWinMoney).ToList();
        }

        /// <summary>
        /// 从Redis查询出合买订单数据
        /// </summary>
        public static Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherListFromRedis(string key, string issuseNumber, string gameCode, string gameType,
           TogetherSchemeSecurity? security, SchemeBettingCategory? betCategory, TogetherSchemeProgress? progressState,
           decimal minMoney, decimal maxMoney, decimal minProgress, decimal maxProgress, string orderBy, int pageIndex, int pageSize)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(8);//.DB_CoreCacheData;
            var redisKey_TogetherList = RedisKeys.Key_Core_Togegher_OrderList;
            //生成列表
            var list = new List<Sports_TogetherSchemeQueryInfo>();
            var redisList = db.ListRangeAsync(redisKey_TogetherList).Result;
            foreach (var item in redisList)
            {
                try
                {
                    if (!item.HasValue) continue;
                    var t = JsonSerializer.Deserialize<Sports_TogetherSchemeQueryInfo>(item.ToString());
                    list.Add(t);
                }
                catch (Exception)
                {
                }
            }

            //查询列表
            var seC = !security.HasValue ? -1 : (int)security.Value;
            var betC = !betCategory.HasValue ? -1 : (int)betCategory.Value;
            var strPro = !progressState.HasValue ? "10|20|30" : ((int)progressState.Value).ToString();
            var arrProg = strPro.Split('|');
            if (!string.IsNullOrEmpty(gameCode))
                gameCode = gameCode.ToUpper();
            if (!string.IsNullOrEmpty(gameType))
                gameType = gameType.ToUpper();
            var cache = new Sports_TogetherSchemeQueryInfoCollection();
            var query = from s in list
                        where arrProg.Contains(Convert.ToInt32(s.ProgressStatus).ToString())
                          && (betC == -1 || Convert.ToInt32(s.SchemeBettingCategory) == betC)
                          && (issuseNumber == string.Empty || s.IssuseNumber == issuseNumber)
                          && (s.StopTime >= DateTime.Now)
                          && (gameCode == string.Empty || s.GameCode == gameCode)
                          && (gameType == string.Empty || s.GameType == gameType)
                          && (minMoney == -1 || s.TotalMoney >= minMoney)
                          && (maxMoney == -1 || s.TotalMoney <= maxMoney)
                          && (minProgress == -1 || s.Progress >= minProgress)
                          && (maxProgress == -1 || s.Progress <= maxProgress)
                          && (seC == -1 || Convert.ToInt32(s.Security) == seC)
                          && (key == string.Empty || s.CreateUserId == key || s.SchemeId == key || s.CreaterDisplayName == key)
                        select s;
            cache.TotalCount = query.Count();
            cache.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return cache;
        }

        /// <summary>
        /// 加载过关统计数据到Redis
        /// </summary>
        public static void LoadGGTJ_To_Redis()
        {
            //从sql查询数据
            var arrGameCode = new string[] { "JCZQ", "JCLQ", "BJDC", "T14C", "TR9", "T4CJQ", "T6BQC" };
            var gameCode = string.Empty;
            var gameType = string.Empty;
            var issuseNumber = string.Empty;
            var key = string.Empty;
            SchemeBettingCategory? schemeType = null;
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = DateTime.Now;
            int pageIndex = 0;
            int pageSize = 2000;
            var totalList = new List<SportsOrder_GuoGuanInfo>();
            foreach (var item in arrGameCode)
            {
                gameCode = item;
                if (item == "T14C" || item == "TR9" || item == "T4CJQ" || item == "T6BQC")
                {
                    gameCode = "CTZQ";
                    gameType = item;
                }
                if (gameCode.ToLower() == "ctzq" || gameCode.ToLower() == "bjdc")
                {
                    string prizedIssuse = WCFClients.GameClient.QueryStopIssuseList(gameCode, gameType, 20, "");
                    var prizedIssuseList = prizedIssuse.Split(',');
                    issuseNumber = prizedIssuseList.FirstOrDefault();
                }

                var tojiList = WCFClients.GameQueryClient.QueryReportInfoList_GuoGuan(null, schemeType, key, gameCode, gameType, issuseNumber, startTime, endTime, pageIndex, pageSize);
                totalList.AddRange(tojiList.ReportItemList);
            }
            //存入Redis
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(8);//.DB_CoreCacheData;
            var redisKey = RedisKeys.Key_Core_GGTJ_List;
            db.KeyDeleteAsync(redisKey);
            foreach (var item in totalList)
            {
                try
                {
                    var json = JsonSerializer.Serialize(item);
                    db.ListRightPushAsync(redisKey, json);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// 从Redis中查询过关统计数据
        /// </summary>
        public static SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool isVirtualOrder, SchemeBettingCategory? category, string key
            , string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(8);//.DB_CoreCacheData;
            var redisKey = RedisKeys.Key_Core_GGTJ_List;
            var list = new List<SportsOrder_GuoGuanInfo>();
            //生成列表
            var redisList = db.ListRangeAsync(redisKey).Result;
            foreach (var item in redisList)
            {
                try
                {
                    if (!item.HasValue)
                        continue;
                    var t = JsonSerializer.Deserialize<SportsOrder_GuoGuanInfo>(item.ToString());
                    list.Add(t);
                }
                catch (Exception)
                {
                }
            }
            //查询列表
            var cache = new SportsOrder_GuoGuanInfoCollection();
            var query = from s in list
                        where s.IsVirtualOrder == isVirtualOrder
                        && (!category.HasValue || s.SchemeBettingCategory == category.Value)
                        && (key == string.Empty || s.UserDisplayName == key)
                        && s.GameCode == gameCode.ToUpper()
                        && (gameType == string.Empty || s.GameType == gameType)
                        && (issuseNumber == string.Empty || s.IssuseNumber == issuseNumber)
                        && (s.BetTime >= startTime && s.BetTime < endTime)
                        select s;
            cache.TotalCount = query.Count();
            cache.ReportItemList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return cache;
        }


        #region 查询用户个人信息

        public static async Task<EntityModel.ProfileUserInfo> QueryProfileUserInfo(string userId)
        {
            var info = LoadProfileUserInfoFromRedis(userId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["UserId"] = userId;
                info = await serviceProxyProvider.Invoke<EntityModel.ProfileUserInfo>(param, "api/user/QueryProfileUserInfo");
                SaveProfileUserInfoToRedis(userId, info);
            }
            return info;
        }

        private static EntityModel.ProfileUserInfo LoadProfileUserInfoFromRedis(string userId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileUserInfo_{0}", userId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.ProfileUserInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileUserInfoToRedis(string userId, EntityModel.ProfileUserInfo info)
        {
            try
            {
                var json = JsonSerializer.Serialize(info);
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
                var key = string.Format("ProfileUserInfo_{0}", userId);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询中奖等级

        public static async Task<EntityModel.ProfileBonusLevelInfo> QueryProfileBonusLevelInfo(string userId)
        {
            var info = LoadProfileBonusLevelInfoFromRedis(userId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["UserId"] = userId;
                info = await serviceProxyProvider.Invoke<EntityModel.ProfileBonusLevelInfo>(param, "api/user/QueryProfileBonusLevelInfo");
                SaveProfileBonusLevelInfoToRedis(userId, info);
            }
            return info;
        }

        private static EntityModel.ProfileBonusLevelInfo LoadProfileBonusLevelInfoFromRedis(string userId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileBonusLevelInfo_{0}", userId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.ProfileBonusLevelInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileBonusLevelInfoToRedis(string userId, EntityModel.ProfileBonusLevelInfo info)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileBonusLevelInfo_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询最新中奖

        public static async Task<EntityModel.ProfileLastBonusCollection> QueryProfileLastBonusCollection(string userId)
        {
            var info = LoadProfileLastBonusCollectionFromRedis(userId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["UserId"] = userId;
                info = await serviceProxyProvider.Invoke<EntityModel.ProfileLastBonusCollection>(param, "api/user/QueryProfileLastBonusCollection");
           
                SaveProfileLastBonusCollectionToRedis(userId, info);
            }
            return info;
        }

        private static EntityModel.ProfileLastBonusCollection LoadProfileLastBonusCollectionFromRedis(string userId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileLastBonusCollection_{0}", userId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.ProfileLastBonusCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileLastBonusCollectionToRedis(string userId, EntityModel.ProfileLastBonusCollection info)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileLastBonusCollection_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询个人数据

        public static async Task<EntityModel.ProfileDataReport> QueryProfileDataReport(string userId)
        {
            var info = LoadProfileDataReportFromRedis(userId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["UserId"] = userId;
                info = await serviceProxyProvider.Invoke<EntityModel.ProfileDataReport>(param, "api/user/QueryProfileDataReport");
            
                SaveProfileDataReportToRedis(userId, info);
            }
            return info;
        }

        private static EntityModel.ProfileDataReport LoadProfileDataReportFromRedis(string userId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileDataReport_{0}", userId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.ProfileDataReport>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveProfileDataReportToRedis(string userId, EntityModel.ProfileDataReport info)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("ProfileDataReport_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户跟单人数

        public static async Task<int> QueryTogetherFollowerCount(string userId)
        {
            var info = LoadTogetherFollowerCountFromRedis(userId);
            if (info == -1)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["createUserId"] = userId;
                info = await serviceProxyProvider.Invoke<int>(param, "api/user/QueryTogetherFollowerCount");
              
                SaveTogetherFollowerCountToRedis(userId, info);
            }
            return info;
        }

        private static int LoadTogetherFollowerCountFromRedis(string userId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("FollowerCount_{0}", userId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
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
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("FollowerCount_{0}", userId);
            try
            {
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, count, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户战绩

        public static async Task<EntityModel.CoreModel.UserBeedingListInfoCollection> QueryUserBeedingListInfoCollection(string userId)
        {
            var info = LoadUserBeedingListInfoCollectionFromRedis(userId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["gameCode"] = "";
                param["gameType"] = "";
                param["userId"] = userId;
                param["userDisplayName"] = string.Empty;
                param["pageIndex"] =0;
                param["pageSize"] = 100;
                param["property"] = QueryUserBeedingListOrderByProperty.TotalBonusMoney;
                param["category"] = OrderByCategory.DESC;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.UserBeedingListInfoCollection>(param, "api/user/QueryUserBeedingList");
                //info = WCFClients.GameClient.QueryUserBeedingList("", "", userId, string.Empty, 0, 100, QueryUserBeedingListOrderByProperty.TotalBonusMoney, OrderByCategory.DESC, string.Empty);
                SaveUserBeedingListInfoCollectionToRedis(userId, info);
            }
            return info;
        }

        private static EntityModel.CoreModel.UserBeedingListInfoCollection LoadUserBeedingListInfoCollectionFromRedis(string userId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("UserBeedingList_{0}", userId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.UserBeedingListInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveUserBeedingListInfoCollectionToRedis(string userId, EntityModel.CoreModel.UserBeedingListInfoCollection info)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("UserBeedingList_{0}", userId);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户当前的订单

        public static async Task<EntityModel.CoreModel.UserCurrentOrderInfoCollection> QueryUserCurrentOrderInfoCollection(string userId, string gameCode)
        {
            var info = LoadUserCurrentOrderInfoCollectionFromRedis(userId, gameCode);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["UserId"] = userId;
                param["gameCode"] = gameCode;
            
                param["pageIndex"] = 0;
                param["pageSize"] = 30;
            
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.UserCurrentOrderInfoCollection>(param, "api/user/QueryUserCurrentOrderList");

                //info = WCFClients.GameClient.QueryUserCurrentOrderList(userId, gameCode, string.Empty, 0, 30);
                SaveUserCurrentOrderInfoCollectionToRedis(userId, gameCode, info);
            }
            return info;
        }

        private static EntityModel.CoreModel.UserCurrentOrderInfoCollection LoadUserCurrentOrderInfoCollectionFromRedis(string userId, string gameCode)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("UserCurrentOrderInfoCollection_{0}_{1}", userId, gameCode);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.UserCurrentOrderInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveUserCurrentOrderInfoCollectionToRedis(string userId, string gameCode, EntityModel.CoreModel.UserCurrentOrderInfoCollection info)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("UserCurrentOrderInfoCollection_{0}_{1}", userId, gameCode);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 查询用户中奖订单

        public static async Task<EntityModel.CoreModel.BonusOrderInfoCollection> QueryBonusOrderInfoCollection(string userId, string gameCode, string gameType)
        {
            gameCode = (gameCode == "SZC" ? gameType : gameCode);
            gameType = (gameCode == "SZC" ? "" : gameType);
            var info = LoadBonusOrderInfoCollectionFromRedis(userId, gameCode, gameType);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["Model"] = new QueryBonusInfoListParam() { userId = userId, gameCode = gameCode, gameType = gameType, pageIndex = 0, pageSize = 30};


                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.BonusOrderInfoCollection> (param, "api/Order/QueryBonusInfoList");
                //info = WCFClients.GameQueryClient.QueryBonusInfoList(userId, gameCode, gameType, string.Empty, string.Empty, string.Empty, 0, 30, string.Empty);
                SaveBonusOrderInfoCollectionToRedis(userId, gameCode, gameType, info);
            }
            return info;
        }

        private static EntityModel.CoreModel.BonusOrderInfoCollection LoadBonusOrderInfoCollectionFromRedis(string userId, string gameCode, string gameType)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("BonusOrderInfoCollection_{0}_{1}_{2}", userId, gameCode, gameType);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.BonusOrderInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SaveBonusOrderInfoCollectionToRedis(string userId, string gameCode, string gameType, EntityModel.CoreModel.BonusOrderInfoCollection info)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(10);//.DB_UserBlogData;
            var key = string.Format("BonusOrderInfoCollection_{0}_{1}_{2}", userId, gameCode, gameType);
            try
            {
                var json = JsonSerializer.Serialize(info);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                db.StringSetAsync(key, json, TimeSpan.FromMinutes(validMinute));
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region 用户订单详细

        public static async Task<EntityModel.CoreModel.Sports_SchemeQueryInfo> QuerySportsSchemeInfo(string schemeId)
        {
            var info = LoadSportsSchemeInfoFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.Sports_SchemeQueryInfo>(param, "api/order/QuerySportsSchemeInfo");
                if (info != null && info.ProgressStatus ==EntityModel.Enum.ProgressStatus.Complate && (info.BonusStatus == EntityModel.Enum.BonusStatus.Lose || info.BonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveSportsSchemeInfoToRedis(info);
            }

            return info;
        }
        private static EntityModel.CoreModel.Sports_SchemeQueryInfo LoadSportsSchemeInfoFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_SportsSchemeInfo", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.Sports_SchemeQueryInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveSportsSchemeInfoToRedis(EntityModel.CoreModel.Sports_SchemeQueryInfo info)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_SportsSchemeInfo", info.SchemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }

        }


        public static async Task<EntityModel.CoreModel.BettingOrderInfo> QueryOrderDetailBySchemeId(string schemeId)
        {
            var info = LoadOrderDetailFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.BettingOrderInfo>(param, "api/order/QueryOrderDetailBySchemeId");
                if (info != null && info.ProgressStatus == EntityModel.Enum.ProgressStatus.Complate && (info.BonusStatus == EntityModel.Enum.BonusStatus.Lose || info.BonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveOrderDetailToRedis(info);
            }

            return info;
        }
        private static EntityModel.CoreModel.BettingOrderInfo LoadOrderDetailFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_OrderDetail", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.BettingOrderInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveOrderDetailToRedis(EntityModel.CoreModel.BettingOrderInfo info)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_OrderDetail", info.SchemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<int> QueryProfileFollowedCount(string userId, string gameCode, string gameType)
        {
            var count = LoadOProfileFollowedCountFromRedis(userId, gameCode, gameType);
            if (count == -1)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userId"] = userId; param["gameCode"] = gameCode; param["gameType"] = gameType;
              
                count = await serviceProxyProvider.Invoke<int>(param, "api/order/QueryProfileFollowedCount");
                SaveProfileFollowedCountToRedis(userId, gameCode, gameType, count);
            }

            return count;
        }
        private static int LoadOProfileFollowedCountFromRedis(string userId, string gameCode, string gameType)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("ProfileFollowedCount_{0}_{1}_{2}", userId, gameCode, gameType);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
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
        private static void SaveProfileFollowedCountToRedis(string userId, string gameCode, string gameType, int count)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("ProfileFollowedCount_{0}_{1}_{2}", userId, gameCode, gameType);
                db.StringSetAsync(key, count, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.Sports_TicketQueryInfoCollection> QuerySportsTicketList(string schemeId, int pageIndex, int pageSize)
        {
            var info = LoadSportsTicketListFromRedis(schemeId, pageIndex, pageSize);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                param["pageIndex"] = pageIndex;
                param["pageSize"] = pageSize;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.Sports_TicketQueryInfoCollection>(param, "api/order/QuerySportsTicketList");
                var noPrizeCount = info.TicketList.Where(p => p.BonusStatus ==EntityModel.Enum.BonusStatus.Waitting).Count();
                if (info.TicketList.Count > 0 && noPrizeCount <= 0)
                    SaveSportsTicketListToRedis(schemeId, pageIndex, pageSize, info);
            }

            return info;
        }
        private static EntityModel.CoreModel.Sports_TicketQueryInfoCollection LoadSportsTicketListFromRedis(string schemeId, int pageIndex, int pageSize)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_{1}_{2}_SportsTicketList", schemeId, pageIndex, pageSize);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.Sports_TicketQueryInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveSportsTicketListToRedis(string schemeId, int pageIndex, int pageSize, EntityModel.CoreModel.Sports_TicketQueryInfoCollection info)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_{1}_{2}_SportsTicketList", schemeId, pageIndex, pageSize);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.BDFXCommisionInfo> QueryBDFXCommision(string schemeId, EntityModel.Enum.BonusStatus bonusStatus)
        {
            var info = LoadBDFXCommisionFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["category"] = "Lottery_Hot";
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.BDFXCommisionInfo>(param, "api/data/QueryBDFXCommision");
                if (info != null && (bonusStatus == EntityModel.Enum.BonusStatus.Lose || bonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveBDFXCommisionToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.BDFXCommisionInfo LoadBDFXCommisionFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_BDFXCommision", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.BDFXCommisionInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveBDFXCommisionToRedis(EntityModel.CoreModel.BDFXCommisionInfo info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_BDFXCommision", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.Sports_AnteCodeQueryInfoCollection> QuerySportsOrderAnteCodeList(string schemeId,EntityModel.Enum.BonusStatus bonusStatus)
        {
            var info = LoadSportsOrderAnteCodeFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.Sports_AnteCodeQueryInfoCollection>(param, "api/order/QuerySportsOrderAnteCodeList");
                if (info != null && info.Count > 0 && (bonusStatus == EntityModel.Enum.BonusStatus.Lose || bonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveSportsOrderAnteCodeToRedis(info, schemeId);
            }

            return info;

        }
        private static EntityModel.CoreModel.Sports_AnteCodeQueryInfoCollection LoadSportsOrderAnteCodeFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_SportsOrderAnteCode", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.Sports_AnteCodeQueryInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveSportsOrderAnteCodeToRedis(EntityModel.CoreModel.Sports_AnteCodeQueryInfoCollection info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_SportsOrderAnteCode", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.SingleScheme_AnteCodeQueryInfo> QuerySingleSchemeFullFileName(string schemeId, EntityModel.Enum.BonusStatus bonusStatus)
        {
            var info = LoadSingleSchemeFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.SingleScheme_AnteCodeQueryInfo>(param, "api/order/QuerySingleSchemeFullFileName");
             
                if (info != null && (bonusStatus == EntityModel.Enum.BonusStatus.Lose || bonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveSingleSchemeToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.SingleScheme_AnteCodeQueryInfo LoadSingleSchemeFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_SingleScheme", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.SingleScheme_AnteCodeQueryInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveSingleSchemeToRedis(EntityModel.CoreModel.SingleScheme_AnteCodeQueryInfo info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_SingleScheme", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.BettingOrderInfoCollection> QueryBettingOrderListByChaseKeyLine(string schemeId)
        {
            var info = LoadBettingOrderListByChaseKeyLineFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;

                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.BettingOrderInfoCollection>(param, "api/order/QueryBettingOrderListByChaseKeyLine");
                var isComplate = true;
                while (true)
                {
                    if (info == null || info.OrderList.Count == 0)
                    {
                        isComplate = false;
                        break;
                    }
                    foreach (var issuse in info.OrderList)
                    {
                        if (issuse.ProgressStatus !=EntityModel.Enum.ProgressStatus.Complate)
                        {
                            isComplate = false;
                            break;
                        }
                    }
                    break;
                }
                if (isComplate)
                    SaveBettingOrderListByChaseKeyLineToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.BettingOrderInfoCollection LoadBettingOrderListByChaseKeyLineFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_BettingOrderListByChaseKeyLine", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.BettingOrderInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveBettingOrderListByChaseKeyLineToRedis(EntityModel.CoreModel.BettingOrderInfoCollection info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_BettingOrderListByChaseKeyLine", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.BettingAnteCodeInfoCollection> QueryAnteCodeListBySchemeId(string schemeId, EntityModel.Enum.BonusStatus bonusStatus)
        {
            var info = LoadAnteCodeListBySchemeIdFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.BettingAnteCodeInfoCollection>(param, "api/order/QueryAnteCodeListBySchemeId");
                if (info != null && (bonusStatus == EntityModel.Enum.BonusStatus.Lose || bonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveAnteCodeListBySchemeIdToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.BettingAnteCodeInfoCollection LoadAnteCodeListBySchemeIdFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_AnteCodeListBySchemeId", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.BettingAnteCodeInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveAnteCodeListBySchemeIdToRedis(EntityModel.CoreModel.BettingAnteCodeInfoCollection info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_AnteCodeListBySchemeId", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.Sports_TogetherJoinInfoCollection> QuerySportsTogetherJoinList(string schemeId, EntityModel.Enum.BonusStatus bonusStatus)
        {
            var info = LoadSportsTogetherJoinListFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                param["pageIndex"] = -1; param["pageSize"] = -1; param["userId"] = string.Empty;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.Sports_TogetherJoinInfoCollection>(param, "api/order/QuerySportsTogetherJoinList");
                if (info != null && info.TotalCount > 0 && (bonusStatus == EntityModel.Enum.BonusStatus.Lose || bonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveSportsTogetherJoinListToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.Sports_TogetherJoinInfoCollection LoadSportsTogetherJoinListFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_SportsTogetherJoinList", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.Sports_TogetherJoinInfoCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveSportsTogetherJoinListToRedis(EntityModel.CoreModel.Sports_TogetherJoinInfoCollection info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_SportsTogetherJoinList", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public  static async Task<EntityModel.CoreModel.Sports_TogetherSchemeQueryInfo> QuerySportsTogetherDetail(string schemeId)
        {
            var info = LoadSportsTogetherDetailFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.Sports_TogetherSchemeQueryInfo>(param, "api/order/QuerySportsTogetherDetail");
           
                if (info != null && (info.BonusStatus == EntityModel.Enum.BonusStatus.Lose || info.BonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveSportsTogetherDetailToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.Sports_TogetherSchemeQueryInfo LoadSportsTogetherDetailFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_SportsTogetherDetail", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.Sports_TogetherSchemeQueryInfo>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveSportsTogetherDetailToRedis(EntityModel.CoreModel.Sports_TogetherSchemeQueryInfo info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_SportsTogetherDetail", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static async Task<EntityModel.CoreModel.OrderSingleSchemeCollection> QueryOrderSingleScheme(string schemeId, EntityModel.Enum.BonusStatus bonusStatus)
        {
            var info = LoadOrderSingleSchemeFromRedis(schemeId);
            if (info == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                info = await serviceProxyProvider.Invoke<EntityModel.CoreModel.OrderSingleSchemeCollection>(param, "api/data/QuerySingSchemeDetail");
                if (info != null && (bonusStatus == EntityModel.Enum.BonusStatus.Lose || bonusStatus == EntityModel.Enum.BonusStatus.Win))
                    SaveOrderSingleSchemeToRedis(info, schemeId);
            }

            return info;
        }
        private static EntityModel.CoreModel.OrderSingleSchemeCollection LoadOrderSingleSchemeFromRedis(string schemeId)
        {
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            var key = string.Format("{0}_OrderSingleScheme", schemeId);
            var v = db.StringGetAsync(key).Result;
            if (!v.HasValue)
                return null;
            try
            {
                return JsonSerializer.Deserialize<EntityModel.CoreModel.OrderSingleSchemeCollection>(v.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveOrderSingleSchemeToRedis(EntityModel.CoreModel.OrderSingleSchemeCollection info, string schemeId)
        {
            try
            {
                var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
                var key = string.Format("{0}_OrderSingleScheme", schemeId);
                var json = JsonSerializer.Serialize(info);
                db.StringSetAsync(key, json, TimeSpan.FromHours(12));
            }
            catch (Exception)
            {
            }
        }

        public static void DoClearOrderCacheFile(string schemeId)
        {
            //var key = string.Format("{0}_SportsSchemeInfo", schemeId);
            //var key = string.Format("{0}_OrderDetail", info.SchemeId);
            //var key = string.Format("{0}_{1}_{2}_SportsTicketList", schemeId, pageIndex, pageSize);
            //var key = string.Format("{0}_BDFXCommision", schemeId);
            //var key = string.Format("{0}_SportsOrderAnteCode", schemeId);
            //var key = string.Format("{0}_SingleScheme", schemeId);
            //var key = string.Format("{0}_BettingOrderListByChaseKeyLine", schemeId);
            //var key = string.Format("{0}_AnteCodeListBySchemeId", schemeId);
            //var key = string.Format("{0}_SportsTogetherJoinList", schemeId);
            //var key = string.Format("{0}_SportsTogetherDetail", schemeId);
            //var key = string.Format("{0}_OrderSingleScheme", schemeId);
            var keyList = new List<string>();
            keyList.Add(string.Format("{0}_SportsSchemeInfo", schemeId));
            keyList.Add(string.Format("{0}_OrderDetail", schemeId));
            keyList.Add(string.Format("{0}_BDFXCommision", schemeId));
            keyList.Add(string.Format("{0}_SportsOrderAnteCode", schemeId));
            keyList.Add(string.Format("{0}_SingleScheme", schemeId));
            keyList.Add(string.Format("{0}_BettingOrderListByChaseKeyLine", schemeId));
            keyList.Add(string.Format("{0}_AnteCodeListBySchemeId", schemeId));
            keyList.Add(string.Format("{0}_SportsTogetherJoinList", schemeId));
            keyList.Add(string.Format("{0}_SportsTogetherDetail", schemeId));
            keyList.Add(string.Format("{0}_OrderSingleScheme", schemeId));
            for (int i = 0; i < 100; i++)
            {
                keyList.Add(string.Format("{0}_{1}_{2}_SportsTicketList", schemeId, i, 5));
            }
            var db = RedisHelper.GetInstance(CacheRedisHost, CacheRedisPost, CacheRedisPassword).GetDatabase(11);//.DB_SchemeDetail;
            foreach (var item in keyList)
            {
                db.KeyDeleteAsync(item);
            }
        }

        #endregion

        #region 数字彩开奖号

        /// <summary>
        /// 生成数字彩开奖号码
        /// </summary>
        public static void BuildLotteryNewNumber_ByGameCode(string gameCode)
        {

        }

        #endregion

        #region 用户余额相关

        /// <summary>
        /// 查询用户余额
        /// </summary>
        public static async Task<EntityModel.CoreModel.UserBalanceInfo> QueryUserBalanceAsync(string userId)
        {
            try
            {
                var db = RedisHelper.DB_UserBalance;
                string key = string.Format("UserBalance_{0}", userId);
                EntityModel.CoreModel.UserBalanceInfo userBalance = null;
                var v = db.StringGetAsync(key).Result;
                if (!v.HasValue)
                {
                    Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                    balanceParam["userId"] = userId;
                    userBalance = await serviceProxyProvider.Invoke<EntityModel.CoreModel.UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
                    
                    var json = JsonSerializer.Serialize(userBalance);
                    await db.StringSetAsync(key, json, TimeSpan.FromSeconds(60 * 2));
                }
                else
                {
                    userBalance = JsonSerializer.Deserialize<EntityModel.CoreModel.UserBalanceInfo>(v.ToString());
                }
                return userBalance;
            }
            catch (Exception ex)
            {
                return new EntityModel.CoreModel.UserBalanceInfo();
            }
        }

        /// <summary>
        /// 刷新Redis中用户余额
        /// </summary>
        public static void RefreshRedisUserBalance(string userId)
        {
            try
            {
                var db = RedisHelper.DB_UserBalance;
                string key = string.Format("UserBalance_{0}", userId);
                var userBalance = WCFClients.GameFundClient.QueryUserBalance(userId);
                var json = JsonSerializer.Serialize(userBalance);
                db.StringSetAsync(key, json, TimeSpan.FromSeconds(60 * 2));
            }
            catch (Exception ex)
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("BusinessHelper", "RefreshRedisUserBalance", ex);
            }
        }

        #endregion

        #region 投注判断

        /// <summary>
        /// 用户是否实名认证
        /// </summary>
        public static bool IsUserHasRealName(External.Core.Authentication.UserRealNameInfo info)
        {
            if (info == null)
                return false;
            if (string.IsNullOrEmpty(info.RealName))
                return false;
            return true;
        }

        /// <summary>
        /// 检查彩种和玩法状态
        /// </summary>
        public static void CheckGameStatus(string gameCode, string gameType)
        {
            gameCode = gameCode.ToUpper();
            gameType = gameType.ToUpper();
            var db = RedisHelper.DB_CoreCacheData;
            var gameKey = RedisKeys.Key_AllGameCode;
            var gameArray = db.ListRangeAsync(gameKey).Result;
            foreach (var item in gameArray)
            {
                if (!item.HasValue)
                    continue;

                var array = item.ToString().Split('^');
                if (array.Length != 2)
                    continue;

                if (array[0] == gameCode && array[1] != "0")
                    throw new Exception("彩种暂停销售");
            }
            if (gameCode == "BJDC")
            {
                if (new string[] { "ZJQ", "SXDS", "BQC", "BF" }.Contains(gameType.ToUpper()))
                    throw new Exception("该玩法暂停销售");
            }
        }

        /// <summary>
        /// 检查奖期是否可以投注
        /// </summary>
        public static void CheckBetIssuse(string gameCode, string issuseNumber)
        {
            gameCode = gameCode.ToUpper();
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "FC3D", "PL3", "SSQ", "DLT" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            var fullKey = string.Format("NextIssuse_List_ByLocalStopTime_{0}", gameCode);
            var issuseList = QueryNextIssuseListByKey(fullKey);
            var betIssuse = issuseList.FirstOrDefault(p => p.IssuseNumber == issuseNumber);
            if (betIssuse == null)
                throw new Exception(string.Format("投注奖期{0}不存在", issuseNumber));
            if (DateTime.Now > betIssuse.LocalStopTime)
                throw new Exception(string.Format("投注奖期{0}已过期", issuseNumber));
        }

        /// <summary>
        /// 检查投注时的资金密是否正确
        /// </summary>
        public static void CheckBalancePwd(EntityModel.CoreModel.UserBalanceInfo info, string pwd)
        {
            if (!info.CheckIsNeedPassword("Bet"))
                return;

            string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
            var codePwd = Encipherment.MD5(string.Format("{0}{1}", pwd, _gbKey)).ToUpper();
            if (info.BalancePwd.ToUpper() != codePwd)
                throw new Exception("资金密码不正确");
        }


        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_BJDC_MatchInfo> QueryBJDCMatch(string[] matchIdArray)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
                var matchList = JsonSerializer.Deserialize<List<Cache_BJDC_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.Id)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_BJDC_MatchInfo>();
            }
        }

        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_JCZQ_MatchInfo> QueryJCZQMatch(string[] matchIdArray)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
                var matchList = JsonSerializer.Deserialize<List<Cache_JCZQ_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_JCZQ_MatchInfo>();
            }
        }

        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_JCLQ_MatchInfo> QueryJCLQMatch(string[] matchIdArray)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
                var matchList = JsonSerializer.Deserialize<List<Cache_JCLQ_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_JCLQ_MatchInfo>();
            }
        }

        /// <summary>
        /// 检查投注比赛是否可投注，并返回最早结束的比赛时间
        /// </summary>
        public static DateTime CheckGeneralBettingMatch(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        {
            gameCode = gameCode.ToUpper();
            gameType = gameType.ToUpper();
            if (gameCode == "BJDC")
            {
                var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
                var matchList = QueryBJDCMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new Exception("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                return matchList.Min(m => m.LocalStopTime);
            }
            if (gameCode == "JCZQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCZQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new Exception("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "JCLQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCLQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new Exception("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            return DateTime.Now;
        }

        //验证 不支持的玩法
        private static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<Cache_JCZQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 0：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<Cache_JCLQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<Cache_BJDC_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        /// <summary>
        /// 解析玩法为中文名称
        /// </summary>
        public static string FormatGameType(string gameCode, string gameType)
        {
            var nameList = new List<string>();
            var typeList = gameType.Split(',', '|');
            foreach (var t in typeList)
            {
                nameList.Add(FormatGameType_Each(gameCode, t));
            }
            return string.Join(",", nameList.ToArray());
        }
        public static string FormatGameType_Each(string gameCode, string gameType)
        {
            switch (gameCode)
            {
                #region 足彩

                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":
                            return "胜平负";
                        case "ZJQ":
                            return "总进球";
                        case "SXDS":
                            return "上下单双";
                        case "BF":
                            return "比分";
                        case "BQC":
                            return "半全场";
                    }
                    break;
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":
                            return "让球胜平负";
                        case "BRQSPF":
                            return "胜平负";
                        case "BF":
                            return "比分";
                        case "ZJQ":
                            return "总进球";
                        case "BQC":
                            return "半全场";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":
                            return "胜负";
                        case "RFSF":
                            return "让分胜负";
                        case "SFC":
                            return "胜分差";
                        case "DXF":
                            return "大小分";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            return "14场胜负";
                        case "TR9":
                            return "胜负任9";
                        case "T6BQC":
                            return "6场半全场";
                        case "T4CJQ":
                            return "4场进球";
                    }
                    break;

                #endregion

                #region 重庆时时彩

                case "CQSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "3XHZ":    // 三星和值
                            return "三星和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XBAODAN":   // 二星组选包胆
                            return "二星组选包胆";
                        case "3XBAODAN":   // 三星组选包胆
                            return "三星组选包胆";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "3XBAODIAN":   // 三星组选包点
                            return "三星组选包点";
                        case "2XZXFS":   // 二星组选复式
                            return "二星组选复式";
                        case "2XZXFW":   // 二星组选分位
                            return "二星组选分位";
                        case "3XZXZH":   // 三星直选组合
                            return "三星直选组合";
                    }
                    break;

                #endregion

                #region 江西时时彩

                case "JXSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "4XDX":
                            return "四星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "2XZX":   // 二星组选
                            return "二星组选";
                        case "RX1":   // 任选一
                            return "任选一";
                        case "RX2":   // 任选二
                            return "任选二";
                    }
                    break;

                #endregion

                #region 山东十一选五、广东十一选五、江西十一选五

                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                    switch (gameType)
                    {
                        case "RX1":
                            return "任选一";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "RX7":
                            return "任选七";
                        case "RX8":
                            return "任选八";
                        case "Q2ZHIX":
                            return "前二直选";
                        case "Q3ZHIX":
                            return "前三直选";
                        case "Q2ZUX":
                            return "前二组选";
                        case "Q3ZUX":
                            return "前三组选";
                    }
                    break;

                #endregion

                #region 福彩3D、排列三

                case "FC3D":
                case "PL3":
                    switch (gameType)
                    {
                        case "FS":
                            return "复式";
                        case "HZ":
                            return "和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                    }
                    break;

                #endregion

                #region 双色球

                case "SSQ":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                    }
                    break;

                #endregion

                #region 大乐透

                case "DLT":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                        case "12X2DS":
                            return "12生肖";
                        case "12X2FS":
                            return "12生肖";
                    }
                    break;


                #endregion

                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
            }
            return gameType;
        }


        #endregion

        #region 订单投注，添加到消息队列

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_BetListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        /// <summary>
        /// 获取可用的队列Key
        /// </summary>
        public static string GetUsableList(string key)
        {
            try
            {
                var db = RedisHelper.DB_NoTicket_Order;
                var currentIndexKey = string.Format("{0}_Current", key);
                var indexValue = db.StringGetAsync(currentIndexKey).Result;
                var index = 0;
                if (indexValue.HasValue)
                {
                    //获取索引
                    index = int.Parse(indexValue.ToString());
                    index = index >= Max_BetListCount ? 0 : index + 1;
                }
                db.StringSetAsync(currentIndexKey, index);
                return string.Format("{0}_{1}", key, index);
            }
            catch (Exception)
            {
                return key;
            }
        }

        /// <summary>
        /// 普通数字彩订单投注到Redis消息队列
        /// </summary>
        public static string Redis_Bet_SZC(RedisBet_LotteryBetting info)
        {
            if (info == null || info.BetInfo == null)
                return string.Empty;
            var schemeId = info.BetInfo.IssuseNumberList.Count > 1 ? GetChaseLotterySchemeKeyLine(info.BetInfo.GameCode) : GetSportsBettingSchemeId(info.BetInfo.GameCode);
            info.BetInfo.SchemeId = schemeId;
            info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var json = JsonSerializer.Serialize(info);
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Lottery_OrderBet_List, info.BetInfo.GameCode.ToUpper());
            fullKey = GetUsableList(fullKey);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
            return schemeId;
        }

        /// <summary>
        /// 普通足彩订单投注到Redis消息队列
        /// </summary>
        public static string Redis_Bet_Sports(RedisBet_SportsBetting info)
        {
            if (info == null || info.BetInfo == null)
                return string.Empty;
            var schemeId = GetSportsBettingSchemeId(info.BetInfo.GameCode);
            info.BetInfo.SchemeId = schemeId;
            info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var json = JsonSerializer.Serialize(info);
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Sports_OrderBet_List, info.BetInfo.GameCode.ToUpper());
            fullKey = GetUsableList(fullKey);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
            return schemeId;
        }

        /// <summary>
        /// 单式上传订单投注到Redis消息队列
        /// </summary>
        //public static string Redis_Bet_SingleScheme(RedisBet_SingleScheme info)
        //{
        //    if (info == null || info.BetInfo == null)
        //        return string.Empty;
        //    var schemeId = GetSportsBettingSchemeId(info.BetInfo.GameCode);
        //    info.BetInfo.SchemeId = schemeId;
        //    info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //    var json = JsonSerializer.Serialize(info);
        //    var fullKey = string.Format("{0}_{1}", RedisKeys.Key_SingleScheme_OrderBet_List, info.BetInfo.GameCode.ToUpper());
        //    var db = RedisHelper.DB_NoTicket_Order;
        //    db.ListRightPushAsync(fullKey, json);
        //    return schemeId;
        //}

        /// <summary>
        /// 优化订单投注到Redis消息队列
        /// </summary>
        public static string Redis_Bet_YouHua(RedisBet_YouHuaBet info)
        {
            if (info == null || info.BetInfo == null)
                return string.Empty;
            var schemeId = GetSportsBettingSchemeId(info.BetInfo.GameCode);
            info.BetInfo.SchemeId = schemeId;
            info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var json = JsonSerializer.Serialize(info);
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_YouHua_OrderBet_List, info.BetInfo.GameCode.ToUpper());
            fullKey = GetUsableList(fullKey);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
            return schemeId;
        }

        /// <summary>
        /// 足彩合买订单投注到Redis消息队列
        /// </summary>
        public static string Redis_Bet_Together_Sports(RedisBet_CreateSportsTogether info)
        {
            if (info == null || info.BetInfo == null)
                return string.Empty;
            var schemeId = GetTogetherBettingSchemeId();
            info.BetInfo.SchemeId = schemeId;
            info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var json = JsonSerializer.Serialize(info);
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Together_Sports_OrderBet_List, info.BetInfo.GameCode.ToUpper());
            fullKey = GetUsableList(fullKey);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
            return schemeId;
        }

        /// <summary>
        /// 优化订单投注到Redis消息队列
        /// </summary>
        public static string Redis_Bet_Together_YouHua(RedisBet_CreateYouHuaSchemeTogether info)
        {
            if (info == null || info.BetInfo == null)
                return string.Empty;
            var schemeId = GetTogetherBettingSchemeId();
            info.BetInfo.SchemeId = schemeId;
            info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var json = JsonSerializer.Serialize(info);
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Together_YouHua_OrderBet_List, info.BetInfo.GameCode.ToUpper());
            fullKey = GetUsableList(fullKey);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
            return schemeId;
        }

        /// <summary>
        /// 单式订单投注到Redis消息队列
        /// </summary>
        //public static string Redis_Bet_Together_SingleScheme(RedisBet_CreateSingleSchemeTogether info)
        //{
        //    if (info == null || info.BetInfo == null)
        //        return string.Empty;
        //    var schemeId = GetTogetherBettingSchemeId();
        //    info.BetInfo.BettingInfo.SchemeId = schemeId;
        //    info.BetDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //    var json = JsonSerializer.Serialize(info);
        //    var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Together_SingleScheme_OrderBet_List, info.BetInfo.BettingInfo.GameCode.ToUpper());
        //    var db = RedisHelper.DB_NoTicket_Order;
        //    db.ListRightPushAsync(fullKey, json);
        //    return schemeId;
        //}


        /// <summary>
        /// 追号订单的KeyLine
        /// </summary>
        private static string GetChaseLotterySchemeKeyLine(string gameCode)
        {
            string prefix = "CHASE" + gameCode.ToUpper();
            var keyLine = prefix + UsefullHelper.UUID();
            return keyLine;
        }

        /// <summary>
        /// 普通投注订单号
        /// </summary>
        private static string GetSportsBettingSchemeId(string gameCode)
        {
            string prefix = gameCode.ToUpper();
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 合买投注方案编号
        /// </summary>
        public static string GetTogetherBettingSchemeId()
        {
            string prefix = "TSM";
            return prefix + UsefullHelper.UUID();
        }

        #endregion
    }

}