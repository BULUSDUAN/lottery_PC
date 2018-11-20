using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 站内信
    /// </summary>
    public class InnerMail
    {
        public virtual string MailId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime SendTime { get; set; }
        public virtual DateTime ActionTime { get; set; }
        public virtual SystemUser Sender { get; set; }
        public virtual string Receivers { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 站内信发送地址
    /// </summary>
    public class InnerMailSendAddress
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual long LId { get; set; }
        public virtual InnerMail Mail { get; set; }
        public virtual InnerMailReceiverType ReceiverType { get; set; }
        public virtual string ReceiverId { get; set; }
    }

    /// <summary>
    /// 站内信阅读
    /// </summary>
    public class InnerMailReadRecord
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual long LId { get; set; }
        public virtual InnerMail Mail { get; set; }
        public virtual SystemUser Receiver { get; set; }
        public virtual InnerMailHandleType HandleType { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 手机短信发送记录
    /// </summary>
    public class MoibleSMSSendRecord
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string SMSContent { get; set; }
        public virtual string SendStatus { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }


    /// <summary>
    /// 站内信息标签
    /// </summary>
    public class SiteMessageTags
    {
        public virtual int Id { get; set; }
        public virtual string TagKey { get; set; }
        public virtual string TagName { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    ///// <summary>
    ///// 站内信息模板
    ///// </summary>
    //public class SiteMessageTemplate
    //{
    //    public virtual int Id { get; set; }
    //    /// <summary>
    //    /// 模板标题
    //    /// </summary>
    //    public virtual string MsgTitle { get; set; }
    //    /// <summary>
    //    /// 模板内容
    //    /// </summary>
    //    public virtual string MsgContent { get; set; }
    //    /// <summary>
    //    /// 模板参数,参数间以|分隔
    //    /// </summary>
    //    public virtual string MsgParams { get; set; }
    //    public virtual DateTime CreateTime { get; set; }
    //}

    /// <summary>
    /// 站内信息场景
    /// </summary>
    public class SiteMessageScene
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 场景Key
        /// </summary>
        public virtual string SceneKey { get; set; }
        /// <summary>
        /// 场景名称
        /// </summary>
        public virtual string SceneName { get; set; }
        /// <summary>
        /// 信息类别
        /// </summary>
        public virtual SiteMessageCategory MsgCategory { get; set; }
        /// <summary>
        /// 消息模板标题
        /// </summary>
        public virtual string MsgTemplateTitle { get; set; }
        /// <summary>
        /// 消息模板内容
        /// </summary>
        public virtual string MsgTemplateContent { get; set; }
        /// <summary>
        /// 消息模板支持的参数(程序主动代入的参数)
        /// </summary>
        public virtual string MsgTemplateParams { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
