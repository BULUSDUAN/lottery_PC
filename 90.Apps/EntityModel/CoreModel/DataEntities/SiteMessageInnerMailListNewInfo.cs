using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    public class SiteMessageInnerMailListNewInfo
    {

        /// <summary>
        /// 站内信编码
        /// </summary>
        /// 
        [ProtoMember(1)]
        public string MailId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        /// 
        [ProtoMember(2)]
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        /// 
        [ProtoMember(3)]
        public string MsgContent { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        /// 
        [ProtoMember(4)]
        public DateTime SendTime { get; set; }
        //public DateTime ActionTime { get; set; }
        /// <summary>
        /// 发送者编号
        /// </summary>
        /// 
        [ProtoMember(5)]
        public string SenderId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        //public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 接收者编号
        /// </summary>
        /// 
        [ProtoMember(6)]
        public string ReceiverId { get; set; }
        /// <summary>
        /// 站内信处理类型
        /// </summary>
        /// 
        [ProtoMember(7)]
        public InnerMailHandleType HandleType { get; set; }
        /// <summary>
        /// 站内信接收者类型
        /// </summary>
        //public InnerMailReceiverType ReceiverType { get; set; }
        /// <summary>
        /// 阅读时间
        /// </summary>
        /// 
        [ProtoMember(8)]
        public DateTime? ReadTime { get; set; }
    }
    [ProtoContract]
    public class SiteMessageInnerMailListNew_Collection
    {
        public SiteMessageInnerMailListNew_Collection()
        {
            MailList = new List<SiteMessageInnerMailListNewInfo>();
        }
        [ProtoMember(1)]
        public int TotalCount { get; set; }
        [ProtoMember(2)]
        public List<SiteMessageInnerMailListNewInfo> MailList { get; set; }
    }
}
