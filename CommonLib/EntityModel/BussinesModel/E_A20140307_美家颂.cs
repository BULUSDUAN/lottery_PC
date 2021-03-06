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
    [Entity("E_A20140307_美家颂",Type = EntityType.Table)]
    public class E_A20140307_美家颂
    { 
        public E_A20140307_美家颂()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 美家颂号码
            ///</summary>
            [ProtoMember(2)]
            [Field("MJSNumber")]
            public string MJSNumber{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("BelongUserId")]
            public string BelongUserId{ get; set; }
            /// <summary>
            // 使用时间
            ///</summary>
            [ProtoMember(4)]
            [Field("UsedTime")]
            public DateTime UsedTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}