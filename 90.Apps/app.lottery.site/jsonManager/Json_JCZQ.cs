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
using GameBiz.Core;

namespace app.lottery.site.jsonManager
{
    /// <summary>
    /// 竞彩足球 - json文件读取管理
    /// </summary>
    public static class Json_JCZQ
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
        /// 竞彩足球队伍信息文件地址
        /// </summary>
        /// <param name="type">奖期，如果为空则取根目录比赛结果</param>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string type, string matchDate = null)
        {
            //http://192.168.0.180:888/matchdata/jczq/new/match_list_spf.json 混合过关
            //http://192.168.0.180:888/matchdata/jczq/new/match_list_zjq.json
            //http://192.168.0.180:888/matchdata/jczq/new/match_list_bqc.json
            //http://192.168.0.180:888/matchdata/jczq/new/match_list_bf.json
            //http://192.168.0.180:888/matchdata/jczq/new/match_list_spf.json  主客二选一
            //http://192.168.0.180:888/matchdata/jczq/new/150818/match_list_spf.json
            if (type == "hh" || type == "spf" || type == "exy")
            {
                type = "spf";
            }
            if (string.IsNullOrEmpty(matchDate))
            {
                return "/matchdata/jczq/Match_List_" + type + ".json";
            }
            return "/matchdata/jczq/" + matchDate + "/Match_List_" + type + ".json";
        }
        /// <summary>
        /// 竞彩足球队伍信息文件地址
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchSingleFile(string type)
        {
            return "/matchdata/jczq/Match_List_HH.json";
            //if (type == "spf" || type == "rqspf")
            //{
            //    return "/matchdata/jczq/Match_List_HH.json";
            //}
            //return "/matchdata/jczq/Match_List_" + type + ".json";
        }

