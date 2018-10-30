using EntityModel.CoreModel;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.Ticket
{
    #region 竞彩比赛信息
    public class JingCai_MatchInfo : JingCaiMatchBase
    {
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
    public class JingCai_MatchResultInfo : JingCaiMatchBase
    {
        public string MatchState { get; set; }
        public int HalfHomeTeamScore { get; set; }
        public int HalfGuestTeamScore { get; set; }
        public int FullHomeTeamScore { get; set; }
        public int FullGuestTeamScore { get; set; }

        public int HomeScore { get; set; }
        public int GuestScore { get; set; }

        #region 竞彩足球玩法赔率
        public string SPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        public string BRQSPF_Result { get; set; }
        public decimal BRQSPF_SP { get; set; }
        public string ZJQ_Result { get; set; }
        public decimal ZJQ_SP { get; set; }
        public string BF_Result { get; set; }
        public decimal BF_SP { get; set; }
        public string BQC_Result { get; set; }
        public decimal BQC_SP { get; set; }
        #endregion

        #region 竞彩篮球玩法赔率
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string RFSF_Result { get; set; }
        public decimal RFSF_SP { get; set; }
        public string SFC_Result { get; set; }
        public decimal SFC_SP { get; set; }
        public string DXF_Result { get; set; }
        public decimal DXF_SP { get; set; }
        #endregion
        public string CreateTime { get; set; }
    }
    #endregion

    #region 竞彩足球赔率信息
    public class JCZQ_SPF_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }


        public decimal GetOdds(string result)
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
        public bool CheckIsValidate()
        {
            return true;
        }
    }
    public class JCZQ_BRQSPF_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
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
        public bool CheckIsValidate()
        {
            return true;
        }
    }
    public class JCZQ_BF_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜-其它
        /// </summary>
        public decimal S_QT { get; set; }
        /// <summary>
        /// 胜-10
        /// </summary>
        public decimal S_10 { get; set; }
        /// <summary>
        /// 胜-20
        /// </summary>
        public decimal S_20 { get; set; }
        /// <summary>
        /// 胜-21
        /// </summary>
        public decimal S_21 { get; set; }
        /// <summary>
        /// 胜-30
        /// </summary>
        public decimal S_30 { get; set; }
        /// <summary>
        /// 胜-31
        /// </summary>
        public decimal S_31 { get; set; }
        /// <summary>
        /// 胜-32
        /// </summary>
        public decimal S_32 { get; set; }
        /// <summary>
        /// 胜-40
        /// </summary>
        public decimal S_40 { get; set; }
        /// <summary>
        /// 胜-41
        /// </summary>
        public decimal S_41 { get; set; }
        /// <summary>
        /// 胜-42
        /// </summary>
        public decimal S_42 { get; set; }
        /// <summary>
        /// 胜-50
        /// </summary>
        public decimal S_50 { get; set; }
        /// <summary>
        /// 胜-51
        /// </summary>
        public decimal S_51 { get; set; }
        /// <summary>
        /// 胜-52
        /// </summary>
        public decimal S_52 { get; set; }
        /// <summary>
        /// 平-其它
        /// </summary>
        public decimal P_QT { get; set; }
        /// <summary>
        /// 平-00
        /// </summary>
        public decimal P_00 { get; set; }
        /// <summary>
        /// 平-11
        /// </summary>
        public decimal P_11 { get; set; }
        /// <summary>
        /// 平-22
        /// </summary>
        public decimal P_22 { get; set; }
        /// <summary>
        /// 平-33
        /// </summary>
        public decimal P_33 { get; set; }
        /// <summary>
        /// 负-其它
        /// </summary>
        public decimal F_QT { get; set; }
        /// <summary>
        /// 负-01
        /// </summary>
        public decimal F_01 { get; set; }
        /// <summary>
        /// 负-02
        /// </summary>
        public decimal F_02 { get; set; }
        /// <summary>
        /// 负-12
        /// </summary>
        public decimal F_12 { get; set; }
        /// <summary>
        /// 负-03
        /// </summary>
        public decimal F_03 { get; set; }
        /// <summary>
        /// 负-13
        /// </summary>
        public decimal F_13 { get; set; }
        /// <summary>
        /// 负-23
        /// </summary>
        public decimal F_23 { get; set; }
        /// <summary>
        /// 负-04
        /// </summary>
        public decimal F_04 { get; set; }
        /// <summary>
        /// 负-14
        /// </summary>
        public decimal F_14 { get; set; }
        /// <summary>
        /// 负-24
        /// </summary>
        public decimal F_24 { get; set; }
        /// <summary>
        /// 负-05
        /// </summary>
        public decimal F_05 { get; set; }
        /// <summary>
        /// 负-15
        /// </summary>
        public decimal F_15 { get; set; }
        /// <summary>
        /// 负-25
        /// </summary>
        public decimal F_25 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
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
        public bool CheckIsValidate()
        {
            return true;
        }
    }
    public class JCZQ_ZJQ_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 进球数 0
        /// </summary>
        public decimal JinQiu_0_Odds { get; set; }
        /// <summary>
        /// 进球数 1
        /// </summary>
        public decimal JinQiu_1_Odds { get; set; }
        /// <summary>
        /// 进球数 2
        /// </summary>
        public decimal JinQiu_2_Odds { get; set; }
        /// <summary>
        /// 进球数 3
        /// </summary>
        public decimal JinQiu_3_Odds { get; set; }
        /// <summary>
        /// 进球数 4
        /// </summary>
        public decimal JinQiu_4_Odds { get; set; }
        /// <summary>
        /// 进球数 5
        /// </summary>
        public decimal JinQiu_5_Odds { get; set; }
        /// <summary>
        /// 进球数 6
        /// </summary>
        public decimal JinQiu_6_Odds { get; set; }
        /// <summary>
        /// 进球数 7
        /// </summary>
        public decimal JinQiu_7_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
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
        public bool CheckIsValidate()
        {
            return true;
        }
    }
    public class JCZQ_BQC_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜-胜
        /// </summary>
        public decimal SH_SH_Odds { get; set; }
        /// <summary>
        /// 胜-平
        /// </summary>
        public decimal SH_P_Odds { get; set; }
        /// <summary>
        /// 胜-负
        /// </summary>
        public decimal SH_F_Odds { get; set; }
        /// <summary>
        /// 平-胜
        /// </summary>
        public decimal P_SH_Odds { get; set; }
        /// <summary>
        /// 平-平
        /// </summary>
        public decimal P_P_Odds { get; set; }
        /// <summary>
        /// 平-负
        /// </summary>
        public decimal P_F_Odds { get; set; }
        /// <summary>
        /// 负-胜
        /// </summary>
        public decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 负-平
        /// </summary>
        public decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负-负
        /// </summary>
        public decimal F_F_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
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
        public bool CheckIsValidate()
        {
            return true;
        }
    }
    #endregion

    #region 竞彩篮球赔率信息
    [BsonIgnoreExtraElements]
    public class JCLQ_SF_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinSP { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseSP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinSP;
                case "0":
                    return LoseSP;
                default:
                    throw new ArgumentException("获取胜负赔率不支持的结果数据 - " + result);
            }
        }
        public bool CheckIsValidate()
        {

            return true;
        }
    }
    [BsonIgnoreExtraElements]
    public class JCLQ_RFSF_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinSP { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseSP { get; set; }
        /// <summary>
        /// 让分
        /// </summary>
        public decimal RF { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinSP;
                case "0":
                    return LoseSP;
                case "RF":
                    return RF;
                default:
                    throw new ArgumentException("获取让分胜负赔率不支持的结果数据 - " + result);
            }
        }
        public bool CheckIsValidate()
        {
            if (RF.ToString("N2").EndsWith(".00"))
            {
                return false;
            }
            return true;
        }
    }
    [BsonIgnoreExtraElements]
    public class JCLQ_SFC_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜-16
        /// </summary>
        public decimal HomeWin1_5 { get; set; }
        /// <summary>
        /// 胜-6-10
        /// </summary>
        public decimal HomeWin6_10 { get; set; }
        /// <summary>
        /// 胜-11-15
        /// </summary>
        public decimal HomeWin11_15 { get; set; }
        /// <summary>
        /// 胜-16-20
        /// </summary>
        public decimal HomeWin16_20 { get; set; }
        /// <summary>
        /// 胜-21-25
        /// </summary>
        public decimal HomeWin21_25 { get; set; }
        /// <summary>
        /// 胜-26+
        /// </summary>
        public decimal HomeWin26 { get; set; }
        /// <summary>
        /// 负-16
        /// </summary>
        public decimal GuestWin1_5 { get; set; }
        /// <summary>
        /// 负-6-10
        /// </summary>
        public decimal GuestWin6_10 { get; set; }
        /// <summary>
        /// 负-11-15
        /// </summary>
        public decimal GuestWin11_15 { get; set; }
        /// <summary>
        /// 负-16-20
        /// </summary>
        public decimal GuestWin16_20 { get; set; }
        /// <summary>
        /// 负-21-25
        /// </summary>
        public decimal GuestWin21_25 { get; set; }
        /// <summary>
        /// 负-26+
        /// </summary>
        public decimal GuestWin26 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
        {
            switch (result)
            {
                case "01":
                    return HomeWin1_5;
                case "02":
                    return HomeWin6_10;
                case "03":
                    return HomeWin11_15;
                case "04":
                    return HomeWin16_20;
                case "05":
                    return HomeWin21_25;
                case "06":
                    return HomeWin26;
                case "11":
                    return GuestWin1_5;
                case "12":
                    return GuestWin6_10;
                case "13":
                    return GuestWin11_15;
                case "14":
                    return GuestWin16_20;
                case "15":
                    return GuestWin21_25;
                case "16":
                    return GuestWin26;
                default:
                    throw new ArgumentException("获取胜分差赔率不支持的结果数据 - " + result);
            }
        }
        public bool CheckIsValidate()
        {
            return true;
        }
    }
    [BsonIgnoreExtraElements]
    public class JCLQ_DXF_SPInfo : JingCaiMatchBase, I_JingCai_Odds
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal DF { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal XF { get; set; }
        /// <summary>
        /// 预设总分
        /// </summary>
        public decimal YSZF { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return DF;
                case "0":
                    return XF;
                case "YSZF":
                    return YSZF;
                default:
                    throw new ArgumentException("获取大小分赔率不支持的结果数据 - " + result);
            }
        }
        public bool CheckIsValidate()
        {
            if (YSZF.ToString("N2").EndsWith(".00"))
            {
                return false;
            }
            return true;
        }
    }
    #endregion
}
