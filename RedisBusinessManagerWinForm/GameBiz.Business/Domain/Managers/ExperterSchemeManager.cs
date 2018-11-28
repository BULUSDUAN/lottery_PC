using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Core;
using Common.Utilities;
using Common.Database.ORM;
using System.Data;

namespace GameBiz.Domain.Managers
{
    public class ExperterSchemeManager : GameBizEntityManagement
    {
        /// <summary>
        /// 添加专家方案 
        /// </summary>
        public void AddExperterScheme(ExperterScheme entity)
        {
            this.Add<ExperterScheme>(entity);
        }

        /// <summary>
        /// 删除专家方案
        /// </summary>
        public void DeleteExperterScheme(ExperterScheme entity)
        {
            this.Delete<ExperterScheme>(entity);
        }

        /// <summary>
        /// 更新专家方案信息
        /// </summary>
        public void UpdateExperterScheme(ExperterScheme entity)
        {
            this.Update<ExperterScheme>(entity);
        }
        public void UpdateExperter(Experter entity)
        {
            this.Update<Experter>(entity);
        }
        /// <summary>
        /// 按方案ID查询专家方案信息
        /// </summary>
        public ExperterScheme QueryExperterSchemeId(string schemeId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterScheme>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        /// <summary>
        /// 按创建时间戳查询专家方案信息
        /// </summary>
        public int QueryExperterCurrentTimeScheme(string userId, string currentTime)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterScheme>().Where(p => p.CurrentTime == currentTime && p.UserId == userId).Count();
        }

        /// <summary>
        /// 查询名家方案关系列表
        /// </summary>
        public List<ExperterQuerySchemeInfo> QueryExperterSchemeList(ExperterType experterType, string userId, string currentTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<ExperterScheme>()
                        where (r.ExperterType == experterType)
                        && (currentTime == "" || r.CurrentTime == currentTime)
                        && (userId == "" || r.UserId == userId)
                        orderby r.CreateTime descending
                        select new ExperterQuerySchemeInfo
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            Against = r.Against,
                            ExperterType = r.ExperterType,
                            GuestTeamComments = r.GuestTeamComments,
                            HomeTeamComments = r.HomeTeamComments,
                            SchemeId = r.SchemeId,
                            Support = r.Support,
                            CurrentTime = r.CurrentTime,
                            CreateTime = r.CreateTime,
                            StopTime = r.StopTime,
                            BonusStatus = r.BonusStatus,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询名家争霸赛排行
        /// </summary>
        public List<ExperterRankingInfo> QueryExperterRankingList(DateTime starTime, DateTime endTime)
        {
            Session.Clear();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Experter_Ranking"));
            query = query.AddInParameter("StarTime", starTime);
            query = query.AddInParameter("EndTime", endTime);
            var dt = query.GetDataTable();
            var list = new List<ExperterRankingInfo>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ExperterRankingInfo
                {
                    BonusCount = row["BonusCount"] == DBNull.Value ? 0 : int.Parse(row["BonusCount"].ToString()),
                    DisplayName = row["DisplayName"].ToString() == null ? "" : row["DisplayName"].ToString(),
                    FollowerUserCount = row["FollowerUserCount"] == DBNull.Value ? 0 : int.Parse(row["FollowerUserCount"].ToString()),
                    HideDisplayNameCount = row["HideDisplayNameCount"] == DBNull.Value ? 0 : int.Parse(row["HideDisplayNameCount"].ToString()),
                    Mizhong = row["Mizhong"] == DBNull.Value ? 0 : decimal.Parse(row["Mizhong"].ToString()),
                    OrderCount = row["OrderCount"] == DBNull.Value ? 0 : int.Parse(row["OrderCount"].ToString()),
                    Yingli = row["Yingli"] == DBNull.Value ? 0 : decimal.Parse(row["Yingli"].ToString()),
                    UserId = row["UserId"].ToString() == null ? "" : row["UserId"].ToString(),
                });

            }
            return list;
        }

        /// <summary>
        /// 查询某专家发布历史列表
        /// </summary>
        public List<ExperterHistorySchemeInfo> QueryExperterHistorySchemeList(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<ExperterScheme>()
                        //join o in this.Session.Query<OrderDetail>() on r.SchemeId equals o.SchemeId
                        where r.UserId == userId
                        orderby r.CreateTime descending
                        select new ExperterHistorySchemeInfo
                        {
                            SchemeId = r.SchemeId,
                            CurrentTime = r.CurrentTime,
                            CreateTime = r.CreateTime,
                            TotalMoney = r.BetMoney,
                            BonusMoney = r.BonusMoney,
                        };
            totalCount = query.Count();
            var list = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var schemeIdArray = list.Select(p => p.SchemeId).ToArray();
            var codeList = this.Session.Query<Sports_AnteCode>().Where(p => schemeIdArray.Contains(p.SchemeId)).ToList();
            foreach (var item in list)
            {
                item.MatchId = string.Join(",", codeList.Where(p => p.SchemeId == item.SchemeId).Select(p => p.MatchId).ToArray());
            }
            return list;
        }

        /// <summary>
        /// 添加名家方案支持率
        /// </summary>
        public void AddExperterSchemeSupport(ExperterSchemeSupport entity)
        {
            this.Add<ExperterSchemeSupport>(entity);
        }

        /// <summary>
        /// 查询是否已经投票
        /// </summary>
        public ExperterSchemeSupport QueryIsVoteScheme(string schemeId, string userId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterSchemeSupport>().FirstOrDefault(p => p.SchemeId == schemeId && (p.AgainstUserId == userId || p.SupportUserId == userId));
        }

        /// <summary>
        /// 查询是否支持该方案
        /// </summary>
        public ExperterSchemeSupport QueryIsVoteSupport(string schemeId, string userId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterSchemeSupport>().FirstOrDefault(p => p.SchemeId == schemeId && p.SupportUserId == userId);
        }

        /// <summary>
        /// 查询是否反对该方案
        /// </summary>
        public ExperterSchemeSupport QueryIsVoteAgainst(string schemeId, string userId)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterSchemeSupport>().FirstOrDefault(p => p.SchemeId == schemeId && p.AgainstUserId == userId);
        }

        public List<ExperterScheme> QueryExperterSchemeList(string userId, DateTime startTime, DateTime endTime)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterScheme>().Where(p => p.UserId == userId && p.CreateTime >= startTime && p.CreateTime < endTime && (p.BonusStatus == BonusStatus.Win || p.BonusStatus == BonusStatus.Lose)).ToList();
        }

        public List<Experter> QueryAllExperter()
        {
            this.Session.Clear();
            return this.Session.Query<Experter>().Where(p => p.IsEnable == true).ToList();
        }
    }
}
