
using Kason.Net.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kason.Net.Common.SMS
{
    internal struct BalanceResult
    {
        public int nResult;
        public long dwCorpId;
        public int nStaffId;
        public float fRemain;
    }


    public class C123 : ISMSSend
    {
        private string _url = "http://smsapi.c123.cn/OpenPlatform/OpenApi";
        private string _username = "1001@500742880001";
        private string _password = "0E72F1A1705B1089322AD131B48A76AA";
        private string config_attach = "";

        public C123(string userName, string password, string attach)
        {
            _username = userName;
            _password = password;
            config_attach = attach;
        }

        public string SendSMS(string mobile, string content, string attach)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("action=sendOnce&ac=");
            sb.Append(_username);
            sb.Append("&authkey=");
            sb.Append(_password);
            sb.Append("&cgid=");
            sb.Append("389");
            //if (uCsid > 0)
            //{
            sb.Append("&csid=");
            //sb.Append("62");
            sb.Append(string.IsNullOrEmpty(attach) ? (string.IsNullOrEmpty(config_attach) ? "536" : config_attach) : attach);
            //}
            sb.Append("&m=");
            sb.Append(mobile);
            sb.Append("&c=");
            sb.Append(UrlEncode(content, Encoding.UTF8));

            string sResult = PostManager.Post(_url, sb.ToString(), Encoding.UTF8);
            try
            {
                // 对返回值分析
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sResult);
                XmlElement root = xml.DocumentElement;
                string sRet = root.GetAttribute("result");
                return sRet;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return string.Empty;

            //return parseResult(sResult);
        }

        public string SendSMS_Batch(string mobileList, string content)
        {
            throw new NotImplementedException();
        }

        public string GetBalance()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("action=getBalance&ac=");
            sb.Append(_username);
            sb.Append("&authkey=");
            sb.Append(_password);

            string sResult = PostManager.Post(_url, sb.ToString(), Encoding.UTF8);
            //return parseBalanceResult(sResult);


            BalanceResult ret = new BalanceResult();
            int nRet = parseResult(sResult);
            ret.nResult = nRet;
            if (nRet < 0) return ret.ToString();

            try
            {
                // 对返回值分析
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sResult);
                XmlElement root = xml.DocumentElement;
                if (nRet > 0)
                {
                    XmlNode item = xml.SelectSingleNode("/xml/Item");
                    if (item != null)
                    {
                        ret.dwCorpId = Convert.ToInt64(item.Attributes["cid"].Value);
                        ret.nStaffId = Convert.ToInt32(item.Attributes["sid"].Value);
                        ret.fRemain = (float)Convert.ToDouble(item.Attributes["remain"].Value);
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret.fRemain.ToString();
        }

        private string UrlEncode(string text, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byData = encoding.GetBytes(text);
            for (int i = 0; i < byData.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byData[i], 16));
            }
            return sb.ToString();
        }

        private int parseResult(string sResult)
        {
            if (sResult != null)
            {
                try
                {
                    // 对返回值分析
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(sResult);
                    XmlElement root = xml.DocumentElement;
                    string sRet = root.GetAttribute("result");
                    return Convert.ToInt32(sRet);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return -100;
        }

    }
}
