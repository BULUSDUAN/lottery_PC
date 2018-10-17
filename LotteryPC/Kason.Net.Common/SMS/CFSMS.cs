
using Kason.Net.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kason.Net.Common.SMS
{
    public class CFSMS : ISMSSend
    {
        private static string _cdKey;
        private static string _password;
        private static string _attach;
        public CFSMS(string cdKey, string password, string attach)
        {
            _cdKey = cdKey;
            _password = password;
            _attach = attach;
        }

        public string SendSMS(string mobile, string content, string attach)
        {
            string _sendurl = "http://106.ihuyi.cn/webservice/sms.php?method=Submit&";
            var pList = new List<string>();
            pList.Add(string.Format("{0}={1}", "account", _cdKey));
            pList.Add(string.Format("{0}={1}", "password", _password));
            pList.Add(string.Format("{0}={1}", "mobile", mobile));
            pList.Add(string.Format("{0}={1}", "content", content));
            _sendurl += string.Join("&", pList);
            //var writer = Common.Log.LogWriterGetter.GetLogWriter();

            var r = PostManager.Get(_sendurl, Encoding.UTF8).Trim();
            //<?xml version="1.0" encoding="utf-8"?>
            //<SubmitResult xmlns="http://106.ihuyi.cn/">
            //<code>2</code>
            //<msg>提交成功</msg>
            //<smsid>168570682</smsid>
            //</SubmitResult>
            //writer.Write("CF短信记录", "SendSMS", Log.LogType.Error, "_sendurl", _sendurl + Environment.NewLine + r);

            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(r);
                return xml.ChildNodes[1].ChildNodes[0].InnerText;
            }
            catch (Exception ex)
            {
                return r;
            }
        }

        public string SendSMS_Batch(string mobileList, string content)
        {
            throw new NotImplementedException();
        }

        public string GetBalance()
        {
            throw new NotImplementedException();
        }
    }
}
