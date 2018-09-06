using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Lottery.Gateway.ZhongMin;

namespace Lottery.CrawGetters
{
    /// <summary>
    ///     中民查询开奖结果
    /// </summary>
    internal class WinNumberGetter_ZhongMin : WinNumberGetter
    {
        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
            string issuseNumber)
        {
            //采集中民的开奖号码
            var dic = new Dictionary<string, string>();

            //"http://121.12.168.124:661/ticketinterface.aspx";
            GatewayHandler.ServiceUrl = ConfigurationManager.AppSettings["ZHM_ServiceUrl"];
            GatewayHandler.Key = ConfigurationManager.AppSettings["ZHM_Key"];
            GatewayHandler.PartnerId = ConfigurationManager.AppSettings["ZHM_PartnerId"];
            GatewayHandler.Version = ConfigurationManager.AppSettings["ZHM_Version"];

            var issuseNumberZM = GetZMIssuse(gameCode, issuseNumber);

            var gate = new GatewayHandler();
            var winNumber = gate.QueryWinNumber(gameCode, issuseNumberZM);

            var number =
                winNumber.Results.Result.Winumber.Split(new[] {",", "|"}, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < number.Length; i++)
                number[i] = number[i].Trim();
            var lasewinNumber = GetWinNumber(gameCode, winNumber.Results.Result.Winumber);

            dic.Add(issuseNumber, lasewinNumber);
            return dic;
        }

        private string GetZMIssuse(string gameCode, string issuse)
        {
            var LaseIssuse = string.Empty;
            switch (gameCode)
            {
                case "DLT":
                    LaseIssuse = issuse.Substring(2, 6).Replace("-", "");
                    break;
                case "PL3":
                    LaseIssuse = issuse.Substring(2, 6).Replace("-", "");
                    break;
                case "FC3D":
                case "SSQ":
                case "CQSSC":
                    LaseIssuse = issuse.Replace("-", "");
                    break;
                case "JX11X5":
                    LaseIssuse = issuse.Substring(2, 9).Replace("-", "");
                    break;
            }

            return LaseIssuse;
        }

        private string GetWinNumber(string gameCode, string winNuber)
        {
            var LasetWinNuber = string.Empty;
            switch (gameCode)
            {
                case "SSQ":
                    LasetWinNuber = winNuber.Replace(" ", "");
                    break;
                case "DLT":
                    LasetWinNuber = winNuber.Replace(" ", "").Replace("+", "|");
                    break;
                case "CQSSC":
                case "PL3":
                case "FC3D":
                    LasetWinNuber = winNuber.Replace(" ", ",");
                    break;
                case "JX11X5":
                    LasetWinNuber = winNuber.Replace(" ", ",");
                    break;
                default:
                    break;
            }
            return LasetWinNuber;
        }
    }
}