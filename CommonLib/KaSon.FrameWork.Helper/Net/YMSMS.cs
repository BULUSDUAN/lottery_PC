using Common.Net.SMS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KaSon.FrameWork.Helper.Net
{
    /// <summary>
    /// 亿美短信
    /// </summary>
    public class YMSMS : ISMSSend
    {
        private static string _cdKey;
        private static string _password;
        private static string _attach;

        public YMSMS(string cdKey, string password, string attach)
        {
            _cdKey = cdKey;
            _password = password;
            _attach = attach;
        }

        public string SendSMS(string mobile, string content, string attach)
        {
            string _sendurl = "http://sdk4report.eucp.b2m.cn:8080/sdkproxy/sendsms.action?";
            var pList = new List<string>();
            pList.Add(string.Format("{0}={1}", "cdkey", _cdKey));
            pList.Add(string.Format("{0}={1}", "password", _password));
            pList.Add(string.Format("{0}={1}", "phone", mobile));
            pList.Add(string.Format("{0}={1}", "message", content));
            pList.Add(string.Format("{0}={1}", "addserial", attach));
            _sendurl += string.Join("&", pList);
            var r = PostManager.Get(_sendurl, Encoding.UTF8).Trim();
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(r);
                var respnseNode = xml.SelectSingleNode("response");
                if (respnseNode == null)
                    throw new Exception("返回结果中找不到节点response");
                var errorNode = respnseNode.SelectSingleNode("error");
                if (errorNode == null)
                    throw new Exception("返回结果中找不到节点error");
                //结果：0 为发送成功，其它失败
                return errorNode.InnerText;
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
