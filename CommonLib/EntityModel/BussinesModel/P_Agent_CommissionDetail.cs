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
    // 代理佣金明细
    ////</summary>
    [ProtoContract]
    [Entity("P_Agent_CommissionDetail",Type = EntityType.Table)]
    public class P_Agent_CommissionDetail
    { 
        public P_Agent_CommissionDetail()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            //// <summary>
            // 彩种代码
            ////</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 申请状态 0：未申请，1：已申请
            ////</summary>
            [ProtoMember(4)]
            [Field("ApplyState")]
            public int? ApplyState{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 上级用户Id
            ////</summary>
            [ProtoMember(6)]
            [Field("PAgentId")]
            public string PAgentId{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(7)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 分类 1：投注，2，申请
            ////</summary>
            [ProtoMember(8)]
            [Field("Category")]
            public int? Category{ get; set; }
            //// <summary>
            // 销量
            ////</summary>
            [ProtoMember(9)]
            [Field("Sale")]
            public decimal? Sale{ get; set; }
            //// <summary>
            // 初始返点
            ////</summary>
            [ProtoMember(10)]
            [Field("InitialPoint")]
            public decimal? InitialPoint{ get; set; }
            //// <summary>
            // 下级的返点
            ////</summary>
            [ProtoMember(11)]
            [Field("LowerPoint")]
            public decimal? LowerPoint{ get; set; }
            //// <summary>
            // 实际返点
            ////</summary>
            [ProtoMember(12)]
            [Field("ActualPoint")]
            public decimal? ActualPoint{ get; set; }
            //// <summary>
            // 扣量
            ////</summary>
            [ProtoMember(13)]
            [Field("Deduction")]
            public decimal? Deduction{ get; set; }
            //// <summary>
            // 扣量前佣金
            ////</summary>
            [ProtoMember(14)]
            [Field("BeforeCommission")]
            public decimal? BeforeCommission{ get; set; }
            //// <summary>
            // 实际佣金
            ////</summary>
            [ProtoMember(15)]
            [Field("ActualCommission")]
            public decimal? ActualCommission{ get; set; }
            //// <summary>
            // 备注说明
            ////</summary>
            [ProtoMember(16)]
            [Field("Remark")]
            public string Remark{ get; set; }
            //// <summary>
            // 关键字
            ////</summary>
            [ProtoMember(17)]
            [Field("DetailKeyword")]
            public string DetailKeyword{ get; set; }
            //// <summary>
            // 完成时间
            ////</summary>
            [ProtoMember(18)]
            [Field("ComplateDateTime")]
            public DateTime? ComplateDateTime{ get; set; }
    }
}