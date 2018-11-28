using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net.SMS;
using System.Net;
using Common.Cryptography;
using System.IO;
using System.Web.Script.Serialization;
using Common.JSON;

namespace Common.Net
{
    /// <summary>
    /// 荣联短信、语音
    /// </summary>
    public class RLSMS
    {
        private string config_sid = "aaf98fda438f75e4014396b2408802d6";
        private string config_token = "254a822fc36a4cc69e24b222b804c678";
        private string config_appid = "8a48b5514767145d0147673b47e90018";

        public RLSMS(string _sid, string _token, string _appid)
        {
            if (!string.IsNullOrEmpty(_sid))
                config_sid = _sid;
            if (!string.IsNullOrEmpty(_token))
                config_token = _token;
            if (!string.IsNullOrEmpty(_appid))
                config_appid = _appid;
        }

        public string SendSMS(string templateId, string mobile, List<string> datas)
        {
            var json = new
            {
                to = mobile,
                appId = config_appid,
                templateId = templateId,
                datas = datas,
            };
            var requestString = JsonHelper.Serialize(json);

            var sid = config_sid;
            var token = config_token;
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sig = Encipherment.MD5(string.Format("{0}{1}{2}", sid, token, time));
            var url = string.Format("https://app.cloopen.com:8883/2013-12-26/Accounts/{0}/SMS/TemplateSMS?sig={1}", sid, sig);// + sig;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            setCertificateValidationCallBack();

            byte[] myByte = Encoding.UTF8.GetBytes(sid + ":" + time);
            string authStr = Convert.ToBase64String(myByte);
            request.Headers.Add("Authorization", authStr);
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.Accept = "application/json";
            request.ContentType = "application/json;charset=utf-8";
            requestString = System.Net.WebUtility.HtmlDecode(requestString);
            byte[] bytes = Encoding.UTF8.GetBytes(requestString);
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            return result;
        }

        public string SendVoice(string mobile, string code)
        {
            var json = new
            {
                to = mobile,
                appId = config_appid,
                verifyCode = code,
                //displayNum = "400-033-0997",
                playTimes = "3"
            };
            var requestString = JsonHelper.Serialize(json);

            var sid = config_sid;
            var token = config_token;
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sig = Encipherment.MD5(string.Format("{0}{1}{2}", sid, token, time));
            var url = string.Format("https://app.cloopen.com:8883/2013-12-26/Accounts/{0}/Calls/VoiceVerify?sig={1}", sid, sig);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            setCertificateValidationCallBack();

            byte[] myByte = Encoding.UTF8.GetBytes(sid + ":" + time);
            string authStr = Convert.ToBase64String(myByte);
            request.Headers.Add("Authorization", authStr);
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.Accept = "application/json";
            request.ContentType = "application/json;charset=utf-8";
            requestString = System.Net.WebUtility.HtmlDecode(requestString);
            byte[] bytes = Encoding.UTF8.GetBytes(requestString);
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            return result;
        }

        public string SendSMS_Batch(string mobileList, string content)
        {
            throw new NotImplementedException();
        }

        public string GetBalance()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置服务器证书验证回调
        /// </summary>
        private void setCertificateValidationCallBack()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidationResult;
        }

        /// <summary>
        ///  证书验证回调函数  
        /// </summary>
        private bool CertificateValidationResult(object obj, System.Security.Cryptography.X509Certificates.X509Certificate cer, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }
    }
}
