using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Core;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20141203GiveLotteryInfo
    {
        public string UserId { get; set; }
        public string AnteCode { get; set; }
        public string SchemeId { get; set; }
        public SchemeSource ActivityType { get; set; }
        public DateTime CreateTime { get; set; }
        public string IdCardNumber { get; set; }
    }
}
