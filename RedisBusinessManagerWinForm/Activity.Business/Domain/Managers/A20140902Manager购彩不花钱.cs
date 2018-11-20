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
using GameBiz.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20140902Manager购彩不花钱 : GameBizEntityManagement
    {
        /// <summary>
        /// A20140902购彩不花钱
        /// </summary>
        public void AddA20140902购彩不花钱(A20140902购彩不花钱 entity)
        {
            this.Add<A20140902购彩不花钱>(entity);
        }

        /// <summary>
        /// 查询今日条数
        /// </summary>
        public int QueryA20140902购彩不花钱()
        {
            this.Session.Clear();
            return this.Session.Query<A20140902购彩不花钱>().Where(p => p.CurrentTime == DateTime.Now.ToString("yyyMMdd")).Count();
        }

        /// <summary>
        /// 该用户今天是否参与
        /// </summary>
        public int QueryUserIsParticipation(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20140902购彩不花钱>().Where(p => p.UserId == userId && p.CurrentTime == DateTime.Now.ToString("yyyMMdd")).Count();
        }

        /// <summary>
        /// 查询用户未得到红包
        /// </summary>
        public List<A20140902Info> QueryGiveRedPageInfo(string currentTime)
        {
            Session.Clear();
            var query = from r in this.Session.Query<A20140902购彩不花钱>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where (r.IsGive == true && r.CurrentTime == currentTime)
                        orderby r.CreateTime descending
                        select new A20140902Info
                        {
                            UserId = r.UserId,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            FillMoney = r.FillMoney,
                            IsGive = r.IsGive,
                            OrderId = r.OrderId,
                            CurrentTime = r.CurrentTime,
                            CreateTime = r.CreateTime,
                        };
            return query.ToList();
        }

        /// <summary>
        /// 购彩不花钱
        /// </summary>
        public List<BuyLotteryNoMoneyInfo> QueryBuyLotteryNoMoneyList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalOrderMoney, out decimal totalGiveMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<A20140902购彩不花钱>()
                        join u in this.Session.Query<UserRegister>() on t.UserId equals u.UserId
                        where (userId == string.Empty || t.UserId == userId)
                        && (t.CreateTime >= starTime && t.CreateTime < endTime)
                        select new BuyLotteryNoMoneyInfo
                        {
                            UserId = t.UserId,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            CurrentTime = t.CurrentTime,
                            FillMoney = t.FillMoney,
                            OrderMoney = t.OrderMoney,
                            IsGive = t.IsGive,
                            OrderId = t.OrderId,
                            CreateTime = t.CreateTime,
                        };
            totalCount = query.Count();
            totalOrderMoney = totalCount == 0 ? 0M : query.Sum(a => a.OrderMoney);
            totalGiveMoney = totalCount == 0 ? 0M : query.Sum(a => a.FillMoney);
            query = query.OrderByDescending(a => a.CreateTime);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
