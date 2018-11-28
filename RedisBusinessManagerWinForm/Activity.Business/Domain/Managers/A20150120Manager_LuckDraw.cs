using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;
using Activity.Core;
using GameBiz.Domain.Entities;

namespace Activity.Business.Domain.Managers
{
    public class A20150120Manager_LuckDraw : GameBizEntityManagement
    {
        public void AddLuckyDraw(A20150120_LuckyDraw entity)
        {
            this.Add<A20150120_LuckyDraw>(entity);
        }
        public A20150120_LuckyDraw GetLuckyDraw(string lotteryNumber)
        {
            return Session.Query<A20150120_LuckyDraw>().FirstOrDefault(s => s.LotteryNumber == lotteryNumber);
        }
        public void UpdateLuckyDraw(A20150120_LuckyDraw entity)
        {
            this.Update<A20150120_LuckyDraw>(entity);
        }
        public void AddListJoinLuckyDraw(params A20150120_JoinLuckyDraw[] ListEntity)
        {
            this.Add<A20150120_JoinLuckyDraw>(ListEntity);
        }
        public void AddJoinLuckyDraw(A20150120_JoinLuckyDraw entity)
        {
            this.Add<A20150120_JoinLuckyDraw>(entity);
        }
        public void UpdateVirtualUserData(VirtualUserData entity)
        {
            this.Update<VirtualUserData>(entity);
        }
        public VirtualUserData GetVirtualUserData()
        {
            return Session.Query<VirtualUserData>().FirstOrDefault(s => s.IsUser == false);
        }
        public A20150120_JoinLuckyDraw GetJoinLuckyDraw(string userId)
        {
            return Session.Query<A20150120_JoinLuckyDraw>().FirstOrDefault(s => s.UserId == userId);
        }
        public A20150120_JoinLuckyDraw GetJoinLuckyDrawByNumber(string lotteryNumber)
        {
            return Session.Query<A20150120_JoinLuckyDraw>().FirstOrDefault(s => s.LotteryNumber == lotteryNumber);
        }
        public bool CheckJoinLuckDraw(string idCardNumber)
        {
            return Session.Query<A20150120_JoinLuckyDraw>().Where(s => s.IdCardNumber == idCardNumber).Count() > 0 ? true : false;
        }
        public List<A20150120LuckyDrawInfo> QueryLuckyDrawList(string lotteryNumber, string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            var sTime = startTime.Date;
            var eTime = endTime.AddDays(1).Date;
            var query = from l in Session.Query<A20150120_LuckyDraw>()
                        where (l.LotteryNumber == lotteryNumber || lotteryNumber == string.Empty) && (l.AgentId == agentId || agentId == string.Empty) && (l.CreateTime >= sTime && l.CreateTime < eTime)
                        select new A20150120LuckyDrawInfo
                        {
                            AgentId = l.AgentId,
                            BelongUserId = l.BelongUserId,
                            BelongUserName = l.BelongUserName,
                            Id = l.Id,
                            CreateTime = l.CreateTime,
                            IsUse = l.IsUse,
                            LotteryNumber = l.LotteryNumber,
                            LotteryType = l.LotteryType,
                        };
            if (query != null && query.ToList().Count > 0)
            {
                totalCount = query.ToList().Count;
                if (pageIndex == 0 && pageSize == 0)
                    return query.ToList();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            totalCount = 0;
            return new List<A20150120LuckyDrawInfo>();
        }
        public List<A20150120_JoinLuckyDrawInfo> QueryJoinLuckDraw(int returnCount)
        {
            var query = from l in Session.Query<A20150120_JoinLuckyDraw>()
                        join u in Session.Query<UserRegister>() on l.UserId equals u.UserId
                        orderby l.CreateTime descending
                        select new A20150120_JoinLuckyDrawInfo
                            {
                                Id = l.Id,
                                UserId = l.UserId,
                                PrizeMoney = l.PrizeMoney,
                                OrderId = l.OrderId,
                                AgentId = l.AgentId,
                                PrizeType = l.PrizeType,
                                IsTestUser = l.IsTestUser,
                                ClientType = l.ClientType,
                                Description = l.Description,
                                LotteryNumber = l.LotteryNumber,
                                LoginName = u.DisplayName,
                                CreateTime = l.CreateTime,

                            };
            if (query == null || query.ToList().Count <= 0)
                return new List<A20150120_JoinLuckyDrawInfo>();
            return query.Take(returnCount).ToList();
        }
    }
}
