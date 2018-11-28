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
    public class MessageWorker
    {
        string loggerHeader = "MessageWorker_";
        public struct trafficMessage
        {
            public string charset;  //合作商户ID 杉德系统分配，唯一标识
            public string signType;// 加密后的AES秘钥 公钥加密(RSA/ECB/PKCS1Padding)，加密结果采用base64编码
            public string data; //加密后的请求/应答报文 AES加密(AES/ECB/PKCS5Padding)，加密结果采用base64编码
            public string sign;//    签名 对encryptData对应的明文进行签名(SHA1WithRSA)，签名结果采用base64编码
            public string extend;//  扩展域 暂时不用
        }

        private Encoding encodeCode = Encoding.UTF8;


        private string pfxFilePath = string.Empty;
        public string PFXFile
        {
            get
            {
                return pfxFilePath;
            }
            set
            {
                pfxFilePath = value;
            }
        }

        private string pfxPassword = string.Empty;
        public string PFXPassword
        {
            get
            {
                return pfxPassword;
            }
            set
            {
                pfxPassword = value;
            }
        }


        private string cerFilePath = string.Empty;
        public string CerFile
        {
            get
            {
                return cerFilePath;
            }
            set
            {
                cerFilePath = value;
            }
        }
        private trafficMessage UrlDecodeMessage(string msgResponse)
        {
            trafficMessage msgEncrypt = new trafficMessage();
            string[] EncryptBody = System.Web.HttpUtility.UrlDecode(msgResponse).Split('&');
            for (int i = 0; i < EncryptBody.Length; i++)
            {
                string[] tmp = EncryptBody[i].Split('=');
                switch (tmp[0])
                {
                    //需要添加引用System.Web，用于url转码，处理base64产生的+/=
                    case "charset": msgEncrypt.charset = EncryptBody[i].Replace("charset=", "").Trim('"'); break;
                    case "signType": msgEncrypt.signType = EncryptBody[i].Replace("signType=", "").Trim('"'); break;
                    case "data": msgEncrypt.data = EncryptBody[i].Replace("data=", "").Trim('"'); break;
                    case "sign": msgEncrypt.sign = EncryptBody[i].Replace("sign=", "").Trim('"'); break;
                    case "extend": msgEncrypt.extend = EncryptBody[i].Replace("extend=", "").Trim('"'); break;
                }
            }
            return msgEncrypt;
        }
        private string UrlEncodeMessage(trafficMessage msgRequest)
        {
            //需要添加引用System.Web，用于url转码，处理base64产生的+/=
            return "charset=" + System.Web.HttpUtility.UrlEncode(msgRequest.charset) + "&" +
                 "signType=" + System.Web.HttpUtility.UrlEncode(msgRequest.signType) + "&" +
                  "data=" + System.Web.HttpUtility.UrlEncode(msgRequest.data) + "&" +
                   "sign=" + System.Web.HttpUtility.UrlEncode(msgRequest.sign) + "&" +
                   "extend=" + System.Web.HttpUtility.UrlEncode(msgRequest.extend);
        }



        private trafficMessage SignMessageBeforePost(trafficMessage msgSource)
        {
            trafficMessage msgEncrypt = new trafficMessage();


            //获取报文字符集
            this.encodeCode = Encoding.GetEncoding(msgSource.charset);
            msgEncrypt.charset = msgSource.charset;
            msgEncrypt.signType = msgSource.signType;
            msgEncrypt.extend = msgSource.extend;
            msgEncrypt.data = msgSource.data;

            //报文签名
            msgEncrypt.sign = CryptUtils.Base64Encoder(CryptUtils.CreateSignWithPrivateKey(CryptUtils.getBytesFromString(msgSource.data, encodeCode),
                CryptUtils.getPrivateKeyXmlFromPFX(pfxFilePath, pfxPassword)));
            return msgEncrypt;

        }
        private trafficMessage CheckSignMessageAfterResponse(trafficMessage msgEncrypt)
        {
            trafficMessage msgSource = new trafficMessage();

            //获取报文字符集
            this.encodeCode = Encoding.GetEncoding(msgEncrypt.charset);
            msgSource.charset = msgEncrypt.charset;
            msgSource.signType = msgEncrypt.signType;
            msgSource.extend = msgEncrypt.extend;
            msgSource.data = msgEncrypt.data;

            msgSource.sign = CryptUtils.VerifySignWithPublicKey(
                                (CryptUtils.getBytesFromString(msgEncrypt.data, encodeCode)),
                                CryptUtils.getPublicKeyXmlFromCer(cerFilePath),
                                CryptUtils.Base64Decoder(msgEncrypt.sign)
                                ).ToString();
            return msgSource;
        }


        public trafficMessage postMessage(string serverUrl, trafficMessage requestSourceMessage)
        {
            trafficMessage responseMessage = new trafficMessage();
            try
            {
                string requestString = UrlEncodeMessage(SignMessageBeforePost(requestSourceMessage));
                string responseString = HttpUtils.HttpPost(serverUrl, requestString, encodeCode);
                responseMessage = CheckSignMessageAfterResponse(UrlDecodeMessage(responseString));
            }
            catch (Exception er)
            {
                Console.WriteLine(er.ToString());
            }
            return responseMessage;
        }
    }

}
