using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20140214Info
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public decimal FillMoney { get; set; }
        public decimal GiveMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class A20140214Info_Collection
    {
        public A20140214Info_Collection()
        {
            ActListInfo = new List<A20140214Info>();
        }
        public int TotalCount { get; set; }
        public decimal TotalFillMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
        public List<A20140214Info> ActListInfo { get; set; }
    }
    [CommunicationObject]
    public class A20140214SSCHBInfo
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameCodeDisplayName { get; set; }
        public string GameType { get; set; }
        public string GameTypeDisplayName { get; set; }
        public string IssuseNumber { get; set; }
        public decimal AddMoney { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class A20140214SSCHBInfo_Collection
    {
        public A20140214SSCHBInfo_Collection()
        {
            ActListInfo = new List<A20140214SSCHBInfo>();
        }
        public int TotalCount { get; set; }
        public decimal TotalAddMoney { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalAfterTaxBonusMoney { get; set; }
        public List<A20140214SSCHBInfo> ActListInfo { get; set; }
    }
}
