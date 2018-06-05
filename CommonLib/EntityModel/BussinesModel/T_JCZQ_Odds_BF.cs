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
    [Entity("T_JCZQ_Odds_BF",Type = EntityType.Table)]
    public class T_JCZQ_Odds_BF
    { 
        public T_JCZQ_Odds_BF()
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
            // 胜-其它
            ///</summary>
            [ProtoMember(3)]
            [Field("S_QT")]
            public decimal S_QT{ get; set; }
            /// <summary>
            // 胜-10
            ///</summary>
            [ProtoMember(4)]
            [Field("S_10")]
            public decimal S_10{ get; set; }
            /// <summary>
            // 胜-20
            ///</summary>
            [ProtoMember(5)]
            [Field("S_20")]
            public decimal S_20{ get; set; }
            /// <summary>
            // 胜-21
            ///</summary>
            [ProtoMember(6)]
            [Field("S_21")]
            public decimal S_21{ get; set; }
            /// <summary>
            // 胜-30
            ///</summary>
            [ProtoMember(7)]
            [Field("S_30")]
            public decimal S_30{ get; set; }
            /// <summary>
            // 胜-31
            ///</summary>
            [ProtoMember(8)]
            [Field("S_31")]
            public decimal S_31{ get; set; }
            /// <summary>
            // 胜-32
            ///</summary>
            [ProtoMember(9)]
            [Field("S_32")]
            public decimal S_32{ get; set; }
            /// <summary>
            // 胜-40
            ///</summary>
            [ProtoMember(10)]
            [Field("S_40")]
            public decimal S_40{ get; set; }
            /// <summary>
            // 胜-41
            ///</summary>
            [ProtoMember(11)]
            [Field("S_41")]
            public decimal S_41{ get; set; }
            /// <summary>
            // 胜-42
            ///</summary>
            [ProtoMember(12)]
            [Field("S_42")]
            public decimal S_42{ get; set; }
            /// <summary>
            // 胜-50
            ///</summary>
            [ProtoMember(13)]
            [Field("S_50")]
            public decimal S_50{ get; set; }
            /// <summary>
            // 胜-51
            ///</summary>
            [ProtoMember(14)]
            [Field("S_51")]
            public decimal S_51{ get; set; }
            /// <summary>
            // 胜-52
            ///</summary>
            [ProtoMember(15)]
            [Field("S_52")]
            public decimal S_52{ get; set; }
            /// <summary>
            // 平-其它
            ///</summary>
            [ProtoMember(16)]
            [Field("P_QT")]
            public decimal P_QT{ get; set; }
            /// <summary>
            // 平-00
            ///</summary>
            [ProtoMember(17)]
            [Field("P_00")]
            public decimal P_00{ get; set; }
            /// <summary>
            // 平-11
            ///</summary>
            [ProtoMember(18)]
            [Field("P_11")]
            public decimal P_11{ get; set; }
            /// <summary>
            // 平-22
            ///</summary>
            [ProtoMember(19)]
            [Field("P_22")]
            public decimal P_22{ get; set; }
            /// <summary>
            // 平-33
            ///</summary>
            [ProtoMember(20)]
            [Field("P_33")]
            public decimal P_33{ get; set; }
            /// <summary>
            // 负-其它
            ///</summary>
            [ProtoMember(21)]
            [Field("F_QT")]
            public decimal F_QT{ get; set; }
            /// <summary>
            // 负-01
            ///</summary>
            [ProtoMember(22)]
            [Field("F_01")]
            public decimal F_01{ get; set; }
            /// <summary>
            // 负-02
            ///</summary>
            [ProtoMember(23)]
            [Field("F_02")]
            public decimal F_02{ get; set; }
            /// <summary>
            // 负-12
            ///</summary>
            [ProtoMember(24)]
            [Field("F_12")]
            public decimal F_12{ get; set; }
            /// <summary>
            // 负-03
            ///</summary>
            [ProtoMember(25)]
            [Field("F_03")]
            public decimal F_03{ get; set; }
            /// <summary>
            // 负-13
            ///</summary>
            [ProtoMember(26)]
            [Field("F_13")]
            public decimal F_13{ get; set; }
            /// <summary>
            // 负-23
            ///</summary>
            [ProtoMember(27)]
            [Field("F_23")]
            public decimal F_23{ get; set; }
            /// <summary>
            // 负-04
            ///</summary>
            [ProtoMember(28)]
            [Field("F_04")]
            public decimal F_04{ get; set; }
            /// <summary>
            // 负-14
            ///</summary>
            [ProtoMember(29)]
            [Field("F_14")]
            public decimal F_14{ get; set; }
            /// <summary>
            // 负-23
            ///</summary>
            [ProtoMember(30)]
            [Field("F_24")]
            public decimal F_24{ get; set; }
            /// <summary>
            // 负-05
            ///</summary>
            [ProtoMember(31)]
            [Field("F_05")]
            public decimal F_05{ get; set; }
            /// <summary>
            // 负-15
            ///</summary>
            [ProtoMember(32)]
            [Field("F_15")]
            public decimal F_15{ get; set; }
            /// <summary>
            // 负-25
            ///</summary>
            [ProtoMember(33)]
            [Field("F_25")]
            public decimal F_25{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(34)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}