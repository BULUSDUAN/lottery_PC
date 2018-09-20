using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using app.lottery.site.Controllers;
using Common.JSON;
using Common.Net;
using System.Text;
using System.Globalization;
using Common.Log;
using app.lottery.site.iqucai;

namespace app.lottery.site.cbbao.Models
{
    public class BusinessHelper
    {
        string ResUrl = string.Empty;
        public BusinessHelper()
        {
            //http://res.wancai.com/matchdata/jczq/match_list.json?_=1425952243703
            ResUrl = ConfigurationManager.AppSettings["DataJsonSiteUrl"] ?? "http://data.wancai.com/";
        }
        private static ILogWriter log = Common.Log.LogWriterGetter.GetLogWriter();
        public void WriteLog(string txt)
        {
            log.Write("LotteryService.", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "玩彩网页", txt);
        }
        public static string GetDomain()
        {
            string strUrl = ConfigurationManager.AppSettings["SelfDomain"] ?? "http://www.wancai.com";
            return strUrl;
        }
        public  List<T> GetMatchInfoList<T>(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (result == null||string.IsNullOrEmpty(result))
                return new List<T>();
            return JsonSerializer.Deserialize<List<T>>(result);
        }

        private string ReadFileString(string fullUrl)
        {
            try
            {
                string strResult = PostManager.Get(fullUrl, Encoding.UTF8);
                if (strResult == "404") return string.Empty;

                if (!string.IsNullOrEmpty(strResult))
                {
                    strResult = strResult.Replace("\\\"", "\"").Replace("\"{", "{").Replace("}\"", "}");
                    if (strResult.ToLower().StartsWith("var"))
                    {
                        string[] strArray = strResult.Split('=');
                        if (strArray != null && strArray.Length == 2)
                        {
                            if (strArray[1].ToString().Trim().EndsWith(";"))
                            {
                                return strArray[1].ToString().Trim().TrimEnd(';');
                            }
                            return strArray[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
        public string QueryCoreConfigByKey(string key)
        {
            var result = WCFClients.GameClient.QueryCoreConfigByKey(key).ConfigValue;
            if (string.IsNullOrEmpty(result))
                return string.Empty;
            return result;
        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        public DateTime ConvertStrToDateTime(string strTime)
        {
            IFormatProvider ifp = new CultureInfo("zh-CN", true);
            var time = DateTime.ParseExact(strTime, "yyyyMMdd", ifp);
            return time;
        }


    }
}