using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
   public class Sports_SchemeQueryInfo
    {
        public Sports_SchemeQueryInfo()
        {

        }
       
        public DateTime CreateTime { get; set; }
       
        public TogetherSchemeSecurity Security { get; set; }
      
        public DateTime StopTime { get; set; }
        
        public int HitMatchCount { get; set; }
       
        public decimal AddMoney { get; set; }
       
        public string AddMoneyDescription { get; set; }
       
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
       
        public int HideDisplayNameCount { get; set; }
        
        public decimal TicketProgress { get; set; }
        
        public bool IsVirtualOrder { get; set; }
         
        public AddMoneyDistributionWay DistributionWay { get; set; }
        
        public decimal MinBonusMoney { get; set; }
       
        public decimal MaxBonusMoney { get; set; }
       
        public string ExtensionOne { get; set; }
       
        public bool IsAppend { get; set; }
        
        public DateTime ComplateDateTime { get; set; }
       
        public DateTime BetTime { get; set; }
        
        public SchemeSource SchemeSource { get; set; }
      
        public DateTime? TicketTime { get; set; }
       
        public decimal RedBagMoney { get; set; }
        
        public string Attach { get; set; }
       
        public bool IsPrizeMoney { get; set; }
        
        public string WinNumber { get; set; }
       
        public int BonusCount { get; set; }
       
        public string UserId { get; set; }
       
        public string UserDisplayName { get; set; }
      
        public string SchemeId { get; set; }
       
        public string GameCode { get; set; }
       
        public string GameDisplayName { get; set; }
      
        public string GameType { get; set; }
        
        public string GameTypeDisplayName { get; set; }
       
        public string PlayType { get; set; }
       
        public SchemeType SchemeType { get; set; }
       
        public string IssuseNumber { get; set; }
        
        public int Amount { get; set; }
       
        public int BetCount { get; set; }
       
        public int TotalMatchCount { get; set; }
      
        public decimal TotalMoney { get; set; }
       
        public TicketStatus TicketStatus { get; set; }
       
        public string TicketId { get; set; }
        
        public string TicketLog { get; set; }
       
        public ProgressStatus ProgressStatus { get; set; }
        
        public BonusStatus BonusStatus { get; set; }
       
        public decimal PreTaxBonusMoney { get; set; }
       
        public decimal AfterTaxBonusMoney { get; set; }
        
        public decimal RedBagAwardsMoney { get; set; }
      
        public decimal BonusAwardsMoney { get; set; }
    }
}
