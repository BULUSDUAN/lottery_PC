using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class TicketMachineConfigInfo
    {
        public int Id { get; set; }
        public decimal LimitMoney { get; set; }
        public decimal LotteryMoney { get; set; }
        public string LotteryNo { get; set; }
        public int ProportionMoney { get; set; }
        public int ReorderId { get; set; }
        public string StationNo { get; set; }
        public string Token { get; set; }
        public DateTime UpdateTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TicketOwner { get; set; }
        public decimal CurrentMoney { get; set; }
        public decimal SuccessMoney { get; set; }
        public decimal SurplusMoney { get; set; }
        public int UpperLimitNunber { get; set; }
    }
    [CommunicationObject]
    public class TicketMachineConfig_Collection
    {
        public TicketMachineConfig_Collection()
        {
            TicketMachineList = new List<TicketMachineConfigInfo>();
        }
        public List<TicketMachineConfigInfo> TicketMachineList { get; set; }
    }
}
