using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
using ProtoBuf;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 中奖订单信息列表
    /// </summary>
    /// 
    [ProtoContract]
    [Serializable]
    public class BonusOrderInfoCollection:Page
    {
        public BonusOrderInfoCollection()
        {
            BonusOrderList = new List<BonusOrderInfo>();
        }
        [ProtoMember(1)]
        public List<BonusOrderInfo> BonusOrderList { get; set; }       
    }
    /// <summary>
    /// 中奖订单信息
    /// </summary>
    [ProtoContract]
    [Serializable]
    public class BonusOrderInfo
    {
        [ProtoMember(1)]
        public long RowNumber { get; set; }
        [ProtoMember(2)]
        public string SchemeId { get; set; }
        [ProtoMember(3)]
        public string GameCode { get; set; }
        [ProtoMember(4)]
        public string GameType { get; set; }
        [ProtoMember(5)]
        public string PlayType { get; set; }
        [ProtoMember(6)]
        public SchemeType SchemeType { get; set; }
        [ProtoMember(7)]
        public string IssuseNumber { get; set; }
        [ProtoMember(8)]
        public int Amount { get; set; }
        [ProtoMember(9)]
        public int BetCount { get; set; }
        [ProtoMember(10)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(11)]
        public decimal TotalMoney { get; set; }
        [ProtoMember(12)]
        public TicketStatus TicketStatus { get; set; }
        [ProtoMember(13)]
        public ProgressStatus ProgressStatus { get; set; }
        [ProtoMember(14)]
        public BonusStatus BonusStatus { get; set; }
        [ProtoMember(15)]
        public decimal PreTaxBonusMoney { get; set; }
        [ProtoMember(16)]
        public decimal AfterTaxBonusMoney { get; set; }
        [ProtoMember(17)]
        public bool IsVirtualOrder { get; set; }
        [ProtoMember(18)]
        public DateTime CreateTime { get; set; }
        [ProtoMember(19)]
        public int BonusCount { get; set; }
        [ProtoMember(20)]
        public string BonusCountDescription { get; set; }
        [ProtoMember(21)]
        public string BonusCountDisplayName { get; set; }
        [ProtoMember(22)]
        public DateTime? ComplateDateTime { get; set; }
        [ProtoMember(23)]
        public string ComplateDate { get; set; }
        [ProtoMember(24)]
        public bool IsPrizeMoney { get; set; }
        [ProtoMember(25)]
        public string AgentId { get; set; }
        [ProtoMember(26)]
        public string UserId { get; set; }
        [ProtoMember(27)]
        public string DisplayName { get; set; }
        [ProtoMember(28)]
        public bool IsAgent { get; set; }
        [ProtoMember(29)]
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
