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
    [Entity("E_A20140902_购彩不花钱",Type = EntityType.Table)]
    public class E_A20140902_购彩不花钱
    { 
        public E_A20140902_购彩不花钱()
        {
        
        }
            /// <summary>
            // 主键编号
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
            // 订单编号
            ///</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            /// <summary>
            // 订单金额
            ///</summary>
            [ProtoMember(4)]
            [Field("OrderMoney")]
            public decimal OrderMoney{ get; set; }
            /// <summary>
            // 充值金额
            ///</summary>
            [ProtoMember(5)]
            [Field("FillMoney")]
            public decimal FillMoney{ get; set; }
            /// <summary>
            // 是否赠送
            ///</summary>
            [ProtoMember(6)]
            [Field("IsGive")]
            public bool IsGive{ get; set; }
            /// <summary>
            // 当前时间
            ///</summary>
            [ProtoMember(7)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}