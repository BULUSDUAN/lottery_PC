using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EntityModel.CoreModel
{
    [Serializable]
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


    [ProtoContract]
    public class WinNumber_QueryInfo
    {
        [ProtoMember(1)]
        public string GameCode { get; set; }
        [ProtoMember(2)]
        public string DisplayName { get; set; }
        [ProtoMember(3)]
        public string GameType { get; set; }
        [ProtoMember(4)]
        public string IssuseNumber { get; set; }
        [ProtoMember(5)]
        public string WinNumber { get; set; }
        [ProtoMember(6)]
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
