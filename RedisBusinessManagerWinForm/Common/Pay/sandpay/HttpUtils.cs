using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Security;

namespace Common.Pay.sandpay
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpUtils
    {
        public static string HttpPost(string postUrl, string paramData, Encoding EncodingName)
        {
            string postDataStr = paramData;
            byte[] buff = EncodingName.GetBytes(postDataStr);

            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (postUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(postUrl) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(buff, 0, buff.Length);
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, EncodingName);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }

}
