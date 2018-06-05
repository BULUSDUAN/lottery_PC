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
    // 用户获取奖品
    ///</summary>
    [ProtoContract]
    [Entity("C_User_GetPrize",Type = EntityType.Table)]
    public class C_User_GetPrize
    { 
        public C_User_GetPrize()
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
            // 订单金额
            ///</summary>
            [ProtoMember(3)]
            [Field("OrderMoney")]
            public decimal OrderMoney{ get; set; }
            /// <summary>
            // 奖品类型[10:IPHONE5s；20：QQ3；30：送2元；40：送5元；50：送10元；60：送58元；70：送588元；80：送2888元]
            ///</summary>
            [ProtoMember(4)]
            [Field("PrizeType")]
            public string PrizeType{ get; set; }
            /// <summary>
            // 积分
            ///</summary>
            [ProtoMember(5)]
            [Field("PayInegral")]
            public int PayInegral{ get; set; }
            /// <summary>
            // 付款金额
            ///</summary>
            [ProtoMember(6)]
            [Field("GiveMoney")]
            public decimal GiveMoney{ get; set; }
            /// <summary>
            // 备注
            ///</summary>
            [ProtoMember(7)]
            [Field("Summary")]
            public string Summary{ get; set; }
            /// <summary>
            // 奖品类型[10:IPHONE5s；20：QQ3；30：送2元；40：送5元；50：送10元；60：送58元；70：送588元；80：送2888元
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}