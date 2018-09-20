using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using app.lottery.site.cbbao.Models;
using MatchBiz.Core;
using Common.JSON;
using System.Configuration;
using System.IO;
using Common.XmlAnalyzer;

namespace app.lottery.site.jsonManager
{
    /// <summary>
    /// 北单 - json文件读取管理
    /// </summary>
    public static class Json_BJDC
    {
        #region 属性
        /// <summary>
        /// 竞彩数据存放物理路径根目录
        /// </summary>
        public static string MatchRoot
        {
            get
            {
                try
                {
                    return SettingConfigAnalyzer.GetConfigValueByKey("WebConfig", "MatchRoot") ?? "~/MatchData/";
                }
                catch
                {
                    return "~/MatchData/";
                }
            }
        }

        /// <summary>
        /// Service请求接口
        /// </summary>
        public static HttpServerUtilityBase Service
        {
            get;
            set;
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 读取物理文件路径
        /// </summary>
        /// <param name="fileName">文件物理地址</param>
        /// <returns>文件内容</returns>
        public static string ReadFileString(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }
        #endregion

        #region 文件路径
        /// <summary>
        /// 北单 - 奖期数据文件
        /// </summary>
        private static string IssuseFile
        {
            get
            {
                return "/matchdata/bjdc/Match_IssuseNumber_List.json";
            }
        }

        /// <summary>
        /// 北单 - 根据奖期获取队伍信息文件地址
        /// </summary>
        /// <param name="issuse">期数</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string issuse)
        {
            return "/matchdata/bjdc" + "/" + issuse + "/Match_List.json";
        }

        /// <summary>
        /// 北单 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="issuse">期数</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string issuse)
        {
            return "/matchdata/bjdc" + "/" + issuse + "/MatchResult_List.json";
        }

        /// <summary>
        /// 北单 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="issuse">期数</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string issuse)
        {
            return "/matchdata/bjdc" + "/" + issuse + "/SP_" + type + ".json";
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
            return Service.MapPath(MatchRoot + "bjdc" + "/" + issuse + "/" + type + "_SP_Trend_" + matchId + ".json");
        }

        #endregion

        #region 北单数据读取
        /// <summary>
        /// 北单 - 奖期数据列表
        /// </summary>
        /// <returns>奖期数据列表</returns>
        public static List<BJDC_IssuseInfo> IssuseList()
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonIssuse = bizHelper.GetMatchInfoList<BJDC_IssuseInfo>(IssuseFile);
                return jsonIssuse;
            }
            catch (Exception ex)
            {
                return new List<BJDC_IssuseInfo>();
            }
        }

