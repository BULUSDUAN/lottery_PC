using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class LotteryNewBonusInfoCollection:Page
    {
        public LotteryNewBonusInfoCollection()
        {
            List = new List<LotteryNewBonusInfo>();
        }
        [ProtoMember(1)]
        public List<LotteryNewBonusInfo> List { get; set; }
    }
    [ProtoContract]
    public class LotteryNewBonusInfo
    {
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        [ProtoMember(2)]
        public string GameCode { get; set; }
        [ProtoMember(3)]
        public string GameType { get; set; }
        [ProtoMember(4)]
        public string PlayType { get; set; }
        [ProtoMember(5)]
        public string IssuseNumber { get; set; }
        [ProtoMember(6)]
        public int Amount { get; set; }
        [ProtoMember(7)]
        public string UserDisplayName { get; set; }
        [ProtoMember(8)]
        public int HideUserDisplayNameCount { get; set; }
        [ProtoMember(9)]
        public decimal TotalMoney { get; set; }
        [ProtoMember(10)]
        public decimal PreTaxBonusMoney { get; set; }
        [ProtoMember(11)]
        public decimal AfterTaxBonusMoney { get; set; }
        [ProtoMember(12)]
        public DateTime CreateTime { get; set; }
    }
}
