using System;
using System.Collections.Generic;
using System.Linq;
using Common.Communication;
using Common.Lottery.Gateway.Liangcai;

namespace Lottery.CrawGetters
{
    /// <summary>
    ///     中民查询开奖结果
    /// </summary>
    internal class WinNumberGetter_Liangcai : WinNumberGetter
    {
        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
            string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var gaet = new GatewayHandler_LC();

            var issuse = string.Empty;
            if (!string.IsNullOrEmpty(issuseNumber))
                issuse = GetIssuse(gameCode, issuseNumber);

            var issuseWinNuber = gaet.QueryWinNumber(gameCode, issuse);
            var array = issuseWinNuber.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 2)
                throw new LogicException("期号与开奖号区分不一致");

            var winNumber = GetWinNumber(gameCode, array[1]);
            dic.Add(issuseNumber, winNumber);
            return dic;
        }

        private string GetIssuse(string gameCode, string issuse)
        {
            var LaseIssuse = string.Empty;
            switch (gameCode)
            {
                case "PL3":
                case "DLT":
                case "SSQ":
                case "FC3D":
                case "JX11X5":
                    LaseIssuse = issuse.Replace("-", "");
                    break;
                case "CQSSC":
                    LaseIssuse = issuse.Substring(2, 10).Replace("-", "");
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
                    var ssqWinNumber = winNuber.Split(new[] {"+"}, StringSplitOptions.RemoveEmptyEntries);
                    if (ssqWinNumber.Length != 2)
                        throw new LogicException("开奖号格式不正确！");

                    var ssqQan = ssqWinNumber[0].Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    if (ssqQan.Length != 6)
                        throw new LogicException("前区开奖号不正确！");

                    var ssqQian = string.Join(",", ssqQan.Take(6).ToArray());
                    LasetWinNuber = string.Join("|", ssqQian, ssqWinNumber[1]);
                    break;
                case "DLT":
                    var winNumber = winNuber.Split(new[] {"+"}, StringSplitOptions.RemoveEmptyEntries);
                    if (winNumber.Length != 2)
                        throw new LogicException("开奖号格式不正确！");

                    var dltQan = winNumber[0].Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    if (dltQan.Length != 5)
                        throw new LogicException("前区开奖号不正确！");
                    var dltQian = string.Join(",", dltQan.Take(5).ToArray());

                    var dltH = winNumber[1].Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    if (dltH.Length != 2)
                        throw new LogicException("后区开奖号不正确！");
                    var dltHou = string.Join(",", dltH.Take(2).ToArray());

                    LasetWinNuber = string.Join("|", dltQian, dltHou);
                    break;
                case "CQSSC":
                case "PL3":
                case "FC3D":
                    var cqsscWinNumber = winNuber.ToArray();
                    LasetWinNuber = string.Join(",", cqsscWinNumber);
                    break;
                case "JX11X5":
                    var jx11x5WinNumber = winNuber.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    if (jx11x5WinNumber.Length != 5)
                        throw new LogicException("开奖号不正确！");
                    LasetWinNuber = string.Join(",", jx11x5WinNumber.Take(5).ToArray());
                    break;
                default:
                    break;
            }
            return LasetWinNuber;
        }
    }
}