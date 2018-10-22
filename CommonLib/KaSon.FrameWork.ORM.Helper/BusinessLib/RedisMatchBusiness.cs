using EntityModel.ExceptionExtend;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//using EntityModel.CoreModel.BetingEntities;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using EntityModel;
using StackExchange.Redis;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Sport;
using EntityModel.Redis;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// Redis比赛相关业务
    /// </summary>
    public static class RedisMatchBusiness
    {
        private static Log4Log writerLog =  new Log4Log();

        #region 检查订单是否可投注

        /// <summary>
        /// 检查投注比赛是否可投注，并返回最早结束的比赛时间
        /// </summary>
        public static DateTime CheckGeneralBettingMatch(string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        {
            var sportsManager = new Sports_Manager();
            if (gameCode == "BJDC")
            {
                var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
                var matchList = QueryBJDCMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                BettingHelper.CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                return matchList.Min(m => m.LocalStopTime);
            }
            if (gameCode == "JCZQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCZQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                BettingHelper.CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "JCLQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCLQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                BettingHelper.CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            throw new LogicException(string.Format("错误的彩种编码：{0}", gameCode));
        }

     
        #endregion


        #region Redis缓存北京单场当前比赛

        /// <summary>
        /// 重新加载比赛数据
        /// </summary>
        public static void ReloadCurrentBJDCMatch()
        {
            try
            {
                var matchList = new Sports_Manager().QueryBJDC_Current_CacheMatchList();
                var json = JsonHelper.Serialize(matchList);
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
                RedisHelperEx.DB_Match.Set(fullKey, json);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_BJDC_MatchInfo> QueryBJDCMatch(string[] matchIdArray)
        {
            //try
            //{
            //    var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
            //    var db = RedisHelperEx.DB_Match;
            //    var json = db.StringGetAsync(fullKey).Result;
            //    var matchList = JsonSerializer.Deserialize<List<Cache_BJDC_MatchInfo>>(json);
            //    return matchList.Where(p => matchIdArray.Contains(p.Id) && p.LocalStopTime <= DateTime.Now).ToList();
            //}
            //catch (Exception)
            //{
            //    return new List<Cache_BJDC_MatchInfo>();
            //}

            try
            {
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
                var db = RedisHelperEx.DB_Match;
                var json = db.GetAsync(fullKey).Result;
                var matchList = JsonHelper.Deserialize<List<Cache_BJDC_MatchInfo>>(json);
                var t = matchList.Where(p => p.LocalStopTime > DateTime.Now).ToList();
                return t.Where(p => matchIdArray.Contains(p.Id)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_BJDC_MatchInfo>();
            }

        }

        #endregion

        #region Redis缓存竞彩足球当前比赛

        /// <summary>
        /// 重新加载比赛数据
        /// </summary>
        public static void ReloadCurrentJCZQMatch()
        {
            try
            {
                var matchList = new Sports_Manager().QueryJCZQ_Current_CacheMatchList();
                var json = JsonHelper.Serialize(matchList);
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelperEx.DB_Match;
                db.Set(fullKey, json);
            }
            catch (Exception)
            {
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
                var db = RedisHelperEx.DB_Match;
                var json = db.Get(fullKey);//.Result;
                var matchList = JsonHelper.Deserialize<List<Cache_JCZQ_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_JCZQ_MatchInfo>();
            }
        }

        #endregion

        #region Redis缓存竞彩篮球当前比赛

        /// <summary>
        /// 重新加载比赛数据
        /// </summary>
        public static void ReloadCurrentJCLQMatch()
        {
            try
            {
                var matchList = new Sports_Manager().QueryJCLQ_Current_CacheMatchList();
                var json = JsonHelper.Serialize(matchList);
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelperEx.DB_Match;
                db.Set(fullKey, json);
            }
            catch (Exception)
            {
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
                var db = RedisHelperEx.DB_Match;
                var json = db.GetAsync(fullKey).Result;
                var matchList = JsonHelper.Deserialize<List<Cache_JCLQ_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_JCLQ_MatchInfo>();
            }
        }

        #endregion

        #region Redis缓存竞彩足球比赛结果数据

        /// <summary>
        /// 加载竞彩足球比赛结果
        /// </summary>
        public static void LoadJCZQMatchResult()
        {
            //查询sql中近三天的比赛结果
            var manager = new JCZQMatchManager();
            var resultList = manager.QueryJCZQMatchResultByDay(-30);
            //写入redis库
            var json = JsonHelper.Serialize<List<EntityModel.C_JCZQ_MatchResult>>(resultList);
            var db = RedisHelperEx.DB_Match;
            var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_MatchResult_List);
            db.Set(fullKey, json);
        }

        /// <summary>
        /// 查询竞彩足球比赛结果
        /// </summary>
        public static List<JCZQ_MatchResult> QueryJCZQMatchResult()
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_MatchResult_List);
                var json = db.GetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<JCZQ_MatchResult>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QueryJCZQMatchResult", ex);
                return new List<JCZQ_MatchResult>();
            }
        }

        #endregion

        #region Redis缓存竞彩篮球比赛结果数据

        /// <summary>
        /// 加载竞彩篮球比赛结果
        /// </summary>
        public static void LoadJCLQMatchResult()
        {
            //查询sql中近三天的比赛结果
            var manager = new JCLQMatchManager(); 
             var resultList = manager.QueryJCLQMatchResultByDay(-5);
            //写入redis库
            var json = JsonHelper.Serialize<List<EntityModel.C_JCLQ_MatchResult>>(resultList);
            var db = RedisHelperEx.DB_Match;
            var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_MatchResult_List);
            db.Set(fullKey, json);
        }

        /// <summary>
        /// 查询竞彩篮球比赛结果
        /// </summary>
        public static List<EntityModel.C_JCLQ_MatchResult> QueryJCLQMatchResult()
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_MatchResult_List);
                var json = db.GetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<EntityModel.C_JCLQ_MatchResult>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QueryJCLQMatchResult", ex);
              //  writer.Write("RedisMatchBusiness", "QueryJCLQMatchResult", ex);
                return new List<EntityModel.C_JCLQ_MatchResult>();
            }
        }

        #endregion #region Redis缓存竞彩篮球比赛结果数据

        #region Redis缓存北京单场比赛结果数据

        /// <summary>
        /// 加载北京单场比赛结果
        /// </summary>
        public static void LoadBJDCMatchResult()
        {
            //查询sql中近两期的比赛结果
            var manager = new BJDCMatchManager();
            var resultList = manager.QueryBJDCMatchResultByIssuse(2);
            //写入redis库
            var json = JsonHelper.Serialize<List<EntityModel.C_BJDC_MatchResult_Prize>>(resultList);
            var db = RedisHelperEx.DB_Match;
            var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_MatchResult_List);
            db.Set(fullKey, json);
        }

        /// <summary>
        /// 查询北京单场比赛结果
        /// </summary>
        public static List<EntityModel.C_BJDC_MatchResult_Prize> QueryBJDCMatchResult()
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_MatchResult_List);
                var json = db.GetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<EntityModel.C_BJDC_MatchResult_Prize>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QueryBJDCMatchResult", ex);
               // writer.Write("RedisMatchBusiness", "QueryBJDCMatchResult", ex);
                return new List<EntityModel.C_BJDC_MatchResult_Prize>();
            }
        }

        #endregion

        #region Redis缓存传统足球比赛结果及奖池数据

        /// <summary>
        /// 加载传统足球比赛结果
        /// </summary>
        public static void LoadCTZQBonusPool()
        {
            //查询sql中近三期的比赛结果
            var manager = new Ticket_BonusManager();
            var gameTypeArray = new string[] { "T14C", "TR9", "T6BQC", "T4CJQ" };
            foreach (var gameType in gameTypeArray)
            {
                var poolList = manager.GetBonusPool("CTZQ", gameType, 30);
                //写入redis库
                var json = JsonHelper.Serialize<List<EntityModel.T_Ticket_BonusPool>>(poolList);
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("CTZQ_{0}_{1}", gameType, RedisKeys.Key_MatchResult_List);
                db.Set(fullKey, json);
            }
        }

        /// <summary>
        /// 查询传统足球比赛结果
        /// </summary>
        public static List<EntityModel.T_Ticket_BonusPool> QuseryCTZQBonusPool(string gameType)
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("CTZQ_{0}_{1}", gameType, RedisKeys.Key_MatchResult_List);
                var json = db.GetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<EntityModel.T_Ticket_BonusPool>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QuseryCTZQBonusPool_", ex);
              //  writer.Write("RedisMatchBusiness", "QuseryCTZQBonusPool_" + gameType, ex);
                return new List<EntityModel.T_Ticket_BonusPool>();
            }
        }

        #endregion

        #region Redis缓存数字彩奖池数据

        /// <summary>
        /// 加载数字彩奖池数据
        /// </summary>
        public static void LoadSZCBonusPool()
        {
            //查询sql中近三期的比赛结果
            var manager = new Ticket_BonusManager();
            var gameCodeArray = new string[] { "SSQ", "DLT" };
            foreach (var gameCode in gameCodeArray)
            {
                var poolList = manager.GetBonusPool(gameCode, "", 30);
                //写入redis库
                var json = JsonHelper.Serialize<List<EntityModel.T_Ticket_BonusPool>>(poolList);
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_SZC_BonusPool);
                db.Set(fullKey, json);
            }
        }

        /// <summary>
        /// 查询数字彩奖池数据
        /// </summary>
        public static List<EntityModel.T_Ticket_BonusPool> QuserySZCBonusPool(string gameCode)
        {
            try
            {
                var gameCodeArray = new string[] { "SSQ", "DLT" };
                if (!gameCodeArray.Contains(gameCode))
                    return new List<T_Ticket_BonusPool>();

                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_SZC_BonusPool);
                var json = db.GetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<T_Ticket_BonusPool>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QuserySZCBonusPool_", ex);
               // writer.Write("RedisMatchBusiness", "QuserySZCBonusPool_" + gameCode, ex);
                return new List<T_Ticket_BonusPool>();
            }
        }

        #endregion

        #region Redis缓存数字彩中奖规则

        /// <summary>
        /// 加载数字彩中奖规则
        /// </summary>
        public static void LoadSZCBonusRule()
        {
            //查询所有数字彩中奖规则
            var list = new BonusRuleManager().QueryAllBonusRule();
            //写入redis库
            var json = JsonHelper.Serialize<List<C_Bonus_Rule>>(list);
            var db = RedisHelperEx.DB_Match;
            var fullKey = "SZC_Bonus_Rule_List";
            db.Set(fullKey, json);
        }

        /// <summary>
        /// 查询数字彩中奖规则
        /// </summary>
        public static List<C_Bonus_Rule> QuerySZCBonusRule()
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = "SZC_Bonus_Rule_List";
                var json = db.GetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<C_Bonus_Rule>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QuerySZCBonusRule", ex);
                //writer.Write("RedisMatchBusiness", "QuerySZCBonusRule", ex);
                return new List<C_Bonus_Rule>();
            }
        }

        #endregion

        #region Redis缓存数字彩开奖号

        /// <summary>
        /// 加载数字彩开奖号码
        /// </summary>
        public static void LoadSZCWinNumber()
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "OZB", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                LoadSZCWinNumber(gameCode);
            }
        }

        /// <summary>
        /// 加载数字彩开奖号码
        /// </summary>
        public static void LoadSZCWinNumber(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "OZB", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;

            if (gameCode == "OZB")
            {
                LoadOZBWinNumber();
            }
            else if (gameCode == "SJB")
            {
                LoadSJBWinNumber();
            }
            else
            {
                var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse(gameCode, 100);
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                //清空Key对应的value值
                db.Del(fullKey);

                //写入redis库
                //格式：期号^开奖号
                var array = lastOpenIssuse.Select(p => (RedisValue)string.Format("{0}^{1}", p.IssuseNumber, p.WinNumber)).ToArray();
                db.SetRPush(fullKey, array);
            }
        }

        /// <summary>
        /// 查询数字彩开奖号码
        /// </summary>
        public static Dictionary<string, string> QuerySZCWinNumber(string gameCode)
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
               // var array = db.LRang(fullKey,0,db.LLen(fullKey));
                var array = db.GetRangeArr(fullKey);//
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QuerySZCWinNumber_"+ gameCode, ex);
              //  writer.Write("RedisMatchBusiness", "QuerySZCWinNumber_" + gameCode, ex);
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region 加载欧洲杯开奖数据

        public static void LoadOZBWinNumber()
        {
            var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse("OZB", 2);
            var gjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GJ");
            var gyjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GYJ");

            var db = RedisHelperEx.DB_Match;
            var fullKey = string.Format("{0}_{1}", "OZB", RedisKeys.Key_MatchResult_List);
            //清空Key对应的value值
            db.Del(fullKey);
            //写入redis库
            //格式：玩法^开奖号
            if (gjIssuse != null && !string.IsNullOrEmpty(gjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GJ", gjIssuse.WinNumber);
                db.SetRPush(fullKey, v);
            }
            if (gyjIssuse != null && !string.IsNullOrEmpty(gyjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GYJ", gyjIssuse.WinNumber);
                db.SetRPush(fullKey, v);
            }
        }

        public static Dictionary<string, string> QueryOZBWinNumber()
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", "OZB", RedisKeys.Key_MatchResult_List);
                var array = db.GetRangeArr(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QueryOZBWinNumber" , ex);
             //   writer.Write("RedisMatchBusiness", "QueryOZBWinNumber", ex);
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region 加载世界杯开奖数据

        public static void LoadSJBWinNumber()
        {
            var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse("SJB", 2);
            var gjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GJ");
            var gyjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GYJ");

            var db = RedisHelperEx.DB_Match;
            var fullKey = string.Format("{0}_{1}", "SJB", RedisKeys.Key_MatchResult_List);
            //清空Key对应的value值
            db.Del(fullKey);
            //写入redis库
            //格式：玩法^开奖号
            if (gjIssuse != null && !string.IsNullOrEmpty(gjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GJ", gjIssuse.WinNumber);
                db.SetRPush(fullKey, v);
            }
            if (gyjIssuse != null && !string.IsNullOrEmpty(gyjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GYJ", gyjIssuse.WinNumber);
                db.SetRPush(fullKey, v);
            }
        }

        public static Dictionary<string, string> QuerySJBWinNumber()
        {
            try
            {
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", "SJB", RedisKeys.Key_MatchResult_List);
                var array = db.LRang(fullKey,0,db.LLen(fullKey));
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                Log4Log.Error("RedisMatchBusiness-QuerySJBWinNumber", ex);
                //writer.Write("RedisMatchBusiness", "QuerySJBWinNumber", ex);
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region Redis缓存数字彩当前奖期信息

        public static void LoadCurrentIssuseByOfficialStopTime()
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                LoadCurrentIssuseByOfficialStopTime(gameCode);
            }
        }

        public static void LoadCurrentIssuseByOfficialStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelperEx.DB_CoreCacheData;
            var issuseList = new LotteryGameManager().QueryAllGameCurrentIssuse(true);
            foreach (var item in issuseList)
            {
                string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_CurrentIssuse, item.GameCode);
                var json = JsonHelper.Serialize(item);
                db.Set(key, json);
            }
        }

        public static LotteryIssuse_QueryInfo QueryCurrentIssuseByOfficialStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_CurrentIssuse, gameCode);
            var db = RedisHelperEx.DB_CoreCacheData;
            var json = db.GetAsync(key).Result;
            return JsonHelper.Deserialize<LotteryIssuse_QueryInfo>(json);
        }

        public static void LoadCurrentIssuseByLocalStopTime()
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                LoadCurrentIssuseByLocalStopTime(gameCode);
            }
        }

        public static void LoadCurrentIssuseByLocalStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelperEx.DB_CoreCacheData;
            var issuseList = new LotteryGameManager().QueryAllGameCurrentIssuse(false);
            foreach (var item in issuseList)
            {
                string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_CurrentIssuse, item.GameCode);
                var json = JsonHelper.Serialize(item);
                db.Set(key, json);
            }
        }

        public static LotteryIssuse_QueryInfo QueryCurrentIssuseByLocalStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_CurrentIssuse, gameCode);
            var db = RedisHelperEx.DB_CoreCacheData;
            var json = db.GetAsync(key).Result;
            return JsonHelper.Deserialize<LotteryIssuse_QueryInfo>(json);
        }

        #endregion

        #region Redis缓存未来奖期列表

        public static void LoadNextIssuseListByOfficialStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelperEx.DB_CoreCacheData;
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            var issuseList = new LotteryGameManager().QueryNextIssuseList(true, gameCode, 10);
            if (issuseList.Count <= 0)
                return;
            db.Del(key);
            foreach (var item in issuseList)
            {
                var json = JsonHelper.Serialize(item);
                db.SetRPush(key, json);
            }
        }

        public static void LoadNextIssuseListByLocalStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelperEx.DB_CoreCacheData;
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            var issuseList = new LotteryGameManager().QueryNextIssuseList(false, gameCode, 10);
            if (issuseList.Count <= 0)
                return;
            db.Del(key);
            foreach (var item in issuseList)
            {
                var json = JsonHelper.Serialize(item);
                db.SetRPush(key, json);
            }
        }

        #endregion

    }
}
