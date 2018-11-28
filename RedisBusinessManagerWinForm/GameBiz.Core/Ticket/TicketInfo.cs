using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core.Ticket
{

    [CommunicationObject]
    public class TicketId_QueryInfoCollection
    {
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalTicketCount { get; set; }
        /// <summary>
        /// 票列表
        /// </summary>
        public IList<Sports_TicketQueryInfo> TicketList { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalMatchCount { get; set; }
        /// <summary>
        /// 相关比赛信息列表
        /// </summary>
        public IList<MatchInfo> MatchList { get; set; }
    }
}
