using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Core;

namespace Activity.Core
{
    /// <summary>
    /// 购彩返利
    /// </summary>
    [CommunicationObject]
    public class A20140902_BuyLotteryRebateInfo
    {
        public string UserId { get; set; }
        public SchemeType SchemeType { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public decimal AddMoney { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class A20140902_购彩返利InfoCollection
    {
        public A20140902_购彩返利InfoCollection()
        {
            List = new List<A20140902_BuyLotteryRebateInfo>();
        }
        public List<A20140902_BuyLotteryRebateInfo> List { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
    }

    /// <summary>
    /// 奖金百分之一十八
    /// </summary>
    [CommunicationObject]
    public class AddMoneryEighteenPercentInfo
    {
        public string UserId { get; set; }
        public SchemeType SchemeType { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public decimal AddMoney { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class AddMoneryEighteenPercentInfoCollection
    {
        public AddMoneryEighteenPercentInfoCollection()
        {
            List = new List<AddMoneryEighteenPercentInfo>();
        }
        public List<AddMoneryEighteenPercentInfo> List { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
    }

    /// <summary>
    /// 首次充值数据
    /// </summary>
    [CommunicationObject]
    public class FistFillMoneyInfo
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public decimal FillMoney { get; set; }
        public decimal CurrentGiveMoney { get; set; }
        public decimal NextGiveMoney { get; set; }
        public bool IsGiveComplate { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class FistFillMoneyInfoCollection
    {
        public FistFillMoneyInfoCollection()
        {
            List = new List<FistFillMoneyInfo>();
        }
        public List<FistFillMoneyInfo> List { get; set; }
        public int TotalCount { get; set; }
        public decimal FillMoney { get; set; }
        public decimal CurrentGiveMoney { get; set; }
        public decimal NextGiveMoney { get; set; }
    }

    /// <summary>
    /// 足彩不中也有奖
    /// </summary>
    [CommunicationObject]
    public class FootballConsolationPrizeInfo
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal GiveMoney { get; set; }
        public bool IsGive { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class FootballConsolationPrizeInfoCollection
    {
        public FootballConsolationPrizeInfoCollection()
        {
            List = new List<FootballConsolationPrizeInfo>();
        }
        public List<FootballConsolationPrizeInfo> List { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
    }

    /// <summary>
    /// 购彩不花钱
    /// </summary>
    [CommunicationObject]
    public class BuyLotteryNoMoneyInfo
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string DisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal FillMoney { get; set; }
        public bool IsGive { get; set; }
        public string CurrentTime { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class BuyLotteryNoMoneyInfoCollection
    {
        public BuyLotteryNoMoneyInfoCollection()
        {
            List = new List<BuyLotteryNoMoneyInfo>();
        }
        public List<BuyLotteryNoMoneyInfo> List { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
    }
}
