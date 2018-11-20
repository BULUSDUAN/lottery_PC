using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.Alipay.GeneralPoint
{
    /// <summary>
    ///配置类
    /// </summary>
    class AlipayConfig
    {
        #region 字段
        private static string partner = "";//商户ID，合作身份者ID，合作伙伴ID
        private static string key = "";//安全校验码
        private static string seller_email = "";//商家签约时的支付宝帐号，即收款的支付宝帐号
        #endregion

        /// <summary>
        /// 商户ID，合作身份者ID，合作伙伴ID
        /// </summary>
        /// <remarks>获取方式是：用签约时支付宝帐号登陆支付宝网站www.alipay.com，在商家服务->我的商家服务 直接可以查看到</remarks>
        public static string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 安全校验码，与partner是一组的
        /// </summary>
        /// <remarks>获取方式是：用签约时支付宝帐号登陆支付宝网站www.alipay.com，在商家服务->我的商家服务 点击查看输入支付密码即可查看到</remarks>
        public static string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 商家签约时的支付宝帐号，即收款的支付宝帐号
        /// </summary>
        /// <remarks>该参数部分接口可能用不到</remarks>
        public static string Seller_Email
        {
            get { return seller_email; }
            set { seller_email = value; }
        }
    }
}
