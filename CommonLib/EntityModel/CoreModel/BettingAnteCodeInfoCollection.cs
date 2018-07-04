using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
   public class BettingAnteCodeInfoCollection:Page
    {
        public BettingAnteCodeInfoCollection()
        { }

        public IList<BettingAnteCodeInfo> AnteCodeList { get; set; }
    }
    public class BettingAnteCodeInfo
    {
        public BettingAnteCodeInfo()
        {

        }
       
        public string SchemeId { get; set; }
      
        public string GameCode { get; set; }
      
        public string GameName { get; set; }
        
        public string GameType { get; set; }
        
        public string GameTypeName { get; set; }
        
        public string IssuseNumber { get; set; }
         
        public string AnteCode { get; set; }
     
        public BonusStatus BonusStatus { get; set; }
        
        public DateTime CreateTime { get; set; }
    }

}
