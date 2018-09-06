using Lottery.CrawTool.Net;
using System;
using System.Collections.Generic;
using System.Text;


namespace Lottery.CrawGetters
{
    /// <summary>
    ///     广福网
    /// </summary>
    public class WinNumberGetter_GFW : WinNumberGetter
    {
        private const string gd11x5_url =
            "http://www.egdfc.com/ltrapi/Commlott/v1/GetHisAwardNum.aspx?ltype=9&year={0}&page=1";

        private const string gdklsf_url =
            "http://www.egdfc.com/ltrapi/Commlott/v1/GetHisAwardNum.aspx?ltype=8&year={0}&page=1";

        private const string sdqyh_url =
            "http://www.esdcp.cn/ltrapi/Commlott/v1/GetHisAwardNum.aspx?ltype=5&year={0}&page=1";

        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
            string issuseNumber1)
        {
            var dic = new Dictionary<string, string>();

            var url = "";
            switch (gameCode.ToUpper())
            {
                case "GD11X5":
                    url = string.Format(gd11x5_url, DateTime.Now.Year);
                    break;
                case "GDKLSF":
                    url = string.Format(gdklsf_url, DateTime.Now.Year);
                    break;
                case "SDQYH":
                    url = string.Format(sdqyh_url, DateTime.Now.Year);
                    break;
            }
            if (string.IsNullOrEmpty(url))
                return dic;

            var html = PostManagerWithProxy.Post(url, string.Empty, Encoding.Default, null);

            var htmlArray = html.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var rows = htmlArray[9].Trim().Replace(" class=\"ntd\"", "")
                .Split(new[] {"</tr>"}, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < lastIssuseCount; i++)
            {
                var tdArray = rows[i].Replace("<tr>", "").Split(new[] {"</td>"}, StringSplitOptions.RemoveEmptyEntries);
                var issuseNumber = string.Empty;
                var winNumber = string.Empty;
                for (var j = 0; j < 2; j++)
                {
                    var td = tdArray[j].Replace("<td>", "");
                    if (string.IsNullOrEmpty(issuseNumber))
                    {
                        if (gameCode == "SDQYH")
                            issuseNumber = string.Format("20{0}-{1}", td.Substring(0, 6), td.Substring(8, 2));
                        else
                            issuseNumber = td.Substring(0, 8) + "-" + td.Substring(8);
                        continue;
                    }
                    winNumber = td.Replace("|", ",");
                }
                dic.Add(issuseNumber, winNumber);
            }
            return dic;
        }
    }
}