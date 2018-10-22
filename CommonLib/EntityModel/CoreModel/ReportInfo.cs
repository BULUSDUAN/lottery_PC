using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class ReportInfo
    {
        #region 过关统计

        /// <summary>
        /// 过关统计订单
        /// </summary>
      
        public class SportsOrder_GuoGuanInfo
        {
            public string SchemeId { get; set; }
            public string GameCode { get; set; }
            public string GameType { get; set; }
            /// <summary>
            /// 期号
            /// </summary>
            public string IssuseNumber { get; set; }
            /// <summary>
            /// 订单类型
            /// </summary>
            public SchemeType SchemeType { get; set; }
            /// <summary>
            /// 投注注数
            /// </summary>
            public int BetCount { get; set; }
            /// <summary>
            /// 正确注数
            /// </summary>
            public int RightCount { get; set; }
            /// <summary>
            /// 错一注数
            /// </summary>
            public int Error1Count { get; set; }
            /// <summary>
            /// 错二注数
            /// </summary>
            public int Error2Count { get; set; }
            /// <summary>
            /// 订单总注数
            /// </summary>
            public int TotalMatchCount { get; set; }
            /// <summary>
            /// 命中场数
            /// </summary>
            public int HitMatchCount { get; set; }
            /// <summary>
            /// 订单金额
            /// </summary>
            public decimal TotalMoney { get; set; }
            /// <summary>
            /// 中奖状态
            /// </summary>
            public BonusStatus BonusStatus { get; set; }
            /// <summary>
            /// 中奖金额
            /// </summary>
            public decimal BonusMoney { get; set; }


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
            /// <summary>
            /// 是否为虚拟订单
            /// </summary>
            public bool IsVirtualOrder { get; set; }
            /// <summary>
            /// 方案投注类别
            /// </summary>
            public SchemeBettingCategory SchemeBettingCategory { get; set; }
            /// <summary>
            /// 投注时间
            /// </summary>
            public DateTime BetTime { get; set; }

        }
      
        public class SportsOrder_GuoGuanInfoCollection
        {
            public SportsOrder_GuoGuanInfoCollection()
            {
                ReportItemList = new List<SportsOrder_GuoGuanInfo>();
            }

            public int TotalCount { get; set; }
            public List<SportsOrder_GuoGuanInfo> ReportItemList { get; set; }
        }


        #endregion
    }
}
