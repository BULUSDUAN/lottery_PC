using System;
using System.Collections.Generic;
using System.Linq;

using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    public class InnerMailManager:DBbase
    {
        public string QueryMobileByUserId(string userId)
        {
           
            //var sql = string.Format("select [Mobile] from [E_Authentication_Mobile] where UserId='{0}'", userId);
            return DB.CreateSQLQuery("select [Mobile] from [E_Authentication_Mobile] where UserId=@userId").SetString("userId", userId).First<string>();
        }

        public E_SiteMessage_SiteMessageScene QuerySiteMessageScene(string key)
        {
           
            var result = DB.CreateQuery<E_SiteMessage_SiteMessageScene>().Where(s => s.SceneKey == key).FirstOrDefault();
            return result;
        }

        public void AddMoibleSMSSendRecord(E_SiteMessage_MoibleSMSSendRecord entity)
        {
            DB.GetDal<E_SiteMessage_MoibleSMSSendRecord>().Add(entity);
        }

        public void AddSiteMessageInnerMailListNew(E_SiteMessage_InnerMail_List_new entity)
        {
            DB.GetDal<E_SiteMessage_InnerMail_List_new>().Add(entity);
        }

        public void UpdateSiteMessageInnerMailListNew(E_SiteMessage_InnerMail_List_new entity)
        {
            DB.GetDal<E_SiteMessage_InnerMail_List_new>().Update(entity);
        }
        /// <summary>
        /// 获取指定站内信包含接收者的数量
        /// </summary>
        /// <param name="innerMailId">站内信编号</param>
        /// <param name="userId">用户编号</param>
        /// <returns>包含数量</returns>
        public int GetMailContainsReceiverCount(string innerMailId, string userId)
        {
         
             var query = DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().Where(s => s.MailId == innerMailId && (s.ReceiverId == userId || s.ReceiverId == "U:" + userId));
            if (query != null) return query.Count();
            return 0;
        }

        public E_SiteMessage_InnerMail_List_new QuerySiteMessageInnerMailListNewByMailId(string mailId)
        {
            return DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().Where(s => s.MailId == mailId).FirstOrDefault();
        }
        public string QueryUserIdByRoleId(string roleId)
        {
            string strUserIds = string.Empty;
            var sql = "select userId from C_Auth_UserRole where RoleId=@roleId";
            var result = DB.CreateSQLQuery(sql)
                              .SetString("roleId", roleId).List<C_Auth_UserRole>();
            if (result != null)
            {
                foreach (var item in result)
                {
                    if (item == null || string.IsNullOrEmpty(item.ToString()))
                        continue;
                    strUserIds += item.ToString() + "|";
                }
            }
            if (!string.IsNullOrEmpty(strUserIds))
                strUserIds.TrimEnd('|');
            return strUserIds;
        }
        public string QuerySiteMessageTags()
        {
            var query = (from t in DB.CreateQuery<E_SiteMessage_SiteMessageTags>()
                         orderby t.CreateTime ascending select t).ToList();
            return string.Join("^", query.Select(x => string.Format("{0}={1}", x.TagKey, x.TagName)).ToList());
        }
        public List<SiteMessageSceneInfo> QuerySiteNoticeConfig()
        {
            var query = from s in DB.CreateQuery<E_SiteMessage_SiteMessageScene>()
                        orderby s.Id ascending
                        select new SiteMessageSceneInfo
                        {
                            CreateTime = s.CreateTime,
                            Id = s.Id,
                            MsgCategory = (SiteMessageCategory)s.MsgCategory,
                            MsgTemplateContent = s.MsgTemplateContent,
                            MsgTemplateParams = s.MsgTemplateParams,
                            MsgTemplateTitle = s.MsgTemplateTitle,
                            SceneKey = s.SceneKey,
                            SceneName = s.SceneName,
                        };
            return query.ToList();
        }
        public void UpdateSiteMessageScene(E_SiteMessage_SiteMessageScene entity)
        {
            DB.GetDal<E_SiteMessage_SiteMessageScene>().Update(entity);
        }
        public List<MoibleSMSSendRecordInfo> QuerySMSSendRecordList(string userId, string mobileNumber, DateTime startTime, DateTime endTime, string status, int pageIndex, int pageSize, out int totalCount)
        {
            var query = from l in DB.CreateQuery<E_SiteMessage_MoibleSMSSendRecord>()
                        where (l.UserId==string.Empty || l.UserId == userId)
                        && (l.Mobile==string.Empty || l.Mobile == mobileNumber)
                        && (l.SendStatus==string.Empty || l.SendStatus == status)
                        && (l.CreateTime >= startTime && l.CreateTime <= endTime)
                        orderby l.CreateTime descending
                        select new MoibleSMSSendRecordInfo
                        {
                            CreateTime = l.CreateTime,
                            Id = l.Id,
                            Mobile = l.Mobile,
                            SendStatus = l.SendStatus,
                            SMSContent = l.SMSContent,
                            UserId = l.UserId,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
