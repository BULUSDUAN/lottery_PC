using System;

namespace EntityModel.CoreModel
{

    /// <summary>
    /// 提现申请实体类
    /// </summary>
    /// <param name="company_id"> DP系统提供的company_id</param>
    /// <param name="bank_id">客户提现的银行编码</param>
    /// <param name="company_order_num">订单号是唯一的</param>
    /// <param name="amount">客户提现金额</param>
    /// <param name="card_num">银行卡卡号</param>
    /// <param name="card_name">银行卡姓名</param>
    /// <param name="key">动态密钥
    /// MD5(MD5(config)+company_id+bank_id+company_order_num+amount+card_num+card_name+company_user+issue_bank_name+issue_bank_address+memo)
    /// </param>
    /// <param name="company_user">平台用户</param>
    /// <param name="issue_bank_name">开户行名称</param>
    /// <param name="issue_bank_address">开户行地址</param>
    /// <param name="memo">备用字段</param>
    /// <returns></returns>
    public class WithdrawApplyInfo
    {
        public int company_id { get; set; }
        public String bank_id { get; set; }
        public String company_order_num { get; set; }
        public decimal amount { get; set; }
        public String card_num { get; set; }
        public String card_name { get; set; }
        public String company_user { get; set; }
        public String issue_bank_name { get; set; }
        public String issue_bank_address { get; set; }
        //手续费
        public decimal transaction_charge { get; set; }
        //DP订单编号
        public String mownecum_order_num { get; set; }
        //状态
        public int status { get; set; }
        //错误信息
        public String error_msg { get; set; }
        public String memo { get; set; }


    }

}
