using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.RestrictionsBetMoney
{
    [CommunicationObject]
    public class BetWhiteListInfo
    {
        public int WhiteLlistId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public bool IsEnable { get; set; }
        public string ExpansionOne { get; set; }
        public decimal ExpansionTwo { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsOpenHeMai { get; set; }
        public bool IsOpenBDFX { get; set; }
        public bool IsSingleScheme { get; set; }
        public string SiteList { get; set; }
    }
    [CommunicationObject]
    public class BetWhiteListInfo_Collection
    {
        public BetWhiteListInfo_Collection()
        {
            BetInfoList = new List<BetWhiteListInfo>();
        }
        public int TotalCount { get; set; }
        public List<BetWhiteListInfo> BetInfoList { get; set; }
    }
}
