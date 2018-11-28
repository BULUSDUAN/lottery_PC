using EntityModel.Interface;
using EntityModel.Ticket;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
    [Entity("T_JCZQ_Odds_BF", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class T_JCZQ_Odds_BF : JingCai_Odds, IMatchData
    {
        public T_JCZQ_Odds_BF()
        {

        }
        [BsonId]
        public ObjectId _id { get; set; }
        /// <summary>
        // 主键
        ///</summary>
        [ProtoMember(1)]
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        // 比赛Id
        ///</summary>
        [ProtoMember(2)]
        [Field("MatchId")]
        public string MatchId { get; set; }
        /// <summary>
        // 胜-其它
        ///</summary>
        [ProtoMember(3)]
        [Field("S_QT")]
        public decimal S_QT { get; set; }
        /// <summary>
        // 胜-10
        ///</summary>
        [ProtoMember(4)]
        [Field("S_10")]
        public decimal S_10 { get; set; }
        /// <summary>
        // 胜-20
        ///</summary>
        [ProtoMember(5)]
        [Field("S_20")]
        public decimal S_20 { get; set; }
        /// <summary>
        // 胜-21
        ///</summary>
        [ProtoMember(6)]
        [Field("S_21")]
        public decimal S_21 { get; set; }
        /// <summary>
        // 胜-30
        ///</summary>
        [ProtoMember(7)]
        [Field("S_30")]
        public decimal S_30 { get; set; }
        /// <summary>
        // 胜-31
        ///</summary>
        [ProtoMember(8)]
        [Field("S_31")]
        public decimal S_31 { get; set; }
        /// <summary>
        // 胜-32
        ///</summary>
        [ProtoMember(9)]
        [Field("S_32")]
        public decimal S_32 { get; set; }
        /// <summary>
        // 胜-40
        ///</summary>
        [ProtoMember(10)]
        [Field("S_40")]
        public decimal S_40 { get; set; }
        /// <summary>
        // 胜-41
        ///</summary>
        [ProtoMember(11)]
        [Field("S_41")]
        public decimal S_41 { get; set; }
        /// <summary>
        // 胜-42
        ///</summary>
        [ProtoMember(12)]
        [Field("S_42")]
        public decimal S_42 { get; set; }
        /// <summary>
        // 胜-50
        ///</summary>
        [ProtoMember(13)]
        [Field("S_50")]
        public decimal S_50 { get; set; }
        /// <summary>
        // 胜-51
        ///</summary>
        [ProtoMember(14)]
        [Field("S_51")]
        public decimal S_51 { get; set; }
        /// <summary>
        // 胜-52
        ///</summary>
        [ProtoMember(15)]
        [Field("S_52")]
        public decimal S_52 { get; set; }
        /// <summary>
        // 平-其它
        ///</summary>
        [ProtoMember(16)]
        [Field("P_QT")]
        public decimal P_QT { get; set; }
        /// <summary>
        // 平-00
        ///</summary>
        [ProtoMember(17)]
        [Field("P_00")]
        public decimal P_00 { get; set; }
        /// <summary>
        // 平-11
        ///</summary>
        [ProtoMember(18)]
        [Field("P_11")]
        public decimal P_11 { get; set; }
        /// <summary>
        // 平-22
        ///</summary>
        [ProtoMember(19)]
        [Field("P_22")]
        public decimal P_22 { get; set; }
        /// <summary>
        // 平-33
        ///</summary>
        [ProtoMember(20)]
        [Field("P_33")]
        public decimal P_33 { get; set; }
        /// <summary>
        // 负-其它
        ///</summary>
        [ProtoMember(21)]
        [Field("F_QT")]
        public decimal F_QT { get; set; }
        /// <summary>
        // 负-01
        ///</summary>
        [ProtoMember(22)]
        [Field("F_01")]
        public decimal F_01 { get; set; }
        /// <summary>
        // 负-02
        ///</summary>
        [ProtoMember(23)]
        [Field("F_02")]
        public decimal F_02 { get; set; }
        /// <summary>
        // 负-12
        ///</summary>
        [ProtoMember(24)]
        [Field("F_12")]
        public decimal F_12 { get; set; }
        /// <summary>
        // 负-03
        ///</summary>
        [ProtoMember(25)]
        [Field("F_03")]
        public decimal F_03 { get; set; }
        /// <summary>
        // 负-13
        ///</summary>
        [ProtoMember(26)]
        [Field("F_13")]
        public decimal F_13 { get; set; }
        /// <summary>
        // 负-23
        ///</summary>
        [ProtoMember(27)]
        [Field("F_23")]
        public decimal F_23 { get; set; }
        /// <summary>
        // 负-04
        ///</summary>
        [ProtoMember(28)]
        [Field("F_04")]
        public decimal F_04 { get; set; }
        /// <summary>
        // 负-14
        ///</summary>
        [ProtoMember(29)]
        [Field("F_14")]
        public decimal F_14 { get; set; }
        /// <summary>
        // 负-23
        ///</summary>
        [ProtoMember(30)]
        [Field("F_24")]
        public decimal F_24 { get; set; }
        /// <summary>
        // 负-05
        ///</summary>
        [ProtoMember(31)]
        [Field("F_05")]
        public decimal F_05 { get; set; }
        /// <summary>
        // 负-15
        ///</summary>
        [ProtoMember(32)]
        [Field("F_15")]
        public decimal F_15 { get; set; }
        /// <summary>
        // 负-25
        ///</summary>
        [ProtoMember(33)]
        [Field("F_25")]
        public decimal F_25 { get; set; }
        /// <summary>
        // 创建时间
        ///</summary>
        [ProtoMember(34)]
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }

        public string MatchData { get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "X0":
                    return S_QT;
                case "52":
                    return S_52;
                case "51":
                    return S_51;
                case "50":
                    return S_50;
                case "42":
                    return S_42;
                case "41":
                    return S_41;
                case "40":
                    return S_40;
                case "32":
                    return S_32;
                case "31":
                    return S_31;
                case "30":
                    return S_30;
                case "21":
                    return S_21;
                case "20":
                    return S_20;
                case "10":
                    return S_10;
                case "XX":
                    return P_QT;
                case "33":
                    return P_33;
                case "22":
                    return P_22;
                case "11":
                    return P_11;
                case "00":
                    return P_00;
                case "0X":
                    return F_QT;
                case "25":
                    return F_25;
                case "15":
                    return F_15;
                case "05":
                    return F_05;
                case "24":
                    return F_24;
                case "14":
                    return F_14;
                case "04":
                    return F_04;
                case "23":
                    return F_23;
                case "13":
                    return F_13;
                case "03":
                    return F_03;
                case "12":
                    return F_12;
                case "02":
                    return F_02;
                case "01":
                    return F_01;
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
            S_QT = odds.GetOdds("X0");
            S_52 = odds.GetOdds("52");
            S_51 = odds.GetOdds("51");
            S_50 = odds.GetOdds("50");
            S_42 = odds.GetOdds("42");
            S_41 = odds.GetOdds("41");
            S_40 = odds.GetOdds("40");
            S_32 = odds.GetOdds("32");
            S_31 = odds.GetOdds("31");
            S_30 = odds.GetOdds("30");
            S_21 = odds.GetOdds("21");
            S_20 = odds.GetOdds("20");
            S_10 = odds.GetOdds("10");

            P_QT = odds.GetOdds("XX");
            P_33 = odds.GetOdds("33");
            P_22 = odds.GetOdds("22");
            P_11 = odds.GetOdds("11");
            P_00 = odds.GetOdds("00");

            F_QT = odds.GetOdds("0X");
            F_25 = odds.GetOdds("25");
            F_15 = odds.GetOdds("15");
            F_05 = odds.GetOdds("05");
            F_24 = odds.GetOdds("24");
            F_14 = odds.GetOdds("14");
            F_04 = odds.GetOdds("04");
            F_23 = odds.GetOdds("23");
            F_13 = odds.GetOdds("13");
            F_03 = odds.GetOdds("03");
            F_12 = odds.GetOdds("12");
            F_02 = odds.GetOdds("02");
            F_01 = odds.GetOdds("01");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return S_QT.Equals(odds.GetOdds("X0"))
                && S_52.Equals(odds.GetOdds("52"))
                && S_51.Equals(odds.GetOdds("51"))
                && S_50.Equals(odds.GetOdds("50"))
                && S_42.Equals(odds.GetOdds("42"))
                && S_41.Equals(odds.GetOdds("41"))
                && S_40.Equals(odds.GetOdds("40"))
                && S_32.Equals(odds.GetOdds("32"))
                && S_31.Equals(odds.GetOdds("31"))
                && S_30.Equals(odds.GetOdds("30"))
                && S_21.Equals(odds.GetOdds("21"))
                && S_20.Equals(odds.GetOdds("20"))
                && S_10.Equals(odds.GetOdds("10"))

                && P_QT.Equals(odds.GetOdds("XX"))
                && P_33.Equals(odds.GetOdds("33"))
                && P_22.Equals(odds.GetOdds("22"))
                && P_11.Equals(odds.GetOdds("11"))
                && P_00.Equals(odds.GetOdds("00"))

                && F_QT.Equals(odds.GetOdds("0X"))
                && F_25.Equals(odds.GetOdds("25"))
                && F_15.Equals(odds.GetOdds("15"))
                && F_05.Equals(odds.GetOdds("05"))
                && F_24.Equals(odds.GetOdds("24"))
                && F_14.Equals(odds.GetOdds("14"))
                && F_04.Equals(odds.GetOdds("04"))
                && F_23.Equals(odds.GetOdds("23"))
                && F_13.Equals(odds.GetOdds("13"))
                && F_03.Equals(odds.GetOdds("03"))
                && F_12.Equals(odds.GetOdds("12"))
                && F_02.Equals(odds.GetOdds("02"))
                && F_01.Equals(odds.GetOdds("01"));
        }

        public override string GetOddsString()
        {
            return "X0|" + S_QT.ToString("F2") + ",52|" + S_52.ToString("F2") + ",51|" + S_51.ToString("F2") + ",50|" + S_50.ToString("F2") + ",42|" + S_42.ToString("F2") + ",41|" + S_41.ToString("F2") + ",40|" + S_40.ToString("F2") + ",32|" + S_32.ToString("F2") + ",31|" + S_31.ToString("F2") + ",30|" + S_30.ToString("F2") + ",21|" + S_21.ToString("F2") + ",20|" + S_20.ToString("F2") + ",10|" + S_10.ToString("F2")
                + ",XX|" + P_QT.ToString("F2") + ",00|" + P_00.ToString("F2") + ",11|" + P_11.ToString("F2") + ",22|" + P_22.ToString("F2") + ",33|" + P_33.ToString("F2")
                + ",0X|" + F_QT.ToString("F2") + ",25|" + F_25.ToString("F2") + ",15|" + F_15.ToString("F2") + ",05|" + F_05.ToString("F2") + ",24|" + F_24.ToString("F2") + ",14|" + F_14.ToString("F2") + ",04|" + F_04.ToString("F2") + ",23|" + F_23.ToString("F2") + ",13|" + F_13.ToString("F2") + ",03|" + F_03.ToString("F2") + ",12|" + F_12.ToString("F2") + ",02|" + F_02.ToString("F2") + ",01|" + F_01.ToString("F2");
        }
    }
}