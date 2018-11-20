using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{
   public  class WithdrawResultRequest
    {

        public String mownecum_order_num { get; set; }
        public String company_order_num { get; set; }
        public long status { get; set; }
        public String detail { get; set; }
        public Decimal amount { get; set; }
        public Decimal exact_transaction_charge { get; set; }
        public String key { get; set; }
        public String operating_time { get; set; }

        public Boolean isValid(String key)
        {
            string ubkey = MD5Utils.encrypt(MD5Utils.encrypt(key) + mownecum_order_num + company_order_num + status + amount + exact_transaction_charge);
            if (this.key == ubkey)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
