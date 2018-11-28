using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.IO;
using System.Net;


namespace Common.Pay.mobao
{
    /// <summary>
    /// mobaoUtil类
    /// </summary>
    public static class mobaoUtil
    {
        private static Encoding _gbk = Encoding.GetEncoding("GBK");
        /// <summary>
        /// 
        /// </summary>
        public static Encoding GBK
        {
            get { return _gbk; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HexString(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GetMD5(byte[] data)
        {
            var md5 = MD5CryptoServiceProvider.Create();
            var md5hash = md5.ComputeHash(data);
            return md5hash;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5String(string str)
        {
            var data = _gbk.GetBytes(str);
            return HexString(GetMD5(data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static string EncodeTransMap(Dictionary<string, string> map)
        {
            var sb = new StringBuilder();
            foreach (var de in map)
            {
                sb.AppendFormat("{0}={1}&", de.Key, de.Value);
            }
            if (map.Count > 0)
            {
                // Remove last & char
                sb.Length--;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] EncryptTransData(string transStr, string key)
        {
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Mode = CipherMode.ECB;
                rijndael.Padding = PaddingMode.PKCS7;
                // Random IV was already generated in ctor
                //rijndael.GenerateIV();
                rijndael.Key = _gbk.GetBytes(key);
                var encryptor = rijndael.CreateEncryptor();
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        var data = _gbk.GetBytes(transStr);
                        csEncrypt.Write(data, 0, data.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static string UrlEncodeTransMap(Dictionary<string, string> map)
        {
            var sb = new StringBuilder();
            foreach (var de in map)
            {
                sb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(de.Key), HttpUtility.UrlEncode(de.Value));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transDataMap"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string PostTransDataMap(Dictionary<string, string> transDataMap, string url)
        {
            var transDataStr = mobaoUtil.EncodeTransMap(transDataMap);
            var transData = mobaoUtil.GBK.GetBytes(transDataStr);
            //transData[transData.Length - 1] = 12;
            var httpReq = WebRequest.Create(url) as HttpWebRequest;
            httpReq.Timeout = 60 * 1000;
            httpReq.Method = "POST";
            httpReq.Headers[HttpRequestHeader.ContentEncoding] = "GBK";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            using (var st = httpReq.GetRequestStream())
            {
                st.Write(transData, 0, transData.Length);
            }
            var httpResp = httpReq.GetResponse() as HttpWebResponse;
            string resBody = null;
            using (var st = httpResp.GetResponseStream())
            {
                using (var r = new StreamReader(st, mobaoUtil.GBK))
                {
                    resBody = r.ReadToEnd();
                    return resBody;
                }
            }
        }
    }
}
