using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    #region 发单盈利排行榜 - 竞彩类

    /// <summary>
    /// 发单盈利排行榜 - 竞彩类
    /// </summary>
    [ProtoContract]
    public class RankInfo_BettingProfit_Sport
    {
        [ProtoMember(1)]
        public string GameCode { get; set; }
        [ProtoMember(2)]
        public string GameType { get; set; }
        [ProtoMember(3)]
        public int TotalOrderCount { get; set; }
        [ProtoMember(4)]
        public decimal TotalMoney { get; set; }
        [ProtoMember(5)]
        public decimal BonusMoney { get; set; }
        [ProtoMember(6)]
        public decimal ProfitMoney { get; set; }

        [ProtoMember(7)]
        public string UserId { get; set; }
        [ProtoMember(8)]
        public string UserDisplayName { get; set; }
        [ProtoMember(9)]
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        /// 
        [ProtoMember(10)]
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        /// 
        [ProtoMember(11)]
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        /// 
        [ProtoMember(12)]
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        /// 
        [ProtoMember(13)]
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        /// 
        [ProtoMember(14)]
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        /// 
        [ProtoMember(15)]
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        /// 
        [ProtoMember(16)]
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        /// 
        [ProtoMember(17)]
        public int SilverCrownCount { get; set; }
    }
    /// <summary>
    /// 发单盈利排行榜报表 - 竞彩类
    /// </summary>
    [ProtoContract]
    public class RankReportCollection_BettingProfit_Sport
    {
        public RankReportCollection_BettingProfit_Sport()
        {
            RankInfoList = new List<RankInfo_BettingProfit_Sport>();
        }
        [ProtoMember(1)]
        public int TotalCount { get; set; }
        [ProtoMember(2)]
        public List<RankInfo_BettingProfit_Sport> RankInfoList { get; set; }
    }

    #endregion

    /// <summary>
    /// 自动跟单排行
    /// </summary>
    [ProtoContract]
    public class RankInfo_BeFollower
    {
        [ProtoMember(1)]
        public int BeFollowCount { get; set; }
        [ProtoMember(2)]
        public string UserId { get; set; }
        [ProtoMember(3)]
        public string UserDisplayName { get; set; }
        [ProtoMember(4)]
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        /// 
        [ProtoMember(5)]
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        /// 
        [ProtoMember(6)]
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        /// 
        [ProtoMember(7)]
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        /// 
        [ProtoMember(8)]
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        /// 
        [ProtoMember(9)]
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        /// 
        [ProtoMember(10)]
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        /// 
        [ProtoMember(11)]
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        /// 
        [ProtoMember(12)]
        public int SilverCrownCount { get; set; }
    }

    [ProtoContract]
    public class RankReportCollection_RankInfo_BeFollower
    {
        public RankReportCollection_RankInfo_BeFollower()
        {
            RankInfoList = new List<RankInfo_BeFollower>();
        }
        [ProtoMember(1)]
        public int TotalCount { get; set; }
        [ProtoMember(2)]
        public List<RankInfo_BeFollower> RankInfoList { get; set; }
    }

    /// <summary>
    /// 合买人气排行
    /// </summary>
    [ProtoContract]
    public class RankInfo_HotTogether
    {
        [ProtoMember(1)]
        public int FollowUserCount { get; set; }
        [ProtoMember(2)]
        public int SucessOrderCount { get; set; }
        [ProtoMember(3)]
        public string UserId { get; set; }
        [ProtoMember(4)]
        public string UserDisplayName { get; set; }
        [ProtoMember(5)]
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        /// 
        [ProtoMember(6)]
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        /// 
        [ProtoMember(7)]
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        /// 
        [ProtoMember(8)]
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        /// 
        [ProtoMember(9)]
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        /// 
        [ProtoMember(10)]
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        /// 
        [ProtoMember(11)]
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        /// 
        [ProtoMember(12)]
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        /// 
        [ProtoMember(13)]
        public int SilverCrownCount { get; set; }
    }

    [ProtoContract]
    public class RankReportCollection_RankInfo_HotTogether
    {

        public RankReportCollection_RankInfo_HotTogether()
        {
            RankInfoList = new List<RankInfo_HotTogether>();
        }
        [ProtoMember(1)]
        public int TotalCount { get; set; }
        [ProtoMember(2)]
        public List<RankInfo_HotTogether> RankInfoList { get; set; }
    }

    #region 累积中奖排行榜 - 竞彩类

    /// <summary>
    /// 累积中奖排行榜 - 竞彩类
    /// </summary>
    [ProtoContract]
    public class RankInfo_TotalBonus_Sport
    {
        [ProtoMember(1)]
        public int TotalOrderCount { get; set; }
        [ProtoMember(2)]
        public decimal TotalOrderMoney { get; set; }
        [ProtoMember(3)]
        public decimal BonusMoney { get; set; }
        [ProtoMember(4)]
        public decimal ProfitMoney { get; set; }

        [ProtoMember(5)]
        public string UserId { get; set; }
        [ProtoMember(6)]
        public string UserDisplayName { get; set; }
        [ProtoMember(7)]
        public int UserHideDisplayNameCount { get; set; }

        ///// <summary>
        ///// 金星个数
        ///// </summary>
        //public int GoldStarCount { get; set; }
        ///// <summary>
        ///// 金钻个数
        ///// </summary>
        //public int GoldDiamondsCount { get; set; }
        ///// <summary>
        ///// 金杯个数
        ///// </summary>
        //public int GoldCupCount { get; set; }
        ///// <summary>
        ///// 金冠个数
        ///// </summary>
        //public int GoldCrownCount { get; set; }

        ///// <summary>
        ///// 银星个数
        ///// </summary>
        //public int SilverStarCount { get; set; }
        ///// <summary>
        ///// 银钻个数
        ///// </summary>
        //public int SilverDiamondsCount { get; set; }
        ///// <summary>
        ///// 银杯个数
        ///// </summary>
        //public int SilverCupCount { get; set; }
        ///// <summary>
        ///// 银冠个数
        ///// </summary>
        //public int SilverCrownCount { get; set; }
    }
    /// <summary>
    /// 累积中奖排行榜报表 - 竞彩类
    /// </summary>
    [ProtoContract]
    public class RankReportCollection_TotalBonus_Sport
    {
        public int TotalCount { get; set; }
        public IList<RankInfo_TotalBonus_Sport> RankInfoList { get; set; }
    }

    #endregion
}
