using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business.Domain.Entities.Ticket;
using GameBiz.Core.Ticket;

namespace GameBiz.Business.Domain.Entities.Ticket
{
    public class JCZQ_Odds_SPF : JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public virtual decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal LoseOdds { get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinOdds;
                case "1":
                    return FlatOdds;
                case "0":
                    return LoseOdds;
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
            WinOdds = odds.GetOdds("3");
            FlatOdds = odds.GetOdds("1");
            LoseOdds = odds.GetOdds("0");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return WinOdds.Equals(odds.GetOdds("3"))
                && FlatOdds.Equals(odds.GetOdds("1"))
                && LoseOdds.Equals(odds.GetOdds("0"));
        }
        public override string GetOddsString()
        {
            return "3|" + WinOdds.ToString("F2") + ",1|" + FlatOdds.ToString("F2") + ",0|" + LoseOdds.ToString("F2");
        }
    }

    public class JCZQ_Odds_BRQSPF : JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public virtual decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal LoseOdds { get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinOdds;
                case "1":
                    return FlatOdds;
                case "0":
                    return LoseOdds;
                default:
                    throw new ArgumentException("获取不让球胜负平赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            WinOdds = odds.GetOdds("3");
            FlatOdds = odds.GetOdds("1");
            LoseOdds = odds.GetOdds("0");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return WinOdds.Equals(odds.GetOdds("3"))
                && FlatOdds.Equals(odds.GetOdds("1"))
                && LoseOdds.Equals(odds.GetOdds("0"));
        }
        public override string GetOddsString()
        {
            return "3|" + WinOdds.ToString("F2") + ",1|" + FlatOdds.ToString("F2") + ",0|" + LoseOdds.ToString("F2");
        }
    }

    public class JCZQ_Odds_BF : JingCai_Odds
    {
        /// <summary>
        /// 胜-其它
        /// </summary>
        public virtual decimal S_QT { get; set; }
        /// <summary>
        /// 胜-10
        /// </summary>
        public virtual decimal S_10 { get; set; }
        /// <summary>
        /// 胜-20
        /// </summary>
        public virtual decimal S_20 { get; set; }
        /// <summary>
        /// 胜-21
        /// </summary>
        public virtual decimal S_21 { get; set; }
        /// <summary>
        /// 胜-30
        /// </summary>
        public virtual decimal S_30 { get; set; }
        /// <summary>
        /// 胜-31
        /// </summary>
        public virtual decimal S_31 { get; set; }
        /// <summary>
        /// 胜-32
        /// </summary>
        public virtual decimal S_32 { get; set; }
        /// <summary>
        /// 胜-40
        /// </summary>
        public virtual decimal S_40 { get; set; }
        /// <summary>
        /// 胜-41
        /// </summary>
        public virtual decimal S_41 { get; set; }
        /// <summary>
        /// 胜-42
        /// </summary>
        public virtual decimal S_42 { get; set; }
        /// <summary>
        /// 胜-50
        /// </summary>
        public virtual decimal S_50 { get; set; }
        /// <summary>
        /// 胜-51
        /// </summary>
        public virtual decimal S_51 { get; set; }
        /// <summary>
        /// 胜-52
        /// </summary>
        public virtual decimal S_52 { get; set; }
        /// <summary>
        /// 平-其它
        /// </summary>
        public virtual decimal P_QT { get; set; }
        /// <summary>
        /// 平-00
        /// </summary>
        public virtual decimal P_00 { get; set; }
        /// <summary>
        /// 平-11
        /// </summary>
        public virtual decimal P_11 { get; set; }
        /// <summary>
        /// 平-22
        /// </summary>
        public virtual decimal P_22 { get; set; }
        /// <summary>
        /// 平-33
        /// </summary>
        public virtual decimal P_33 { get; set; }
        /// <summary>
        /// 负-其它
        /// </summary>
        public virtual decimal F_QT { get; set; }
        /// <summary>
        /// 负-01
        /// </summary>
        public virtual decimal F_01 { get; set; }
        /// <summary>
        /// 负-02
        /// </summary>
        public virtual decimal F_02 { get; set; }
        /// <summary>
        /// 负-12
        /// </summary>
        public virtual decimal F_12 { get; set; }
        /// <summary>
        /// 负-03
        /// </summary>
        public virtual decimal F_03 { get; set; }
        /// <summary>
        /// 负-13
        /// </summary>
        public virtual decimal F_13 { get; set; }
        /// <summary>
        /// 负-23
        /// </summary>
        public virtual decimal F_23 { get; set; }
        /// <summary>
        /// 负-04
        /// </summary>
        public virtual decimal F_04 { get; set; }
        /// <summary>
        /// 负-14
        /// </summary>
        public virtual decimal F_14 { get; set; }
        /// <summary>
        /// 负-24
        /// </summary>
        public virtual decimal F_24 { get; set; }
        /// <summary>
        /// 负-05
        /// </summary>
        public virtual decimal F_05 { get; set; }
        /// <summary>
        /// 负-15
        /// </summary>
        public virtual decimal F_15 { get; set; }
        /// <summary>
        /// 负-25
        /// </summary>
        public virtual decimal F_25 { get; set; }

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

    public class JCZQ_Odds_ZJQ : JingCai_Odds
    {
        /// <summary>
        /// 进球数 0
        /// </summary>
        public virtual decimal JinQiu_0_Odds { get; set; }
        /// <summary>
        /// 进球数 1
        /// </summary>
        public virtual decimal JinQiu_1_Odds { get; set; }
        /// <summary>
        /// 进球数 2
        /// </summary>
        public virtual decimal JinQiu_2_Odds { get; set; }
        /// <summary>
        /// 进球数 3
        /// </summary>
        public virtual decimal JinQiu_3_Odds { get; set; }
        /// <summary>
        /// 进球数 4
        /// </summary>
        public virtual decimal JinQiu_4_Odds { get; set; }
        /// <summary>
        /// 进球数 5
        /// </summary>
        public virtual decimal JinQiu_5_Odds { get; set; }
        /// <summary>
        /// 进球数 6
        /// </summary>
        public virtual decimal JinQiu_6_Odds { get; set; }
        /// <summary>
        /// 进球数 7
        /// </summary>
        public virtual decimal JinQiu_7_Odds { get; set; }

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

    public class JCZQ_Odds_BQC : JingCai_Odds
    {
        /// <summary>
        /// 胜-胜
        /// </summary>
        public virtual decimal SH_SH_Odds { get; set; }
        /// <summary>
        /// 胜-平
        /// </summary>
        public virtual decimal SH_P_Odds { get; set; }
        /// <summary>
        /// 胜-负
        /// </summary>
        public virtual decimal SH_F_Odds { get; set; }
        /// <summary>
        /// 平-胜
        /// </summary>
        public virtual decimal P_SH_Odds { get; set; }
        /// <summary>
        /// 平-平
        /// </summary>
        public virtual decimal P_P_Odds { get; set; }
        /// <summary>
        /// 平-负
        /// </summary>
        public virtual decimal P_F_Odds { get; set; }
        /// <summary>
        /// 负-胜
        /// </summary>
        public virtual decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 负-平
        /// </summary>
        public virtual decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负-负
        /// </summary>
        public virtual decimal F_F_Odds { get; set; }

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
