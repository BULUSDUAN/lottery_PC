using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using NHibernate.Criterion;
using NHibernate;
using NHibernate.Linq;
using GameBiz.Core;
using GameBiz.Domain.Entities;
using Common.Business;
using GameBiz.Business;
using Common.Utilities;
using Common;

namespace GameBiz.Domain.Managers
{
    public class SchemeManager : GameBizEntityManagement
    {
        #region 实体对象操作

        public void AddOrderDetail(OrderDetail entity)
        {
            this.Add<OrderDetail>(entity);
        }

        public void UpdateOrderDetail(OrderDetail entity)
        {
            this.Update<OrderDetail>(entity);
        }

        #endregion

        public string QueryMobileNumberBySchemeId(string schemeId)
        {
            string sql = string.Format(@"select m.Mobile
                        from E_Authentication_Mobile as m
                        left join C_OrderDetail as o on m.UserId = o.UserId
                        where o.SchemeId='{0}' ", schemeId);
            return Session.CreateSQLQuery(sql).UniqueResult<string>();
        }

        public OrderDetail QueryOrderDetail(string schemeId)
        {
            Session.Clear();
            return Session.Query<OrderDetail>().FirstOrDefault(o => o.SchemeId == schemeId);
        }

        public List<OrderDetail> QueryOrderDetailList(DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            var query = from d in this.Session.Query<OrderDetail>()
                        where d.CreateTime >= startTime && d.CreateTime < endTime
                        select d;

            return query.ToList();
        }

        public List<OrderDetail> QueryOrderDetailListBySchemeId(string[] schemeIdArray)
        {
            Session.Clear();
            var query = from d in this.Session.Query<OrderDetail>()
                        where schemeIdArray.Contains(d.SchemeId)
                        select d;

            return query.ToList();
        }

        /// <summary>
        /// 查询中奖次数
        /// </summary>
        public int QueryUserWinCount(string userId, string playType)
        {
            Session.Clear();
            var query = from d in this.Session.Query<OrderDetail>()
                        where d.UserId == userId
                        && (playType == string.Empty || (d.PlayType == playType && (d.GameCode == "JCZQ" || d.GameCode == "JCLQ")))
                        && d.BonusStatus == BonusStatus.Win
                        select d;
            return query.Count();
        }

        /// <summary>
        /// 查询用户中奖总金额
        /// </summary>
        public decimal QueryUserWinMoney(string userId)
        {
            Session.Clear();
            var query = from d in this.Session.Query<OrderDetail>()
                        where d.UserId == userId
                        && d.BonusStatus == BonusStatus.Win
                        select d;
            return query.Sum(d => d.AfterTaxBonusMoney);
        }

        public System.Data.DataSet QueryBettingAnteCode(DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            var sql = string.Format(@"SELECT c.[SchemeId]
	                                ,c.[GameCode]
	                                ,c.[GameType]
	                                ,c.[PlayType]
	                                ,c.[IssuseNumber]
	                                ,c.[MatchId]
	                                ,c.[AnteCode]
	                                ,c.[IsDan]
	                                ,c.[Odds]
	                                ,c.[CreateTime]
	                                ,o.Amount
	                                ,o.TotalMoney
	                                FROM [C_Sports_AnteCode] c
	                                left join C_OrderDetail o on c.SchemeId = o.SchemeId
	                                where c.CreateTime>'{0}' and c.CreateTime<'{1}'", startTime.ToString("yyyy/MM/dd"), endTime.AddDays(1).ToString("yyyy/MM/dd"));
            var query = CreateOutputQuery(Session.CreateSQLQuery(sql));
            return query.GetDataSet();
        }

        /// <summary>
        /// 查询用户投注金额
        /// </summary>
        public decimal QueryUserBetTotalMoney(string userId)
        {
            this.Session.Clear();
            var totalMoney = 0M;

            var daigouTotalMoney = this.Session.Query<OrderDetail>().Where(p => p.SchemeType != SchemeType.TogetherBetting
                && p.TicketStatus == TicketStatus.Ticketed && p.IsVirtualOrder == false && p.UserId == userId);
            if (daigouTotalMoney.Count() > 0)
                totalMoney += daigouTotalMoney.Sum(p => p.CurrentBettingMoney);

            var togetherTotalMoney = this.Session.Query<Sports_TogetherJoin>().Where(p => p.JoinUserId == userId && p.JoinSucess == true);
            if (togetherTotalMoney.Count() > 0)
                totalMoney += togetherTotalMoney.Sum(p => p.Price * p.RealBuyCount);

            return totalMoney;
        }
        public int GetCurrDayGiveGrowth(string userId)
        {
            var query = from g in Session.Query<UserGrowthDetail>()
                        where g.Category == BusinessHelper.FundCategory_BuyGrowthValue && g.UserId == userId && (g.CreateTime >= DateTime.Now.Date && g.CreateTime < DateTime.Now.AddDays(1).Date)
                        select g.PayMoney;
            if (query != null && query.Count() > 0)
            {
                return query.Sum();
            }
            return 0;
        }
    }
}