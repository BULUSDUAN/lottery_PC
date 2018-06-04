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
    // 
    ////</summary>
    [ProtoContract]
    [Entity("E_Authentication_QQ",Type = EntityType.Table)]
    public class E_Authentication_QQ
    { 
        public E_Authentication_QQ()
        {
        
        }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            //// <summary>
            // QQ号码
            ////</summary>
            [ProtoMember(2)]
            [Field("QQ")]
            public string QQ{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(3)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}