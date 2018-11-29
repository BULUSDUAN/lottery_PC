using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.Hk6
{
    /// <summary>
    /// 生肖计算
    /// </summary>
  public  class SXHelper
    {
        private static string  shuxiang()
        {
            string[] shuxiang = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
            string[] shuCode = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            int tmp =DateTime.Now.Year - 2008;
            if (DateTime.Now.Year < 2008)
            {
                // Console.WriteLine(shuxiang[tmp % 12 + 12]);
                return shuCode[tmp % 12 + 12];
            }
            else
            {
                // Console.WriteLine(shuxiang[tmp % 12]);
                return shuCode[tmp % 12];
            }
        }
        private static int SCode(int code)
        {
           int t=int.Parse( shuxiang());
            int scode = 1; 
            if (code > t)
            {
                 scode = (code - t + 1);
            }
            else if (code < t)
            {
                  scode = 12 - t+code+1;
            }
            return scode;
        }

        public static List<string> ScodeArr(int code) {
           int scode= SCode(code);
            List<string> list = new List<string>();
            if (scode >= 10)
            {
                list.Add(scode.ToString());
            }
            else {
                list.Add("0"+scode.ToString());
            }
           
            int temp =scode;
            while (true)
            {
                temp = temp + 12;
                if (temp <= 49)
                {
                    list.Add(temp.ToString());
                }
                else {
                    break;
                }
            }
            temp = scode;
            while (true)
            {
                temp = temp - 12;
                if (temp <=1)
                {
                    if (temp >= 10)
                    {
                        list.Add(temp.ToString());
                    }
                    else {
                        list.Add("0" + temp.ToString());
                    }
                   
                }
                else
                {
                    break;
                }
            }

            return list;

        }
    }
}