        /// <summary>
        /// 北单 - 获取队伍信息列表
        /// </summary>
        /// <param name="issuse">期号</param>
        /// <returns>队伍信息列表</returns>
        public static List<BJDC_MatchInfo> MatchList(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BJDC_MatchInfo>(MatchFile(issuse));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BJDC_MatchInfo>();
            }
        }

        /// <summary>
        /// 北单 - 获取队伍比赛结果信息
        /// </summary>
        /// <param name="issuse">期号</param>
        /// <returns>队伍信息列表</returns>
        public static List<BJDC_MatchResultInfo> MatchResultList(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BJDC_MatchResultInfo>(MatchResultFile(issuse));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BJDC_MatchResultInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="oddtype"></param>
        /// <param name="type"></param>
        /// <param name="issuse">期数</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<BJDC_MatchInfo_WEB> MatchList_WEB(int oddtype, string type, string issuse, bool isLeftJoin = true)
        {
            var list = new List<BJDC_MatchInfo_WEB>();
            var match = MatchList(issuse);
            if (oddtype == 3) //  单式
            {
                foreach (var item in match)
                {
                    #region 队伍基础信息
                    var info = new BJDC_MatchInfo_WEB()
                    {
                        CreateTime = DateTime.Parse(item.CreateTime),
                        //CreateTime = item.CreateTime,
                        FlatOdds = item.FlatOdds,
                        GuestTeamName = item.GuestTeamName,
                        GuestTeamSort = item.GuestTeamSort,
                        GuestTeamId = item.GuestTeamId.ToString(),
                        HomeTeamName = item.HomeTeamName,
                        HomeTeamSort = item.HomeTeamSort,
                        HomeTeamId = item.HomeTeamId.ToString(),
                        Id = item.Id,
                        IssuseNumber = item.IssuseNumber,
                        LetBall = item.LetBall,
                        MatchState = item.MatchState,
                        MatchColor = item.MatchColor,
                        MatchId = item.MatchId,
                        MatchName = item.MatchName,
                        //LocalStopTime = item.LocalStopTime,
                        LocalStopTime = DateTime.Parse(item.LocalStopTime),
                        LoseOdds = item.LoseOdds,
                        MatchOrderId = item.MatchOrderId,
                        //MatchStartTime = item.MatchStartTime,
                        MatchStartTime = DateTime.Parse(item.MatchStartTime),
                        WinOdds = item.WinOdds,
                        Mid = item.Mid,
                        FXId = item.FXId,
                        Gi = item.Gi,
                        Hi = item.Hi,
                    };
                    list.Add(info);
                    #endregion
                }
                return list;
            }
            var matchresult = MatchResultList(issuse);

            switch (type)
            {
                case "spf":
                    var spSpf = SPList_SPF(issuse); //胜平负sp数据
                    foreach (var item in match)
                    {
                        #region 队伍基础信息
                        var info = new BJDC_MatchInfo_WEB()
                        {
                            CreateTime = DateTime.Parse(item.CreateTime),
                            //CreateTime = item.CreateTime,
                            FlatOdds = item.FlatOdds,
                            GuestTeamName = item.GuestTeamName,
                            GuestTeamSort = item.GuestTeamSort,
                            GuestTeamId = item.GuestTeamId.ToString(),
                            HomeTeamName = item.HomeTeamName,
                            HomeTeamSort = item.HomeTeamSort,
                            HomeTeamId = item.HomeTeamId.ToString(),
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            LetBall = item.LetBall,
                            MatchState = item.MatchState,
                            MatchColor = item.MatchColor,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            //LocalStopTime = item.LocalStopTime,
                            LocalStopTime = DateTime.Parse(item.LocalStopTime),
                            LoseOdds = item.LoseOdds,
                            MatchOrderId = item.MatchOrderId,
                            //MatchStartTime = item.MatchStartTime,
                            MatchStartTime = DateTime.Parse(item.MatchStartTime),
                            WinOdds = item.WinOdds,
                            Mid = item.Mid,
                            FXId = item.FXId,
                            Gi = item.Gi,
                            Hi = item.Hi,
                        };
                        #endregion

                        #region 附加队伍结果信息
                        var res = matchresult.FirstOrDefault(p => p.Id == item.Id);
                        if (res != null)
                        {
                            info.ZJQ_Result = res.ZJQ_Result;
                            info.ZJQ_SP = res.ZJQ_SP;
                            info.SXDS_SP = res.SXDS_SP;
                            info.SXDS_Result = res.SXDS_Result;
                            info.SPF_SP = res.SPF_SP;
                            info.SPF_Result = res.SPF_Result;
                            info.MatchStateName = res.MatchState;
                            info.GuestHalf_Result = res.GuestHalf_Result;
                            info.GuestFull_Result = res.GuestFull_Result;
                            info.BQC_SP = res.BQC_SP;
                            info.BQC_Result = res.BQC_Result;
                            info.BF_SP = res.BF_SP;
                            info.BF_Result = res.BF_Result;
                            info.HomeFull_Result = res.HomeFull_Result;
                            info.HomeHalf_Result = res.HomeHalf_Result;
                            //info.LotteryTime = res.CreateTime;
                            info.LotteryTime = DateTime.Parse(res.CreateTime);
                        }
                        else if (!isLeftJoin)
                        {
                            continue;
                        }
                        #endregion

                        #region 附加胜平负sp数据
                        var spSpfItem = spSpf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                        if (spSpfItem != null)
                        {
                            info.SP_Win_Odds = spSpfItem.Win_Odds;
                            info.SP_Lose_Odds = spSpfItem.Lose_Odds;
                            info.SP_Flat_Odds = spSpfItem.Flat_Odds;
                        }
                        #endregion

                        list.Add(info);
                    }
                    break;
                case "zjq":
                    var spZjq = SPList_ZJQ(issuse); //总进球sp数据
                    foreach (var item in match)
                    {
                        #region 队伍基础信息
                        var info = new BJDC_MatchInfo_WEB()
                        {
                            CreateTime = DateTime.Parse(item.CreateTime),
                            //CreateTime = item.CreateTime,
                            FlatOdds = item.FlatOdds,
                            GuestTeamName = item.GuestTeamName,
                            GuestTeamSort = item.GuestTeamSort,
                            GuestTeamId = item.GuestTeamId.ToString(),
                            HomeTeamName = item.HomeTeamName,
                            HomeTeamSort = item.HomeTeamSort,
                            HomeTeamId = item.HomeTeamId.ToString(),
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            LetBall = item.LetBall,
                            MatchState = item.MatchState,
                            MatchColor = item.MatchColor,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            //LocalStopTime = item.LocalStopTime,
                            LocalStopTime = DateTime.Parse(item.LocalStopTime),
                            LoseOdds = item.LoseOdds,
                            MatchOrderId = item.MatchOrderId,
                            //MatchStartTime = item.MatchStartTime,
                            MatchStartTime = DateTime.Parse(item.MatchStartTime),
                            WinOdds = item.WinOdds,
                            Mid = item.Mid,
                            FXId = item.FXId,
                            Gi = item.Gi,
                            Hi = item.Hi,
                        };
                        #endregion

                        #region 附加队伍结果信息
                        var res = matchresult.FirstOrDefault(p => p.Id == item.Id);
                        if (res != null)
                        {
                            info.ZJQ_Result = res.ZJQ_Result;
                            info.ZJQ_SP = res.ZJQ_SP;
                            info.SXDS_SP = res.SXDS_SP;
                            info.SXDS_Result = res.SXDS_Result;
                            info.SPF_SP = res.SPF_SP;
                            info.SPF_Result = res.SPF_Result;
                            info.MatchStateName = res.MatchState;
                            info.GuestHalf_Result = res.GuestHalf_Result;
                            info.GuestFull_Result = res.GuestFull_Result;
                            info.BQC_SP = res.BQC_SP;
                            info.BQC_Result = res.BQC_Result;
                            info.BF_SP = res.BF_SP;
                            info.BF_Result = res.BF_Result;
                            info.HomeFull_Result = res.HomeFull_Result;
                            info.HomeHalf_Result = res.HomeHalf_Result;
                            //info.LotteryTime = res.CreateTime;
                            info.LotteryTime = DateTime.Parse(res.CreateTime);
                        }
                        else if (!isLeftJoin)
                        {
                            continue;
                        }
                        #endregion

                        #region 附加总进球sp数据
                        var spZjqItem = spZjq.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                        if (spZjqItem != null)
                        {
                            info.JinQiu_0_Odds = spZjqItem.JinQiu_0_Odds;
                            info.JinQiu_1_Odds = spZjqItem.JinQiu_1_Odds;
                            info.JinQiu_2_Odds = spZjqItem.JinQiu_2_Odds;
                            info.JinQiu_3_Odds = spZjqItem.JinQiu_3_Odds;
                            info.JinQiu_4_Odds = spZjqItem.JinQiu_4_Odds;
                            info.JinQiu_5_Odds = spZjqItem.JinQiu_5_Odds;
                            info.JinQiu_6_Odds = spZjqItem.JinQiu_6_Odds;
                            info.JinQiu_7_Odds = spZjqItem.JinQiu_7_Odds;
                        }
                        #endregion

                        list.Add(info);
                    }
                    break;
                case "sxds":
                    var spSxds = SPList_SXDS(issuse); //上下单双sp数据
                    foreach (var item in match)
                    {
                        #region 队伍基础信息
                        var info = new BJDC_MatchInfo_WEB()
                        {
                            CreateTime = DateTime.Parse(item.CreateTime),
                            //CreateTime = item.CreateTime,
                            FlatOdds = item.FlatOdds,
                            GuestTeamName = item.GuestTeamName,
                            GuestTeamSort = item.GuestTeamSort,
                            GuestTeamId = item.GuestTeamId.ToString(),
                            HomeTeamName = item.HomeTeamName,
                            HomeTeamSort = item.HomeTeamSort,
                            HomeTeamId = item.HomeTeamId.ToString(),
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            LetBall = item.LetBall,
                            MatchState = item.MatchState,
                            MatchColor = item.MatchColor,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            //LocalStopTime = item.LocalStopTime,
                            LocalStopTime = DateTime.Parse(item.LocalStopTime),
                            LoseOdds = item.LoseOdds,
                            MatchOrderId = item.MatchOrderId,
                            //MatchStartTime = item.MatchStartTime,
                            MatchStartTime = DateTime.Parse(item.MatchStartTime),
                            WinOdds = item.WinOdds,
                            Mid = item.Mid,
                            FXId = item.FXId,
                            Gi = item.Gi,
                            Hi = item.Hi,
                        };
                        #endregion

                        #region 附加队伍结果信息
                        var res = matchresult.FirstOrDefault(p => p.Id == item.Id);
                        if (res != null)
                        {
                            info.ZJQ_Result = res.ZJQ_Result;
                            info.ZJQ_SP = res.ZJQ_SP;
                            info.SXDS_SP = res.SXDS_SP;
                            info.SXDS_Result = res.SXDS_Result;
                            info.SPF_SP = res.SPF_SP;
                            info.SPF_Result = res.SPF_Result;
                            info.MatchStateName = res.MatchState;
                            info.GuestHalf_Result = res.GuestHalf_Result;
                            info.GuestFull_Result = res.GuestFull_Result;
                            info.BQC_SP = res.BQC_SP;
                            info.BQC_Result = res.BQC_Result;
                            info.BF_SP = res.BF_SP;
                            info.BF_Result = res.BF_Result;
                            info.HomeFull_Result = res.HomeFull_Result;
                            info.HomeHalf_Result = res.HomeHalf_Result;
                            //info.LotteryTime = res.CreateTime;
                            info.LotteryTime = DateTime.Parse(res.CreateTime);
                        }
                        else if (!isLeftJoin)
                        {
                            continue;
                        }
                        #endregion

                        #region 附加上下单双sp数据
                        var spSxdsItem = spSxds.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                        if (spSxdsItem != null)
                        {
                            info.SH_D_Odds = spSxdsItem.SH_D_Odds;
                            info.SH_S_Odds = spSxdsItem.SH_S_Odds;
                            info.X_D_Odds = spSxdsItem.X_D_Odds;
                            info.X_S_Odds = spSxdsItem.X_S_Odds;
                        }
                        #endregion

                        list.Add(info);
                    }
                    break;
                case "bf":
                    var spBf = SPList_BF(issuse); //比分sp数据
                    foreach (var item in match)
                    {
                        #region 队伍基础信息
                        var info = new BJDC_MatchInfo_WEB()
                        {
                            CreateTime = DateTime.Parse(item.CreateTime),
                            //CreateTime = item.CreateTime,
                            FlatOdds = item.FlatOdds,
                            GuestTeamName = item.GuestTeamName,
                            GuestTeamSort = item.GuestTeamSort,
                            GuestTeamId = item.GuestTeamId.ToString(),
                            HomeTeamName = item.HomeTeamName,
                            HomeTeamSort = item.HomeTeamSort,
                            HomeTeamId = item.HomeTeamId.ToString(),
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            LetBall = item.LetBall,
                            MatchState = item.MatchState,
                            MatchColor = item.MatchColor,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            //LocalStopTime = item.LocalStopTime,
                            LocalStopTime = DateTime.Parse(item.LocalStopTime),
                            LoseOdds = item.LoseOdds,
                            MatchOrderId = item.MatchOrderId,
                            //MatchStartTime = item.MatchStartTime,
                            MatchStartTime = DateTime.Parse(item.MatchStartTime),
                            WinOdds = item.WinOdds,
                            Mid = item.Mid,
                            FXId = item.FXId,
                            Gi = item.Gi,
                            Hi = item.Hi,
                        };
                        #endregion

                        #region 附加队伍结果信息
                        var res = matchresult.FirstOrDefault(p => p.Id == item.Id);
                        if (res != null)
                        {
                            info.ZJQ_Result = res.ZJQ_Result;
                            info.ZJQ_SP = res.ZJQ_SP;
                            info.SXDS_SP = res.SXDS_SP;
                            info.SXDS_Result = res.SXDS_Result;
                            info.SPF_SP = res.SPF_SP;
                            info.SPF_Result = res.SPF_Result;
                            info.MatchStateName = res.MatchState;
                            info.GuestHalf_Result = res.GuestHalf_Result;
                            info.GuestFull_Result = res.GuestFull_Result;
                            info.BQC_SP = res.BQC_SP;
                            info.BQC_Result = res.BQC_Result;
                            info.BF_SP = res.BF_SP;
                            info.BF_Result = res.BF_Result;
                            info.HomeFull_Result = res.HomeFull_Result;
                            info.HomeHalf_Result = res.HomeHalf_Result;
                            //info.LotteryTime = res.CreateTime;
                            info.LotteryTime = DateTime.Parse(res.CreateTime);
                        }
                        else if (!isLeftJoin)
                        {
                            continue;
                        }
                        #endregion

                        #region 附加比分sp数据
                        var spBfItem = spBf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                        if (spBfItem != null)
                        {
                            info.F_01 = spBfItem.F_01;
                            info.F_02 = spBfItem.F_02;
                            info.F_03 = spBfItem.F_03;
                            info.F_04 = spBfItem.F_04;
                            info.F_12 = spBfItem.F_12;
                            info.F_13 = spBfItem.F_13;
                            info.F_14 = spBfItem.F_14;
                            info.F_23 = spBfItem.F_23;
                            info.F_24 = spBfItem.F_24;
                            info.F_QT = spBfItem.F_QT;
                            info.P_00 = spBfItem.P_00;
                            info.P_11 = spBfItem.P_11;
                            info.P_22 = spBfItem.P_22;
                            info.P_33 = spBfItem.P_33;
                            info.P_QT = spBfItem.P_QT;
                            info.S_10 = spBfItem.S_10;
                            info.S_20 = spBfItem.S_20;
                            info.S_21 = spBfItem.S_21;
                            info.S_30 = spBfItem.S_30;
                            info.S_31 = spBfItem.S_31;
                            info.S_32 = spBfItem.S_32;
                            info.S_40 = spBfItem.S_40;
                            info.S_41 = spBfItem.S_41;
                            info.S_42 = spBfItem.S_42;
                            info.S_QT = spBfItem.S_QT;
                        }
                        #endregion

                        list.Add(info);
                    }
                    break;
                case "bqc":
                    var spBqc = SPList_BQC(issuse); //半全场sp数据
                    foreach (var item in match)
                    {
                        #region 队伍基础信息
                        var info = new BJDC_MatchInfo_WEB()
                        {
                            CreateTime = DateTime.Parse(item.CreateTime),
                            //CreateTime = item.CreateTime,
                            FlatOdds = item.FlatOdds,
                            GuestTeamName = item.GuestTeamName,
                            GuestTeamSort = item.GuestTeamSort,
                            GuestTeamId = item.GuestTeamId.ToString(),
                            HomeTeamName = item.HomeTeamName,
                            HomeTeamSort = item.HomeTeamSort,
                            HomeTeamId = item.HomeTeamId.ToString(),
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            LetBall = item.LetBall,
                            MatchState = item.MatchState,
                            MatchColor = item.MatchColor,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            //LocalStopTime = item.LocalStopTime,
                            LocalStopTime = DateTime.Parse(item.LocalStopTime),
                            LoseOdds = item.LoseOdds,
                            MatchOrderId = item.MatchOrderId,
                            //MatchStartTime = item.MatchStartTime,
                            MatchStartTime = DateTime.Parse(item.MatchStartTime),
                            WinOdds = item.WinOdds,
                            Mid = item.Mid,
                            FXId = item.FXId,
                            Gi = item.Gi,
                            Hi = item.Hi,
                        };
                        #endregion

                        #region 附加队伍结果信息
                        var res = matchresult.FirstOrDefault(p => p.Id == item.Id);
                        if (res != null)
                        {
                            info.ZJQ_Result = res.ZJQ_Result;
                            info.ZJQ_SP = res.ZJQ_SP;
                            info.SXDS_SP = res.SXDS_SP;
                            info.SXDS_Result = res.SXDS_Result;
                            info.SPF_SP = res.SPF_SP;
                            info.SPF_Result = res.SPF_Result;
                            info.MatchStateName = res.MatchState;
                            info.GuestHalf_Result = res.GuestHalf_Result;
                            info.GuestFull_Result = res.GuestFull_Result;
                            info.BQC_SP = res.BQC_SP;
                            info.BQC_Result = res.BQC_Result;
                            info.BF_SP = res.BF_SP;
                            info.BF_Result = res.BF_Result;
                            info.HomeFull_Result = res.HomeFull_Result;
                            info.HomeHalf_Result = res.HomeHalf_Result;
                            //info.LotteryTime = res.CreateTime;
                            info.LotteryTime = DateTime.Parse(res.CreateTime);
                        }
                        else if (!isLeftJoin)
                        {
                            continue;
                        }
                        #endregion

                        #region 附加半全场sp数据
                        var spBqcItem = spBqc.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                        if (spBqcItem != null)
                        {
                            info.F_F_Odds = spBqcItem.F_F_Odds;
                            info.F_P_Odds = spBqcItem.F_P_Odds;
                            info.F_SH_Odds = spBqcItem.F_SH_Odds;
                            info.P_F_Odds = spBqcItem.P_F_Odds;
                            info.P_P_Odds = spBqcItem.P_P_Odds;
                            info.P_SH_Odds = spBqcItem.P_SH_Odds;
                            info.SH_F_Odds = spBqcItem.SH_F_Odds;
                            info.SH_P_Odds = spBqcItem.SH_P_Odds;
                            info.SH_SH_Odds = spBqcItem.SH_SH_Odds;
                        }
                        #endregion
                        list.Add(info);
                    }
                    break;
            }
            return list;
        }

        #region 北单SP数据
        /// <summary>
        /// 北单 - 胜平负SP数据
        /// </summary>
        /// <param name="issuse">期号</param>
        /// <returns>北单胜平负SP数据</returns>
        public static List<BJDC_SPF_SpInfo> SPList_SPF(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BJDC_SPF_SpInfo>(SPFile("spf", issuse));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BJDC_SPF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 总进球SP数据
        /// </summary>
        /// <param name="issuse">期号</param>
        /// <returns>北单总进球SP数据</returns>
        public static List<BJDC_ZJQ_SpInfo> SPList_ZJQ(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BJDC_ZJQ_SpInfo>(SPFile("zjq", issuse));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BJDC_ZJQ_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 上下单双SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单上下单双SP数据</returns>
        public static List<BJDC_SXDS_SpInfo> SPList_SXDS(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BJDC_SXDS_SpInfo>(SPFile("sxds", issuse));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BJDC_SXDS_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 比分SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单比分SP数据</returns>
        public static List<BJDC_BF_SpInfo> SPList_BF(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BJDC_BF_SpInfo>(SPFile("bf", issuse));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BJDC_BF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 半全场SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单半全场SP数据</returns>
        public static List<BJDC_BQC_SpInfo> SPList_BQC(string issuse)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var json_match = bizHelper.GetMatchInfoList<BJDC_BQC_SpInfo>(SPFile("bqc", issuse));
                return json_match;
            }
            catch (Exception)
            {
                return new List<BJDC_BQC_SpInfo>();
            }
        }
        #endregion

        #region 北单SP走势数据
        /// <summary>
        /// 北单 - 胜平负SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单胜平负SP数据</returns>
        public static List<BJDC_SPF_SpInfo> Trend_SPList_SPF(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("spf", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_SPF_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SPF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 总进球SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单总进球SP数据</returns>
        public static List<BJDC_ZJQ_SpInfo> Trend_SPList_ZJQ(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("zjq", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_ZJQ_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_ZJQ_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 上下单双SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单上下单双SP数据</returns>
        public static List<BJDC_SXDS_SpInfo> Trend_SPList_SXDS(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("sxds", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_SXDS_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SXDS_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 比分SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单比分SP数据</returns>
        public static List<BJDC_BF_SpInfo> Trend_SPList_BF(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("bf", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_BF_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_BF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 半全场SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单半全场SP数据</returns>
        public static List<BJDC_BQC_SpInfo> Trend_SPList_BQC(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("bqc", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_BQC_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_BQC_SpInfo>();
            }
        }
        #endregion

        #endregion
    }
}