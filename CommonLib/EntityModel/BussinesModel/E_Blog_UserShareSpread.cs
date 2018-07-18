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
    // fxid分享推广
    ///</summary>
    [ProtoContract]
    [Entity("E_Blog_UserShareSpread",Type = EntityType.Table)]
    public class E_Blog_UserShareSpread
    { 
        public E_Blog_UserShareSpread()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id")]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 代理商编号
            ///</summary>
            [ProtoMember(3)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            /// <summary>
            // 是否领取注册(要求绑定银行卡)红包
            ///</summary>
            [ProtoMember(4)]
            [Field("isGiveRegisterRedBag")]
            public bool isGiveRegisterRedBag{ get; set; }
            /// <summary>
            // 是否领取首次购彩红包
            ///</summary>
            [ProtoMember(5)]
            [Field("isGiveLotteryRedBag")]
            public bool isGiveLotteryRedBag{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
            /// <summary>
            // 赠送红包金额
            ///</summary>
            [ProtoMember(8)]
            [Field("giveRedBagMoney")]
            public decimal giveRedBagMoney{ get; set; }

        /// <summary>
        // 是否领取充值赠送红包
        ///</summary>
        [ProtoMember(9)]
        [Field("isGiveRechargeRedBag")]
        public bool isGiveRechargeRedBag { get; set; }
        

    }
}