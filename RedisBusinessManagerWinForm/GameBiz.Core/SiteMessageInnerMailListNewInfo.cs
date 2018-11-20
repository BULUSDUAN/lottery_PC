using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
   public class SiteMessageInnerMailListNewInfo
    {

        /// <summary>
        /// 站内信编码
        /// </summary>
        public string MailId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
        //public DateTime ActionTime { get; set; }
        /// <summary>
        /// 发送者编号
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        //public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 接收者编号
        /// </summary>
        public string ReceiverId { get; set; }
        /// <summary>
        /// 站内信处理类型
        /// </summary>
        public InnerMailHandleType HandleType { get; set; }
        /// <summary>
        /// 站内信接收者类型
        /// </summary>
        //public InnerMailReceiverType ReceiverType { get; set; }
        /// <summary>
        /// 阅读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }
    }
    [CommunicationObject]
    public class SiteMessageInnerMailListNew_Collection
    {
        public SiteMessageInnerMailListNew_Collection()
        {
            MailList = new List<SiteMessageInnerMailListNewInfo>();
        }
        public int TotalCount { get; set; }
        public List<SiteMessageInnerMailListNewInfo> MailList { get; set; }
    }
}
