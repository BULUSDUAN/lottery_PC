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
    [Entity("E_A20140731_豆豆兑换",Type = EntityType.Table)]
    public class E_A20140731_豆豆兑换
    { 
        public E_A20140731_豆豆兑换()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 豆豆
            ///</summary>
            [ProtoMember(3)]
            [Field("DouDou")]
            public int DouDou{ get; set; }
            /// <summary>
            // 金额
            ///</summary>
            [ProtoMember(4)]
            [Field("Money")]
            public decimal Money{ get; set; }
            /// <summary>
            // 活动金额
            ///</summary>
            [ProtoMember(5)]
            [Field("ActivityMoney")]
            public decimal ActivityMoney{ get; set; }
            /// <summary>
            // 奖项
            ///</summary>
            [ProtoMember(6)]
            [Field("Prize")]
            public string Prize{ get; set; }
            /// <summary>
            // 奖项金额
            ///</summary>
            [ProtoMember(7)]
            [Field("PrizeMoney")]
            public decimal PrizeMoney{ get; set; }
            /// <summary>
            // 是否赠送
            ///</summary>
            [ProtoMember(8)]
            [Field("IsGive")]
            public bool IsGive{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}