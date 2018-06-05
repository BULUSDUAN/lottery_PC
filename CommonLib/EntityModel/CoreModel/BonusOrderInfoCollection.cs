using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
    /// <summary>
    /// 中奖订单信息列表
    /// </summary>
    public class BonusOrderInfoCollection:Page
    {
        public BonusOrderInfoCollection()
        {
            BonusOrderList = new List<BonusOrderInfo>();
        }
        public List<BonusOrderInfo> BonusOrderList { get; set; }       
    }
    /// <summary>
    /// 中奖订单信息
    /// </summary>
    public class BonusOrderInfo
    {

        public long RowNumber { get; set; }

        public string SchemeId { get; set; }

        public string GameCode { get; set; }

        public string GameType { get; set; }

        public string PlayType { get; set; }

        public SchemeType SchemeType { get; set; }

        public string IssuseNumber { get; set; }

        public int Amount { get; set; }

        public int BetCount { get; set; }

        public int TotalMatchCount { get; set; }

        public decimal TotalMoney { get; set; }

        public TicketStatus TicketStatus { get; set; }

        public ProgressStatus ProgressStatus { get; set; }

        public BonusStatus BonusStatus { get; set; }

        public decimal PreTaxBonusMoney { get; set; }

        public decimal AfterTaxBonusMoney { get; set; }

        public bool IsVirtualOrder { get; set; }

        public DateTime CreateTime { get; set; }

        public int BonusCount { get; set; }

        public string BonusCountDescription { get; set; }

        public string BonusCountDisplayName { get; set; }

        public DateTime? ComplateDateTime { get; set; }

        public string ComplateDate { get; set; }

        public bool IsPrizeMoney { get; set; }

        public string AgentId { get; set; }

        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public bool IsAgent { get; set; }

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
}
