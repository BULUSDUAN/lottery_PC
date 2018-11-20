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
    public class ExperterAnalyzeSchemeeManager : GameBizEntityManagement
    {
        /// <summary>
        /// 添加专家推荐分析
        /// </summary>
        public void AddExperterAnalyzeScheme(ExperterAnalyzeScheme entity)
        {
            this.Add<ExperterAnalyzeScheme>(entity);
        }

        /// <summary>
        /// 删除专家推荐分析
        /// </summary>
        public void DeleteExperterAnalyzeScheme(ExperterAnalyzeScheme entity)
        {
            this.Delete<ExperterAnalyzeScheme>(entity);
        }

        /// <summary>
        /// 更新专家推荐分析
        /// </summary>
        public void UpdateExperterAnalyzeScheme(ExperterAnalyzeScheme entity)
        {
            this.Update<ExperterAnalyzeScheme>(entity);
        }

        /// <summary>
        /// 按分析ID查询分析详细
        /// </summary>
        public ExperterAnalyzeScheme QueryExperterAnalyzeId(string analyzeId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterAnalyzeScheme>().FirstOrDefault(p => p.AnalyzeId == analyzeId);
        }

        /// <summary>
        /// 查询专家推荐分析列表
        /// </summary>
        public List<ExperterAnalyzeSchemeInfo> QueryExperterAnalyzeSchemeList(int pageIndex, int pageSize, out int totalCount, string currentTime)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<ExperterAnalyzeScheme>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where (r.DealWithType == DealWithType.HasDealWith)
                        && (currentTime == string.Empty || currentTime == r.CurrentTime)
                        orderby r.CreateTime descending
                        select new ExperterAnalyzeSchemeInfo
                        {
                            UserId = r.UserId,
                            DisplayName = u.DisplayName,
                            AnalyzeId = r.AnalyzeId,
                            Title = r.Title,
                            IsBuy = false,
                            Source = r.Source,
                            CurrentTime = r.CurrentTime,
                            DealWithType = r.DealWithType,
                            DisposeOpinion = r.DisposeOpinion,
                            Price = r.Price,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询某专家推荐分析列表
        /// </summary>
        public List<ExperterAnalyzeInfo> QueryUserAnalyzeList(string userId, string currentDate, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<ExperterAnalyzeScheme>()
                        where (r.DealWithType == DealWithType.HasDealWith)
                        && (currentDate == string.Empty || r.CurrentTime == currentDate)
                        && r.UserId == userId
                        orderby r.CreateTime descending
                        select new ExperterAnalyzeInfo
                        {
                            AnalyzeId = r.AnalyzeId,
                            Title = r.Title,
                            SellCount = r.SellCount,
                            Price = r.Price,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 后台查询专家推荐分析列表
        /// </summary>
        public List<ExperterAnalyzeSchemeInfo> QueryBackgroundExperterAnalyzeList(int pageIndex, int pageSize, out int totalCount, string currentTime)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<ExperterAnalyzeScheme>()
                        where (r.DealWithType == DealWithType.NoneDealWith)
                        && (currentTime == string.Empty || currentTime == r.CurrentTime)
                        orderby r.CreateTime descending
                        select new ExperterAnalyzeSchemeInfo
                        {
                            UserId = r.UserId,
                            AnalyzeId = r.AnalyzeId,
                            Title = r.Title,
                            Content = r.Content,
                            Source = r.Source,
                            DisposeOpinion = r.DisposeOpinion,
                            CurrentTime = r.CurrentTime,
                            DealWithType = r.DealWithType,
                            Price = r.Price,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        #region 分析交易相关

        /// <summary>
        /// 按分析ID查询购买详细
        /// </summary>
        public ExperterAnalyzeTransaction QueryExperterAnalyzeTransaction(string analyzeId, string userId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterAnalyzeTransaction>().FirstOrDefault(p => p.AnalyzeId == analyzeId && p.UserId == userId);
        }

        /// <summary>
        /// 添加购买分析信息
        /// </summary>
        public void AddExperterAnalyzeTransaction(ExperterAnalyzeTransaction entity)
        {
            this.Add<ExperterAnalyzeTransaction>(entity);
        }

        public string[] QueryUserBuyedAnalyze(string userId, string[] analyzeIdArray)
        {
            this.Session.Clear();
            var query = from e in this.Session.Query<ExperterAnalyzeTransaction>()
                        where e.UserId == userId
                        && analyzeIdArray.Contains(e.AnalyzeId)
                        select e.AnalyzeId;
            return query.ToArray();
        }

        #endregion

    }
}
