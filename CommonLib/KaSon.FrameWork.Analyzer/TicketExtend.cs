using EntityModel.Ticket;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Analyzer
{
   public static class TicketExtend
    {
        /// <summary>
        /// 扩展方法 
        /// </summary>
        /// <param name="tik"></param>
        /// <param name="GameCode"></param>
        /// <param name="price"></param>
        public static void AnalyzeTicket(Ticket tik,string GameCode, decimal price = 2M)
        {
           // throw new NotImplementedException("NotImplementedException");
            var analyzer = KaSon.FrameWork.Analyzer.AnalyzerFactory.AnalyzerFactory.GetAntecodeAnalyzer(GameCode, tik.GameType);
            tik.BetCount = 0;
            tik.TicketMoney = 0;
            tik.AnteCodeList.ForEach(c =>
            {
                c.AnalyzeAntecode(analyzer, price);
                tik.BetCount += c.BetCount;
                tik.TicketMoney += c.AntecodeMoney * tik.Amount;
            });
        }
        /// <summary>
        /// 订单分析扩展方法
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="GameCode"></param>
        /// <param name="price"></param>

        public static void AnalyzeOrderEx(this TicketCollection tc,string GameCode, decimal price = 2M)
        {
            tc.BetCount = 0;
            tc.TotalMoney = 0;
            tc.ForEach(t =>
            {

                AnalyzeTicket(t, GameCode, price);
               // t.AnalyzeTicket(GameCode, price);
                tc.BetCount += t.BetCount;
                tc.TotalMoney += t.TicketMoney;
            });
        }
    }
}
