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
    // yqid普通用户推广 领取红包
    ///</summary>
    [ProtoContract]
    [Entity("E_Blog_UserSpreadGiveRedBag",Type = EntityType.Table)]
    public class E_Blog_UserSpreadGiveRedBag
    { 
        public E_Blog_UserSpreadGiveRedBag()
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
            // 红包
            ///</summary>
            [ProtoMember(3)]
            [Field("giveRedBagMoney")]
            public decimal giveRedBagMoney{ get; set; }
            /// <summary>
            // 现金(预留字段)
            ///</summary>
            [ProtoMember(4)]
            [Field("GiveBonusMoney")]
            public decimal GiveBonusMoney{ get; set; }
            /// <summary>
            // 满足条件的会员个数
            ///</summary>
            [ProtoMember(5)]
            [Field("userCount")]
            public int userCount{ get; set; }
            /// <summary>
            // 领取的红包个数
            ///</summary>
            [ProtoMember(6)]
            [Field("userGiveCount")]
            public int userGiveCount{ get; set; }
            /// <summary>
            // 当前领取了红包的会员数 比如10个满足条件的会员领取了一次 20个满足条件的也可以领取一次(这里值10 20 30 ...)
            ///</summary>
            [ProtoMember(7)]
            [Field("redBagCount")]
            public int redBagCount{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(9)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}