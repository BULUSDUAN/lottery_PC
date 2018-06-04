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
    // 名家吐槽
    ////</summary>
    [ProtoContract]
    [Entity("E_Experter_Comments",Type = EntityType.Table)]
    public class E_Experter_Comments
    { 
        public E_Experter_Comments()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 专家编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 推荐方案
            ////</summary>
            [ProtoMember(3)]
            [Field("RecommendSchemeId")]
            public string RecommendSchemeId{ get; set; }
            //// <summary>
            // 分析方案编号
            ////</summary>
            [ProtoMember(4)]
            [Field("AnalyzeSchemeId")]
            public string AnalyzeSchemeId{ get; set; }
            //// <summary>
            // 吐槽内容
            ////</summary>
            [ProtoMember(5)]
            [Field("Content")]
            public string Content{ get; set; }
            //// <summary>
            // 发送用户编号
            ////</summary>
            [ProtoMember(6)]
            [Field("SendUserId")]
            public string SendUserId{ get; set; }
            //// <summary>
            // 处理类别
            ////</summary>
            [ProtoMember(7)]
            [Field("DealWithType")]
            public int? DealWithType{ get; set; }
            //// <summary>
            // 吐槽类别
            ////</summary>
            [ProtoMember(8)]
            [Field("CommentsTpye")]
            public int? CommentsTpye{ get; set; }
            //// <summary>
            // 处理意见
            ////</summary>
            [ProtoMember(9)]
            [Field("DisposeOpinion")]
            public string DisposeOpinion{ get; set; }
            //// <summary>
            // 当前发布时间
            ////</summary>
            [ProtoMember(10)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}