using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using GameBiz.Core;
using GameBiz.Domain.Entities;
using Common.Utilities;
using GameBiz.Business.Domain.Managers;
using Common.Net.SMS;

namespace GameBiz.Business
{
    /// <summary>
    /// 消息、短信控制器
    /// </summary>
    public class SiteMessageControllBusiness
    {
        #region 站内信

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

            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                using (var manager = new InnerMailManager())
                {
                    biz.BeginTran();

                    foreach (var item in arrReceive)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        var mail = new SiteMessageInnerMailListNew
                        {
                            MailId = Guid.NewGuid().ToString(),
                            SendTime = DateTime.Now,
                            SenderId = createBy,
                            HandleType = InnerMailHandleType.UnRead,
                            MsgContent = innerMail.Content,
                            ReceiverId = item,
                            Title = innerMail.Title,
                        };
                        manager.AddSiteMessageInnerMailListNew(mail);
                    }

                    biz.CommitTran();
                }
            }
        }
        #region 20151226
        //public IList QueryInnerMailListByReceiver(string userId, int pageIndex, int pageSize, out int totalCount)
        //{
        //    using (var manager = new InnerMailManager())
        //    {
        //        var obj = manager.QueryInnerMailList_ByReceiverId(userId, pageIndex, pageSize, out totalCount);
        //        return obj;
        //    }
        //} 

        public SiteMessageInnerMailListNew_Collection QueryInnerMailListByReceiver(string userId, int pageIndex, int pageSize)
        {
            using (var manager = new InnerMailManager())
            {
                return manager.QueryInnerMailList_ByReceiverId(userId, pageIndex, pageSize);
            }
        }
        #endregion

        #region 20151226
        //public InnerMailInfo_Query QueryInnerMailDetailById(string innerMailId)
        //{
        //    using (var manager = new InnerMailManager())
        //    {
        //        var mail = manager.GetInnerMailById(innerMailId);
        //        var info = new InnerMailInfo_Query
        //        {
        //            MailId = mail.MailId,
        //            Title = mail.Title,
        //            Content = mail.Content,
        //            SenderId = mail.Sender.UserId,
        //            SendTime = mail.SendTime,
        //            UpdateTime = mail.UpdateTime,
        //            ActionTime = mail.ActionTime,
        //        };
        //        return info;
        //    }
        //} 

        public InnerMailInfo_Query QueryInnerMailDetailById(string innerMailId)
        {
            using (var manager = new InnerMailManager())
            {
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
        }
        #endregion


        #region 20151226
        ///// <summary>
        ///// 阅读站内信
        ///// </summary>
        //public void ReadInnerMail(string innerMailId, string userId)
        //{
        //    using (var biz = new GameBiz.Business.GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var manager = new InnerMailManager())
        //        {
        //            var user = manager.LoadUser(userId);
        //            var mail = manager.GetInnerMailById(innerMailId);
        //            var record = manager.GetReadRecord(mail, user);
        //            if (record == null)
        //            {
        //                record = new InnerMailReadRecord
        //                {
        //                    Mail = mail,
        //                    Receiver = user,
        //                    HandleType = InnerMailHandleType.Readed,
        //                };
        //                manager.AddReadRecord(record);
        //            }
        //            else
        //            {
        //                record.HandleType = InnerMailHandleType.Readed;
        //                manager.UpdateReadRecord(record);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //} 

        /// <summary>
        /// 阅读站内信
        /// </summary>
        public void ReadInnerMail(string innerMailId, string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                using (var manager = new InnerMailManager())
                {
                    var mail = manager.QuerySiteMessageInnerMailListNewByMailId(innerMailId);
                    if (mail != null)
                    {
                        mail.ReadTime = DateTime.Now;
                        mail.HandleType = InnerMailHandleType.Readed;
                        manager.UpdateSiteMessageInnerMailListNew(mail);
                    }
                }
            }
        }

        #endregion

        #region 20151226

        //public void DeleteInnerMail(string innerMailId, string userId)
        //{
        //    using (var biz = new GameBiz.Business.GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var manager = new InnerMailManager())
        //        {
        //            var user = manager.LoadUser(userId);
        //            var mail = manager.GetInnerMailById(innerMailId);
        //            var record = manager.GetReadRecord(mail, user);
        //            if (record == null)
        //            {
        //                record = new InnerMailReadRecord
        //                {
        //                    Mail = mail,
        //                    Receiver = user,
        //                    HandleType = InnerMailHandleType.Deleted,
        //                };
        //                manager.AddReadRecord(record);
        //            }
        //            else
        //            {
        //                record.HandleType = InnerMailHandleType.Deleted;
        //                manager.UpdateReadRecord(record);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //} 

        public void DeleteInnerMail(string innerMailId, string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                using (var manager = new InnerMailManager())
                {
                    var mail = manager.QuerySiteMessageInnerMailListNewByMailId(innerMailId);
                    if (mail != null)
                    {
                        mail.HandleType = InnerMailHandleType.Deleted;
                        if (!mail.ReadTime.HasValue || mail.ReadTime.Value.Year <= 1900)
                            mail.ReadTime = DateTime.Now;
                        manager.UpdateSiteMessageInnerMailListNew(mail);
                    }
                }
            }
        }

        #endregion
        public bool IsMyInnerMail(string innerMailId, string userId)
        {
            using (var manager = new InnerMailManager())
            {
                var count = manager.GetMailContainsReceiverCount(innerMailId, userId);
                return (count > 0);
            }
        }
        public int GetUnreadMailCountByUser(string userId)
        {
            using (var manager = new InnerMailManager())
            {
                var count = manager.GetUnreadMailCount(userId);
                return count;
            }
        }

        #region 20151226

        //public InnerMailInfo_QueryCollection QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, InnerMailHandleType handleType)
        //{
        //    using (var manager = new InnerMailManager())
        //    {
        //        var result = new InnerMailInfo_QueryCollection();
        //        int totalCount;
        //        var list = manager.QueryUnReadInnerMailList_ByReceiverId(userId, pageIndex, pageSize, Convert.ToInt32(handleType), out totalCount);

        //        result.LoadList(list);
        //        if (result != null && result.InnerMailList != null)
        //        {
        //            result.TotalCount = totalCount;
        //            //var tempResult = result.InnerMailList.Where(s => s.HandleType == handleType);
        //            //if (tempResult != null && tempResult.Count() > 0)
        //            //{
        //            //    result.TotalCount = tempResult.Count();
        //            //    result.InnerMailList = tempResult.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        //            //}
        //        }
        //        return result;
        //    }
        //} 

        public SiteMessageInnerMailListNew_Collection QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, InnerMailHandleType handleType)
        {
            using (var manager = new InnerMailManager())
            {
                var result = new SiteMessageInnerMailListNew_Collection();
                return manager.QueryUnReadInnerMailList_ByReceiverId(userId, pageIndex, pageSize, Convert.ToInt32(handleType));
            }
        }

        #endregion

        #region 20151226
        //private void ForeachInnerMainSendAddress(string receivers, Func<InnerMailSendAddress> addressCreateInstanceHandler, Action<InnerMailSendAddress> addressAction)
        //{
        //    foreach (var item in receivers.Split('|'))
        //    {
        //        var address = addressCreateInstanceHandler();
        //        if (item.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        //        {
        //            address.ReceiverType = InnerMailReceiverType.All;
        //            address.ReceiverId = null;
        //        }
        //        else if (item.StartsWith("R:", StringComparison.OrdinalIgnoreCase))
        //        {
        //            address.ReceiverType = InnerMailReceiverType.Roles;
        //            address.ReceiverId = item.Substring(2);
        //        }
        //        else if (item.StartsWith("U:", StringComparison.OrdinalIgnoreCase))
        //        {
        //            address.ReceiverType = InnerMailReceiverType.Users;
        //            address.ReceiverId = item.Substring(2);
        //        }
        //        else
        //        {
        //            throw new ArgumentException("接收者格式错误 - " + receivers);
        //        }
        //        addressAction(address);
        //    }
        //} 
        #endregion

        #endregion

        #region 手机短信

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
            new InnerMailManager().AddMoibleSMSSendRecord(new MoibleSMSSendRecord
            {
                CreateTime = DateTime.Now,
                Mobile = mobile,
                SendStatus = r,
                SMSContent = content,
                UserId = userId,
            });
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

        #endregion

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
                if (scene.MsgCategory == SiteMessageCategory.None)
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
                    case SiteMessageCategory.None:
                        break;
                    case SiteMessageCategory.MobileSMS:
                        if (string.IsNullOrEmpty(mobile))
                            mobile = manager.QueryMobileByUserId(userId);
                        if (!string.IsNullOrEmpty(mobile))
                        {
                            this.SendSMS(mobile, msgContent, userId);
                        }
                        break;
                    case SiteMessageCategory.InnerMail:
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
                Common.Log.LogWriterGetter.GetLogWriter().Write("SiteMessageControllBusiness", "DoSendSiteMessage", ex);
            }
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

            notice.MsgCategory = category;
            notice.MsgTemplateTitle = title;
            notice.MsgTemplateContent = content;
            manager.UpdateSiteMessageScene(notice);
        }
        public int GetUserInnerMailCount(string userId)
        {
            using (var manager = new InnerMailManager())
            {
                return manager.GetUserInnerMailCount(userId);
            }
        }
        public string QueryUserIdByRoleId(string roleId)
        {
            using (var manager = new InnerMailManager())
            {
                return manager.QueryUserIdByRoleId(roleId);
            }
        }
    }
}
