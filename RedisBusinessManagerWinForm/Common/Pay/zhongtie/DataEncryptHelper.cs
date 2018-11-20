using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace FancyOne.Payment
{
    public static class DataEncryptHelper
    {
        public static string Encode(string data)
        {
            return System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(data)));
            //return System.Web.HttpUtility.UrlEncode(EncodeHelper.Base64Encode(Encoding.UTF8.GetBytes(data)));
        }

        public static string Decode(string data)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(data)));
        }
    }
}
