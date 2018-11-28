using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace Common.Gateway.YinBao
{
    /// <summary>
    ///通知信息封装类
    /// </summary>
    public class YinBaoNotify
    {
        /// <summary>
        /// 构造函数
        /// 从配置文件中初始化变量
        /// </summary>
        /// <param name="key">安全校验码，与partner是一组的</param>
        public YinBaoNotify(string key)
        {
            Config.Key = key;
        }

        /// <summary>
        /// 将返回的URL生成Md5摘要
        /// </summary>
        /// <param name="coll">NameValues集合，通过Request.QueryString或Request.Form方式取得</param>
        /// <returns>返回sign串</returns>
        public string GetMD5Sign(System.Collections.Specialized.NameValueCollection coll)
        {
            return YinBao.GetMD5(BuildSignString(coll, Config.Key), YinBao._Input_Charset);
        }

        /// <summary>
        /// 构造md5签名字符串
        /// </summary>
        /// <param name="nvc">NameValues集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>返回排序后的字符串（自动剔除末尾的sign和sign_type类型）</returns>
        private string BuildSignString(System.Collections.Specialized.NameValueCollection nvc, string key)
        {
            string[] Sortedstr = YinBao.BubbleSort(nvc.AllKeys);  //对参数进行排序
            StringBuilder prestr = new StringBuilder();           //构造待md5摘要字符串 
            for (int i = 0; i < Sortedstr.Length; i++)
            {
                if (nvc.Get(Sortedstr[i]) != "" && Sortedstr[i] != "sign")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i] + "=" + nvc.Get(Sortedstr[i]));
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "=" + nvc.Get(Sortedstr[i]) + "&");
                    }
                }
            }
            prestr.Append(key);//追加key
            return prestr.ToString();
        }
    }
}
