using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GameBiz.Business.Domain.Entities
{
  
    public class WithdrawApplyResult
    {
      
        public String mownecum_order_num { get; set; }

       
        public String company_order_num { get; set; }

       

       
        public int status { get; set; }

      
        public String error_msg { get; set; }
      

    }
}
