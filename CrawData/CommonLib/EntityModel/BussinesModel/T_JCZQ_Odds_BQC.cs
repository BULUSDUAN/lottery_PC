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
    [Entity("T_JCZQ_Odds_BQC",Type = EntityType.Table)]
    public class T_JCZQ_Odds_BQC:JingCai_Odds
    { 
        public T_JCZQ_Odds_BQC()
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
            // 胜-胜
            ///</summary>
            [ProtoMember(3)]
            [Field("SH_SH_Odds")]
            public decimal SH_SH_Odds{ get; set; }
            /// <summary>
            // 胜-平
            ///</summary>
            [ProtoMember(4)]
            [Field("SH_P_Odds")]
            public decimal SH_P_Odds{ get; set; }
            /// <summary>
            // 胜-负
            ///</summary>
            [ProtoMember(5)]
            [Field("SH_F_Odds")]
            public decimal SH_F_Odds{ get; set; }
            /// <summary>
            // 平-胜
            ///</summary>
            [ProtoMember(6)]
            [Field("P_SH_Odds")]
            public decimal P_SH_Odds{ get; set; }
            /// <summary>
            // 平-平
            ///</summary>
            [ProtoMember(7)]
            [Field("P_P_Odds")]
            public decimal P_P_Odds{ get; set; }
            /// <summary>
            // 平-负
            ///</summary>
            [ProtoMember(8)]
            [Field("P_F_Odds")]
            public decimal P_F_Odds{ get; set; }
            /// <summary>
            // 负-胜
            ///</summary>
            [ProtoMember(9)]
            [Field("F_SH_Odds")]
            public decimal F_SH_Odds{ get; set; }
            /// <summary>
            // 负-平
            ///</summary>
            [ProtoMember(10)]
            [Field("F_P_Odds")]
            public decimal F_P_Odds{ get; set; }
            /// <summary>
            // 负-负
            ///</summary>
            [ProtoMember(11)]
            [Field("F_F_Odds")]
            public decimal F_F_Odds{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(12)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "33":
                    return SH_SH_Odds;
                case "31":
                    return SH_P_Odds;
                case "30":
                    return SH_F_Odds;
                case "13":
                    return P_SH_Odds;
                case "11":
                    return P_P_Odds;
                case "10":
                    return P_F_Odds;
                case "03":
                    return F_SH_Odds;
                case "01":
                    return F_P_Odds;
                case "00":
                    return F_F_Odds;
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
            SH_SH_Odds = odds.GetOdds("33");
            SH_P_Odds = odds.GetOdds("31");
            SH_F_Odds = odds.GetOdds("30");
            P_SH_Odds = odds.GetOdds("13");
            P_P_Odds = odds.GetOdds("11");
            P_F_Odds = odds.GetOdds("10");
            F_SH_Odds = odds.GetOdds("03");
            F_P_Odds = odds.GetOdds("01");
            F_F_Odds = odds.GetOdds("00");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return SH_SH_Odds.Equals(odds.GetOdds("33"))
                 && SH_P_Odds.Equals(odds.GetOdds("31"))
                 && SH_F_Odds.Equals(odds.GetOdds("30"))
                 && P_SH_Odds.Equals(odds.GetOdds("13"))
                 && P_P_Odds.Equals(odds.GetOdds("11"))
                 && P_F_Odds.Equals(odds.GetOdds("10"))
                 && F_SH_Odds.Equals(odds.GetOdds("03"))
                 && F_P_Odds.Equals(odds.GetOdds("01"))
                 && F_F_Odds.Equals(odds.GetOdds("00"));
        }
        public override string GetOddsString()
        {
            return "33|" + SH_SH_Odds.ToString("F2") + ",31|" + SH_P_Odds.ToString("F2") + ",30|" + SH_F_Odds.ToString("F2") + ",13|" + P_SH_Odds.ToString("F2") + ",11|" + P_P_Odds.ToString("F2") + ",10|" + P_F_Odds.ToString("F2") + ",03|" + F_SH_Odds.ToString("F2") + ",01|" + F_P_Odds.ToString("F2") + ",00|" + F_F_Odds.ToString("F2");
        }
    }
}