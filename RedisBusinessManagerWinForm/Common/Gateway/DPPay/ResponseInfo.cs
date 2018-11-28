using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{

    /// <summary>
    /// 提现返回结果类
    /// </summary>
    [Serializable]
    public class ResponseInfo
    {
        public String company_order_num { get; set; }
        public String mownecum_order_num { get; set; }
        public String status { get; set; }
        public String error_msg { get; set; }
       
    }
}
