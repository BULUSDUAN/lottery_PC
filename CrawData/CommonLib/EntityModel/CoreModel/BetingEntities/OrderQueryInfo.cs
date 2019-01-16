using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EntityModel.CoreModel
{
        #region 票信息

     
    public class Sports_TicketQueryInfo
    {
        public string SchemeId { get; set; }
        public string TicketId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        public int BetUnits { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 票金额
        /// </summary>
        public decimal BetMoney { get; set; }
        public string BetContent { get; set; }
        public string LocOdds { get; set; }
        public TicketStatus TicketStatus { get; set; }
        /// <summary>
        /// 票号 1
        /// </summary>
        public string PrintNumber1 { get; set; }
        /// <summary>
        /// 票号 2
        /// </summary>
        public string PrintNumber2 { get; set; }
        /// <summary>
        /// 票号 3
        /// </summary>
        public string PrintNumber3 { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        public BonusStatus BonusStatus { get; set; }

        public decimal PreTaxBonusMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? PrintDateTime { get; set; }
    }

     
    public class Sports_TicketQueryInfoCollection
    {
        public int TotalCount { get; set; }
        public List<Sports_TicketQueryInfo> TicketList { get; set; }
    }
    #endregion

    /// <summary>
    /// 票查询结果
    /// </summary>
     
    public class QueryTicketInfo
    {
        public string SchemeId { get; set; }
        public string TicketId { get; set; }
        public string UserId { get; set; }
        public bool IsAllFail { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal TotalErrorMoney { get; set; }
        public bool SaveComplate { get; set; }
    }

    #region 延误开奖订单列表

    /// <summary>
    /// 延误开奖订单列表
    /// </summary>
     
    public class DelayPrizeOrderInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal SuccessMoney { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public int Amount { get; set; }
        public int BetCount { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime BetTime { get; set; }
        public SchemeType SchemeType { get; set; }
        public SchemeSource SchemeSource { get; set; }
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public ProgressStatus ProgressStatus { get; set; }
        public BonusStatus BonusStatus { get; set; }
    }
     
    public class DelayPrizeOrder_Collection
    {
        public DelayPrizeOrder_Collection()
        {
            DelayPrizeOrderList = new List<DelayPrizeOrderInfo>();
        }
        public int TotalCount { get; set; }
        public List<DelayPrizeOrderInfo> DelayPrizeOrderList { get; set; }
    }

    #endregion


    
}
