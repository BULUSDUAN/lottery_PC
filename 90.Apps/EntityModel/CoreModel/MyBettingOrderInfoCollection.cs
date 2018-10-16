using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class MyBettingOrderInfoCollection:Page
    {
        public decimal TotalBuyMoney { get; set; }
        public decimal TotalBonusMoney { get; set; }
        public IList<MyBettingOrderInfo> OrderList { get; set; }
    }
    public class MyBettingOrderInfo
    {
        // 行号
        public long RowNumber { get; set; }
        // 方案号
        public string SchemeId { get; set; }
        // 方案创建者编号
        public string UserId { get; set; }
        // 方案创建者VIP级别
        public int VipLevel { get; set; }
        // 方案创建者显示名称
        public string CreatorDisplayName { get; set; }
        // 方案创建者是否隐藏显示名称
         
        public int HideDisplayNameCount { get; set; }
        // 参与者编号
       
        public string JoinUserId { get; set; }
        // 是否参与成功
        
        public string JoinSucessString { get; set; }
        public bool JoinSucess
        {
            get { return JoinSucessString == "1"; }
            set { JoinSucessString = value ? "1" : "0"; }
        }
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
        // 认购时间
       
        public DateTime BuyTime { get; set; }
        // 认购金额
       
        public decimal BuyMoney { get; set; }
        // 方案总金额
        
        public decimal TotalMoney { get; set; }
        // 方案进度
        
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
      
        public TicketStatus TicketStatus { get; set; }
        // 购买期数
       
        public int TotalIssuseCount { get; set; }
        // 购买期号
       
        public string IssuseNumber { get; set; }
        // 是否虚拟订单
        
        public bool IsVirtualOrder { get; set; }
        // 中奖状态
       
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
       
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
       
        public decimal AfterTaxBonusMoney { get; set; }
        // 中奖后停止
         
        public bool StopAfterBonus { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
       
        public DateTime BetTime { get; set; }
        /// <summary>
        /// 彩种玩法
        /// </summary>
      
        public string GameType { get; set; }
        
        public decimal AddMoney { get; set; }
       
        public decimal RedBagAwardsMoney { get; set; }
      
        public decimal BonusAwardsMoney { get; set; }
    }
}
