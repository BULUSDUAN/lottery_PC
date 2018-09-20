using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;

namespace app.lottery.site.cbbao.Models
{
    public class JczqWeb
    {
        public string MatchData { get; set; }
        public string MatchId { get; set; }
        public string MatchNumber { get; set; }
        public string CreateTime { get; set; } 
        public string DSStopBettingTime { get; set; }
        public decimal FlatOdds { get; set; }
        public string FSStopBettingTime { get; set; }
        public int FXId { get; set; }
        public string Gi { get; set; }
        public int GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public string Hi { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string LeagueColor { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public int LetBall { get; set; }
        public decimal LoseOdds { get; set; }
        public string MatchIdName { get; set; }
        public int Mid { get; set; }
        public string PrivilegesType { get; set; }
        public string ShortGuestTeamName { get; set; }
        public string ShortHomeTeamName { get; set; }
        public string ShortLeagueName { get; set; }
        public string StartDateTime { get; set; }
        public decimal WinOdds { get; set; }
        public string State { get; set; }
        public string HRank { get; set; }
        public string HLg { get; set; }
        public string GRank { get; set; }
        public string GLg { get; set; }
        public SPF SPF { get; set; }
        public BRQSPF BRQSPF { get; set; }
        public ZJQ ZJQ { get; set; }
        public BQC BQC { get; set; }
        public BF BF { get; set; }

        public string ZJQ_Result{ get; set; }
        public decimal ZJQ_SP { get; set; }
        public decimal SPF_SP{ get; set; }
        public string SPF_Result{ get; set; }
        public decimal BQC_SP { get; set; }
        public string BQC_Result { get; set; }
        public decimal BF_SP { get; set; }
        public string BF_Result { get; set; }
        public decimal BRQSPF_SP { get; set; }
        public string BRQSPF_Result { get; set; }
        public int FullGuestTeamScore { get; set; }
        public int FullHomeTeamScore { get; set; }
        public int HalfGuestTeamScore{ get; set; }
        public int HalfHomeTeamScore{ get; set; }
        public string MatchState{ get; set; }
    }

    public class SPF
    {
        public string MatchId { get; set; }
        public decimal WinOdds { get; set; }
        public decimal FlatOdds { get; set; }
        public decimal LoseOdds { get; set; }
        public string NoSaleState { get; set; }
    }
    public class BRQSPF
    {
        public string MatchId { get; set; }
        public decimal WinOdds { get; set; }
        public decimal FlatOdds { get; set; }
        public decimal LoseOdds { get; set; }
        public string NoSaleState { get; set; }
    }

    public class ZJQ
    {
        public string MatchId { get; set; }
        public decimal JinQiu_0_Odds { get; set; }
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        public decimal JinQiu_7_Odds { get; set; }
        public string NoSaleState { get; set; }
    }

    public class BQC
    {
        public string MatchId { get; set; }
        public decimal F_F_Odds { get; set; }
        public decimal F_P_Odds { get; set; }
        public decimal F_SH_Odds { get; set; }
        public decimal P_F_Odds { get; set; }
        public decimal P_P_Odds { get; set; }
        public decimal P_SH_Odds { get; set; }
        public decimal SH_F_Odds { get; set; }
        public decimal SH_P_Odds { get; set; }
        public decimal SH_SH_Odds { get; set; }
        public string NoSaleState { get; set; }
    }

    public class BF
    {
        public string MatchId { get; set; }
        public decimal F_01 { get; set; }
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_05 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_15 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        public decimal F_25 { get; set; }
        public decimal F_QT { get; set; }
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        public decimal P_QT { get; set; }
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        public decimal S_50 { get; set; }
        public decimal S_51 { get; set; }
        public decimal S_52 { get; set; }
        public decimal S_QT { get; set; }
        public string NoSaleState { get; set; }
    }
}