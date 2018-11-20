﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using Common.Net;

namespace Common.Lottery.WinNumberGetters
{
    /// <summary>
    /// 我中啦  接口   注意：没有江西11选5和广东11选5
    /// </summary>
    internal class WinNumberGetter_WoZhongLa : WinNumberGetter
    {
        private const string urlFormat = @"http://www.wozhongla.com/sp2/act/inter.info.action?wAgent=8848&wPassword=888888&wReturnFmt=2&&wAction=1014&wParam=lotId={0}_pageno=1_pagesize={1}_startIssue=_endIssue=&d={2}";
        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(gameName) || lastIssuseCount == 0)
                return dic;

            string lotId = string.Empty;
            switch (gameName)
            {
                case "FC3D":
                    lotId = "52";
                    break;
                case "PL3":
                    lotId = "33";
                    break;
                case "CQSSC":
                    lotId = "10401";
                    break;
                case "JXSSC":
                    lotId = "13001";
                    break;
                case "SD11X5":
                    lotId = "21406";
                    break;
                //case "JX11X5":
                //    lotId = "23009";
                //    break;
                //case "GD11X5":
                //    lotId = "23009";
                //    break;
            }
            if (string.IsNullOrEmpty(lotId))
                return dic;

            try
            {
                string url = string.Format(urlFormat, lotId, lastIssuseCount, DateTime.Now.Ticks);
                string json = PostManager.Get(url, Encoding.UTF8, 0);
                if (string.IsNullOrEmpty(json))
                    return dic;
                int pageParamsIndex = json.IndexOf("},");
                if (pageParamsIndex == -1)
                    return dic;
                json = json.Substring(pageParamsIndex + 2);
                var result = Deserialize(json);
                foreach (var item in result)
                {
                    dic.Add(FormatIssuseNumber(gameName, item.lotIssue), FormatWinNumber(gameName, item.kjCode));
                }
                return dic;
            }
            catch
            {
                return dic;
            }

        }

        private WinNumberInfo[] Deserialize(string json)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WinNumberInfo[]));
            var result = serializer.ReadObject(ms) as WinNumberInfo[];
            ms.Close();
            return result;
        }

        private string FormatIssuseNumber(string gameName, string issuseNumber)
        {
            switch (gameName)
            {
                case "FC3D":
                case "PL3":
                    //2011028格式
                    return issuseNumber.Insert(4, "-");
                case "CQSSC":
                    //110128084格式
                    return "20" + issuseNumber.Insert(6, "-");
                case "JXSSC":
                    //110128084格式
                    string issuse1 = "20" + issuseNumber.Substring(0, 6);
                    string issuse2 = issuseNumber.Substring(7, 2);
                    return issuse1 + "-" + issuse2;
                case "JX11X5":
                case "SD11X5":
                case "GD11X5":
                    //2011012918格式
                    return issuseNumber.Insert(8, "-");
                default:
                    return issuseNumber;
            }
        }

        private string FormatWinNumber(string gameName, string winNumber)
        {
            switch (gameName)
            {
                case "FC3D":
                case "PL3":
                case "CQSSC":
                case "JXSSC":
                    //123格式
                    var result = new List<string>();
                    foreach (var item in winNumber.ToCharArray())
                    {
                        result.Add(item.ToString());
                    }
                    return string.Join(",", result.ToArray());
                case "JX11X5":
                case "SD11X5":
                case "GD11X5":
                    //05 07 10 06 04 格式
                    return string.Join(",", winNumber.Split(' '));
                default:
                    return winNumber;
            }
        }
    }

    [DataContract]
    public class WinNumberInfo
    {
        [DataMember]
        public int bonusBalance { get; set; }
        [DataMember]
        public string endTime { get; set; }
        [DataMember]
        public string kjCode { get; set; }
        [DataMember]
        public int lotId { get; set; }
        [DataMember]
        public string lotIssue { get; set; }
        [DataMember]
        public int sale { get; set; }
        [DataMember]
        public string startTime { get; set; }
        [DataMember]
        public string winCount { get; set; }
        [DataMember]
        public string winMoney { get; set; }
        [DataMember]
        public string winName { get; set; }
    }


    public class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }


}
