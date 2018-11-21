using EntityModel.CoreModel;
using EntityModel.LotteryJsonInfo;
using EntityModel.MatchModel;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Common.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KaSon.FrameWork.Common
{
    public class Json_CTZQ
    {
        #region 文件路径

        /// <summary>
        /// 传统足球 - 奖期数据文件
        /// </summary>
        private static string IssuseFile(string type)
        {
            return "/MatchData/" + "ctzq/Match_" + type + "_Issuse_List.json";
        }
        private static IList<CTZQ_IssuseInfo> IssuseFile_Mg(string type)
        {
          //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter_ZJQ = Builders<CTZQ_IssuseInfo>.Filter.Eq(b => b.GameType, type);
            return MgHelper.MgDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo").Find<CTZQ_IssuseInfo>(filter_ZJQ).ToList();
        }


        /// <summary>
        /// 传统足球 - 根据奖期获取队伍信息文件地址
        /// </summary>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string type, string issuse)
        {
            return "/MatchData/" + "ctzq" + "/" + issuse + "/Match_" + type + "_List.json";
        }
        private static IList<CTZQ_MatchInfo> MatchFile_Mg(string type, string issuse)
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter_ZJQ = Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.GameType, type) & Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
            return MgHelper.MgDB.GetCollection<CTZQ_MatchInfo>("CTZQ_MatchInfo").Find<CTZQ_MatchInfo>(filter_ZJQ).ToList();
        }
        /// <summary>
        /// 传统足球 - 根据奖期获取队伍平均赔率数据
        /// </summary>
        /// <returns>队伍信息文件地址</returns>
        private static string OddFiles(string type, string issuse)
        {
            return "/MatchData/" + "ctzq" + "/" + issuse + "/Match_" + type + "_Odds_List.json";
        }
        private static string OddFiles_Mg(string type, string issuse)
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter = Builders<BsonDocument>.Filter.Eq("GameType", type) & Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuse);
            var document = MgHelper.MgDB.GetCollection<BsonDocument>("C_CTZQ_Odds").Find(filter).FirstOrDefault();
            string text = "";
            if (document != null)
            {
                 text = document["Content"].ToString().Trim();
            }
            return text;
        }
        /// <summary>
        /// 传统足球 - 开奖结果文件
        /// </summary>
        /// <returns>开奖结果文件地址</returns>
        private static string BonusFile(string type, string issuse)
        {
            return "/MatchData/" + "ctzq" + "/" + issuse + "/CTZQ_" + type + "_BonusLevel.json";
        }

        private static IList<CTZQ_BonusLevelInfo> BonusFile_Mg(string type, string issuse)
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter_ZJQ = Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.GameType, type) & Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
            return MgHelper.MgDB.GetCollection<CTZQ_BonusLevelInfo>("CTZQ_BonusLevelInfo").Find<CTZQ_BonusLevelInfo>(filter_ZJQ).ToList();
        }
        #endregion

        public static List<CTZQ_MatchInfo_WEB> MatchList_WEB(string issuse, string gameType)
        {
            BettingHelper bizHelper = new BettingHelper();


#if MGDB
             var match = MatchFile_Mg(gameType, issuse);
            var odds = bizHelper.GetMatchInfoList<CTZQ_OddInfo>(OddFiles_Mg(gameType, issuse));
#else
            var match = bizHelper.GetMatchInfoList<CTZQ_MatchInfo>(MatchFile(gameType, issuse));
            var odds = bizHelper.GetMatchInfoList<CTZQ_OddInfo>(OddFiles(gameType, issuse));
#endif

           

            var list = new List<CTZQ_MatchInfo_WEB>();
            foreach (var item in match)
            {
                var res = odds.FirstOrDefault(p => p.Id == item.Id);

#region 队伍基础信息
                var info = new CTZQ_MatchInfo_WEB()
                {
                    GameCode = item.GameCode,
                    Color = item.Color,
                    GameType = item.GameType,
                    OrderNumber = item.OrderNumber,
                    Id = item.Id,
                    //UpdateTime = item.UpdateTime.ToString("yyyyMMddHHmmss"),
                    //MatchId = item.MatchId,
                    MatchName = item.MatchName,
                    //MatchResult = item.MatchResult,
                    MatchStartTime = Convert.ToDateTime(item.MatchStartTime),
                    //MatchStartTime = item.MatchStartTime.ToString("yyyyMMddHHmmss"),
                    MatchState = item.MatchState,
                    //HomeTeamHalfScore = item.HomeTeamHalfScore,
                    //HomeTeamId = item.HomeTeamId,
                    HomeTeamName = BettingHelper.GetTeamName(item.HomeTeamName),
                    //HomeTeamScore = item.HomeTeamScore,
                    HomeTeamStanding = item.HomeTeamStanding,
                    //GuestTeamHalfScore = item.GuestTeamHalfScore,
                    //GuestTeamId = item.GuestTeamId,
                    GuestTeamName = BettingHelper.GetTeamName(item.GuestTeamName),
                    //GuestTeamScore = item.GuestTeamScore,
                    GuestTeamStanding = item.GuestTeamStanding,
                    IssuseNumber = item.IssuseNumber,
                    //Mid = item.Mid,
                    FXId = item.FXId,
                };
#endregion

#region 附加平均赔率数据

                if (res != null)
                {
                    info.AverageOdds = res.AverageOdds;
                    if (gameType == "T6BQC")
                    {
                        info.AverageOdds = res.FullAverageOdds + ";" + res.HalfAverageOdds;
                    }
                    //info.KLFlat = res.KLFlat;
                    //info.KLLose = res.KLLose;
                    //info.KLWin = res.KLWin;
                    //info.LSFlat = res.LSFlat;
                    //info.LSLose = res.LSLose;
                    //info.LSWin = res.LSWin;
                    //info.YPSW = res.YPSW;
                }

#endregion

                list.Add(info);
            }
            return list;
        }

        public static List<CTZQ_MatchList_AnteCode> GetMatchListToOrderDetail(string issuse, string gameType, string anteCode)
        {
            List<CTZQ_MatchList_AnteCode> ctzqList = new List<CTZQ_MatchList_AnteCode>();

            BettingHelper bizHelper = new BettingHelper();
          //  var match = MatchFile_Mg(gameType, issuse);// bizHelper.GetMatchInfoList<CTZQ_MatchInfo>(MatchFile(gameType, issuse));



#if MGDB
          var match = MatchFile_Mg(gameType, issuse);
#else
            var match = bizHelper.GetMatchInfoList<CTZQ_MatchInfo>(MatchFile(gameType, issuse));
#endif

            if (match != null && match.Count > 0 && !string.IsNullOrEmpty(anteCode))
            {
                var arr = anteCode.Split('|');
                var anteCodeArr = anteCode.Split('|')[0].Split(',');
                match = match.OrderBy(s => s.OrderNumber).ToList();
                foreach (var item in match)
                {
                    var antecode = anteCodeArr[item.OrderNumber - 1];
                    if ( gameType.ToUpper() == "T4CJQ")
                    {
                        antecode = anteCodeArr[item.OrderNumber * 2 - 2] + "," + anteCodeArr[item.OrderNumber * 2 - 1];
                    }
                    if (gameType.ToUpper() == "T6BQC")
                    {
                        antecode = anteCodeArr[item.OrderNumber - 2] + "," + anteCodeArr[item.OrderNumber - 1];
                    }
                    CTZQ_MatchList_AnteCode info = new CTZQ_MatchList_AnteCode();
                    info.AnteCode = antecode;
                    info.CurrentSp = string.Empty;
                    info.Detail_RF = string.Empty;
                    info.Detail_YSZF = string.Empty;
                    if (item.HomeTeamScore != -1)
                        info.FullResult = item.HomeTeamScore + "-" + item.GuestTeamScore;
                    else
                        info.FullResult = string.Empty;
                    info.GuestTeamId = item.GuestTeamId;
                    info.GuestTeamName = item.GuestTeamName;
                    if (item.HomeTeamHalfScore != -1)
                        info.HalfResult = item.HomeTeamHalfScore + "-" + item.GuestTeamHalfScore;
                    else
                        info.HalfResult = string.Empty;
                    info.HomeTeamId = item.HomeTeamId;
                    info.HomeTeamName = item.HomeTeamName;
                    if (arr.Length > 1)
                        info.IsDan = arr[1].Split(',').Contains((item.OrderNumber - 1).ToString());
                    else info.IsDan = false;
                    info.IssuseNumber = item.IssuseNumber;
                    info.LeagueColor = item.Color;
                    info.LeagueName = item.MatchName;
                    info.LetBall = 0;
                    info.MatchId = item.MatchId.ToString();
                    info.MatchResult = item.MatchResult;
                    info.MatchResultSp = 0M;
                    info.MatchState = item.MatchState.ToString();
                    info.StartTime = Convert.ToDateTime(item.MatchStartTime);
                    info.OrderNumber = item.OrderNumber;
                    ctzqList.Add(info);
                }
            }
            return ctzqList;
        }



        public static CTZQ_MatchInfo_New MatchList_New(string issuse, string gameType)
        {
            BettingHelper bizHelper = new BettingHelper();



#if MGDB
             var match = MatchFile_Mg(gameType, issuse);// bizHelper.GetMatchInfoList<CTZQ_MatchInfo>(MatchFile(gameType, issuse));
            var odds = bizHelper.GetMatchInfoList<CTZQ_OddInfo>(OddFiles_Mg(gameType, issuse));
#else
            var match = bizHelper.GetMatchInfoList<CTZQ_MatchInfo>(MatchFile(gameType, issuse));
            var odds = bizHelper.GetMatchInfoList<CTZQ_OddInfo>(OddFiles(gameType, issuse));
#endif



            CTZQ_MatchInfo_New matchinfo = new CTZQ_MatchInfo_New();
            matchinfo.stop_sale_time = "";
            matchinfo.term_no = issuse;
            matchinfo.match = new List<Match>();
            foreach (var item in match)
            {
                var res = odds.FirstOrDefault(p => p.Id == item.Id);

#region 队伍基础信息
                var info = new Match()
                {
                    match_name = item.MatchName,
                    away_team = item.GuestTeamName.Replace("&nbsp;", ""),
                    home_team = item.HomeTeamName.Replace("&nbsp;", ""),
                    bout_index = item.OrderNumber.ToString(),
                    match_time = item.MatchStartTime,
                    url = "",
                    color = item.Color
                };
#endregion

#region 附加平均赔率数据

                if (res != null)//"全:2.23|3.22|3.02"
                {
                    if (gameType == "T14C" || gameType == "T4CJQ" || gameType == "TR9")
                    {
                        string[] aOdds = res.AverageOdds.Split('|');
                        if (aOdds.Length == 3)
                        {
                            info.odd_home = aOdds[0] == null ? "0" : aOdds[0];
                            info.odd_draw = aOdds[2] == null ? "0" : aOdds[2];
                            info.odd_away = aOdds[1] == null ? "0" : aOdds[1];
                        }
                    }
                    else if (gameType == "T6BQC")
                    {
                        string[] aOdds = res.FullAverageOdds.Replace("全:", "").Split('|');
                        if (aOdds.Length == 3)
                        {
                            info.odd_home = aOdds[0] == null ? "0" : aOdds[0];
                            info.odd_draw = aOdds[1] == null ? "0" : aOdds[1];
                            info.odd_away = aOdds[2] == null ? "0" : aOdds[2];
                        }
                    }
                   
                }

#endregion

                matchinfo.match.Add(info);
            }
            return matchinfo;
        }


        /// <summary>
        /// 传统足球 - 奖期数据列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns>奖期数据列表</returns>
        public static List<CtzqIssuesWeb> IssuseList(string type)
        {
            var bizHelper = new BettingHelper();
            List<CtzqIssuesWeb> resultlist = new List<CtzqIssuesWeb>();
            try
            {

#if MGDB


                
                var issues = IssuseFile_Mg(type);
               
                foreach (var item in issues)
                {
                    CtzqIssuesWeb iweb = new CtzqIssuesWeb()
                    {
                        CreateTime = item.CreateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        GameCode = item.GameCode,
                        GameType = item.GameType,
                        Id = item.Id,
                        IssuseNumber = item.IssuseNumber,
                        OfficialStopTime = item.OfficialStopTime,
                        StartTime = item.StartTime,
                        StopBettingTime = item.StopBettingTime,
                        WinNumber = item.WinNumber

                    };
                    resultlist.Add(iweb);
                }


#else
                resultlist = bizHelper.GetMatchInfoList<CtzqIssuesWeb>(IssuseFile(type));


#endif



            }
            catch (Exception ex)
            {
              
            }
            return resultlist;
        }
    }
}