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
    /// Odds赔率信息 - json文件读取管理
    /// </summary>
    public static class Json_Odds
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
        /// 竞彩足球赔率文件
        /// </summary>
        /// <param name="oddtype">选择赔率种类</param>
        /// <param name="issuseNumber">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string oddtype, string game = "jczq", string issuseNumber = null)
        {
            if (string.IsNullOrEmpty(issuseNumber))
            {
                return Service.MapPath(MatchRoot + "/" + game + "/" + oddtype + "_SP.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/" + game + "/" + issuseNumber + "/" + oddtype + "_SP.json");
            }
        }

        /// <summary>
        /// 竞彩足球赔率趋势文件
        /// </summary>
        /// <param name="oddtype">选择赔率种类</param>
        /// <param name="mid">对应的MID编号</param>
        /// <param name="issuseNumber">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPTrendFile(string oddtype, string mid, string game = "jczq", string issuseNumber = null)
        {
            if (string.IsNullOrEmpty(issuseNumber))
            {
                return Service.MapPath(MatchRoot + "/" + game + "/" + oddtype + "_" + mid + "_sp.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/" + game + "/" + issuseNumber + "/" + oddtype + "_" + mid + "_sp.json");
            }
        }

        #endregion

        #region 赔率数据
        /// <summary>
        /// 竞彩足球 - 赔率选择信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">选择赔率种类</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<JCZQ_SPF_OZ_SPInfo> oddslist(HttpServerUtilityBase service, string oddtype)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile(oddtype, "jczq"));
                return JsonSerializer.Deserialize<List<JCZQ_SPF_OZ_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_SPF_OZ_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 赔率趋势信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">选择赔率种类</param>
        /// <param name="mid">对应的MID编号</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<JCZQ_SPF_SP_Trend> trendlist(HttpServerUtilityBase service, string oddtype, string mid)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile(oddtype, mid, "jczq"));
                return JsonSerializer.Deserialize<List<JCZQ_SPF_SP_Trend>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_SPF_SP_Trend>();
            }
        }

        /// <summary>
        /// 北京单场 - 赔率选择信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">选择赔率种类</param>
        /// <param name="issuseNumer">期号</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<BJDC_SPF_OZ_SPInfo> oddslist(HttpServerUtilityBase service, string oddtype, string issuseNumer)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile(oddtype, "bjdc", issuseNumer));
                return JsonSerializer.Deserialize<List<BJDC_SPF_OZ_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SPF_OZ_SPInfo>();
            }
        }

        /// <summary>
        /// 北京单场 - 赔率趋势信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="oddtype">选择赔率种类</param>
        /// <param name="mid">对应的MID编号</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<BJDC_SPF_SP_Trend> trendlist(HttpServerUtilityBase service, string oddtype, string mid, string issuseNumer)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile(oddtype, mid, "bjdc", issuseNumer));
                return JsonSerializer.Deserialize<List<BJDC_SPF_SP_Trend>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SPF_SP_Trend>();
            }
        }
        #endregion
    }
}