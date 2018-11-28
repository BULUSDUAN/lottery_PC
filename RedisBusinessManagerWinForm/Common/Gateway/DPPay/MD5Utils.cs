using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common.Gateway.DPPay
{
    public class MD5Utils
    {
        private MD5Utils()
        {

        }

        /**
         * MessageDigest encrypt default md5
         * 
         * @param inputText
         * @return
         */
        public static String encrypt(String inputText)
        {
            return encrypt(inputText, "md5");
        }

        /**
         * MessageDigest encrypt default md5 , if algorithm null or empty
         * 
         * @param inputText
         * @param algorithmName
         * @return
         */
        public static String encrypt(String inputText, String algorithmName)
        {
            if (inputText == null || inputText.Trim() == "")
            {
                throw new Exception("加密字符窜不可为空");

            }
            if (algorithmName == null || algorithmName.Trim() == "")
            {
                algorithmName = "md5";
            }
            String encryptText = null;

            byte[] result = Encoding.UTF8.GetBytes(inputText.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            result = md5.ComputeHash(result);

            for (int i = 0; i < result.Length; i++)
            {
                encryptText += Convert.ToString(result[i], 16).PadLeft(2, '0');
            }
            return encryptText.PadLeft(32, '0');


        }
    }
}
