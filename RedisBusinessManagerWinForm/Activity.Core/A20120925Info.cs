using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20120925Info
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public decimal TransferMoney { get; set; }
        public decimal GiveMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class A20120925Info_Collection
    {
        public A20120925Info_Collection()
        {
            ActListInfo = new List<A20120925Info>();
        }
        public int TotalCount { get; set; }
        public decimal TotalTransferMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
        public List<A20120925Info> ActListInfo { get; set; }
    }
    [CommunicationObject]
    public class A20120925CZGiveMoneyInfo
    {
        public Int64 Id { get; set; }
        public string UserId { get; set; }
        public decimal PayMoney { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    [CommunicationObject]
    public class A20120925CZGiveMoneyInfo_Collection
    {
        public A20120925CZGiveMoneyInfo_Collection()
        {
            ActListInfo = new List<A20120925CZGiveMoneyInfo>();
        }
        public int TotalCount { get; set; }
        public decimal TotalPayMoney { get; set; }
        public List<A20120925CZGiveMoneyInfo> ActListInfo { get; set; }
    }
}
