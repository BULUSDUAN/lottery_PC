using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Pay.tianfubao
{
    public class Pay
    {
        /// <summary>
        /// 天付宝微信
        /// </summary>
        public static string tfbweixinresult(string express_notify_url, string express_return_url, decimal money, string userId, string pay_type, string orderId, string tfb_spid, string tfb_Key, string tfb_PostUrl_weixin)
        {
            //string express_show_url = fromDomain;
            //string express_return_url = WebSiteUrl + "/member/safe";
            //string express_notify_url = fromDomain + "/user/TFBNotifyUrl";


            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            dict.Add("spid", tfb_spid);//商户号
            dict.Add("sp_billno", orderId.Replace("UFM", ""));// 商户系统内部的订单号,32个字符内、可包含字母
            dict.Add("spbill_create_ip", "127.0.0.1");
            dict.Add("pay_type", "800201"); //// 支付方式：扫码支付:800201；刷卡支付:800208；微信公众号支付:800207
            dict.Add("cur_type", "CNY");//金额类型 1 – 人民币(单位：分)
            dict.Add("tran_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dict.Add("tran_amt", (money * 100).ToString());
            dict.Add("item_name", orderId);

            dict.Add("bank_mch_id", "0000001");
            dict.Add("bank_mch_name", "0000002");

            dict.Add("notify_url", express_notify_url);//后台回调地址
            dict.Add("return_url", express_return_url);//页面回调地址

            dict.Add("out_channel", "wxpay");

            string signstr = Common.Pay.tianfubao.Util.GetSignStr(dict);//拼接签名原串
            string sign = Common.Pay.tianfubao.Util.MD5(signstr + "&key=" + tfb_Key, Encoding.GetEncoding("GBK"));//生成签名

            dict.Add("sign", sign);


            dict.Add("sign_type", "MD5");
            dict.Add("sign_key_index", "1");
            dict.Add("ver", "1");
            //dict.Add("input_charset", "UTF-8");
            string content = Common.Pay.tianfubao.Util.GetdictStr(dict);
            string response = Common.Pay.tianfubao.Util.HttpPost(tfb_PostUrl_weixin, content, Encoding.GetEncoding("GBK"));
            var dic = Common.Pay.tianfubao.Util.ParseXml(response);
            if (dic["retcode"].ToString() == "00")
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["qrcode"].ToString()); //return dic["qrcode"].ToString();
            }
            else
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["retmsg"].ToString());
            }
        }

        /// <summary>
        /// 天付宝qq
        /// </summary>
        public static string tfbqqresult(string express_notify_url, string express_return_url, decimal money, string userId, string pay_type, string orderId, string tfb_spid, string tfb_Key, string tfb_PostUrl_weixin)
        {

            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            dict.Add("spid", tfb_spid);//商户号
            dict.Add("sp_billno", orderId.Replace("UFM", ""));// 商户系统内部的订单号,32个字符内、可包含字母
            dict.Add("spbill_create_ip", "127.0.0.1");
            dict.Add("pay_type", "800201"); //// 支付方式：扫码支付:800201；刷卡支付:800208；微信公众号支付:800207
            dict.Add("cur_type", "CNY");//金额类型 1 – 人民币(单位：分)
            dict.Add("tran_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dict.Add("tran_amt", (money * 100).ToString());
            dict.Add("item_name", orderId);

            dict.Add("bank_mch_id", "0000001");
            dict.Add("bank_mch_name", "0000002");

            dict.Add("notify_url", express_notify_url);//后台回调地址
            dict.Add("return_url", express_return_url);//页面回调地址

            dict.Add("out_channel", "qqpay");

            string signstr = Common.Pay.tianfubao.Util.GetSignStr(dict);//拼接签名原串
            string sign = Common.Pay.tianfubao.Util.MD5(signstr + "&key=" + tfb_Key, Encoding.GetEncoding("GBK"));//生成签名

            dict.Add("sign", sign);


            dict.Add("sign_type", "MD5");
            dict.Add("sign_key_index", "1");
            dict.Add("ver", "1");
            //dict.Add("input_charset", "UTF-8");
            string content = Common.Pay.tianfubao.Util.GetdictStr(dict);
            string response = Common.Pay.tianfubao.Util.HttpPost(tfb_PostUrl_weixin, content, Encoding.GetEncoding("GBK"));
            var dic = Common.Pay.tianfubao.Util.ParseXml(response);
            if (dic["retcode"].ToString() == "00")
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["qrcode"].ToString()); //return dic["qrcode"].ToString();
            }
            else
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["retmsg"].ToString());
            }
        }
        /// <summary>
        /// 天付宝支付宝
        /// </summary>
        public static string tfbalipayresult(string express_notify_url, string express_return_url, decimal money, string userId, string pay_type, string orderId, string tfb_spid, string tfb_Key, string tfb_PostUrl_alipay)
        {

            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            dict.Add("spid", tfb_spid);//商户号
            dict.Add("sp_billno", orderId.Replace("UFM", ""));// 商户系统内部的订单号,32个字符内、可包含字母
            dict.Add("spbill_create_ip", "127.0.0.1");
            dict.Add("pay_type", "800201"); //// 支付方式：扫码支付:800201；刷卡支付:800208；微信公众号支付:800207
            dict.Add("cur_type", "CNY");//金额类型 1 – 人民币(单位：分)
            dict.Add("tran_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dict.Add("tran_amt", (money * 100).ToString());
            dict.Add("item_name", orderId);

            dict.Add("bank_mch_id", "0000001");
            dict.Add("bank_mch_name", "0000002");

            dict.Add("notify_url", express_notify_url);//后台回调地址
            dict.Add("return_url", express_return_url);//页面回调地址

            //dict.Add("out_channel", "wxpay");

            string signstr = Common.Pay.tianfubao.Util.GetSignStr(dict);//拼接签名原串
            string sign = Common.Pay.tianfubao.Util.MD5(signstr + "&key=" + tfb_Key, Encoding.GetEncoding("GBK"));//生成签名

            dict.Add("sign", sign);


            dict.Add("sign_type", "MD5");
            dict.Add("sign_key_index", "1");
            dict.Add("ver", "1");
            //dict.Add("input_charset", "UTF-8");
            string content = Common.Pay.tianfubao.Util.GetdictStr(dict);
            string response = Common.Pay.tianfubao.Util.HttpPost(tfb_PostUrl_alipay, content, Encoding.GetEncoding("GBK"));
            var dic = Common.Pay.tianfubao.Util.ParseXml(response);
            if (dic["retcode"].ToString() == "00")
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["qrcode"].ToString()); //return dic["qrcode"].ToString();
            }
            else
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["retmsg"].ToString());
            }

        }


        /// <summary>
        /// 天付宝网银
        /// </summary>
        public static string tfbbankresult(string express_notify_url, string express_return_url, decimal money, string userId, string pay_type, string orderId, string tfb_spid, string tfb_Key, string tfb_PostUrl)
        {
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            dict.Add("spid", tfb_spid);//商户号
            dict.Add("sp_userid", "1000");
            dict.Add("spbillno", orderId.Replace("UFM", ""));// 商户系统内部的订单号,32个字符内、可包含字母
            dict.Add("money", (money * 100).ToString());
            dict.Add("cur_type", "1");
            dict.Add("memo", orderId);
            dict.Add("expire_time", "7200");
            dict.Add("card_type", "1");
            dict.Add("bank_segment", "6666");
            dict.Add("user_type", "1");
            dict.Add("channel", "1");
            dict.Add("encode_type", "MD5");
            dict.Add("notify_url", express_notify_url);//后台回调地址
            dict.Add("return_url", express_return_url);//页面回调地址
            string signstr = Common.Pay.tianfubao.Util.GetSignStr(dict);//拼接签名原串
            string sign = Common.Pay.tianfubao.Util.MD5(signstr + "&key=" + tfb_Key, Encoding.GetEncoding("GBK"));//生成签名
            dict.Add("sign", sign.ToLower());
            string content = Common.Pay.tianfubao.Util.GetdictStr(dict);
            string response = Common.Pay.tianfubao.Util.HttpPost(tfb_PostUrl, content, Encoding.GetEncoding("GBK"));
            return response.ToString();
        }
        /// <summary>
        /// 天下付银联扫码
        /// </summary>
        /// <param name="express_notify_url"></param>
        /// <param name="express_return_url"></param>
        /// <param name="money"></param>
        /// <param name="orderId"></param>
        /// <param name="tfb_spid"></param>
        /// <param name="tfb_Key"></param>
        /// <param name="tfb_PostUrl_upay"></param>
        /// <returns></returns>
        public static string tfbUpayResult(string express_notify_url, string express_return_url, decimal money, string orderId, string tfb_spid, string tfb_Key, string tfb_PostUrl_upay)
        {
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            dict.Add("spid", tfb_spid);//商户号
            dict.Add("notify_url", express_notify_url);//后台回调地址
            dict.Add("pay_show_url", express_return_url);//页面成功跳转地址
            dict.Add("sp_billno", orderId.Replace("UFM", ""));// 商户系统内部的订单号,32个字符内、可包含字母
            dict.Add("pay_type", "800201");//扫码:800201 刷卡:800208
            dict.Add("tran_time", DateTime.Now.ToString("yyyyMMddHHmmss"));//发起交易的时间，格式为yyyyMMddhhmmss
            dict.Add("tran_amt", (money * 100).ToString());//交易金额 单位为分
            dict.Add("cur_type", "CNY");//人民币：CNY
            dict.Add("auth_code", "");//用户二维码内容，支付方式为 刷卡必填,
            dict.Add("termId", "");//终端号,支付方式为刷卡必填
            dict.Add("item_name", orderId);//商品名称或标示
            dict.Add("item_attach", orderId);//商品附加数据，如商品描述等信息

            string signstr = Common.Pay.tianfubao.Util.GetSignStr(dict);//拼接签名原串
            string sign = Common.Pay.tianfubao.Util.MD5(signstr + "&key=" + tfb_Key, Encoding.GetEncoding("GB2312")).ToUpper();//生成签名
            dict.Add("sign", sign.ToLower());

            dict.Add("sign_type", "MD5");//签名类型
            dict.Add("ver", "1");//版本号，默认为1
            //dict.Add("input_charset", "GB2312");//字符编码,取值：GBK、UTF-8，默认：GBK。
            dict.Add("sign_key_index", "1");//多密钥支持的密钥序号，默认1

            string content = Common.Pay.tianfubao.Util.GetdictStr(dict);
            string response = Common.Pay.tianfubao.Util.HttpPost(tfb_PostUrl_upay, content, Encoding.GetEncoding("GB2312"));
            var dic = Common.Pay.tianfubao.Util.ParseXml(response);
            if (dic["retcode"].ToString() == "00")
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["qrcode"].ToString()); //return dic["qrcode"].ToString();
            }
            else
            {
                return string.Format("{0}|{1}", dic["retcode"].ToString(), dic["retmsg"].ToString());
            }
        }
    }
}
