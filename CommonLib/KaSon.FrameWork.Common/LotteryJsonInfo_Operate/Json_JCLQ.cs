using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using KaSon.FrameWork.Common;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common.Sport;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Utilities;

namespace KaSon.FrameWork.Common
{
    public class Json_JCLQ
    {
        #region 文件路径

        /// <summary>
        /// 竞彩篮球队伍信息文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string type, string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                //return "/MatchData/" + "jclq/Match_" + (type.ToLower() == "sf" ? "sf" : "rfsf") + "_List.json";20150519 sf读取match_list文件
                return "/MatchData/" + "jclq/Match_" + (type.ToLower() == "sf" ? "" : "rfsf_") + "List.json";
            }
            else
            {
                return "/MatchData/" + "/jclq/" + matchDate + "/Match_List.json";
            }
        }
        /// <summary>
        /// 根据文件名获取文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFilePath(string fileName)
        {
            return "/MatchData/jclq/" + fileName + ".json";
        }
        /// <summary>
        /// 竞彩篮球 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return "/MatchData/" + "jclq/Match_Result_List.json";
            }
            else
            {
                return "/MatchData/" + "/jclq/" + matchDate + "/Match_Result_List.json";
            }
        }

        /// <summary>
        /// 竞彩篮球 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchdate">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string matchdate = null)
        {
            //if (string.IsNullOrEmpty(matchdate))
            //{
            //    return "/MatchData/" + "jclq/" + type + "_SP.json";
            //}
            //else
            //{
            //    return "/MatchData/" + "/jclq/" + matchdate + "/" + type + "_SP.json";
            //}
            if (type.ToLower() == "hh")
            {
                return "/MatchData/" + "jclq/SP.json";
            }
            else if (string.IsNullOrEmpty(matchdate))
            {
                return "/MatchData/" + "jclq/" + type + "_SP.json";
            }
            else
            {
                return "/MatchData/" + "/jclq/" + matchdate + "/" + type + "_SP.json";
            }
        }

        #endregion

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="type">玩法类型</param>
        /// <param name="matchDate">查询日期</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<JCLQ_MatchInfo_WEB> MatchList_WEB(string gameType, string matchDate = null, bool isLeftJoin = true)
        {
            BettingHelper bizHelper = new BettingHelper();
            var match = bizHelper.GetMatchInfoList<JCLQ_MatchInfo>(MatchFile(gameType, matchDate));
            var matchresult = bizHelper.GetMatchInfoList<JCLQ_MatchResultInfo>(MatchResultFile(matchDate));
            //var sp_sf =bizHelper.GetMatchInfoList<JCLQ_SF_SPInfo>(SPFile("SF", matchDate)); //胜负sp数据
            //var sp_rfsf = bizHelper.GetMatchInfoList<JCLQ_RFSF_SPInfo>(SPFile("RFSF", matchDate)); //让分胜负sp数据
            //var sp_sfc = bizHelper.GetMatchInfoList<JCLQ_SFC_SPInfo>(SPFile("SFC", matchDate)); //胜分差sp数据
            //var sp_dxf = bizHelper.GetMatchInfoList<JCLQ_DXF_SPInfo>(SPFile("DXF", matchDate)); //大小分sp数据

            var sp_sf = bizHelper.GetMatchInfoList<JCLQ_SF_SPInfo>(SPFile(gameType, matchDate)); //胜负sp数据
            var sp_rfsf = bizHelper.GetMatchInfoList<JCLQ_RFSF_SPInfo>(SPFile(gameType, matchDate)); //让分胜负sp数据
            var sp_sfc = bizHelper.GetMatchInfoList<JCLQ_SFC_SPInfo>(SPFile(gameType, matchDate)); //胜分差sp数据
            var sp_dxf = bizHelper.GetMatchInfoList<JCLQ_DXF_SPInfo>(SPFile(gameType, matchDate)); //大小分sp数据

            var list = new List<JCLQ_MatchInfo_WEB>();
            match = match.Where(t => long.Parse(Convert.ToDateTime(t.FSStopBettingTime).ToString("yyyyMMddHHmmss")) > long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))).ToList();
            foreach (var item in match)
            {
                #region 队伍基础信息

                var matchDataTime = ConvertHelper.ConvertStrToDateTime("20" + item.MatchData);
                var info = new JCLQ_MatchInfo_WEB()
                {
                    //CreateTime = item.CreateTime.ToString("yyyyMMddHHmmss"),
                    //DSStopBettingTime = item.DSStopBettingTime.ToString("yyyyMMddHHmmss"),
                    MatcheDateTime = matchDataTime,
                    //FSStopBettingTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.FSStopBettingTime)).ToString(),
                    FSStopBettingTime = item.FSStopBettingTime,
                    //FSStopBettingTime = item.FSStopBettingTime.ToString("yyyyMMddHHmmss"),
                    //GuestTeamId = item.GuestTeamId,
                    GuestTeamName = item.GuestTeamName,
                    HomeTeamName = item.HomeTeamName,
                    //HomeTeamId = item.HomeTeamId,
                    LeagueColor = item.LeagueColor.Contains("#") ? item.LeagueColor : "#" + "DB7917",
                    //LeagueId = item.LeagueId,
                    LeagueName = item.LeagueName,
                    MatchIdName = item.MatchIdName,
                    StartDateTime = Convert.ToDateTime(item.StartDateTime),
                    //StartDateTime = item.StartDateTime.ToString("yyyyMMddHHmmss"),
                    MatchData = item.MatchData,
                    MatchId = item.MatchId,
                    MatchNumber = item.MatchNumber,
                    //AverageLose = item.AverageLose,
                    //AverageWin = item.AverageWin,
                    //Mid = item.Mid,
                    FXId = item.FXId,
                    PrivilegesType = item.PrivilegesType == null ? string.Empty : item.PrivilegesType,
                    State_HHDG = item.State
                };
                #endregion

                #region 附加队伍结果信息
                var res = matchresult.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (res != null)
                {
                    //info.DXF_Result = res.DXF_Result;
                    //info.DXF_SP = res.DXF_SP;
                    //info.DXF_Trend = res.DXF_Trend;
                    //info.GuestScore = res.GuestScore;
                    //info.HomeScore = res.HomeScore;
                    //info.RFSF_Result = res.RFSF_Result;
                    //info.RFSF_SP = res.RFSF_SP;
                    //info.RFSF_Trend = res.RFSF_Trend;
                    //info.SF_Result = res.SF_Result;
                    //info.SF_SP = res.SF_SP;
                    //info.SFC_Result = res.SFC_Result;
                    //info.SFC_SP = res.SFC_SP;
                    //info.MatchState = res.MatchState;
                }
                else if (!isLeftJoin)
                {
                    continue;
                }
                #endregion

                #region 附加胜负sp数据

                var sp_sf_item = sp_sf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_sf_item != null && sp_sf_item.SF != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_SF_SPInfo>(sp_sf_item.SF);
                        info.SF_WinSP = json.WinSP;
                        info.SF_LoseSP = json.LoseSP;
                        //info.PrivilegesType = json.PrivilegesType == null ? string.Empty : json.PrivilegesType;
                    }
                }
                else
                {
                    if (sp_sf_item != null)
                    {
                        info.SF_WinSP = sp_sf_item.WinSP;
                        info.SF_LoseSP = sp_sf_item.LoseSP;
                        //info.PrivilegesType = sp_sf_item.PrivilegesType == null ? string.Empty : sp_sf_item.PrivilegesType;
                    }
                }

                #endregion

                #region 附加让分胜负sp数据
                var sp_rfsf_item = sp_rfsf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_rfsf_item != null && sp_rfsf_item.RFSF != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_RFSF_SPInfo>(sp_rfsf_item.RFSF);
                        info.RF = json.RF;
                        info.RF_LoseSP = json.LoseSP;
                        info.RF_WinSP = json.WinSP;
                        //info.PrivilegesType = json.PrivilegesType == null ? string.Empty : json.PrivilegesType;
                    }
                }
                else
                {
                    if (sp_rfsf_item != null)
                    {
                        info.RF = sp_rfsf_item.RF;
                        info.RF_LoseSP = sp_rfsf_item.LoseSP;
                        info.RF_WinSP = sp_rfsf_item.WinSP;
                        //info.PrivilegesType = sp_rfsf_item.PrivilegesType == null ? string.Empty : sp_rfsf_item.PrivilegesType;
                    }
                }
                #endregion

                #region 附加胜分差sp数据
                var sp_sfc_item = sp_sfc.FirstOrDefault(p => p.MatchId == item.MatchId);

                if (gameType.ToLower() == "hh")
                {
                    if (sp_sfc_item != null && sp_sfc_item.SFC != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_SFC_SPInfo>(sp_sfc_item.SFC);
                        info.GuestWin1_5 = json.GuestWin1_5;
                        info.GuestWin11_15 = json.GuestWin11_15;
                        info.GuestWin16_20 = json.GuestWin16_20;
                        info.GuestWin21_25 = json.GuestWin21_25;
                        info.GuestWin26 = json.GuestWin26;
                        info.GuestWin6_10 = json.GuestWin6_10;

                        info.HomeWin1_5 = json.HomeWin1_5;
                        info.HomeWin11_15 = json.HomeWin11_15;
                        info.HomeWin16_20 = json.HomeWin16_20;
                        info.HomeWin21_25 = json.HomeWin21_25;
                        info.HomeWin26 = json.HomeWin26;
                        info.HomeWin6_10 = json.HomeWin6_10;
                        //info.PrivilegesType = json.PrivilegesType == null ? string.Empty : json.PrivilegesType;
                    }
                }
                else
                {
                    if (sp_sfc_item != null)
                    {
                        info.GuestWin1_5 = sp_sfc_item.GuestWin1_5;
                        info.GuestWin11_15 = sp_sfc_item.GuestWin11_15;
                        info.GuestWin16_20 = sp_sfc_item.GuestWin16_20;
                        info.GuestWin21_25 = sp_sfc_item.GuestWin21_25;
                        info.GuestWin26 = sp_sfc_item.GuestWin26;
                        info.GuestWin6_10 = sp_sfc_item.GuestWin6_10;

                        info.HomeWin1_5 = sp_sfc_item.HomeWin1_5;
                        info.HomeWin11_15 = sp_sfc_item.HomeWin11_15;
                        info.HomeWin16_20 = sp_sfc_item.HomeWin16_20;
                        info.HomeWin21_25 = sp_sfc_item.HomeWin21_25;
                        info.HomeWin26 = sp_sfc_item.HomeWin26;
                        info.HomeWin6_10 = sp_sfc_item.HomeWin6_10;
                        //info.PrivilegesType = sp_sfc_item.PrivilegesType == null ? string.Empty : sp_sfc_item.PrivilegesType;
                    }
                }

                #endregion

                #region 附加大小分sp数据

                var sp_dxf_item = sp_dxf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (gameType.ToLower() == "hh")
                {
                    if (sp_dxf_item != null && sp_dxf_item.DXF != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_DXF_SPInfo>(sp_dxf_item.DXF);
                        info.DF = json.DF;
                        info.XF = json.XF;
                        info.YSZF = json.YSZF;
                        //info.PrivilegesType = json.PrivilegesType == null ? string.Empty : json.PrivilegesType;
                    }
                }
                else
                {
                    if (sp_dxf_item != null)
                    {
                        info.DF = sp_dxf_item.DF;
                        info.XF = sp_dxf_item.XF;
                        info.YSZF = sp_dxf_item.YSZF;
                        //info.PrivilegesType = sp_dxf_item.PrivilegesType == null ? string.Empty : sp_dxf_item.PrivilegesType;
                    }
                }

                #endregion

                list.Add(info);
            }
            return list;
        }
        /// <summary>
        /// 获取混合单关数据
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="matchDate"></param>
        /// <param name="isLeftJoin"></param>
        /// <returns></returns>
        public static List<JCLQ_MatchInfo_WEB> GetJCLQHHDGList()
        {
            BettingHelper bizHelper = new BettingHelper();
            var match = bizHelper.GetMatchInfoList<JCLQHHDGBase>(GetFilePath("New/Match_HHDG_List"));
            var list = new List<JCLQ_MatchInfo_WEB>();
            match = match.Where(t => long.Parse(Convert.ToDateTime(t.FSStopBettingTime).ToString("yyyyMMddHHmmss")) > long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))).ToList();
            if (match != null && match.Count > 0)
            {
                foreach (var item in match)
                {
                    var matchDataTime = ConvertHelper.ConvertStrToDateTime("20" + item.MatchData);
                    var info = new JCLQ_MatchInfo_WEB()
                    {
                        //CreateTime = item.CreateTime.ToString("yyyyMMddHHmmss"),
                        //DSStopBettingTime = item.DSStopBettingTime.ToString("yyyyMMddHHmmss"),
                        MatcheDateTime = matchDataTime,
                        //FSStopBettingTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.FSStopBettingTime)).ToString(),
                        FSStopBettingTime = item.FSStopBettingTime,
                        //FSStopBettingTime = item.FSStopBettingTime.ToString("yyyyMMddHHmmss"),
                        //GuestTeamId = item.GuestTeamId,
                        GuestTeamName = item.GuestTeamName,
                        HomeTeamName = item.HomeTeamName,
                        //HomeTeamId = item.HomeTeamId,
                        LeagueColor = item.LeagueColor.Contains("#") ? item.LeagueColor : "#" + "DB7917",
                        //LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        MatchIdName = item.MatchIdName,
                        //StartDateTime = bizHelper.ConvertDateTimeInt(Convert.ToDateTime(item.StartDateTime)).ToString(),
                        StartDateTime = Convert.ToDateTime(item.StartDateTime),
                        //StartDateTime = item.StartDateTime.ToString("yyyyMMddHHmmss"),
                        MatchData = item.MatchData,
                        MatchId = item.MatchId,
                        MatchNumber = item.MatchNumber,
                        //AverageLose = item.AverageLose,
                        //AverageWin = item.AverageWin,
                        //Mid = item.Mid,
                        FXId = item.FXId,
                        State_HHDG = item.State,
                        PrivilegesType = item.PrivilegesType == null ? string.Empty : item.PrivilegesType,
                    };

                    #region 附加胜负sp数据

                    if (item.SF != null)
                    {
                        var sfcjson = JsonHelper.Deserialize<JCLQ_SF_SPInfo>(item.SF);
                        if (sfcjson != null)
                        {
                            info.SF_WinSP = sfcjson.WinSP;
                            info.SF_LoseSP = sfcjson.LoseSP;
                        }
                    }
                    #endregion


                    #region 附加让分胜负sp数据
                    if (item.RFSF != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_RFSF_SPInfo>(item.RFSF);
                        if (json != null)
                        {
                            info.RF = json.RF;
                            info.RF_LoseSP = json.LoseSP;
                            info.RF_WinSP = json.WinSP;
                        }
                    }
                    #endregion

                    #region 附加胜分差sp数据

                    if (item.SFC != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_SFC_SPInfo>(item.SFC);
                        if (json != null)
                        {
                            info.GuestWin1_5 = json.GuestWin1_5;
                            info.GuestWin11_15 = json.GuestWin11_15;
                            info.GuestWin16_20 = json.GuestWin16_20;
                            info.GuestWin21_25 = json.GuestWin21_25;
                            info.GuestWin26 = json.GuestWin26;
                            info.GuestWin6_10 = json.GuestWin6_10;

                            info.HomeWin1_5 = json.HomeWin1_5;
                            info.HomeWin11_15 = json.HomeWin11_15;
                            info.HomeWin16_20 = json.HomeWin16_20;
                            info.HomeWin21_25 = json.HomeWin21_25;
                            info.HomeWin26 = json.HomeWin26;
                            info.HomeWin6_10 = json.HomeWin6_10;
                        }
                    }

                    #endregion

                    #region 附加大小分sp数据

                    if (item.DXF != null)
                    {
                        var json = JsonHelper.Deserialize<JCLQ_DXF_SPInfo>(item.DXF);
                        if (json != null)
                        {
                            info.DF = json.DF;
                            info.XF = json.XF;
                            info.YSZF = json.YSZF;
                        }
                    }

                    #endregion

                    list.Add(info);
                }
            }
            return list;
        }


        /*
         [12:10:06] ios app: 索引7位是1111，1标识未开玩法，0表示开售玩法，第18位，1000表示是否开单关。0标识开单关
[12:10:29] ios app: 竞彩篮球标记顺序：胜负  让分胜负  大小分  胜分差
         * SF是1
            RFSF是2
            SFC是3
            DXF 是4
         */
        /// <summary>
        /// 转换JCLQjsoN
        /// </summary>
        /// <returns></returns>
        public static string GetJson(string gametype, string matchDate = null)
        {
            var jcLq = GetJCLQHHDGList();
            //var jcLq = MatchList_WEB(gametype, matchDate);
            StringBuilder json = new StringBuilder("[");
            string index7 = "";
            foreach (var item in jcLq)
            {
                index7 = "";
                json.Append("[");
                json.AppendFormat("\"{0}\",", item.MatchId);
                json.AppendFormat("\"{0}{1}\",", BettingHelper.Week(), item.MatchNumber);
                json.AppendFormat("\"{0}\",", item.LeagueName);
                json.AppendFormat("\"{0}\",", item.HomeTeamName);
                json.AppendFormat("\"{0}\",", item.GuestTeamName);
                json.AppendFormat("\"{0}\",", item.StartDateTime);
                json.AppendFormat("\"{0}\",", item.FSStopBettingTime);
                if (item.SF_WinSP == 0 || item.SF_LoseSP == 0)
                {
                    index7 += "1";
                }
                else
                {
                    index7 += "0";
                }
                if(item.RF_WinSP==0||item.RF_LoseSP==0)
                {
                    index7 += "1";
                }
                else
                {
                    index7 += "0";
                }
                if (item.HomeWin1_5 == 0)
                {
                    index7 += "1";
                }
                else
                {
                    index7 += "0";
                }
                if (item.DF == 0 || item.XF == 0)
                {
                    index7 += "1";
                }
                else
                {
                    index7 += "0";
                }
                json.AppendFormat("\"{0}\",",index7);
                //json.Append("\"00000\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                //json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"-\",");
                json.Append("\"\",");
                json.Append("\"\",");
                json.Append("\"[");
                //非让分
                json.AppendFormat("[{0},{1}],", item.SF_WinSP, item.SF_LoseSP);
                //让分
                json.AppendFormat("[{0},{1},{2}],", item.RF, item.RF_WinSP, item.RF_LoseSP);
                //大小分
                json.AppendFormat("[{0},{1},{2}],", item.YSZF, item.DF, item.XF);
                //身负差
                json.AppendFormat("[{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}],", item.HomeWin1_5, item.HomeWin6_10, item.HomeWin11_15, item.HomeWin16_20, item.HomeWin21_25, item.HomeWin26, item.GuestWin1_5, item.GuestWin6_10, item.GuestWin11_15, item.GuestWin16_20, item.GuestWin21_25, item.GuestWin26);
                json.Append("[],[],[],[]]\",");
                json.AppendFormat("\"{0}\",", BettingHelper.GetLeagueColor());
                json.AppendFormat("\"{0}\",", item.MatchData);
                if (item.State_HHDG == "234")
                {
                    json.Append("\"1000\"");
                }
                else if (item.State_HHDG == "3")
                {
                    json.Append("\"1101\"");
                }
                else
                {
                    json.Append("\"1101\"");
                }
                //switch (item.State_HHDG)
                //{
                //    case "1"://SF是1
                //        json.Append("\"0101\"");
                //        break;
                //    case "2"://RFSF是2
                //        json.Append("\"1001\"");
                //        break;
                //    case "3"://SFC是3
                //        json.Append("\"1101\"");
                //        break;
                //    case "4"://DXF 是4
                //        json.Append("\"1100\"");
                //        break;
                //    default:
                //        json.Append("\"1101\"");
                //        break;
                  
                //}
                //json.Append("\"1000\"");
                json.Append("],");
            }
            string data = json.ToString().TrimEnd(',');
            //json.Append("]");
            return data + "]";
        }
    }
}