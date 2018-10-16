using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class FillMoneyQueryInfoCollection:Page
    {
        public decimal TotalRequestMoney { get; set; }
        public decimal TotalResponseMoney { get; set; }
        public IList<FillMoneyQueryInfo> FillMoneyList { get; set; }
    }
    public class FillMoneyQueryInfo
    {
        public FillMoneyQueryInfo() { }

         
        public string OuterFlowId { get; set; }
        
        public DateTime? ResponseTime { get; set; }
        
        public decimal? ResponseMoney { get; set; }
       
        public string ResponseMessage { get; set; }
        
        public string ResponseCode { get; set; }
        
        public string ResponseBy { get; set; }
       
        public FillMoneyStatus Status { get; set; }
       
        public string ShowUrl { get; set; }
        
        public string NotifyUrl { get; set; }
        
        public string ReturnUrl { get; set; }
        
        public DateTime RequestTime { get; set; }
        
        public decimal PayMoney { get; set; }
        
        public decimal RequestMoney { get; set; }
       
        public string RequestExtensionInfo { get; set; }
        
        public string RequestBy { get; set; }
       
        public string DeliveryAddress { get; set; }
       
        public string IsNeedDelivery { get; set; }
        
        public string GoodsDescription { get; set; }
      
        public string GoodsType { get; set; }
       
        public string GoodsName { get; set; }
        
        public string UserComeFrom { get; set; }
       
        public string UserDisplayName { get; set; }
      
        public string UserId { get; set; }
       
        public FillMoneyAgentType FillMoneyAgent { get; set; }
        
        public string OrderId { get; set; }
      
        public SchemeSource SchemeSource { get; set; }
       
        public string AgentId { get; set; }
    }
}
