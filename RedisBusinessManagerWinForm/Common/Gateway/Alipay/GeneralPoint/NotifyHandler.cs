using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace Common.Gateway.Alipay.GeneralPoint
{
    /// <summary>
    ///通知信息封装类
    /// </summary>
    public class NotifyHandler
    {
        /// <summary>
        /// 构造函数
        /// 从配置文件中初始化变量
        /// </summary>
        /// <param name="partner">商户ID</param>
        /// <param name="key">安全校验码，与partner是一组的</param>
        /// <param name="seller_email">商家签约时的支付宝账号，即收款的支付宝账号</param>
        public NotifyHandler(string partner, string key, string seller_email)
        {
            AlipayConfig.Partner = partner;
            AlipayConfig.Key = key;
            AlipayConfig.Seller_Email = seller_email;
        }

        /// <summary>
        /// 将string转成bool型
        /// </summary>
        /// <param name="text">bool型字符串</param>
        /// <returns>返回true或false</returns>
        private bool InputBoolean(string text)
        {
            bool result = false;
            if (bool.TryParse(text, out result) == true)
                return result;
            else
                return false;
        }

        /// <summary>
        /// 验证通知ID的合法性
        /// </summary>
        /// <param name="IsHttps">是否使用Https,这个要你的服务器支持HTPPS</param>
        /// <param name="notifyid">通知ID</param>
        /// <returns>返回true或false</returns>
        public bool VerificationNotifyID(bool IsHttps, string notifyid)
        {
            string alipayNotifyURL = "https://www.alipay.com/cooperate/gateway.do?service=notify_verify";
            string parameters = "&partner=" + AlipayConfig.Partner + "&notify_id=" + notifyid;
            if (IsHttps == false)
            {
                alipayNotifyURL = "http://notify.alipay.com/trade/notify_query.do?";
            }
            string getstr = VerificationHttp(alipayNotifyURL += parameters);
            return InputBoolean(getstr);
        }

        /// <summary>
        /// 验证通知ID的合法性(默认使用http请求验证)
        /// </summary>
        /// <param name="notifyid">通知ID</param>
        /// <returns>返回true或false</returns>
        public bool VerificationNotifyID(string notifyid)
        {
            return VerificationNotifyID(false, notifyid);
        }

        /// <summary>
        /// 将返回的URL生成Md5摘要
        /// </summary>
        /// <param name="coll">NameValues集合，通过Request.QueryString或Request.Form方式取得</param>
        /// <returns>返回sign串</returns>
        public string GetMD5Sign(System.Collections.Specialized.NameValueCollection coll)
        {
            return Alipay.GetMD5(BuildSignString(coll, AlipayConfig.Key), Alipay._Input_Charset);
        }

        /// <summary>
        /// 获取支付宝远程服务器ATN结果，验证是否是支付宝服务器发来的请求
        /// </summary>
        /// <param name="URLStr">请求URL地址，需带通知的ID,partnerID等参数</param>
        /// <returns>返回true是正确的信息，false是无效的</returns>
        private string VerificationHttp(string URLStr)
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            try
            {
                System.Net.HttpWebRequest myReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URLStr);
                myReq.Timeout = 120000;//超时时间
                System.Net.HttpWebResponse HttpWResp = (System.Net.HttpWebResponse)myReq.GetResponse();
                System.IO.Stream myStream = HttpWResp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(myStream, Encoding.Default);
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }
            }
            catch (Exception exp)
            {
                strBuilder.Append("错误:" + exp.Message);
            }

            return strBuilder.ToString();
        }


        /// <summary>
        /// 构造md5签名字符串
        /// </summary>
        /// <param name="nvc">NameValues集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>返回排序后的字符串（自动剔除末尾的sign和sign_type类型）</returns>
        private string BuildSignString(System.Collections.Specialized.NameValueCollection nvc, string key)
        {
            string[] Sortedstr = Alipay.BubbleSort(nvc.AllKeys);  //对参数进行排序
            StringBuilder prestr = new StringBuilder();           //构造待md5摘要字符串 
            for (int i = 0; i < Sortedstr.Length; i++)
            {
                if (nvc.Get(Sortedstr[i]) != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i] + "=" + nvc.Get(Sortedstr[i]));
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "=" + nvc.Get(Sortedstr[i]) + "&");
                    }
                }
            }
            prestr.Append(key);//追加key
            return prestr.ToString();
        }
    }
}
