using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;



namespace Common.Pay.payworth
{
    /// <summary>
    /// 
    /// </summary>
    public class UtilSign
    {

        public static String GetMd5str(Dictionary<String, String> Parms,String KeyStr)
        {
            List<String> SortStr = new List<String>(Parms.Keys);
            SortStr.Sort();
            String Md5Str = CreateLinkstring(Parms, SortStr);
           // Log.LogWrite("Md5拼接字串（不包含密码）：" + Md5Str);

            return CMD5.GetMD5Hash(Md5Str + KeyStr);
        }

        /**
         * 判断值是否为空 FALSE 为不空  TRUE 为空
         * @param Temp
         * @return
         */
        public static bool StrEmpty(String Temp)
        {
            if (null == Temp || String.IsNullOrEmpty(Temp))
            {
                return true;
            }
            return false;
        }

        /**
         * 拼接报文
         * @param Parm
         * @param SortStr
         * @return
         */
        public static String CreateLinkstring(Dictionary<String, String> Parms, List<String> SortStr)
        {
            String LinkStr = "";
            
            for (int i = 0; i < SortStr.Count; i++)
            {

                if (!StrEmpty(Parms[SortStr[i].ToString()]))
                {
                    LinkStr += SortStr[i] + "=" + Parms[SortStr[i].ToString()];
                    if ((i + 1) < SortStr.Count)
                    {
                        LinkStr += "&";
                    }
                }
            }
            return LinkStr;
        }

    }
}