using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 订单明细
    ////</summary>
    [ProtoContract]
    [Entity("C_OrderDetail",Type = EntityType.Table)]
    public class C_OrderDetail
    { 
        public C_OrderDetail()
        {
        
        }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(2)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 下注时间
            ////</summary>
            [ProtoMember(3)]
            [Field("BetTime")]
            public DateTime? BetTime{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(4)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 彩种
            ////</summary>
            [ProtoMember(5)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(6)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 玩法名称
            ////</summary>
            [ProtoMember(7)]
            [Field("GameTypeName")]
            public string GameTypeName{ get; set; }
            //// <summary>
            // 串关方式
            ////</summary>
            [ProtoMember(8)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            //// <summary>
            // 倍数
            ////</summary>
            [ProtoMember(9)]
            [Field("Amount")]
            public int? Amount{ get; set; }
            //// <summary>
            // 投注方案类别
            ////</summary>
            [ProtoMember(10)]
            [Field("SchemeType")]
            public int SchemeType{ get; set; }
            //// <summary>
            // 方案来源
            ////</summary>
            [ProtoMember(11)]
            [Field("SchemeSource")]
            public int? SchemeSource{ get; set; }
            //// <summary>
            // 方案投注类别
            ////</summary>
            [ProtoMember(12)]
            [Field("SchemeBettingCategory")]
            public int? SchemeBettingCategory{ get; set; }
            //// <summary>
            // 当前投注金额
            ////</summary>
            [ProtoMember(13)]
            [Field("CurrentBettingMoney")]
            public decimal? CurrentBettingMoney{ get; set; }
            //// <summary>
            // 总金额
            ////</summary>
            [ProtoMember(14)]
            [Field("TotalMoney")]
            public decimal? TotalMoney{ get; set; }
            //// <summary>
            // 进行状态
            ////</summary>
            [ProtoMember(15)]
            [Field("ProgressStatus")]
            public int ProgressStatus{ get; set; }
            //// <summary>
            // 彩票状态
            ////</summary>
            [ProtoMember(16)]
            [Field("TicketStatus")]
            public int TicketStatus{ get; set; }
            //// <summary>
            // 总期数
            ////</summary>
            [ProtoMember(17)]
            [Field("TotalIssuseCount")]
            public int TotalIssuseCount{ get; set; }
            //// <summary>
            // 开始期号
            ////</summary>
            [ProtoMember(18)]
            [Field("StartIssuseNumber")]
            public string StartIssuseNumber{ get; set; }
            //// <summary>
            // 当前期号
            ////</summary>
            [ProtoMember(19)]
            [Field("CurrentIssuseNumber")]
            public string CurrentIssuseNumber{ get; set; }
            //// <summary>
            // 奖金状态
            ////</summary>
            [ProtoMember(20)]
            [Field("BonusStatus")]
            public int BonusStatus{ get; set; }
            //// <summary>
            // 税前奖金
            ////</summary>
            [ProtoMember(21)]
            [Field("PreTaxBonusMoney")]
            public decimal? PreTaxBonusMoney{ get; set; }
            //// <summary>
            // 税后奖金
            ////</summary>
            [ProtoMember(22)]
            [Field("AfterTaxBonusMoney")]
            public decimal? AfterTaxBonusMoney{ get; set; }
            //// <summary>
            // 加奖金额
            ////</summary>
            [ProtoMember(23)]
            [Field("AddMoney")]
            public decimal? AddMoney{ get; set; }
            //// <summary>
            // 是否停止追号
            ////</summary>
            [ProtoMember(24)]
            [Field("StopAfterBonus")]
            public bool? StopAfterBonus{ get; set; }
            //// <summary>
            // 是否虚拟订单
            ////</summary>
            [ProtoMember(25)]
            [Field("IsVirtualOrder")]
            public bool? IsVirtualOrder{ get; set; }
            //// <summary>
            // 代理商编号
            ////</summary>
            [ProtoMember(26)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            //// <summary>
            // 完成时间
            ////</summary>
            [ProtoMember(27)]
            [Field("ComplateTime")]
            public DateTime? ComplateTime{ get; set; }
            //// <summary>
            // 是否追加投注
            ////</summary>
            [ProtoMember(28)]
            [Field("IsAppend")]
            public bool? IsAppend{ get; set; }
            //// <summary>
            // 出票时间
            ////</summary>
            [ProtoMember(29)]
            [Field("TicketTime")]
            public DateTime? TicketTime{ get; set; }
            //// <summary>
            // 红包金额
            ////</summary>
            [ProtoMember(30)]
            [Field("RedBagMoney")]
            public decimal? RedBagMoney{ get; set; }
            //// <summary>
            // 总返点金额
            ////</summary>
            [ProtoMember(31)]
            [Field("TotalPayRebateMoney")]
            public decimal? TotalPayRebateMoney{ get; set; }
            //// <summary>
            // 实际计算返点的金额(享受返点的金额)
            ////</summary>
            [ProtoMember(32)]
            [Field("RealPayRebateMoney")]
            public decimal? RealPayRebateMoney{ get; set; }
            //// <summary>
            // 红包支付费用
            ////</summary>
            [ProtoMember(33)]
            [Field("RedBagAwardsMoney")]
            public decimal? RedBagAwardsMoney{ get; set; }
            //// <summary>
            // 奖金
            ////</summary>
            [ProtoMember(34)]
            [Field("BonusAwardsMoney")]
            public decimal? BonusAwardsMoney{ get; set; }
    }
}