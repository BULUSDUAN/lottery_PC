using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class UserIntegralBalanceInfo
    {
        public string UserId { get; set; }
        public int CurrIntegralBalance { get; set; }
        public int UseIntegralBalance { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class UserIntegralBalanceInfo_Collection
    {
        public UserIntegralBalanceInfo_Collection()
        {
            IntegralBalanceList = new List<UserIntegralBalanceInfo>();
        }
        public int TotalCount { get; set; }
        public List<UserIntegralBalanceInfo> IntegralBalanceList { get; set; }
    }

}
