using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;
using Common.Net;

namespace Common.Gateway.SXF
{
    /// <summary>
    /// 关于三峡付
    /// </summary>
    public class SXFPay
    {
        private static string _sxfKey = string.Empty;
        /// <summary>
        /// 三峡付key
        /// </summary>
        public static string SXFKey
        {
            get { return _sxfKey; }
        }

        private static string _sxfUrl = string.Empty;
        /// <summary>
        /// 三峡付接口地址
        /// </summary>
        public static string SXFUrl
        {
            get { return _sxfUrl; }
        }

        private static string _sxfNotifyUrl = string.Empty;
        /// <summary>
        /// 三峡付回调地址
        /// </summary>
        public static string NotifyUrl
        {
            get { return _sxfNotifyUrl; }
        }

        private static string _sxfPartnerId = string.Empty;
        /// <summary>
        /// 合作商ID
        /// </summary>
        public static string SXFPartnerId
        {
            get { return _sxfPartnerId; }
        }


        private static string _sxfCaiFuUserId = string.Empty;
        /// <summary>
        /// 彩富UserId
        /// </summary>
        public static string CaiFuUserId
        {
            get { return _sxfCaiFuUserId; }
        }

        private static int _sxfOutTimeSeconds = 10;
        /// <summary>
        /// 超时时间
        /// </summary>
        public static int SXFOutTimeSeconds
        {
            get { return _sxfOutTimeSeconds; }
        }

        /// <summary>
        /// 及时到账,用户转到彩富
        /// </summary>
        public static TransResult TransMoneyToCaiFu(string transFrom, string orderId, decimal money, out string sendData, out string resultData)
        {
            sendData = "";
            resultData = "";
            return TransMoney(transFrom, CaiFuUserId, orderId, money, out sendData, out resultData);
        }

        /// <summary>
        /// 及时到账,彩富转到用户
        /// </summary>
        public static TransResult TransMoneyToUser(string userId, string orderId, decimal money, out string sendData, out string resultData)
        {
            sendData = "";
            resultData = "";
            return TransMoney(CaiFuUserId, userId, orderId, money, out sendData, out resultData);
        }

        /// <summary>
        /// 及时到账
        /// </summary>
        public static TransResult TransMoney(string transFrom, string transTo, string orderId, decimal money, out string sendData, out string resultData)
        {
            sendData = "";
            resultData = "";

            var list = new List<string>();
            list.Add(string.Format("{0}={1}", "service", "create_direct_pay_by_user"));//不可空  接口名称  
            list.Add(string.Format("{0}={1}", "partner_id", SXFPartnerId));//不可空  合作商 ID
            //list.Add(string.Format("{0}={1}", "sign", ""));//不可空  签名串
            list.Add(string.Format("{0}={1}", "input_charset", "UTF-8"));//不可空  编码方式
            list.Add(string.Format("{0}={1}", "sign_type", "MD5"));//不可空  签名类型 
            list.Add(string.Format("{0}={1}", "notify_url", NotifyUrl));//通知地址
            list.Add(string.Format("{0}={1}", "out_trade_no", orderId));//不可空  外部交易系统交易流水号（合作商城要唯一）
            list.Add(string.Format("{0}={1}", "subject", ""));//交易标题 (商品名称)
            list.Add(string.Format("{0}={1}", "buyer_email", transFrom));//买家账号(买家账号，不为空表示授权支付无需登录，为空表示非登陆账户。)
            list.Add(string.Format("{0}={1}", "seller_email", transTo));//不可空  卖家账号
            list.Add(string.Format("{0}={1}", "amount", money * 100));//不可空  交易金额(单位为分)
            list.Add(string.Format("{0}={1}", "body", ""));//交易详细内容
            list.Add(string.Format("{0}={1}", "show_url", ""));//商品展示网址(收银台页面上，商品展示的超链接。)
            list.Add(string.Format("{0}={1}", "payMethod", ""));//支付方式(支付方式，如果为空，则支持余额支付和到网银支付，bankPay 为网银支付)
            list.Add(string.Format("{0}={1}", "default_bank", ""));//默认银行 (网银支付默认选中的银行，参考银行列表)
            list.Add(string.Format("{0}={1}", "royalty_parameters", ""));//分润账号 :（平级分润）：收款方 Email1^金额 1^备注 1|收款方Email2^ 金额 2^备注 2|收款方Email3^ 金额 3^备注 3
            list.Add(string.Format("{0}={1}", "return_url", ""));//跳转地址(平台处理完后，页面直接跳转的地址(默认为签约时的返回地址))

            try
            {
                resultData = DoRequest(list, out sendData);
                var p = System.Web.Helpers.Json.Decode(resultData);
                var process_date = p.process_date;  //支付时间
                var partner_id = p.partner_id;      //合作商户号
                var buyer_email = p.buyer_email;    //买家账号
                var is_success = p.is_success;      //是否成功：T-成功；F-失败
                var total_fee = p.total_fee;        //交易总金额
                var md5_sign = p.md5_sign;          //加密字符串
                var seller_email = p.seller_email;  //卖家账户号（商城账户号）
                var notify_id = p.notify_id;        //通知校验 ID（通知校验 ID，商户可以用这个流水号询问三峡付该条通知的合法性）
                var out_trade_no = p.out_trade_no;  //外部订单号（商城订单号）
                var pay_order_no = p.pay_order_no;  //内部订单号（支付平台订单号）
                var trade_status = p.trade_status;  //交易状态

            }
            catch (Exception ex)
            {
                resultData = ex.ToString();
                return TransResult.OutTime;
            }
            return TransResult.Success;
        }

