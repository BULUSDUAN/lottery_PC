using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Net;

namespace Common.Gateway.Alipay.Pay
{
    /// <summary>
    /// 支付宝urlSign构造类（商户可在此类构造URL Sign）
    /// </summary>
    public class AlipaySign
    {
        /// <summary>
        /// 构造函数
        /// 从配置文件中初始化变量
        /// </summary>
        public AlipaySign(string partner, string key, string seller_email)
        {
            AlipayConfig.Partner = partner;
            AlipayConfig.Key = key;
            AlipayConfig.Seller_Email = seller_email;
        }

        #region 即时到账接口

        /// <summary>
        /// 构造高级即时到账接口sign
        /// </summary>
        /// <param name="out_trade_no">客户自己的订单号，订单号必须在订单系统中保持唯一性</param>
        /// <param name="subject">商品名称，也可称为订单名称，该接口并不是单一的只能买一样东西，可把一次支付当作一次下订单</param>
        /// <param name="body">商品描述，即备注</param>
        /// <param name="total_fee">商品价格，也可称为订单的总金额 0.01-50000.00</param>
        /// <param name="show_url">商品展示Url</param>
        /// <param name="return_url">返回通知url，必须是完整的路径地址，错误或不正确会收不到通知</param>
        /// <param name="notify_url">服务器通知url，必须是完整的路径外网地址，内网ip或不正确的地址会导致收不到通知</param>
        /// <param name="qr_pay_mode">是否扫码支付 1：订单码-前置模式 2：订单码-跳转模式 默认为空表示不使用扫码支付</param>
        /// <returns>生成的URL签名</returns>
        /// <remarks>注意传入的参数不能为空</remarks>
        public string CreateDirectPayByUser(string out_trade_no, string subject, string body, string total_fee, string show_url, string return_url, string notify_url, string paymethod, string defaultbank, string token, string qr_pay_mode = "")
        {
            //扩展功能参数——防钓鱼//

            //防钓鱼时间戳
            string anti_phishing_key = Query_timestamp();  //获取防钓鱼时间戳函数
            //获取客户端的IP地址，建议：编写获取客户端IP地址的程序
            string exter_invoke_ip = IpManager.IPAddress;
            //注意：
            //请慎重选择是否开启防钓鱼功能
            //exter_invoke_ip、anti_phishing_key一旦被设置过，那么它们就会成为必填参数
            //建议使用POST方式请求数据
            //示例：
            //exter_invoke_ip = "";
            //Service aliQuery_timestamp = new Service();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("service", "create_direct_pay_by_user");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partner", AlipayConfig.Partner);
            dic.Add("_input_charset", Alipay._Input_Charset);
            dic.Add("seller_email", AlipayConfig.Seller_Email);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("subject", subject);
            dic.Add("body", body);
            dic.Add("total_fee", total_fee);
            dic.Add("show_url", show_url);
            dic.Add("payment_type", "1");
            dic.Add("return_url", return_url);
            dic.Add("notify_url", notify_url);
            if (!string.IsNullOrEmpty(qr_pay_mode))
            {
                dic.Add("qr_pay_mode", qr_pay_mode);
            }
            dic.Add("paymethod", paymethod);
            if (paymethod == "bankPay")
            {
                dic.Add("defaultbank", defaultbank);
            }
            if (!string.IsNullOrEmpty(token))
            {
                dic.Add("token", token);
            }
            dic.Add("anti_phishing_key", anti_phishing_key);
            dic.Add("exter_invoke_ip", exter_invoke_ip);
            return Alipay.AlipayDoGet(dic, AlipayConfig.Key);
        }

        /// <summary>
        /// 构造高级即时到账接口sign
        /// </summary>
        /// <param name="out_trade_no">客户自己的订单号，订单号必须在订单系统中保持唯一性</param>
        /// <param name="subject">商品名称，也可称为订单名称，该接口并不是单一的只能买一样东西，可把一次支付当作一次下订单</param>
        /// <param name="body">商品描述，即备注</param>
        /// <param name="total_fee">商品价格，也可称为订单的总金额 0.01-50000.00</param>
        /// <param name="return_url">返回通知url，必须是完整的路径地址，错误或不正确会收不到通知</param>
        /// <param name="notify_url">服务器通知url，必须是完整的路径外网地址，内网ip或不正确的地址会导致收不到通知</param>
        /// <param name="qr_pay_mode">是否扫码支付 1：订单码-前置模式 2：订单码-跳转模式 默认为空表示不使用扫码支付</param>
        /// <returns>生成的URL签名</returns>
        /// <remarks>注意传入的参数不能为空</remarks>
        public string CreateDirectPayByUser(string out_trade_no, string subject, string body, string total_fee, string return_url, string notify_url, string qr_pay_mode = "")
        {
            //扩展功能参数——防钓鱼//

            //防钓鱼时间戳
            string anti_phishing_key = Query_timestamp();  //获取防钓鱼时间戳函数
            //获取客户端的IP地址，建议：编写获取客户端IP地址的程序
            string exter_invoke_ip = IpManager.IPAddress;
            //注意：
            //请慎重选择是否开启防钓鱼功能
            //exter_invoke_ip、anti_phishing_key一旦被设置过，那么它们就会成为必填参数
            //建议使用POST方式请求数据
            //示例：
            //exter_invoke_ip = "";
            //Service aliQuery_timestamp = new Service();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("service", "create_direct_pay_by_user");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partner", AlipayConfig.Partner);
            dic.Add("_input_charset", Alipay._Input_Charset);
            dic.Add("seller_email", AlipayConfig.Seller_Email);
            dic.Add("out_trade_no", out_trade_no);
            dic.Add("subject", subject);
            dic.Add("body", body);
            dic.Add("total_fee", total_fee);
            dic.Add("payment_type", "1");
            dic.Add("return_url", return_url);
            dic.Add("notify_url", notify_url);
            dic.Add("qr_pay_mode", qr_pay_mode);
            dic.Add("anti_phishing_key", anti_phishing_key);
            dic.Add("exter_invoke_ip", exter_invoke_ip);
            return Alipay.AlipayDoGet(dic, AlipayConfig.Key);
        }

        /// <summary>
        /// 用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
        /// 注意：远程解析XML出错，与IIS服务器配置有关
        /// </summary>
        /// <returns>时间戳字符串</returns>
        public string Query_timestamp()
        {
            string url = Alipay.GateWayN + "service=query_timestamp&partner=" + AlipayConfig.Partner;
            string encrypt_key = "";

            XmlTextReader Reader = new XmlTextReader(url);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Reader);

            encrypt_key = xmlDoc.SelectSingleNode("/alipay/response/timestamp/encrypt_key").InnerText;

            return encrypt_key;
        }
        #endregion


    }
}
