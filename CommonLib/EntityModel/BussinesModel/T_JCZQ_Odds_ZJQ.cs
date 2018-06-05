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
    [Entity("T_JCZQ_Odds_ZJQ",Type = EntityType.Table)]
    public class T_JCZQ_Odds_ZJQ
    { 
        public T_JCZQ_Odds_ZJQ()
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
            // 进球数 0
            ///</summary>
            [ProtoMember(3)]
            [Field("JinQiu_0_Odds")]
            public decimal JinQiu_0_Odds{ get; set; }
            /// <summary>
            // 进球数 1
            ///</summary>
            [ProtoMember(4)]
            [Field("JinQiu_1_Odds")]
            public decimal JinQiu_1_Odds{ get; set; }
            /// <summary>
            // 进球数 2
            ///</summary>
            [ProtoMember(5)]
            [Field("JinQiu_2_Odds")]
            public decimal JinQiu_2_Odds{ get; set; }
            /// <summary>
            // 进球数 3
            ///</summary>
            [ProtoMember(6)]
            [Field("JinQiu_3_Odds")]
            public decimal JinQiu_3_Odds{ get; set; }
            /// <summary>
            // 进球数 4
            ///</summary>
            [ProtoMember(7)]
            [Field("JinQiu_4_Odds")]
            public decimal JinQiu_4_Odds{ get; set; }
            /// <summary>
            // 进球数 5
            ///</summary>
            [ProtoMember(8)]
            [Field("JinQiu_5_Odds")]
            public decimal JinQiu_5_Odds{ get; set; }
            /// <summary>
            // 进球数 6
            ///</summary>
            [ProtoMember(9)]
            [Field("JinQiu_6_Odds")]
            public decimal JinQiu_6_Odds{ get; set; }
            /// <summary>
            // 进球数 7
            ///</summary>
            [ProtoMember(10)]
            [Field("JinQiu_7_Odds")]
            public decimal JinQiu_7_Odds{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}