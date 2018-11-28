using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.Collections.Generic;
using System.Xml;

namespace Common.Pay
{
    public static class HttpHelp
    {

        //商户私钥签名
        public static string RSASign(string signStr, string privateKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters para = new RSAParameters();
                rsa.FromXmlString(privateKey);
                byte[] signBytes = rsa.SignData(UTF8Encoding.UTF8.GetBytes(signStr), "md5");
                return Convert.ToBase64String(signBytes);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //RSA私钥格式转换
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }

        //使用多得宝公钥验签
        public static bool ValidateRsaSign(string plainText, string publicKey, string signedData)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters para = new RSAParameters();
                rsa.FromXmlString(publicKey);
                return rsa.VerifyData(UTF8Encoding.UTF8.GetBytes(plainText), "md5", Convert.FromBase64String(signedData));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //多得宝公钥格式转换
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }

        /// <summary>
        /// post请求到指定地址并获取返回的信息内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求参数</param>
        /// <param name="encodeType">编码类型如：UTF-8</param>
        /// <returns>返回响应内容</returns>
        public static string httppost(string URL, string strPostdata, string strEncoding)
        {
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// post请求到指定地址并获取返回的信息内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求参数</param>
        /// <param name="encodeType">编码类型如：UTF-8</param>
        /// <returns>返回响应内容</returns>
        public static string HttpPost(string POSTURL, string PostData)
        {
            //发送请求的数据
            WebRequest myHttpWebRequest = WebRequest.Create(POSTURL);
            myHttpWebRequest.Method = "POST";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(PostData);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            //发送成功后接收返回的XML信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            return lcHtml;
        }


        /// <summary>
        /// 以GET方式抓取远程页面内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static string Get_Http(string url)
        {
            string strResult;
            try
            {
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);
                hwr.Timeout = 19600;
                HttpWebResponse hwrs = (HttpWebResponse)hwr.GetResponse();
                Stream myStream = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    sb.Append(sr.ReadLine() + "\r\n");
                }
                strResult = sb.ToString();
                hwrs.Close();
            }
            catch (Exception ee)
            {
                strResult = ee.Message;
            }
            return strResult;
        }


        public static String GetSignStr(SortedDictionary<String, String> dic)
        {
            StringBuilder query = new StringBuilder();
            bool hasParam = false;
            foreach (string eachkey in dic.Keys)
            {
                string value = "";
                dic.TryGetValue(eachkey, out value);
                // 除sign、signType字段
                if (!string.IsNullOrEmpty(eachkey) && !string.IsNullOrEmpty(value) && !eachkey.Equals("sign"))
                {
                    if (hasParam)
                    {
                        query.Append("&");
                    }
                    else
                    {
                        hasParam = true;
                    }
                    query.Append(eachkey).Append("=").Append(value);
                }
            }

            string originalstring = query.ToString();

            return originalstring;
        }

        public static string MD5(string strToEncrypt, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(strToEncrypt);
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string encryptStr = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                encryptStr = encryptStr + bytes[i].ToString("x").PadLeft(2, '0');
            }
            return encryptStr.ToLower();
        }

        public static string HttpPost(string url, string postdata, Encoding encoding)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 60 * 1000;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] data = encoding.GetBytes(postdata);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            string response = "";
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                response = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            return response;
        }

        public static SortedDictionary<string, string> ParseXml(string response)
        {
            SortedDictionary<string, string> resData = new SortedDictionary<string, string>();
            XmlDataDocument xmldoc = new XmlDataDocument();
            xmldoc.LoadXml(response);
            XmlNodeList listnode = xmldoc.SelectSingleNode("/root").ChildNodes;

            foreach (XmlNode node in listnode)
            {
                resData.Add(node.Name, node.InnerText);
            }

            return resData;
        }

        public static string GetdictStr(SortedDictionary<String, String> dic)
        {
            StringBuilder query = new StringBuilder();
            bool hasParam = false;
            foreach (string eachkey in dic.Keys)
            {
                string value = "";
                dic.TryGetValue(eachkey, out value);
                // 除sign、signType字段
                if (!string.IsNullOrEmpty(eachkey) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        query.Append("&");
                    }
                    else
                    {
                        hasParam = true;
                    }
                    query.Append(eachkey).Append("=").Append(value);
                }
            }

            string originalstring = query.ToString();

            return originalstring;
        }


        public static string Post_toPage(string url, SortedDictionary<string, string> postParam)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<Html> \n");
            html.Append("    <Head> \n");
            html.Append("        <Body> \n");
            html.Append("            <Form name='SendForm' method='post' action='" + url + "' target='_self'> \n");
            foreach (KeyValuePair<string, string> param in postParam)
            {
                html.AppendFormat("   <input type='Hidden' Name='{0}' value='{1}' /> \n", param.Key, param.Value);
            }
            html.Append("            </Form> \n");
            html.Append("            <script language='javascript'type='text/JavaScript'> \n");
            html.Append("                SendForm.submit(); \n");
            html.Append("            </script> \n");
            html.Append("        </Body> \n");
            html.Append("    </Head> \n");
            html.Append("</Html> \n");
            return html.ToString();
        }


        public static string GetdictStr(Dictionary<String, String> dic)
        {
            StringBuilder query = new StringBuilder();
            bool hasParam = false;
            foreach (string eachkey in dic.Keys)
            {
                string value = "";
                dic.TryGetValue(eachkey, out value);
                // 除sign、signType字段
                if (!string.IsNullOrEmpty(eachkey) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        query.Append("&");
                    }
                    else
                    {
                        hasParam = true;
                    }
                    query.Append(eachkey).Append("=").Append(value);
                }
            }

            string originalstring = query.ToString();

            return originalstring;
        }


        public static string Post_toPage(string url, Dictionary<string, string> postParam)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<Html> \n");
            html.Append("    <Head> \n");
            html.Append("        <Body> \n");
            html.Append("            <Form name='SendForm' method='post' action='" + url + "' target='_self'> \n");
            foreach (KeyValuePair<string, string> param in postParam)
            {
                html.AppendFormat("   <input type='Hidden' Name='{0}' value='{1}' /> \n", param.Key, param.Value);
            }
            html.Append("            </Form> \n");
            html.Append("            <script language='javascript'type='text/JavaScript'> \n");
            html.Append("                SendForm.submit(); \n");
            html.Append("            </script> \n");
            html.Append("        </Body> \n");
            html.Append("    </Head> \n");
            html.Append("</Html> \n");
            return html.ToString();
        }



        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }


        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

    }
}
