using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
           
            var result = DB.CreateQuery<E_SiteMessage_SiteMessageScene>().Where(s => s.SceneKey == key).ToList();
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            return null;
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
    }
}
