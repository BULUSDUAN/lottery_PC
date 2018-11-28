using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace Common.Pay.af
{
    /// <summary>
    /// afUtil
    /// </summary>
   public class afUtil
    {
       /// <summary>
       /// af md5编码
       /// </summary>
       /// <param name="strToEncrypt"></param>
       /// <param name="encoding"></param>
       /// <returns></returns>
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
       /// <summary>
       /// 
       /// </summary>
       /// <param name="dic"></param>
       /// <returns></returns>
       public static string GetdictStr(Dictionary<String, String> dic)
       {
           StringBuilder query = new StringBuilder();
           bool hasParam = false;
           foreach (string eachkey in dic.Keys)
           {
               string value = "";
               dic.TryGetValue(eachkey, out value);
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
           return  query.ToString();
       }
       /// <summary>
       /// Base64编码
       /// </summary>
       /// <param name="encode">加密采用的编码方式</param>
       /// <param name="source">待加密的明文</param>
       /// <returns></returns>
       public static string EncodeBase64(Encoding encode, string source)
       {
           string encodeStr = "";
           byte[] bytes = encode.GetBytes(source);
           try
           {
               encodeStr = Convert.ToBase64String(bytes);
           }
           catch
           {
               encodeStr = source;
           }
           return encodeStr;
       }



       /// <summary>
       /// Base64解码
       /// </summary>
       /// <param name="encode">解码采用的编码方式，注意和加密时采用的方式一致</param>
       /// <param name="result">待解码的密文</param>
       /// <returns>解码后的字符串</returns>
       public static string DecodeBase64(Encoding encode, string result)
       {
           string decodeStr = "";
           byte[] bytes = Convert.FromBase64String(result);
           try
           {
               decodeStr = encode.GetString(bytes);
           }
           catch
           {
               decodeStr = result;
           }
           return decodeStr;
       }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="url"></param>
       /// <param name="postdata"></param>
       /// <param name="encoding"></param>
       /// <returns></returns>
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

    }
}
