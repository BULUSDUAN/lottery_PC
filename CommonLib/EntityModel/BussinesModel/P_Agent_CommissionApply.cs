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
    // 代理商提成申请
    ////</summary>
    [ProtoContract]
    [Entity("P_Agent_CommissionApply",Type = EntityType.Table)]
    public class P_Agent_CommissionApply
    { 
        public P_Agent_CommissionApply()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = false, IsPrimaryKey = true)]
            public string ID{ get; set; }
            //// <summary>
            // 申请时间
            ////</summary>
            [ProtoMember(2)]
            [Field("RequestTime")]
            public DateTime? RequestTime{ get; set; }
            //// <summary>
            // 周期开始时间
            ////</summary>
            [ProtoMember(3)]
            [Field("FromTime")]
            public DateTime? FromTime{ get; set; }
            //// <summary>
            // 周期停止时间
            ////</summary>
            [ProtoMember(4)]
            [Field("ToTime")]
            public DateTime? ToTime{ get; set; }
            //// <summary>
            // 申请人ID
            ////</summary>
            [ProtoMember(5)]
            [Field("RequestUserId")]
            public string RequestUserId{ get; set; }
            //// <summary>
            // 申请的佣金
            ////</summary>
            [ProtoMember(6)]
            [Field("RequestCommission")]
            public decimal? RequestCommission{ get; set; }
            //// <summary>
            // 响应的佣金
            ////</summary>
            [ProtoMember(7)]
            [Field("ResponseCommission")]
            public decimal? ResponseCommission{ get; set; }
            //// <summary>
            // 结算人
            ////</summary>
            [ProtoMember(8)]
            [Field("ResponseUserId")]
            public string ResponseUserId{ get; set; }
            //// <summary>
            // 结算销量
            ////</summary>
            [ProtoMember(9)]
            [Field("DealSale")]
            public decimal? DealSale{ get; set; }
            //// <summary>
            // 结算时间
            ////</summary>
            [ProtoMember(10)]
            [Field("ResponseTime")]
            public DateTime? ResponseTime{ get; set; }
            //// <summary>
            // 申请状态，1：处理中，2：已处理，3：已拒绝
            ////</summary>
            [ProtoMember(11)]
            [Field("ApplyState")]
            public int? ApplyState{ get; set; }
            //// <summary>
            // 备注说明
            ////</summary>
            [ProtoMember(12)]
            [Field("Remark")]
            public string Remark{ get; set; }
    }
}