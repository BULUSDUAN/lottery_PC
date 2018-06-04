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
    [Entity("T_JCZQ_Odds_BQC",Type = EntityType.Table)]
    public class T_JCZQ_Odds_BQC
    { 
        public T_JCZQ_Odds_BQC()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 比赛Id
            ////</summary>
            [ProtoMember(2)]
            [Field("MatchId")]
            public string MatchId{ get; set; }
            //// <summary>
            // 胜-胜
            ////</summary>
            [ProtoMember(3)]
            [Field("SH_SH_Odds")]
            public decimal? SH_SH_Odds{ get; set; }
            //// <summary>
            // 胜-平
            ////</summary>
            [ProtoMember(4)]
            [Field("SH_P_Odds")]
            public decimal? SH_P_Odds{ get; set; }
            //// <summary>
            // 胜-负
            ////</summary>
            [ProtoMember(5)]
            [Field("SH_F_Odds")]
            public decimal? SH_F_Odds{ get; set; }
            //// <summary>
            // 平-胜
            ////</summary>
            [ProtoMember(6)]
            [Field("P_SH_Odds")]
            public decimal? P_SH_Odds{ get; set; }
            //// <summary>
            // 平-平
            ////</summary>
            [ProtoMember(7)]
            [Field("P_P_Odds")]
            public decimal? P_P_Odds{ get; set; }
            //// <summary>
            // 平-负
            ////</summary>
            [ProtoMember(8)]
            [Field("P_F_Odds")]
            public decimal? P_F_Odds{ get; set; }
            //// <summary>
            // 负-胜
            ////</summary>
            [ProtoMember(9)]
            [Field("F_SH_Odds")]
            public decimal? F_SH_Odds{ get; set; }
            //// <summary>
            // 负-平
            ////</summary>
            [ProtoMember(10)]
            [Field("F_P_Odds")]
            public decimal? F_P_Odds{ get; set; }
            //// <summary>
            // 负-负
            ////</summary>
            [ProtoMember(11)]
            [Field("F_F_Odds")]
            public decimal? F_F_Odds{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(12)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}