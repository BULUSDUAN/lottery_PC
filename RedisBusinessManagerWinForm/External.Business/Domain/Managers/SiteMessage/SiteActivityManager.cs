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
using NHibernate;
using External.Domain.Entities.SiteMessage;
using GameBiz.Business;
using External.Core.SiteMessage;

namespace External.Domain.Managers.SiteMessage
{
    public class SiteActivityManager : GameBiz.Business.GameBizEntityManagement
    {
        /// <summary>
        /// 添加活动
        /// </summary>
        public void AddSiteActivity(SiteActivity entity)
        {
            Add<SiteActivity>(entity);
        }

        public void AddLotteryNewBonus(LotteryNewBonus entity)
        {
            this.Add<LotteryNewBonus>(entity);
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        public void UpdateSiteActivity(SiteActivity entity)
        {
            Update<SiteActivity>(entity);
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        public void DeleteSiteActivity(SiteActivity entity)
        {
            this.Delete<SiteActivity>(entity);
        }

        /// <summary>
        /// 查询所有活动配置
        /// </summary>
        public List<SiteActivityInfo> QueryAllSiteActivity()
        {
            Session.Clear();
            var query = from s in Session.Query<SiteActivity>()
                        select new SiteActivityInfo
                        {
                            Id = s.Id,
                            ArticleUrl = s.ArticleUrl,
                            ImageUrl = s.ImageUrl,
                            Titile = s.Titile,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                        };
            if (query != null)
            {
                return query.ToList();
            }
            return null;
        }


        /// <summary>
        /// 查询某一个活动配置
        /// </summary>
        public SiteActivity QuerySiteActivity(int id)
        {
            Session.Clear();
            var query = from a in this.Session.Query<SiteActivity>()
                        where a.Id == id
                        select a;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 查询某一个活动配置
        /// </summary>
        public SiteActivityInfo QuerySiteActivityInfo(int id)
        {
            Session.Clear();
            var query = from a in this.Session.Query<SiteActivity>()
                        where a.Id == id
                        select new SiteActivityInfo
                        {
                            ArticleUrl = a.ArticleUrl,
                            Id = a.Id,
                            Titile = a.Titile,
                            ImageUrl = a.ImageUrl,
                            StartTime = a.StartTime,
                            EndTime = a.EndTime,
                        };
            return query.FirstOrDefault();
        }

        public List<LotteryNewBonusInfo> QueryLotteryNewBonusInfoList(int count)
        {
            Session.Clear();

            var query = from b in this.Session.Query<LotteryNewBonus>()
                        orderby b.CreateTime descending
                        select new LotteryNewBonusInfo
                        {
                            AfterTaxBonusMoney = b.AfterTaxBonusMoney,
                            Amount = b.Amount,
                            CreateTime = b.CreateTime,
                            GameCode = b.GameCode,
                            GameType = b.GameType,
                            HideUserDisplayNameCount = b.HideUserDisplayNameCount,
                            IssuseNumber = b.IssuseNumber,
                            PlayType = b.PlayType,
                            PreTaxBonusMoney = b.PreTaxBonusMoney,
                            SchemeId = b.SchemeId,
                            TotalMoney = b.TotalMoney,
                            UserDisplayName = b.UserDisplayName,
                        };
            return query.Take(count).ToList();
        }


    }
}
