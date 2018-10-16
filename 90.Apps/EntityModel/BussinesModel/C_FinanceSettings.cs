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
    [Entity("C_FinanceSettings",Type = EntityType.Table)]
    public class C_FinanceSettings
    { 
        public C_FinanceSettings()
        {
        
        }
            /// <summary>
            // 主键ID，自增长
            ///</summary>
            [ProtoMember(1)]
            [Field("FinanceId", IsIdenty = true, IsPrimaryKey = true)]
            public int FinanceId{ get; set; }
            /// <summary>
            // 财务人员
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 操作级别 101：初级；102：高级
            ///</summary>
            [ProtoMember(3)]
            [Field("OperateRank")]
            public string OperateRank{ get; set; }
            /// <summary>
            // 10：提现;20充值
            ///</summary>
            [ProtoMember(4)]
            [Field("OperateType")]
            public string OperateType{ get; set; }
            /// <summary>
            // 最小金额
            ///</summary>
            [ProtoMember(5)]
            [Field("MinMoney")]
            public decimal MinMoney{ get; set; }
            /// <summary>
            // 最大金额
            ///</summary>
            [ProtoMember(6)]
            [Field("MaxMoney")]
            public decimal MaxMoney{ get; set; }
            /// <summary>
            // 创建人员Id
            ///</summary>
            [ProtoMember(7)]
            [Field("OperatorId")]
            public string OperatorId{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}