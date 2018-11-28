using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Collections;

namespace Common.Gateway.YiJiFu
{
    /// <summary>
    /// 易极付功能函数
    /// </summary>
    /// <remarks>加密,排序,签名公用算法类</remarks>
    public class Functions
    {
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
                sb.Append(t[i].ToString("x2"));
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

        #region 易极付交易加密

        public static string Encrypt(string strPwd)
        {
            //获取加密服务  
            System.Security.Cryptography.MD5CryptoServiceProvider md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //获取要加密的字段，并转化为Byte[]数组  
            byte[] testEncrypt = System.Text.Encoding.Default.GetBytes(strPwd);

            //加密Byte[]数组  
            byte[] resultEncrypt = md5CSP.ComputeHash(testEncrypt);

            //将加密后的数组转化为字段(普通加密)  
            string testResult = System.Text.Encoding.Default.GetString(resultEncrypt);

            //作为密码方式加密   
            string pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strPwd, "MD5");
            return pwd;
        }


        // 格式化md5 hash 字节数组所用的格式（两位小写16进制数字）
        private static readonly string m_strHexFormat = "x2";
        /// <summary>
        /// 使用当前缺省的字符编码对字符串进行加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <returns>用小写字母表示的32位16进制数字字符串</returns>
        public static string md5(string str)
        {
            return (string)md5(str, false, Encoding.UTF8);
        }
        /// <summary>
        /// 对字符串进行md5 hash计算
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <param name="raw_output">
        /// false则返回经过格式化的加密字符串(等同于 string md5(string) )
        /// true则返回原始的md5 hash 长度16 的 byte[] 数组
        /// </param>
        /// <returns>
        /// byte[] 数组或者经过格式化的 string 字符串
        /// </returns>
        public static object md5(string str, bool raw_output)
        {
            return md5(str, raw_output, Encoding.UTF8);
        }
        /// <summary>
        /// 对字符串进行md5 hash计算
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <param name="raw_output">
        /// false则返回经过格式化的加密字符串(等同于 string md5(string) )
        /// true则返回原始的md5 hash 长度16 的 byte[] 数组
        /// </param>
        /// <param name="charEncoder">
        /// 用来指定对输入字符串进行编解码的 Encoding 类型，
        /// 当输入字符串中包含多字节文字（比如中文）的时候
        /// 必须保证进行匹配的 md5 hash 所使用的字符编码相同，
        /// 否则计算出来的 md5 将不匹配。
        /// </param>
        /// <returns>
        /// byte[] 数组或者经过格式化的 string 字符串
        /// </returns>
        public static object md5(string str, bool raw_output,
                                                    Encoding charEncoder)
        {
            if (!raw_output)
                return md5str(str, charEncoder);
            else
                return md5raw(str, charEncoder);
        }

        /// <summary>
        /// 使用当前缺省的字符编码对字符串进行加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <returns>用小写字母表示的32位16进制数字字符串</returns>
        protected static string md5str(string str)
        {
            return md5str(str, Encoding.UTF8);
        }
        /// <summary>
        /// 对字符串进行md5加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <param name="charEncoder">
        /// 指定对输入字符串进行编解码的 Encoding 类型
        /// </param>
        /// <returns>用小写字母表示的32位16进制数字字符串</returns>
        protected static string md5str(string str, Encoding charEncoder)
        {
            byte[] bytesOfStr = md5raw(str, charEncoder);
            int bLen = bytesOfStr.Length;
            StringBuilder pwdBuilder = new StringBuilder(32);
            for (int i = 0; i < bLen; i++)
            {
                pwdBuilder.Append(bytesOfStr[i].ToString(m_strHexFormat));
            }
            return pwdBuilder.ToString();
        }
        /// <summary>
        /// 使用当前缺省的字符编码对字符串进行加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <returns>长度16 的 byte[] 数组</returns>
        protected static byte[] md5raw(string str)
        {
            return md5raw(str, Encoding.UTF8);
        }
        /// <summary>
        /// 对字符串进行md5加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <param name="charEncoder">
        /// 指定对输入字符串进行编解码的 Encoding 类型
        /// </param>
        /// <returns>长度16 的 byte[] 数组</returns>
        protected static byte[] md5raw(string str, Encoding charEncoder)
        {
            System.Security.Cryptography.MD5 md5 =
                System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(charEncoder.GetBytes(str));
        }
        #endregion

