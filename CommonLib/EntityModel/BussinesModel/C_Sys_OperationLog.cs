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
    // 系统操作日志
    ///</summary>
    [ProtoContract]
    [Entity("C_Sys_OperationLog",Type = EntityType.Table)]
    public class C_Sys_OperationLog
    { 
        public C_Sys_OperationLog()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 菜单名称
            ///</summary>
            [ProtoMember(2)]
            [Field("MenuName")]
            public string MenuName{ get; set; }
            /// <summary>
            // 描述
            ///</summary>
            [ProtoMember(3)]
            [Field("Description")]
            public string Description{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(4)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 开放用户编号
            ///</summary>
            [ProtoMember(5)]
            [Field("OperUserId")]
            public string OperUserId{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}