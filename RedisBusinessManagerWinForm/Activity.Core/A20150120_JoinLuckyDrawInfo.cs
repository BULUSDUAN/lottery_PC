using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
   public class A20150120_JoinLuckyDrawInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal PrizeMoney { get; set; }
        public string OrderId { get; set; }
        public string AgentId { get; set; }
        public string PrizeType { get; set; }
        public bool IsTestUser { get; set; }
        public int ClientType { get; set; }
        public string Description { get; set; }
        public string LotteryNumber { get; set; }
        public string LoginName { get; set; }
        public DateTime CreateTime { get; set; }
        public string IdCardNumber { get; set; }
    }
    [CommunicationObject]
    public class A20150120_JoinLuckyDraw_Collection
    {
        public A20150120_JoinLuckyDraw_Collection()
        {
            JoinLuckyDrawList = new List<A20150120_JoinLuckyDrawInfo>();
        }
        public int TotalCount { get;set; }
        public List<A20150120_JoinLuckyDrawInfo> JoinLuckyDrawList{get;set;}
    }
}
