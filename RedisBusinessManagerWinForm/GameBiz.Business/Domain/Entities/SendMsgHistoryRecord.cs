using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class SendMsgHistoryRecord
    {
        /// <summary>
        /// 消息主键Id
        /// </summary>
        public virtual Int64 MsgId { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public virtual string PhoneNumber { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string IP { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public virtual string MsgContent { get; set; }
        /// <summary>
        /// 回执状态
        /// </summary>
        public virtual string MsgResultStatus { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public virtual string MsgStatusDesc { get; set; }
        /// <summary>
        /// 消息类型 1:发送手机网页地址；2 发送APP，IOS下载地址
        /// </summary>
        public virtual int MsgType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 短信发送时间
        /// </summary>
        public virtual DateTime SendTime { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 短信平台回复Id
        /// </summary>
        public virtual string SMSId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public virtual int SendNumber { get; set; }
    }
}
