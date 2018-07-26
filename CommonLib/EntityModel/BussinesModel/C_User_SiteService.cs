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
    [Entity("C_User_SiteService",Type = EntityType.Table)]
    public class C_User_SiteService
    { 
        public C_User_SiteService()
        {
        
        }
            /// <summary>
            // 主键Id
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 服务类型
            ///</summary>
            [ProtoMember(3)]
            [Field("ServiceType")]
            public int ServiceType{ get; set; }
            /// <summary>
            // 扩展字段1
            ///</summary>
            [ProtoMember(4)]
            [Field("ExtendedOne")]
            public string ExtendedOne{ get; set; }
            /// <summary>
            // 扩展字段2
            ///</summary>
            [ProtoMember(5)]
            [Field("ExtendedTwo")]
            public decimal ExtendedTwo{ get; set; }
            /// <summary>
            // 备注
            ///</summary>
            [ProtoMember(6)]
            [Field("Remarks")]
            public string Remarks{ get; set; }
            /// <summary>
            // 是否启用
            ///</summary>
            [ProtoMember(7)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
            /// <summary>
            // 修改时间
            ///</summary>
            [ProtoMember(8)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}