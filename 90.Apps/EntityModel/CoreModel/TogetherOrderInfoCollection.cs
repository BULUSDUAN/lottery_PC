using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
    public class TogetherOrderInfoCollection:Page
    {
        public decimal TotalBuyMoney { get; set; }
        public decimal TotalOrderMoney { get; set; }

        public IList<TogetherOrderInfo> OrderList { get; set; }
    }
    public class TogetherOrderInfo
    {
        // 行号
        
        public long RowIndex { get; set; }
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
        // 彩种
        
        public string GameType { get; set; }
        // 玩法名称
     
        public string GameTypeName { get; set; }
        // 方案类型
        
        public SchemeType SchemeType { get; set; }
        // 方案总金额
      
        public decimal TotalMoney { get; set; }
        // 参与合买金额
       
        public decimal JoinMoney { get; set; }
        // 合买进度
       
        public TogetherSchemeProgress TogetherSchemeProgress { get; set; }
        // 合买进度金额
       
        public decimal Progress { get; set; }
        // 方案进度
      
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        
        public TicketStatus TicketStatus { get; set; }
        // 购买期号
        
        public string IssuseNumber { get; set; }
        // 购买期号
        
        public bool IsVirtualOrder { get; set; }
        // 是否已派奖
         
        public bool IsPrizeMoney { get; set; }
        // 中奖状态
        
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
       
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        
        public decimal AfterTaxBonusMoney { get; set; }
        // 创建时间
        
        public DateTime CreateTime { get; set; }
        //投注类别
       
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        //嘉奖
       
        public decimal AddMoney { get; set; }

    }
}
