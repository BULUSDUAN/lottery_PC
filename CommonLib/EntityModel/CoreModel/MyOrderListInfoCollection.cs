using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class MyOrderListInfoCollection: Page
    {
        [ProtoMember(1)]
        public IList<MyOrderListInfo> List { get; set; }
    }
    [ProtoContract]
    public class MyOrderListInfo
    {
        // 方案号
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        // 彩种
        [ProtoMember(2)]
        public string GameCode { get; set; }
        // 玩法名称
        [ProtoMember(3)]
        public string GameTypeName { get; set; }
        // 方案类型
        [ProtoMember(4)]
        public SchemeType SchemeType { get; set; }
        // 方案来源
        [ProtoMember(5)]
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
        [ProtoMember(6)]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 方案总金额
        [ProtoMember(7)]
        public decimal TotalMoney { get; set; }
        [ProtoMember(8)]
        public bool StopAfterBonus { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        [ProtoMember(9)]
        public int Amount { get; set; }
        // 方案进度
        [ProtoMember(10)]
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        [ProtoMember(11)]
        public TicketStatus TicketStatus { get; set; }
        // 购买期号
        [ProtoMember(12)]
        public string IssuseNumber { get; set; }
        // 中奖状态
        [ProtoMember(13)]
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
        [ProtoMember(14)]
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        [ProtoMember(15)]
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        [ProtoMember(16)]
        public string BetTime { get; set; }
        /// <summary>
        /// 彩种玩法
        /// </summary>
        [ProtoMember(17)]
        public string GameType { get; set; }
        /// <summary>
        /// 加奖总金额
        /// </summary>
        [ProtoMember(18)]
        public decimal AddMoney { get; set; }
        /// <summary>
        /// 红包加奖金额
        /// </summary>
        [ProtoMember(19)]
        public decimal RedBagAwardsMoney { get; set; }
        /// <summary>
        /// 奖金加奖金额
        /// </summary>
        [ProtoMember(20)]
        public decimal BonusAwardsMoney { get; set; }
    }
}
