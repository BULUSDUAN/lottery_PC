using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20130903Info
    {
        public Int64 Id { get; set; }
        public string UserId { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameCodeDisplayName { get; set; }
        public string GameType { get; set; }
        public string GameTypeDisplayName { get; set; }
        public string IssuseNumber { get; set; }
        public decimal AddMoney { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class A20130903Info_Collection
    {
        public A20130903Info_Collection()
        {
            ActListInfo = new List<A20130903Info>();
        }
        public int TotalCount { get; set; }
        public decimal TotalAddMoney { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalAfterTaxBonusMoney { get; set; }
        public List<A20130903Info> ActListInfo { get; set; }
    }

}
