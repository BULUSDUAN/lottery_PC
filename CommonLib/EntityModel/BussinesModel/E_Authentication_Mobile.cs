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
    [Entity("E_Authentication_Mobile",Type = EntityType.Table)]
    public class E_Authentication_Mobile
    { 
        public E_Authentication_Mobile()
        {
        
        }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // 是否设置手机号码
            ///</summary>
            [ProtoMember(2)]
            [Field("IsSettedMobile")]
            public bool IsSettedMobile{ get; set; }
            /// <summary>
            // 认证来源
            ///</summary>
            [ProtoMember(3)]
            [Field("AuthFrom")]
            public string AuthFrom{ get; set; }
            /// <summary>
            // 手机号码
            ///</summary>
            [ProtoMember(4)]
            [Field("Mobile")]
            public string Mobile{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            /// <summary>
            // 创建自
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateBy")]
            public string CreateBy{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
            /// <summary>
            // 更新自
            ///</summary>
            [ProtoMember(8)]
            [Field("UpdateBy")]
            public string UpdateBy{ get; set; }
    }
}