        /// <summary>
        /// 构成URL参数的sign值
        /// </summary>
        /// <param name="dic">参数的字典集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>返回sign的值</returns>
        /// <remarks>参与加密的参数存入Dictionary，参数的值不允许为空，若该参数为空，不要把这个参数加到Dictionary集合里面</remarks>
        public static string GetSign(Dictionary<string, string> dic, string key)
        {
            string str = "";
            ArrayList list = new ArrayList();
            foreach (KeyValuePair<string, string> entry in dic)
            {
                if (!String.IsNullOrEmpty(entry.Value))
                {
                    list.Add(entry.Key + "=" + entry.Value);
                }
            }
            list.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    str += list[i].ToString();
                else
                    str += "&" + list[i].ToString();
            }

            return md5(str + key);

            ////构造数组
            //string[] Oristr = new string[dic.Count];
            //int ct = 0;
            //foreach (KeyValuePair<string, string> entry in dic)
            //{
            //    if (!String.IsNullOrEmpty(entry.Value))
            //    {
            //        Oristr[ct] = entry.Key + "=" + entry.Value;
            //        ct++;
            //    }
            //}
            ////进行排序
            //string[] Sortedstr = BubbleSort(Oristr);

            ////构造待md5摘要字符串
            //System.Text.StringBuilder prestr = new System.Text.StringBuilder();

            //for (int i = 0; i < Sortedstr.Length; i++)
            //{
            //    if (i == Sortedstr.Length - 1)
            //    {
            //        prestr.Append(Sortedstr[i]);
            //    }
            //    else
            //    {
            //        prestr.Append(Sortedstr[i] + "&");
            //    }
            //}
            //prestr.Append(key);//追加key
            ////生成Md5摘要
            //return GetMD5(prestr.ToString(), Config.Input_Charset);
        }

        /// <summary>
        /// 构造易极付带签名的get请求字符串
        /// </summary>
        /// <param name="dic">参数的字典集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>业务get请求字符串</returns>
        /// <remarks>URL需要传递的参数存入Dictionary里。参数的值不允许为空，若该参数值为空，不要把这个参数加到Dictionary集合里面</remarks>
        public static string BuildUrl(Dictionary<string, string> dic, string key)
        {
            if (!dic.ContainsKey("signType"))
            {
                dic.Add("signType", Config.SignType);
            }
            System.Text.StringBuilder parameter = new System.Text.StringBuilder();
            parameter.Append(Config.GateWay);//添加网关
            foreach (KeyValuePair<string, string> entry in dic)
            {
                if (!String.IsNullOrEmpty(entry.Value))
                {
                    parameter.Append(entry.Key + "=" + HttpUtility.UrlEncode(entry.Value) + "&");
                }
            }
            parameter.Append("sign=" + GetSign(dic, key));
            return parameter.ToString();
        }

        /// <summary>
        /// 根据返回的通知url生成md5签名字符串
        /// </summary>
        /// <param name="nvc">NameValues集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>返回排序后的字符串（自动剔除末尾的sign）</returns>
        public static string GetSign(System.Collections.Specialized.NameValueCollection nvc, string key)
        {
            string str = "";
            ArrayList list = new ArrayList();
            foreach (var entry in nvc.AllKeys)
            {
                if (nvc.Get(entry) != "" && entry != "sign")
                {
                    list.Add(entry + "=" + nvc.Get(entry));
                }
            }
            list.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    str += list[i].ToString();
                else
                    str += "&" + list[i].ToString();
            }

            return md5(str + key);

            //string[] Sortedstr = BubbleSort(nvc.AllKeys);  //对参数进行排序
            //StringBuilder prestr = new StringBuilder();           //构造待md5摘要字符串 
            //for (int i = 0; i < Sortedstr.Length; i++)
            //{
            //    if (nvc.Get(Sortedstr[i]) != "" && Sortedstr[i] != "sign")
            //    {
            //        if (i == Sortedstr.Length - 1)
            //        {
            //            prestr.Append(Sortedstr[i] + "=" + nvc.Get(Sortedstr[i]));
            //        }
            //        else
            //        {
            //            prestr.Append(Sortedstr[i] + "=" + nvc.Get(Sortedstr[i]) + "&");
            //        }
            //    }
            //}
            //prestr.Append(key);//追加key
            //return prestr.ToString();
        }
    }
}
