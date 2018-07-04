using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.Expansion
{
    public static class StringHelper
    {
        public static string GetObjectString(this object obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString();
        }

        public static string GetNullString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            return str.Trim();
        }

        public static int GetInt32(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return 0;
            return int.Parse(str.Trim());
        }

        public static decimal GetDecimal(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return 0M;
            return decimal.Parse(str.Trim());
        }

        public static DateTime GetDateTime(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return DateTime.Now;
            return DateTime.Parse(str.Trim());
        }

        public static bool GetBoolen(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            return bool.Parse(str.Trim());
        }

    }
}
