using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net.SMS;
using WebSocket4Net;
using Common.JSON;

namespace Common.Net
{
    /// <summary>
    /// 利用安卓发送短信
    /// </summary>
    public class AndroidSMS : ISMSSend
    {
        private string config_account = "ws://192.168.0.180:8889";
        private string config_password = "";
        private string config_attach = "";

        public AndroidSMS(string _account, string _password, string attach)
        {
            config_account = _account;
            config_password = _password;
            config_attach = attach;
        }

        public string SendSMS(string mobile, string content, string attach)
        {
            var webSocket = new WebSocket(config_account);
            webSocket.Opened += (object sender, EventArgs e) =>
            {
                //发送消息
                var msg = new AndroidSMSMessageInfo
                {
                    ClientCategory = AndroidSMSClientCategory.Web,
                    MessageCategory = AndroidSMSMessageCategory.SendSMS,
                    MessageJson = JsonSerializer.Serialize(new AndroidSMSSendInfo
                    {
                        Content = content,
                        MobileArray = mobile,
                    }),
                };
                webSocket.Send(JsonSerializer.Serialize(msg));

                webSocket.Close();
            };
            webSocket.Open();
            return string.Empty;
        }

        public string SendSMS_Batch(string mobileList, string content)
        {
            return SendSMS(mobileList, content, string.Empty);
        }

        public string GetBalance()
        {
            return string.Empty;
        }
    }


    /// <summary>
    /// 消息对象
    /// </summary>
    public class AndroidSMSMessageInfo
    {
        /// <summary>
        /// 消息类别
        /// </summary>
        public AndroidSMSMessageCategory MessageCategory { get; set; }
        /// <summary>
        /// 客户端类别
        /// </summary>
        public AndroidSMSClientCategory ClientCategory { get; set; }
        /// <summary>
        /// 消息json
        /// </summary>
        public string MessageJson { get; set; }
    }

    /// <summary>
    /// 状态消息
    /// </summary>
    public class AndroidSMSResponseStatusInfo
    {
        public AndroidSMSClientStatus StatusCode { get; set; }
        public string Msg { get; set; }
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    public class AndroidSMSResponseInfo
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// 回复代码
        /// </summary>
        public string ResponseCode { get; set; }
        /// <summary>
        /// 回复消息
        /// </summary>
        public string ResponseSMS { get; set; }
        /// <summary>
        /// 会话id
        /// </summary>
        public string ResponseSessionId { get; set; }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public class AndroidSMSSendInfo
    {
        /// <summary>
        /// 手机号 以 “,”分隔
        /// </summary>
        public string MobileArray { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// 发送消息到手机客户端
    /// </summary>
    public class AndroidSMSSendSMSToMobileInfo
    {
        /// <summary>
        /// 会话id
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// 手机号数组
        /// </summary>
        public string[] MobileList { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// 消息类别
    /// </summary>
    public enum AndroidSMSMessageCategory
    {
        /// <summary>
        /// 连接
        /// </summary>
        Connect = 0,
        /// <summary>
        /// 发送短信
        /// </summary>
        SendSMS = 1,
        /// <summary>
        /// 回复短信结果
        /// </summary>
        ResponseSMS = 2,
        /// <summary>
        /// 回复状态
        /// </summary>
        ResponseStatus = 3,
    }

    /// <summary>
    /// 客户端类别
    /// </summary>
    public enum AndroidSMSClientCategory
    {
        /// <summary>
        /// 网页
        /// </summary>
        Web = 0,
        /// <summary>
        /// 手机
        /// </summary>
        Mobile = 1,
    }

    /// <summary>
    /// 客户端状态
    /// </summary>
    public enum AndroidSMSClientStatus
    {
        /// <summary>
        /// 繁忙
        /// </summary>
        Busy = 10,
        /// <summary>
        /// 空闲
        /// </summary>
        Free = 20,
    }
}
