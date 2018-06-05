using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameBiz.Core
{
    
    public class GameInfo
    {
        public string GameCode { get; set; }
        public string DisplayName { get; set; }
    }

    public class GameInfoCollection : List<GameInfo>
    {
    }


    public class LotteryGameInfo
    {
        public string GameCode { get; set; }
        public string DisplayName { get; set; }
        public EnableStatus EnableStatus { get; set; }
    }

    public class LotteryGameInfoCollection : List<LotteryGameInfo>
    {
    }


    public class GameTypeInfo
    {
        public GameInfo Game { get; set; }
        public string GameType { get; set; }
        public string DisplayName { get; set; }
    }

    public class GameTypeInfoCollection : List<GameTypeInfo>
    {
    }


    public class WinNumber_QueryInfo
    {
        public string GameCode { get; set; }
        public string DisplayName { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public string WinNumber { get; set; }
        public DateTime? AwardTime { get; set; }
    }


    public class WinNumber_QueryInfoCollection
    {
        public WinNumber_QueryInfoCollection()
        {
            List = new List<WinNumber_QueryInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinNumber_QueryInfo> List { get; set; }
    }


    public class SiteSummaryInfo
    {
        public int TodayRegisterUserCount { get; set; }
        public decimal MonthTotalFillMoney { get; set; }
        public decimal MonthTotalWithdraw { get; set; }
        public decimal TotalCommonMoney { get; set; }
        public decimal TotalBonusMoney { get; set; }
    }

}
