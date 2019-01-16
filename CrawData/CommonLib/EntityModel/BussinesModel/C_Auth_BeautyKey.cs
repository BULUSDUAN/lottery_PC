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
    // 靓号
    ///</summary>
    [ProtoContract]
    [Entity("C_Auth_BeautyKey",Type = EntityType.Table)]
    public class C_Auth_BeautyKey
    { 
        public C_Auth_BeautyKey()
        {
        
        }
            /// <summary>
            // 靓号
            ///</summary>
            [ProtoMember(1)]
            [Field("BeautyKey", IsIdenty = false, IsPrimaryKey = true)]
            public string BeautyKey{ get; set; }
            /// <summary>
            // 上一靓号
            ///</summary>
            [ProtoMember(2)]
            [Field("PrevUserKey")]
            public string PrevUserKey{ get; set; }
            /// <summary>
            // 下一靓号
            ///</summary>
            [ProtoMember(3)]
            [Field("NextUserKey")]
            public string NextUserKey{ get; set; }
            /// <summary>
            // 状态
            ///</summary>
            [ProtoMember(4)]
            [Field("Status")]
            public string Status{ get; set; }
    }
}