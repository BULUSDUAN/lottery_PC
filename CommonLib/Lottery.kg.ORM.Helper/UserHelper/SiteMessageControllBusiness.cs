using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Net.SMS;
using EntityModel;
using EntityModel.CoreModel;

namespace Lottery.Kg.ORM.Helper.UserHelper
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

            DB.Begin();

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
            DB.Commit();
        }
    }
}
