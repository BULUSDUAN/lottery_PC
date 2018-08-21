using EntityModel;
using GameBiz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class BlogManager:DBbase
    {
        /// <summary>
        /// 初始化用户获奖记录
        /// </summary>
        /// <param name="BlogProfileBonusLevel"></param>
        public void AddBlog_ProfileBonusLevel(E_Blog_ProfileBonusLevel BlogProfileBonusLevel)
        {

            DB.GetDal<E_Blog_ProfileBonusLevel>().Add(BlogProfileBonusLevel);
        }

        /// <summary>
        /// 用户数据统计
        /// </summary>
        /// <param name="BlogProfileBonusLevel"></param>
        public void AddBlog_DataReport(E_Blog_DataReport BlogDataReport)
        {

            DB.GetDal<E_Blog_DataReport>().Add(BlogDataReport);
        }


       
            /// <summary>
            /// 添加最新动态
            /// </summary>
            public void AddBlog_Dynamic(E_Blog_Dynamic entity)
            {
            DB.GetDal<E_Blog_Dynamic>().Add(entity);
            }

            /// <summary>
            /// 查询最新动态
            /// </summary>
            public List<ProfileDynamicInfo> QueryProfileDynamicList(string userId, int pageIndex, int pageSize, out int totalCount)
            {
                
                pageIndex = pageIndex < 0 ? 0 : pageIndex;
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

                var query = from r  in DB.CreateQuery<E_Blog_Dynamic>()
                            join u1 in DB.CreateQuery<C_User_Register>() on r.UserId equals u1.UserId
                            join u2 in DB.CreateQuery<C_User_Register>() on r.UserId2 equals u2.UserId
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
            /// 查询用户的统计数据
            /// </summary>
            public E_Blog_DataReport QueryBlog_DataReport(string userId)
            {
               
                return DB.CreateQuery<E_Blog_DataReport>().Where(p => p.UserId == userId).FirstOrDefault();
            }

            /// <summary>
            /// 更新用户数据
            /// </summary>
            public void UpdateBlog_DataReport(E_Blog_DataReport entity)
            {
            DB.GetDal<E_Blog_DataReport>().Update(entity);
            }
            #endregion

            #region 用户获奖记录

         

            /// <summary>
            /// 查询用户的获奖记录
            /// </summary>
            public E_Blog_ProfileBonusLevel QueryBlog_ProfileBonusLevel(string userId)
            {
              
                return DB.CreateQuery<E_Blog_ProfileBonusLevel>().Where(p => p.UserId == userId).FirstOrDefault();
            }

            /// <summary>
            /// 查询用户的获奖记录
            /// </summary>
            public List<ProfileBonusLevelInfo> QueryBlog_ProfileBonusLevel1(string userId)
            {
              
                var query = from pb in DB.CreateQuery<E_Blog_ProfileBonusLevel>()
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
            public void UpdateBlog_ProfileBonusLevel(E_Blog_ProfileBonusLevel entity)
            {
            DB.GetDal<E_Blog_ProfileBonusLevel>().Update(entity);
            }
            #endregion

            /// <summary>
            /// 添加用户最新中奖
            /// </summary>
            public void AddBlog_NewProfileLastBonus(E_Blog_NewProfileLastBonus entity)
            {
            DB.GetDal<E_Blog_NewProfileLastBonus>().Add(entity);
            }

            /// <summary>
            /// 查询最新新中奖
            /// </summary>
            public List<ProfileLastBonusInfo> QueryProfileLastBonusList(string userId, out int totalCount)
            {
          
                var query = from r in DB.CreateQuery<E_Blog_NewProfileLastBonus>()
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
            public void AddBlog_UserLoginHistory(E_Blog_UserLoginHistory entity)
            {
            DB.GetDal<E_Blog_UserLoginHistory>().Add(entity);
            }
            /// <summary>
            /// 更新信息
            /// </summary>
            /// <param name="entity"></param>
            public void UpdateBlog_UserLoginHistory(E_Blog_UserLoginHistory entity)
            {
            DB.GetDal<E_Blog_UserLoginHistory>().Update(entity);
            }
            /// <summary>
            /// 查询
            /// </summary>
            public List<UserLoginHistoryInfo> QueryBlog_UserLoginHistory(string userId)
            {
             
                var query = from pb in DB.CreateQuery<E_Blog_UserLoginHistory>()
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
               
                var query = from pb in DB.CreateQuery<E_Blog_UserLoginHistory>()
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
            public void AddBlog_UserVisitHistory(E_Blog_UserVisitHistory entity)
            {
            DB.GetDal<E_Blog_UserVisitHistory>().Add(entity);
            }

            public void UpdateBlog_UserVisitHistory(E_Blog_UserVisitHistory entity)
            {
            DB.GetDal<E_Blog_UserVisitHistory>().Update(entity);
            }
            public E_Blog_UserVisitHistory QueryBlog_UserVisitHistory(string userId, string visitorId)
            {
              
                return DB.CreateQuery<E_Blog_UserVisitHistory>().Where(p => p.UserId == userId && p.VisitUserId == visitorId).FirstOrDefault();
            }

            /// <summary>
            /// 查询访客历史记录
            /// </summary>
            public List<ProfileVisitHistoryInfo> QueryBlog_UserVisitHistory(string userId)
            {
               
                var query = from pb in DB.CreateQuery<E_Blog_UserVisitHistory>()
                            join r in DB.CreateQuery<E_Blog_ProfileBonusLevel>() on pb.VisitUserId equals r.UserId
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
            public void AddBlog_UserSpread(E_Blog_UserSpread entity)
            {
            DB.GetDal<E_Blog_UserSpread>().Add(entity);
            }

            /// <summary>
            /// 查询普通用户推广
            /// </summary>
            public E_Blog_UserSpread QueryBlog_UserSpread(string userId)
            {
                
                return DB.CreateQuery<E_Blog_UserSpread>().Where(p => p.UserId == userId).FirstOrDefault();
            }

            /// <summary>
            /// 更新普通用户推广
            /// </summary>
            public void UpdateBlog_UserSpread(E_Blog_UserSpread entity)
            {
            DB.GetDal<E_Blog_UserSpread>().Update(entity);
            }

            /// <summary>
            /// 查询普通用户推广
            /// </summary>
            public List<E_Blog_UserSpread> QueryBlog_UserSpreadList(string userId, int pageIndex, int pageSize, DateTime begin, DateTime end, out int totalCount)
            {
              
                var query = from r in DB.CreateQuery<E_Blog_UserSpread>()
                            where (r.AgentId == userId && r.CrateTime <= end && r.CrateTime >= begin)
                            orderby r.CrateTime descending
                            select new E_Blog_UserSpread
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
            public void AddBlog_UserSpreadGiveRedBag(E_Blog_UserSpreadGiveRedBag entity)
            {
            DB.GetDal<E_Blog_UserSpreadGiveRedBag>().Add(entity);
            }

            /// <summary>
            /// 查询普通用户推广领红包
            /// </summary>
            public E_Blog_UserSpreadGiveRedBag QueryBlog_UserSpreadGiveRedBag(string userId)
            {
               
                return DB.CreateQuery<E_Blog_UserSpreadGiveRedBag>().Where(p => p.UserId == userId).FirstOrDefault();
            }

            /// <summary>
            /// 更新普通用户推广领红包
            /// </summary>
            public void UpdateBlog_UserSpreadGiveRedBag(E_Blog_UserSpreadGiveRedBag entity)
            {
            DB.GetDal<E_Blog_UserSpreadGiveRedBag>().Update(entity);
            }
            #endregion


            #region "fxid分享推广"
            /// <summary>
            /// 新增fxid分享推广
            /// </summary>
            /// <param name="entity"></param>
            public void AddBlog_UserShareSpread(E_Blog_UserShareSpread entity)
            {
            DB.GetDal<E_Blog_UserShareSpread>().Add(entity);
            }
            /// <summary>
            /// 查询fxid分享推广
            /// </summary>
            /// <param name="userId"></param>
            /// <returns></returns>
            public E_Blog_UserShareSpread QueryBlog_UserShareSpread(string userId)
            {
              
                return DB.CreateQuery<E_Blog_UserShareSpread>().FirstOrDefault(p => p.UserId == userId);
            }
            /// <summary>
            /// 更新fxid分享推广
            /// </summary>
            /// <param name="entity"></param>
            public void UpdateBlog_UserShareSpread(E_Blog_UserShareSpread entity)
            {
                DB.GetDal<E_Blog_UserShareSpread>().Update(entity);
            }

            /// <summary>
            /// 查询fxid分享推广
            /// </summary>
            public List<Blog_UserShareSpread> QueryBlog_UserShareSpreadList(string userId, int pageIndex, int pageSize, DateTime begin, DateTime end, out int userTotalCount, out decimal RedBagMoneyTotal)
            {
              
                var query = from r in DB.CreateQuery<E_Blog_UserShareSpread>()
                            join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
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
            public E_Blog_OrderShareRegisterRedBag QueryBlog_OrderShareRegisterRedBag(string schemeId, string userId)
            {
                
                return DB.CreateQuery<E_Blog_OrderShareRegisterRedBag>().Where(p => p.UserId == userId && p.SchemeId == schemeId).FirstOrDefault();
            }

            /// <summary>
            /// 更新
            /// </summary>
            /// <param name="entity"></param>
            public void UpdateBlog_OrderShareRegisterRedBag(E_Blog_OrderShareRegisterRedBag entity)
            {
            DB.GetDal<E_Blog_OrderShareRegisterRedBag>().Update(entity);
            }

            /// <summary>
            /// 新增
            /// </summary>
            /// <param name="entity"></param>
            public void Add_OrderShareRegisterRedBag(E_Blog_OrderShareRegisterRedBag entity)
            {
            DB.GetDal<E_Blog_OrderShareRegisterRedBag>().Add(entity);
            }
            #endregion
        }
    
}
