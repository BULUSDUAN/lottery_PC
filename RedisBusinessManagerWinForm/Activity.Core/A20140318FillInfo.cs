using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Core;

namespace Activity.Core
{
    public class A20140318FillInfo
    {
        /// <summary>
        /// 充值最高送1000
        /// </summary>
        [CommunicationObject]
        public class A20140318Return1000Info
        {
            public long Id { get; set; }
            public string UserId { get; set; }
            public string UserDisplayName { get; set; }
            public string OrderId { get; set; }
            public decimal FillMoney { get; set; }
            public decimal GivedMoney { get; set; }
            public decimal NextMonthGiveMoney { get; set; }
            public int NextMonth { get; set; }
            public bool GiveComplete { get; set; }
            public DateTime CreateTime { get; set; }
        }
        [CommunicationObject]
        public class A20140318Return1000InfoCollection
        {
            public List<A20140318Return1000Info> RecordList { get; set; }

            public A20140318Return1000InfoCollection()
            {
                RecordList = new List<A20140318Return1000Info>();
            }

            public int TotalCount { get; set; }
            public decimal TotalFillMoney { get; set; }
            public decimal TotalGiveMoney { get; set; }
            public decimal TotalNextMonthGiveMoney { get; set; }
        }

        public class A20140318PrizeDoubleInfo
        {
            public long Id { get; set; }
            public string UserId { get; set; }
            public SchemeType SchemeType { get; set; }
            public string SchemeId { get; set; }
            public string GameCode { get; set; }
            public string GameType { get; set; }
            public string PlayType { get; set; }
            public string IssuseNumber { get; set; }
            public decimal AddMoney { get; set; }
            public decimal OrderMoney { get; set; }
            public decimal AfterTaxBonusMoney { get; set; }
            public DateTime CreateTime { get; set; }
        }

        [CommunicationObject]
        public class A20140318PrizeDoubleInfoCollection
        {
            public List<A20140318PrizeDoubleInfo> RecordList { get; set; }

            public A20140318PrizeDoubleInfoCollection()
            {
                RecordList = new List<A20140318PrizeDoubleInfo>();
            }
            public int TotalCount { get; set; }
        }
    }
}
