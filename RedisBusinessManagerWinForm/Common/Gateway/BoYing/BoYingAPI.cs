using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;
using System.Web;

namespace Common.Gateway.BoYing
{
    /// <summary>
    /// 波银API
    /// </summary>
    public class BoYingAPI
    {
        /// <summary>
        /// 波银提交地址
        /// </summary>
        public static string BoYingUrl { get { return "http://api.baozhang1.com/api/pay"; } }

        /// <summary>
        /// 生成网银支付请求参数
        /// </summary>
        public static string BuildPayRequestParams(string orderId, int bank, decimal money, string callback_front, string callback_push)
        {
            var requestList = new List<string>();
            requestList.Add(string.Format("{0}", 8));
            requestList.Add(string.Format("1={0}", orderId));
            requestList.Add(string.Format("2={0}", GetTimeStamp()));
            requestList.Add(string.Format("3={0}", bank));
            requestList.Add(string.Format("4={0}", "1"));
            requestList.Add(string.Format("5={0}", money));
            requestList.Add(string.Format("6={0}", "在线充值"));
            requestList.Add(string.Format("7={0}", callback_front));
            requestList.Add(string.Format("8={0}", callback_push));
            var request = string.Join("|", requestList);
            var data = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(request)), Encoding.UTF8);
            return data;
        }

        private static long GetTimeStamp()
        {
            System.DateTime startTime = new System.DateTime(1970, 1, 1);
            return (long)((DateTime.Now.ToUniversalTime() - startTime).TotalSeconds);

            var transtime = (long)((DateTime.Now.AddHours(-8) - startTime).TotalSeconds);
            return transtime;
        }

        /// <summary>
        /// 生成加密数据
        /// </summary>
        public static string BuildPaySign(string partnerId, string hashKey, string data)
        {
            var sign = Encipherment.MD5(string.Format("{0}&{1}&{2}", partnerId, data, hashKey), Encoding.UTF8);
            return sign;
        }

        /// <summary>
        /// 解析回复数据中的data
        /// </summary>
        public static string GetResponseData(string response)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(response)));
        }

        /// <summary>
        /// 获取回复的sign
        /// </summary>
        public static string GetResponseSign(string partnerId, string hashKey, string data)
        {
            return Encipherment.MD5(string.Format("{0}&{1}&{2}", partnerId, data, hashKey));
        }

    }
}
