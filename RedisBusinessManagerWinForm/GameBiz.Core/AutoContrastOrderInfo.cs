using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class AutoContrastOrderInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        public DateTime BetTime { get; set; }
        /// <summary>
        /// 出票状态
        /// </summary>
        public TicketStatus TicketStatus { get; set; }
        /// <summary>
        /// 进行状态
        /// </summary>
        public ProgressStatus ProgressStatus { get; set; }
        /// <summary>
        /// 实际投注额
        /// </summary>
        public decimal CurrentBettingMoney { get; set; }
        /// <summary>
        /// 订单投注额度
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 打票时间
        /// </summary>
        public DateTime PrintTime { get; set; }
    }
    [CommunicationObject]
    public class AutoContrastOrder_Collection
    {
        public AutoContrastOrder_Collection()
        {
            OrderList = new List<AutoContrastOrderInfo>();
        }
        public int TotalCount { get; set; }
        public int TicketTotalCount { get; set; }
        public decimal TicketAfterTaxBonusMoney { get; set; }
        public decimal TicketPreTaxBonusMoney { get; set; }
        public decimal FailTicketMoney { get; set; }
        public decimal TicketTotalMoney { get; set; }
        public List<AutoContrastOrderInfo> OrderList { get; set; }
        public Ticket_Collection TicketCollection { get; set; }
    }
    [CommunicationObject]
    public class TicketInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketId { get; set; }
        /// <summary>
        /// 票类型:1：出票中心票数据；2：网站票数据；
        /// </summary>
        public int TicketType { get; set; }
        /// <summary>
        /// 出票中心票投注金额
        /// </summary>
        public decimal TicketBetMoney { get; set; }
        /// <summary>
        /// 出票中心票税前金额
        /// </summary>
        public decimal TicketPreTaxBonusMoney { get; set; }
        /// <summary>
        /// 出票中心票税后金额
        /// </summary>
        public decimal TicketAfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 网站票税前金额
        /// </summary>
        public decimal WebPreTaxBonusMoney { get; set; }
        /// <summary>
        /// 网站票税后金额
        /// </summary>
        public decimal WebAfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 打票时间
        /// </summary>
        public DateTime PrintTime { get; set; }

    }
    public class Ticket_Collection
    {
        public Ticket_Collection()
        {
            TicketList = new List<TicketInfo>();
        }
        public List<TicketInfo> TicketList { get; set; }

    }
}