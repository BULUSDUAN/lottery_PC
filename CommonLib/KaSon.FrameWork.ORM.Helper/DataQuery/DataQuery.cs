﻿using EntityModel;
using GameBiz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class DataQuery : DBbase
    {
        #region 获取当前奖期
        /// <summary>
        /// 获取当前奖期
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public Issuse_QueryInfo QueryCurrentIssuse(string gameCode, string gameType = "")
        {
            var query = from i in DB.CreateQuery<C_Game_Issuse>()
                        where i.GameCode == gameCode
                        && (string.IsNullOrEmpty(gameType) || i.GameType == gameType)
                        && i.Status == (int)IssuseStatus.OnSale
                        && i.LocalStopTime > DateTime.Now
                        orderby i.LocalStopTime ascending
                        select i;
            return query.ToList().Select(p => new Issuse_QueryInfo
            {
                CreateTime = p.CreateTime,
                GameCode_IssuseNumber = p.GameCode_IssuseNumber,
                GatewayStopTime = p.GatewayStopTime,
                IssuseNumber = p.IssuseNumber,
                LocalStopTime = p.LocalStopTime,
                OfficialStopTime = p.OfficialStopTime,
                StartTime = p.StartTime,
                Status = (IssuseStatus)p.Status,
                WinNumber = p.WinNumber,
                Game = new GameInfo()
                {
                    GameCode = p.GameCode
                }
            }).FirstOrDefault();
        }
        #endregion

        #region 前台查询广告信息
        /// <summary>
        /// 前台查询广告信息
        /// </summary>
        /// <param name="bannerType"></param>
        /// <param name="returnRecord"></param>
        /// <returns></returns>
        public SiteMessageBannerInfo_Collection QuerySitemessageBanngerList_Web(BannerType bannerType, int returnRecord = 10)
        {
            var query = from s in DB.CreateQuery<E_Sitemessage_Banner>()
                        where s.BannerType == (int)bannerType && s.IsEnable == true
                        orderby s.BannerIndex ascending
                        select new SiteMessageBannerInfo
                        {
                            BannerId = s.BannerId,
                            BannerIndex = s.BannerIndex,
                            BannerTitle = s.BannerTitle,
                            BannerType = (BannerType)s.BannerType,
                            CreateTime = s.CreateTime,
                            ImageUrl = s.ImageUrl,
                            IsEnable = s.IsEnable,
                            JumpUrl = s.JumpUrl,
                        };
            SiteMessageBannerInfo_Collection collection = new SiteMessageBannerInfo_Collection();
            collection.ListInfo = null;
            if (query != null)
            {
                collection.ListInfo = query.Take(returnRecord).ToList();
            }
            return collection;
        }
        #endregion

        #region 查询文章列表
        public ArticleInfo_QueryCollection QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            string QueryArticleList_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryArticleList").SQL;
            var collection = DB.CreateSQLQuery(QueryArticleList_sql)
              .SetString("@Key", key)
              .SetString("@GameCode", gameCode)
              .SetString("@Category", category)
              .SetInt("@PageIndex", pageIndex)
              .SetInt("@PageSize", pageSize).List<ArticleInfo_Query>();

            var returnmodel = new ArticleInfo_QueryCollection();
            returnmodel.ArticleList = collection;
            string QueryArticleCount_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryArticleList_TotalCount").SQL;
            var TotalCount = DB.CreateSQLQuery(QueryArticleList_sql)
             .SetString("@Key", key)
             .SetString("@GameCode", gameCode)
             .SetString("@Category", category)
             .First<int>();
            returnmodel.TotalCount = TotalCount;
            return returnmodel;
        }
        #endregion

        #region 文章详情相关
        /// <summary>
        /// 根据Id查询文章详情
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public E_SiteMessage_Article_List GetArticleById(string articleId)
        {
            return DB.CreateQuery<E_SiteMessage_Article_List>().Where(p => p.Id == articleId).FirstOrDefault();
        }

        /// <summary>
        /// 更新查看人数
        /// </summary>
        /// <param name="article"></param>
        public void UpdateArticle(E_SiteMessage_Article_List article)
        {
            article.UpdateTime = DateTime.Now;
            DB.GetDal<E_SiteMessage_Article_List>().Update(article);
        }
        #endregion

        #region 查询公告
        /// <summary>
        /// 查询前台显示的公告列表
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public BulletinInfo_Collection QueryDisplayBulletins(BulletinAgent agent, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            string QueryArticleList_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryBulletinList_Web").SQL;
            var collection = DB.CreateSQLQuery(QueryArticleList_sql)
              .SetInt("@BulletinAgent", (int)agent)
              .SetInt("@PageIndex", pageIndex)
              .SetInt("@PageSize", pageSize).List<BulletinInfo_Query>().ToList();

            var returnmodel = new BulletinInfo_Collection();
            returnmodel.BulletinList = collection;

            string QueryArticleList_Count_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryBulletinList_Web_Total").SQL;
            var totalCount = DB.CreateSQLQuery(QueryArticleList_sql)
              .SetInt("@BulletinAgent", (int)agent).First<int>();
            returnmodel.TotalCount = totalCount;
            return returnmodel;
            //string QueryArticleCount_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryArticleList_TotalCount").SQL;
            //var list = CreateOutputQuery(Session.GetNamedQuery("Data_QueryBulletinList_Web"))
            //    .AddInParameter("BulletinAgent", (int)agent)
            //    .AddInParameter("PageIndex", pageIndex)
            //    .AddInParameter("PageSize", pageSize)
            //    .AddOutParameter("TotalCount", "Int32")
            //    .List(out outputs);
            //totalCount = (int)outputs["TotalCount"];
            //return list;
        }
        #endregion

        #region 提交建议
        public void SubmitUserIdea(UserIdeaInfo_Add userIdea)
        {
            var userIdeaInfo = new E_SiteMessage_UserIdea_List
            {
                Description = userIdea.Description,
                Category = userIdea.Category,
                Status = userIdea.Status,
                CreateUserId = userIdea.CreateUserId,
                CreateUserDisplayName = userIdea.CreateUserDisplayName,
                IsAnonymous = userIdea.IsAnonymous,
                UpdateUserId = userIdea.CreateUserId,
                CreateTime = DateTime.Now,
                CreateUserMoibile = userIdea.CreateUserMoibile,
                UpdateTime = DateTime.Now,
                UpdateUserDisplayName = userIdea.CreateUserDisplayName,
                PageOpenSpeed = userIdea.PageOpenSpeed,
                InterfaceBeautiful = userIdea.InterfaceBeautiful,
                ComposingReasonable = userIdea.ComposingReasonable,
                OperationReasonable = userIdea.OperationReasonable,
                ContentConveyDistinct = userIdea.ContentConveyDistinct
            };
            DB.GetDal<E_SiteMessage_UserIdea_List>().Add(userIdeaInfo);
        }
        #endregion

        #region 查询活动列表
        /// <summary>
        /// 活动列表
        /// </summary>
        public ActivityListInfoCollection QueryActivInfoList(int pageIndex, int pageSize)
        {
            var query = from a in DB.CreateQuery<E_ActivityList>()
                        orderby a.CreateTime descending
                        select new ActivityListInfo
                        {
                            ActivityIndex = a.ActivityIndex,
                            ImageUrl = a.ImageUrl,
                            IsShow = a.IsShow,
                            ActiveName = a.ActiveName,
                            LinkUrl = a.LinkUrl,
                            Title = a.Title,
                            Summary = a.Summary,
                            BeginTime = a.BeginTime,
                            EndTime = a.EndTime,
                            CreateTime = a.CreateTime
                        };
            var totalCount = query.Count();
            var list = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var returnModel = new ActivityListInfoCollection();
            if (list != null && list.Count > 0)
            {
                returnModel.List.AddRange(list);
            }
            returnModel.TotalCount = totalCount;
            return returnModel;
        }
        #endregion

        #region LotteryGameManager/BJDCMatchManager/CacheDataBusiness/UserIntegralManager
        public GameIssuse QueryCurrentNewIssuseInfo(string gameCode, string gameType)
        {
            var sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryCurrentNewIssuseInfo").SQL;
            var model = DB.CreateSQLQuery(sql)
              .SetString("@GameCode", gameCode)
              .SetString("@GameType", gameType)
              .List<GameIssuse>().FirstOrDefault();
            return model;
        }
        /// <summary>
        /// 北京单场场次信息
        /// </summary>
        /// <returns></returns>
        public BJDCIssuseInfo QueryBJDCCurrentIssuseInfo()
        {
            var query = from b in DB.CreateQuery<C_BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select b;

            return query.ToList().Select(p => new BJDCIssuseInfo
            {
                IssuseNumber = p.IssuseNumber,
                MinLocalStopTime = p.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MinMatchStartTime = p.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据公告Id查询公告信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BulletinInfo_Query QueryBulletinDetailById(long id)
        {
            var query = from b in DB.CreateQuery<E_SiteMessage_Bulletin_List>()
                        join u_create in DB.CreateQuery<C_User_Register>() on b.CreateBy equals u_create.UserId
                        join u_update in DB.CreateQuery<C_User_Register>() on b.CreateBy equals u_update.UserId
                        where b.Id == id
                        select new { b, u_create, u_update };

            return query.ToList().Select(t => new BulletinInfo_Query
            {
                Id = t.b.Id,
                Title = t.b.Title,
                Content = t.b.Content,
                Status = (EnableStatus)t.b.Status,
                Priority = t.b.Priority,
                IsPutTop = t.b.IsPutTop,
                EffectiveFrom = t.b.EffectiveFrom,
                EffectiveTo = t.b.EffectiveTo,
                CreateTime = t.b.CreateTime,
                CreateBy = t.b.CreateBy,
                CreatorDisplayName = t.u_create.DisplayName,
                UpdateTime = t.b.UpdateTime,
                UpdateBy = t.b.UpdateBy,
                UpdatorDisplayName = t.u_update.DisplayName,
                BulletinAgent = (BulletinAgent)t.b.BulletinAgent
            }).FirstOrDefault();
        }


        public List<APPConfigInfo> QueryAppConfigList()
        {
            return (from a in DB.CreateQuery<C_App_Config>()
                    select a).ToList().Select(a => new APPConfigInfo
                    {
                        AppAgentId = string.IsNullOrEmpty(a.AppAgentId) ? string.Empty : a.AppAgentId,
                        AgentName = string.IsNullOrEmpty(a.AgentName) ? string.Empty : a.AgentName,
                        ConfigCode = string.IsNullOrEmpty(a.ConfigCode) ? string.Empty : a.ConfigCode,
                        ConfigDownloadUrl = string.IsNullOrEmpty(a.ConfigDownloadUrl) ? string.Empty : a.ConfigDownloadUrl,
                        ConfigExtended = string.IsNullOrEmpty(a.ConfigExtended) ? string.Empty : a.ConfigExtended,
                        ConfigName = string.IsNullOrEmpty(a.ConfigName) ? string.Empty : a.ConfigName,
                        ConfigUpdateContent = string.IsNullOrEmpty(a.ConfigUpdateContent) ? string.Empty : a.ConfigUpdateContent,
                        IsForcedUpgrade = a.IsForcedUpgrade == null ? false : a.IsForcedUpgrade,
                        ConfigVersion = string.IsNullOrEmpty(a.ConfigVersion) ? string.Empty : a.ConfigVersion
                    }).ToList();
        }

        public C_App_Config QueryAppConfigByAgentId(string appAgentId)
        {
            return DB.CreateQuery<C_App_Config>().FirstOrDefault(p => p.AppAgentId == appAgentId);
        }
        #endregion

        public List<C_APP_NestedUrlConfig> QueryNestedUrlList()
        {
            return DB.CreateQuery<C_APP_NestedUrlConfig>().Where(s => s.IsEnable == true).ToList();
        }

        public SiteMessageInnerMailListNew_Collection QueryInnerMailListByReceiver(string userId, int pageIndex, int pageSize)
        {
            SiteMessageInnerMailListNew_Collection collection = new SiteMessageInnerMailListNew_Collection();
            collection.TotalCount = 0;
            //var query = from m in Session.Query<SiteMessageInnerMailListNew>()
            //            where m.ReceiverId == "U:"+userId
            //            select new SiteMessageInnerMailListNewInfo
            //                {
            //                    HandleType = m.HandleType,
            //                    MailId = m.MailId,
            //                    MsgContent = m.MsgContent,
            //                    ReadTime = m.ReadTime,
            //                    ReceiverId = m.ReceiverId,
            //                    SenderId = m.SenderId,
            //                    SendTime = m.SendTime,
            //                    Title = m.Title,
            //                };
            var query = from m in DB.CreateQuery<E_SiteMessage_InnerMail_List_new>()
                        where (m.ReceiverId == userId || m.ReceiverId == "U:" + userId)
                        && m.HandleType != (int)InnerMailHandleType.Deleted
                        select m;
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.MailList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(m => new SiteMessageInnerMailListNewInfo
                {
                    HandleType = (InnerMailHandleType)m.HandleType,
                    MailId = m.MailId,
                    MsgContent = m.MsgContent,
                    ReadTime = m.ReadTime,
                    ReceiverId = m.ReceiverId,
                    SenderId = m.SenderId,
                    SendTime = m.SendTime,
                    Title = m.Title,
                }).ToList();
            }
            return collection;
        }

        /// <summary>
        /// 根据接收人查询邮件列表
        /// </summary>
        public SiteMessageInnerMailListNew_Collection QueryUnReadInnerMailList_ByReceiverId(string userId, int pageIndex, int pageSize, int handleType)
        {
            SiteMessageInnerMailListNew_Collection collection = new SiteMessageInnerMailListNew_Collection();
            collection.TotalCount = 0;
            var query = from m in DB.CreateQuery<E_SiteMessage_InnerMail_List_new>()
                        where (m.ReceiverId == userId ||
                        m.ReceiverId == "U:" + userId)
                        && m.HandleType == handleType
                        select m;
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.MailList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(m => new SiteMessageInnerMailListNewInfo
                {
                    HandleType = (InnerMailHandleType)m.HandleType,
                    MailId = m.MailId,
                    MsgContent = m.MsgContent,
                    ReadTime = m.ReadTime,
                    ReceiverId = m.ReceiverId,
                    SenderId = m.SenderId,
                    SendTime = m.SendTime,
                    Title = m.Title,
                }).ToList();
            }
            return collection;
        }

        #region 阅读站内信
        public bool IsMyInnerMail(string innerMailId, string userId)
        {
            //var count = manager.GetMailContainsReceiverCount(innerMailId, userId);
            var totalcount = 0;
            var query = DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().Where(s => s.MailId == innerMailId && (s.ReceiverId == userId || s.ReceiverId == "U:" + userId));
            if (query != null) totalcount = query.Count();
            return (totalcount > 0);
        }


        public InnerMailInfo_Query QueryInnerMailDetailByIdAndRead(string innerMailId,string userId)
        {
            //1.增加阅读数
            ReadInnerMail(innerMailId, userId);
            //2.返回数据
            var mail = QuerySiteMessageInnerMailListNewByMailId(innerMailId);
            var info = new InnerMailInfo_Query
            {
                MailId = mail.MailId,
                Title = mail.Title,
                Content = mail.MsgContent,
                SenderId = mail.SenderId,
                SendTime = mail.SendTime,
            };
            return info;
        }


        public E_SiteMessage_InnerMail_List_new QuerySiteMessageInnerMailListNewByMailId(string mailId)
        {
            return DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().FirstOrDefault(s => s.MailId == mailId);
        }

        /// <summary>
        /// 阅读站内信
        /// </summary>
        public void ReadInnerMail(string innerMailId, string userId)
        {
            var mail = QuerySiteMessageInnerMailListNewByMailId(innerMailId);
            if (mail != null)
            {
                mail.ReadTime = DateTime.Now;
                mail.HandleType = (int)InnerMailHandleType.Readed;
                UpdateSiteMessageInnerMailListNew(mail);
            }
        }

        public void UpdateSiteMessageInnerMailListNew(E_SiteMessage_InnerMail_List_new entity)
        {
            DB.GetDal<E_SiteMessage_InnerMail_List_new>().Update(entity);
        }
        #endregion

        #region 红包可使用比率
        public string QueryRedBagUseConfig()
        {
            var list= DB.CreateQuery<E_A20150919_红包使用配置>().ToList();
            var query = from l in list
                        select string.Format("{0}_{1}_{2}", l.Id, l.GameCode, l.UsePercent.ToString("N2"));
            return string.Join("|", query.ToArray());
        }
        #endregion

        public ArticleInfo_QueryCollection QueryArticleList_YouHua(string[] category, string[] gameCode, int pageIndex, int pageSize)
        {
            ArticleInfo_QueryCollection collection = new ArticleInfo_QueryCollection();
            // 通过数据库存储过程进行查询
            var query = from a in DB.CreateQuery<E_SiteMessage_Article_List>()
                        where category.Contains(a.Category)
                        && gameCode.Contains(a.GameCode)
                        orderby a.CreateTime descending
                        select a;

            collection.TotalCount = query.Count();
            collection.ArticleList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(a=> new ArticleInfo_Query
            {
                Category = a.Category.Trim(),
                CreateTime = a.CreateTime,
                CreateUserDisplayName = a.CreateUserDisplayName,
                CreateUserKey = a.CreateUserKey,
                DescContent = a.DescContent,
                Description = a.Description,
                GameCode = a.GameCode,
                Id = a.Id,
                IsRedTitle = a.IsRedTitle,
                KeyWords = a.KeyWords,
                NextId = a.NextId,
                NextTitle = a.NextTitle,
                PreId = a.PreId,
                PreTitle = a.PreTitle,
                ReadCount = a.ReadCount,
                ShowIndex = a.ShowIndex,
                Title = a.Title,
                UpdateTime = a.UpdateTime,
                UpdateUserDisplayName = a.UpdateUserDisplayName,
                UpdateUserKey = a.UpdateUserKey,
                NextStaticPath = a.NextStaticPath,
                PreStaticPath = a.PreStaticPath,
                StaticPath = a.StaticPath,
            }).ToList();

            return collection;
            //var list = manager.QueryArticleList_YouHua(array, gameCodeArray, pageIndex, pageSize);
            //    return list;
        }

        /// <summary>
        /// 查询fxid分享推广
        /// </summary>
        public List<Blog_UserShareSpread> QueryBlog_UserShareSpreadList(string userId, int pageIndex, int pageSize, DateTime begin, DateTime end, out int userTotalCount, out decimal RedBagMoneyTotal)
        {
            var query = from r in DB.CreateQuery<E_Blog_UserShareSpread>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where (r.AgentId == userId)
                        select new { r,u};
            if (query != null && query.Count() > 0)
            {
                userTotalCount = query.Count();//总人数
                RedBagMoneyTotal = query.Sum(g => g.r.giveRedBagMoney);//总红包金额
                return query.OrderByDescending(p => p.r.UpdateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(p=> new Blog_UserShareSpread
                {
                    Id = p.r.Id,
                    UserId = p.r.UserId,
                    userName = p.u.DisplayName,
                    AgentId = p.r.AgentId,
                    isGiveLotteryRedBag = p.r.isGiveLotteryRedBag,
                    isGiveRegisterRedBag = p.r.isGiveRegisterRedBag,
                    giveRedBagMoney = p.r.giveRedBagMoney,
                    CreateTime = p.r.CreateTime,
                    UpdateTime = p.r.UpdateTime
                }).ToList();
            }
            userTotalCount = 0;
            RedBagMoneyTotal = 0;
            return new List<Blog_UserShareSpread>();
        }
    }
}
