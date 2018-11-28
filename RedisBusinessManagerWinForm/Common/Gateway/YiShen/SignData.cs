using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Common.Gateway.YiShen
{
    public class SignData
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="xml">xml数据</param>
        /// <returns>签名数据</returns>
        public static String signData(String xml, string pfxpath, string pfxpassword)
        {
            // X509Certificate2 objx5092 = new X509Certificate2(pfxpath, pfxpassword);   //当前用户存储，本地测试
            X509Certificate2 objx5092 = new X509Certificate2(pfxpath, pfxpassword, X509KeyStorageFlags.MachineKeySet);   //本地存储，服务器测试(windows server2008)要使用这个
            RSACryptoServiceProvider rsa = objx5092.PrivateKey as RSACryptoServiceProvider;
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(xml);
            byte[] hashValue = rsa.SignData(data, "MD5");
            string msg = Convert.ToBase64String(hashValue);
            return msg.PadRight(256);
        }

        /// 
        /// 解签 
        /// </summary>
        /// <param name="signData">解签数据</param>
        /// <returns>是否成功</returns>
        public static bool verifyData(String msg, String check, string cerpath)
        {
            byte[] msgByte = System.Convert.FromBase64String(msg);
            byte[] checkByte = System.Convert.FromBase64String(check);
            X509Certificate2 pub = new X509Certificate2(cerpath);
            RSACryptoServiceProvider rsaPublic = pub.PublicKey.Key as RSACryptoServiceProvider;
            return rsaPublic.VerifyData(msgByte, "MD5", checkByte);
        }
    }
}
