using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain;
using Common.Business;
using NHibernate.Linq;
using Common;
using GameBiz.Domain.Entities;
using NHibernate.Criterion;
using System.Collections;
using External.Domain.Entities.SiteMessage;
using External.Core.SiteMessage;
using GameBiz.Business;
using GameBiz.Core;

namespace External.Domain.Managers.SiteMessage
{
    public class BulletinManager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddBulletin(Bulletin bulletin)
        {
            bulletin.CreateTime = DateTime.Now;
            bulletin.UpdateTime = DateTime.Now;
            Add<Bulletin>(bulletin);
        }
        public void UpdateBulletin(Bulletin bulletin)
        {
            bulletin.UpdateTime = DateTime.Now;
            Update<Bulletin>(bulletin);
        }
        public Bulletin GetBulletinById(long id)
        {
            return SingleByKey<Bulletin>(id);
        }
        public BulletinInfo_Query QueryBulletinDetailById(long id)
        {
            Session.Clear();
            var query = from b in Session.Query<Bulletin>()
                        join u_create in Session.Query<UserRegister>() on b.CreateBy equals u_create.UserId
                        join u_update in Session.Query<UserRegister>() on b.CreateBy equals u_update.UserId
                        where b.Id == id
                        select new BulletinInfo_Query
                        {

                            Id = b.Id,
                            Title = b.Title,
                            Content = b.Content,
                            Status = b.Status,
                            Priority = b.Priority,
                            IsPutTop = b.IsPutTop,
                            EffectiveFrom = b.EffectiveFrom,
                            EffectiveTo = b.EffectiveTo,
                            CreateTime = b.CreateTime,
                            CreateBy = b.CreateBy,
                            CreatorDisplayName = u_create.DisplayName,
                            UpdateTime = b.UpdateTime,
                            UpdateBy = b.UpdateBy,
                            UpdatorDisplayName = u_update.DisplayName,
                            BulletinAgent = b.BulletinAgent,
                        };
            return query.FirstOrDefault();
        }
        public IList QueryDispayBulletinList(BulletinAgent agent, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var list = CreateOutputQuery(Session.GetNamedQuery("P_QueryBulletinList_Web"))
                .AddInParameter("BulletinAgent", (int)agent)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32")
                .List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }
        public IList QueryAdminBulletinList(string key, EnableStatus status, int priority, int isPutTop, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var list = CreateOutputQuery(Session.GetNamedQuery("P_QueryBulletinList_Admin"))
                .AddInParameter("Key", key)
                .AddInParameter("Status", (int)status)
                .AddInParameter("Priority", priority)
                .AddInParameter("IsPutTop", isPutTop)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32")
                .List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }

        public List<SiteMessageBannerInfo> QuerySiteMessageBannerCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from s in Session.QueryOver<SiteMessageBanner>().List()
                        where (title == string.Empty || s.BannerTitle.Contains(title)) &&
                            (s.CreateTime >= startTime.Date && s.CreateTime < endTime.AddDays(1).Date)
                        select new SiteMessageBannerInfo
                        {
                            BannerId = s.BannerId,
                            BannerTitle = s.BannerTitle,
                            ImageUrl = s.ImageUrl,
                            BannerType = s.BannerType,
                            JumpUrl = s.JumpUrl,
                            IsEnable = s.IsEnable,
                            CreateTime = s.CreateTime
                        };
            if (query != null)
            {
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            totalCount = 0;
            return new List<SiteMessageBannerInfo>();
        }
        #region 广告管理

        public SiteMessageBanner GetBannerManagerInfo(string bannerId)
        {
            Session.Clear();
            return Session.Get<SiteMessageBanner>(Convert.ToInt32(bannerId));
        }

        public void UpdateBannerInfo(SiteMessageBanner entity)
        {
            this.Update<SiteMessageBanner>(entity);
        }
        public void AddBannerInfo(SiteMessageBanner entity)
        {
            this.Add<SiteMessageBanner>(entity);
        }
        public List<SiteMessageBannerInfo> QuerySitemessageBanngerList_Web(BannerType bannerType, int returnRecord = 10)
        {
            Session.Clear();
            var query = from s in Session.Query<SiteMessageBanner>()
                        where s.BannerType == bannerType && s.IsEnable == true
                        orderby s.BannerIndex ascending
                        select new SiteMessageBannerInfo
                        {
                            BannerId = s.BannerId,
                            BannerIndex=s.BannerIndex,
                            BannerTitle = s.BannerTitle,
                            BannerType = s.BannerType,
                            CreateTime = s.CreateTime,
                            ImageUrl = s.ImageUrl,
                            IsEnable = s.IsEnable,
                            JumpUrl = s.JumpUrl,
                        };
            if (query != null)
            {
                return query.Take(returnRecord).ToList();
            }
            return null;
        }

        public SiteMessageBanner QueryBannerManager(int bannerId)
        {
            this.Session.Clear();
            return this.Session.Query<SiteMessageBanner>().Where(p => p.BannerId == bannerId).FirstOrDefault();
        }

        public void DeleteBanner(SiteMessageBanner entity)
        {
            this.Delete(entity);
        }
        #endregion
    }
}
