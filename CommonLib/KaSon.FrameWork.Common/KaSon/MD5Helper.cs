namespace KaSon.FrameWork.Common
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class MD5Helper
    {
        public static string Encrypt(string text)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(provider.ComputeHash(bytes));
        }
        public static string MD5(string SourceString)
        {
            return MD5(SourceString, Encoding.Default);
        }
        public static string MD5(string sourceString, Encoding encoding)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(sourceString));
            StringBuilder builder = new StringBuilder(0x20);
            for (int i = 0; i < buffer.Length; i = (int)(i + 1))
            {
                builder.Append(((byte)buffer[i]).ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString();
        }

        public static string UpperMD5(string encypStr, string charset = "UTF-8")
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception ex)
            {
                inputBye = Encoding.GetEncoding("utf-8").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        public static string LowerMD5(string encypStr, string charset = "UTF-8")
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception ex)
            {
                inputBye = Encoding.GetEncoding("utf-8").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToLower();
            return retStr;
        }
    }
}

