using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20140318Info
    {
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string SchemeId { get; set; }
        public int BetCount { get; set; }
        public decimal BetMoney { get; set; }
        public decimal BonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class A20140318Info_Collection
    {
        public A20140318Info_Collection()
        {
            InfoList = new List<A20140318Info>();
        }

        public int TotalCount { get; set; }
        public List<A20140318Info> InfoList { get; set; }
    }
}
