using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20121128Info
    {
        public Int64 Id { get; set; }
        public string UserId { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameCodeDisplayName { get; set; }
        public string GameType { get; set; }
        public string GameTypeDisplayName { get; set; }
        public string IssuseNumber { get; set; }
        public decimal GiveMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class A20121128Info_Colleciton
    {
        public A20121128Info_Colleciton()
        {
            ActListInfo = new List<A20121128Info>();
        }
        public int TotalCount { get; set; }
        public decimal TotalGiveMoney { get; set; }
        public List<A20121128Info> ActListInfo { get; set; }
    }
}
