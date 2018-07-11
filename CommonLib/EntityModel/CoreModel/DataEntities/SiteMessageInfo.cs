using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using EntityModel.Enum;
using ProtoBuf;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 站点消息异常
    /// </summary>
    public class SiteMessageException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        public SiteMessageException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">引起此异常的异常</param>
        public SiteMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 发送站内信的信息对象
    /// </summary>
    public class InnerMailInfo_Send
    {
        /// <summary>
        /// 站内信标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 站内信内容。可以是HTML
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 站内信生效时间。如为null表示任何时候注册的用户（包括以后注册的用户），只要满足条件，都能收到
        /// </summary>
        public DateTime? ActionTime { get; set; }
        /// <summary>
        /// 站内信接收者。多个接收者之间以"|"符号分隔。每个接收者的格式：ALL 或者 R:Admin 或者 U:atest
        /// </summary>
        public string Receivers { get; set; }
    }
    /// <summary>
    /// 查询站内信的对象
    /// </summary>
    /// 
    [ProtoContract]
    [Serializable]
    public class InnerMailInfo_Query
    {
        /// <summary>
        /// 邮件编号
        /// </summary>
        /// 
        [ProtoMember(1)]
        public string MailId { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        /// 
        [ProtoMember(2)]
        public string Title { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        /// 
        [ProtoMember(3)]
        public string Content { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        /// 
        [ProtoMember(4)]
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        /// 
        [ProtoMember(5)]
        public DateTime ActionTime { get; set; }
        /// <summary>
        /// 发送人名称
        /// </summary>
        /// 
        [ProtoMember(6)]
        public string SenderId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        /// 
        [ProtoMember(7)]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 处理类型
        /// </summary>
        /// 
        [ProtoMember(8)]
        public InnerMailHandleType HandleType { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length != 7)
            {
                throw new ArgumentException("转换成此InnerMailInfo_Query对象的数据数组长度必须是7，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
            MailId = (string)dataArray[0];
            Title = (string)dataArray[1];
            SendTime = (DateTime)dataArray[2];
            ActionTime = (DateTime)dataArray[3];
            SenderId = (string)dataArray[4];
            UpdateTime = (DateTime)dataArray[5];
            if (dataArray[6] == null || dataArray[6] == DBNull.Value)
            {
                HandleType = InnerMailHandleType.UnRead;
            }
            else
            {
                HandleType = (InnerMailHandleType)dataArray[6];
            }
        }
    }
    /// <summary>
    /// 查询站内信的对象列表
    /// </summary>
    public class InnerMailInfo_QueryCollection
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InnerMailInfo_QueryCollection()
        {
            InnerMailList = new List<InnerMailInfo_Query>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 站内信列表
        /// </summary>
        public List<InnerMailInfo_Query> InnerMailList { get; set; }
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new InnerMailInfo_Query();
                info.LoadArray(item);
                InnerMailList.Add(info);
            }
        }
    }

    /// <summary>
    /// 网站通知配置
    /// </summary>
    public class SiteMessageSceneInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 场景Key
        /// </summary>
        public string SceneKey { get; set; }
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName { get; set; }
        /// <summary>
        /// 信息类别
        /// </summary>
        public SiteMessageCategory MsgCategory { get; set; }
        /// <summary>
        /// 消息模板标题
        /// </summary>
        public string MsgTemplateTitle { get; set; }
        /// <summary>
        /// 消息模板内容
        /// </summary>
        public string MsgTemplateContent { get; set; }
        /// <summary>
        /// 消息模板支持的参数(程序主动代入的参数)
        /// </summary>
        public string MsgTemplateParams { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class SiteMessageSceneInfoCollection : List<SiteMessageSceneInfo>
    {
    }

    /// <summary>
    /// 手机短信记录
    /// </summary>
    public class MoibleSMSSendRecordInfo
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Mobile { get; set; }
        public string SMSContent { get; set; }
        public string SendStatus { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class MoibleSMSSendRecordInfoCollection
    {
        public MoibleSMSSendRecordInfoCollection()
        {
            RecordList = new List<MoibleSMSSendRecordInfo>();
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 站内信列表
        /// </summary>
        public List<MoibleSMSSendRecordInfo> RecordList { get; set; }
    }
}
