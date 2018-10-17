
using Kason.Net.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kason.Net.Common.SMS
{
    /// <summary>
    /// 华唐短信接口
    /// </summary>
    public class HTSMS : ISMSSend
    {
        //发送单条短信接口地址
        private static string sendurl = "http://www.ht3g.com/htWS/Send.aspx?";
        //批量发送短信接口地址
        private static string batchsendurl = "http://www.ht3g.com/htWS/BatchSend.aspx?";
        //查询余额接口
        private static string balanceurl = "http://www.ht3g.com/htWS/SelSum.aspx?";

        //用户账号
        private string corpid = "";
        //用户密码
        private string pwd = "";
        private string config_attach = "";

        /// <summary>
        /// 初始化接口类
        /// </summary>
        /// <param name="pid">用户账号</param>
        /// <param name="password">用户密码</param>
        public HTSMS(string pid, string password, string attach)
        {
            corpid = pid;
            pwd = password;
            config_attach = attach;

        }

        /// <summary>
        /// 发送单条短信-禁止使用本函数发送多条短信
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信手机70个字为一条，小灵通58个字为一条，超过会按两条发送</param>
        /// <returns>发送结果</returns>
        public string SendSMS(string mobile, string content, string attach)
        {
            string requeststr = "corpid=" + corpid + "&pwd=" + pwd + "&Mobile=" + mobile + "&content=" + content + "&cell=" + "&sendtime=";
            string result = PostManager.Post(sendurl, requeststr, Encoding.GetEncoding("GB2312"));
            switch (result)
            {
                case "0":
                    return "发送成功";
                case "-1":
                    return "账号未注册";
                case "-2":
                    return "其他错误";
                case "-3":
                    return "密码错误";
                case "-4":
                    return "手机号码格式错误";
                case "-5":
                    return "余额不足";
                case "-6":
                    return "定时发送时间不是有效的时间格式";
                case "-7":
                    return "发送短信内容包含黑字典关键字";
                default:
                    return "未知错误";
            }
        }

        /// <summary>
        /// 批量发送短信
        /// </summary>
        /// <param name="mobile">手机号码，每个手机号码用，隔开，不能超过600个</param>
        /// <param name="content">短信手机70个字为一条，小灵通58个字为一条，超过会按两条发送</param>
        /// <returns>发送结果</returns>
        public string SendSMS_Batch(string mobile, string content)
        {
            string requeststr = "corpid=" + corpid + "&pwd=" + pwd + "&Mobile=" + mobile + "&content=" + content + "&cell=" + "&sendtime=";
            string result = PostManager.Post(batchsendurl, requeststr, Encoding.GetEncoding("GB2312"));
            switch (result)
            {
                case "0":
                    return "发送成功进入审核阶段";
                case "1":
                    return "发送成功";
                case "-1":
                    return "账号未注册";
                case "-2":
                    return "其他错误";
                case "-3":
                    return "密码错误";
                case "-4":
                    return "一次提交信息不能超过600个手机号码";
                case "-5":
                    return "余额不足";
                case "-6":
                    return "定时发送时间不是有效的时间格式";
                case "-7":
                    return "发送短信内容包含黑字典关键字";
                case "-8":
                    return "发送内容需在3到250个字之间";
                case "-9":
                    return "发送号码为空";
                default:
                    return "未知错误";
            }
        }

        /// <summary>
        /// 获取账户短信余额
        /// </summary>
        /// <returns>余额字符串</returns>
        public string GetBalance()
        {
            string reqstr = "corpid=" + corpid + "&pwd=" + pwd;
            return PostManager.Post(balanceurl, reqstr, Encoding.GetEncoding("GB2312"));
        }
    }
}
