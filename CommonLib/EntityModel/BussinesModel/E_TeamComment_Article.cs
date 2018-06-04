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
    // 球队点评
    ////</summary>
    [ProtoContract]
    [Entity("E_TeamComment_Article",Type = EntityType.Table)]
    public class E_TeamComment_Article
    { 
        public E_TeamComment_Article()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 彩种编码
            ////</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 期号
            ////</summary>
            [ProtoMember(4)]
            [Field("MatchDate")]
            public string MatchDate{ get; set; }
            //// <summary>
            // 场次信息
            ////</summary>
            [ProtoMember(5)]
            [Field("OrderNumber")]
            public string OrderNumber{ get; set; }
            //// <summary>
            // 联赛名称
            ////</summary>
            [ProtoMember(6)]
            [Field("LeagueName")]
            public string LeagueName{ get; set; }
            //// <summary>
            // 主队信息
            ////</summary>
            [ProtoMember(7)]
            [Field("HomeTeamName")]
            public string HomeTeamName{ get; set; }
            //// <summary>
            // 客队信息
            ////</summary>
            [ProtoMember(8)]
            [Field("GuestTeamName")]
            public string GuestTeamName{ get; set; }
            //// <summary>
            // 比赛时间
            ////</summary>
            [ProtoMember(9)]
            [Field("MatchTime")]
            public DateTime? MatchTime{ get; set; }
            //// <summary>
            // 用户的发布文章
            ////</summary>
            [ProtoMember(10)]
            [Field("ArticleContent")]
            public string ArticleContent{ get; set; }
            //// <summary>
            // 被用户支持
            ////</summary>
            [ProtoMember(11)]
            [Field("ByTop")]
            public int? ByTop{ get; set; }
            //// <summary>
            // 被用户鄙视
            ////</summary>
            [ProtoMember(12)]
            [Field("ByTrample")]
            public int? ByTrample{ get; set; }
            //// <summary>
            // 发布时间
            ////</summary>
            [ProtoMember(13)]
            [Field("PublishTime")]
            public DateTime? PublishTime{ get; set; }
    }
}