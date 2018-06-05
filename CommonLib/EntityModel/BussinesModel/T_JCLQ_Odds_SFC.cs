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
    [Entity("T_JCLQ_Odds_SFC",Type = EntityType.Table)]
    public class T_JCLQ_Odds_SFC
    { 
        public T_JCLQ_Odds_SFC()
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
            // 胜-16
            ///</summary>
            [ProtoMember(3)]
            [Field("S_1_5")]
            public decimal S_1_5{ get; set; }
            /// <summary>
            // 胜-6-10
            ///</summary>
            [ProtoMember(4)]
            [Field("S_6_10")]
            public decimal S_6_10{ get; set; }
            /// <summary>
            // 胜-11-15
            ///</summary>
            [ProtoMember(5)]
            [Field("S_11_15")]
            public decimal S_11_15{ get; set; }
            /// <summary>
            // 胜-16-20
            ///</summary>
            [ProtoMember(6)]
            [Field("S_16_20")]
            public decimal S_16_20{ get; set; }
            /// <summary>
            // 胜-21-25
            ///</summary>
            [ProtoMember(7)]
            [Field("S_21_25")]
            public decimal S_21_25{ get; set; }
            /// <summary>
            // 胜-26+
            ///</summary>
            [ProtoMember(8)]
            [Field("S_26")]
            public decimal S_26{ get; set; }
            /// <summary>
            // 负-16
            ///</summary>
            [ProtoMember(9)]
            [Field("F_1_5")]
            public decimal F_1_5{ get; set; }
            /// <summary>
            // 负-6-10
            ///</summary>
            [ProtoMember(10)]
            [Field("F_6_10")]
            public decimal F_6_10{ get; set; }
            /// <summary>
            // 负-11-15
            ///</summary>
            [ProtoMember(11)]
            [Field("F_11_15")]
            public decimal F_11_15{ get; set; }
            /// <summary>
            // 负-16-20
            ///</summary>
            [ProtoMember(12)]
            [Field("F_16_20")]
            public decimal F_16_20{ get; set; }
            /// <summary>
            // 负-21-25
            ///</summary>
            [ProtoMember(13)]
            [Field("F_21_25")]
            public decimal F_21_25{ get; set; }
            /// <summary>
            // 负-26+
            ///</summary>
            [ProtoMember(14)]
            [Field("F_26")]
            public decimal F_26{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(15)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}