using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class ExperterRequestInfo
    {
        public string UserId { get; set; }
        public string UserSummary { get; set; }
        public int LotteryAge { get; set; }
        /// <summary>
        /// 多种彩种
        /// </summary>
        public string RequestGameCodes { get; set; }
        public string UpLoadFilePaths { get; set; }
    }

    /// <summary>
    /// 订阅的专家查询对象
    /// </summary>
    [CommunicationObject]
    public class BookingExperterQueryInfo
    {
        public string ExperterId { get; set; }
        public string DisplayName { get; set; }
        public int HideNameLength { get; set; }
        public string ExperterHeadImage { get; set; }
        public int LotteryAge { get; set; }
        /// <summary>
        /// 命中概率
        /// </summary>
        public decimal BonusRadio { get; set; }
        /// <summary>
        /// 最近发单数
        /// </summary>
        public int RecentlyOrderCount { get; set; }

        public BookingExperterCategory Category { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    [CommunicationObject]
    public class BookingExperterQueryInfoCollection
    {
        public int TotalCount { get; set; }
        public List<BookingExperterQueryInfo> ExperterList { get; set; }
    }

}
