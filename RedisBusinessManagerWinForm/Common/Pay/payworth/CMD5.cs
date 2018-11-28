using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Pay.payworth
{
    /// <summary>
    /// 
    /// </summary>
    public class CMD5
    {
        public static String GetMD5Hash(String MD5String)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] MD5Temp = System.Text.Encoding.UTF8.GetBytes(MD5String);
            MD5Temp = x.ComputeHash(MD5Temp);
            System.Text.StringBuilder StrTemp = new System.Text.StringBuilder();
            foreach (byte Res in MD5Temp)
            {
                StrTemp.Append(Res.ToString("x2").ToLower());
            }
            String password = StrTemp.ToString();
            return password;
        }
    }
}
