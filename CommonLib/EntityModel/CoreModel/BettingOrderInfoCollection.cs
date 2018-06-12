using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
   public class BettingOrderInfoCollection:Page
    {      
        public int TotalUserCount { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalPreTaxBonusMoney { get; set; }
        public decimal TotalAfterTaxBonusMoney { get; set; }
        public decimal TotalAddMoney { get; set; }
        public decimal TotalRedbagMoney { get; set; }
        public decimal TotalRealPayRebateMoney { get; set; }
        public decimal TotalRedBagAwardsMoney { get; set; }
        public decimal TotalBonusAwardsMoney { get; set; }

        public IList<BettingOrderInfo> OrderList { get; set; }
    }
    public class BettingOrderInfo
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
        // 彩种
        
        public string GameCode { get; set; }
        // 彩种名称
      
        public string GameName { get; set; }
        // 玩法名称
        
        public string GameTypeName { get; set; }
        //过关方式
        
        public string PlayType { get; set; }
        //倍数
       
        public int Amount { get; set; }
        // 方案类型
        
        public SchemeType SchemeType { get; set; }
        // 方案来源
         
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
       
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 方案投注方案
       
        public TogetherSchemeSecurity Security { get; set; }
        // 当前投注金额
        
        public decimal CurrentBettingMoney { get; set; }
        // 方案总金额
        
        public decimal TotalMoney { get; set; }
        // 中奖号码
        
        public string WinNumber { get; set; }
        // 方案进度
        
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
       
        public TicketStatus TicketStatus { get; set; }
        // 购买期数
       
        public int TotalIssuseCount { get; set; }
        // 购买期号
       
        public string IssuseNumber { get; set; }
        // 中奖状态
       
        public BonusStatus BonusStatus { get; set; }
        // 中奖后停止
        
        public bool StopAfterBonus { get; set; }
        // 税前奖金
       
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        
        public decimal AfterTaxBonusMoney { get; set; }
        // 加奖奖金
        
        public decimal AddMoney { get; set; }
        // 创建时间
        
        public DateTime CreateTime { get; set; }
        // 所属经销商
        
        public string AgentId { get; set; }
       
        public bool IsVirtualOrder { get; set; }
      
        public DateTime BetTime { get; set; }
       
        public decimal RedBagMoney { get; set; }
        
        public decimal RealPayRebateMoney { get; set; }
         
        public decimal RedBagAwardsMoney { get; set; }
      
        public decimal BonusAwardsMoney { get; set; }
    }
}
