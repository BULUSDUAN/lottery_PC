using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class UserIntegralDetailInfo
    {
        public Int64 IntegralDetailId { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string Summary { get; set; }
        public int PayIntegral { get; set; }
        public int BeforeIntegral { get; set; }
        public int AfterIntegral { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class UserIntegralDetailInfo_Collection
    {
        public UserIntegralDetailInfo_Collection()
        {
            IntegralDetailList = new List<UserIntegralDetailInfo>();
        }
        public int TotalCount { get; set; }
        public List<UserIntegralDetailInfo> IntegralDetailList { get; set; }
    }
}
