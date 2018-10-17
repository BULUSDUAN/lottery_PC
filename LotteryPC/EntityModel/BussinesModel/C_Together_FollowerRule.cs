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
    // 定制跟单规则
    ///</summary>
    [ProtoContract]
    [Entity("C_Together_FollowerRule",Type = EntityType.Table)]
    public class C_Together_FollowerRule
    { 
        public C_Together_FollowerRule()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 是否启用
            ///</summary>
            [ProtoMember(2)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
            /// <summary>
            // 合买发起人用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("CreaterUserId")]
            public string CreaterUserId{ get; set; }
            /// <summary>
            // 跟单人用户编号
            ///</summary>
            [ProtoMember(4)]
            [Field("FollowerUserId")]
            public string FollowerUserId{ get; set; }
            /// <summary>
            // 序号
            ///</summary>
            [ProtoMember(5)]
            [Field("FollowerIndex")]
            public int FollowerIndex{ get; set; }
            /// <summary>
            // 彩种编码
            ///</summary>
            [ProtoMember(6)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法编码
            ///</summary>
            [ProtoMember(7)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 跟单方案个数(每认购一个就 -1)
            ///</summary>
            [ProtoMember(8)]
            [Field("SchemeCount")]
            public int SchemeCount{ get; set; }
            /// <summary>
            // 方案最小金额
            ///</summary>
            [ProtoMember(9)]
            [Field("MinSchemeMoney")]
            public decimal MinSchemeMoney{ get; set; }
            /// <summary>
            // 方案最大金额
            ///</summary>
            [ProtoMember(10)]
            [Field("MaxSchemeMoney")]
            public decimal MaxSchemeMoney{ get; set; }
            /// <summary>
            // 跟单份数
            ///</summary>
            [ProtoMember(11)]
            [Field("FollowerCount")]
            public int FollowerCount{ get; set; }
            /// <summary>
            // 跟单百分比
            ///</summary>
            [ProtoMember(12)]
            [Field("FollowerPercent")]
            public decimal FollowerPercent{ get; set; }
            /// <summary>
            // 当方案剩余份数/百分比不足时 是否跟单
            ///</summary>
            [ProtoMember(13)]
            [Field("CancelWhenSurplusNotMatch")]
            public bool CancelWhenSurplusNotMatch{ get; set; }
            /// <summary>
            // 连续未中奖方案数
            ///</summary>
            [ProtoMember(14)]
            [Field("NotBonusSchemeCount")]
            public int NotBonusSchemeCount{ get; set; }
            /// <summary>
            // 连续X个方案未中奖则停止跟单
            ///</summary>
            [ProtoMember(15)]
            [Field("CancelNoBonusSchemeCount")]
            public int CancelNoBonusSchemeCount{ get; set; }
            /// <summary>
            // 当用户金额小于X时停止跟单
            ///</summary>
            [ProtoMember(16)]
            [Field("StopFollowerMinBalance")]
            public decimal StopFollowerMinBalance{ get; set; }
            /// <summary>
            // 已跟单订单数
            ///</summary>
            [ProtoMember(17)]
            [Field("TotalBetOrderCount")]
            public int TotalBetOrderCount{ get; set; }
            /// <summary>
            // 已跟单且中奖订单数
            ///</summary>
            [ProtoMember(18)]
            [Field("TotalBonusOrderCount")]
            public int TotalBonusOrderCount{ get; set; }
            /// <summary>
            // 已跟单总金额
            ///</summary>
            [ProtoMember(19)]
            [Field("TotalBetMoney")]
            public decimal TotalBetMoney{ get; set; }
            /// <summary>
            // 已跟单中奖金额
            ///</summary>
            [ProtoMember(20)]
            [Field("TotalBonusMoney")]
            public decimal TotalBonusMoney{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(21)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}