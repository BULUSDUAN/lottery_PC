using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class OrderTicketListInfo
    {
        public decimal AfterTaxBonusMoney { get; set; }
        public int BetAmount { get; set; }
        public decimal BetMoney { get; set; }
        public string BetType { get; set; }
        public int BetUnits { get; set; }
        /// <summary>
        /// 中奖状态:0：未派奖；1:中奖；2：未中奖；
        /// </summary>
        public int BonusStatus { get; set; }
        public DateTime CreateTime { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public string SchemeId { get; set; }
        public decimal PreTaxBonusMoney { get; set; }
        public string TicketId { get; set; }
        public string TicketLog { get; set; }
        public string LotteryNo { get; set; }
        /// <summary>
        /// 票状态:1:保存票；10：投注中；90：出票成功；99：出票失败；
        /// </summary>
        public int TicketStatus { get; set; }
        /// <summary>
        /// 互爱税后金额
        /// </summary>
        public decimal HA_AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 最后打票时间
        /// </summary>
        public DateTime DeadlineTime { get; set; }
    }
    [CommunicationObject]
    public class OrderTicketList_Collection
    {
        public OrderTicketList_Collection()
        {
            TicketList = new List<OrderTicketListInfo>();
            StoreClientList = new List<StoreClientInfo>();
        }
        public decimal ErrorMoney { get; set; }
        public decimal Money { get; set; }
        public decimal RunningMoney { get; set; }
        public decimal SucessMoney { get; set; }
        public int TotalCount { get; set; }
        public int TotalSuccessCount { get; set; }
        public int TotalFailCount { get; set; }
        public int TotalRunningCount { get; set; }
        public decimal TotalAfterMoney { get; set; }
        public int TotalWinningCount { get; set; }
        public decimal Total_HA_AfterTaxBonusMoney { get; set; }
        public List<OrderTicketListInfo> TicketList { get; set; }
        public List<StoreClientInfo> StoreClientList { get; set; }
    }
    [CommunicationObject]
    public class StoreClientInfo
    {
        public string LotteryNO { get; set; }
        public string LotteryName { get; set; }
        public string Token { get; set; }
    }
    [CommunicationObject]
    public class OnlineGmaeCodeConfigInfo
    {
        public string GameCode { get; set; }
        public int Id { get; set; }
        public decimal LimitMoney { get; set; }
        public string LotteryNo { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    [CommunicationObject]
    public class OnlineGmaeCodeConfig_Collection
    {
        public OnlineGmaeCodeConfig_Collection()
        {
            GameCodeConfigList = new List<OnlineGmaeCodeConfigInfo>();
        }
        public List<OnlineGmaeCodeConfigInfo> GameCodeConfigList { get; set; }
    }
}
