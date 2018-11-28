using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core.Ticket;

namespace GameBiz.Business.Domain.Entities.Ticket
{
    public class JCLQ_Odds_SF : JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal WinOdds { get; set; }
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
                case "0":
                    return LoseOdds;
                default:
                    throw new ArgumentException("获取胜负赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            WinOdds = odds.GetOdds("3");
            LoseOdds = odds.GetOdds("0");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return WinOdds.Equals(odds.GetOdds("3"))
                && LoseOdds.Equals(odds.GetOdds("0"));
        }
        public override string GetOddsString()
        {
            return "3|" + WinOdds.ToString("F2") + ",0|" + LoseOdds.ToString("F2");
        }
    }

    public class JCLQ_Odds_RFSF : JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal WinOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal LoseOdds { get; set; }
        /// <summary>
        /// 让分数
        /// </summary>
        public virtual decimal RF { get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinOdds;
                case "0":
                    return LoseOdds;
                case "RF":
                    return RF;
                default:
                    throw new ArgumentException("获取让分胜负赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            if (RF.ToString("N2").EndsWith(".00"))
            {
                return false;
            }
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            WinOdds = odds.GetOdds("3");
            LoseOdds = odds.GetOdds("0");
            RF = odds.GetOdds("RF");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return WinOdds.Equals(odds.GetOdds("3"))
                && LoseOdds.Equals(odds.GetOdds("0"))
                && RF.Equals(odds.GetOdds("RF"));
        }
        public override string GetOddsString()
        {
            return "3|" + WinOdds.ToString("F2") + ",0|" + LoseOdds.ToString("F2") + ",RF|" + RF.ToString("F2");
        }
        //public virtual override string GetOddsStringToRF()
        //{
        //    return "3|" + WinOdds + ",0|" + LoseOdds + ",RF|" + RF;
        //}
    }

    public class JCLQ_Odds_SFC : JingCai_Odds
    {
        /// <summary>
        /// 胜-16
        /// </summary>
        public virtual decimal S_1_5 { get; set; }
        /// <summary>
        /// 胜-6-10
        /// </summary>
        public virtual decimal S_6_10 { get; set; }
        /// <summary>
        /// 胜-11-15
        /// </summary>
        public virtual decimal S_11_15 { get; set; }
        /// <summary>
        /// 胜-16-20
        /// </summary>
        public virtual decimal S_16_20 { get; set; }
        /// <summary>
        /// 胜-21-25
        /// </summary>
        public virtual decimal S_21_25 { get; set; }
        /// <summary>
        /// 胜-26+
        /// </summary>
        public virtual decimal S_26 { get; set; }
        /// <summary>
        /// 负-16
        /// </summary>
        public virtual decimal F_1_5 { get; set; }
        /// <summary>
        /// 负-6-10
        /// </summary>
        public virtual decimal F_6_10 { get; set; }
        /// <summary>
        /// 负-11-15
        /// </summary>
        public virtual decimal F_11_15 { get; set; }
        /// <summary>
        /// 负-16-20
        /// </summary>
        public virtual decimal F_16_20 { get; set; }
        /// <summary>
        /// 负-21-25
        /// </summary>
        public virtual decimal F_21_25 { get; set; }
        /// <summary>
        /// 负-26+
        /// </summary>
        public virtual decimal F_26 { get; set; }

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

    public class JCLQ_Odds_DXF : JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal DaOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal XiaoOdds { get; set; }
        /// <summary>
        /// 预设总分
        /// </summary>
        public virtual decimal YSZF { get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return DaOdds;
                case "0":
                    return XiaoOdds;
                case "YSZF":
                    return YSZF;
                default:
                    throw new ArgumentException("获取大小分赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            if (YSZF.ToString("N2").EndsWith(".00"))
            {
                return false;
            }
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            DaOdds = odds.GetOdds("3");
            XiaoOdds = odds.GetOdds("0");
            YSZF = odds.GetOdds("YSZF");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return DaOdds.Equals(odds.GetOdds("3"))
                && XiaoOdds.Equals(odds.GetOdds("0"))
                && YSZF.Equals(odds.GetOdds("YSZF"));
        }
        public override string GetOddsString()
        {
            return "3|" + DaOdds.ToString("F2") + ",0|" + XiaoOdds.ToString("F2") + ",YSZF|" + YSZF.ToString("F2");
        }
        //public virtual override string GetOddsStringToYSZF()
        //{
        //    return "3|" + DaOdds + ",0|" + XiaoOdds + ",YSZF|" + YSZF;
        //}
    }

}
