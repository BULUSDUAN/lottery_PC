using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;
using Common.JSON;
using System.Configuration;
using System.IO;
using Common.XmlAnalyzer;

namespace app.lottery.site.jsonManager
{
    /// <summary>
    /// 队伍对阵历史信息 - json文件读取管理
    /// </summary>
    public static class Json_TeamH
    {
        #region 属性
        /// <summary>
        /// 数据存放物理路径根目录
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
        /// 队伍对阵历史信息文件
        /// </summary>
        /// <param name="teamid">队伍信息</param>
        /// <param name="matchdate">奖期</param>
        /// <param name="game">彩种，现支持：jczq bjdc</param>
        /// <returns>SP文件地址</returns>
        private static string TeamHistoryFile(string teamid, string matchdate, string game)
        {

            return Service.MapPath(MatchRoot + "/" + game + "/" + matchdate + "/TeamHistory_" + teamid + ".json");
        }
        #endregion

        /// <summary>
        /// 竞彩足球 - 队伍对阵历史信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="teamid">选择赔率种类</param>
        /// <param name="matchdate">奖期</param>
        /// <returns>竞彩足球-队伍对阵历史信息</returns>
        public static List<JCZQ_Team_History> TeamHistory_JCZQ(HttpServerUtilityBase service, string teamid, string matchdate)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(TeamHistoryFile(teamid, matchdate, "jczq"));
                return JsonSerializer.Deserialize<List<JCZQ_Team_History>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_Team_History>();
            }
        }

        /// <summary>
        /// 北单 - 队伍对阵历史信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="teamid">选择赔率种类</param>
        /// <param name="matchdate">奖期</param>
        /// <returns>北单-队伍对阵历史信息</returns>
        public static List<BJDC_Team_History> TeamHistory_BJDC(HttpServerUtilityBase service, string teamid, string matchdate)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(TeamHistoryFile(teamid, matchdate, "bjdc"));
                return JsonSerializer.Deserialize<List<BJDC_Team_History>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_Team_History>();
            }
        }
    }
}