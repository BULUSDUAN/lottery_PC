namespace KaSon.FrameWork.Common
{
    using System;
    using System.Text;

    public static class Base64Helper
    {
        public static string DecodeBase64(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }
            try
            {
                byte[] bytes = Convert.FromBase64String(code);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return code;
            }
        }

        public static string EncodeBase64(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(code));
            }
            catch
            {
                return code;
            }
        }
    }
}

