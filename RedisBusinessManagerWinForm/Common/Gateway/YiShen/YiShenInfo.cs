using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.YiShen
{
    /// <summary>
    /// 银盛支付
    /// </summary>
    public class YiShenInfo
    {
        public string YSPay_Url { get; set; }
        public string Ver { get; set; }
        public string Src { get; set; }
        public string MsgCode { get; set; }//S3001
        public string Time { get; set; }
        /// <summary>
        /// 商家订单号
        /// </summary>
        public string OrderId{get;set;}
        /// <summary>
        /// 业务代码（商户需申请）
        /// </summary>
        public string BusiCode { get; set; }
        /// <summary>
        /// 商户日期
        /// </summary>
        public string ShopDate { get; set; }
        /// <summary>
        /// 币种（CNY）
        /// </summary>
        public string Cur { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 订单说明
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string ExtraData { get; set; }
        /// <summary>
        /// 备注 （原单返回商户）
        /// </summary>
        public string Remark { get; set; }
        public string BankType { get; set; }
        public string BankAccountType { get; set; }
        /// <summary>
        /// 订单有效时间（单位：分）
        /// </summary>
        public long Timeout { get; set; }
        /// <summary>
        /// //支持卡（为空时，全支持。第一位代表信用卡，第二位代表预付费卡。Y代表支持，N代表不支持。）
        /// </summary>
        public string SupportCards { get; set; }
        //Payee 收款方信息
        /// <summary>
        /// 收款方用户号（收款方在银盛支付平台的用户号）商户号
        /// </summary>
        public string PayeeUserCode { get; set; }
        /// <summary>
        /// 收款方客户名（收款方在银盛支付平台的客户名）
        /// </summary>
        public string PayeeName { get; set; }
        /// <summary>
        /// 收款方联系电话
        /// </summary>
        public string PhoneNum { get; set; }
        ///// <summary>
        ///// 收款方入账金额（当有别的分润方时，此处必填。不填写，则此处默认和订单金额相符。）
        ///// </summary>
        //public double Amount { get; set; }
        //Payer 付款方信息
        /// <summary>
        /// 付款方用户号（付款方信息，若填写，则不允许修改付款方信息（银盛支付平台用户））
        /// </summary>
        public string PayerUserCode { get; set; }
        /// <summary>
        /// 付款方客户名（付款方在银盛支付平台的客户名）
        /// </summary>
        public string PayerName { get; set; }
        //Notice 通知地址
        /// <summary>
        /// 前台回调页面URL（商户系统提供，可空，失败不跳转，成功才跳转。银盛支付平台在此URL后追加固定的参数向商户系统跳转：Msg=订单号|金额（到分）然后base64编码Check= Msg的签名后base64）
        /// </summary>
        public string noticePgUrl { get; set; }
        /// <summary>
        /// 后台回调URL（业务系统提供，必填，支付成功后，返回3501报文）
        /// </summary>
        public string noticeBgUrl { get; set; }
        //Fee 手续费
        /// <summary>
        /// 是否平台外计算手续费（不填写时默认为N。填写时，平台则需要检查发起方是否有计算手续费的权限，若无，则返回失败，不进行处理。若有则根据以下项进行计算手续费。）
        /// </summary>
        public string feeFlag { get; set; }
        /// <summary>
        /// 收款方手续费（不填写时，默认为0）
        /// </summary>
        public double feePayeeFee { get; set; }
        /// <summary>
        /// 发起方手续费（不填写时，默认为0）
        /// </summary>
        public double feeSrcFee { get; set; }
       
        ///////////////////////////////////
      
        /// <summary>
        /// 商户私钥证书路径(发送交易签名使用)
        /// </summary>
        public string Pfxpath { get; set; }

        /// <summary>
        /// 银盛支付公钥证书路径(接收到银盛支付回执时验签使用)
        /// </summary>
        public string Cerpath { get; set; }

        /// <summary>
        /// 商户私钥证书密码
        /// </summary>
        public string Pfxpassword { get; set; }


    }


}
