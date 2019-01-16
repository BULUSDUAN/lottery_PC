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
    // 球队投票
    ///</summary>
    [ProtoContract]
    [Entity("E_TeamVote_Fans",Type = EntityType.Table)]
    public class E_TeamVote_Fans
    { 
        public E_TeamVote_Fans()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            /// <summary>
            // 彩种编码
            ///</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(3)]
            [Field("MatchDate")]
            public string MatchDate{ get; set; }
            /// <summary>
            // 场次信息
            ///</summary>
            [ProtoMember(4)]
            [Field("OrderNumber")]
            public string OrderNumber{ get; set; }
            /// <summary>
            // 比赛开始时间
            ///</summary>
            [ProtoMember(5)]
            [Field("MatchStartTime")]
            public DateTime MatchStartTime{ get; set; }
            /// <summary>
            // 球队斗志 赛前状态 对阵佳绩 战绩 欧赔取向 亚培取向 的主客分类
            ///</summary>
            [ProtoMember(6)]
            [Field("Category")]
            public int Category{ get; set; }
            /// <summary>
            // 支持主队粉丝
            ///</summary>
            [ProtoMember(7)]
            [Field("HomeTeamFans")]
            public int HomeTeamFans{ get; set; }
            /// <summary>
            // 支持客队粉丝
            ///</summary>
            [ProtoMember(8)]
            [Field("GuestTeamNameFans")]
            public int GuestTeamNameFans{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}