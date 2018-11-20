using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Lottery.Objects;

namespace Common.Lottery
{
    /// <summary>
    /// 订单的票分析器
    /// </summary>
    public static class TicketAnalyzer
    {
        ///<summary>
        /// 分析订单的票
        ///</summary>
        ///<param name="order">订单</param>
        ///<returns>票集合</returns>
        public static TicketCollection AnalyzeTickets(Order order)
        {
            // 将所有号码，按照玩法进行分组
            var groupAntecodes = order.AntecodeList.GroupBy((item) => item.GameType);
            var ticketList = new TicketCollection();
            foreach (var group in groupAntecodes)
            {
                var gameType = group.Key;
                // 获取玩法一张票最多可以携带的号码数量
                var maxCount = TicketRuleGetter.GetMaxAntecodeCountEachTicket(order.GameCode, gameType);
                // 解析此玩法所有号码，并返回多张票
                var innerTicketList = GetTicketsByAntecodes(group.ToArray(), maxCount, () => new Ticket() { GameType = gameType, Amount = order.Amount, });
                ticketList.AddRange(innerTicketList);
            }
            return ticketList;
        }
        private static int GetMaxTicketAmount(string gameCode)
        {
            switch (gameCode.ToLower())
            {
                case "ssq":
                case "fc3d":
                    return 50;
                default:
                    return 99;
            }
        }
        private static IEnumerable<Ticket> GetTicketsByAntecodes(IEnumerable<Antecode> antecodeList, int maxCount, Func<Ticket> createTicketHandler)
        {
            var ticketList = new List<Ticket>();
            var tmpTicket = createTicketHandler();
            foreach (var antecode in antecodeList)
            {
                // 添加号码到票
                tmpTicket.AnteCodeList.Add(antecode);
                // 如果票包含的号码数量达到上限，则将票添加到列表，并重建票对象
                if (tmpTicket.AnteCodeList.Count >= maxCount)
                {
                    ticketList.Add(tmpTicket);
                    tmpTicket = createTicketHandler();
                }
            }
            if (tmpTicket.AnteCodeList.Count > 0)
            {
                ticketList.Add(tmpTicket);
            }
            return ticketList.ToArray();
        }
    }
}
