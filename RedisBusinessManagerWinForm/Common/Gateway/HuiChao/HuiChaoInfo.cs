using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.HuiChao
{
   public class HuiChaoInfo
    {
        /// <summary>
        /// 商户号_注册所得 商户号，注册所得_必填
        /// </summary>
        public string MerNo { get; set; }
        /// <summary>
        /// 订单号_网站订单号_必填
        /// </summary>
        public string BillNo { get; set; }
        /// <summary>
        /// 金额_该笔订单的资金总额，取值范围:0.01_必填
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 页面跳转同步通知页面_支付完成后WEB页面跳转显示支付结果_必填
        /// </summary>
        public string ReturnURL { get; set; }
        /// <summary>
        /// 服务器异步通知路径_汇潮服务器主动异步通知商户网站指定的路径_必填
        /// </summary>
        public string AdviceURL { get; set; }
        /// <summary>
        /// 签名信息_MerNo&BillNo&Amount&ReturnURL&MD5key_必填
        /// </summary>
        public string SignInfo { get; set; }
        /// <summary>
        /// 请求时间_交易时间:yyyyMMddHHmmss_必填
        /// </summary>
        public string orderTime { get; set; }
        /// <summary>
        /// 银行编码_可选
        /// </summary>
        public string defaultBankNumber { get; set; }
        /// <summary>
        /// 备注_可选
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 商品信息_可选
        /// </summary>
        public string products { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string MD5Key { get; set; }
        /// <summary>
        /// 支付请求地址
        /// </summary>
        public string PayUrl { get; set; }

       /// <summary>
       /// 支付方式
       /// </summary>
        public string PayType { get; set; }
    }
}
