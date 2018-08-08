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
    // 函数要求的权限信息
    ///</summary>
    [ProtoContract]
    [Entity("C_Auth_MethodFunction_List_New", Type = EntityType.Table)]
    public class C_Auth_MethodFunction_List
    { 
        public C_Auth_MethodFunction_List()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 方法全名：命名空间.类名.方法名
            ///</summary>
            [ProtoMember(2)]
            [Field("MethodFullName")]
            public string MethodFullName{ get; set; }
            /// <summary>
            // 权限编码
            ///</summary>
            [ProtoMember(3)]
            [Field("FunctionId")]
            public string FunctionId{ get; set; }
            /// <summary>
            // R:读；W:写
            ///</summary>
            [ProtoMember(4)]
            [Field("Mode")]
            public string Mode{ get; set; }
            /// <summary>
            // 方法描述
            ///</summary>
            [ProtoMember(5)]
            [Field("Description")]
            public string Description{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}