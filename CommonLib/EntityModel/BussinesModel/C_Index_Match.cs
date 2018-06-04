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
    [Entity("C_Index_Match",Type = EntityType.Table)]
    public class C_Index_Match
    { 
        public C_Index_Match()
        {
        
        }
            //// <summary>
            // 主键Id
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 比赛编号
            ////</summary>
            [ProtoMember(2)]
            [Field("MatchId")]
            public string MatchId{ get; set; }
            //// <summary>
            // 比赛名字
            ////</summary>
            [ProtoMember(3)]
            [Field("MatchName")]
            public string MatchName{ get; set; }
            //// <summary>
            // 队伍图片路径
            ////</summary>
            [ProtoMember(4)]
            [Field("ImgPath")]
            public string ImgPath{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}