using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class SiteMessageInnerMailListNew
    {
        /// <summary>
        /// 站内信编码
        /// </summary>
        public virtual string MailId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string MsgContent { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public virtual DateTime SendTime { get; set; }
        //public virtual DateTime ActionTime { get; set; }
        /// <summary>
        /// 发送者编号
        /// </summary>
        public virtual string SenderId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        //public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 接收者编号
        /// </summary>
        public virtual string ReceiverId { get; set; }
        /// <summary>
        /// 站内信处理类型
        /// </summary>
        public virtual InnerMailHandleType HandleType { get; set; }
        /// <summary>
        /// 站内信接收者类型
        /// </summary>
        //public virtual InnerMailReceiverType ReceiverType { get; set; }
        /// <summary>
        /// 阅读时间
        /// </summary>
        public virtual DateTime? ReadTime { get; set; }
    }
}

