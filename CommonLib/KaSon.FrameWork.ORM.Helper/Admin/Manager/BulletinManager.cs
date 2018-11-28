using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using EntityModel;
using EntityModel.Enum;
using EntityModel.CoreModel;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class BulletinManager:DBbase
    {
        public void AddBulletin(E_SiteMessage_Bulletin_List bulletin)
        {
            bulletin.CreateTime = DateTime.Now;
            bulletin.UpdateTime = DateTime.Now;
            DB.GetDal<E_SiteMessage_Bulletin_List>().Add(bulletin);
        }
        public void UpdateBulletin(E_SiteMessage_Bulletin_List bulletin)
        {
            bulletin.UpdateTime = DateTime.Now;
            DB.GetDal<E_SiteMessage_Bulletin_List>().Update(bulletin);
        }
        public E_SiteMessage_Bulletin_List GetBulletinById(long id)
        {
            return DB.CreateQuery<E_SiteMessage_Bulletin_List>().Where(p=>p.Id==id).FirstOrDefault();
        }
        
        public List<BulletinInfo_Query> QueryAdminBulletinList(string key, EnableStatus status, int priority, int isPutTop, int pageIndex, int pageSize, out int totalCount)
        {
            //pageIndex = pageIndex < 0 ? 0 : pageIndex;
            //pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            //Dictionary<string, object> outputs;
            //var list = CreateOutputQuery(Session.GetNamedQuery("P_QueryBulletinList_Admin"))
            //    .AddInParameter("Key", key)
            //    .AddInParameter("Status", (int)status)
            //    .AddInParameter("Priority", priority)
            //    .AddInParameter("IsPutTop", isPutTop)
            //    .AddInParameter("PageIndex", pageIndex)
            //    .AddInParameter("PageSize", pageSize)
            //    .AddOutParameter("TotalCount", "Int32")
            //    .List(out outputs);
            //totalCount = (int)outputs["TotalCount"];
            //return list;

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            string count_Sql = SqlModule.AdminModule.FirstOrDefault(x => x.Key == "Admin_QueryAdminBulletinList_Count").SQL;
            totalCount = DB.CreateSQLQuery(count_Sql)
                .SetString("Key", key)
                .SetInt("Status", (int)status)
                .SetInt("Priority", priority)
                .SetInt("IsPutTop", isPutTop)
                .First<int>();
            if (totalCount == 0) return new List<BulletinInfo_Query>();
            string sql = SqlModule.AdminModule.FirstOrDefault(x => x.Key == "Admin_QueryAdminBulletinList").SQL;
            List<BulletinInfo_Query> list = DB.CreateSQLQuery(sql)
                .SetString("Key", key)
                .SetInt("Status", (int)status)
                .SetInt("Priority", priority)
                .SetInt("IsPutTop", isPutTop)
                .SetInt("PageIndex", pageIndex)
                .SetInt("PageSize", pageSize)
                .List<BulletinInfo_Query>().ToList();
            return list;
        }

        public List<SiteMessageBannerInfo> QuerySiteMessageBannerCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            var endDate = endTime.AddDays(1).Date;
            var query = from s in DB.CreateQuery<E_Sitemessage_Banner>()
                        where (title == "" || s.BannerTitle.Contains(title)) &&
                            (s.CreateTime >= startTime.Date && s.CreateTime < endDate)
                        select s;
            if (query != null)
            {
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList()
                    .Select(p => new SiteMessageBannerInfo()
                    {
                        BannerId = p.BannerId,
                        BannerIndex = p.BannerIndex,
                        BannerTitle = p.BannerTitle,
                        BannerType = (BannerType)p.BannerType,
                        CreateTime = p.CreateTime,
                        ImageUrl = p.ImageUrl,
                        IsEnable = p.IsEnable,
                        JumpUrl = p.JumpUrl
                    }).ToList();
            }
            totalCount = 0;
            return new List<SiteMessageBannerInfo>();
        }
        #region 广告管理

        public E_Sitemessage_Banner GetBannerManagerInfo(string bannerId)
        {
            var id = Convert.ToInt32(bannerId);
            return DB.CreateQuery<E_Sitemessage_Banner>().Where(p=>p.BannerId== id).FirstOrDefault();
        }

        public void UpdateBannerInfo(E_Sitemessage_Banner entity)
        {
            DB.GetDal<E_Sitemessage_Banner>().Update(entity);
        }
        public void AddBannerInfo(E_Sitemessage_Banner entity)
        {
            DB.GetDal<E_Sitemessage_Banner>().Add(entity);
        }

        public E_Sitemessage_Banner QueryBannerManager(int bannerId)
        {
            return DB.CreateQuery<E_Sitemessage_Banner>().Where(p => p.BannerId == bannerId).FirstOrDefault();
        }

        public void DeleteBanner(E_Sitemessage_Banner entity)
        {
            DB.GetDal<E_Sitemessage_Banner>().Delete(entity);
        }
        #endregion
    }
}
