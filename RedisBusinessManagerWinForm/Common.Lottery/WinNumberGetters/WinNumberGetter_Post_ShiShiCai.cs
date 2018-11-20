﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using Common.JSON;
using System.Net;

namespace Common.Lottery.WinNumberGetters
{
    class WinNumberGetter_Post_ShiShiCai : WinNumberGetter
    {
        //重庆时时彩
        private const string urlCQSSC = @"http://www.shishicai.cn/KAjax.ashx?method=SpeedAjax_GetRecentBetRecord&randomStr=14821";
        //福彩3D
        private const string urlFC3D = @"http://baidu.lehecai.com/lottery/draw/view/52";
        //广东十一选五
        private const string urlGD11X5 = @"http://baidu.lehecai.com/lottery/draw/view/23";
        //江西十一选五
        private const string urlJX11X5 = @"http://baidu.lehecai.com/lottery/draw/view/22";
        //江西时时彩
        private const string urlJXSSC = @"http://baidu.lehecai.com/lottery/draw/view/202";
        //排列三
        private const string urlPL3 = @"http://baidu.lehecai.com/lottery/draw/view/3";
        //山东十一选五
        private const string urlSD11X5 = @"http://baidu.lehecai.com/lottery/draw/view/20";
        //山东群英会
        private const string urlSDQYH = @"http://baidu.lehecai.com/lottery/draw/view/517";
        //广东快乐十分
        private const string urlGDKLSF = @"http://baidu.lehecai.com/lottery/draw/view/544";
        //广西快乐十分
        private const string urlGXKLSF = @"http://baidu.lehecai.com/lottery/draw/view/545";

        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount, string issuseNumber)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string json = this.GetJson(gameName);

            switch (gameName)
            {
                case "CQSSC":
                    result = GetCQSSCWinNumber(json, lastIssuseCount);
                    break;
                case "FC3D":
                    result = GetFC3DWinNumber(json, lastIssuseCount);
                    break;
                case "GD11X5":
                    result = GetGD11X5WinNumber(json, lastIssuseCount);
                    break;
                case "JX11X5":
                    result = GetJX11X5WinNumber(json, lastIssuseCount);
                    break;
                case "JXSSC":
                    result = GetJXSSCWinNumber(json, lastIssuseCount);
                    break;
                case "PL3":
                    result = GetPL3WinNumber(json, lastIssuseCount);
                    break;
                case "SD11X5":
                    result = GetSD11X5WinNumber(json, lastIssuseCount);
                    break;
                case "SDQYH":
                    result = GetSDQYHWinNumber(json, lastIssuseCount);
                    break;
                case "GDKLSF":
                    result = GetGDKLSFWinNumber(json, lastIssuseCount);
                    break;
                case "GXKLSF":
                    result = GetGXKLSFWinNumber(json, lastIssuseCount);
                    break;
            }

            result = this.FormatWinNumber(gameName, result);

