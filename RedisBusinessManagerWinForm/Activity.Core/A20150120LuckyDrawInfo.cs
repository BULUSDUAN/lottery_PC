using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20150120LuckyDrawInfo
    {
        public int Id { get; set; }
        public string LotteryNumber { get; set; }
        public bool IsUse { get; set; }
        public string BelongUserId { get; set; }
        public string BelongUserName { get; set; }
        public string LotteryType { get; set; }
        public DateTime CreateTime { get; set; }
        public string AgentId { get; set; }
    }
    [CommunicationObject]
    public class A20150120LuckyDraw_Collection
    {
        public A20150120LuckyDraw_Collection()
        {
            LuckyDrawList = new List<A20150120LuckyDrawInfo>();
        }
        public int TotalCount { get; set; }
        public List<A20150120LuckyDrawInfo> LuckyDrawList { get; set; }
    }
}
