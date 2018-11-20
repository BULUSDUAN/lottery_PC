using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Activity.Business;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;
using Activity.Core;

namespace Activity.Domain.Managers
{
    public class A20140902Manager足彩安慰奖 : GameBizEntityManagement
    {
        /// <summary>
        /// A20140902足彩安慰奖
        /// </summary>
        public void AddA20140902足彩安慰奖(A20140902足彩安慰奖 entity)
        {
            this.Add<A20140902足彩安慰奖>(entity);
        }

        /// <summary>
        /// 查询本期改用户是否参与
        /// </summary>
        public A20140902足彩安慰奖 QueryA20140902足彩安慰奖(string issuseNuber, string userId, string gameType)
        {
            this.Session.Clear();
            return this.Session.Query<A20140902足彩安慰奖>().FirstOrDefault(p => p.UserId == userId && p.IssuseNumber == issuseNuber && p.GameType == gameType);
        }

        /// <summary>
        /// 查询足彩不中也就奖记录
        /// </summary>
        public List<FootballConsolationPrizeInfo> QueryFootballConsolationPrizeList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalOrderMoney, out decimal totalGiveMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<A20140902足彩安慰奖>()
                        where (userId == string.Empty || t.UserId == userId)
                        && (t.CreateTime >= starTime && t.CreateTime < endTime)
                        select new FootballConsolationPrizeInfo
                        {
                            UserId = t.UserId,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            GiveMoney = t.GiveMoney,
                            IsGive = t.IsGive,
                            IssuseNumber = t.IssuseNumber,
                            OrderMoney = t.OrderMoney,
                            OrderId = t.OrderId,
                            CreateTime = t.CreateTime,
                        };
            totalCount = query.Count();
            totalOrderMoney = totalCount == 0 ? 0M : query.Sum(a => a.OrderMoney);
            totalGiveMoney = totalCount == 0 ? 0M : query.Sum(a => a.GiveMoney);
            query = query.OrderByDescending(a => a.CreateTime);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
