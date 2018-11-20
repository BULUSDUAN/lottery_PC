using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20130111Info
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public decimal TotalMoney { get; set; }
    }

    [CommunicationObject]
    public class A20130111InfoCollection : List<A20130111Info>
    {
    }
}
