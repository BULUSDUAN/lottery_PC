using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
   public class Sports_TogetherSchemeQueryInfoParam : Page
    {
       public string key { get; set; }
       public string issuseNumber { get; set; }
       public string gameCode { get; set; }

       public string gameType { get; set; }
       public TogetherSchemeSecurity?  security { get; set; }
       public SchemeBettingCategory? betCategory { get; set; }
       public TogetherSchemeProgress? progressState { get; set; }
       public decimal minMoney { get; set; }
       public decimal maxMoney { get; set; }
       public decimal minProgress { get; set; }
       public decimal maxProgress { get; set; }
       public string orderBy { get; set; }
       public string userId { get; set; }
    }
}
