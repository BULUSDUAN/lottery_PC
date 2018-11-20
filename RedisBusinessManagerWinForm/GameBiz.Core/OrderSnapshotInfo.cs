using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class OrderSnapshotInfo
    {

    }
    [CommunicationObject]
    public class OrderSnapshotHeadInfo
    {
        public string KeyLine { get;set; }
        public string AnteCode { get; set; }
        public bool IsBonusStop { get; set; }
        public string UserName { get; set; }
        public string IssuseNumber { get; set; }
        public string SchemeId { get; set; }
        public decimal TotalMoney { get; set; }
        public DateTime CreateTime { get; set; }
        public int Amount { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
    }

    [CommunicationObject]
    public class OrderSnapshotDetailInfo_JC
    {
        public string OrderGameType { get; set; }
        public string MatchIdName { get; set; }
        public int TotalCount { get; set; }
        public int Guarantees { get; set; }
        public int Subscription { get; set; }
        public decimal Price { get; set; }

        public string UserName { get; set; }
        public string SchemeId { get; set; }
        public string IssuseNumber { get; set; }
        public decimal TotalMoney { get; set; }
        public int Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime FSStopBettingTime { get; set; }
        public string HomeTeamName { get; set; }
        public string GuestTeamName { get; set; }
        public string AnteCode { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
    }
    [CommunicationObject]
    public class OrderSnapshotDetailInfo_JC_Collection
    {
        public OrderSnapshotDetailInfo_JC_Collection()
        {
            ListInfo = new List<OrderSnapshotDetailInfo_JC>();
        }
        public IList<OrderSnapshotDetailInfo_JC> ListInfo { get; set; }
        public OrderSnapshotHeadInfo HeadInfo { get; set; }
    }
    [CommunicationObject]
    public class OrderSnapshotDetailInfo_PT
    {
        public bool IsBonusStop { get; set; }
        public string OrderGameType { get; set; }
        public string UserName { get; set; }
        public string SchemeId { get; set; }
        public string KeyLine { get; set; }
        public decimal TotalMoney { get; set; }
        public string AnteCode { get; set; }
        public int BetCount { get; set; }
        public int Amount { get; set; }
        public string IssuseNumber { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class OrderSnapshotDetailInfo_PT_Collection
    {
        public OrderSnapshotDetailInfo_PT_Collection()
        {
            ListInfo = new List<OrderSnapshotDetailInfo_PT>();
            HeadListInfo=new  List<OrderSnapshotHeadInfo>();
        }
        public IList<OrderSnapshotDetailInfo_PT> ListInfo { get; set; }
        public IList<OrderSnapshotHeadInfo> HeadListInfo { get; set; }
    }
    [CommunicationObject]
    public class OrderSnapshotDetailInfo_Together
    {
        public int BetCount { get; set; }
        public string UserName { get; set; }
        public string SchemeId { get; set; }
        public string IssuseNumber { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalMoney { get; set; }
        public int Guarantees { get; set; }
        public int Subscription { get; set; }
        public decimal Price { get; set; }
        public string AnteCode { get; set; }
        public int Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
    }
    [CommunicationObject]
    public class OrderSnapshotDetailInfo_Together_Collection
    {
        public OrderSnapshotDetailInfo_Together_Collection()
        {
            ListInfo = new List<OrderSnapshotDetailInfo_Together>();
        }
        public IList<OrderSnapshotDetailInfo_Together> ListInfo { get; set; }
        public OrderSnapshotHeadInfo HeadInfo { get; set; }
    }
}
