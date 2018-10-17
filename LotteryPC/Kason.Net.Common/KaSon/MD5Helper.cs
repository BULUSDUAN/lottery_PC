namespace Kason.Net.Common
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
    }
}

