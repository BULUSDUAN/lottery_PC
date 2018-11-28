using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{
    public class WithdrawApplyResult
    {

        public String mownecum_order_num { get; set; }
        public String company_order_num { get; set; }
        public int status { get; set; }
        public String error_msg { get; set; }
        public Decimal transaction_charge { get; set; }
      
    }
}
