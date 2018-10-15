
using System;
using System.Collections.Generic;
using System.Text;

namespace Kason.Net.Common.SMS
{
    /// <summary>
    /// 国宇短信
    /// </summary>
    public class GYSMS : ISMSSend
    {
        private string _url = "http://www.gysoft.cn/smspost_utf8/send.aspx";
        private string _username = "";
        private string _password = "";
        private string config_attach = "";

        public GYSMS(string userName, string password, string attach)
        {
            _username = userName;
            _password = password;
            config_attach = attach;

        }

        public string SendSMS(string mobile, string content, string attach)
        {
            // 初始化WebClient 
            var webClient = new System.Net.WebClient();
            webClient.Headers.Add("Accept", "*/*");
            webClient.Headers.Add("Accept-Language", "zh-cn");
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            try
            {
                //string sendContent = "username=xinti&password=898440&mobile=" + phoneNums + "&content=新体网短信测试222";
                content = string.Format("username={0}&password={1}&mobile={2}&content={3}", _username, _password, mobile, content);
                byte[] responseData = webClient.UploadData(_url, "POST", Encoding.GetEncoding("UTF-8").GetBytes(content));
                return Encoding.GetEncoding("UTF-8").GetString(responseData);
            }
            catch
            {
                return "-1";
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
