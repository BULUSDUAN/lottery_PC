using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using  System.Collections;
namespace KaSon.FrameWork.Common
{
    public static class Encipherment
    {
        public static string MD5(string SourceString)
        {
            return MD5(SourceString, Encoding.Default);
        }

        /// <summary>
        /// 用指定编码得到哈希密码
        /// </summary>
        /// <param name="sourceString">源</param>
        /// <param name="charsetName">gb2312 utf-8等</param>
        /// <returns></returns>
        public static string MD5(string sourceString, Encoding encoding)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(sourceString));
            StringBuilder builder = new StringBuilder(0x20);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="privatekey"></param>
        /// <returns></returns>
        public static string handleMD5(SortedDictionary<string, object> sd, string privatekey)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            StringBuilder sbuffer = new StringBuilder();
            foreach (KeyValuePair<string, object> item in sd)
            {
                sbuffer.Append(item.Value);
            }
            string sign = "";
            sign = MD5(sbuffer.ToString() + privatekey);
            return sign;
        }
      

   

        /// <summary>
        /// CMS MD5
        /// </summary>
        /// <param name="key">加密字串</param>
        /// <returns></returns>
        public static string MD5_CMS(string key)
        {
            string strPwd = "";

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                strPwd = strPwd + s[i].ToString("X");

            }

            return strPwd;
        }

        private static byte[] Keys = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
        private static string key = "lottery_";

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(key);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV),
                    CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        public static byte[] HexStr2ByteArr(string str)
        {
            var iLen = str.Length;
            // 两个字符表示一个字节，所以字节数组长度是字符串长度除以2
            var arrOut = new byte[iLen/2];
            for (int i = 0; i < iLen; i = i + 2)
            {
                var strTmp = str.Substring(i, 2);
                arrOut[i/2] = (byte) int.Parse(strTmp);
            }
            return arrOut;
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(key);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV),
                    CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        public static string EncryptDES(string str, byte[] key, CipherMode mode)
        {
            //将传递的字符创转化为字节数组
            byte[] iText = Encoding.UTF8.GetBytes(str);

            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            dCSP.Mode = mode;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, dCSP.CreateEncryptor(key, key), CryptoStreamMode.Write))
                {
                    cs.Write(iText, 0, iText.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string DecryptDES(string str, byte[] key, CipherMode mode)
        {
            //将传递的字符创转化为字节数组
            byte[] iText = Convert.FromBase64String(str);

            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            dCSP.Mode = mode;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, dCSP.CreateDecryptor(key, key), CryptoStreamMode.Write))
                {
                    cs.Write(iText, 0, iText.Length);
                    cs.FlushFinalBlock();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        //public static string handleMD5(Dictionary<string, object> map, string md5key)
        //{
          

        //}
    }
}
