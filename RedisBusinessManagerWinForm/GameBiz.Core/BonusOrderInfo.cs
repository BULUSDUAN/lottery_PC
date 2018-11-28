using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common.Utilities;
using Common.Mappings;

namespace GameBiz.Core
{
    #region 中奖订单信息
    [CommunicationObject]
    public class BonusOrderInfo
    {
        [EntityMappingField("RowNumber")]
        public long RowNumber { get; set; }
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("PlayType")]
        public string PlayType { get; set; }
        [EntityMappingField("SchemeType")]
        public SchemeType SchemeType { get; set; }
        [EntityMappingField("IssuseNumber")]
        public string IssuseNumber { get; set; }
        [EntityMappingField("Amount")]
        public int Amount { get; set; }
        [EntityMappingField("BetCount")]
        public int BetCount { get; set; }
        [EntityMappingField("TotalMatchCount")]
        public int TotalMatchCount { get; set; }
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
        [EntityMappingField("TicketStatus")]
        public TicketStatus TicketStatus { get; set; }
        [EntityMappingField("ProgressStatus")]
        public ProgressStatus ProgressStatus { get; set; }
        [EntityMappingField("BonusStatus")]
        public BonusStatus BonusStatus { get; set; }
        [EntityMappingField("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }
        [EntityMappingField("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        [EntityMappingField("IsVirtualOrder")]
        public bool IsVirtualOrder { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [EntityMappingField("BonusCount")]
        public int BonusCount { get; set; }
        [EntityMappingField("BonusCountDescription")]
        public string BonusCountDescription { get; set; }
        [EntityMappingField("BonusCountDisplayName")]
        public string BonusCountDisplayName { get; set; }
        [EntityMappingField("ComplateDateTime")]
        public DateTime? ComplateDateTime { get; set; }
        [EntityMappingField("ComplateDate")]
        public string ComplateDate { get; set; }
        [EntityMappingField("IsPrizeMoney")]
        public bool IsPrizeMoney { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }
        [EntityMappingField("IsAgent")]
        public bool IsAgent { get; set; }
        [EntityMappingField("HideDisplayNameCount")]
        public int HideDisplayNameCount { get; set; }

        public int GetBonusCountByLevel(int level)
        {
            if (string.IsNullOrEmpty(BonusCountDescription) || BonusCountDescription == "-1")
            {
                return 0;
            }
            var items = BonusCountDescription.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                var tmp = item.Split('|');
                if (tmp[0].Equals(level.ToString()))
                {
                    return int.Parse(tmp[1]);
                }
            }
            return 0;
        }
    }
    [CommunicationObject]
    public class BonusOrderInfoCollection
    {
        public BonusOrderInfoCollection()
        {
            BonusOrderList = new List<BonusOrderInfo>();
        }
        public int TotalCount { get; set; }
        public List<BonusOrderInfo> BonusOrderList { get; set; }
    }

    #endregion
}
