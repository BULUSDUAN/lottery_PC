using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Pay.af
{
    /// <summary>
    /// 
    /// </summary>
    public class afPay
    {

        #region "艾付"
        /// <summary>
        /// 艾付 扫码通用方法
        /// </summary>
        /// <param name="af_url"></param>
        /// <param name="af_key"></param>
        /// <param name="af_merchant_no"></param>
        /// <param name="orderId"></param>
        /// <param name="notify_url"></param>
        /// <param name="return_url"></param>
        /// <param name="money"></param>
        /// <param name="bankid"></param>
        /// <returns></returns>
        public static string af_reslut(string af_url, string af_key, string af_merchant_no, string orderId, string notify_url, string return_url, decimal money, string bankid)
        {
            string version = "v1";//接口版本
            string merchant_no = af_merchant_no;//商户号
            string order_no = orderId;//商户订单号
            string goods_name = "在线充值";//商品名称
            string order_amount = money.ToString();//订单金额
            string backend_url = notify_url;//支付结果异步通知地址
            string frontend_url = return_url;//支付结果同步通知地址
            string reserve = "";//商户保留信息
            string pay_mode = "09";//01支付模式 09 扫码
            string bank_code = bankid;//银行编号
            string card_type = "0";//允许支付的银行卡类型
            goods_name = Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, goods_name);
            //MD5签名串
            string src = "version=" + version + "&merchant_no=" + merchant_no + "&order_no="
                    + order_no + "&goods_name=" + goods_name + "&order_amount=" + order_amount
                    + "&backend_url=" + backend_url + "&frontend_url="
                    + frontend_url + "&reserve=" + reserve
                    + "&pay_mode=" + pay_mode + "&bank_code=" + bank_code + "&card_type="
                    + card_type;
            src += "&key=" + af_key;
            string sign = Common.Pay.af.afUtil.MD5(src, Encoding.UTF8);
            src += "&sign=" + sign;
            return Common.Pay.af.afUtil.HttpPost(af_url + "?" + src.ToString(), "", Encoding.UTF8);
        }
        /// <summary>
        /// 艾付 扫码通用方法
        /// </summary>
        /// <param name="af_url"></param>
        /// <param name="af_key"></param>
        /// <param name="af_merchant_no"></param>
        /// <param name="orderId"></param>
        /// <param name="notify_url"></param>
        /// <param name="return_url"></param>
        /// <param name="money"></param>
        /// <param name="bankid"></param>
        /// <returns></returns>
        public static string af_H5reslut(string af_url, string af_key, string af_merchant_no, string orderId, string notify_url, string return_url, decimal money, string bankid)
        {
            string version = "v1";//接口版本
            string merchant_no = af_merchant_no;//商户号
            string order_no = orderId;//商户订单号
            string goods_name = "在线充值";//商品名称
            string order_amount = money.ToString();//订单金额
            string backend_url = notify_url;//支付结果异步通知地址
            string frontend_url = return_url;//支付结果同步通知地址
            string reserve = "";//商户保留信息
            string pay_mode = "12";//01支付模式 09 扫码 12：H5支付模式
            string bank_code = bankid;//银行编号
            string card_type = "0";//允许支付的银行卡类型
            goods_name = Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, goods_name);
            //MD5签名串
            string src = "version=" + version + "&merchant_no=" + merchant_no + "&order_no="
                    + order_no + "&goods_name=" + goods_name + "&order_amount=" + order_amount
                    + "&backend_url=" + backend_url + "&frontend_url="
                    + frontend_url + "&reserve=" + reserve
                    + "&pay_mode=" + pay_mode + "&bank_code=" + bank_code + "&card_type="
                    + card_type;
            src += "&key=" + af_key;
            string sign = Common.Pay.af.afUtil.MD5(src, Encoding.UTF8);
            src += "&sign=" + sign;
            //return Common.Pay.af.afUtil.HttpPost(af_url + "?" + src.ToString(), "", Encoding.UTF8);
            return af_url + "?" + src.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="_posturl"></param>
        public static void AFPost_toPage(string url, out StringBuilder _posturl)
        {
            _posturl = new StringBuilder();
            _posturl.Append("<Html> \n");
            _posturl.Append("    <Head> \n");
            _posturl.Append("        <Body> \n");
            _posturl.Append("            <Form name='SendForm' method='post' action='" + url + "' target='_self'> \n");
            _posturl.Append("            </Form> \n");
            _posturl.Append("            <script language='javascript'type='text/JavaScript'> \n");
            _posturl.Append("                SendForm.submit(); \n");
            _posturl.Append("            </script> \n");
            _posturl.Append("        </Body> \n");
            _posturl.Append("    </Head> \n");
            _posturl.Append("</Html> \n");
        }

        /// <summary>
        /// 艾付代付支持的银行
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
                    no = "BOCOM";
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
                case "宁波银行":
                    no = "NBB";
                    break;
                case "杭州银行":
                    no = "HZB";
                    break;
                case "南京银行":
                    no = "NJB";
                    break;
                case "北京银行":
                    no = "BCCB";
                    break;
            }
            return no;
        }
        #endregion
    }
}
