using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Net.SMS;
using System.Text.RegularExpressions;
using KaSon.FrameWork.Entity.Helper.Net;

namespace Common.Net.SMS
{
    /// <summary>
    /// 短信配置信息
    /// </summary>
    public class SMSConfigInfo
    {
        public string AgentName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Attach { get; set; }

        public static SMSConfigInfo GetConfigByXmlConfig(XmlElement config)
        {
            var agent = config.GetAttribute("Agent");
            switch (agent.ToUpper())
            {
                case "HTSMS":
                case "IOSMS":
                case "BYSMS":
                case "GYSMS":
                case "C123":
                case "WXT":
                case "VeeSing":
                case "YMSMS":
                case "CFSMS":
                    return new SMSConfigInfo
                    {
                        AgentName = agent,
                        UserName = config.SelectSingleNode(agent).Attributes["userId"].Value,
                        Password = config.SelectSingleNode(agent).Attributes["pwd"].Value,
                        Attach = config.SelectSingleNode(agent).Attributes["attach"].Value,
                    };
                default:
                    throw new ArgumentException("不支持的短信代理 - " + agent);
            }

        }
    }
    /// <summary>
    /// 短信发送接口
    /// </summary>
    public interface ISMSSend
    {
        /// <summary>
        /// 发送单条短信
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信手机70个字为一条，小灵通58个字为一条，超过会按两条发送</param>
        /// <returns>发送结果</returns>
        string SendSMS(string mobile, string content, string attach);
        /// <summary>
        /// 批量发送短信
        /// </summary>
        /// <param name="mobile">手机号码，每个手机号码用，隔开，不能超过600个</param>
        /// <param name="content">短信手机70个字为一条，小灵通58个字为一条，超过会按两条发送</param>
        /// <returns>发送结果</returns>
        string SendSMS_Batch(string mobileList, string content);
        /// <summary>
        /// 获取账户短信余额
        /// </summary>
        /// <returns>余额字符串</returns>
        string GetBalance();
    }
    /// <summary>
    /// 短信发送实体
    /// </summary>
    public static class SMSSenderFactory
    {
        public static ISMSSend GetSMSSenderInstance(SMSConfigInfo config)
        {
            switch (config.AgentName.ToUpper())
            {
                case "HTSMS":
                    return new HTSMS(config.UserName, config.Password, config.Attach);
                case "IOSMS":
                    return new IOSMS(config.UserName, config.Password, config.Attach);
                case "BYSMS":
                    return new BYSMS(config.UserName, config.Password, config.Attach);
                case "GYSMS":
                    return new GYSMS(config.UserName, config.Password, config.Attach);
                case "C123":
                    return new C123(config.UserName, config.Password, config.Attach);
                case "WXT":
                    return new WXTSMS(config.UserName, config.Password, config.Attach);
                case "ANDROIDSMS":
                    return new AndroidSMS(config.UserName, config.Password, config.Attach);
                case "VEESING":
                    return new VeeSingSMS(config.UserName, config.Password, config.Attach);
                case "YMSMS":
                    return new YMSMS(config.UserName, config.Password, config.Attach);
                case "CFSMS":
                    return new CFSMS(config.UserName, config.Password, config.Attach);
                default:
                    throw new ArgumentException("不支持的短信代理 - " + config.AgentName);
            }
        }

        /// <summary>
        /// 发短信（公共发送短信）
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        //public static string SendSMS(string mobile, string content)
        //{
        //    try
        //    {
        //        var config = SMSConfigInfo.GetConfigByXmlConfig(SettingConfigAnalyzer.GetConfigElementByKey("SMSAgent"));
        //        var instance = GetSMSSenderInstance(config);
        //        return instance.SendSMS(mobile, content, config.Attach);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        /// <summary>
        /// 获取短信接口余额
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        //public static string GetBalance
        //{
        //    get
        //    {
        //        try
        //        {
        //            var config = SMSConfigInfo.GetConfigByXmlConfig(SettingConfigAnalyzer.GetConfigElementByKey("SMSAgent"));
        //            var instance = GetSMSSenderInstance(config);
        //            return instance.GetBalance();
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //}
    }
}
