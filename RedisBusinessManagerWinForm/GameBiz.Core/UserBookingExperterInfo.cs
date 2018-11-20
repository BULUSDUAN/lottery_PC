using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class UserBookingExperterInfo
    {
        public string ExperterId { get; set; }
        public string UserId { get; set; }
        public BookingExperterCategory Category { get; set; }
        public decimal BookingPrice { get; set; }
        public string BalancePassword { get; set; }
    }
}
