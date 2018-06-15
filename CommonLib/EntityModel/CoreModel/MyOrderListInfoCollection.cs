using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
   public class MyOrderListInfoCollection: Page
    {
        public IList<MyOrderListInfo> List { get; set; }
    }
    public class MyOrderListInfo
    {
        // 方案号
        public string SchemeId { get; set; }
        // 彩种
        public string GameCode { get; set; }
        // 玩法名称
        public string GameTypeName { get; set; }
        // 方案类型
        public SchemeType SchemeType { get; set; }
        // 方案来源
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 方案总金额
        public decimal TotalMoney { get; set; }
        public bool StopAfterBonus { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        // 方案进度
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        public TicketStatus TicketStatus { get; set; }
        // 购买期号
        public string IssuseNumber { get; set; }
        // 中奖状态
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        public string BetTime { get; set; }
        /// <summary>
        /// 彩种玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 加奖总金额
        /// </summary>
        public decimal AddMoney { get; set; }
        /// <summary>
        /// 红包加奖金额
        /// </summary>
        public decimal RedBagAwardsMoney { get; set; }
        /// <summary>
        /// 奖金加奖金额
        /// </summary>
        public decimal BonusAwardsMoney { get; set; }
    }
}
