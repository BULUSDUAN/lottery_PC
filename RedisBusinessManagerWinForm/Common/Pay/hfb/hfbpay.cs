using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Common.Pay.hfb
{
    /// <summary>
    /// 合付宝
    /// </summary>
    public class hfbpay
    {
        private static hfbpay _instance = null;
        private static readonly object obj = new object();
        public static hfbpay GetInstance()
        {
            if (null == _instance)
            {
                lock (obj)
                {
                    if (null == _instance)
                    {
                        _instance = new hfbpay();
                    }
                }
            }
            return _instance;
        }
        public string SignCertPath { get; set; }
        public string PublicCertPath { get; set; }
        public string SignCertPwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Paying(string url, string merchantNo, string version, string channelNo, string tranSerialNum, string bankId, string cardType, decimal moeny, string notifyUrl, string returnUrl, string ip)
        {
            SortedDictionary<string, string> resData = new SortedDictionary<string, string>(new HFBComparer());
            resData.Add("merchantNo", merchantNo);//商户编号
            resData.Add("version", version);//接口版本号
            resData.Add("channelNo", channelNo);//渠道编号
            resData.Add("tranSerialNum", tranSerialNum);//交易流水号
            if (!string.IsNullOrEmpty(bankId))
                resData.Add("bankId", bankId);//交易流水号
            if (!string.IsNullOrEmpty(cardType))
                resData.Add("cardType", cardType);//支付卡种
            resData.Add("tranTime", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易时间
            resData.Add("currency", "CNY");//交易币种
            resData.Add("amount", (moeny * 100).ToString());//交易金额
            resData.Add("bizType", "01");//业务代码
            resData.Add("goodsName", "充值卡");//商品名称
            resData.Add("goodsInfo", "");//商品信息
            resData.Add("goodsNum", "");//商品数量
            resData.Add("notifyUrl", notifyUrl);//后台通知地址
            resData.Add("returnUrl", returnUrl);//前台跳转地址
            resData.Add("buyerName", HFBUtil.EncryptData("小二", Encoding.UTF8));//买家姓名 
            resData.Add("buyerId", "10000");//买家 id 
            resData.Add("contact", "");//买家联系方式
            resData.Add("valid", "60");//订单有效时间
            resData.Add("ip", ip);//用户支付 ip 
            resData.Add("referer", "");//支付网址
            resData.Add("remark", "");//备注字段
            resData.Add("YUL1", "");//预留字段 1
           
            //签名
            string sign = HFBUtil.Sign(resData, Encoding.UTF8);
            resData.Add("sign", sign);//签名
            string html = HFBUtil.CreateAutoSubmitForm(url, resData, Encoding.UTF8);
            return html;
        }




    }
}
