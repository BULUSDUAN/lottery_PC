using EntityModel.CoreModel;
using EntityModel.Ticket;
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
    public class T_JCZQ_Odds_ZJQ : JingCai_Odds
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

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "0":
                    return JinQiu_0_Odds;
                case "1":
                    return JinQiu_1_Odds;
                case "2":
                    return JinQiu_2_Odds;
                case "3":
                    return JinQiu_3_Odds;
                case "4":
                    return JinQiu_4_Odds;
                case "5":
                    return JinQiu_5_Odds;
                case "6":
                    return JinQiu_6_Odds;
                case "7":
                    return JinQiu_7_Odds;
                default:
                    throw new ArgumentException("获取胜负平赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            JinQiu_0_Odds = odds.GetOdds("0");
            JinQiu_1_Odds = odds.GetOdds("1");
            JinQiu_2_Odds = odds.GetOdds("2");
            JinQiu_3_Odds = odds.GetOdds("3");
            JinQiu_4_Odds = odds.GetOdds("4");
            JinQiu_5_Odds = odds.GetOdds("5");
            JinQiu_6_Odds = odds.GetOdds("6");
            JinQiu_7_Odds = odds.GetOdds("7");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return JinQiu_0_Odds.Equals(odds.GetOdds("0"))
                && JinQiu_1_Odds.Equals(odds.GetOdds("1"))
                && JinQiu_2_Odds.Equals(odds.GetOdds("2"))
                && JinQiu_3_Odds.Equals(odds.GetOdds("3"))
                && JinQiu_4_Odds.Equals(odds.GetOdds("4"))
                && JinQiu_5_Odds.Equals(odds.GetOdds("5"))
                && JinQiu_6_Odds.Equals(odds.GetOdds("6"))
                && JinQiu_7_Odds.Equals(odds.GetOdds("7"));
        }
        public override string GetOddsString()
        {
            return "0|" + JinQiu_0_Odds.ToString("F2") + ",1|" + JinQiu_1_Odds.ToString("F2") + ",2|" + JinQiu_2_Odds.ToString("F2") + ",3|" + JinQiu_3_Odds.ToString("F2") + ",4|" + JinQiu_4_Odds.ToString("F2") + ",5|" + JinQiu_5_Odds.ToString("F2") + ",6|" + JinQiu_6_Odds.ToString("F2") + ",7|" + JinQiu_7_Odds.ToString("F2");
        }
    }
}