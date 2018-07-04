using Common.Net.SMS;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KaSon.FrameWork.Entity.Helper.Net
{
    /// <summary>
    /// 无限通(大汉三通)
    /// </summary>
    public class WXTSMS : ISMSSend
    {
        private string config_account = "dh20919";
        private string config_password = "20919.com";
        private string config_attach = "";

        public WXTSMS(string _account, string _password, string attach)
        {
            config_account = _account;
            config_password = _password;
            config_attach = attach;
        }

        public string SendSMS(string mobile, string smscontent, string attach)
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));

            var account = doc.CreateElement("account");
            account.InnerXml = config_account;
            var password = doc.CreateElement("password");
            password.InnerXml = Encipherment.MD5(config_password);
            var msgid = doc.CreateElement("msgid");
            msgid.InnerXml = Guid.NewGuid().ToString("N");
            var phones = doc.CreateElement("phones");
            phones.InnerXml = mobile;
            var content = doc.CreateElement("content");
            content.InnerXml = smscontent;
            var subcode = doc.CreateElement("subcode");
            subcode.InnerXml = "";
            var sendtime = doc.CreateElement("sendtime");
            sendtime.InnerXml = DateTime.Now.ToString("yyyyMMddHHmm");

            var message = doc.CreateElement("message");
            message.AppendChild(account);
            message.AppendChild(password);
            message.AppendChild(msgid);
            message.AppendChild(phones);
            message.AppendChild(content);
            message.AppendChild(subcode);
            message.AppendChild(sendtime);

            doc.AppendChild(message);

            //var url = "http://3tong.net/http/sms/Submit";
            var url = "http://www.10690300.com/http/sms/Submit";
            return PostManager.Post(url, string.Format("message={0}", doc.InnerXml), Encoding.UTF8);
        }

        public string SendSMS_Batch(string mobileList, string content)
        {
            return "在SendSMS中传入多个手机号，以','分开";
        }

        public string GetBalance()
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));

            var account = doc.CreateElement("account");
            account.InnerXml = config_account;
            var password = doc.CreateElement("password");
            password.InnerXml = Encipherment.MD5(config_password);
            var message = doc.CreateElement("message");
            message.AppendChild(account);
            message.AppendChild(password);
            doc.AppendChild(message);

            var url = "http://3tong.net/http/sms/Balance";
            return PostManager.Post(url, string.Format("message={0}", doc.InnerXml), Encoding.UTF8);
        }
    }
}
