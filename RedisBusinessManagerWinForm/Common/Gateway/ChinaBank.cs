using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Common.Gateway
{
    /// <summary>
    /// 网银在线支付功能
    /// </summary>
    public class ChinaBank
    {
        #region 字段
        //MD5密钥
        private static string _key = "";
        //商户号
        private static string _mid = "";
        //签名方式
        private static string _sign_type = "md5";
        //货币种类
        private static string _money_type = "CNY";
        //支付网关地址
        private static string GATEWAY = "https://pay3.chinabank.com.cn/PayGate";
        #endregion

        /// <summary>
        /// 构造函数::初始化网银在线类
        /// </summary>
        /// <param name="mid">商户号</param>
        /// <param name="key">密钥</param>
        /// <param name="signtype">签名方式</param>
        /// <param name="moneytype">货币种类</param>
        public ChinaBank(string mid, string key, string signtype = "md5", string moneytype = "CNY")
        {
            _key = key;
            _mid = mid;
            _sign_type = signtype;
            _money_type = moneytype;
        }

        /// <summary>
        /// 构造提交表单HTML数据
        /// </summary>
        /// <param name="orderId">订单编号，如果为空则用网银在线格式制作一个orderid</param>
        /// <param name="money">支付金额</param>
        /// <param name="bankid">银行ID</param>
        /// <param name="returnurl">支付返回地址</param>
        /// <param name="remark1">备注1</param>
        /// <param name="remark2">备注2</param>
        /// <returns>提交表单HTML文本</returns>
        public string BuildFormHtml(string orderId, string money, string bankid, string returnurl, string remark1 = "", string remark2 = "")
        {
            if (string.IsNullOrEmpty(orderId))
            {
                DateTime dt = DateTime.Now;
                string v_ymd = dt.ToString("yyyyMMdd"); // yyyyMMdd
                string timeStr = dt.ToString("HHmmss"); // HHmmss
                orderId = v_ymd + _mid + timeStr;
            }

            string text = money + _money_type + orderId + _mid + returnurl + _key; // 拼凑加密串
            var sign = GetSign(text);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='chinabankform' action='" + GATEWAY + "' method='post'>");

            sbHtml.Append("<input type='hidden' name='v_md5info' value='" + sign + "'/>");
            sbHtml.Append("<input type='hidden' name='v_mid' value='" + _mid + "'/>");
            sbHtml.Append("<input type='hidden' name='v_oid' value='" + orderId + "'/>");
            sbHtml.Append("<input type='hidden' name='v_amount' value='" + money + "'/>");
            sbHtml.Append("<input type='hidden' name='v_moneytype' value='" + _money_type + "'/>");
            sbHtml.Append("<input type='hidden' name='v_url' value='" + returnurl + "'/>");
            sbHtml.Append("<input type='hidden' name='pmode_id' value='" + bankid + "'/>");
            sbHtml.Append("<input type='hidden' name='remark1' value='" + remark1 + "'/>");
            sbHtml.Append("<input type='hidden' name='remark2' value='[url:="+remark2+"]'/>");

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='Submit' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['chinabankform'].submit();</script>");

            return sbHtml.ToString();
        }

        /// <summary>
        /// 获取加密串
        /// </summary>
        /// <param name="text">需要加密的字符串</param>
        /// <returns>加密串</returns>
        public string GetSign(string text)
        {
            var sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, _sign_type);
            return sign;
        }

        /// <summary>
        /// 校验返回数据
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="status">支付状态，20为支付成功，30为支付失败</param>
        /// <param name="money">支付金额</param>
        /// <param name="sign">加密串</param>
        /// <param name="moneytype">货币种类，初始化类时的货币种类</param>
        /// <param name="key">商户密钥，默认为初始化类时的密钥</param>
        /// <returns>是否通过校验</returns>
        public string GetReciveSign(string orderId, string status, string money, string moneytype = "", string key = "")
        {
            try
            {
                moneytype = string.IsNullOrEmpty(moneytype) ? _money_type : moneytype;
                key = string.IsNullOrEmpty(key) ? _key : key;

                string str = orderId + status + money + moneytype + key;

                var mysign = GetSign(str);

                return mysign;
            }
            catch
            {
                return "";
            }
        }
    }
}

