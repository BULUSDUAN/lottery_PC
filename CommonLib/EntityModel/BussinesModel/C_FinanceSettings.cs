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
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("FinanceId", IsIdenty = true, IsPrimaryKey = true)]
            public int FinanceId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 
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
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("MinMoney")]
            public decimal? MinMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("MaxMoney")]
            public decimal? MaxMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("OperatorId")]
            public string OperatorId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}