namespace KaSon.FrameWork.Helper
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
   using System.Configuration;

    public static class DESHelper
    {
        public static string Decrypt(string text, string key)
        {
            return Decrypt(text, key, Encoding.UTF8);
        }
        public static string Decrypt(string text)
        {
            return Decrypt(text, _key, Encoding.UTF8);
        }
        private static string _key = "kason"; //ConfigurationSettings.AppSettings["PasswordEnKey"];

        public static string Decrypt(string text, string key, Encoding encoding)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(key));
            provider.Mode = CipherMode.ECB;
            ICryptoTransform transform = provider.CreateDecryptor();
            string str = "";
            try
            {
                byte[] inputBuffer = Convert.FromBase64String(text);
                str = encoding.GetString(transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
            }
            catch (Exception exception)
            {
                throw new Exception("Invalid Key or input string is not a valid base64 string", exception);
            }
            return str;
        }

        public static string Encrypt(string text, string key)
        {
            return Encrypt(text, key, Encoding.UTF8);
        }
        public static string Encrypt(string text)
        {
            return Encrypt(text, _key, Encoding.UTF8);
        }
        public static string Encrypt(string text, string key, Encoding encoding)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(key));
            provider.Mode = CipherMode.ECB;
            ICryptoTransform transform = provider.CreateEncryptor();
            byte[] bytes = encoding.GetBytes(text);
            return Convert.ToBase64String(transform.TransformFinalBlock(bytes, 0, bytes.Length));
        }
    }
}