            return result;
        }

        /// <summary>
        /// 重庆时时彩
        /// </summary>
        private Dictionary<string, string> GetCQSSCWinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 福彩3D
        /// </summary>
        private Dictionary<string, string> GetFC3DWinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson2(json, lastIssuseCount);
        }

        /// <summary>
        /// 广东十一选五
        /// </summary>
        private Dictionary<string, string> GetGD11X5WinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 江西十一选五
        /// </summary>
        private Dictionary<string, string> GetJX11X5WinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 江西时时彩
        /// </summary>
        private Dictionary<string, string> GetJXSSCWinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 排列三
        /// </summary>
        private Dictionary<string, string> GetPL3WinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson2(json, lastIssuseCount);
        }

        /// <summary>
        /// 山东十一选五
        /// </summary>
        private Dictionary<string, string> GetSD11X5WinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 山东群英会
        /// </summary>
        private Dictionary<string, string> GetSDQYHWinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 广东快乐十分
        /// </summary>
        private Dictionary<string, string> GetGDKLSFWinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        /// 广西快乐十分
        /// </summary>
        private Dictionary<string, string> GetGXKLSFWinNumber(string json, int lastIssuseCount)
        {
            return this.GetWinNumberFromJson(json, lastIssuseCount);
        }
        
        private string GetJson(string gameName)
        {
            string json = string.Empty;
            string strHost = "www.shishicai.cn";
            string strRefer = "http://www.shishicai.cn/lottery/speed/fccqssc/base.aspx";
            string strUrl = "http://www.shishicai.cn/KAjax.ashx?method=SpeedAjax_GetTodayBonus&randomStr=1482125";
            string strPostData = "data=%7B%22CommandName%22%3A%22SpeedAjax%22%2C%22Parameters%22%3A%5B%22%7B%5C%22CommandName%5C%22%3A%5C%22GetTodayBonus%5C%22%2C%5C%22Parameters%5C%22%3A%5B4%2C0%2C%5C%22%5B%5D%5C%22%5D%7D%22%5D%7D";
            CookieContainer cc = PostManager.GetCookie("http://www.shishicai.cn/lottery/speed/fccqssc/base.aspx");

            switch (gameName)
            {
                case "GDKLSF":
                    
                    break;
                case "GXKLSF":
                    
                    break;
                case "CQSSC":
                    
                    json = PostManager.Post(strHost, strRefer, strUrl, strPostData, cc);
                    break;
                case "FC3D":
                    
                    break;
                case "GD11X5":
                    
                    break;
                case "JX11X5":
                    
                    break;
                case "JXSSC":
                    
                    break;
                case "PL3":
                    
                    break;
                case "SD11X5":
                    
                    break;
                case "SDQYH":
                    
                    break;
            }

            return json;
        }

        private string GetJsonForPL3(string html)
        {
            string json = html;

            string strStart = "var phaseData = ";
            int index = json.IndexOf(strStart) + strStart.Length;
            json = json.Substring(index, json.Length - index);
            index = json.IndexOf("}};") + 2;
            json = json.Substring(0, index);

            return json;
        }

        private Dictionary<string, string> GetWinNumberFromJson(string json, int total)
        {
            Dictionary<string, string> dicWinNumber = new Dictionary<string, string>();

            Dictionary<string, string> dicRoot = JsonParse.GetStrDictionary(json);

            int i = 1;

            //该部分和网站返回的json数据的层次有关系
            foreach (var root in dicRoot)
            {
                //年月日
                string strNumbers = root.Value.ToString();

                Dictionary<string, string> dicNumbers = JsonParse.GetStrDictionary(strNumbers);

                foreach (var number in dicNumbers)
                {
                    string strIssue = number.Key.ToString();

                    string strResult = JsonParse.GetNode("result", number.Value.ToString());
                    string strRed = JsonParse.GetNode("red", strResult);

                    strRed = strRed.Replace("[", "").Replace("]", "");

                    dicWinNumber.Add(strIssue, strRed);

                    if (i >= total)
                    {
                        break;
                    }

                    i++;
                }

                if (i >= total)
                {
                    break;
                }
            }

            return dicWinNumber;
        }

        private Dictionary<string, string> GetWinNumberFromJson2(string json, int total)
        {
            //取福彩3D和排列3

            Dictionary<string, string> dicWinNumber = new Dictionary<string, string>();

            Dictionary<string, string> dicRoot = JsonParse.GetStrDictionary(json);

            int i = 1;

            //该部分和网站返回的json数据的层次有关系
            foreach (var root in dicRoot)
            {
                string strIssue = root.Key.ToString();

                string strNumbers = root.Value.ToString();

                string strResult = JsonParse.GetNode("result", strNumbers);
                string strRed = JsonParse.GetNode("red", strResult);

                strRed = strRed.Replace("[", "").Replace("]", "");

                dicWinNumber.Add(strIssue, strRed);

                if (i >= total)
                {
                    break;
                }

                i++;
            }

            return dicWinNumber;
        }

        private Dictionary<string, string> FormatWinNumber(string gameName, Dictionary<string, string> dic)
        {
            Dictionary<string, string> dicResult = new Dictionary<string, string>();

            foreach (var result in dic)
            {
                string strIssue = result.Key;
                string numbers = result.Value.ToString();

                switch (gameName)
                {
                    case "GDKLSF":
                        strIssue = strIssue.Insert(8, "-");//20111215-76
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "GXKLSF":
                        strIssue = strIssue.Insert(7, "-");//2011342-50
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "CQSSC":
                        strIssue = strIssue.Insert(8, "-");//20111215-094
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "FC3D":
                        strIssue = strIssue.Insert(4, "-");//2011-342
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "GD11X5":
                        strIssue = ("20" + strIssue).Insert(8, "-");//20111215-64
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "JX11X5":
                        strIssue = strIssue.Insert(8, "-");//20111215-64
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "JXSSC":
                        strIssue = strIssue.Remove(8, 1).Insert(8, "-");//20111215-75
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "PL3":
                        strIssue = ("20" + strIssue).Insert(4, "-");//2011-342
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "SD11X5":
                        strIssue = ("20" + strIssue).Insert(8, "-");//20111215-77
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "SDQYH":
                        strIssue = strIssue.Remove(8, 1).Insert(8, "-");//20111215-39 
                        numbers = numbers.Replace("\"", "");
                        break;
                }

                dicResult.Add(strIssue, numbers);
            }

            return dicResult;
        }
    }
}
