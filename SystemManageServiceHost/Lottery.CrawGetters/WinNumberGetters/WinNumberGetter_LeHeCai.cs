using System.Collections.Generic;
using System.Text;
using Common.JSON;
using Common.Net;

namespace Lottery.CrawGetters
{
    internal class WinNumberGetter_LeHeCai : WinNumberGetter
    {
        //重庆时时彩
        private const string urlCQSSC = @"http://baidu.lehecai.com/lottery/draw/view/200";

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

        //北京赛车 or 幸运赛车
        private const string urlBJSC = @"http://baidu.lecai.com/lottery/draw/view/563";


        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var url = GetUrl(gameName);
            var result = new Dictionary<string, string>();
            var html = PostManagerWithProxy.Get(url, Encoding.UTF8);
            var json = GetJson(html);

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
                    json = GetJsonForPL3(html);
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
                case "BJSC":
                    result = GetBJSCWinNumber(json, lastIssuseCount);
                    break;
            }

            result = FormatWinNumber(gameName, result);

            return result;
        }

        /// <summary>
        ///     重庆时时彩
        /// </summary>
        private Dictionary<string, string> GetCQSSCWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     福彩3D
        /// </summary>
        private Dictionary<string, string> GetFC3DWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson2(json, lastIssuseCount);
        }

        /// <summary>
        ///     广东十一选五
        /// </summary>
        private Dictionary<string, string> GetGD11X5WinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     江西十一选五
        /// </summary>
        private Dictionary<string, string> GetJX11X5WinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     江西时时彩
        /// </summary>
        private Dictionary<string, string> GetJXSSCWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     排列三
        /// </summary>
        private Dictionary<string, string> GetPL3WinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson2(json, lastIssuseCount);
        }

        /// <summary>
        ///     山东十一选五
        /// </summary>
        private Dictionary<string, string> GetSD11X5WinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     山东群英会
        /// </summary>
        private Dictionary<string, string> GetSDQYHWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     广东快乐十分
        /// </summary>
        private Dictionary<string, string> GetGDKLSFWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     广西快乐十分
        /// </summary>
        private Dictionary<string, string> GetGXKLSFWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        /// <summary>
        ///     北京赛车
        /// </summary>
        private Dictionary<string, string> GetBJSCWinNumber(string json, int lastIssuseCount)
        {
            return GetWinNumberFromJson(json, lastIssuseCount);
        }

        private string GetUrl(string gameName)
        {
            var url = string.Empty;

            switch (gameName)
            {
                case "GDKLSF":
                    url = urlGDKLSF;
                    break;
                case "GXKLSF":
                    url = urlGXKLSF;
                    break;
                case "CQSSC":
                    url = urlCQSSC;
                    break;
                case "FC3D":
                    url = urlFC3D;
                    break;
                case "GD11X5":
                    url = urlGD11X5;
                    break;
                case "JX11X5":
                    url = urlJX11X5;
                    break;
                case "JXSSC":
                    url = urlJXSSC;
                    break;
                case "PL3":
                    url = urlPL3;
                    break;
                case "SD11X5":
                    url = urlSD11X5;
                    break;
                case "SDQYH":
                    url = urlSDQYH;
                    break;
                case "BJSC":
                    url = urlBJSC;
                    break;
            }

            return url;
        }

        private string GetJson(string html)
        {
            var json = html;

            var strStart = "var phaseData = ";
            var index = json.IndexOf(strStart) + strStart.Length;
            json = json.Substring(index, json.Length - index);
            index = json.IndexOf("}}};") + 3;
            json = json.Substring(0, index);

            return json;
        }

        private string GetJsonForPL3(string html)
        {
            var json = html;

            var strStart = "var phaseData = ";
            var index = json.IndexOf(strStart) + strStart.Length;
            json = json.Substring(index, json.Length - index);
            index = json.IndexOf("}};") + 2;
            json = json.Substring(0, index);

            return json;
        }

        private Dictionary<string, string> GetWinNumberFromJson(string json, int total)
        {
            var dicWinNumber = new Dictionary<string, string>();

            var dicRoot = JsonParse.GetStrDictionary(json);

            var i = 1;

            //该部分和网站返回的json数据的层次有关系
            foreach (var root in dicRoot)
            {
                //年月日
                var strNumbers = root.Value;

                var dicNumbers = JsonParse.GetStrDictionary(strNumbers);

                foreach (var number in dicNumbers)
                {
                    var strIssue = number.Key;

                    var strResult = JsonParse.GetNode("result", number.Value);
                    var strRed = JsonParse.GetNode("red", strResult);

                    strRed = strRed.Replace("[", "").Replace("]", "");

                    dicWinNumber.Add(strIssue, strRed);

                    if (i >= total)
                        break;

                    i++;
                }

                if (i >= total)
                    break;
            }

            return dicWinNumber;
        }

        private Dictionary<string, string> GetWinNumberFromJson2(string json, int total)
        {
            //取福彩3D和排列3

            var dicWinNumber = new Dictionary<string, string>();

            var dicRoot = JsonParse.GetStrDictionary(json);

            var i = 1;

            //该部分和网站返回的json数据的层次有关系
            foreach (var root in dicRoot)
            {
                var strIssue = root.Key;

                var strNumbers = root.Value;

                var strResult = JsonParse.GetNode("result", strNumbers);
                var strRed = JsonParse.GetNode("red", strResult);

                strRed = strRed.Replace("[", "").Replace("]", "");

                dicWinNumber.Add(strIssue, strRed);

                if (i >= total)
                    break;

                i++;
            }

            return dicWinNumber;
        }

        private Dictionary<string, string> FormatWinNumber(string gameName, Dictionary<string, string> dic)
        {
            var dicResult = new Dictionary<string, string>();

            foreach (var result in dic)
            {
                var strIssue = result.Key;
                var numbers = result.Value;

                switch (gameName)
                {
                    case "GDKLSF":
                    case "JX11X5":
                    case "CQSSC":
                        strIssue = strIssue.Insert(8, "-"); //20111215-76
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "GXKLSF":
                        strIssue = strIssue.Insert(7, "-"); //2011342-50
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "FC3D":
                        strIssue = strIssue.Insert(4, "-"); //2011-342
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "GD11X5":
                    case "SD11X5":
                        strIssue = ("20" + strIssue).Insert(8, "-"); //20111215-64
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "JXSSC":
                    case "SDQYH":
                        strIssue = strIssue.Remove(8, 1).Insert(8, "-"); //20111215-75
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "PL3":
                        strIssue = ("20" + strIssue).Insert(4, "-"); //2011-342
                        numbers = numbers.Replace("\"", "");
                        break;
                    case "BJSC":
                        strIssue = strIssue.Insert(6, "-"); //2011-342
                        numbers = numbers.Replace("\"", "");
                        break;
                }

                dicResult.Add(strIssue, numbers);
            }

            return dicResult;
        }
    }
}