using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>
    [ProtoContract]
    [Entity("C_TotalSingleTreasure",Type = EntityType.Table)]
    public class C_TotalSingleTreasure
    { 
        public C_TotalSingleTreasure()
        {
        
        }
            /// <summary>
            // 订单号
            ///</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 累计购买人数
            ///</summary>
            [ProtoMember(3)]
            [Field("TotalBuyCount")]
            public int TotalBuyCount{ get; set; }
            /// <summary>
            // 累计购买金额
            ///</summary>
            [ProtoMember(4)]
            [Field("TotalBuyMoney")]
            public decimal TotalBuyMoney{ get; set; }
            /// <summary>
            // 累计中奖金额
            ///</summary>
            [ProtoMember(5)]
            [Field("TotalBonusMoney")]
            public decimal TotalBonusMoney{ get; set; }
            /// <summary>
            // 所有抄单盈利率
            ///</summary>
            [ProtoMember(6)]
            [Field("ProfitRate")]
            public decimal ProfitRate{ get; set; }
            /// <summary>
            // 当前投注金额
            ///</summary>
            [ProtoMember(7)]
            [Field("CurrentBetMoney")]
            public decimal CurrentBetMoney{ get; set; }
            /// <summary>
            // 当前中奖金额
            ///</summary>
            [ProtoMember(8)]
            [Field("CurrBonusMoney")]
            public decimal CurrBonusMoney{ get; set; }
            /// <summary>
            // 预计中奖奖金
            ///</summary>
            [ProtoMember(9)]
            [Field("ExpectedBonusMoney")]
            public decimal ExpectedBonusMoney{ get; set; }
            /// <summary>
            // 预计回报率
            ///</summary>
            [ProtoMember(10)]
            [Field("ExpectedReturnRate")]
            public decimal ExpectedReturnRate{ get; set; }
            /// <summary>
            // 提成
            ///</summary>
            [ProtoMember(11)]
            [Field("Commission")]
            public decimal Commission{ get; set; }
            /// <summary>
            // 总的提成金额
            ///</summary>
            [ProtoMember(12)]
            [Field("TotalCommissionMoney")]
            public decimal TotalCommissionMoney{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 所选比赛第一场结束时间
            ///</summary>
            [ProtoMember(14)]
            [Field("FirstMatchStopTime")]
            public DateTime FirstMatchStopTime{ get; set; }
            /// <summary>
            // 所选比赛最后一场结束时间
            ///</summary>
            [ProtoMember(15)]
            [Field("LastMatchStopTime")]
            public DateTime LastMatchStopTime{ get; set; }
            /// <summary>
            // 是否中奖
            ///</summary>
            [ProtoMember(16)]
            [Field("IsBonus")]
            public bool IsBonus{ get; set; }
            /// <summary>
            // 是否完成
            ///</summary>
            [ProtoMember(17)]
            [Field("IsComplate")]
            public bool IsComplate{ get; set; }
            /// <summary>
            // 晒单宣言
            ///</summary>
            [ProtoMember(18)]
            [Field("SingleTreasureDeclaration")]
            public string SingleTreasureDeclaration{ get; set; }
            /// <summary>
            // 方案保密性
            ///</summary>
            [ProtoMember(19)]
            [Field("Security")]
            public int Security{ get; set; }
            /// <summary>
            // 当前宝单的盈利率
            ///</summary>
            [ProtoMember(20)]
            [Field("CurrProfitRate")]
            public decimal CurrProfitRate{ get; set; }
    }
}