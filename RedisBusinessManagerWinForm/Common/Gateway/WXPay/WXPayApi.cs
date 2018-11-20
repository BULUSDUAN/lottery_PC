using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;

namespace Common.Gateway.WXPay
{
    /// <summary>
    /// 微信支付api
    /// </summary>
    public class WXPayApi
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        //public const string APPID = "wx2428e34e0e7dc6ef";
        //public const string MCHID = "1233410002";
        //public const string KEY = "e10adc3849ba56abbe56e056f20f883e";
        //public const string APPSECRET = "51c56b886b5be869567dd389b3e5d1d6";
        public string AppId { get; set; }
        public string Mchid { get; set; }
        public string Key { get; set; }
        public string Appsecret { get; set; }
        public string NoticeUrl { get; set; }


        public WXPayApi(string _appId, string _mchid, string _key, string _noticeUrl, string _appsecret = "")
        {
            this.AppId = _appId;
            this.Mchid = _mchid;
            this.Key = _key;
            this.Appsecret = _appsecret;
            this.NoticeUrl = _noticeUrl;
        }

        /**
        * 
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public WxPayData UnifiedOrder(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new Exception("缺少统一支付接口必填参数out_trade_no！");
            }
            if (!inputObj.IsSet("body"))
            {
                throw new Exception("缺少统一支付接口必填参数body！");
            }
            if (!inputObj.IsSet("total_fee"))
            {
                throw new Exception("缺少统一支付接口必填参数total_fee！");
            }
            if (!inputObj.IsSet("trade_type"))
            {
                throw new Exception("缺少统一支付接口必填参数trade_type！");
            }
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", NoticeUrl);//异步通知url
            }
            //if (!inputObj.IsSet("notify_url"))
            //{
            //    throw new Exception("缺少统一支付接口必填参数异步通知url！");
            //}
            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            inputObj.SetValue("appid", this.AppId);//公众账号ID
            inputObj.SetValue("mch_id", this.Mchid);//商户号
            //inputObj.SetValue("spbill_create_ip", IpManager.IPAddress);//终端ip	  	    
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString("N"));//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign(this.Key));
            string xml = inputObj.ToXml();
            string response = HttpService.Post(xml, url, false, timeOut);
            WxPayData result = new WxPayData();
            result.FromXml(response, this.Key);
            return result;
        }

        /**
     * 生成扫描支付模式一URL
     * @param productId 商品ID
     * @return 模式一URL
     */
        public string GetPrePayUrl(string productId)
        {
            WxPayData data = new WxPayData();
            data.SetValue("appid", this.AppId);//公众帐号id
            data.SetValue("mch_id", this.Mchid);//商户号
            data.SetValue("time_stamp", this.GenerateTimeStamp());//时间戳
            data.SetValue("nonce_str", Guid.NewGuid().ToString("N"));//随机字符串
            data.SetValue("product_id", productId);//商品ID
            data.SetValue("sign", data.MakeSign(this.Key));//签名
            string str = ToUrlParams(data.GetValues());//转换为URL串
            string url = "weixin://wxpay/bizpayurl?" + str;
            return url;
        }

        /**
        * 生成直接支付url，支付url有效期为2小时,模式二
        * @param productId 商品ID
        * @return 模式二URL
        */
        public string GetPayUrl(string productId, int money, string ip)
        {
            WxPayData data = new WxPayData();
            data.SetValue("body", "帐户余额充值");//商品描述
            data.SetValue("attach", "帐户余额充值");//附加数据
            data.SetValue("out_trade_no", productId);//随机字符串
            data.SetValue("total_fee", money * 100);//总金额，分为单位
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", "jjj");//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", productId);//商品ID
            data.SetValue("spbill_create_ip", ip);//ip

            WxPayData result = this.UnifiedOrder(data);//调用统一下单接口
            string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
            return url;
        }

        /**
        *    
        * 查询订单
        * @param WxPayData inputObj 提交给查询订单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回订单查询结果，其他抛异常
        */
        public WxPayData OrderQuery(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new Exception("订单查询接口中，out_trade_no、transaction_id至少填一个！");
            }

            inputObj.SetValue("appid", this.AppId);//公众账号ID
            inputObj.SetValue("mch_id", this.Mchid);//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString("N"));//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(this.Key));

            string xml = inputObj.ToXml();
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口提交数据

            //将xml格式的数据转化为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response, this.Key);
            return result;
        }

        /// <summary>
        /// 订单状态查询
        /// </summary>
        public WXOrderStatus OrderQuery(string out_trade_no)
        {
            WxPayData queryOrderInput = new WxPayData();
            queryOrderInput.SetValue("out_trade_no", out_trade_no);
            WxPayData result = OrderQuery(queryOrderInput);

            if (result.GetValue("return_code").ToString() == "SUCCESS"
                && result.GetValue("result_code").ToString() == "SUCCESS")
            {
                //支付成功
                if (result.GetValue("trade_state").ToString() == "SUCCESS")
                {
                    return WXOrderStatus.SUCCESS;
                }
                else if (result.GetValue("trade_state").ToString() == "NOTPAY")
                {
                    return WXOrderStatus.NOTPAY;
                }
                //用户支付中，需要继续查询
                else if (result.GetValue("trade_state").ToString() == "USERPAYING")
                {
                    return WXOrderStatus.USERPAYING;
                }
            }
            if (result.GetValue("return_code").ToString() == "FAIL")
                return WXOrderStatus.ERROR;

            //如果返回错误码为“此交易订单号不存在”则直接认定失败
            if (result.GetValue("err_code").ToString() == "ORDERNOTEXIST")
            {
                return WXOrderStatus.ORDERNOTEXIST;
            }
            return WXOrderStatus.ERROR;
        }

        /**
      * 参数数组转换为url格式
      * @param map 参数名与参数值的映射表
      * @return URL字符串
      */
        private string ToUrlParams(SortedDictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }

        /**
        * 根据当前系统时间加随机序列来生成订单号
         * @return 订单号
        */
        public string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", this.Mchid, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
        public string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

    }
}
