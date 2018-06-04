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
    // 专家方案
    ////</summary>
    [ProtoContract]
    [Entity("E_Experter_Scheme",Type = EntityType.Table)]
    public class E_Experter_Scheme
    { 
        public E_Experter_Scheme()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 方案ID
            ////</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 名家ID
            ////</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 专家类别
            ////</summary>
            [ProtoMember(4)]
            [Field("ExperterType")]
            public int? ExperterType{ get; set; }
            //// <summary>
            // 投注金额
            ////</summary>
            [ProtoMember(5)]
            [Field("BetMoney")]
            public decimal? BetMoney{ get; set; }
            //// <summary>
            // 中奖金额
            ////</summary>
            [ProtoMember(6)]
            [Field("BonusMoney")]
            public decimal? BonusMoney{ get; set; }
            //// <summary>
            // 中奖状态
            ////</summary>
            [ProtoMember(7)]
            [Field("BonusStatus")]
            public int? BonusStatus{ get; set; }
            //// <summary>
            // 支持
            ////</summary>
            [ProtoMember(8)]
            [Field("Support")]
            public int? Support{ get; set; }
            //// <summary>
            // 反对
            ////</summary>
            [ProtoMember(9)]
            [Field("Against")]
            public int? Against{ get; set; }
            //// <summary>
            // 主队的评论
            ////</summary>
            [ProtoMember(10)]
            [Field("HomeTeamComments")]
            public string HomeTeamComments{ get; set; }
            //// <summary>
            // 主队的评论
            ////</summary>
            [ProtoMember(11)]
            [Field("GuestTeamComments")]
            public string GuestTeamComments{ get; set; }
            //// <summary>
            // 方案截止时间
            ////</summary>
            [ProtoMember(12)]
            [Field("StopTime")]
            public DateTime? StopTime{ get; set; }
            //// <summary>
            // 当前发布时间
            ////</summary>
            [ProtoMember(13)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(14)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}