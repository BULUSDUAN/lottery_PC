using BettingLottery.Service.ModuleServices;
using EntityModel.Enum;
using KaSon.FrameWork.Common.JSON;
using System;

namespace BettiongLottery.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            BettingService bettingService = new BettingService();

            var param = new {
                UserToken="",
                BalancePassword = "",
                GameCode = "",
                GameType = "",
                PlayType = "",
                Security = "",
                TotalMoney = "",
                StopAfterBonus = "",
                SavaOrder="",
                RedBagMoney = "",
                IssuseList="",
            };
           String paramStr=  JsonHelper.Serialize(param);
            bettingService.Betting(paramStr, SchemeSource.Android, "001");

            Console.ReadKey(true);
            //KaSon.FrameWork.AnalyzerFactory

        }
    }
}
