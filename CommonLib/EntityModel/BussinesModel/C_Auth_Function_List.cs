using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 功能权限
    ////</summary>
    [ProtoContract]
    [Entity("C_Auth_Function_List",Type = EntityType.Table)]
    public class C_Auth_Function_List
    { 
        public C_Auth_Function_List()
        {
        
        }
            //// <summary>
            // 权限编号
            ////</summary>
            [ProtoMember(1)]
            [Field("FunctionId", IsIdenty = false, IsPrimaryKey = true)]
            public string FunctionId{ get; set; }
            //// <summary>
            // 是否前台的基础功能
            ////</summary>
            [ProtoMember(2)]
            [Field("IsWebBasic")]
            public bool? IsWebBasic{ get; set; }
            //// <summary>
            // 是否后台的基础功能
            ////</summary>
            [ProtoMember(3)]
            [Field("IsBackBasic")]
            public bool? IsBackBasic{ get; set; }
            //// <summary>
            // 显示名称
            ////</summary>
            [ProtoMember(4)]
            [Field("DisplayName")]
            public string DisplayName{ get; set; }
            //// <summary>
            // 上级节点Id
            ////</summary>
            [ProtoMember(5)]
            [Field("ParentId")]
            public string ParentId{ get; set; }
            //// <summary>
            // 节点路径
            ////</summary>
            [ProtoMember(6)]
            [Field("ParentPath")]
            public string ParentPath{ get; set; }
    }
}