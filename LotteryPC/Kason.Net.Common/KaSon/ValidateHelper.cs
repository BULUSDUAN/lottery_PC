using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kason.Net.Common
{
    /// <summary>
    /// 校验类，包含各种正则校验。不涉及网络及数据库判断。
    /// </summary>
    public class ValidateHelper
    {
        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="Id">身份证号码</param>
        /// <returns>验证成功为True，否则为False</returns>
        public static bool CheckIdCard(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        #region 身份证号码验证

        /// <summary>
        /// 验证15位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 验证18位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion

        /// <summary>
        /// 是否是手机号码
        /// </summary>
        public static bool IsMobile(string mobile)
        {
            return Regex.IsMatch(mobile, @"^1[34578]\d{9}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否是QQ号码
        /// </summary>
        public static bool IsQQNumber(string qqNumber)
        {
            return Regex.IsMatch(qqNumber, @"^\d{5,11}$", RegexOptions.IgnoreCase);
        }

        ///  <summary>
        ///  判断是否是IP地址格式  0.0.0.0
        ///  </summary>
        ///  <param  name="ipAddress">待判断的IP地址</param>
        ///  <returns>true  or  false</returns>
        public static bool IsIPAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Length < 7 || ipAddress.Length > 15) return false;

            string regexIp = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";
            Regex ipReg = new Regex(regexIp);
            return ipReg.IsMatch(ipAddress);

            //string regexIp = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";
            //string regformat = @"^d{1,3}[.]d{1,3}[.]d{1,3}[.]d{1,3}$";

            //Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            //return regex.IsMatch(ipAddress);
        }

        /// <summary>
        /// 判断输入字符串是否为邮箱格式
        /// </summary>
        public static bool IsEmail(string emailAddress)
        {
            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(emailAddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        ///  <summary>
        ///  判断是否为16-19位银行卡号码
        ///  </summary>
        ///  <param  name="cardNumber">银行卡号码</param>
        ///  <returns>true  or  false</returns>
        public static bool IsBankCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 16 || cardNumber.Length > 19) return false;

            return Regex.IsMatch(cardNumber, @"^\d{16,19}$", RegexOptions.IgnoreCase);
        }



    }

    /// <summary>
    /// 靓号辅助类
    /// </summary>
    public  class BeautyNumberHelper
    {
        /// <summary>
        /// 检查指定的号码是否为靓号
        /// </summary>
        /// <param name="number">号码</param>
        /// <param name="level">返回靓号的级别。号码中最大的连号数</param>
        /// <returns>是否为靓号</returns>
        public static bool CheckIsBeautyNumber(string number, out int level)
        {
            var i = 1;
            level = 1;
            char flag = ' ';
            var result = false;
            return result;
        }
        /// <summary>
        /// 获取指定号码的下一个普通号码（非靓号）
        /// </summary>
        /// <param name="number">号码</param>
        /// <param name="skipList">返回中间跳过的靓号列表</param>
        /// <returns>普通号码</returns>
        public static string GetNextCommonNumber(string number, out IList<string> skipList)
        {
            var num = int.Parse(number);
            skipList = new List<string>();
            while (true)
            {
                num++;
                int level;
                if (!CheckIsBeautyNumber(num.ToString(), out level))
                {
                    return num.ToString();
                }
                else
                {
                    skipList.Add(num.ToString());
                }
            }
        }
    }
}
