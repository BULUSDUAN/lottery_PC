using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common.Mappings;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class SendMsgHistoryRecordInfo
    {
        /// <summary>
        /// 消息主键Id
        /// </summary>
        public Int64 MsgId { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 回执状态
        /// </summary>
        public string MsgResultStatus { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string MsgStatusDesc { get; set; }
        /// <summary>
        /// 消息类型 0:自定义发送；1:发送触屏端地址；2: 发送APP，IOS下载地址；3:发送投注、出票失败相关短信；4：财务类充值提现短信；5:发送验证码、找回密码等；
        /// </summary>
        public  int MsgType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 短信发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 短信平台回复Id
        /// </summary>
        public string SMSId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendNumber { get; set; }
    }
    [CommunicationObject]
    public class SendMsgHistoryRecord_Collection
    {
        public SendMsgHistoryRecord_Collection()
        {
            ListHistoryRecord = new List<SendMsgHistoryRecordInfo>();
        }
        public int TotalCount { get; set; }
        public List<SendMsgHistoryRecordInfo> ListHistoryRecord { get; set; }
    }
    [CommunicationObject]
    public class ResultVeeSingInfo
    {
        public string SubmitResult { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public string smsid { get; set; }
        public string num { get; set; }
    }
}
