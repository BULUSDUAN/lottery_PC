using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using EntityModel.Enum;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Common.Utilities;
using EntityModel;
using MongoDB.Driver;
using MongoDB.Bson;

namespace KaSon.FrameWork.Common
{
    public class Json_BJDC
    {

        #region 文件路径

        /// <summary>
        /// 北单 - 根据奖期获取队伍信息文件地址
        /// </summary>
        /// <param name="issuse">期数</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string issuse)
        {
            return   "/MatchData/" + "bjdc" + "/" + issuse + "/Match_List.json";
        }


        //private static string MatchFile_Mg(string issuse)
        //{

        //    // .

        //    return "/MatchData/" + "bjdc" + "/" + issuse + "/Match_List.json";
        //}

        /// <summary>
        /// 北单 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="issuse">期数</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string issuse)
        {
            return "/MatchData/" + "bjdc" + "/" + issuse + "/MatchResult_List.json";
        }

        /// <summary>
        /// 北单 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="issuse">期数</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string issuse)
        {

            return "/MatchData/" + "bjdc" + "/" + issuse + "/SP_" + type + ".json";

        }
        /// <summary>
        /// 改为Mg 读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="issuse"></param>
        /// <returns></returns>
        private static IList<T>  SPFile_Mg<T>(string type, string issuse)
        {

            ///
            ///BJDC_SP_SPF  胜平负SPF
            ///BJDC_SP_ZJQ  总进球ZJQ
            ///BJDC_SP_SXDS  上下单双SXDS
            ///BJDC_SP_BF  比分BF
            ///BJDC_SP_BQC  半全场BJBQC
            ///
            object list = null;
           
            switch (type)
            {
                
                case "ZJQ":
                    var filter_ZJQ = Builders<BJDC_ZJQ_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_ZJQ_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_ZJQ_SpInfo>(filter_ZJQ).ToList();

                    break;
                case "SXDS":
                    var filter_SXDS = Builders<BJDC_SXDS_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_SXDS_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_SXDS_SpInfo>(filter_SXDS).ToList();

                    break;
                case "BF":
                    var filter_BF = Builders<BJDC_BF_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_BF_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_BF_SpInfo>(filter_BF).ToList();

                    break;
                case "BQC":
                    var filter_BQC = Builders<BJDC_BQC_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_BQC_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_BQC_SpInfo>(filter_BQC).ToList();

                    break;
                default://BJDC_SP_SPF
                    var filter = Builders<BJDC_SPF_SpInfo>.Filter.Eq(b=>b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_SPF_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_SPF_SpInfo>(filter).ToList();
                    break;

            }
           

            return (List<T>)list;

        }
        /// <summary>
        /// 北单 - SP走势文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="issuse">期数</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>SP文件地址</returns>
        private static string SPTrendFile(string type, string issuse, string matchId)
        {
            return "/MatchData/" + "bjdc" + "/" + issuse + "/" + type + "_SP_Trend_" + matchId + ".json";
        }

        #endregion
        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期数</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<BJDC_MatchInfo_WEB> MatchList_WEB(string issuse, string gameType, bool isLeftJoin = true)
        {
            BettingHelper bizHelper = new BettingHelper();




#if MGDB
               var match = MgMatchDataHelper.BJDC_Match_List_ByIssuse(issuse);
            var matchresult = MgMatchDataHelper.BJDC_MatchResult_List_ByIssuse(issuse);
            var sp_spf = SPFile_Mg<BJDC_SPF_SpInfo>(gameType, issuse); //胜平负sp数据
            var sp_zjq = SPFile_Mg<BJDC_ZJQ_SpInfo>(gameType, issuse);  //总进球sp数据
            var sp_sxds = SPFile_Mg<BJDC_SXDS_SpInfo>(gameType, issuse); //上下单双sp数据
            var sp_bf = SPFile_Mg<BJDC_BF_SpInfo>(gameType, issuse);  //比分sp数据
            var sp_bqc = SPFile_Mg<BJDC_BQC_SpInfo>(gameType, issuse); //半全场sp数据
#else

            var match = bizHelper.GetMatchInfoList<BJDC_MatchInfo>(MatchFile(issuse)).Where(p => p.MatchState == BJDCMatchState.Sales).ToList();
            var matchresult = bizHelper.GetMatchInfoList<BJDC_MatchResultInfo>(MatchResultFile(issuse));
            var sp_spf = bizHelper.GetMatchInfoList<BJDC_SPF_SpInfo>(SPFile(gameType, issuse)); //胜平负sp数据
            var sp_zjq = bizHelper.GetMatchInfoList<BJDC_ZJQ_SpInfo>(SPFile(gameType, issuse)); //总进球sp数据
            var sp_sxds = bizHelper.GetMatchInfoList<BJDC_SXDS_SpInfo>(SPFile(gameType, issuse)); //上下单双sp数据
            var sp_bf = bizHelper.GetMatchInfoList<BJDC_BF_SpInfo>(SPFile(gameType, issuse)); //比分sp数据

            var sp_bqc = bizHelper.GetMatchInfoList<BJDC_BQC_SpInfo>(SPFile(gameType, issuse)); //半全场sp数据
#endif
            //   var sp_bqc = bizHelper.GetMatchInfoList<BJDC_BQC_SpInfo>(SPFile(gameType, issuse)); //半全场sp数据











            match = match.Where(s => long.Parse(Convert.ToDateTime(s.LocalStopTime).ToString("yyyyMMddHHmmss")) > long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))).ToList();
            var list = new List<BJDC_MatchInfo_WEB>();
            foreach (var item in match)
            {
                var res = matchresult.FirstOrDefault(p => p.Id == item.Id);

#region 队伍基础信息
                var info = new BJDC_MatchInfo_WEB()
                {
                    //CreateTime = item.CreateTime.ToString("yyyyMMddHHmmss"),
                    //FlatOdds = item.FlatOdds,
                    GuestTeamName = BettingHelper.GetTeamName(item.GuestTeamName),
                    //GuestTeamSort = item.GuestTeamSort,
                    //GuestTeamId = item.GuestTeamId.ToString(),
                    HomeTeamName = BettingHelper.GetTeamName(item.HomeTeamName),
                    //HomeTeamSort = item.HomeTeamSort,
                    //HomeTeamId = item.HomeTeamId.ToString(),
                    Id = item.Id,
                    IssuseNumber = item.IssuseNumber,
                    LetBall = item.LetBall,
                    MatchState = (int)item.MatchState,
                    MatchColor = item.MatchColor,
                    //MatchId = item.MatchId,
                    MatchName = item.MatchName,
                    LocalStopTime = Convert.ToDateTime(item.LocalStopTime),
                    //LocalStopTime = item.LocalStopTime.ToString("yyyyMMddHHmmss"),
                    //LoseOdds = item.LoseOdds,
                    MatchOrderId = item.MatchOrderId,
                    //MatchStartTime = item.MatchStartTime.ToString("yyyyMMddHHmmss"),
                    MatchStartTime = Convert.ToDateTime(item.MatchStartTime),
                    //WinOdds = item.WinOdds,
                    //Mid = item.Mid,
                    FXId = item.FXId,
                };
#endregion

#region 附加队伍结果信息
                if (res != null)
                {
                    //info.ZJQ_Result = res.ZJQ_Result;
                    //info.ZJQ_SP = res.ZJQ_SP;
                    //info.SXDS_SP = res.SXDS_SP;
                    //info.SXDS_Result = res.SXDS_Result;
                    //info.SPF_SP = res.SPF_SP;
                    //info.SPF_Result = res.SPF_Result;
                    //info.MatchStateName = res.MatchState;
                    //info.GuestHalf_Result = res.GuestHalf_Result;
                    //info.GuestFull_Result = res.GuestFull_Result;
                    //info.BQC_SP = res.BQC_SP;
                    //info.BQC_Result = res.BQC_Result;
                    //info.BF_SP = res.BF_SP;
                    //info.BF_Result = res.BF_Result;
                    //info.HomeFull_Result = res.HomeFull_Result;
                    //info.HomeHalf_Result = res.HomeHalf_Result;
                    //info.LotteryTime = res.CreateTime.ToString("yyyyMMddHHmmss");
                }
                else if (!isLeftJoin)
                {
                    continue;
                }
#endregion

#region 附加胜平负sp数据
                var sp_spf_item = sp_spf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_spf_item != null)
                {
                    info.SP_Win_Odds = sp_spf_item.Win_Odds;
                    info.SP_Lose_Odds = sp_spf_item.Lose_Odds;
                    info.SP_Flat_Odds = sp_spf_item.Flat_Odds;
                }
#endregion

#region 附加总进球sp数据
                var sp_zjq_item = sp_zjq.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_zjq_item != null)
                {
                    info.JinQiu_0_Odds = sp_zjq_item.JinQiu_0_Odds;
                    info.JinQiu_1_Odds = sp_zjq_item.JinQiu_1_Odds;
                    info.JinQiu_2_Odds = sp_zjq_item.JinQiu_2_Odds;
                    info.JinQiu_3_Odds = sp_zjq_item.JinQiu_3_Odds;
                    info.JinQiu_4_Odds = sp_zjq_item.JinQiu_4_Odds;
                    info.JinQiu_5_Odds = sp_zjq_item.JinQiu_5_Odds;
                    info.JinQiu_6_Odds = sp_zjq_item.JinQiu_6_Odds;
                    info.JinQiu_7_Odds = sp_zjq_item.JinQiu_7_Odds;
                }
#endregion

#region 附加上下单双sp数据
                var sp_sxds_item = sp_sxds.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_sxds_item != null)
                {
                    info.SH_D_Odds = sp_sxds_item.SH_D_Odds;
                    info.SH_S_Odds = sp_sxds_item.SH_S_Odds;
                    info.X_D_Odds = sp_sxds_item.X_D_Odds;
                    info.X_S_Odds = sp_sxds_item.X_S_Odds;
                }
