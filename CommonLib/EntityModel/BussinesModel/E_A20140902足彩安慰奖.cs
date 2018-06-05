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
    [Entity("E_A20140902足彩安慰奖",Type = EntityType.Table)]
    public class E_A20140902足彩安慰奖
    { 
        public E_A20140902足彩安慰奖()
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
            // 订单编号
            ///</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            /// <summary>
            // 彩种代码
            ///</summary>
            [ProtoMember(4)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(5)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(6)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 订单金额
            ///</summary>
            [ProtoMember(7)]
            [Field("OrderMoney")]
            public decimal OrderMoney{ get; set; }
            /// <summary>
            // 赠送金额
            ///</summary>
            [ProtoMember(8)]
            [Field("GiveMoney")]
            public decimal GiveMoney{ get; set; }
            /// <summary>
            // 是否赠送
            ///</summary>
            [ProtoMember(9)]
            [Field("IsGive")]
            public bool IsGive{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(10)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}