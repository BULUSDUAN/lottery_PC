using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

//加奖配置

namespace External.Core.AppendBonus
{
    [CommunicationObject]
    public class AppendBonusConfigInfo
    {
        public string GameName { get; set; }
        public string GameTypeName { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public decimal AppendBonusMoney { get; set; }
        public decimal AppendRatio { get; set; }
        public int StartMultiple { get; set; }
        public int ColorBeanNumber { get; set; }
        public decimal ColorBeanRatio { get; set; }
        public int ColorBeanStartMultiple { get; set; }
        public DateTime ModifyTime { get; set; }
        public int StartIssueNumber { get; set; }
        public int EndIssueNumber { get; set; }
        public int BonusMoneyStartMultiple { get; set; }
    }

    [CommunicationObject]
    public class AppendBonusConfigInfo_QueryCollection
    {
        public AppendBonusConfigInfo_QueryCollection()
        {
            ConfigList = new List<AppendBonusConfigInfo>();
        }

        public IList<AppendBonusConfigInfo> ConfigList { get; set; }
    }
}
