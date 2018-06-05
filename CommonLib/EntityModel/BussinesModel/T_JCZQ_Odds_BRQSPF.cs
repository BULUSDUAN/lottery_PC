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
    [Entity("T_JCZQ_Odds_BRQSPF",Type = EntityType.Table)]
    public class T_JCZQ_Odds_BRQSPF
    { 
        public T_JCZQ_Odds_BRQSPF()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 比赛Id
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchId")]
            public string MatchId{ get; set; }
            /// <summary>
            // 胜 平均赔率
            ///</summary>
            [ProtoMember(3)]
            [Field("WinOdds")]
            public decimal WinOdds{ get; set; }
            /// <summary>
            // 平 平均赔率
            ///</summary>
            [ProtoMember(4)]
            [Field("FlatOdds")]
            public decimal FlatOdds{ get; set; }
            /// <summary>
            // 负 平均赔率
            ///</summary>
            [ProtoMember(5)]
            [Field("LoseOdds")]
            public decimal LoseOdds{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}