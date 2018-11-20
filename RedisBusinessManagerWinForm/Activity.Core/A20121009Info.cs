using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20121009Info
    {
        public Int64 Id { get; set; }
        public string UserId { get; set; }
        public int SchemeType { get; set; }
        public int SchemeId { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set;}
        public int HitMatchCount { get; set; }
        public decimal AddMoney { get; set; }
        public DateTime CreateTime { get; set; }

    }
    [CommunicationObject]
    public class A20121009Info_Collection
    {
        public A20121009Info_Collection()
        {
            ActListInfo = new List<A20121009Info>();
        }
        public int TotalCount { get; set; }
        public decimal TotalAddMoney { get; set; }
        public List<A20121009Info> ActListInfo { get; set; }
    }
}
