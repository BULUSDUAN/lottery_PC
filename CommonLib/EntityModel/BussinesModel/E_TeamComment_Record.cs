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
    // 球队点评记录
    ////</summary>
    [ProtoContract]
    [Entity("E_TeamComment_Record",Type = EntityType.Table)]
    public class E_TeamComment_Record
    { 
        public E_TeamComment_Record()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 点评编号
            ////</summary>
            [ProtoMember(2)]
            [Field("TeamCommentId")]
            public int? TeamCommentId{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 是否被用户支持
            ////</summary>
            [ProtoMember(4)]
            [Field("IsByTop")]
            public bool? IsByTop{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}