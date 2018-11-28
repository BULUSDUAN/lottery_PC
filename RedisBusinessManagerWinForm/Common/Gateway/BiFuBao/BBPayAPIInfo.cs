using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.BiFuBao
{
    public class BBPayAPIInfo
    {
        /// <summary>
        /// 指令类型 网银=1,卡类=2
        /// </summary>
        public int p1_md { get; set; }
        /// <summary>
        /// 商户订单号 商户系统生成的订单号
        /// </summary>
        public string p2_xn { get; set; }
        /// <summary>
        /// 商户ID 注册后提取
        /// </summary>
        public string p3_bn { get; set; }
        /// <summary>
        /// 支付方式ID 用户选择的支付银行ID
        /// </summary>
        public string p4_pd { get; set; }
        /// <summary>
        /// 产品名称 商户自定义产品名称
        /// </summary>
        public string p5_name { get; set; }
        /// <summary>
        /// 支付金额 支付金额(两位小数)
        /// </summary>
        public decimal p6_amount { get; set; }
        /// <summary>
        /// 币种 1=人民币
        /// </summary>
        public int p7_cr { get; set; }
        /// <summary>
        /// 扩展信息 商户自定义信息，原样返回
        /// </summary>
        public string p8_ex { get; set; }
        /// <summary>
        /// 通知地址 充值成功后通知和显示的地址
        /// </summary>
        public string p9_url { get; set; }
        /// <summary>
        /// 是否通知 不通知=0，通知=1
        /// </summary>
        public int p10_reply { get; set; }
        /// <summary>
        /// 调用模式 0 返回充值地址，由商户负责跳转;1 显示币付宝充值界面,跳转到充值;2 不显示币付宝充值界面 直接跳转到
        /// </summary>
        public int p11_mode { get; set; }
        /// <summary>
        /// 版本号 当前为 1 ,如有变动我们回通知您
        /// </summary>
        public int p12_ver { get; set; }
        /// <summary>
        /// 签名值 由以上字段加上商户KEY生成的MD5签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 支付地址
        /// </summary>
        public string BBPayUrl { get; set; }
        /// <summary>
        /// 商户key
        /// </summary>
        public string BBPayKey { get; set; }
    }
    public class BBPayCallBackInfo
    {
        /// <summary>
        /// 指令类型 网银=1,卡类=2
        /// </summary>
        public int p1_md { get; set; }
        /// <summary>
        /// 币付宝订单 币付宝系统生成的订单号
        /// </summary>
        public string p2_sn { get; set; }
        /// <summary>
        /// 商户订单号 
        /// </summary>
        public string p3_xn { get; set; }
        /// <summary>
        /// 支付金额 (两位小数)
        /// </summary>
        public decimal p4_amt { get; set; }
        /// <summary>
        /// 扩展信息 商户自定义信息，原样返回
        /// </summary>
        public string p5_ex { get; set; }
        //支付方式
        public int p6_pd { get; set; }
        /// <summary>
        /// 状态 成功=success,失败=faile
        /// </summary>
        public string p7_st { get; set; }
        /// <summary>
        /// 通知方式 1=通知,2=显示
        /// </summary>
        public int p8_reply { get; set; }
        /// <summary>
        /// 签名值 由以上字段加上商户KEY生成的MD5签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 币币商户唯一编码
        /// </summary>
        public string BBPayKey { get; set; }
    }
}
