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
    [Entity("E_A20150919_充值送红包配置",Type = EntityType.Table)]
    public class E_A20150919_充值送红包配置
    { 
        public E_A20150919_充值送红包配置()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 充值金额
            ///</summary>
            [ProtoMember(2)]
            [Field("FillMoney")]
            public decimal FillMoney{ get; set; }
            /// <summary>
            // 赠送金额
            ///</summary>
            [ProtoMember(3)]
            [Field("GiveMoney")]
            public decimal GiveMoney{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}