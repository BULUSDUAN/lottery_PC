using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common.Utilities;
using Common.Mappings;

namespace GameBiz.Core
{
    #region 发单盈利排行榜 - 竞彩类

    /// <summary>
    /// 发单盈利排行榜 - 竞彩类
    /// </summary>
    [CommunicationObject]
    public class RankInfo_BettingProfit_Sport
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public int TotalOrderCount { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal BonusMoney { get; set; }
        public decimal ProfitMoney { get; set; }


        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        public int SilverCrownCount { get; set; }
    }
    /// <summary>
    /// 发单盈利排行榜报表 - 竞彩类
    /// </summary>
    [CommunicationObject]
    public class RankReportCollection_BettingProfit_Sport
    {
        public RankReportCollection_BettingProfit_Sport()
        {
            RankInfoList = new List<RankInfo_BettingProfit_Sport>();
        }

        public int TotalCount { get; set; }
        public List<RankInfo_BettingProfit_Sport> RankInfoList { get; set; }
    }

    #endregion

    /// <summary>
    /// 自动跟单排行
    /// </summary>
    [CommunicationObject]
    public class RankInfo_BeFollower
    {
        public int BeFollowCount { get; set; }

        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        public int SilverCrownCount { get; set; }
    }

    [CommunicationObject]
    public class RankReportCollection_RankInfo_BeFollower
    {
        public RankReportCollection_RankInfo_BeFollower()
        {
            RankInfoList = new List<RankInfo_BeFollower>();
        }

        public int TotalCount { get; set; }
        public List<RankInfo_BeFollower> RankInfoList { get; set; }
    }

    /// <summary>
    /// 合买人气排行
    /// </summary>
    [CommunicationObject]
    public class RankInfo_HotTogether
    {
        public int FollowUserCount { get; set; }
        public int SucessOrderCount { get; set; }

        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        public int SilverCrownCount { get; set; }
    }

    [CommunicationObject]
    public class RankReportCollection_RankInfo_HotTogether
    {
        public RankReportCollection_RankInfo_HotTogether()
        {
            RankInfoList = new List<RankInfo_HotTogether>();
        }

        public int TotalCount { get; set; }
        public List<RankInfo_HotTogether> RankInfoList { get; set; }
    }

    #region 累积中奖排行榜 - 竞彩类

    /// <summary>
    /// 累积中奖排行榜 - 竞彩类
    /// </summary>
    [CommunicationObject]
    public class RankInfo_TotalBonus_Sport
    {
        [EntityMappingField("TotalCount")]
        public int TotalOrderCount { get; set; }
        [EntityMappingField("TotalMoney")]
        public decimal TotalOrderMoney { get; set; }
        [EntityMappingField("BonusMoney")]
        public decimal BonusMoney { get; set; }
        [EntityMappingField("ProfitMoney")]
        public decimal ProfitMoney { get; set; }

        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("DisplayName")]
        public string UserDisplayName { get; set; }
        [EntityMappingField("HideDisplayNameCount")]
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
    [CommunicationObject]
    public class RankReportCollection_TotalBonus_Sport
    {
        public int TotalCount { get; set; }
        public IList<RankInfo_TotalBonus_Sport> RankInfoList { get; set; }
    }

    #endregion
}
