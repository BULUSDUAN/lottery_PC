using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Core;
using GameBiz.Business;
using Common.Expansion;
using Common.Utilities;
using System.Data;

namespace GameBiz.Domain.Managers
{
    public class BlogManager : GameBizEntityManagement
    {
        /// <summary>
        /// 添加最新动态
        /// </summary>
        public void AddBlog_Dynamic(Blog_Dynamic entity)
        {
            this.Add<Blog_Dynamic>(entity);
        }

        /// <summary>
        /// 查询最新动态
        /// </summary>
        public List<ProfileDynamicInfo> QueryProfileDynamicList(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<Blog_Dynamic>()
                        join u1 in this.Session.Query<UserRegister>() on r.UserId equals u1.UserId
                        join u2 in this.Session.Query<UserRegister>() on r.UserId2 equals u2.UserId
                        where (r.UserId == userId)
                        orderby r.CreateTime descending
                        select new ProfileDynamicInfo
                        {
                            UserId = r.UserId,
                            UserDisplayName = r.UserDisplayName,
                            HideDisplayNameCount = u1.HideDisplayNameCount,
                            UserId2 = r.UserId2,
                            User2DisplayName = r.User2DisplayName,
                            User2HideDisplayNameCount = u2.HideDisplayNameCount,
                            GameCode = r.GameCode,
                            GameType = r.GameType,
                            Subscription = r.Subscription,
                            DynamicType = r.DynamicType,
                            IssuseNumber = r.IssuseNumber,
                            Guarantees = r.Guarantees,
                            Progress = r.Progress,
                            SchemeId = r.SchemeId,
                            Price = r.Price,
                            TotalMonery = r.TotalMonery,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        #region 用户统计数据

        /// <summary>
        /// 添加用户统计数据
        /// </summary>
        public void AddBlog_DataReport(Blog_DataReport entity)
        {
            this.Add<Blog_DataReport>(entity);
        }

        /// <summary>
        /// 查询用户的统计数据
        /// </summary>
        public Blog_DataReport QueryBlog_DataReport(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<Blog_DataReport>().FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 更新用户数据
        /// </summary>
        public void UpdateBlog_DataReport(Blog_DataReport entity)
        {
            this.Update<Blog_DataReport>(entity);
        }
        #endregion

        #region 用户获奖记录

        /// <summary>
        /// 添加用户获奖记录
        /// </summary>
        public void AddBlog_ProfileBonusLevel(Blog_ProfileBonusLevel entity)
        {
            this.Add<Blog_ProfileBonusLevel>(entity);
        }

        /// <summary>
        /// 查询用户的获奖记录
        /// </summary>
        public Blog_ProfileBonusLevel QueryBlog_ProfileBonusLevel(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<Blog_ProfileBonusLevel>().FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 查询用户的获奖记录
        /// </summary>
        public List<ProfileBonusLevelInfo> QueryBlog_ProfileBonusLevel1(string userId)
        {
            Session.Clear();
            var query = from pb in this.Session.Query<Blog_ProfileBonusLevel>()
                        where (pb.UserId == userId)
                        select new ProfileBonusLevelInfo
                        {
                            UserId = pb.UserId,
                            MaxLevelName = pb.MaxLevelName,
                            MaxLevelValue = pb.MaxLevelValue,
                            WinHundredMillionCount = pb.WinHundredMillionCount,
                            WinOneHundredCount = pb.WinOneHundredCount,
                            WinOneHundredThousandCount = pb.WinOneHundredThousandCount,
                            WinOneMillionCount = pb.WinOneMillionCount,
                            WinOneThousandCount = pb.WinOneThousandCount,
                            WinTenMillionCount = pb.WinTenMillionCount,
                            WinTenThousandCount = pb.WinTenThousandCount,
                        };
            return query.ToList();
        }

        /// <summary>
        /// 更新获奖记录
        /// </summary>
        public void UpdateBlog_ProfileBonusLevel(Blog_ProfileBonusLevel entity)
        {
            this.Update<Blog_ProfileBonusLevel>(entity);
        }
        #endregion

        /// <summary>
        /// 添加用户最新中奖
        /// </summary>
        public void AddBlog_NewProfileLastBonus(Blog_NewProfileLastBonus entity)
        {
            this.Add<Blog_NewProfileLastBonus>(entity);
        }

        /// <summary>
        /// 查询最新新中奖
        /// </summary>
        public List<ProfileLastBonusInfo> QueryProfileLastBonusList(string userId, out int totalCount)
        {
            Session.Clear();
            var query = from r in this.Session.Query<Blog_NewProfileLastBonus>()
                        where (r.UserId == userId)
                        orderby r.BonusTime descending
                        select new ProfileLastBonusInfo
                        {
                            UserId = r.UserId,
                            GameCode = r.GameCode,
                            GameType = r.GameType,
                            IssuseNumber = r.IssuseNumber,
                            BonusMoney = r.BonusMoney,
                            SchemeId = r.SchemeId,
                            BonusTime = r.BonusTime,
                        };
            totalCount = query.Count();
            return query.Take(10).ToList();
        }

        #region 用户登陆历史
        /// <summary>
        /// 添加信息
        /// </summary>
        public void AddBlog_UserLoginHistory(Blog_UserLoginHistory entity)
        {
            this.Add<Blog_UserLoginHistory>(entity);
        }
        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateBlog_UserLoginHistory(Blog_UserLoginHistory entity)
        {
            this.Update<Blog_UserLoginHistory>(entity);
        }
        /// <summary>
        /// 查询
        /// </summary>
        public List<UserLoginHistoryInfo> QueryBlog_UserLoginHistory(string userId)
        {
            Session.Clear();
            var query = from pb in this.Session.Query<Blog_UserLoginHistory>()
                        where (pb.UserId == userId)
                        orderby pb.LoginTime descending
                        select new UserLoginHistoryInfo
                        {
                            UserId = pb.UserId,
                            Id = pb.Id,
                            LoginFrom = pb.LoginFrom,
                            IpDisplayName = pb.IpDisplayName,
                            LoginIp = pb.LoginIp,
                            LoginTime = pb.LoginTime
                        };
            return query.Take(10).ToList();
        }

        public UserLoginHistoryInfo QueryLastLoginInfo(string userId)
        {
            Session.Clear();
            var query = from pb in this.Session.Query<Blog_UserLoginHistory>()
                        where (pb.UserId == userId)
                        orderby pb.LoginTime descending
                        select new UserLoginHistoryInfo
                        {
                            UserId = pb.UserId,
                            Id = pb.Id,
                            LoginFrom = pb.LoginFrom,
                            IpDisplayName = pb.IpDisplayName,
                            LoginIp = pb.LoginIp,
                            LoginTime = pb.LoginTime
                        };
            return query.FirstOrDefault();
        }

        #endregion


        #region 访客历史记录

        /// <summary>
        /// 添加访客历史记录
        /// </summary>
        public void AddBlog_UserVisitHistory(Blog_UserVisitHistory entity)
        {
            this.Add<Blog_UserVisitHistory>(entity);
        }

        public void UpdateBlog_UserVisitHistory(Blog_UserVisitHistory entity)
        {
            this.Update<Blog_UserVisitHistory>(entity);
        }
        public Blog_UserVisitHistory QueryBlog_UserVisitHistory(string userId, string visitorId)
        {
            Session.Clear();
            return this.Session.Query<Blog_UserVisitHistory>().FirstOrDefault(p => p.UserId == userId && p.VisitUserId == visitorId);
        }

        /// <summary>
        /// 查询访客历史记录
        /// </summary>
        public List<ProfileVisitHistoryInfo> QueryBlog_UserVisitHistory(string userId)
        {
            Session.Clear();
            var query = from pb in this.Session.Query<Blog_UserVisitHistory>()
                        join r in this.Session.Query<Blog_ProfileBonusLevel>() on pb.VisitUserId equals r.UserId
                        where (pb.UserId == userId)
                        orderby pb.CreateTime descending
                        select new ProfileVisitHistoryInfo
                        {
                            UserId = pb.UserId,
                            MaxLevelName = r.MaxLevelName,
                            IpDisplayName = pb.IpDisplayName,
                            VisitUserId = pb.VisitUserId,
                            VisitorHideNameCount = pb.VisitorHideNameCount,
                            VisitorUserDisplayName = pb.VisitorUserDisplayName,
                            VisitorIp = pb.VisitorIp,
                            CreateTime = DateTime.Now,
                        };
            return query.Take(10).ToList();
        }

        #endregion

        #region 普通用户推广

        /// <summary>
        /// 添加普通用户推广
        /// </summary>
        public void AddBlog_UserSpread(Blog_UserSpread entity)
        {
            this.Add<Blog_UserSpread>(entity);
        }

        /// <summary>
        /// 查询普通用户推广
        /// </summary>
        public Blog_UserSpread QueryBlog_UserSpread(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<Blog_UserSpread>().FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 更新普通用户推广
        /// </summary>
        public void UpdateBlog_UserSpread(Blog_UserSpread entity)
        {
            this.Update<Blog_UserSpread>(entity);
        }

        /// <summary>
        /// 查询普通用户推广
        /// </summary>
        public List<Blog_UserSpread> QueryBlog_UserSpreadList(string userId, int pageIndex, int pageSize, DateTime begin, DateTime end, out int totalCount)
        {
            Session.Clear();
            var query = from r in this.Session.Query<Blog_UserSpread>()
                        where (r.AgentId == userId && r.CrateTime <= end && r.CrateTime >= begin)
                        orderby r.CrateTime descending
                        select new Blog_UserSpread
                        {
                            UserId = r.UserId,
                            userName = r.userName,
                            AgentId = r.AgentId,
                            CrateTime = r.CrateTime,
                            CTZQ = r.CTZQ,
                            BJDC = r.BJDC,
                            JCZQ = r.JCZQ,
                            JCLQ = r.JCLQ,
                            SZC = r.SZC,
                            GPC = r.GPC,
                            UpdateTime = r.UpdateTime
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        #endregion

        #region "普通用户推广领红包"
        /// <summary>
        /// 新增普通用户推广领红包
        /// </summary>
        public void AddBlog_UserSpreadGiveRedBag(Blog_UserSpreadGiveRedBag entity)
        {
            this.Add<Blog_UserSpreadGiveRedBag>(entity);
        }

        /// <summary>
        /// 查询普通用户推广领红包
        /// </summary>
        public Blog_UserSpreadGiveRedBag QueryBlog_UserSpreadGiveRedBag(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<Blog_UserSpreadGiveRedBag>().FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 更新普通用户推广领红包
        /// </summary>
        public void UpdateBlog_UserSpreadGiveRedBag(Blog_UserSpreadGiveRedBag entity)
        {
            this.Update<Blog_UserSpreadGiveRedBag>(entity);
        }
        #endregion


        #region "fxid分享推广"
        /// <summary>
        /// 新增fxid分享推广
        /// </summary>
        /// <param name="entity"></param>
        public void AddBlog_UserShareSpread(Blog_UserShareSpread entity)
        {
            this.Add<Blog_UserShareSpread>(entity);
        }
        /// <summary>
        /// 查询fxid分享推广
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Blog_UserShareSpread QueryBlog_UserShareSpread(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<Blog_UserShareSpread>().FirstOrDefault(p => p.UserId == userId);
        }
        /// <summary>
        /// 更新fxid分享推广
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateBlog_UserShareSpread(Blog_UserShareSpread entity)
        {
            this.Update<Blog_UserShareSpread>(entity);
        }

        /// <summary>
        /// 查询fxid分享推广
        /// </summary>
        public List<Blog_UserShareSpread> QueryBlog_UserShareSpreadList(string userId, int pageIndex, int pageSize, DateTime begin, DateTime end, out int userTotalCount, out decimal RedBagMoneyTotal)
        {
            Session.Clear();
            var query = from r in this.Session.Query<Blog_UserShareSpread>()
                        join u in Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where (r.AgentId == userId)//&& r.CreateTime <= end && r.CreateTime >= begin)
                        select new Blog_UserShareSpread
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            userName = u.DisplayName,
                            AgentId = r.AgentId,
                            isGiveLotteryRedBag = r.isGiveLotteryRedBag,
                            isGiveRegisterRedBag = r.isGiveRegisterRedBag,
                            giveRedBagMoney = r.giveRedBagMoney,
                            CreateTime = r.CreateTime,
                            UpdateTime = r.UpdateTime
                        };
            if (query != null && query.Count() > 0)
            {
                userTotalCount = query.Count();//总人数
                RedBagMoneyTotal = query.Sum(g => g.giveRedBagMoney);//总红包金额
                return query.OrderByDescending(p => p.UpdateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            userTotalCount = 0;
            RedBagMoneyTotal = 0;
            return new List<Blog_UserShareSpread>();
        }

        #endregion

        #region 根据分享订单送红包
        public BlogOrderShareRegisterRedBag QueryBlog_OrderShareRegisterRedBag(string schemeId, string userId) 
        {
            this.Session.Clear();
            return this.Session.Query<BlogOrderShareRegisterRedBag>().FirstOrDefault(p => p.UserId == userId && p.SchemeId == schemeId);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateBlog_OrderShareRegisterRedBag(BlogOrderShareRegisterRedBag entity) 
        {
            this.Update<BlogOrderShareRegisterRedBag>(entity);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        public void Add_OrderShareRegisterRedBag(BlogOrderShareRegisterRedBag entity)
        {
            this.Add<BlogOrderShareRegisterRedBag>(entity);
        }
        #endregion
    }
}