#endregion

#region 附加比分sp数据
                var sp_bf_item = sp_bf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_bf_item != null)
                {
                    info.F_01 = sp_bf_item.F_01;
                    info.F_02 = sp_bf_item.F_02;
                    info.F_03 = sp_bf_item.F_03;
                    info.F_04 = sp_bf_item.F_04;
                    info.F_12 = sp_bf_item.F_12;
                    info.F_13 = sp_bf_item.F_13;
                    info.F_14 = sp_bf_item.F_14;
                    info.F_23 = sp_bf_item.F_23;
                    info.F_24 = sp_bf_item.F_24;
                    info.F_QT = sp_bf_item.F_QT;
                    info.P_00 = sp_bf_item.P_00;
                    info.P_11 = sp_bf_item.P_11;
                    info.P_22 = sp_bf_item.P_22;
                    info.P_33 = sp_bf_item.P_33;
                    info.P_QT = sp_bf_item.P_QT;
                    info.S_10 = sp_bf_item.S_10;
                    info.S_20 = sp_bf_item.S_20;
                    info.S_21 = sp_bf_item.S_21;
                    info.S_30 = sp_bf_item.S_30;
                    info.S_31 = sp_bf_item.S_31;
                    info.S_32 = sp_bf_item.S_32;
                    info.S_40 = sp_bf_item.S_40;
                    info.S_41 = sp_bf_item.S_41;
                    info.S_42 = sp_bf_item.S_42;
                    info.S_QT = sp_bf_item.S_QT;
                }
