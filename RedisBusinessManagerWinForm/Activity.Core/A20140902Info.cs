using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    /// <summary>
    /// 优惠券
    /// </summary>
    [CommunicationObject]
    public class A20140902Info
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public string OrderId { get; set; }
        public decimal FillMoney { get; set; }
        public bool IsGive { get; set; }
        public string CurrentTime { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class A20140902InfoCollection
    {
        public A20140902InfoCollection()
        {
            List = new List<A20140902Info>();
        }
        public List<A20140902Info> List { get; set; }
    }
}
