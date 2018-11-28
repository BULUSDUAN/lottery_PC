using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;

namespace Common.Pay.sfb
{
    public class pay
    {

        public static string sfb_result(string express_return_url, string express_notify_url, string merchantcode, string orderId, decimal money, string userId, string pay_type, string sfb_privatekey, string service_type = "direct_pay")
        {
            string strmoney = Convert.ToInt32(money).ToString();
            string order_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string product_name = "pay";
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            dict.Add("merchant_code", merchantcode);
            dict.Add("service_type", service_type);//qq_scan //QQ扫码            
            dict.Add("notify_url", express_notify_url);
            string interface_version = "V3.0";
            string url = "https://pay.zfbill.net/gateway?input_charset=UTF-8";
            if (service_type != "direct_pay")
            {
                url = "https://api.zfbill.net/gateway/api/scanpay";
                interface_version = "V3.1";
            }
            else
            {
                dict.Add("input_charset", "UTF-8");
            }
            dict.Add("interface_version", interface_version);
            dict.Add("order_no", orderId);
            dict.Add("order_time", order_time);
            dict.Add("order_amount", strmoney);
            dict.Add("product_name", product_name);
            dict.Add("bank_code", "");
            dict.Add("client_ip_check", "");
            dict.Add("client_ip", "127.0.0.1");
            dict.Add("extend_param", "");
            dict.Add("extra_return_param", "");
            dict.Add("product_code", "");
            dict.Add("product_desc", "");
            dict.Add("product_num", "");
            dict.Add("return_url", express_return_url);
            dict.Add("show_url", "");
            dict.Add("redo_flag", "1");
            dict.Add("pay_type", pay_type);
            string signSrc = HttpHelp.GetdictStr(dict);
            string merchant_private_key = Common.Pay.HttpHelp.RSAPrivateKeyJava2DotNet(sfb_privatekey);
            string signData = Common.Pay.HttpHelp.RSASign(signSrc, merchant_private_key);
            if (service_type != "direct_pay")
            {
                signData = HttpUtility.UrlEncode(signData);
                dict.Add("sign_type", "RSA-S");
                dict.Add("sign", signData);
                string para = HttpHelp.GetdictStr(dict);
                string _xml = HttpHelp.HttpPost(url, para);
                //将同步返回的xml中的参数提取出来
                var el = XElement.Load(new StringReader(_xml));
                //将QRcode从XML中提取出来
                var qrcode1 = el.XPathSelectElement("/response/qrcode");
                if (qrcode1 == null)
                {
                    return "";
                }
                //去掉首尾的标签并转换成string
                string qrcode = Regex.Match(qrcode1.ToString(), "(?<=>).*?(?=<)").Value;   //二维码链接
                return qrcode;
            }
            else
            {
                dict.Add("sign_type", "RSA-S");
                dict.Add("sign", signData);
                string html = HttpHelp.Post_toPage(url,dict);
                return html;
            }
           
        }

    }
}
