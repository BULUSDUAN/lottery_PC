using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace Common.Gateway.Alipay.GeneralPoint
{
    /// <summary>
    /// 加密,排序,签名公用算法类
    /// </summary>
    public class Alipay
    {

        #region 字段
        /// <summary>
        /// 网关地址一
        /// </summary>
        private static string gatewayone;
        /// <summary>
        /// 网关地址二
        /// </summary>
        private static string gatewaytwo;
        /// <summary>
        /// 编码类型
        /// </summary>
        private static string _input_charset;
        /// <summary>
        /// 加密类型
        /// </summary>
        private static string sign_type;
        #endregion
        #region 属性
        /// <summary>
        /// 返回老网关地址
        /// </summary>
        public static string GateWay
        {
            get { return gatewayone; }
        }

        /// <summary>
        /// 返回新网关地址
        /// </summary>
        public static string GateWayN
        {
            get { return gatewaytwo; }
        }
        /// <summary>
        /// 返回编码类型
        /// </summary>
        public static string _Input_Charset
        {
            get { return _input_charset; }
        }

        /// <summary>
        /// 返回加密类型
        /// </summary>
        public static string Sign_Type
        {
            get { return sign_type; }
        }
        #endregion

        /// <summary>
        /// 初始化参数,如果需要更改编码请直接修改_input_charset的默认的赋值
        /// </summary>
        static Alipay()
        {
            gatewayone = "https://www.alipay.com/cooperate/gateway.do?";//支付宝网关一,不用更改(部分接口只有使用网关一)
            gatewaytwo = "https://mapi.alipay.com/gateway.do?";//支付宝网关二,不用更改(部分接口只有使用网关二)
            //gatewaytwo = "http://mapi.alipay.net/gateway.do?";//支付宝联调网关
            _input_charset = "UTF-8";//编码类型,完全根据客户自身的项目的编码格式而定,千万不要填错。否则极其容易造成MD5加密错误。
            sign_type = "MD5"; //加密类型,签名方式"不用更改"
        }

        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        /// <param name="s">待MD5的字符串</param>
        /// <param name="_input_charset">编码</param>
        /// <remarks>返回MD5值</remarks>
        public static string GetMD5(string s, string _input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 冒泡排序法
        /// 按照字母序列从a到z的顺序排列
        /// </summary>
        /// <param name="r">待排序的字符串数组</param>
        /// <remarks>返回排序后的字符串数组</remarks>
        public static string[] BubbleSort(string[] r)
        {
            int i, j; //交换标志 
            string temp;

            bool exchange;

            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假

                for (j = r.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)　//交换条件
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return r;
        }


        /// <summary>
        /// 构成URL参数的sign值
        /// </summary>
        /// <param name="dic">参数的字典集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>返回sign的值</returns>
        /// <remarks>参与加密的参数存入Dictionary，参数的值不允许为空，若该参数为空，不要把这个参数加到Dictionary集合里面</remarks>
        public static string GetSign(Dictionary<string, string> dic, string key)
        {
            //构造数组
            string[] Oristr = new string[dic.Count];
            int ct = 0;
            foreach (KeyValuePair<string, string> entry in dic)
            {
                if (!String.IsNullOrEmpty(entry.Value))
                {
                    Oristr[ct] = entry.Key + "=" + entry.Value;
                    ct++;
                }
            }
            //进行排序
            string[] Sortedstr = Alipay.BubbleSort(Oristr);

            //构造待md5摘要字符串
            System.Text.StringBuilder prestr = new System.Text.StringBuilder();

            for (int i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);
                }
                else
                {
                    prestr.Append(Sortedstr[i] + "&");
                }
            }
            prestr.Append(key);//追加key
            //生成Md5摘要
            return Alipay.GetMD5(prestr.ToString(), _Input_Charset);
        }


        /// <summary>
        /// 构造支付宝带签名的GET方式URL串
        /// </summary>
        /// <param name="dic">参数的字典集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>支付url字符串</returns>
        /// <remarks>URL需要传递的参数存入Dictionary里。参数的值不允许为空，若该参数值为空，不要把这个参数加到Dictionary集合里面</remarks>
        public static string AlipayDoGet(Dictionary<string, string> dic, string key)
        {
            System.Text.StringBuilder parameter = new System.Text.StringBuilder();
            parameter.Append(GateWay);//添加网关
            foreach (KeyValuePair<string, string> entry in dic)
            {
                if (!String.IsNullOrEmpty(entry.Value))
                {
                    parameter.Append(entry.Key + "=" + HttpUtility.UrlEncode(entry.Value) + "&");
                }
            }
            parameter.Append("sign=" + GetSign(dic, key) + "&sign_type=" + Sign_Type);
            return parameter.ToString();
        }


        /// <summary>
        /// 构造支付宝带签名的GET方式URL串
        /// </summary>
        /// <param name="dic">参数的字典集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>支付url字符串</returns>
        /// <remarks>URL需要传递的参数存入Dictionary里。参数的值不允许为空，若该参数值为空，不要把这个参数加到Dictionary集合里面</remarks>
        public static string AlipayDoGetN(Dictionary<string, string> dic, string key)
        {
            System.Text.StringBuilder parameter = new System.Text.StringBuilder();
            parameter.Append(GateWayN);//添加网关
            foreach (KeyValuePair<string, string> entry in dic)
            {
                if (!String.IsNullOrEmpty(entry.Value))
                {
                    parameter.Append(entry.Key + "=" + HttpUtility.UrlEncode(entry.Value) + "&");
                }
            }
            parameter.Append("sign=" + GetSign(dic, key) + "&sign_type=" + Sign_Type);
            return parameter.ToString();
        }

    }
}
