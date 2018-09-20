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
    /// 竞彩篮球 - json文件读取管理
    /// </summary>
    public static class Json_JCLQ
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
        /// 竞彩篮球队伍信息文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchDate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string type, string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return "/matchdata/jclq/new/Match_" + type + "_List.json";
            }
            return "/matchdata/jclq/new/" + matchDate + "/Match_List.json";
        }
        /// <summary>
        /// 竞彩篮球队伍单式上传信息文件地址
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchSingleFile(string type)
        {
            return "/matchdata/jclq/Match_List_HH.json";
        }

        /// <summary>
        /// 竞彩篮球 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="matchDate"></param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return "/matchdata/jclq/Match_Result_List.json";
            }
            return "/matchdata/jclq/" + matchDate + "/Match_Result_List.json";
        }

        /// <summary>
        /// 竞彩篮球 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchdate">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, int oddtype = 2, string matchdate = null)
        {
            if (string.IsNullOrEmpty(matchdate))
            {
                return "/matchdata/jclq/" + type + "_SP" + (oddtype == 1 ? "_DS" : "") + ".json";
            }
            return "/matchdata/jclq/" + matchdate + "/" + type + "_SP" + (oddtype == 1 ? "_DS" : "") + ".json";
        }

        #endregion

        #region 竞彩篮球数据读取
        /// <summary>
        /// 竞彩篮球 - 获取队伍信息列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="type">玩法类型</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JclqWeb> MatchList(string type, int oddtype, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            List<JclqWeb> matches = null;
            try
            {
                if (oddtype == 2)
                {
                    matches = bizHelper.GetMatchInfoList<JclqWeb>(MatchFile(type, matchDate));
                }
                else if (oddtype == 3)   //单式
                {
                    matches = bizHelper.GetMatchInfoList<JclqWeb>(MatchSingleFile(string.Empty));
                }

                return matches;
            }
            catch (Exception)
            {
                return new List<JclqWeb>();
            }


            //try
            //{
            //    string json_match = ReadFileString(MatchFile(type, matchDate));
            //    var res = JsonSerializer.Deserialize<List<JCLQ_MatchInfo>>(json_match);

            //    if (type.ToLower() == "hh")
            //    {
            //        json_match = ReadFileString(MatchFile("sf", matchDate));
            //        var hhres = JsonSerializer.Deserialize<List<JCLQ_MatchInfo>>(json_match);
            //        if (hhres.Count > res.Count)
            //        {
            //            res = hhres;
            //        }
            //    }

            //    return res;
            //}
            //catch (Exception)
            //{
            //    return new List<JCLQ_MatchInfo>();
            //}
        }

        /// <summary>
        /// 竞彩篮球 - 获取队伍比赛结果信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCLQ_MatchResultInfo> MatchResultList(string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<JCLQ_MatchResultInfo>(MatchResultFile(matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<JCLQ_MatchResultInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="type">玩法类型</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<JclqWeb> MatchList_WEB(string type, int oddtype = 2, string matchDate = null, bool isLeftJoin = true)
        {
            var match = MatchList(type, oddtype, matchDate);
            if (oddtype == 3)
            {
                return match.OrderBy(p => p.MatchId).ToList();
            }
            if (oddtype == 2)
            {
                var flag = matchDate != null;
                var matchresult = MatchResultList(matchDate); ;
                switch (type)
                {
                    case "hh":
                        var spSf = SPList_SF(oddtype, matchDate); //胜负sp数据
                        var spRfsf = SPList_RFSF(oddtype, matchDate); //让分胜负sp数据
                        var spDxf = SPList_DXF(oddtype, matchDate); //大小分sp数据
                        var spSfc = SPList_SFC(oddtype, matchDate); //胜分差sp数据
                        if (flag)
                        {
                            foreach (var item in match)
                            {
                                ResultData(item, matchresult);
                                SfData(item, spSf);
                                RfsfData(item, spRfsf);
                                DxfData(item, spDxf);
                                SfcData(item, spSfc);
                            }
                        }
                        else
                        {
                            foreach (var item in match)
                            {
                                SfData(item, spSf);
                                RfsfData(item, spRfsf);
                                DxfData(item, spDxf);
                                if (Convert.ToDateTime(item.FSStopBettingTime) <= DateTime.Now)
                                {
                                    ResultData(item, matchresult);
                                    SfcData(item, spSfc);
                                }
                            }
                        }
                        break;
                    case "sf":
                        spSf = SPList_SF(oddtype, matchDate); //胜负sp数据
                        if (flag)
                        {
                            foreach (var item in match)
                            {
                                ResultData(item, matchresult);
                                SfData(item, spSf);
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
                                SfData(item, spSf);
                            }
                        }
                        break;
                    case "rfsf":
                        spRfsf = SPList_RFSF(oddtype, matchDate); //让分胜负sp数据
                        if (flag)
                        {
                            foreach (var item in match)
                            {
                                ResultData(item, matchresult);
                                RfsfData(item, spRfsf);
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
                                RfsfData(item, spRfsf);
                            }
                        }
                        break;
                    case "sfc":
                        spSfc = SPList_SFC(oddtype, matchDate); //胜分差sp数据
                        if (flag)
                        {
                            foreach (var item in match)
                            {
                                ResultData(item, matchresult);
                                SfcData(item, spSfc);
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
                                SfcData(item, spSfc);
                            }
                        }
                        break;
                    case "dxf":
                        spDxf = SPList_DXF(oddtype, matchDate); //大小分sp数据
                        if (flag)
                        {
                            foreach (var item in match)
                            {
                                ResultData(item, matchresult);
                                DxfData(item, spDxf);
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
                                DxfData(item, spDxf);
                            }
                        }
                        break;
                }
            }
            return match.OrderBy(p => p.MatchId).ToList();
        }

        #region 竞彩篮球-SP数据
        /// <summary>
        /// 竞彩篮球 - 胜负SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-胜负SP数据</returns>
        public static List<SF> SPList_SF(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<SF>(SPFile("sf", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<SF>();
            }
        }

        /// <summary>
        /// 竞彩篮球 - 让分胜负SP数据
        /// </summary>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-让分胜负SP数据</returns>
        public static List<RFSF> SPList_RFSF(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<RFSF>(SPFile("rfsf", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<RFSF>();
            }
        }

        /// <summary>
        /// 竞彩篮球 -胜分差SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-胜分差SP数据</returns>
        public static List<SFC> SPList_SFC(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<SFC>(SPFile("sfc", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<SFC>();
            }
        }

        /// <summary>
        /// 竞彩篮球 - 大小分SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">过关类型，如果是单关则单独取SP</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-大小分SP数据</returns>
        public static List<DXF> SPList_DXF(int oddtype = 2, string matchDate = null)
        {
            var bizHelper = new BusinessHelper();
            try
            {
                var jsonMatch = bizHelper.GetMatchInfoList<DXF>(SPFile("dxf", oddtype, matchDate));
                return jsonMatch;
            }
            catch (Exception)
            {
                return new List<DXF>();
            }
        }
        #endregion
        private static void ResultData(JclqWeb item, IEnumerable<JCLQ_MatchResultInfo> matchresult)
        {
            #region 附加队伍结果信息
            var res = matchresult.FirstOrDefault(p => p.MatchId == item.MatchId);
            if (res != null)
            {
                item.DXF_Result = res.DXF_Result;
                item.DXF_SP = res.DXF_SP;
                item.DXF_Trend = res.DXF_Trend;
                item.GuestScore = res.GuestScore;
                item.HomeScore = res.HomeScore;
                item.RFSF_Result = res.RFSF_Result;
                item.RFSF_SP = res.RFSF_SP;
                item.RFSF_Trend = res.RFSF_Trend;
                item.SF_Result = res.SF_Result;
                item.SF_SP = res.SF_SP;
                item.SFC_Result = res.SFC_Result;
                item.SFC_SP = res.SFC_SP;
                item.MatchState = res.MatchState;
            }
            else
            {
                item.GuestScore = -1;
                item.HomeScore = -1;
            }

            #endregion
        }
        private static void SfData(JclqWeb item, IEnumerable<SF> spSf)
        {
            #region 附加胜负sp数据
            var spSfItem = spSf.FirstOrDefault(p => p.MatchId == item.MatchId);
            item.SF = new SF();
            if (spSfItem != null)
            {
                item.SF.WinSP = spSfItem.WinSP;
                item.SF.LoseSP = spSfItem.LoseSP;
                if (string.IsNullOrEmpty(spSfItem.NoSaleState) || "null" == spSfItem.NoSaleState)
                {
                    if (spSfItem.WinSP > 0 && spSfItem.LoseSP > 0)
                    {
                        item.SF.NoSaleState = "0";
                    }
                    else
                    {
                        item.SF.NoSaleState = "1";
                    }
                }
                else
                {
                    item.SF.NoSaleState = spSfItem.NoSaleState;
                }
            }
            else
            {
                item.SF.NoSaleState = "1";
            }

            #endregion
        }
        private static void RfsfData(JclqWeb item, IEnumerable<RFSF> spRfsf)
        {
            #region 附加让分胜负sp数据
            var spRfsfItem = spRfsf.FirstOrDefault(p => p.MatchId == item.MatchId);
            item.RFSF = new RFSF();
            if (spRfsfItem != null)
            {
                item.RFSF.RF = spRfsfItem.RF;
                item.RFSF.LoseSP = spRfsfItem.LoseSP;
                item.RFSF.WinSP = spRfsfItem.WinSP;
                if (string.IsNullOrEmpty(spRfsfItem.NoSaleState) || "null" == spRfsfItem.NoSaleState)
                {
                    if (spRfsfItem.WinSP > 0 && spRfsfItem.LoseSP > 0)
                    {
                        item.RFSF.NoSaleState = "0";
                    }
                    else
                    {
                        item.RFSF.NoSaleState = "1";
                    }
                }
                else
                {
                    item.RFSF.NoSaleState = spRfsfItem.NoSaleState;
                }
            }
            else
            {
                item.RFSF.NoSaleState = "1";
            }

            #endregion
        }
        private static void SfcData(JclqWeb item, IEnumerable<SFC> spSfc)
        {
            #region 附加胜分差sp数据
            var spSfcItem = spSfc.FirstOrDefault(p => p.MatchId == item.MatchId);
            item.SFC = new SFC();
            if (spSfcItem != null)
            {
                item.SFC.GuestWin1_5 = spSfcItem.GuestWin1_5;
                item.SFC.GuestWin11_15 = spSfcItem.GuestWin11_15;
                item.SFC.GuestWin16_20 = spSfcItem.GuestWin16_20;
                item.SFC.GuestWin21_25 = spSfcItem.GuestWin21_25;
                item.SFC.GuestWin26 = spSfcItem.GuestWin26;
                item.SFC.GuestWin6_10 = spSfcItem.GuestWin6_10;

                item.SFC.HomeWin1_5 = spSfcItem.HomeWin1_5;
                item.SFC.HomeWin11_15 = spSfcItem.HomeWin11_15;
                item.SFC.HomeWin16_20 = spSfcItem.HomeWin16_20;
                item.SFC.HomeWin21_25 = spSfcItem.HomeWin21_25;
                item.SFC.HomeWin26 = spSfcItem.HomeWin26;
                item.SFC.HomeWin6_10 = spSfcItem.HomeWin6_10;
                if (string.IsNullOrEmpty(spSfcItem.NoSaleState) || "null" == spSfcItem.NoSaleState)
                {
                    if (spSfcItem.GuestWin1_5 > 0 &&
                        spSfcItem.GuestWin11_15 > 0 &&
                        spSfcItem.GuestWin16_20 > 0 &&
                        spSfcItem.GuestWin21_25 > 0 &&
                        spSfcItem.GuestWin26 > 0 &&
                        spSfcItem.GuestWin6_10 > 0 &&
                        spSfcItem.HomeWin1_5 > 0 &&
                        spSfcItem.HomeWin11_15 > 0 &&
                        spSfcItem.HomeWin16_20 > 0 &&
                        spSfcItem.HomeWin21_25 > 0 &&
                        spSfcItem.HomeWin26 > 0 &&
                        spSfcItem.HomeWin6_10 > 0
                        )
                    {
                        item.SFC.NoSaleState = "0";
                    }
                    else
                    {
                        item.SFC.NoSaleState = "1";
                    }
                }
                else
                {
                    item.SFC.NoSaleState = spSfcItem.NoSaleState;
                }
            }
            else
            {
                item.SFC.NoSaleState = "1";
            }

            #endregion
        }
        private static void DxfData(JclqWeb item, IEnumerable<DXF> spDxf)
        {
            #region 附加大小分sp数据
            var spDxfItem = spDxf.FirstOrDefault(p => p.MatchId == item.MatchId);
            item.DXF = new DXF();
            if (spDxfItem != null)
            {
                item.DXF.DF = spDxfItem.DF;
                item.DXF.XF = spDxfItem.XF;
                item.DXF.YSZF = spDxfItem.YSZF;
                if (string.IsNullOrEmpty(spDxfItem.NoSaleState) || "null" == spDxfItem.NoSaleState)
                {
                    if (spDxfItem.DF > 0 && spDxfItem.XF > 0)
                    {
                        item.DXF.NoSaleState = "0";
                    }
                    else
                    {
                        item.DXF.NoSaleState = "1";
                    }
                }
                else
                {
                    item.DXF.NoSaleState = spDxfItem.NoSaleState;
                }
            }
            else
            {
                item.DXF.NoSaleState = "1";
            }

            #endregion
        }
        #endregion
    }
}