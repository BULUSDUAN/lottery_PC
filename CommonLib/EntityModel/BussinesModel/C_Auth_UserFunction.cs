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
    [Entity("C_Auth_UserFunction",Type = EntityType.Table)]
    public class C_Auth_UserFunction
    { 
        public C_Auth_UserFunction()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("IId", IsIdenty = false, IsPrimaryKey = true)]
            public int IId{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 函数编号
            ///</summary>
            [ProtoMember(3)]
            [Field("FunctionId")]
            public string FunctionId{ get; set; }
            /// <summary>
            // 状态
            ///</summary>
            [ProtoMember(4)]
            [Field("Status")]
            public int Status{ get; set; }
            /// <summary>
            // R:读；W:写
            ///</summary>
            [ProtoMember(5)]
            [Field("Mode")]
            public string Mode{ get; set; }
    }
}