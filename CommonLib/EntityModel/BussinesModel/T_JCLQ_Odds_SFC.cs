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
    [Entity("T_JCLQ_Odds_SFC",Type = EntityType.Table)]
    public class T_JCLQ_Odds_SFC : EntityModel.CoreModel.JingCai_Odds
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

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "01":
                    return S_1_5;
                case "02":
                    return S_6_10;
                case "03":
                    return S_11_15;
                case "04":
                    return S_16_20;
                case "05":
                    return S_21_25;
                case "06":
                    return S_26;
                case "11":
                    return F_1_5;
                case "12":
                    return F_6_10;
                case "13":
                    return F_11_15;
                case "14":
                    return F_16_20;
                case "15":
                    return F_21_25;
                case "16":
                    return F_26;
                default:
                    throw new ArgumentException("获取胜分差赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            S_1_5 = odds.GetOdds("01");
            S_6_10 = odds.GetOdds("02");
            S_11_15 = odds.GetOdds("03");
            S_16_20 = odds.GetOdds("04");
            S_21_25 = odds.GetOdds("05");
            S_26 = odds.GetOdds("06");

            F_1_5 = odds.GetOdds("11");
            F_6_10 = odds.GetOdds("12");
            F_11_15 = odds.GetOdds("13");
            F_16_20 = odds.GetOdds("14");
            F_21_25 = odds.GetOdds("15");
            F_26 = odds.GetOdds("16");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return S_1_5.Equals(odds.GetOdds("01"))
                && S_6_10.Equals(odds.GetOdds("02"))
                && S_11_15.Equals(odds.GetOdds("03"))
                && S_16_20.Equals(odds.GetOdds("04"))
                && S_21_25.Equals(odds.GetOdds("05"))
                && S_26.Equals(odds.GetOdds("06"))

                && F_1_5.Equals(odds.GetOdds("11"))
                && F_6_10.Equals(odds.GetOdds("12"))
                && F_11_15.Equals(odds.GetOdds("13"))
                && F_16_20.Equals(odds.GetOdds("14"))
                && F_21_25.Equals(odds.GetOdds("15"))
                && F_26.Equals(odds.GetOdds("16"));
        }
        public override string GetOddsString()
        {
            return "01|" + S_1_5.ToString("F2") + ",02|" + S_6_10.ToString("F2") + ",03|" + S_11_15.ToString("F2") + ",04|" + S_16_20.ToString("F2") + ",05|" + S_21_25.ToString("F2") + ",06|" + S_26.ToString("F2")
               + ",11|" + F_1_5.ToString("F2") + ",12|" + F_6_10.ToString("F2") + ",13|" + F_11_15.ToString("F2") + ",14|" + F_16_20.ToString("F2") + ",15|" + F_21_25.ToString("F2") + ",16|" + F_26.ToString("F2");
        }
    }
}