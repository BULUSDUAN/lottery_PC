using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;



<<<<<<< HEAD:CommonLib/Lottery.kg.ORM.Helper/Redis/WebRedisHelper.cs
using EntityModel.CoreModel;
using KaSon.FrameWork.Helper;
=======
>>>>>>> 77f9da3f72619db400865d437806eabbbff34413:CommonLib/KaSon.FrameWork.Common/Redis/WebRedisHelper.cs



namespace KaSon.FrameWork.Common.Redis
{
    public class WebRedisHelper
    {
        //        /// <summary>
        //        /// 按官方结束时间查询未来期的奖期
        //        /// </summary>
        //        public static List<LotteryIssuse_QueryInfo> QueryNextIssuseListByOfficialStopTime(string gameCode)
        //        {
        //            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
        //            return QueryNextIssuseListByKey(key);
        //        }

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
            var db = RedisHelper.DB_CoreCacheData;
            var jsonList = db.ListRangeAsync(key).Result;
            foreach (var json in jsonList)
            {
                if (!json.HasValue) continue;
                try
                {
                    var issuse = JsonHelper.Deserialize<LotteryIssuse_QueryInfo>(json);
                    list.Add(issuse);
                }
                catch (Exception)
                {
                }
            }
            return list;
        }

        //        /// <summary>
        //        /// 从Redis中查询系统配置
        //        /// </summary>
        //        //public static string QueryCoreConfigFromRedis(string key)
        //        //{
        //        //    var db = RedisHelper.DB_CoreCacheData;
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
   //         //var db = RedisHelper.DB_CoreCacheData;
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
            //              && (issuseNumber == string.Empty || s.IssuseNumber == issuseNumber)
            //              && (s.StopTime >= DateTime.Now)
            //              && (gameCode == string.Empty || s.GameCode == gameCode)
            //              && (gameType == string.Empty || s.GameType == gameType)
            //              && (minMoney == -1 || s.TotalMoney >= minMoney)
            //              && (maxMoney == -1 || s.TotalMoney <= maxMoney)
            //              && (minProgress == -1 || s.Progress >= minProgress)
            //              && (maxProgress == -1 || s.Progress <= maxProgress)
            //              && (seC == -1 || Convert.ToInt32(s.Security) == seC)
            //              && (key == string.Empty || s.CreateUserId == key || s.SchemeId == key || s.CreaterDisplayName == key)
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

    }
}