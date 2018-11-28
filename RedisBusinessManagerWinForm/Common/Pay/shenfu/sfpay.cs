using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace Common.Pay.shenfu
{
    public class sfpay
    {

        public string DoPost(string orderId, int payMethod, decimal amount, string userId, string userName, string notifyUrl, string returnUrl, bool h5, string key, string customerId, out bool isSuccess)
        {
            isSuccess = false;
            PayRequest request = new PayRequest()
            {
                orderId = orderId,
                amount = Convert.ToInt32(amount * 100),
                userId = userId,
                userName = userName,
                orderName = "wc",
                comment = "wc",
                notifyUrl = notifyUrl,
                returnUrl = returnUrl,
                payMethod = (PayMethod)payMethod,
                h5 = h5
            };

            string json = Serialize(request);//https://paytz.qianqicn.com
            string url = string.Format("https://paytz.shenfu8.com/paygate/api/pay?customer={0}&sign={1}", customerId, MD5(json, key));
            string resultString = PostJson(url, json);
            Dictionary<string, string> resultMap = Deserialize<Dictionary<string, string>>(resultString);
            if (resultMap["code"] == "200")
            {
                isSuccess = true;
                return resultMap["data"];
            }
            else
            {
                return resultString;
            }
            
            
        }

        public bool payNotify(string sign, string json, string key)
        {
            string mySign = MD5(json, key);
            if (!mySign.Equals(sign))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string MD5(params string[] inputs)
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("", inputs)));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sBuilder.Append(bytes[i].ToString("x2"));
            }
            return sBuilder.ToString().ToLower();
        }

        public string Serialize(object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public T Deserialize<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        public string PostJson(string url, string json)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            byte[] datas = Encoding.UTF8.GetBytes(json);
            webRequest.GetRequestStream().Write(datas, 0, datas.Length);
            StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string result = reader.ReadToEnd();
            reader.Close();
            return result;
        }
    }


    public class PayRequest
    {
        public PayMethod payMethod;
        //public PaySence paySence;

        public int amount;// 分
        public string orderId;// 商户订单号
        public string notifyUrl;// 异步通知URL
        public string returnUrl;// 支付成功返回URL
        public string orderName;
        public string comment;
        public string userId;//商户用户ID，必须唯一
        public string userName;//商户用户名，充值时备注，最好唯一

        //TODO:实名认证，反洗钱
        public string mobile;//手机号
        public string realName;//实名
        public string idCard;//身份证

        public string bankCode;

        public bool h5;

    }


    public enum PayMethod
    {
        EXPRESS = 1,
        BANK = 2,
        WEIXIN = 3,
        ALIPAY = 4,
        QQ = 5,
        UNIONPAY = 6,
    }

    public class NotifyInfo
    {
        /// <summary>
        /// 支付平台订单号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 商户平台订单号
        /// </summary>
        public string customerOrderId { get; set; }

        /// <summary>
        /// 支付平台商户号
        /// </summary>
        public string customerId { get; set; }

        /// <summary>
        /// 商户提交订单时填写的备注
        /// </summary>
        public string customerComment { get; set; }

        /// <summary>
        /// 订单状态。200为成功；403为失败；202为正在进行中
        /// </summary>
        public OrderStatus status { get; set; }

        /// <summary>
        /// 订单金额，单位为分
        /// </summary>
        public long amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }

        public string payUrl { get; set; }

        public string payQRCode { get; set; }
    }

    public enum OrderStatus
    {
        Accepted = 202, //正在确认充值/打款
        OK = 200, //充值/提款成功
        Forbidden = 403 //拒绝提款/充值
    }

    public class Result<T>
    {
        public int code { get; set; }

        public T data { get; set; }

        public string message { get; set; }

        public bool succeed
        {
            get { return code == 200; }
        }
    }
}
