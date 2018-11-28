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
    public class A20140902Manager_购彩返利 : GameBizEntityManagement
    {
        /// <summary>
        /// 购彩返利
        /// </summary>
        public void AddA20140902_购彩返利(A20140902_购彩返利 entity)
        {
            this.Add<A20140902_购彩返利>(entity);
        }

        /// <summary>
        /// 查询返利信息
        /// </summary>
        public List<A20140902_BuyLotteryRebateInfo> QueryA20140902_BuyLotteryRebate(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalOrderMoney, out decimal totalGiveMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<A20140902_购彩返利>()
                        where (userId == string.Empty || t.UserId == userId)
                        && (t.CreateTime >= starTime && t.CreateTime < endTime)
                        select new A20140902_BuyLotteryRebateInfo
                        {
                            UserId = t.UserId,
                            AddMoney = t.AddMoney,
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            IssuseNumber = t.IssuseNumber,
                            OrderMoney = t.OrderMoney,
                            PlayType = t.PlayType,
                            SchemeId = t.SchemeId,
                            SchemeType = t.SchemeType,
                            CreateTime = t.CreateTime,
                        };
            totalCount = query.Count();
            totalOrderMoney = totalCount == 0 ? 0M : query.Sum(a => a.OrderMoney);
            totalGiveMoney = totalCount == 0 ? 0M : query.Sum(a => a.AddMoney);
            query = query.OrderByDescending(a => a.CreateTime);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

    }
}
