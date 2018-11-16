using System;
using System.Linq;

using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;

using KaSon.FrameWork.Common.SMS;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 消息、短信控制器
    /// </summary>
    public class SiteMessageControllBusiness:DBbase
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        public void SendSMS(string mobile, string content, string userId)
        {
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(content))
                return;

            var biz = new CacheDataBusiness();
            var filterArray = biz.QueryCoreConfigFromRedis("SMS.Filter.Words").Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in filterArray)
            {
                content = content.Replace(word, "");
            }

            var key = biz.QueryCoreConfigFromRedis("CFSMS.Key");
            var pwd = biz.QueryCoreConfigFromRedis("CFSMS.Password");
            var sender = SMSSenderFactory.GetSMSSenderInstance(new SMSConfigInfo
            {
                AgentName = "CFSMS",
                UserName = key,
                Password = pwd,
                Attach = "",
            });
            var r = string.Empty;
            r = sender.SendSMS(mobile, content, "");

            //保存到发送记录
            new InnerMailManager().AddMoibleSMSSendRecord(new E_SiteMessage_MoibleSMSSendRecord
            {
                CreateTime = DateTime.Now,
                Mobile = mobile,
                SendStatus = r,
                SMSContent = content,
                UserId = userId,
            });
        }

        /// <summary>
        /// 某场景触发的发送站内消息
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="sceneKey">场景编码</param>
        /// <param name="msgTemplateParams">消息模板参数，key=value 如：[UserName]=张三</param>
        public void DoSendSiteMessage(string userId, string mobile, string sceneKey, params string[] msgTemplateParams)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneKey))
                    return;
                var manager = new InnerMailManager();
                var scene = manager.QuerySiteMessageScene(sceneKey);
                if (scene == null)
                    return;
                if (scene.MsgCategory == (int)SiteMessageCategory.None)
                    return;
                if (string.IsNullOrEmpty(scene.MsgTemplateContent))
                    return;

                var msgContent = scene.MsgTemplateContent;
                var msgTitle = scene.MsgTemplateTitle;
                var category = scene.MsgCategory;
                foreach (var item in msgTemplateParams)
                {
                    var array = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length != 2)
                        continue;
                    msgContent = msgContent.Replace(array[0], array[1]);
                }

                switch (category)
                {
                    case (int)SiteMessageCategory.None:
                        break;
                    case (int)SiteMessageCategory.MobileSMS:
                        if (string.IsNullOrEmpty(mobile))
                            mobile = manager.QueryMobileByUserId(userId);
                        if (!string.IsNullOrEmpty(mobile))
                        {
                            this.SendSMS(mobile, msgContent, userId);
                        }
                        break;
                    case (int)SiteMessageCategory.InnerMail:
                        this.SendInnerMail(new InnerMailInfo_Send
                        {
                            Title = msgTitle,
                            Content = msgContent,
                            ActionTime = DateTime.Now,
                            Receivers = string.Format("U:{0}", userId),
                        }, userId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //Common.Log.LogWriterGetter.GetLogWriter().Write("SiteMessageControllBusiness", "DoSendSiteMessage", ex);
            }
        }

        /// <summary>
        /// 发送站内信
        /// </summary>
        public void SendInnerMail(InnerMailInfo_Send innerMail, string createBy)
        {
            if (innerMail == null || string.IsNullOrEmpty(innerMail.Receivers))
                return;
            var arrReceive = innerMail.Receivers.Split('|');
            if (arrReceive.Length <= 0)
                return;
            var manager = new InnerMailManager();

            //DB.Begin();
            try
            {
                foreach (var item in arrReceive)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    var mail = new E_SiteMessage_InnerMail_List_new
                    {
                        MailId = Guid.NewGuid().ToString(),
                        SendTime = DateTime.Now,
                        SenderId = createBy,
                        HandleType = (int)InnerMailHandleType.UnRead,
                        MsgContent = innerMail.Content,
                        ReceiverId = item,
                        Title = innerMail.Title,
                    };
                    manager.AddSiteMessageInnerMailListNew(mail);
                }
                //DB.Commit();
            }
            catch  (Exception ex)
            {
                //DB.Rollback();
                throw ex;
            }
          
           
        }

        public int GetUserInnerMailCount(string userId)
        {
            var query = DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().Where(s => (s.ReceiverId == userId || s.ReceiverId == "U:" + userId) && s.HandleType != (int)InnerMailHandleType.Deleted);
            if (query != null) return query.Count();
            return 0;
        }

        public SiteMessageInnerMailListNew_Collection QueryInnerMailListByReceiver(string userId, int pageIndex, int pageSize)
        {
          
            SiteMessageInnerMailListNew_Collection collection = new SiteMessageInnerMailListNew_Collection();
            collection.TotalCount = 0;

            var query = (from m in DB.CreateQuery<E_SiteMessage_InnerMail_List_new>()
                        where (m.ReceiverId == userId || m.ReceiverId == "U:" + userId)
                        && m.HandleType != (int)InnerMailHandleType.Deleted
                        select m).ToList().Select(m=> new SiteMessageInnerMailListNewInfo
                        {
                            HandleType = (InnerMailHandleType)m.HandleType,
                            MailId = m.MailId,
                            MsgContent = m.MsgContent,
                            ReadTime = m.ReadTime,
                            ReceiverId = m.ReceiverId,
                            SenderId = m.SenderId,
                            SendTime = m.SendTime,
                            Title = m.Title,
                        });
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.MailList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }


        public bool IsMyInnerMail(string innerMailId, string userId)
        {
                var manager = new InnerMailManager();           
                var count = manager.GetMailContainsReceiverCount(innerMailId, userId);
                return (count > 0);
            
        }

       

        public InnerMailInfo_Query QueryInnerMailDetailById(string innerMailId)
        {
            var manager = new InnerMailManager();

            var mail = manager.QuerySiteMessageInnerMailListNewByMailId(innerMailId);
            var info = new InnerMailInfo_Query
            {
                MailId = mail.MailId,
                Title = mail.Title,
                Content = mail.MsgContent,
                SenderId = mail.SenderId,
                SendTime = mail.SendTime,
            };
            return info;

        }

        public InnerMailInfo_Query ReadInnerMail(string innerMailId, string UserId)
        {
            if (!IsMyInnerMail(innerMailId, UserId))
            {
                throw new SiteMessageException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, UserId));
            }
            var read = new DataQuery();
            read.ReadInnerMail(innerMailId, UserId);
            var info = QueryInnerMailDetailById(innerMailId);
            return info;
        }

        public CommonActionResult DeleteInnerMail(string innerMailId, string userId)
        {

            if (!IsMyInnerMail(innerMailId, userId))
            {
                throw new SiteMessageException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, userId));
            }
            deleteInnerMail(innerMailId, userId);
            return new CommonActionResult(true, "删除站内信完成。");
        }

        public void deleteInnerMail(string innerMailId, string userId)
        {
            var manager = new InnerMailManager();
            var mail = manager.QuerySiteMessageInnerMailListNewByMailId(innerMailId);
            if (mail != null)
            {
                mail.HandleType = (int)InnerMailHandleType.Deleted;           
                    mail.ReadTime = DateTime.Now;
                manager.UpdateSiteMessageInnerMailListNew(mail);
            }
        }
        public string QueryUserIdByRoleId(string roleId)
        {

            return new InnerMailManager().QueryUserIdByRoleId(roleId);
        }
        /// <summary>
        /// 查询站点信息参数
        /// </summary>
        public string QuerySiteMessageTags()
        {
            var manager = new InnerMailManager();
            return manager.QuerySiteMessageTags();
        }
        /// <summary>
        /// 查询网站通知配置
        /// </summary>
        public SiteMessageSceneInfoCollection QuerySiteNoticeConfig()
        {
            var r = new SiteMessageSceneInfoCollection();
            r.AddRange(new InnerMailManager().QuerySiteNoticeConfig());
            return r;
        }
        /// <summary>
        /// 修改网站通知配置
        /// </summary>
        public void UpdateSiteNotice(string key, SiteMessageCategory category, string title, string content)
        {
            var manager = new InnerMailManager();
            var notice = manager.QuerySiteMessageScene(key);
            if (notice == null)
                return;

            notice.MsgCategory = (int)category;
            notice.MsgTemplateTitle = title;
            notice.MsgTemplateContent = content;
            manager.UpdateSiteMessageScene(notice);
        }
        /// <summary>
        /// 查询短信发送记录
        /// </summary>
        public MoibleSMSSendRecordInfoCollection QuerySMSSendRecordList(string userId, string mobileNumber, DateTime startTime, DateTime endTime, string status, int pageIndex, int pageSize)
        {
            var r = new MoibleSMSSendRecordInfoCollection();
            var totalCount = 0;
            var list = new InnerMailManager().QuerySMSSendRecordList(userId, mobileNumber, startTime, endTime, status, pageIndex, pageSize, out totalCount);
            r.TotalCount = totalCount;
            r.RecordList = list;
            return r;
        }
    }
}
