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
    // 宝单、大单推荐专家
    ///</summary>
    [ProtoContract]
    [Entity("E_User_SchemeShareExpert",Type = EntityType.Table)]
    public class E_User_SchemeShareExpert
    { 
        public E_User_SchemeShareExpert()
        {
        
        }
            /// <summary>
            // 主键
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
            // 专家类别：分为宝单专家和大单专家
            ///</summary>
            [ProtoMember(3)]
            [Field("ExpertType")]
            public int ExpertType{ get; set; }
            /// <summary>
            // 是否启用
            ///</summary>
            [ProtoMember(4)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
            /// <summary>
            // 显示排序号
            ///</summary>
            [ProtoMember(5)]
            [Field("ShowSort")]
            public int ShowSort{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}