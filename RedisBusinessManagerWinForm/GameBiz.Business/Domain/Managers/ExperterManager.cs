using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Core;

namespace GameBiz.Domain.Managers
{
    public class ExperterManager : GameBizEntityManagement
    {
        /// <summary>
        /// 添加专家
        /// </summary>
        public void AddExperter(Experter entity)
        {
            this.Add<Experter>(entity);
        }

        /// <summary>
        /// 删除专家
        /// </summary>
        public void DeleteExperter(Experter entity)
        {
            this.Delete<Experter>(entity);
        }

        /// <summary>
        /// 更新专家信息
        /// </summary>
        public void UpdateExperter(Experter entity)
        {
            this.Update<Experter>(entity);
        }

        /// <summary>
        /// 按专家ID查询专家信息
        /// </summary>
        public Experter QueryExperterById(string experterId)
        {
            this.Session.Clear();
            return this.Session.Query<Experter>().FirstOrDefault(p => p.UserId == experterId);
        }

        public ExperterInfo QueryExperterInfo(string experterId)
        {
            this.Session.Clear();
            var query = from e in this.Session.Query<Experter>()
                        join u in this.Session.Query<UserRegister>() on e.UserId equals u.UserId
                        join a in this.Session.Query<UserAttentionSummary>() on e.UserId equals a.UserId
                        where e.UserId == experterId
                        select new ExperterInfo
                        {
                            Attention = a.BeAttentionUserCount,
                            CreateTime = e.CreateTime,
                            DisplayName = u.DisplayName,
                            ExperterHeadImage = e.ExperterHeadImage,
                            ExperterSummary = e.ExperterSummary,
                            AdeptGameCode = e.AdeptGameCode,
                            ExperterType = e.ExperterType,
                            IsEnable = e.IsEnable,
                            MonthShooting = e.MonthShooting,
                            RecentlyOrderCount = e.RecentlyOrderCount,
                            TotalShooting = e.TotalShooting,
                            UserId = e.UserId,
                            WeekShooting = e.WeekShooting,
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 查询名家列表
        /// </summary>
        public List<ExperterInfo> QueryExperterList(ExperterType? experterType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<Experter>()
                        join a in this.Session.Query<UserAttentionSummary>() on r.UserId equals a.UserId
                        join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                        where (experterType == null || r.ExperterType == experterType)
                        && r.DealWithType == DealWithType.HasDealWith
                        //orderby new { TotalShooting = r.TotalShooting, TotalRate = r.TotalRate, CreateTime = r.CreateTime } descending
                        orderby r.TotalShooting descending, r.TotalRate descending, r.CreateTime descending
                        select new ExperterInfo
                        {
                            UserId = r.UserId,
                            DisplayName = ur.DisplayName,
                            ExperterHeadImage = r.ExperterHeadImage,
                            ExperterSummary = r.ExperterSummary,
                            IsEnable = r.IsEnable,
                            RecentlyOrderCount = r.RecentlyOrderCount,
                            ExperterType = r.ExperterType,
                            CreateTime = r.CreateTime,
                            Attention = a.BeAttentionUserCount == null ? 0 : a.BeAttentionUserCount,
                            WeekShooting = r.WeekShooting == null ? 0M : r.WeekShooting,
                            MonthShooting = r.MonthShooting == null ? 0M : r.MonthShooting,
                            TotalShooting = r.TotalShooting == null ? 0M : r.TotalShooting,
                            AdeptGameCode = r.AdeptGameCode,
                            MonthRate = r.MonthRate == null ? 0M : r.MonthRate,
                            TotalRate = r.TotalRate == null ? 0M : r.TotalRate,
                            WeekRate = r.WeekRate == null ? 0M : r.WeekRate,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 专家命中率排行列表
        /// </summary>
        public List<ExperterShootingInfo> QueryExperterShootingList(ShootingType? shootingType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            if (shootingType == ShootingType.Week)
            {
                var query = from r in this.Session.Query<Experter>()
                            join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                            where r.DealWithType == DealWithType.HasDealWith && r.WeekShooting > 0M
                            orderby r.WeekShooting descending
                            select new ExperterShootingInfo
                            {
                                UserId = r.UserId,
                                DisplayName = ur.DisplayName,
                                ExperterSummary = r.ExperterSummary,
                                Shooting = r.WeekShooting,
                            };
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else if (shootingType == ShootingType.Month)
            {
                var query = from r in this.Session.Query<Experter>()
                            join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                            where r.DealWithType == DealWithType.HasDealWith && r.MonthShooting > 0M
                            orderby r.MonthShooting descending
                            select new ExperterShootingInfo
                            {
                                UserId = r.UserId,
                                DisplayName = ur.DisplayName,
                                ExperterSummary = r.ExperterSummary,
                                Shooting = r.MonthShooting,
                            };
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                var query = from r in this.Session.Query<Experter>()
                            join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                            where (shootingType == null || shootingType == ShootingType.Total)
                            where r.DealWithType == DealWithType.HasDealWith && r.TotalShooting > 0M
                            orderby r.TotalShooting descending
                            select new ExperterShootingInfo
                            {
                                UserId = r.UserId,
                                DisplayName = ur.DisplayName,
                                ExperterSummary = r.ExperterSummary,
                                Shooting = r.TotalShooting,
                            };
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 查询名家发单记录
        /// </summary>
        public List<ExperterPublishedInfo> QueryExperterPublishedList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<OrderDetail>()
                        join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                        join or in this.Session.Query<Experter>() on r.UserId equals or.UserId
                        where (r.SchemeType == SchemeType.ExperterScheme)
                           && or.CreateTime >= startTime && or.CreateTime < endTime.AddDays(1)
                           && r.UserId == userId
                           && or.DealWithType == DealWithType.HasDealWith
                        orderby r.CreateTime descending

                        select new ExperterPublishedInfo
                        {
                            UserId = r.UserId,
                            DisplayName = ur.DisplayName,
                            ExperterSummary = or.ExperterSummary,
                            SchemeId = r.SchemeId,
                            TotalMoney = r.TotalMoney,
                            CreateTime = or.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询修改后待审核列表
        /// </summary>
        public List<ExperterAuditInfo> QueryExperterAuditList(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<ExperterUpdateHitstroy>()
                        join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                        where (r.DealWithType == DealWithType.NoneDealWith)
                        orderby r.CreateTime descending

                        select new ExperterAuditInfo
                        {
                            UserId = r.UserId,
                            DisplayName = ur.DisplayName,
                            ExperterSummary = r.ExperterSummary,
                            AdeptGameCode = r.AdeptGameCode,
                            ExperterHeadImage = r.ExperterHeadImage,
                            UpdateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        #region 名家修改资料历史

        /// <summary>
        /// 添加修改资料审核
        /// </summary>
        public void AddExperterUpdateHitstroy(ExperterUpdateHitstroy entity)
        {
            this.Add<ExperterUpdateHitstroy>(entity);
        }

        /// <summary>
        /// 审核修改资料
        /// </summary>
        public void UpdateExperterUpdateHitstroy(ExperterUpdateHitstroy entity)
        {
            this.Update<ExperterUpdateHitstroy>(entity);
        }

        /// <summary>
        /// 按Id查询专家审核信息
        /// </summary>
        public ExperterUpdateHitstroy QueryExperterUpdateHitstroy(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterUpdateHitstroy>().FirstOrDefault(p => p.UserId == userId && p.DealWithType == DealWithType.NoneDealWith);
        }

        /// <summary>
        /// 查询某名家的修改记录
        /// </summary>
        public List<ExperterUpdateInfo> QueryExperterUpdateList(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<ExperterUpdateHitstroy>()
                        join ur in this.Session.Query<UserRegister>() on r.UserId equals ur.UserId
                        where r.UserId == userId
                        orderby r.CreateTime descending

                        select new ExperterUpdateInfo
                        {
                            UserId = r.UserId,
                            DisplayName = ur.DisplayName,
                            ExperterSummary = r.ExperterSummary,
                            AdeptGameCode = r.AdeptGameCode,
                            DealWithType = r.DealWithType,
                            DisposeOpinion = r.DisposeOpinion,
                            ExperterHeadImage = r.ExperterHeadImage,
                            UpdateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        #endregion

    }
}
