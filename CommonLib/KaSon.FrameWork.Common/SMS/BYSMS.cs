
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace KaSon.FrameWork.Common.SMS
{
    public class BYSMS : ISMSSend
    {
        // Fields
        private string password = "";
        private static string sendurl = "http://sms.c8686.com/Api/BayouSmsApiEx.aspx?";
        private string username = "";
        private string config_attach = "";

        // Methods
        public BYSMS(string pid, string pass, string attach)
        {
            this.username = pid;
            this.password = pass;
            config_attach = attach;

        }

        public string GetBalance()
        {
            return "本接口暂不支持查询剩余条数，请至短信网关后台查询！";
        }

        private static string md5(string s)
        {
            byte[] buffer = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(s));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public string SendSMS(string mobile, string content, string attach)
        {
            Encoding e = Encoding.GetEncoding("GB2312");
            content = HttpUtility.UrlEncode(HttpUtility.UrlEncode(content, e), e);
            string requestString = "func=sendsms&username=" + this.username + "&password=" + md5(this.password) + "&mobiles=" + mobile + "&message=" + content + "&cell=&sendtime=";
            string xml = PostManager.Post(sendurl, requestString, Encoding.GetEncoding("GB2312"), 0, null, "application/x-www-form-urlencoded");
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            string innerText = document.SelectSingleNode("response/errorcode").InnerText;
            string str4 = document.SelectSingleNode("response/errordescription").InnerText;
            string text1 = document.SelectSingleNode("response/time").InnerText;
            string text2 = document.SelectSingleNode("response/msgcount").InnerText;
            return string.Format("发送{0},返回消息：{1}", (innerText.ToString() == "0") ? "成功" : "失败", str4);
        }

        public string SendSMS_Batch(string mobiles, string content)
        {
            Encoding e = Encoding.GetEncoding("GB2312");
            content = HttpUtility.UrlEncode(HttpUtility.UrlEncode(content, e), e);
            string requestString = "func=sendsms&username=" + this.username + "&password=" + md5(this.password) + "&mobiles=" + mobiles + "&message=" + content + "&cell=&sendtime=";
            string xml = PostManager.Post(sendurl, requestString, Encoding.GetEncoding("GB2312"), 0, null, "application/x-www-form-urlencoded");
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            string innerText = document.SelectSingleNode("response/errorcode").InnerText;
            string str4 = document.SelectSingleNode("response/errordescription").InnerText;
            string text1 = document.SelectSingleNode("response/time").InnerText;
            string text2 = document.SelectSingleNode("response/msgcount").InnerText;
            return string.Format("发送{0},返回消息：{1}", (innerText.ToString() == "0") ? "成功" : "失败", str4);
        }
    }
}