        /// <summary>
        /// 竞彩足球 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="matchDate"></param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return "/matchdata/jczq/Match_Result_List.json";
            }
            return "/matchdata/jczq/" + matchDate + "/Match_Result_List.json";
        }

        /// <summary>
        /// 竞彩足球 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchdate">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, int oddtype = 2, string matchdate = null)
        {
            if (string.IsNullOrEmpty(matchdate))
            {
                return "/matchdata/jczq/" + type + "_SP" + (oddtype == 1 ? "_DS" : "") + ".json";
            }
            return "/matchdata/jczq/" + matchdate + "/" + type + "_SP" + (oddtype == 1 ? "_DS" : "") + ".json";
        }

        #endregion

        #region 竞彩足球数据读取
        /// <summary>
        /// 竞彩足球 - 获取队伍信息列表
        /// </summary>
        /// <param name="type">球类</param>
        /// <param name="oddtype">球类1:足球、2:足球单式、3:</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JczqWeb> MatchList(string type, int oddtype, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            List<JczqWeb> matches = null;
            try
            {
                if (oddtype == 2)
                {
                    matches = bizHelper.GetMatchInfoList<JczqWeb>(MatchFile(type, matchDate));
                }
                else if (oddtype == 3)   //单式
                {
                    matches = bizHelper.GetMatchInfoList<JczqWeb>(MatchSingleFile(string.Empty));
                }

                return matches;
            }
            catch (Exception)
            {
                return new List<JczqWeb>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 获取队伍比赛结果信息
        /// </summary>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCZQ_MatchResultInfo> MatchResultList(string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<JCZQ_MatchResultInfo>(MatchResultFile(matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<JCZQ_MatchResultInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="type">玩法</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<JczqWeb> MatchList_WEB(string type, int oddtype = 2, string matchDate = null, bool isLeftJoin = true)
        {
            var match = MatchList(type, oddtype, matchDate);
            if (oddtype == 3)
            {
                return match.OrderBy(p => p.MatchId).ToList();
            }
            if ("exy" == type)
            {
                match = match.Where(o => o.LetBall == -1 || o.LetBall == 1).ToList();
            }
            if (oddtype == 2)
            {
                var flag = matchDate != null;
                var matchresult = MatchResultList(matchDate);
                switch (type)
                {
                    case "hh":
                        {
                            var spSpf = SPList_SPF(oddtype, matchDate); //让球胜平负sp数据
                            var spBrqspf = SPList_BRQSPF(oddtype, matchDate); //胜平负sp数据
                            var spZjq = SPList_ZJQ(oddtype, matchDate); //总进球sp数据
                            var spBf = SPList_BF(oddtype, matchDate); //比分sp数据
                            var spBqc = SPList_BQC(oddtype, matchDate); //半全场sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    SpfData(item, spBrqspf);
                                    RqspfData(item, spSpf);
                                    ZjqData(item, spZjq);
                                    BqcData(item, spBqc);
                                    BfData(item, spBf);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    SpfData(item, spBrqspf);
                                    RqspfData(item, spSpf);
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                        ZjqData(item, spZjq);
                                        BqcData(item, spBqc);
                                        BfData(item, spBf);
                                    }
                                }
                            }
                        }
                        break;
                    case "exy":
                        {
                            var spSpf = SPList_SPF(oddtype, matchDate); //让球胜平负sp数据
                            var spBrqspf = SPList_BRQSPF(oddtype, matchDate); //胜平负sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    SpfData(item, spBrqspf);
                                    RqspfData(item, spSpf);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                    }
                                    SpfData(item, spBrqspf);
                                    RqspfData(item, spSpf);
                                }
                            }
                        }
                        break;
                    case "spf":
                        {
                            var spBrqspf = SPList_BRQSPF(oddtype, matchDate); //胜平负sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    SpfData(item, spBrqspf);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                    }
                                    SpfData(item, spBrqspf);
                                }
                            }
                        }
                        break;
                    case "rqspf":
                        {
                            var spSpf = SPList_SPF(oddtype, matchDate); //让球胜平负sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    RqspfData(item, spSpf);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                    }
                                    RqspfData(item, spSpf);
                                }
                            }
                        }
                        break;
                    case "zjq":
                        {
                            var spZjq = SPList_ZJQ(oddtype, matchDate); //总进球sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    ZjqData(item, spZjq);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                    }
                                    ZjqData(item, spZjq);
                                }
                            }
                        }
                        break;
                    case "bqc":
                        {
                            var spBqc = SPList_BQC(oddtype, matchDate); //半全场sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    BqcData(item, spBqc);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                    }
                                    BqcData(item, spBqc);
                                }
                            }
                        }
                        break;
                    case "bf":
                        {
                            var spBf = SPList_BF(oddtype, matchDate); //比分sp数据
                            if (flag)
                            {
                                foreach (var item in match)
                                {
                                    ResultData(item, matchresult);
                                    BfData(item, spBf);
                                }
                            }
                            else
                            {
                                foreach (var item in match)
                                {
                                    if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                    {
                                        ResultData(item, matchresult);
                                    }
                                    BfData(item, spBf);
                                }
                            }
                        }
                        break;
                }
            }
            return match.OrderBy(p => p.MatchId).ToList();
        }

        private static void ResultData(JczqWeb item, IEnumerable<JCZQ_MatchResultInfo> matchresult)
        {
            #region 附加队伍结果信息
            var res = matchresult.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (res != null)
            {
                item.ZJQ_Result = res.ZJQ_Result;
                item.ZJQ_SP = res.ZJQ_SP;
                item.SPF_SP = res.SPF_SP;
                item.SPF_Result = res.SPF_Result;
                item.BQC_SP = res.BQC_SP;
                item.BQC_Result = res.BQC_Result;
                item.BF_SP = res.BF_SP;
                item.BF_Result = res.BF_Result;
                item.BRQSPF_Result = res.BRQSPF_Result;
                item.BRQSPF_SP = res.BRQSPF_SP;
                item.FullGuestTeamScore = res.FullGuestTeamScore;
                item.FullHomeTeamScore = res.FullHomeTeamScore;
                item.HalfGuestTeamScore = res.HalfGuestTeamScore;
                item.HalfHomeTeamScore = res.HalfHomeTeamScore;
                item.MatchState = res.MatchState;
            }
            else
            {
                item.FullGuestTeamScore = -1;
                item.FullHomeTeamScore = -1;
            }

            #endregion
        }
        private static void BfData(JczqWeb item, IEnumerable<BF> spBf)
        {
            #region 附加比分sp数据
            item.BF = new BF();
            var spBfItem = spBf.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (spBfItem != null)
            {
                item.BF.F_01 = spBfItem.F_01;
                item.BF.F_02 = spBfItem.F_02;
                item.BF.F_03 = spBfItem.F_03;
                item.BF.F_04 = spBfItem.F_04;
                item.BF.F_05 = spBfItem.F_05;
                item.BF.F_12 = spBfItem.F_12;
                item.BF.F_13 = spBfItem.F_13;
                item.BF.F_14 = spBfItem.F_14;
                item.BF.F_15 = spBfItem.F_15;
                item.BF.F_23 = spBfItem.F_23;
                item.BF.F_24 = spBfItem.F_24;
                item.BF.F_25 = spBfItem.F_25;
                item.BF.F_QT = spBfItem.F_QT;
                item.BF.P_00 = spBfItem.P_00;
                item.BF.P_11 = spBfItem.P_11;
                item.BF.P_22 = spBfItem.P_22;
                item.BF.P_33 = spBfItem.P_33;
                item.BF.P_QT = spBfItem.P_QT;
                item.BF.S_10 = spBfItem.S_10;
                item.BF.S_20 = spBfItem.S_20;
                item.BF.S_21 = spBfItem.S_21;
                item.BF.S_30 = spBfItem.S_30;
                item.BF.S_31 = spBfItem.S_31;
                item.BF.S_32 = spBfItem.S_32;
                item.BF.S_40 = spBfItem.S_40;
                item.BF.S_41 = spBfItem.S_41;
                item.BF.S_42 = spBfItem.S_42;
                item.BF.S_50 = spBfItem.S_50;
                item.BF.S_51 = spBfItem.S_51;
                item.BF.S_52 = spBfItem.S_52;
                item.BF.S_QT = spBfItem.S_QT;
                item.BF.NoSaleState = spBfItem.NoSaleState;
            }
            #endregion
        }
        private static void SpfData(JczqWeb item, IEnumerable<BRQSPF> spBrqspf)
        {
            #region 附加胜平负sp数据
            item.BRQSPF = new BRQSPF();
            var spBrqspfItem = spBrqspf.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (spBrqspfItem != null)
            {
                item.BRQSPF.WinOdds = spBrqspfItem.WinOdds;
                item.BRQSPF.FlatOdds = spBrqspfItem.FlatOdds;
                item.BRQSPF.LoseOdds = spBrqspfItem.LoseOdds;
                if (string.IsNullOrEmpty(spBrqspfItem.NoSaleState) ||
                    "null" == spBrqspfItem.NoSaleState.ToLower())
                {
                    if (spBrqspfItem.WinOdds > 0 && spBrqspfItem.FlatOdds > 0 &&
                        spBrqspfItem.LoseOdds > 0)
                    {
                        item.BRQSPF.NoSaleState = "0";
                    }
                    else
                    {
                        item.BRQSPF.NoSaleState = "1";
                    }
                }
                else
                {
                    item.BRQSPF.NoSaleState = spBrqspfItem.NoSaleState;
                }
            }
            else
            {
                item.BRQSPF.NoSaleState = "1";
            }
            #endregion
        }
        private static void RqspfData(JczqWeb item, IEnumerable<SPF> spSpf)
        {
            #region 附加让球胜平负sp数据
            item.SPF = new SPF();
            var spSpfItem = spSpf.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (spSpfItem != null)
            {
                item.SPF.WinOdds = spSpfItem.WinOdds;
                item.SPF.FlatOdds = spSpfItem.FlatOdds;
                item.SPF.LoseOdds = spSpfItem.LoseOdds;
                if (string.IsNullOrEmpty(spSpfItem.NoSaleState) ||
                    "null" == spSpfItem.NoSaleState.ToLower())
                {
                    if (spSpfItem.WinOdds > 0 && spSpfItem.FlatOdds > 0 && spSpfItem.LoseOdds > 0)
                    {
                        item.SPF.NoSaleState = "0";
                    }
                    else
                    {
                        item.SPF.NoSaleState = "1";
                    }
                }
                else
                {
                    item.SPF.NoSaleState = spSpfItem.NoSaleState;
                }
            }
            else
            {
                item.SPF.NoSaleState = "1";
            }

            #endregion
        }
        private static void ZjqData(JczqWeb item, IEnumerable<ZJQ> spZjq)
        {
            #region 附加总进球sp数据
            item.ZJQ = new ZJQ();
            var spZjqItem = spZjq.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (spZjqItem != null)
            {
                item.ZJQ.JinQiu_0_Odds = spZjqItem.JinQiu_0_Odds;
                item.ZJQ.JinQiu_1_Odds = spZjqItem.JinQiu_1_Odds;
                item.ZJQ.JinQiu_2_Odds = spZjqItem.JinQiu_2_Odds;
                item.ZJQ.JinQiu_3_Odds = spZjqItem.JinQiu_3_Odds;
                item.ZJQ.JinQiu_4_Odds = spZjqItem.JinQiu_4_Odds;
                item.ZJQ.JinQiu_5_Odds = spZjqItem.JinQiu_5_Odds;
                item.ZJQ.JinQiu_6_Odds = spZjqItem.JinQiu_6_Odds;
                item.ZJQ.JinQiu_7_Odds = spZjqItem.JinQiu_7_Odds;
                if (string.IsNullOrEmpty(spZjqItem.NoSaleState) || "null" == spZjqItem.NoSaleState)
                {
                    if (spZjqItem.JinQiu_0_Odds > 0 &&
                        spZjqItem.JinQiu_1_Odds > 0 &&
                        spZjqItem.JinQiu_2_Odds > 0 &&
                        spZjqItem.JinQiu_3_Odds > 0 &&
                        spZjqItem.JinQiu_4_Odds > 0 &&
                        spZjqItem.JinQiu_5_Odds > 0 &&
                        spZjqItem.JinQiu_6_Odds > 0 &&
                        spZjqItem.JinQiu_6_Odds > 0 &&
                        spZjqItem.JinQiu_7_Odds > 0)
                    {
                        item.ZJQ.NoSaleState = "0";
                    }
                    else
                    {
                        item.ZJQ.NoSaleState = "1";
                    }
                }
                else
                {
                    item.ZJQ.NoSaleState = spZjqItem.NoSaleState;
                }
            }
            else
            {
                item.ZJQ.NoSaleState = "1";
            }

            #endregion
        }
        private static void BqcData(JczqWeb item, IEnumerable<BQC> spBqc)
        {
            #region 附加半全场sp数据
            item.BQC = new BQC();
            var spBqcItem = spBqc.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (spBqcItem != null)
            {
                item.BQC.F_F_Odds = spBqcItem.F_F_Odds;
                item.BQC.F_P_Odds = spBqcItem.F_P_Odds;
                item.BQC.F_SH_Odds = spBqcItem.F_SH_Odds;
                item.BQC.P_F_Odds = spBqcItem.P_F_Odds;
                item.BQC.P_P_Odds = spBqcItem.P_P_Odds;
                item.BQC.P_SH_Odds = spBqcItem.P_SH_Odds;
                item.BQC.SH_F_Odds = spBqcItem.SH_F_Odds;
                item.BQC.SH_P_Odds = spBqcItem.SH_P_Odds;
                item.BQC.SH_SH_Odds = spBqcItem.SH_SH_Odds;
                if (string.IsNullOrEmpty(spBqcItem.NoSaleState) || "null" == spBqcItem.NoSaleState)
                {
                    if (spBqcItem.F_F_Odds > 0 &&
                        spBqcItem.F_P_Odds > 0 &&
                        spBqcItem.F_SH_Odds > 0 &&
                        spBqcItem.P_F_Odds > 0 &&
                        spBqcItem.P_P_Odds > 0 &&
                        spBqcItem.P_SH_Odds > 0 &&
                        spBqcItem.SH_F_Odds > 0 &&
                        spBqcItem.SH_P_Odds > 0 &&
                        spBqcItem.SH_SH_Odds > 0)
                    {
                        item.BQC.NoSaleState = "0";
                    }
                    else
                    {
                        item.BQC.NoSaleState = "1";
                    }
                }
                else
                {
                    item.BQC.NoSaleState = spBqcItem.NoSaleState;
                }
            }
            else
            {
                item.BQC.NoSaleState = "1";
            }

            #endregion
        }

        #region 竞彩足球-SP数据
        /// <summary>
        /// 竞彩足球 - 让球胜平负SP数据
        /// </summary>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<SPF> SPList_SPF(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<SPF>(SPFile("spf", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<SPF>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 胜平负SP数据
        /// </summary>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<BRQSPF> SPList_BRQSPF(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BRQSPF>(SPFile("brqspf", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BRQSPF>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 总进球SP数据
        /// </summary>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-总进球SP数据</returns>
        public static List<ZJQ> SPList_ZJQ(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<ZJQ>(SPFile("zjq", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<ZJQ>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 比分SP数据
        /// </summary>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-比分SP数据</returns>
        public static List<BF> SPList_BF(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BF>(SPFile("bf", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BF>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 半全场SP数据
        /// </summary>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-半全场SP数据</returns>
        public static List<BQC> SPList_BQC(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<BQC>(SPFile("bqc", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<BQC>();
            }
        }
        #endregion

        #endregion


        #region 欧洲冠军竞猜读取数据

        /// <summary>
        /// 欧洲杯文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string OZBFile(string type)
        {
            return "/matchdata/OZB/OZB_" + type + ".json";
        }

        /// <summary>
        /// 竞彩足球 - 欧洲冠军竞猜读取数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCZQ_OZBMatchInfo> OZBMatchList(string type)
        {
            var bizHelper = new BusinessHelper();
            List<JCZQ_OZBMatchInfo> matches = null;
            try
            {
                matches = bizHelper.GetMatchInfoList<JCZQ_OZBMatchInfo>(OZBFile(type));
                return matches;
            }
            catch (Exception)
            {
                return new List<JCZQ_OZBMatchInfo>();
            }
        }
        #endregion


        #region 世界冠军竞猜读取数据

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
        #endregion
    }
}