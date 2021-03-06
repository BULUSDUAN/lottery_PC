﻿using KaSon.FrameWork.Services.Attribute;
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
    [Entity("E_A20130807_充值送钱_认证后充200送50_充值送百分之10",Type = EntityType.Table)]
    public class E_A20130807_充值送钱_认证后充200送50_充值送百分之10
    { 
        public E_A20130807_充值送钱_认证后充200送50_充值送百分之10()
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
            // 充值金额
            ///</summary>
            [ProtoMember(4)]
            [Field("FillMoney")]
            public decimal FillMoney{ get; set; }
            /// <summary>
            // 赠送金额
            ///</summary>
            [ProtoMember(5)]
            [Field("GiveMoney")]
            public decimal GiveMoney{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(6)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}