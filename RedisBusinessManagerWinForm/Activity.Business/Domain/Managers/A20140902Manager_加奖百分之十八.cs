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
    public class A20140902Manager_加奖百分之十八 : GameBizEntityManagement
    {
        /// <summary>
        /// 疯狂加奖最高加奖百分之十八
        /// </summary>
        public void AddA20140902_加奖百分之十八(A20140902_加奖百分之十八 entity)
        {
            this.Add<A20140902_加奖百分之十八>(entity);
        }

        /// <summary>
        /// 查询疯狂加奖最高加奖百分之十八记录
        /// </summary>
        public List<AddMoneryEighteenPercentInfo> QueryA20140902_AddMoneryEighteenPercent(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalOrderMoney, out decimal totalGiveMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<A20140902_加奖百分之十八>()
                        where (userId == string.Empty || t.UserId == userId)
                        && (t.CreateTime >= starTime && t.CreateTime < endTime)
                        select new AddMoneryEighteenPercentInfo
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


        public A20140902_加奖百分之十八 QueryRecord(string userId, decimal addMoney)
        {
            this.Session.Clear();
            return this.Session.Query<A20140902_加奖百分之十八>().FirstOrDefault(p => p.UserId == userId && p.AddMoney == addMoney);
        }
    }
}
