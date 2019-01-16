using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using EntityModel.LotteryJsonInfo;

namespace KaSon.FrameWork.ORM.Helper
{
    public class Json_JCZQ
    {
        #region 文件路径

        /// <summary>
        /// 竞彩足球队伍信息文件地址
        /// </summary>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string newVerType = null, string matchDate = null)
        {
            if (!string.IsNullOrEmpty(newVerType) && newVerType == "1")
            {
                return "/MatchData/" + "jczq/Match_List_FB.json";
            }
            else if (string.IsNullOrEmpty(matchDate))
            {
                return "/MatchData/" + "jczq/Match_List.json";
            }
            else
            {
                return "/MatchData/" + "/jczq/" + matchDate + "/Match_List.json";
            }
        }

        /// <summary>
        /// 竞彩足球 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return "/MatchData/" + "jczq/Match_Result_List.json";
            }
            else
            {
                return "/MatchData/" + "/jczq/" + matchDate + "/Match_Result_List.json";
            }
        }

        /// <summary>
        /// 竞彩足球 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchdate">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string matchdate = null)
        {
            if (type.ToLower() == "hh")
            {
                return "/MatchData/" + "jczq/SP.json";
            }
            else if (string.IsNullOrEmpty(matchdate))
            {
                return "/MatchData/" + "jczq/" + type + "_SP.json";
            }
            else
            {
                return "/MatchData/" + "/jczq/" + matchdate + "/" + type + "_SP.json";
            }
        }
        /// <summary>
        /// 根据文件名获取文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFilePath(string fileName)
        {
            return "/MatchData/jczq/" + fileName + ".json";
        }

        #endregion

        #region 竞彩足球数据读取

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<JCZQ_MatchInfo_WEB> MatchList_WEB(string gameType, string newVerType, string matchDate = null, bool isLeftJoin = true)
        {
            BusinessHelper bizHelper = new BusinessHelper();
            var match = bizHelper.GetMatchInfoList<JCZQ_MatchInfo>(MatchFile(newVerType, matchDate));

            var matchresult = bizHelper.GetMatchInfoList<JCZQ_MatchResultInfo>(MatchResultFile(matchDate));
            //var sp_spf = bizHelper.GetMatchInfoList<JCZQ_SPF_SPInfo>(SPFile("SPF",matchDate)); //让球胜平负sp数据
            //var sp_brqspf = bizHelper.GetMatchInfoList<JCZQ_SPF_SPInfo>(SPFile("BRQSPF", matchDate)); //胜平负sp数据
            //var sp_zjq = bizHelper.GetMatchInfoList<JCZQ_ZJQ_SPInfo>(SPFile("ZJQ", matchDate)); //总进球sp数据
            //var sp_bf = bizHelper.GetMatchInfoList<JCZQ_BF_SPInfo>(SPFile("BF", matchDate)); //比分sp数据
            //var sp_bqc = bizHelper.GetMatchInfoList<JCZQ_BQC_SPInfo>(SPFile("BQC", matchDate)); //半全场sp数据

            var sp_spf = bizHelper.GetMatchInfoList<JCZQ_SPF_SPInfo>(SPFile(gameType, matchDate)); //让球胜平负sp数据
            var sp_brqspf = bizHelper.GetMatchInfoList<JCZQ_SPF_SPInfo>(SPFile(gameType, matchDate)); //胜平负sp数据
            var sp_zjq = bizHelper.GetMatchInfoList<JCZQ_ZJQ_SPInfo>(SPFile(gameType, matchDate)); //总进球sp数据
            var sp_bf = bizHelper.GetMatchInfoList<JCZQ_BF_SPInfo>(SPFile(gameType, matchDate)); //比分sp数据
            var sp_bqc = bizHelper.GetMatchInfoList<JCZQ_BQC_SPInfo>(SPFile(gameType, matchDate)); //半全场sp数据

            var list = new List<JCZQ_MatchInfo_WEB>();
            match = match.Where(t => long.Parse(Convert.ToDateTime(t.FSStopBettingTime).ToString("yyyyMMddHHmmss")) > long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) && t.MatchStopDesc != "2").ToList();
            foreach (var item in match)
            {
                #region 队伍基础信息
                //var startTime=Convert.ToDateTime(item.StartDateTime);
                //if (startTime.Date == DateTime.Now.AddDays(1).Date)
                //    startTime = startTime.AddHours(-9);

                var matchDataTime = bizHelper.ConvertDateTimeInt(bizHelper.ConvertStrToDateTime("20" + item.MatchData));
                //var matchDataTime = bizHelper.ConvertDateTimeInt(bizHelper.ConvertStrToDateTime("20150623"));
                var info = new JCZQ_MatchInfo_WEB()
                {
                    //CreateTime = item.CreateTime.ToString("yyyyMMddHHmmss"),
                    //DSStopBettingTime = item.DSStopBettingTime.ToString("yyyyMMddHHmmss"),
                    MatcheDateTime = matchDataTime,
                    //FSStopBettingTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.FSStopBettingTime)).ToString(),
                    FSStopBettingTime = item.FSStopBettingTime,
                    //FSStopBettingTime = item.FSStopBettingTime.ToString("yyyyMMddHHmmss"),
                    //GuestTeamId = item.GuestTeamId,
                    GuestTeamName = item.GuestTeamName,
                    //HomeTeamId = item.HomeTeamId,
                    HomeTeamName = item.HomeTeamName,
                    LeagueColor = item.LeagueColor,
                    //LeagueId = item.LeagueId,
                    LeagueName = item.LeagueName,
                    LetBall = item.LetBall,
                    //LoseOdds = item.LoseOdds,
                    MatchIdName = item.MatchIdName,
                    StartDateTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.StartDateTime)).ToString(),
                    //StartDateTime = item.StartDateTime.ToString("yyyyMMddHHmmss"),
                    //WinOdds = item.WinOdds,
                    //FlatOdds = item.FlatOdds,
                    MatchData = item.MatchData,
                    MatchId = item.MatchId,
                    MatchNumber = item.MatchNumber,
                    //Mid = item.Mid,
                    FXId = item.FXId,
                    State = item.State,
                    PrivilegesType = item.PrivilegesType == null ? string.Empty : item.PrivilegesType,
                };
                #endregion

                #region 附加队伍结果信息
                var res = matchresult.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (res != null)
                {
                    //info.ZJQ_Result = res.ZJQ_Result;
                    //info.ZJQ_SP = res.ZJQ_SP;
                    //info.SPF_SP = res.SPF_SP;
                    //info.SPF_Result = res.SPF_Result;
                    //info.BQC_SP = res.BQC_SP;
                    //info.BQC_Result = res.BQC_Result;
                    //info.BF_SP = res.BF_SP;
                    //info.BF_Result = res.BF_Result;
                    //info.FullGuestTeamScore = res.FullGuestTeamScore;
                    //info.FullHomeTeamScore = res.FullHomeTeamScore;
                    //info.HalfGuestTeamScore = res.HalfGuestTeamScore;
                    //info.HalfHomeTeamScore = res.HalfHomeTeamScore;
                    //info.MatchState = res.MatchState;
                }
                else if (!isLeftJoin)
                {
                    continue;
                }
                #endregion

                #region 附加让球胜平负sp数据
                var sp_spf_item = sp_spf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_spf_item != null && sp_spf_item.SPF != null)
                    {
                        var json = JsonHelper.Deserialize<JCZQ_SPF_SPInfo>(sp_spf_item.SPF);
                        info.SP_Win_Odds = json.WinOdds;
                        info.SP_Lose_Odds = json.LoseOdds;
                        info.SP_Flat_Odds = json.FlatOdds;
                        //info.PrivilegesType = json.PrivilegesType==null?string.Empty:json.PrivilegesType;
                    }
                }
                else if (sp_spf_item != null)
                {
                    info.SP_Win_Odds = sp_spf_item.WinOdds;
                    info.SP_Lose_Odds = sp_spf_item.LoseOdds;
                    info.SP_Flat_Odds = sp_spf_item.FlatOdds;
                    //info.PrivilegesType = sp_spf_item.PrivilegesType;
                }
                #endregion

                #region 附加胜平负sp数据
                var sp_brqspf_item = sp_brqspf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_brqspf_item != null && sp_brqspf_item.BRQSPF != null)
                    {
                        var json = JsonHelper.Deserialize<JCZQ_SPF_SPInfo>(sp_brqspf_item.BRQSPF);
                        info.SP_Win_Odds_BRQ = json.WinOdds;
                        info.SP_Lose_Odds_BRQ = json.LoseOdds;
                        info.SP_Flat_Odds_BRQ = json.FlatOdds;
                        //info.PrivilegesType = json.PrivilegesType==null?string.Empty:json.PrivilegesType;
                    }
                }
                else if (sp_brqspf_item != null)
                {
                    info.SP_Win_Odds_BRQ = sp_brqspf_item.WinOdds;
                    info.SP_Lose_Odds_BRQ = sp_brqspf_item.LoseOdds;
                    info.SP_Flat_Odds_BRQ = sp_brqspf_item.FlatOdds;
                    //info.PrivilegesType = sp_brqspf_item.PrivilegesType==null?string.Empty:sp_brqspf_item.PrivilegesType;
                }
                #endregion

                #region 附加总进球sp数据
                var sp_zjq_item = sp_zjq.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_zjq_item != null && sp_zjq_item.ZJQ != null)
                    {
                        var json = JsonHelper.Deserialize<JCZQ_ZJQ_SPInfo>(sp_zjq_item.ZJQ);
                        info.JinQiu_0_Odds = json.JinQiu_0_Odds;
                        info.JinQiu_1_Odds = json.JinQiu_1_Odds;
                        info.JinQiu_2_Odds = json.JinQiu_2_Odds;
                        info.JinQiu_3_Odds = json.JinQiu_3_Odds;
                        info.JinQiu_4_Odds = json.JinQiu_4_Odds;
                        info.JinQiu_5_Odds = json.JinQiu_5_Odds;
                        info.JinQiu_6_Odds = json.JinQiu_6_Odds;
                        info.JinQiu_7_Odds = json.JinQiu_7_Odds;
                        //info.PrivilegesType = json.PrivilegesType==null?string.Empty:json.PrivilegesType;
                    }
                }
                else if (sp_zjq_item != null)
                {
                    info.JinQiu_0_Odds = sp_zjq_item.JinQiu_0_Odds;
                    info.JinQiu_1_Odds = sp_zjq_item.JinQiu_1_Odds;
                    info.JinQiu_2_Odds = sp_zjq_item.JinQiu_2_Odds;
                    info.JinQiu_3_Odds = sp_zjq_item.JinQiu_3_Odds;
                    info.JinQiu_4_Odds = sp_zjq_item.JinQiu_4_Odds;
                    info.JinQiu_5_Odds = sp_zjq_item.JinQiu_5_Odds;
                    info.JinQiu_6_Odds = sp_zjq_item.JinQiu_6_Odds;
                    info.JinQiu_7_Odds = sp_zjq_item.JinQiu_7_Odds;
                    //info.PrivilegesType = sp_zjq_item.PrivilegesType==null?string.Empty:sp_zjq_item.PrivilegesType;
                }
                #endregion

                #region 附加比分sp数据
                var sp_bf_item = sp_bf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_bf_item != null && sp_bf_item.BF != null)
                    {
                        var json = JsonHelper.Deserialize<JCZQ_BF_SPInfo>(sp_bf_item.BF);
                        info.F_01 = json.F_01;
                        info.F_02 = json.F_02;
                        info.F_03 = json.F_03;
                        info.F_04 = json.F_04;
                        info.F_05 = json.F_05;
                        info.F_12 = json.F_12;
                        info.F_13 = json.F_13;
                        info.F_14 = json.F_14;
                        info.F_15 = json.F_15;
                        info.F_23 = json.F_23;
                        info.F_24 = json.F_24;
                        info.F_25 = json.F_25;
                        info.F_QT = json.F_QT;
                        info.P_00 = json.P_00;
                        info.P_11 = json.P_11;
                        info.P_22 = json.P_22;
                        info.P_33 = json.P_33;
                        info.P_QT = json.P_QT;
                        info.S_10 = json.S_10;
                        info.S_20 = json.S_20;
                        info.S_21 = json.S_21;
                        info.S_30 = json.S_30;
                        info.S_31 = json.S_31;
                        info.S_32 = json.S_32;
                        info.S_40 = json.S_40;
                        info.S_41 = json.S_41;
                        info.S_42 = json.S_42;
                        info.S_50 = json.S_50;
                        info.S_51 = json.S_51;
                        info.S_52 = json.S_52;
                        info.S_QT = json.S_QT;
                        //info.PrivilegesType = json.PrivilegesType==null?string.Empty:json.PrivilegesType;
                    }
                }
                else if (sp_bf_item != null)
                {
                    info.F_01 = sp_bf_item.F_01;
                    info.F_02 = sp_bf_item.F_02;
                    info.F_03 = sp_bf_item.F_03;
                    info.F_04 = sp_bf_item.F_04;
                    info.F_05 = sp_bf_item.F_05;
                    info.F_12 = sp_bf_item.F_12;
                    info.F_13 = sp_bf_item.F_13;
                    info.F_14 = sp_bf_item.F_14;
                    info.F_15 = sp_bf_item.F_15;
                    info.F_23 = sp_bf_item.F_23;
                    info.F_24 = sp_bf_item.F_24;
                    info.F_25 = sp_bf_item.F_25;
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
                    info.S_50 = sp_bf_item.S_50;
                    info.S_51 = sp_bf_item.S_51;
                    info.S_52 = sp_bf_item.S_52;
                    info.S_QT = sp_bf_item.S_QT;
                    //info.PrivilegesType = sp_bf_item.PrivilegesType==null?string.Empty:sp_bf_item.PrivilegesType;
                }
                #endregion

                #region 附加半全场sp数据
                var sp_bqc_item = sp_bqc.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_bqc_item != null && sp_bqc_item.BQC != null)
                    {
                        var json = JsonHelper.Deserialize<JCZQ_BQC_SPInfo>(sp_bqc_item.BQC);
                        info.F_F_Odds = json.F_F_Odds;
                        info.F_P_Odds = json.F_P_Odds;
                        info.F_SH_Odds = json.F_SH_Odds;
                        info.P_F_Odds = json.P_F_Odds;
                        info.P_P_Odds = json.P_P_Odds;
                        info.P_SH_Odds = json.P_SH_Odds;
                        info.SH_F_Odds = json.SH_F_Odds;
                        info.SH_P_Odds = json.SH_P_Odds;
                        info.SH_SH_Odds = json.SH_SH_Odds;
                        //info.PrivilegesType = json.PrivilegesType==null?string.Empty:json.PrivilegesType;
                    }
                }
                else if (sp_bqc_item != null)
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
                    //info.PrivilegesType = sp_bqc_item.PrivilegesType==null?string.Empty:sp_bqc_item.PrivilegesType;
                }
                #endregion

                list.Add(info);
            }

            return list;
        }

        #endregion

        #region 竞彩足球混合单关数据读取

        public static List<JCZQ_MatchInfo_WEB> GetJCZQHHDGList()
        {
            List<JCZQ_MatchInfo_WEB> jczqMatchList = new List<JCZQ_MatchInfo_WEB>();
            BusinessHelper bizHelper = new BusinessHelper();
            var matchList = bizHelper.GetMatchInfoList<JCZQHHDGBase>(GetFilePath("Match_List_HH"));
            matchList = matchList.Where(s => long.Parse(Convert.ToDateTime(s.FSStopBettingTime).ToString("yyyyMMddHHmmss")) > long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) && s.MatchStopDesc != "2").ToList();
            if (matchList != null && matchList.Count > 0)
            {
                foreach (var item in matchList)
                {
                    JCZQ_MatchInfo_WEB info = new JCZQ_MatchInfo_WEB();
                    info.MatcheDateTime = bizHelper.ConvertDateTimeInt(bizHelper.ConvertStrToDateTime("20" + item.MatchData));
                    //info.FSStopBettingTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.FSStopBettingTime)).ToString();
                    info.FSStopBettingTime = item.FSStopBettingTime;
                    //info.//FSStopBettingTime = item.FSStopBettingTime.ToString("yyyyMMddHHmmss"),
                    //info.//GuestTeamId = item.GuestTeamId,
                    info.GuestTeamName = item.GuestTeamName;
                    //info.//HomeTeamId = item.HomeTeamId,
                    info.HomeTeamName = item.HomeTeamName;
                    info.LeagueColor = item.LeagueColor;
                    //info.//LeagueId = item.LeagueId,
                    info.LeagueName = item.LeagueName;
                    info.LetBall = item.LetBall;
                    //info.//LoseOdds = item.LoseOdds,
                    info.MatchIdName = item.MatchIdName;
                    //info.StartDateTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.StartDateTime)).ToString();
                    info.StartDateTime = item.StartDateTime;
                    //info.//StartDateTime = item.StartDateTime.ToString("yyyyMMddHHmmss"),
                    //info.//WinOdds = item.WinOdds,
                    //info.//FlatOdds = item.FlatOdds,
                    info.MatchData = item.MatchData;
                    info.MatchId = item.MatchId;
                    info.MatchNumber = item.MatchNumber;
                    //info.//Mid = item.Mid,
                    info.FXId = item.FXId;
                    info.State_HHDG = item.State;
                    info.PrivilegesType = item.PrivilegesType == null ? string.Empty : item.PrivilegesType;

                    //info.State = item.State;
                    #region 附加让球胜平负sp数据
                    if (item.SPF != null)
                    {
                        var json_spf = JsonHelper.Deserialize<JCZQ_SPF_SPInfo>(item.SPF);
                        info.SP_Win_Odds = json_spf.WinOdds;
                        info.SP_Lose_Odds = json_spf.LoseOdds;
                        info.SP_Flat_Odds = json_spf.FlatOdds;
                        info.NoSaleState_SPF = json_spf.NoSaleState;

                        //info.PrivilegesType = json_spf.PrivilegesType==null?string.Empty:json_spf.PrivilegesType;
                    }
                    else
                    {
                        info.SP_Win_Odds = 0;
                        info.SP_Lose_Odds = 0;
                        info.SP_Flat_Odds = 0;
                        info.NoSaleState_SPF = "0";
                        //info.PrivilegesType = string.Empty;
                    }

                    #endregion

                    #region 附加胜平负sp数据

                    if (item.BRQSPF != null)
                    {
                        var json_brqspf = JsonHelper.Deserialize<JCZQ_SPF_SPInfo>(item.BRQSPF);
                        info.SP_Win_Odds_BRQ = json_brqspf.WinOdds;
                        info.SP_Lose_Odds_BRQ = json_brqspf.LoseOdds;
                        info.SP_Flat_Odds_BRQ = json_brqspf.FlatOdds;
                        info.NoSaleState_BRQSPF = json_brqspf.NoSaleState;
                        //info.PrivilegesType = json_brqspf.PrivilegesType==null?string.Empty:json_brqspf.PrivilegesType;
                    }
                    else
                    {
                        info.SP_Win_Odds_BRQ = 0;
                        info.SP_Lose_Odds_BRQ = 0;
                        info.SP_Flat_Odds_BRQ = 0;
                        info.NoSaleState_BRQSPF = "0";
                        //info.PrivilegesType = string.Empty;
                    }

                    #endregion

                    #region 附加总进球sp数据

                    if (item.ZJQ != null)
                    {
                        var json_zjq = JsonHelper.Deserialize<JCZQ_ZJQ_SPInfo>(item.ZJQ);
                        info.JinQiu_0_Odds = json_zjq.JinQiu_0_Odds;
                        info.JinQiu_1_Odds = json_zjq.JinQiu_1_Odds;
                        info.JinQiu_2_Odds = json_zjq.JinQiu_2_Odds;
                        info.JinQiu_3_Odds = json_zjq.JinQiu_3_Odds;
                        info.JinQiu_4_Odds = json_zjq.JinQiu_4_Odds;
                        info.JinQiu_5_Odds = json_zjq.JinQiu_5_Odds;
                        info.JinQiu_6_Odds = json_zjq.JinQiu_6_Odds;
                        info.JinQiu_7_Odds = json_zjq.JinQiu_7_Odds;
                        info.NoSaleState_ZJQ = json_zjq.NoSaleState;
                        //info.PrivilegesType = json_zjq.PrivilegesType==null?string.Empty:json_zjq.PrivilegesType;
                    }
                    else
                    {
                        info.JinQiu_0_Odds = 0;
                        info.JinQiu_1_Odds = 0;
                        info.JinQiu_2_Odds = 0;
                        info.JinQiu_3_Odds = 0;
                        info.JinQiu_4_Odds = 0;
                        info.JinQiu_5_Odds = 0;
                        info.JinQiu_6_Odds = 0;
                        info.JinQiu_7_Odds = 0;
                        info.NoSaleState_ZJQ = "0";
                        //info.PrivilegesType = string.Empty;
                    }

                    #endregion

                    #region 附加比分sp数据

                    if (item.BF != null)
                    {
                        var json_bf = JsonHelper.Deserialize<JCZQ_BF_SPInfo>(item.BF);
                        info.F_01 = json_bf.F_01;
                        info.F_02 = json_bf.F_02;
                        info.F_03 = json_bf.F_03;
                        info.F_04 = json_bf.F_04;
                        info.F_05 = json_bf.F_05;
                        info.F_12 = json_bf.F_12;
                        info.F_13 = json_bf.F_13;
                        info.F_14 = json_bf.F_14;
                        info.F_15 = json_bf.F_15;
                        info.F_23 = json_bf.F_23;
                        info.F_24 = json_bf.F_24;
                        info.F_25 = json_bf.F_25;
                        info.F_QT = json_bf.F_QT;
                        info.P_00 = json_bf.P_00;
                        info.P_11 = json_bf.P_11;
                        info.P_22 = json_bf.P_22;
                        info.P_33 = json_bf.P_33;
                        info.P_QT = json_bf.P_QT;
                        info.S_10 = json_bf.S_10;
                        info.S_20 = json_bf.S_20;
                        info.S_21 = json_bf.S_21;
                        info.S_30 = json_bf.S_30;
                        info.S_31 = json_bf.S_31;
                        info.S_32 = json_bf.S_32;
                        info.S_40 = json_bf.S_40;
                        info.S_41 = json_bf.S_41;
                        info.S_42 = json_bf.S_42;
                        info.S_50 = json_bf.S_50;
                        info.S_51 = json_bf.S_51;
                        info.S_52 = json_bf.S_52;
                        info.S_QT = json_bf.S_QT;
                        info.NoSaleState_BF = json_bf.NoSaleState;
                        //info.PrivilegesType = json_bf.PrivilegesType==null?string.Empty:json_bf.PrivilegesType;
                    }
                    else
                    {
                        info.F_01 = 0;
                        info.F_02 = 0;
                        info.F_03 = 0;
                        info.F_04 = 0;
                        info.F_05 = 0;
                        info.F_12 = 0;
                        info.F_13 = 0;
                        info.F_14 = 0;
                        info.F_15 = 0;
                        info.F_23 = 0;
                        info.F_24 = 0;
                        info.F_25 = 0;
                        info.F_QT = 0;
                        info.P_00 = 0;
                        info.P_11 = 0;
                        info.P_22 = 0;
                        info.P_33 = 0;
                        info.P_QT = 0;
                        info.S_10 = 0;
                        info.S_20 = 0;
                        info.S_21 = 0;
                        info.S_30 = 0;
                        info.S_31 = 0;
                        info.S_32 = 0;
                        info.S_40 = 0;
                        info.S_41 = 0;
                        info.S_42 = 0;
                        info.S_50 = 0;
                        info.S_51 = 0;
                        info.S_52 = 0;
                        info.S_QT = 0;
                        info.NoSaleState_BF = "0";
                        //info.PrivilegesType = string.Empty;
                    }

                    #endregion

                    #region 附加半全场sp数据

                    if (item.BQC != null)
                    {
                        var json_bqc = JsonHelper.Deserialize<JCZQ_BQC_SPInfo>(item.BQC);
                        info.F_F_Odds = json_bqc.F_F_Odds;
                        info.F_P_Odds = json_bqc.F_P_Odds;
                        info.F_SH_Odds = json_bqc.F_SH_Odds;
                        info.P_F_Odds = json_bqc.P_F_Odds;
                        info.P_P_Odds = json_bqc.P_P_Odds;
                        info.P_SH_Odds = json_bqc.P_SH_Odds;
                        info.SH_F_Odds = json_bqc.SH_F_Odds;
                        info.SH_P_Odds = json_bqc.SH_P_Odds;
                        info.SH_SH_Odds = json_bqc.SH_SH_Odds;
                        info.NoSaleState_BQC = json_bqc.NoSaleState;
                        //info.PrivilegesType = json_bqc.PrivilegesType==null?string.Empty:json_bqc.PrivilegesType;
                    }
                    else
                    {
                        info.F_F_Odds = 0;
                        info.F_P_Odds = 0;
                        info.F_SH_Odds = 0;
                        info.P_F_Odds = 0;
                        info.P_P_Odds = 0;
                        info.P_SH_Odds = 0;
                        info.SH_F_Odds = 0;
                        info.SH_P_Odds = 0;
                        info.SH_SH_Odds = 0;
                        info.NoSaleState_BQC = "0";
                        //info.PrivilegesType = string.Empty;
                    }

                    #endregion

                    jczqMatchList.Add(info);
                }

            }
            return jczqMatchList;
        }

        /*
[2016/8/5 22:36:57] ios app: /*判断是否单关字符串，五位10001，代表不让球胜平负 进球数 比分 半全场 让球是否单关，0是1否*/

        /// <summary>
        /// 转换JCZQjson
        /// </summary>
        /// <returns></returns>
        public static string GetJson()
        {
            var jczq = GetJCZQHHDGList();
            StringBuilder json = new StringBuilder("[");
            foreach (var item in jczq)
            {
                json.Append("[");
                json.AppendFormat("\"{0}\",", item.MatchId);
                json.AppendFormat("\"{0}{1}\",", BusinessHelper.GetWeek(Convert.ToDateTime(item.FSStopBettingTime)), item.MatchNumber);
                json.AppendFormat("\"{0}\",", item.LeagueName);
                json.AppendFormat("\"{0}\",", item.HomeTeamName);
                json.AppendFormat("\"{0}\",", item.GuestTeamName);
                json.AppendFormat("\"{0}\",", item.LetBall);
                json.AppendFormat("\"{0}\",", item.StartDateTime);
                json.AppendFormat("\"{0}\",", item.FSStopBettingTime);
                //json.Append("\"00000\",");
                json.AppendFormat("\"{0}{1}{2}{3}{4}\",", item.NoSaleState_BRQSPF, item.NoSaleState_ZJQ, item.NoSaleState_BF, item.NoSaleState_BQC, item.NoSaleState_SPF);
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\",,\",");

                //sp start
                json.Append("\"[");
                //不让球
                json.AppendFormat("[{0},{1},{2}],", item.SP_Win_Odds_BRQ, item.SP_Flat_Odds_BRQ, item.SP_Lose_Odds_BRQ);
                //让球赔率
                json.AppendFormat("[{0},{1},{2}],", item.SP_Win_Odds, item.SP_Flat_Odds, item.SP_Lose_Odds);
                //让球数
                json.AppendFormat("[{0},{1},{2},{3},{4},{5},{6},{7}],", item.JinQiu_0_Odds, item.JinQiu_1_Odds, item.JinQiu_2_Odds, item.JinQiu_3_Odds, item.JinQiu_4_Odds, item.JinQiu_5_Odds, item.JinQiu_6_Odds, item.JinQiu_7_Odds);
                //比分,
                json.AppendFormat("[{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}],", item.S_10, item.S_20, item.S_21, item.S_30, item.S_31, item.S_32, item.S_40, item.S_41, item.S_42, item.S_50, item.S_51, item.S_52, item.S_QT, item.P_00, item.P_11, item.P_22, item.P_33, item.P_QT, item.F_01, item.F_02, item.F_12, item.F_03, item.F_13, item.F_23, item.F_04, item.F_14, item.F_24, item.F_05, item.F_15, item.F_25, item.F_QT);
                //半全场
                json.AppendFormat("[{0},{1},{2},{3},{4},{5},{6},{7},{8}],", item.SH_SH_Odds, item.SH_P_Odds, item.SH_F_Odds, item.P_SH_Odds, item.P_P_Odds, item.P_F_Odds, item.F_SH_Odds, item.F_P_Odds, item.F_F_Odds);
                json.Append("[],[],[],[]]\",");
                //sp end
                json.AppendFormat("\"{0}\",", item.LeagueColor);//BusinessHelper.GetLeagueColor()
                json.AppendFormat("\"{0}\",", item.MatchData);
                //2354 354  代表不让球胜平负 进球数 比分 半全场 让球是否单关，0是1否
                if (item.State_HHDG == "354")
                {
                    json.Append("\"10001\",");
                }
                else if (item.State_HHDG == "2354")
                {
                    json.Append("\"00001\",");
                }
                else
                {
                    json.Append("\"10001\",");
                }

                json.Append("\"http://live.159cai.com/odds/match/928061?lotyid=6&from=iosapp#1\"");
                json.Append("],");
            }
            string data = json.ToString().TrimEnd(',');
            //json.Append("]");
            return data + "]";
        }
        #endregion


        #region sjb
        /// <summary>
        /// 世界杯文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string SJBFile(string type)
        {
            return "/matchdata/SJB/SJB_" + type + ".json";
        }

         /// <summary>
        /// 竞彩足球 - 世界冠军竞猜读取数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCZQ_SJBMatchInfo> SJBMatchList(string type)
        {
            var bizHelper = new BusinessHelper();
            List<JCZQ_SJBMatchInfo> matches = null;
            try
            {
                matches = bizHelper.GetMatchInfoList<JCZQ_SJBMatchInfo>(SJBFile(type));
                return matches;
            }
            catch (Exception)
            {
                return new List<JCZQ_SJBMatchInfo>();
            }
        }

        private static string GetSJB_MatchName(string gameType, string match)
        {
            var gjArray = new string[] { "巴西", "德国", "西班牙", "阿根廷", "法国", "比利时", "葡萄牙", "英格兰", "乌拉圭", "哥伦比亚", "克罗地亚", "俄罗斯", "墨西哥", "波兰", 
                                    "瑞士", "丹麦", "塞尔维亚", "瑞典", "秘鲁", "日本", "尼日利亚", "塞内加尔", "埃及", "冰岛","突尼斯","澳大利亚","摩洛哥",
                                    "韩国","伊朗","哥斯达黎加","巴拿马","沙特"};
            var gyjArray = new string[] { "巴西—德国", "巴西—西班牙", "巴西—阿根廷", "巴西—法国", "巴西—比利时", "巴西—葡萄牙", "巴西—英格兰", "巴西—乌拉圭", 
                                    "巴西—哥伦比亚", "巴西—克罗地亚", "巴西—俄罗斯", "巴西—波兰", "巴西—瑞士", "德国—西班牙", "德国—阿根廷", "德国—法国", 
                                    "德国—比利时", "德国—葡萄牙", "德国—英格兰", "德国—乌拉圭", "德国—哥伦比亚", "德国—克罗地亚", "德国—俄罗斯", "西班牙—阿根廷", 
                                    "西班牙—法国", "西班牙—比利时", "西班牙—葡萄牙", "西班牙—英格兰", "西班牙—乌拉圭", "西班牙—哥伦比亚", "西班牙—克罗地亚", 
                                    "阿根廷—法国", "阿根廷—比利时", "阿根廷—葡萄牙", "阿根廷—英格兰", "阿根廷—乌拉圭", "阿根廷—哥伦比亚", "阿根廷—克罗地亚", 
                                    "法国—比利时", "法国—葡萄牙", "法国—英格兰", "法国—乌拉圭", "法国—哥伦比亚", "法国—克罗地亚", "比利时—葡萄牙", "比利时—英格兰", 
                                    "葡萄牙—英格兰", "葡萄牙—乌拉圭", "英格兰—乌拉圭", "其它" };
            var matchId = int.Parse(match);
            switch (gameType.ToUpper())
            {
                case "GJ":
                case "冠军":
                    return gjArray[matchId - 1];
                case "GYJ":
                case "冠亚军":
                    return gyjArray[matchId - 1];
                default:
                    break;
            }
            return string.Empty;
        }


        public static string GetAnteCode(string gameType, string anteCode, string currentSp)
        {
            var codeContentArray = anteCode.Split(',');//  "AnteCode": "1,4",
            var codeSpArray = currentSp.Split('/');//"CurrentSp": "1|7.0000/4|23.0000",
            string codes = string.Empty;
            string currentsps = string.Empty;
            if (codeContentArray.Length == codeSpArray.Length)
            {
                for (int i = 0; i < codeContentArray.Length; i++)
                {
                    var sjb_code = codeContentArray[i];
                    var matchName = GetSJB_MatchName(gameType, sjb_code);
                    var sjb_odd = decimal.Parse(codeSpArray[i].Replace(sjb_code + "|", ""));
                    currentsps += string.Format("{0}|{1}/", matchName, sjb_odd);
                }
            }
            return currentsps.TrimEnd('/');
        }
        #endregion
    }
}