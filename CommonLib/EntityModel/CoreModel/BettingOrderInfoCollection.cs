using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
using ProtoBuf;

namespace EntityModel.CoreModel
{
    [Serializable]
    [ProtoContract]
   public class BettingOrderInfoCollection:Page
    {
        [ProtoMember(1)]
        public int TotalUserCount { get; set; }
        [ProtoMember(2)]
        public decimal TotalBuyMoney { get; set; }
        [ProtoMember(3)]
        public decimal TotalOrderMoney { get; set; }
        [ProtoMember(4)]
        public decimal TotalPreTaxBonusMoney { get; set; }
        [ProtoMember(5)]
        public decimal TotalAfterTaxBonusMoney { get; set; }
        [ProtoMember(6)]
        public decimal TotalAddMoney { get; set; }
        [ProtoMember(7)]
        public decimal TotalRedbagMoney { get; set; }
        [ProtoMember(8)]
        public decimal TotalRealPayRebateMoney { get; set; }
        [ProtoMember(9)]
        public decimal TotalRedBagAwardsMoney { get; set; }
        [ProtoMember(10)]
        public decimal TotalBonusAwardsMoney { get; set; }
        [ProtoMember(11)]

        public IList<BettingOrderInfo> OrderList { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public class BettingOrderInfo
    {
        [ProtoMember(1)]
        // 行号
        public long RowNumber { get; set; }
        // 方案号
        [ProtoMember(2)]
        public string SchemeId { get; set; }
        // 方案创建者编号
        [ProtoMember(3)]
        public string UserId { get; set; }
        // 方案创建者VIP级别
        [ProtoMember(4)]
        public int VipLevel { get; set; }
        // 方案创建者显示名称
        [ProtoMember(5)]
        public string CreatorDisplayName { get; set; }
        // 方案创建者是否隐藏显示名称
        [ProtoMember(6)]
        public int HideDisplayNameCount { get; set; }
        // 彩种
        [ProtoMember(7)]
        public string GameCode { get; set; }
        // 彩种名称
        [ProtoMember(8)]
        public string GameName { get; set; }
        // 玩法名称
        [ProtoMember(9)]
        public string GameTypeName { get; set; }
        //过关方式
        [ProtoMember(10)]
        public string PlayType { get; set; }
        //倍数
        [ProtoMember(11)]
        public int Amount { get; set; }
        // 方案类型
        [ProtoMember(12)]
        public SchemeType SchemeType { get; set; }
        // 方案来源
        [ProtoMember(13)]
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
        [ProtoMember(14)]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 方案投注方案
        [ProtoMember(15)]
        public TogetherSchemeSecurity Security { get; set; }
        // 当前投注金额
        [ProtoMember(16)]
        public decimal CurrentBettingMoney { get; set; }
        // 方案总金额
        [ProtoMember(17)]
        public decimal TotalMoney { get; set; }
        // 中奖号码
        [ProtoMember(18)]
        public string WinNumber { get; set; }
        // 方案进度
        [ProtoMember(19)]
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        [ProtoMember(20)]
        public TicketStatus TicketStatus { get; set; }
        // 购买期数
        [ProtoMember(21)]
        public int TotalIssuseCount { get; set; }
        // 购买期号
        [ProtoMember(22)]
        public string IssuseNumber { get; set; }
        // 中奖状态
        [ProtoMember(23)]
        public BonusStatus BonusStatus { get; set; }
        // 中奖后停止
        [ProtoMember(24)]
        public bool StopAfterBonus { get; set; }
        // 税前奖金
        [ProtoMember(25)]
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        [ProtoMember(26)]
        public decimal AfterTaxBonusMoney { get; set; }
        // 加奖奖金
        [ProtoMember(27)]
        public decimal AddMoney { get; set; }
        // 创建时间
        [ProtoMember(28)]
        public DateTime CreateTime { get; set; }
        // 所属经销商
        [ProtoMember(29)]
        public string AgentId { get; set; }
        [ProtoMember(30)]
        public bool IsVirtualOrder { get; set; }
        [ProtoMember(31)]
        public DateTime BetTime { get; set; }
        [ProtoMember(32)]
        public decimal RedBagMoney { get; set; }
        [ProtoMember(33)]
        public decimal RealPayRebateMoney { get; set; }
        [ProtoMember(34)]
        public decimal RedBagAwardsMoney { get; set; }
        [ProtoMember(35)]
        public decimal BonusAwardsMoney { get; set; }
    }
}
