using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.BBPay
{
    public class BBPayAPIInfo
    {
        /// <summary>
        /// 商户订单号 商户系统生成的订单号
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 交易时间，
        /// </summary>
        public long transtime { get; set; }
        /// <summary>
        /// 交易币种 默认1=人民币
        /// </summary>
        public int  currency { get; set; }
        /// <summary>
        /// 交易金额 以"分"为单位的整型
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 商品种类
        /// </summary>
        public string productcategory { get; set; }
        /// <summary>
        /// 商品名称，商户自定义商品名称
        /// </summary>
        public string productname { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string productdesc { get; set; }
        /// <summary>
        /// 商品单价 以"分"为单位的整型
        /// </summary>
        public int productprice { get; set; }
        /// <summary>
        /// 商品数量 最大数量值99999
        /// </summary>
        public int productcount { get; set; }
        /// <summary>
        /// 商户备注信息  商户自定义备注信息，回调时原样返回
        /// </summary>
        public string merrmk { get; set; }
        /// <summary>
        /// 商户标识 商户生成的用户账号
        /// </summary>
        public  string identityid { get; set; }
        /// <summary>
        /// 用户标识类型
        /// </summary>
        public string identitytype { get; set; }
        /// <summary>
        /// 终端ua
        /// </summary>
        public string userua { get; set; }
        /// <summary>
        /// 用户ip
        /// </summary>
        public  string userip { get; set; }
        /// <summary>
        /// 商户后台回调地址
        /// </summary>
        public string areturl { get; set; }
        /// <summary>
        /// 商户前台回调地址
        /// </summary>
        public  string sreturl { get; set; }
       /// <summary>
       /// 支付节点编码  银行编码
       /// </summary>
        public int pnc { get; set; }
        /// <summary>
        /// 签名 MD5加密之后的签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 支付地址
        /// </summary>
        public string BBPayUrl { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public  string  BBPayKey { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string BBPayId { get; set; }
    }

    public class BBPayCallBackInfo
    {
        /// <summary>
        /// 订单金额 以"分"为单位
        /// </summary>
        public int amount { get; set; }

        /// <summary>
        /// 币币的订单号 
        /// </summary>
        public string bborderid { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string merid { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string order { get; set; }

        /// <summary>
        /// 订单状态 1=成功
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 商户备注信息  商户自定义备注信息，回调时原样返回
        /// </summary>
        public string merrmk { get; set; }
        /// <summary>
        /// 商户标识 商户生成的用户账号
        /// </summary>
        public string identityid { get; set; }
        /// <summary>
        /// 用户标识类型
        /// </summary>
        public string identitytype { get; set; }
        public  string sign { get; set; }
    }

}
