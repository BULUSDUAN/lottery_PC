using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace Common.Pay.tianfubao
{
    public class Util
    {
        const int Max_Block = 117;
        const int Res_Max_Block = 128;
        public byte[] EncryptData(string xmlPublicKey, byte[] data, Encoding encoding)
        {
            try
            {
                byte[] CypherTextBArray;
                byte[] Result;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPublicKey);

                int Len = data.Length;
                int offset = 0;
                List<byte> cache = new List<byte>();
                while (Len - offset > 0)
                {
                    byte[] PlainTextBArray = new byte[Max_Block];
                    if (Len - offset > Max_Block)
                    {
                        Array.Copy(data, offset, PlainTextBArray, 0, Max_Block);
                    }
                    else
                    {
                        Array.Copy(data, offset, PlainTextBArray, 0, Len - offset);
                    }
                    offset += Max_Block;
                    CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
                    cache.AddRange(CypherTextBArray);
                }
                Result = cache.ToArray();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RSADecrypt(string xmlPrivateKey, byte[] data, Encoding encoding)
        {
            try
            {
                byte[] DypherTextBArray;
                string Result;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPrivateKey);

                int Len = data.Length;
                int offset = 0;
                List<byte> cache = new List<byte>();
                while (Len - offset > 0)
                {
                    byte[] PlainTextBArray = new byte[Res_Max_Block];
                    if (Len - offset > Res_Max_Block)
                    {
                        Array.Copy(data, offset, PlainTextBArray, 0, Res_Max_Block);
                    }
                    else
                    {
                        Array.Copy(data, offset, PlainTextBArray, 0, Len - offset);
                    }
                    offset += Res_Max_Block;
                    DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
                    cache.AddRange(DypherTextBArray);
                }

                Result = encoding.GetString(cache.ToArray());
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
    }
}
