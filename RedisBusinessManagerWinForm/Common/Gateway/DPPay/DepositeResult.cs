using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{
    /// <summary>
    ///充值申请--返回参数类
    ///
    /// {"bank_card_num":"","bank_acc_name":"张三","amount":"1.25","email":"icbc238icbc@163.com","company_order_num":"DDP12345678",
    /// "datetime":"20160106132821","note":"123890","mownecum_order_num":"DP52016010602013021157","status":1,"error_msg":"","mode":2,
    /// "issuing_bank_address":"广东省惠州大亚湾支行营业厅","break_url":"baidu.com","deposit_mode":1,"collection_bank_id":1,"key":"979fa9a802bdb76914c95c9d6b1eb7fb"}
    /// </summary>
    public class DepositeResult
    {
        public String bank_card_num { get; set; }
        public String bank_acc_name { get; set; }
        public decimal amount { get; set; }
        public String email { get; set; }
        public String company_order_num { get; set; }
        public String datetime { get; set; }
        public String note { get; set; }
        public String mownecum_order_num { get; set; }
        public int status { get; set; }
        public String error_msg { get; set; }
        public int Model { get; set; }
        public String issuing_bank_address { get; set; }
        public String break_url { get; set; }
        public int deposit_mode { get; set; }
        public int collection_bank_id { get; set; }
        public String key { get; set; }
    }
}
