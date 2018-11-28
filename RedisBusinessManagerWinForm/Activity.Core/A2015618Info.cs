using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A2015618Info
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public decimal FillMoney { get; set; }
        public decimal GiveMoney { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    [CommunicationObject]
    public class A2015618Info_Collection
    {
        public A2015618Info_Collection()
        {
            ActListInfo = new List<A2015618Info>();
        }
        public int TotalCount { get; set; }
        public decimal TotalFillMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
        public List<A2015618Info> ActListInfo { get; set; }
    }
}
