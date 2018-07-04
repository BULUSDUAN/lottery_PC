using EntityModel.ExceptionExtend;
using KaSon.FrameWork.ORM.Helper.UserHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel.BetingEntities;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using EntityModel.XmlAnalyzer;
using KaSon.FrameWork.Common.Redis;
using EntityModel;
using StackExchange.Redis;
using EntityModel.CoreModel;
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
        public static DateTime CheckGeneralBettingMatch(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        {
            var sportsManager = new Sports_Manager();
            if (gameCode == "BJDC")
            {
                var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
                var matchList = QueryBJDCMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                return matchList.Min(m => m.LocalStopTime);
            }
            if (gameCode == "JCZQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCZQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
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
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            throw new LogicException(string.Format("错误的彩种编码：{0}", gameCode));
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
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
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
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
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
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
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
                var db = RedisHelper.DB_Match;
                db.StringSetAsync(fullKey, json);
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
            //    var db = RedisHelper.DB_Match;
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
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
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
                var db = RedisHelper.DB_Match;
                db.StringSetAsync(fullKey, json);
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
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
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
                var db = RedisHelper.DB_Match;
                db.StringSetAsync(fullKey, json);
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
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
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
            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_MatchResult_List);
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询竞彩足球比赛结果
        /// </summary>
        public static List<JCZQ_MatchResult> QueryJCZQMatchResult()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<JCZQ_MatchResult>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("RedisMatchBusiness-QueryJCZQMatchResult", ex);
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
            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_MatchResult_List);
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询竞彩篮球比赛结果
        /// </summary>
        public static List<EntityModel.C_JCLQ_MatchResult> QueryJCLQMatchResult()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<EntityModel.C_JCLQ_MatchResult>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("RedisMatchBusiness-QueryJCLQMatchResult", ex);
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
            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_MatchResult_List);
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询北京单场比赛结果
        /// </summary>
        public static List<EntityModel.C_BJDC_MatchResult_Prize> QueryBJDCMatchResult()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<EntityModel.C_BJDC_MatchResult_Prize>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("RedisMatchBusiness-QueryBJDCMatchResult", ex);
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
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("CTZQ_{0}_{1}", gameType, RedisKeys.Key_MatchResult_List);
                db.StringSetAsync(fullKey, json);
            }
        }

        /// <summary>
        /// 查询传统足球比赛结果
        /// </summary>
        public static List<EntityModel.T_Ticket_BonusPool> QuseryCTZQBonusPool(string gameType)
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("CTZQ_{0}_{1}", gameType, RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<EntityModel.T_Ticket_BonusPool>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("RedisMatchBusiness-QuseryCTZQBonusPool_", ex);
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
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_SZC_BonusPool);
                db.StringSetAsync(fullKey, json);
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

                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_SZC_BonusPool);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<T_Ticket_BonusPool>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("RedisMatchBusiness-QuserySZCBonusPool_", ex);
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
            var db = RedisHelper.DB_Match;
            var fullKey = "SZC_Bonus_Rule_List";
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询数字彩中奖规则
        /// </summary>
        public static List<C_Bonus_Rule> QuerySZCBonusRule()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = "SZC_Bonus_Rule_List";
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonHelper.Deserialize<List<C_Bonus_Rule>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("RedisMatchBusiness-QuerySZCBonusRule", ex);
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
            else
            {
                var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse(gameCode, 100);
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                //清空Key对应的value值
                db.KeyDeleteAsync(fullKey);

                //写入redis库
                //格式：期号^开奖号
                var array = lastOpenIssuse.Select(p => (RedisValue)string.Format("{0}^{1}", p.IssuseNumber, p.WinNumber)).ToArray();
                db.ListRightPushAsync(fullKey, array);
            }
        }

        /// <summary>
        /// 查询数字彩开奖号码
        /// </summary>
        public static Dictionary<string, string> QuerySZCWinNumber(string gameCode)
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
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
                writerLog.ErrrorLog("RedisMatchBusiness-QuerySZCWinNumber_"+ gameCode, ex);
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

            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "OZB", RedisKeys.Key_MatchResult_List);
            //清空Key对应的value值
            db.KeyDeleteAsync(fullKey);
            //写入redis库
            //格式：玩法^开奖号
            if (gjIssuse != null && !string.IsNullOrEmpty(gjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GJ", gjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
            if (gyjIssuse != null && !string.IsNullOrEmpty(gyjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GYJ", gyjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
        }

        public static Dictionary<string, string> QueryOZBWinNumber()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "OZB", RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
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
                writerLog.ErrrorLog("RedisMatchBusiness-QueryOZBWinNumber" , ex);
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

            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "SJB", RedisKeys.Key_MatchResult_List);
            //清空Key对应的value值
            db.KeyDeleteAsync(fullKey);
            //写入redis库
            //格式：玩法^开奖号
            if (gjIssuse != null && !string.IsNullOrEmpty(gjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GJ", gjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
            if (gyjIssuse != null && !string.IsNullOrEmpty(gyjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GYJ", gyjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
        }

        public static Dictionary<string, string> QuerySJBWinNumber()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "SJB", RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
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
                writerLog.ErrrorLog("RedisMatchBusiness-QuerySJBWinNumber", ex);
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
            var db = RedisHelper.DB_CoreCacheData;
            var issuseList = new LotteryGameManager().QueryAllGameCurrentIssuse(true);
            foreach (var item in issuseList)
            {
                string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_CurrentIssuse, item.GameCode);
                var json = JsonHelper.Serialize(item);
                db.StringSetAsync(key, json);
            }
        }

        public static LotteryIssuse_QueryInfo QueryCurrentIssuseByOfficialStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_CurrentIssuse, gameCode);
            var db = RedisHelper.DB_CoreCacheData;
            var json = db.StringGetAsync(key).Result;
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
            var db = RedisHelper.DB_CoreCacheData;
            var issuseList = new LotteryGameManager().QueryAllGameCurrentIssuse(false);
            foreach (var item in issuseList)
            {
                string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_CurrentIssuse, item.GameCode);
                var json = JsonHelper.Serialize(item);
                db.StringSetAsync(key, json);
            }
        }

        public static LotteryIssuse_QueryInfo QueryCurrentIssuseByLocalStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_CurrentIssuse, gameCode);
            var db = RedisHelper.DB_CoreCacheData;
            var json = db.StringGetAsync(key).Result;
            return JsonHelper.Deserialize<LotteryIssuse_QueryInfo>(json);
        }

        #endregion

        #region Redis缓存未来奖期列表

        public static void LoadNextIssuseListByOfficialStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            var issuseList = new LotteryGameManager().QueryNextIssuseList(true, gameCode, 10);
            if (issuseList.Count <= 0)
                return;
            db.KeyDeleteAsync(key);
            foreach (var item in issuseList)
            {
                var json = JsonHelper.Serialize(item);
                db.ListRightPushAsync(key, json);
            }
        }

        public static void LoadNextIssuseListByLocalStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            var issuseList = new LotteryGameManager().QueryNextIssuseList(false, gameCode, 10);
            if (issuseList.Count <= 0)
                return;
            db.KeyDeleteAsync(key);
            foreach (var item in issuseList)
            {
                var json = JsonHelper.Serialize(item);
                db.ListRightPushAsync(key, json);
            }
        }

        #endregion

    }
}