        /// <summary>
        /// 用户退款
        /// </summary>
        public static TransResult RefundMoney(string orderId, string sxfOrderId, decimal money, out string sendData, out string resultData)
        {

            sendData = "";
            resultData = "";

            var list = new List<string>();
            list.Add(string.Format("{0}={1}", "service", "create_direct_pay_by_user"));//不可空  接口名称  
            list.Add(string.Format("{0}={1}", "partner_id", SXFPartnerId));//不可空  合作商 ID
            list.Add(string.Format("{0}={1}", "sign", ""));//不可空  签名串
            list.Add(string.Format("{0}={1}", "input_charset", "UTF-8"));//不可空  编码方式
            list.Add(string.Format("{0}={1}", "sign_type", "MD5"));//不可空  签名类型 
            list.Add(string.Format("{0}={1}", "notify_url", NotifyUrl));//通知地址
            list.Add(string.Format("{0}={1}", "out_trade_no", orderId));//不可空  外部交易系统交易流水号（合作商城要唯一）
            list.Add(string.Format("{0}={1}", "refund_out_trade_no", sxfOrderId));//不可空  外部交易系统原支付交易订单号
            list.Add(string.Format("{0}={1}", "amount", money * 100));//单位为分
            list.Add(string.Format("{0}={1}", "return_url", ""));//跳转地址(平台处理完后，页面直接跳转的地址(默认为签约时的返回地址))

            try
            {
                resultData = DoRequest(list, out sendData);
                var p = System.Web.Helpers.Json.Decode(resultData);
                var process_date = p.process_date;  //支付时间
                var partner_id = p.partner_id;      //合作商户号
                var is_success = p.is_success;      //是否成功：T-成功；F-失败
                var total_fee = p.total_fee;        //交易总金额
                var md5_sign = p.md5_sign;          //加密字符串
                var notify_id = p.notify_id;        //通知校验 ID（通知校验 ID，商户可以用这个流水号询问三峡付该条通知的合法性）
                var out_trade_no = p.out_trade_no;  //外部订单号（商城订单号）
                var refund_out_trade_no = p.refund_out_trade_no;  //外部退款订单号（原支付订单号）
                var payMethod = p.payMethod;  //支付方式

            }
            catch (Exception ex)
            {
                resultData = ex.ToString();
                return TransResult.OutTime;
            }
            return TransResult.Success;
        }

        /// <summary>
        /// 加密请求，得到返回数据
        /// </summary>
        private static string DoRequest(List<string> list, out string sendData)
        {
            var source = string.Join("&", list.ToArray());
            var sign = GetSign(source, SXFKey);
            list.Add(string.Format("{0}={1}", "sign", sign));

            sendData = string.Join("&", list.ToArray());

            var url = string.Format("{0}?{1}", SXFUrl, sendData);
            return PostManager.Get(url, Encoding.UTF8, SXFOutTimeSeconds);
        }

        private static string GetSign(string source, string key)
        {
            return Encipherment.MD5(string.Format("{0}{1}", source, key), Encoding.UTF8);
        }
    }
}
