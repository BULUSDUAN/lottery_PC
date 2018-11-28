using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using Activity.Business;
using Activity.Domain.Entities;
using GameBiz.Business;
using Activity.Core;

namespace Activity.Domain.Managers
{
    public class A20140731_DouDouManager : GameBizEntityManagement
    {
        public void AddA20140731_DouDou(A20140731_DouDou entity)
        {
            this.Add<A20140731_DouDou>(entity);
        }

        /// <summary>
        /// 按ID查询豆豆兑换记录
        /// </summary>
        public A20140731_DouDou QueryDouDouById(int id)
        {
            this.Session.Clear();
            return this.Session.Query<A20140731_DouDou>().FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 更新查询豆豆兑换记录
        /// </summary>
        public void UpdateA20140731_DouDou(A20140731_DouDou entity)
        {
            this.Update<A20140731_DouDou>(entity);
        }

        /// <summary>
        /// 查询豆豆兑换记录
        /// </summary>
        public List<A20140731_DouDouInfo> QueryDouDouList(bool isGive, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<A20140731_DouDou>()
                        where (r.IsGive == isGive)
                        orderby r.CreateTime descending
                        select new A20140731_DouDouInfo
                        {
                            UserId = r.UserId,
                            ActivityMoney = r.ActivityMoney,
                            DouDou = r.DouDou,
                            IsGive = r.IsGive,
                            Money = r.Money,
                            Prize = r.Prize,
                            PrizeMoney = r.PrizeMoney,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

    }
}