#endregion

#region 附加半全场sp数据
                var sp_bqc_item = sp_bqc.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_bqc_item != null)
                {
                    info.F_F_Odds = sp_bqc_item.F_F_Odds;
                    info.F_P_Odds = sp_bqc_item.F_P_Odds;
                    info.F_SH_Odds = sp_bqc_item.F_SH_Odds;
                    info.P_F_Odds = sp_bqc_item.P_F_Odds;
                    info.P_P_Odds = sp_bqc_item.P_P_Odds;
                    info.P_SH_Odds = sp_bqc_item.P_SH_Odds;
                    info.SH_F_Odds = sp_bqc_item.SH_F_Odds;
                    info.SH_P_Odds = sp_bqc_item.SH_P_Odds;
                    info.SH_SH_Odds = sp_bqc_item.SH_SH_Odds;
                }
#endregion

                list.Add(info);
            }
            return list;
        }

        ///// <summary>
        ///// 北京单场-胜平负
        ///// </summary>
        ///// <returns></returns>
        //public static string BJDC_SPF_JSON(string issuse, string gameType)
        //{
        //    BettingHelper bizHelper = new BettingHelper();
        //    var match = bizHelper.GetMatchInfoList<BJDC_MatchInfo>(MatchFile(issuse)).Where(p => p.MatchState == BJDCMatchState.Sales).ToList();
        //    var matchresult = bizHelper.GetMatchInfoList<BJDC_MatchResultInfo>(MatchResultFile(issuse));
        //    var sp_spf = bizHelper.GetMatchInfoList<BJDC_SPF_SpInfo>(SPFile(gameType, issuse)); //胜平负sp数据
        //    var sp_zjq = bizHelper.GetMatchInfoList<BJDC_ZJQ_SpInfo>(SPFile(gameType, issuse)); //总进球sp数据
        //    var sp_sxds = bizHelper.GetMatchInfoList<BJDC_SXDS_SpInfo>(SPFile(gameType, issuse)); //上下单双sp数据
        //    var sp_bf = bizHelper.GetMatchInfoList<BJDC_BF_SpInfo>(SPFile(gameType, issuse)); //比分sp数据
        //    var sp_bqc = bizHelper.GetMatchInfoList<BJDC_BQC_SpInfo>(SPFile(gameType, issuse)); //半全场sp数据
        //    match = match.Where(s => long.Parse(Convert.ToDateTime(s.LocalStopTime).ToString("yyyyMMddHHmmss")) > long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))).ToList();
        //    StringBuilder json = new StringBuilder("{\"content\":[");
        //    foreach (var item in match)
        //    {
        //        json.Append("[");
        //        json.AppendFormat("\"{0}\",", item.IssuseNumber);
        //        json.AppendFormat("\"{0}\",", item.MatchOrderId);
        //        json.AppendFormat("\"{0}\",", item.MatchName.Replace("&nbsp;", ""));
        //        json.AppendFormat("\"{0}\",", item.HomeTeamName.Replace("&nbsp;", ""));
        //        json.AppendFormat("\"{0}\",", item.GuestTeamName.Replace("&nbsp;", ""));
        //        json.AppendFormat("\"{0}\",", item.LetBall);
        //        json.AppendFormat("\"{0}\",", item.MatchStartTime);
        //        json.AppendFormat("\"{0}\",", item.LocalStopTime);
        //        json.Append("\"-\",");
        //        json.Append("\"-\",");
        //        json.Append("\"-\",");
        //        json.Append("\"-\",");
        //        json.Append("\"-\",");
        //        json.Append("\"-\",");
        //        //json.Append("\"0,0,0\",");
        //        switch (gameType)
        //        {
        //            case "SPF":
        //                #region 附加胜平负sp数据
        //                var sp_spf_item = sp_spf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
        //                if (sp_spf_item != null)
        //                {
        //                    json.AppendFormat("\"{0},{1},{2}\",", sp_spf_item.Win_Odds, sp_spf_item.Flat_Odds, sp_spf_item.Lose_Odds);
        //                }
        //                else
        //                {
        //                    json.Append("\"0,0,0\",");
        //                }
        //                #endregion
        //                break;
        //            case "ZJQ"://总进球
        //                #region 附加总进球sp数据
        //                var sp_zjq_item = sp_zjq.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
        //                if (sp_zjq_item != null)
        //                {
        //                    json.AppendFormat("\"{0},{1},{2},{3},{4},{5},{6},{7}\",", sp_zjq_item.JinQiu_0_Odds, sp_zjq_item.JinQiu_1_Odds, sp_zjq_item.JinQiu_2_Odds, sp_zjq_item.JinQiu_3_Odds, sp_zjq_item.JinQiu_4_Odds, sp_zjq_item.JinQiu_5_Odds, sp_zjq_item.JinQiu_6_Odds, sp_zjq_item.JinQiu_7_Odds);
        //                }
        //                else
        //                {
        //                    json.Append("\"0,0,0,0,0,0,0,0\",");
        //                }
        //                #endregion
        //                break;
        //            case "BF"://比分
        //                #region 附加比分sp数据
        //                var sp_bf_item = sp_bf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
        //                if (sp_bf_item != null)
        //                {
        //                    json.AppendFormat("\"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24}\",", sp_bf_item.S_10, sp_bf_item.S_20, sp_bf_item.S_21, sp_bf_item.S_30, sp_bf_item.S_31, sp_bf_item.S_32, sp_bf_item.S_40, sp_bf_item.S_41, sp_bf_item.S_42, sp_bf_item.S_QT, sp_bf_item.P_00, sp_bf_item.P_11, sp_bf_item.P_22, sp_bf_item.P_33, sp_bf_item.P_QT, sp_bf_item.F_01, sp_bf_item.F_02, sp_bf_item.F_12, sp_bf_item.F_03, sp_bf_item.F_13, sp_bf_item.F_23, sp_bf_item.F_04, sp_bf_item.F_14, sp_bf_item.F_24, sp_bf_item.F_QT);
        //                }
        //                else
        //                {
        //                    json.Append("\"0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0\"");
        //                }
        //                #endregion
        //                break;
        //            case "BQC"://半全场
        //                #region 附加半全场sp数据
        //                var sp_bqc_item = sp_bqc.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
        //                if (sp_bqc_item != null)
        //                {
        //                    json.AppendFormat("\"{0},{1},{2},{3},{4},{5},{6},{7},{8}\",", sp_bqc_item.SH_SH_Odds, sp_bqc_item.SH_P_Odds, sp_bqc_item.SH_F_Odds, sp_bqc_item.P_SH_Odds, sp_bqc_item.P_P_Odds, sp_bqc_item.P_F_Odds, sp_bqc_item.F_SH_Odds, sp_bqc_item.F_P_Odds, sp_bqc_item.F_F_Odds);
        //                }
        //                else
        //                {
        //                    json.Append("\"0,0,0,0,0,0,0,0,0\",");
        //                }
        //                #endregion
        //                break;
        //            case "SXDS"://上下单双
        //                #region 附加上下单双sp数据
        //                var sp_sxds_item = sp_sxds.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
        //                if (sp_sxds_item != null)
        //                {
        //                    json.AppendFormat("\"{0},{1},{2},{3}\",",sp_sxds_item.SH_D_Odds,sp_sxds_item.SH_S_Odds,sp_sxds_item.X_D_Odds,sp_sxds_item.X_S_Odds);
        //                }
        //                else
        //                {
        //                    json.Append("\"0,0,0,0\",");
        //                }
        //                #endregion
        //                break;
        //        }

        //        json.AppendFormat("\"{0}\",", item.MatchColor);//BettingHelper.GetLeagueColor()
        //        json.AppendFormat("\"{0}\",", item.MatchStartTime);
        //        json.Append("\"\",");
        //        json.Append("\"0\",");
        //        json.Append("\"http://live.159cai.com/odds/match/927028?lotyid=5&from=iosapp#1\"");
        //        json.Append("],");
        //    }
        //    string data = json.ToString().TrimEnd(',');
        //    return string.Format("{0}],\"total\":{1}}}", data, match.Count);
        //}


    }
}