using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Pay.payworth
{
    /// <summary>
    /// 
    /// </summary>
    public class Assist
    {
        public static String GetSerialNumber()
        {
            return "TSNNET" + GetTimeStamp() + RandomString();
        }

        public static String GetOderNo()
        {
            return "TIDNET" + GetTimeStamp() + RandomString();
        }

        public static String GetTimeStamp()   //获取时间戳
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static String RandomString() //生成四位随机数
        {
            Random rad = new Random();
            int value = rad.Next(1000, 10000);
            return value.ToString();
        }
    }
}