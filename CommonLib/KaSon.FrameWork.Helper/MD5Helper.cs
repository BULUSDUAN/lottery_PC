namespace KaSon.FrameWork.Helper
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
    }
}

