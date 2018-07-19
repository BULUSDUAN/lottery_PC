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
            return DB.CreateSQLQuery("select [Mobile] from [E_Authentication_Mobile] where UserId=:userId").SetString("userId", userId).First<string>();
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

    }
}
