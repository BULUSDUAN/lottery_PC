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
    [Entity("E_Menu_List",Type = EntityType.Table)]
    public class E_Menu_List
    { 
        public E_Menu_List()
        {
        
        }
            /// <summary>
            // 菜单编号
            ///</summary>
            [ProtoMember(1)]
            [Field("MenuId", IsIdenty = false, IsPrimaryKey = true)]
            public string MenuId{ get; set; }
            /// <summary>
            // 名称
            ///</summary>
            [ProtoMember(2)]
            [Field("DisplayName")]
            public string DisplayName{ get; set; }
            /// <summary>
            // 备注
            ///</summary>
            [ProtoMember(3)]
            [Field("Description")]
            public string Description{ get; set; }
            /// <summary>
            // 父菜单编号
            ///</summary>
            [ProtoMember(4)]
            [Field("ParentMenuId")]
            public string ParentMenuId{ get; set; }
            /// <summary>
            // 方法编号
            ///</summary>
            [ProtoMember(5)]
            [Field("FunctionId")]
            public string FunctionId{ get; set; }
            /// <summary>
            // 路径
            ///</summary>
            [ProtoMember(6)]
            [Field("Url")]
            public string Url{ get; set; }
            /// <summary>
            // 菜单类型
            ///</summary>
            [ProtoMember(7)]
            [Field("MenuType")]
            public int MenuType{ get; set; }
            /// <summary>
            // 是否启用
            ///</summary>
            [ProtoMember(8)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
    }
}