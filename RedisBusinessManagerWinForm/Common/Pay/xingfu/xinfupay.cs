using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Common.Pay.xingfu
{
    public class xinfupay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="express_return_url"></param>
        /// <param name="express_notify_url"></param>
        /// <param name="apiName"></param>
        /// <param name="vendor_id"></param>
        /// <param name="orderId"></param>
        /// <param name="money"></param>
        /// <param name="userId"></param>
        /// <param name="privatekey"></param>
        /// <returns></returns>
        public static string xinfu_result(string url, string express_return_url, string express_notify_url, string apiName, string vendor_id, string orderId, decimal money, string userId, string privatekey)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("apiName", apiName);//网关支付:WEB_PAY_B2C (移动 P WAP ： 快捷支付： “ MOBI_PAY_B2C”)(微信： 支付： “ WECHAT_PAY ”)(QQ  直连 ：“ QQ_PAY”)
            dict.Add("apiVersion", "1.0.0.0");
            dict.Add("platformID", vendor_id);
            dict.Add("merchNo", vendor_id);
            dict.Add("orderNo", orderId);
            dict.Add("tradeDate", DateTime.Now.ToString("yyyyMMdd"));
            dict.Add("amt", money.ToString());
            dict.Add("merchUrl", express_notify_url);
            dict.Add("merchParam", userId);
            dict.Add("tradeSummary", "xin001");
            if (apiName != "WEB_PAY_B2C" && apiName != "MOBI_PAY_B2C")
                dict.Add("customerIP", "127.0.0.1");
            string signSrc = HttpHelp.GetdictStr(dict);
            dict.Add("signMsg", HttpHelp.MD5(signSrc + privatekey, Encoding.UTF8));
            if (apiName == "WEB_PAY_B2C" || apiName == "MOBI_PAY_B2C")
                dict.Add("bankCode", "");
            if (apiName == "WEB_PAY_B2C" || apiName == "MOBI_PAY_B2C")
            {
                string response = HttpHelp.Post_toPage(url, dict);
                return response;
            }
            else
            {
                string para = HttpHelp.GetdictStr(dict);
                string response = HttpHelp.HttpPost(url, para);
                var xml = XDocument.Parse(response);
                var xml_root = xml.Root;
                var xml_root_respData = xml_root.Element("respData");
                var respCode = xml_root_respData.Element("respCode").Value;
                if (respCode == "00")
                {
                    var code_url = xml_root_respData.Element("codeUrl").Value;
                    //code_url = Encoding.GetEncoding("GBK").GetString(Convert.FromBase64String(code_url));
                    code_url = HttpHelp.Base64Decode(code_url);
                    return string.Format("{0}|{1}", respCode, code_url);
                }
                else
                {
                    return string.Format("{0}|{1}", respCode, xml_root_respData.Element("respDesc").Value);
                }
            }
        }

        /// <summary>
        /// 新付代付支持的银行
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public static string checkBankName(string bankName)
        {
            string no = "0";
            switch (bankName)
            {
                case "中国工商银行":
                case "工商银行":
                    no = "ICBC";
                    break;
                case "中国农业银行":
                case "农业银行":
                    no = "ABC";
                    break;
                case "中国银行":
                    no = "BOC";
                    break;
                case "中国建设银行":
                case "建设银行":
                    no = "CCB";
                    break;
                case "交通银行":
                case "中国交通银行":
                    no = "COMM";
                    break;
                case "邮政储蓄银行":
                case "中国邮政储蓄银行":
                    no = "PSBC";
                    break;
                case "中信银行":
                case "中国中信银行":
                    no = "CNCB";
                    break;
                case "中国光大银行":
                case "光大银行":
                    no = "CEB";
                    break;
                case "华夏银行":
                case "中国华夏银行":
                    no = "HXB";
                    break;
                case "中国民生银行":
                case "民生银行":
                    no = "CMBC";
                    break;
                case "中国招商银行":
                case "招商银行":
                    no = "CMB";
                    break;
                case "兴业银行":
                case "中国兴业银行":
                    no = "CIB";
                    break;
                case "广发银行":
                case "中国广发银行":
                    no = "GDB";
                    break;
                case "平安银行":
                case "中国平安银行":
                    no = "PAB";
                    break;
                case "上海浦东发展银行":
                case "浦发银行":
                case "上海浦发银行":
                    no = "SPDB";
                    break;
            }
            return no;
        }
    }
}
