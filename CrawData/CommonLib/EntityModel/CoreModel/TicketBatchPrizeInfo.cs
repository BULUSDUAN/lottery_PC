using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   // [Common.Communication.CommunicationObjectAttribute]
    public class TicketBatchPrizeInfo
    {
       // public TicketBatchPrizeInfo();

        public string TicketId { get; set; }
        public BonusStatus BonusStatus { get; set; }
        public decimal PreMoney { get; set; }
        public decimal AfterMoney { get; set; }
    }
}
