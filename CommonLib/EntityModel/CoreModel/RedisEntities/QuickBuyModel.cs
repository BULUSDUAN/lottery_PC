using EntityModel.LotteryJsonInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [Serializable]
    public class QuickBuyModel
    {
        /// <summary>
        /// 数字彩
        /// </summary>
        public List<Issuse_QueryInfo> SZCList { get; set; }

        /// <summary>
        /// 竞彩足球
        /// </summary>
        public List<JCZQ_MatchInfo_WEB> JCZQList { get; set; }
    }
}
