using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;

namespace app.lottery.site.cbbao.Models
{
    public class JclqWeb
    {

        //队伍基础信息
        public decimal AverageLose { get; set; }
        public decimal AverageWin { get; set; }
        public string CreateTime { get; set; }
        public string DSStopBettingTime { get; set; }
        public string FSStopBettingTime { get; set; }
        public string GuestTeamName { get; set; }
        public int GuestTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public int HomeTeamId { get; set; }
        public string LeagueColor { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string MatchIdName { get; set; }
        public string MatchData { get; set; }
        public string MatchId { get; set; }
        public string MatchNumber { get; set; }
        public string State { get; set; }
        public string PrivilegesType { get; set; }
        public string StartDateTime { get; set; }
        public int Mid { get; set; }
        public int FXId { get; set; }
        public SF SF { get; set; }
        public RFSF RFSF { get; set; }
        public SFC SFC { get; set; }
        public DXF DXF { get; set; }
        public string DXF_Result{ get; set; }
        public decimal DXF_SP{ get; set; }
        public string DXF_Trend{ get; set; }
        public int GuestScore  { get; set; }
        public int HomeScore { get; set; }
        public string RFSF_Result{ get; set; }
        public decimal RFSF_SP{ get; set; }
        public string RFSF_Trend { get; set; }
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string SFC_Result { get; set; }
        public decimal SFC_SP { get; set; }
        public string MatchState { get; set; }
    }

    public class SF
    {
        public string MatchId { get; set; }
        public decimal WinSP { get; set; }
        public decimal LoseSP { get; set; }
        public string NoSaleState { get; set; }
    }
    public class RFSF
    {
        public string MatchId { get; set; }
        public decimal RF { get; set; }
        public decimal LoseSP { get; set; }
        public decimal WinSP { get; set; }
        public string NoSaleState { get; set; }
    }

    public class SFC
    {
        public string MatchId { get; set; }
        public decimal GuestWin1_5 { get; set; }
        public decimal GuestWin11_15 { get; set; }
        public decimal GuestWin16_20 { get; set; }
        public decimal GuestWin21_25 { get; set; }
        public decimal GuestWin26 { get; set; }
        public decimal GuestWin6_10 { get; set; }
        public decimal HomeWin1_5 { get; set; }
        public decimal HomeWin11_15 { get; set; }
        public decimal HomeWin16_20 { get; set; }
        public decimal HomeWin21_25 { get; set; }
        public decimal HomeWin26 { get; set; }
        public decimal HomeWin6_10 { get; set; }
        public string NoSaleState { get; set; }
    }

    public class DXF
    {
        public string MatchId { get; set; }
        public decimal DF { get; set; }
        public decimal XF { get; set; }
        public decimal YSZF { get; set; }
        public string NoSaleState { get; set; }
    }

}