using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    /// <summary>
    /// 充值赠送，次月再赠送
    /// </summary>
    [CommunicationObject]
    public class A20131101Info
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string OrderId { get; set; }
        public decimal FillMoney { get; set; }
        public decimal GivedMoney { get; set; }
        public decimal NextMonthGiveMoney { get; set; }
        public int NextMonth { get; set; }
        public bool GiveComplate { get; set; }
        public DateTime CreateTime { get; set; }
    }


    [CommunicationObject]
    public class A20131101InfoCollection
    {
        public A20131101InfoCollection()
        {
            RecordList = new List<A20131101Info>();
        }

        public List<A20131101Info> RecordList { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalFillMoney { get; set; }
        public decimal TotalGiveMoney { get; set; }
        public decimal TotalNextMonthGiveMoney { get; set; }

    }

    /// <summary>
    /// 用户月返点
    /// </summary>
    [CommunicationObject]
    public class ActivityMonthReturnPointInfo
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int Month { get; set; }
        public decimal TotalBetMoney { get; set; }
        public decimal GiveMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class ActivityMonthReturnPointInfoCollection : List<ActivityMonthReturnPointInfo>
    {
    }
    [CommunicationObject]
    public class ActivityMonthReturnPointInfo_Colleciton
    {
        public ActivityMonthReturnPointInfo_Colleciton()
        {
            ActListInfo = new List<ActivityMonthReturnPointInfo>();
        }
        public int TotalCount { get; set; }
        public decimal TotalGiveMoney { get; set; }      
        public List<ActivityMonthReturnPointInfo> ActListInfo { get; set; }
}

}
