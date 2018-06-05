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
    [Entity("E_TeamVote_Record",Type = EntityType.Table)]
    public class E_TeamVote_Record
    { 
        public E_TeamVote_Record()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 球队投票编号
            ///</summary>
            [ProtoMember(2)]
            [Field("TeamVoteId")]
            public int TeamVoteId{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 球队斗志 赛前状态 对阵佳绩 战绩 欧赔取向 亚培取向 的主客分类
            ///</summary>
            [ProtoMember(4)]
            [Field("Category")]
            public int Category{ get; set; }
            /// <summary>
            // 是否投票给主队
            ///</summary>
            [ProtoMember(5)]
            [Field("VoteToHomeTeam")]
            public bool VoteToHomeTeam{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}