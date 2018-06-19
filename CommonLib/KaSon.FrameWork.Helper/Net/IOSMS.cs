using Common.Net.SMS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KaSon.FrameWork.Helper.Net
{
    /// <summary>
    /// 爱欧短信接口
    /// </summary>
    public class IOSMS : ISMSSend
    {
        //短信接口地址
        private static string serviceUrl = "http://sms.4006555441.com/WebService.asmx";
        //private static string serviceUrl = "http://58.67.156.147/WebService.asmx/mt?";

        //用户账号
        private string corpid = "";
        //用户密码
        private string pwd = "";
        private string config_attach = "";

        /// <summary>
        /// 初始化接口类
        /// </summary>
        /// <param name="pid">用户账号</param>
        /// <param name="password">用户密码</param>
        public IOSMS(string pid, string password, string attach)
        {
            corpid = pid;
            pwd = password;
            config_attach = attach;

        }

        /// <summary>
        /// 立即发送短信
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信手机70个字为一条，小灵通58个字为一条，超过会按两条发送</param>
        /// <returns>发送结果</returns>
        public string SendSMS(string mobile, string content, string attach)
        {
            string requeststr = "Sn=" + corpid + "&Pwd=" + pwd + "&mobile=" + mobile + "&content=" + content;
            string result = PostManager.Post(serviceUrl + "/mt?", requeststr, Encoding.UTF8);
            switch (result)
            {
                case "0":
                    return "发送成功";
                case "-1":
                    return "账号未注册";
                case "-2":
                    return "其他错误";
                case "-3":
                    return "密码错误";
                case "-4":
                    return "手机号码格式错误";
                case "-5":
                    return "余额不足";
                case "-6":
                    return "定时发送时间不是有效的时间格式";
                case "-7":
                    return "发送短信内容包含黑字典关键字";
                default:
                    return result;
            }
        }

        /// <summary>
        /// 批量发送短信
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信手机70个字为一条，小灵通58个字为一条，超过会按两条发送</param>
        /// <returns>发送结果</returns>
        public string SendSMS_Batch(string mobile, string content)
        {
            return "本接口不支持批量发送。";
        }

        /// <summary>
        /// 获取账户短信余额
        /// </summary>
        /// <returns>余额字符串</returns>
        public string GetBalance()
        {
            string reqstr = "Sn=" + corpid + "&Pwd=" + pwd;
            var balanceString = PostManager.Post(serviceUrl + "/balance?", reqstr, Encoding.UTF8);
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(balanceString);
            if (xdoc.GetElementsByTagName("int").Count > 0)
            {
                var ba = xdoc.GetElementsByTagName("int")[0].InnerText;
                return ba;
            }
            else
            {
                return balanceString;
            }
        }
    }
}
