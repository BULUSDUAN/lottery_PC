using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Activity.Domain.Entities;
using NHibernate.Linq;
using Activity.Core;
using GameBiz.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20140902Manager首冲送钱 : GameBizEntityManagement
    {
        public void AddA20140902首冲送钱(A20140902首冲送钱 entity)
        {
            this.Add<A20140902首冲送钱>(entity);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateA20140902首冲送钱(A20140902首冲送钱 entity)
        {
            this.Update<A20140902首冲送钱>(entity);
        }

        public A20140902首冲送钱 QueryA20140902首冲送钱(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20140902首冲送钱>().FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 查询首冲送钱记录
        /// </summary>
        public List<FistFillMoneyInfo> QueryFistFillMoneyList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal fillMoney, out decimal currentGiveMoney, out decimal nextGiveMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<A20140902首冲送钱>()
                        where (userId == string.Empty || t.UserId == userId)
                        && (t.CreateTime >= starTime && t.CreateTime < endTime)
                        select new FistFillMoneyInfo
                        {
                            UserId = t.UserId,
                            CurrentGiveMoney = t.CurrentGiveMoney,
                            FillMoney = t.FillMoney,
                            IsGiveComplate = t.IsGiveComplate,
                            NextGiveMoney = t.NextGiveMoney,
                            OrderId = t.OrderId,
                            CreateTime = t.CreateTime,
                        };
            totalCount = query.Count();
            fillMoney = totalCount == 0 ? 0M : query.Sum(a => a.FillMoney);
            currentGiveMoney = totalCount == 0 ? 0M : query.Sum(a => a.CurrentGiveMoney);
            nextGiveMoney = totalCount == 0 ? 0M : query.Sum(a => a.NextGiveMoney);
            query = query.OrderByDescending(a => a.CreateTime);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
