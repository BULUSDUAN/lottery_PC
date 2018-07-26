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
    [Entity("C_UserUpdateMoney_Alipay",Type = EntityType.Table)]
    public class C_UserUpdateMoney_Alipay
    { 
        public C_UserUpdateMoney_Alipay()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("id", IsIdenty = true, IsPrimaryKey = true)]
            public int id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("userid")]
            public string userid{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("updateStatus")]
            public bool? updateStatus{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("createTime")]
            public DateTime? createTime{ get; set; }
    }
}