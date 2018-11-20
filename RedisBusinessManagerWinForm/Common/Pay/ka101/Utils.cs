using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ka101
{
    /// <summary>
    /// Utils 的摘要说明
    /// </summary>
    public class Utils
    {
        //日志
        public static void logstr(string orderid, string str, string hmac)
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("Bank_HTMLCommon.log"), true);
                sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                sw.WriteLine(DateTime.Now.ToString() + "[" + orderid + "]" + "[" + str + "]" + "[" + hmac + "]");
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}