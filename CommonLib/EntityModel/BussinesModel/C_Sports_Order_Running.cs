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
    // 
    ////</summary>
    [ProtoContract]
    [Entity("C_Sports_Order_Running",Type = EntityType.Table)]
    public class C_Sports_Order_Running
    { 
        public C_Sports_Order_Running()
        {
        
        }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 彩种代码
            ////</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(4)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 串关方式
            ////</summary>
            [ProtoMember(5)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            //// <summary>
            // 方案类型
            ////</summary>
            [ProtoMember(6)]
            [Field("SchemeType")]
            public int? SchemeType{ get; set; }
            //// <summary>
            // 方案保密性
            ////</summary>
            [ProtoMember(7)]
            [Field("Security")]
            public int? Security{ get; set; }
            //// <summary>
            // 方案来源
            ////</summary>
            [ProtoMember(8)]
            [Field("SchemeSource")]
            public int? SchemeSource{ get; set; }
            //// <summary>
            // 方案投注类别
            ////</summary>
            [ProtoMember(9)]
            [Field("SchemeBettingCategory")]
            public int? SchemeBettingCategory{ get; set; }
            //// <summary>
            // 期号
            ////</summary>
            [ProtoMember(10)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            //// <summary>
            // 倍数
            ////</summary>
            [ProtoMember(11)]
            [Field("Amount")]
            public int? Amount{ get; set; }
            //// <summary>
            // 注数
            ////</summary>
            [ProtoMember(12)]
            [Field("BetCount")]
            public int? BetCount{ get; set; }
            //// <summary>
            // 比赛场数
            ////</summary>
            [ProtoMember(13)]
            [Field("TotalMatchCount")]
            public int? TotalMatchCount{ get; set; }
            //// <summary>
            // 总金额
            ////</summary>
            [ProtoMember(14)]
            [Field("TotalMoney")]
            public decimal? TotalMoney{ get; set; }
            //// <summary>
            // 红包金额
            ////</summary>
            [ProtoMember(15)]
            [Field("RedBagMoney")]
            public decimal? RedBagMoney{ get; set; }
            //// <summary>
            // 方案返利金额
            ////</summary>
            [ProtoMember(16)]
            [Field("SchemeDeduct")]
            public decimal? SchemeDeduct{ get; set; }
            //// <summary>
            // 停止时间
            ////</summary>
            [ProtoMember(17)]
            [Field("StopTime")]
            public DateTime? StopTime{ get; set; }
            //// <summary>
            // 出票状态
            ////</summary>
            [ProtoMember(18)]
            [Field("TicketStatus")]
            public int? TicketStatus{ get; set; }
            //// <summary>
            // 出票口
            ////</summary>
            [ProtoMember(19)]
            [Field("TicketGateway")]
            public string TicketGateway{ get; set; }
            //// <summary>
            // 出票进度
            ////</summary>
            [ProtoMember(20)]
            [Field("TicketProgress")]
            public decimal? TicketProgress{ get; set; }
            //// <summary>
            // 彩票编号
            ////</summary>
            [ProtoMember(21)]
            [Field("TicketId")]
            public string TicketId{ get; set; }
            //// <summary>
            // 彩票记录
            ////</summary>
            [ProtoMember(22)]
            [Field("TicketLog")]
            public string TicketLog{ get; set; }
            //// <summary>
            // 进行状态
            ////</summary>
            [ProtoMember(23)]
            [Field("ProgressStatus")]
            public int? ProgressStatus{ get; set; }
            //// <summary>
            // 状态
            ////</summary>
            [ProtoMember(24)]
            [Field("BonusStatus")]
            public int? BonusStatus{ get; set; }
            //// <summary>
            // 中奖数
            ////</summary>
            [ProtoMember(25)]
            [Field("BonusCount")]
            public int? BonusCount{ get; set; }
            //// <summary>
            // 命中数
            ////</summary>
            [ProtoMember(26)]
            [Field("HitMatchCount")]
            public int? HitMatchCount{ get; set; }
            //// <summary>
            // 正确数
            ////</summary>
            [ProtoMember(27)]
            [Field("RightCount")]
            public int? RightCount{ get; set; }
            //// <summary>
            // 错误数1
            ////</summary>
            [ProtoMember(28)]
            [Field("Error1Count")]
            public int? Error1Count{ get; set; }
            //// <summary>
            // 错误数2
            ////</summary>
            [ProtoMember(29)]
            [Field("Error2Count")]
            public int? Error2Count{ get; set; }
            //// <summary>
            // 最小奖金
            ////</summary>
            [ProtoMember(30)]
            [Field("MinBonusMoney")]
            public decimal? MinBonusMoney{ get; set; }
            //// <summary>
            // 最大奖金
            ////</summary>
            [ProtoMember(31)]
            [Field("MaxBonusMoney")]
            public decimal? MaxBonusMoney{ get; set; }
            //// <summary>
            // 税前奖金
            ////</summary>
            [ProtoMember(32)]
            [Field("PreTaxBonusMoney")]
            public decimal? PreTaxBonusMoney{ get; set; }
            //// <summary>
            // 税后奖金
            ////</summary>
            [ProtoMember(33)]
            [Field("AfterTaxBonusMoney")]
            public decimal? AfterTaxBonusMoney{ get; set; }
            //// <summary>
            // 是否可追号
            ////</summary>
            [ProtoMember(34)]
            [Field("CanChase")]
            public bool? CanChase{ get; set; }
            //// <summary>
            // 是否虚拟订单
            ////</summary>
            [ProtoMember(35)]
            [Field("IsVirtualOrder")]
            public bool? IsVirtualOrder{ get; set; }
            //// <summary>
            // 是否已返点
            ////</summary>
            [ProtoMember(36)]
            [Field("IsPayRebate")]
            public bool? IsPayRebate{ get; set; }
            //// <summary>
            // 实际计算返点的金额(享受返点的金额)
            ////</summary>
            [ProtoMember(37)]
            [Field("RealPayRebateMoney")]
            public decimal? RealPayRebateMoney{ get; set; }
            //// <summary>
            // 总返点金额
            ////</summary>
            [ProtoMember(38)]
            [Field("TotalPayRebateMoney")]
            public decimal? TotalPayRebateMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(39)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 投注时间
            ////</summary>
            [ProtoMember(40)]
            [Field("BetTime")]
            public DateTime? BetTime{ get; set; }
            //// <summary>
            // 代理商编号
            ////</summary>
            [ProtoMember(41)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            //// <summary>
            // 成功金额
            ////</summary>
            [ProtoMember(42)]
            [Field("SuccessMoney")]
            public decimal? SuccessMoney{ get; set; }
            //// <summary>
            // 扩展字段，活动三选一
            ////</summary>
            [ProtoMember(43)]
            [Field("ExtensionOne")]
            public string ExtensionOne{ get; set; }
            //// <summary>
            // 附加信息 （不能包含特殊符号）
            ////</summary>
            [ProtoMember(44)]
            [Field("Attach")]
            public string Attach{ get; set; }
            //// <summary>
            // 查询彩票停止时间
            ////</summary>
            [ProtoMember(45)]
            [Field("QueryTicketStopTime")]
            public string QueryTicketStopTime{ get; set; }
            //// <summary>
            // 是否追加
            ////</summary>
            [ProtoMember(46)]
            [Field("IsAppend")]
            public bool? IsAppend{ get; set; }
            //// <summary>
            // 出票时间
            ////</summary>
            [ProtoMember(47)]
            [Field("TicketTime")]
            public DateTime? TicketTime{ get; set; }
            //// <summary>
            // 是否拆票
            ////</summary>
            [ProtoMember(48)]
            [Field("IsSplitTickets")]
            public bool? IsSplitTickets{ get; set; }
    }
}