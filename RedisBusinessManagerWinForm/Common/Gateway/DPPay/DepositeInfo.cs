using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{
    /// <summary>
    /// 充值申请实体类
    /// depositeId  主键
    ///company_id 平台id
    ///amount 充值金额
    ///company_user 平台用户
    /// estimated_payment_bank 预计付款银行
    ///web_url 平台访问地址
    ///memo 备用字段
    //note 附言
    //note_model 附言模式
    //terminal 使用终端
    //createtime 下单时间
    //pay_time DP从银行抓取到的客户充值时间
    //bank_id 银行id
    //amount客户实际转账金额
    //company_order_num 平台订单编号
    // mownecum_order_num  DP系统订单号
    //pay_card_num  付款卡卡号
    //pay_card_name 付款卡用户名
    // channel 交易渠道
    //area 交易地址
    //fee  手续费
    //transaction_charge 服务费
    //deposit_mode  充值渠道
    //base_info  渠道原始信息
    //operationg_time DP系统的订单完成时间
    // email  收款的email账号
    //staus 状态
    //error_msg 错误信息
    //mode   收款方式
    ///group_id 组
    /// </summary>
    public class DepositeInfo
    {

        public DepositeInfo()
        {
        }
        public DepositeInfo(String depositeId, String company_id, String bank_id, decimal amount, String company_order_num, String company_user,
            String estimated_payment_bank, String deposit_mode, String group_id, String web_url, String memo, String note, String note_model, String terminal, String status)
        {
            this.depositeId = depositeId;
            this.company_id = company_id;
            this.bank_id = bank_id;
            this.amount = amount;
            this.company_user = company_user;
            this.company_order_num = company_order_num;
            this.estimated_payment_bank = estimated_payment_bank;
            this.deposit_mode = deposit_mode;
            this.group_id = group_id;
            this.web_url = web_url;
            this.memo = memo;
            this.note = note;
            this.note_model = note;
            this.terminal = terminal;
            this.status = status;
        }

        public String depositeId { get; set; }
        public String company_id { get; set; }
        public decimal amount { get; set; }
        public String company_user { get; set; }
        public String estimated_payment_bank { get; set; }
        public String web_url { get; set; }
        public String memo { get; set; }
        public String note { get; set; }
        public String note_model { get; set; }
        public String terminal { get; set; }
        public String createtime { get; set; }
        public String pay_time { get; set; }
        public String bank_id { get; set; }
        public decimal actualAmount { get; set; }
        public String company_order_num { get; set; }
        public String mownecum_order_num { get; set; }
        public String pay_card_num { get; set; }
        public String pay_card_name { get; set; }
        public String channel { get; set; }
        public String area { get; set; }
        public decimal fee { get; set; }
        public decimal transaction_charge { get; set; }
        public String deposit_mode { get; set; }
        public String base_info { get; set; }
        public String operating_time { get; set; }
        public String email { get; set; }
        public String status { get; set; }
        public String error_msg { get; set; }
        public String mode { get; set; }
        public String group_id { get; set; }
        /// <summary>
        /// 充值申请key
        /// MD5(MD5(config)+company_id+bank_id+amount+company_order_num+company_user+estimated_payment_bank+deposit_mode+group_id+web_url+memo+note+note_model)
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public String getKey(String config)
        {
            string ubkey = MD5Utils.encrypt(MD5Utils.encrypt(config) + company_id + bank_id + amount + company_order_num + company_user + estimated_payment_bank + deposit_mode + group_id + web_url + memo + note + note_model);
            return ubkey;
        }
        /// <summary>
        /// 充值确认key
        /// MD5(MD5(config)+pay_time+bank_id+amount+company_order_num+mownecum_order_num+pay_card_num+pay_card_name+channel+area+fee+transaction_charge+deposit_mode)
        /// </summary>
        /// <returns></returns>
        public String getConfirmKey(String config)
        {
            string ubkey = MD5Utils.encrypt(MD5Utils.encrypt(config) + pay_time + bank_id + amount + company_order_num + mownecum_order_num + pay_card_num + pay_card_name + channel + area + fee + transaction_charge + deposit_mode);
            return ubkey;
        }
    }
}
