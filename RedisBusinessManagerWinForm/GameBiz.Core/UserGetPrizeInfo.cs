using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class UserGetPrizeInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PrizeType { get; set; }
        public int PayInegral { get; set; }
        public decimal GiveMoney { get; set; }
        public decimal OrderMoey { get; set; }
        public string Summary { get; set; }
        public DateTime CreateTime { get; set; }
        public string OrderId { get; set; }
    }
    [CommunicationObject]
    public class UserGetPrizeInfo_Collection
    {
        public UserGetPrizeInfo_Collection()
        {
            GetPrizeList = new List<UserGetPrizeInfo>();
        }
        public int TotalCount { get; set; }
        public List<UserGetPrizeInfo> GetPrizeList { get; set; }
    }
}
