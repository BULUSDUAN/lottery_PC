using EntityModel;
using GameBiz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using System.Linq;
using KaSon.FrameWork.ORM.Provider;
using EntityModel.Communication;
using KaSon.FrameWork.Common.Sport;
using EntityModel.ExceptionExtend;

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
            var bol = string.IsNullOrEmpty(gameType);
            //var query = from i in DB.CreateQuery<C_Game_Issuse>()
            //            where i.GameCode == gameCode
            //            && (string.IsNullOrEmpty(gameType) || i.GameType == gameType)
            //            && i.Status == (int)IssuseStatus.OnSale
            //            && i.LocalStopTime > DateTime.Now
            //            orderby i.LocalStopTime ascending
            //            select i;
            var query = DB.CreateQuery<C_Game_Issuse>();
            if (bol)
            {
                query = query.Where(p => p.GameCode == gameCode &&
                  p.Status == (int)IssuseStatus.OnSale &&
                  p.LocalStopTime > DateTime.Now
                ).OrderBy(p => p.LocalStopTime);
            }
            else
            {
                query = query.Where(p => p.GameCode == gameCode &&
                  p.Status == (int)IssuseStatus.OnSale &&
                  p.LocalStopTime > DateTime.Now &&
                  p.GameType == gameType
                ).OrderBy(p => p.LocalStopTime);
            }
            var info = query.FirstOrDefault();
            Issuse_QueryInfo returninfo = null;
            if (info != null)
            {
                returninfo = new Issuse_QueryInfo()
                {
                    CreateTime = info.CreateTime,
                    GameCode_IssuseNumber = info.GameCode_IssuseNumber,
                    GatewayStopTime = info.GatewayStopTime,
                    IssuseNumber = info.IssuseNumber,
                    LocalStopTime = info.LocalStopTime,
                    OfficialStopTime = info.OfficialStopTime,
                    StartTime = info.StartTime,
                    Status = (IssuseStatus)info.Status,
                    WinNumber = info.WinNumber,
                    Game = new GameInfo()
                    {
                        GameCode = info.GameCode
                    }
                };
            }
            return returninfo;
        }
        #endregion

        #region 前台查询广告信息
        /// <summary>
        /// 前台查询广告信息
        /// </summary>
        /// <param name="bannerType"></param>
        /// <param name="returnRecord"></param>
        /// <returns></returns>
        public SiteMessageBannerInfo_Collection QuerySitemessageBanngerList_Web(int bannerType, int returnRecord = 10)
        {
            var query = from s in DB.CreateQuery<E_Sitemessage_Banner>()
                        where s.BannerType == bannerType && s.IsEnable == true
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
            collection.ListInfo = new List<SiteMessageBannerInfo>();
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
        public BulletinInfo_Collection QueryDisplayBulletins(int agent, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            string QueryArticleList_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryBulletinList_Web").SQL;
            var collection = DB.CreateSQLQuery(QueryArticleList_sql)
              .SetInt("BulletinAgent", agent)
              .SetInt("PageIndex", pageIndex)
              .SetInt("PageSize", pageSize).List<BulletinInfo_Query>().ToList();

            var returnmodel = new BulletinInfo_Collection();
            returnmodel.BulletinList = collection;

            string QueryArticleList_Count_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryBulletinList_Web_Total").SQL;
            var totalCount = DB.CreateSQLQuery(QueryArticleList_Count_sql)
              .SetInt("BulletinAgent", agent).First<int>();
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
            var query = DB.CreateQuery<E_ActivityList>().Where(p => p.IsShow == true);
            var totalCount = DB.CreateQuery<E_ActivityList>().Count();

            var list = query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(a => new ActivityListInfo
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
            }).ToList();
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

        public IList<C_BJDC_Issuse> CurrentBJDCIssuseInfo()
        {
            var query = from b in DB.CreateQuery<C_BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select b;
            return query.ToList();
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
            var info = query.FirstOrDefault();
            BJDCIssuseInfo returninfo = null;
            if (info != null)
            {
                returninfo = new BJDCIssuseInfo
                {
                    IssuseNumber = info.IssuseNumber,
                    MinLocalStopTime = info.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    MinMatchStartTime = info.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                };
            }
            return returninfo;
            //return query.ToList().Select(p => new BJDCIssuseInfo
            //{
            //    IssuseNumber = p.IssuseNumber,
            //    MinLocalStopTime = p.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //    MinMatchStartTime = p.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //}).FirstOrDefault();
        }

        /// <summary>
        /// 根据公告Id查询公告信息  kason 框架修改支持 int 强制转换枚举，输出新对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BulletinInfo_Query QueryBulletinDetailById(long id)
        {
            var query = from b in DB.CreateQuery<E_SiteMessage_Bulletin_List>()
                        join u_create in DB.CreateQuery<C_User_Register>() on b.CreateBy equals u_create.UserId
                        //join u_update in DB.CreateQuery<C_User_Register>() on b.UpdateBy equals u_update.UserId
                        where b.Id == id
                        select new BulletinInfo_Query
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Content = b.Content,
                            Status = (EnableStatus)b.Status,
                            Priority = b.Priority,
                            IsPutTop = b.IsPutTop,
                            EffectiveFrom = b.EffectiveFrom,
                            EffectiveTo = b.EffectiveTo,
                            CreateTime = b.CreateTime,
                            CreateBy = b.CreateBy,
                            CreatorDisplayName = u_create.DisplayName,
                            UpdateTime = b.UpdateTime,
                            UpdateBy = b.UpdateBy,
                            //UpdatorDisplayName = t.u_update.DisplayName,
                            BulletinAgent = (BulletinAgent)b.BulletinAgent
                        };

            var model = query.FirstOrDefault();

            //    .Select(t => new BulletinInfo_Query
            //{
            //    Id = t.b.Id,
            //    Title = t.b.Title,
            //    Content = t.b.Content,
            //    Status = (EnableStatus)t.b.Status,
            //    Priority = t.b.Priority,
            //    IsPutTop = t.b.IsPutTop,
            //    EffectiveFrom = t.b.EffectiveFrom,
            //    EffectiveTo = t.b.EffectiveTo,
            //    CreateTime = t.b.CreateTime,
            //    CreateBy = t.b.CreateBy,
            //    CreatorDisplayName = t.u_create.DisplayName,
            //    UpdateTime = t.b.UpdateTime,
            //    UpdateBy = t.b.UpdateBy,
            //    //UpdatorDisplayName = t.u_update.DisplayName,
            //    BulletinAgent = (BulletinAgent)t.b.BulletinAgent
            //}).FirstOrDefault();
            if (model != null && !string.IsNullOrEmpty(model.UpdateBy))
            {
                var updateuser = DB.CreateQuery<C_User_Register>().Where(p => p.UserId == model.UpdateBy).FirstOrDefault();
                if (updateuser != null)
                {
                    model.UpdatorDisplayName = updateuser.DisplayName;
                }
            }
            return model;
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
                        IsForcedUpgrade = a.IsForcedUpgrade == false ? false : a.IsForcedUpgrade,
                        ConfigVersion = string.IsNullOrEmpty(a.ConfigVersion) ? string.Empty : a.ConfigVersion
                    }).ToList();
        }

        public C_App_Config QueryAppConfigByAgentId(string appAgentId)
        {
            return DB.CreateQuery<C_App_Config>().Where(p => p.AppAgentId == appAgentId).FirstOrDefault();
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


        public InnerMailInfo_Query QueryInnerMailDetailByIdAndRead(string innerMailId, string userId)
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
                ActionTime = mail.SendTime,
                UpdateTime = mail.SendTime
            };
            return info;
        }


        public E_SiteMessage_InnerMail_List_new QuerySiteMessageInnerMailListNewByMailId(string mailId)
        {
            return DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().Where(s => s.MailId == mailId).FirstOrDefault();
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
            var list = DB.CreateQuery<E_A20150919_红包使用配置>().ToList();
            var query = from l in list
                        select string.Format("{0}_{1}_{2}", l.Id, l.GameCode, l.UsePercent.ToString("N2"));
            return string.Join("|", query.ToArray());
        }

        public List<C_Bank_Info> GetBankList()
        {
            return DB.CreateQuery<C_Bank_Info>().Where(p => p.Disabled == false).ToList();
        }
        #endregion

        public ArticleInfo_QueryCollection QueryArticleList_YouHua(string[] category, string[] gameCode, int pageIndex, int pageSize)
        {
            ArticleInfo_QueryCollection collection = new ArticleInfo_QueryCollection();
            // 通过数据库存储过程进行查询
            var query = from a in DB.CreateQuery<E_SiteMessage_Article_List>()
                        where category.Contains(a.Category)
                        && gameCode.Contains(a.GameCode)
                        //orderby a.CreateTime descending
                        select a;

            collection.TotalCount = query.Count();
            collection.ArticleList = query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(a => new ArticleInfo_Query
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


        #region 根据GameCode与期号获取当年/当天最大期号

        public List<string> GetMaxIssueByGameCode(string gameCode, string currIssuseNumber, int issueCount)
        {
            //var DayGame = new List<string>() {"SSQ","DLT","FC3D","PL3"};
            //var MinGame = new List<string>() {"CQSSC","GD11X5","SD11X5","JX11X5"};
            var theGameCode = gameCode.ToLower();
            var model = DB.CreateQuery<C_Game_Issuse>().Where(p => p.GameCode == theGameCode && p.IssuseNumber == currIssuseNumber).FirstOrDefault();
            var IssuseList = new List<string>();
            if (model != null)
            {
                //"<>h__TransparentIdentifier2.b"
                //if (DayGame.Contains(theGameCode))
                //{
                //    var thisYear = model.OfficialStopTime.Year;
                //    var lastDay = new DateTime(thisYear + 1, 1, 1);
                //    var lastIssuse = DB.CreateQuery<C_Game_Issuse>().Where(p => p.OfficialStopTime < lastDay && p.GameCode == theGameCode).OrderBy(p => p.OfficialStopTime).Take(100);
                //    MaxIssue = lastIssuse.IssuseNumber;
                //}
                //else if (MinGame.Contains(theGameCode))
                //{
                //    var today = model.LocalStopTime.Date;
                //    var tomorrow = today.AddDays(1);
                //    var lastIssuse = DB.CreateQuery<C_Game_Issuse>().Where(p => p.LocalStopTime > today && p.LocalStopTime < tomorrow &&p.GameCode== theGameCode).OrderBy(p => p.OfficialStopTime).FirstOrDefault();
                //    MaxIssue= lastIssuse.IssuseNumber;
                //}
                // DbProvider.IsShowOneSQL = true;
                IssuseList = DB.CreateQuery<C_Game_Issuse>().Where(p => p.OfficialStopTime >= model.OfficialStopTime && p.GameCode == theGameCode).OrderBy(p => p.OfficialStopTime).Take(issueCount).Select(p => p.IssuseNumber).ToList();
            }
            return IssuseList;
        }
        #endregion

        /// <summary>
        /// 查询fxid分享推广
        /// </summary>
        public List<Blog_UserShareSpread> QueryBlog_UserShareSpreadList(string userId, int pageIndex, int pageSize, DateTime begin, DateTime end, out int userTotalCount, out decimal RedBagMoneyTotal)
        {
            var query = (from r in DB.CreateQuery<E_Blog_UserShareSpread>()
                         join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                         where (r.AgentId == userId)
                         select new { r, u }).ToList();
            if (query != null && query.Count() > 0)
            {
                userTotalCount = query.Count();//总人数
                RedBagMoneyTotal = query.Sum(g => g.r.giveRedBagMoney);//总红包金额
                return query.OrderByDescending(p => p.r.UpdateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(p => new Blog_UserShareSpread
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

        public CommonActionResult FreezeGameRecharge(string userId, decimal money, string userDisplayName, int gameType)
        {
            var orderId = BettingHelper.GetGameTransferId();
            var msg = string.Format("游戏充值订单号{0}", orderId);
            DB.Begin();
            try
            {
                BusinessHelper.Payout_To_Frozen(BusinessHelper.FundCategory_GameRecharge, userId, orderId, money, msg, "GameTransfer", "");
                DB.GetDal<C_Game_Transfer>().Add(new C_Game_Transfer()
                {
                    OrderId = orderId,
                    RequestMoney = money,
                    RequestTime = DateTime.Now,
                    Status = (int)FillMoneyStatus.Requesting,
                    UserId = userId,
                    TransferType = (int)GameTransferType.Recharge,
                    UserDisplayName = userDisplayName,
                    GameType = gameType
                });
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        }

        /// <summary>
        /// 充值完成或失败，扣除冻结金额或返还冻结金额
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public CommonActionResult EndFreezeGameRecharge(string orderId, bool isSuccess, string providerSerialNo)
        {
            var oldModel = DB.CreateQuery<C_Game_Transfer>().Where(p => p.OrderId == orderId).FirstOrDefault();
            if (oldModel == null) throw new LogicException("发生错误，找不到相关订单");
            if (oldModel.Status != (int)FillMoneyStatus.Requesting) throw new LogicException("相关订单已被处理，无需重复操作");
            DB.Begin();
            try
            {
                if (isSuccess)
                {
                    oldModel.Status = (int)FillMoneyStatus.Success;
                    oldModel.UpdateTime = DateTime.Now;
                    oldModel.ProviderSerialNo = providerSerialNo;
                    DB.GetDal<C_Game_Transfer>().Update(oldModel);
                    BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_GameRecharge, oldModel.UserId, orderId, string.Format("游戏充值成功，扣除冻结{1:N2}元", orderId, oldModel.RequestMoney), oldModel.RequestMoney);
                }
                else
                {
                    oldModel.Status = (int)FillMoneyStatus.Failed;
                    oldModel.UpdateTime = DateTime.Now;
                    oldModel.ProviderSerialNo = providerSerialNo;
                    DB.GetDal<C_Game_Transfer>().Update(oldModel);
                    BusinessHelper.Payin_FrozenBack(BusinessHelper.FundCategory_GameRecharge, oldModel.UserId, orderId, oldModel.RequestMoney, string.Format("游戏充值不成功，返还资金{0:N2}元", oldModel.RequestMoney));
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        }

        public CommonActionResult AddGameWithdraw(string userId, decimal money, string userDisplayName, string orderId, string providerSerialNo)
        {
            //var orderId = BettingHelper.GetGameTransferId();
            DB.Begin();
            try
            {
                //if (IsSuccess)
                //{
                DB.GetDal<C_Game_Transfer>().Add(new C_Game_Transfer()
                {
                    OrderId = orderId,
                    RequestMoney = money,
                    RequestTime = DateTime.Now,
                    Status = (int)FillMoneyStatus.Success,
                    UserId = userId,
                    TransferType = (int)GameTransferType.Withdraw,
                    UserDisplayName = userDisplayName,
                    UpdateTime = DateTime.Now,
                    ProviderSerialNo = providerSerialNo
                });
                //oldModel.Status = (int)FillMoneyStatus.Success;
                //    oldModel.UpdateTime = DateTime.Now;
                //    DB.GetDal<C_Game_Transfer>().Update(oldModel);
                BusinessHelper.Payin_To_Balance(AccountType.FillMoney, BusinessHelper.FundCategory_GameWithdraw, userId, orderId, money,
                string.Format("游戏提款成功，金额：{0:N2}元存入账号", money));
                //}
                //else
                //{
                //    oldModel.Status = (int)FillMoneyStatus.Failed;
                //    oldModel.UpdateTime = DateTime.Now;
                //    DB.GetDal<C_Game_Transfer>().Update(oldModel);
                //}
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        }

        public CommonActionResult AddGameWithdraw_Step1(string userId, decimal money, string userDisplayName, int gameType)
        {
            //var orderId = BettingHelper.GetGameTransferId();
            var orderId = BettingHelper.GetGameTransferId();
            DB.GetDal<C_Game_Transfer>().Add(new C_Game_Transfer()
            {
                OrderId = orderId,
                RequestMoney = money,
                RequestTime = DateTime.Now,
                Status = (int)FillMoneyStatus.Requesting,
                UserId = userId,
                TransferType = (int)GameTransferType.Withdraw,
                UserDisplayName = userDisplayName,
                GameType = gameType
            });
            return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        }

        //public CommonActionResult AddGameWithdraw_Step2(string orderId, string providerSerialNo)
        //{
        //    ////var orderId = BettingHelper.GetGameTransferId();
        //    //var oldmodel = DB.CreateQuery<C_Game_Transfer>().Where(p => p.OrderId == orderId).FirstOrDefault();
        //    //if (oldmodel == null) throw new LogicException("发生错误，找不到相关订单");
        //    //oldmodel.ProviderSerialNo = providerSerialNo;
        //    //oldmodel.UpdateTime = DateTime.Now;
        //    //DB.GetDal<C_Game_Transfer>().Update(oldmodel);
        //    //return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };

        //    var oldModel = DB.CreateQuery<C_Game_Transfer>().Where(p => p.OrderId == orderId).FirstOrDefault();
        //    if (oldModel == null) throw new LogicException("发生错误，找不到相关订单");
        //    if (oldModel.Status != (int)FillMoneyStatus.Requesting) throw new LogicException("相关订单已被处理，无需重复操作");
        //    DB.Begin();
        //    try
        //    {
        //        oldModel.Status = (int)FillMoneyStatus.Success;
        //        oldModel.UpdateTime = DateTime.Now;
        //        oldModel.ProviderSerialNo = providerSerialNo;
        //        DB.GetDal<C_Game_Transfer>().Update(oldModel);
        //        BusinessHelper.Payin_To_Balance(AccountType.FillMoney, BusinessHelper.FundCategory_GameWithdraw, oldModel.UserId, oldModel.OrderId, oldModel.RequestMoney,
        //        string.Format("游戏提款成功，金额：{0:N2}元存入账号", oldModel.RequestMoney));
        //        DB.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        throw ex;
        //    }
        //    return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        //}

        public CommonActionResult EndAddGameWithdraw(string OrderId, bool IsSuccess, string providerSerialNo)
        {
            var oldModel = DB.CreateQuery<C_Game_Transfer>().Where(p => p.OrderId == OrderId).FirstOrDefault();
            if (oldModel == null) throw new LogicException("发生错误，找不到相关订单");
            if (oldModel.Status != (int)FillMoneyStatus.Requesting) throw new LogicException("相关订单已被处理，无需重复操作");
            DB.Begin();
            try
            {
                if (IsSuccess)
                {
                    oldModel.Status = (int)FillMoneyStatus.Success;
                    oldModel.UpdateTime = DateTime.Now;
                    oldModel.ProviderSerialNo = providerSerialNo;
                    DB.GetDal<C_Game_Transfer>().Update(oldModel);
                    BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_GameWithdraw, oldModel.UserId, oldModel.OrderId, oldModel.RequestMoney,
                    string.Format("游戏提款成功，金额：{0:N2}元存入账号", oldModel.RequestMoney));
                }
                else
                {
                    oldModel.Status = (int)FillMoneyStatus.Failed;
                    oldModel.UpdateTime = DateTime.Now;
                    oldModel.ProviderSerialNo = providerSerialNo;
                    DB.GetDal<C_Game_Transfer>().Update(oldModel);
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return new CommonActionResult() { IsSuccess = true, ReturnValue = OrderId };
        }

        public List<C_Game_Transfer> QueryNotFinishGame(int minutes)
        {
            var now = DateTime.Now;
            var date = now.AddMinutes(-minutes);
            return DB.CreateQuery<C_Game_Transfer>().Where(p => p.RequestTime < date && p.Status == 0).ToList();
        }

        ///// <summary>
        ///// 第二步把外部订单号存入数据库
        ///// </summary>
        ///// <param name="orderId"></param>
        ///// <param name="userId"></param>
        ///// <param name="providerSerialNo"></param>
        ///// <returns></returns>
        //public CommonActionResult GameRecharge_Step2(string orderId, string providerSerialNo)
        //{
        //    //var oldModel = DB.CreateQuery<C_Game_Transfer>().Where(p => p.OrderId == orderId && p.UserId == userId).FirstOrDefault();
        //    //if (oldModel == null) throw new LogicException("发生错误，找不到相关订单");
        //    //oldModel.ProviderSerialNo = providerSerialNo;
        //    //oldModel.UpdateTime = DateTime.Now;
        //    //DB.GetDal<C_Game_Transfer>().Update(oldModel);
        //    //return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        //    var oldModel = DB.CreateQuery<C_Game_Transfer>().Where(p => p.OrderId == orderId).FirstOrDefault();
        //    if (oldModel == null) throw new LogicException("发生错误，找不到相关订单");
        //    if (oldModel.Status != (int)FillMoneyStatus.Requesting) throw new LogicException("相关订单已被处理，无需重复操作");
        //    DB.Begin();
        //    try
        //    {
        //        oldModel.Status = (int)FillMoneyStatus.Success;
        //        oldModel.UpdateTime = DateTime.Now;
        //        oldModel.ProviderSerialNo = providerSerialNo;
        //        DB.GetDal<C_Game_Transfer>().Update(oldModel);
        //        BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_GameRecharge, oldModel.UserId, orderId, string.Format("游戏充值成功，扣除冻结{1:N2}元", orderId, oldModel.RequestMoney), oldModel.RequestMoney);
        //        DB.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        throw ex;
        //    }
        //    return new CommonActionResult() { IsSuccess = true, ReturnValue = orderId };
        //}
        #region PC端接口
        /// <summary>
        /// 大奖排行榜
        /// </summary>
        public RankReportCollection_BettingProfit_Sport QueryRankInfoList_BigBonus_Sport(QueryBonusBase QueryBase)
        {
            //原执行存储过程改为直接执行SQL
            var result = new RankReportCollection_BettingProfit_Sport();
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportBigBonusSport").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
              .SetInt("@pageIndex", QueryBase.pageIndex)
              .SetInt("@pagePage", QueryBase.pageSize).List<RankInfo_BettingProfit_Sport>().ToList();

            string BonusSport_Total_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportBigBonusSport_Total").SQL;
            var total = DB.CreateSQLQuery(BonusSport_Total_sql)
             .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
             .First<int>();

            result.RankInfoList = collection;
            result.TotalCount = total;
            return result;
        }

        /// <summary>
        /// 发单盈利排行榜 - 竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public RankReportCollection_BettingProfit_Sport QueryRankReport_BettingProfit_Sport(QueryBonusBase QueryBase)
        {
            //原执行存储过程改为直接执行SQL
            var result = new RankReportCollection_BettingProfit_Sport();
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportBettingProfitSport").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
              .SetInt("@pageIndex", QueryBase.pageIndex)
              .SetInt("@pagePage", QueryBase.pageSize).List<RankInfo_BettingProfit_Sport>().ToList();
            result.RankInfoList = collection;
            //原代码已注释总条数
            result.TotalCount = 100;
            return result;
        }

        /// <summary>
        /// 跟单盈利排行榜_竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public RankReportCollection_BettingProfit_Sport QueryRankReport_JoinProfit_Sport(QueryBonusBase QueryBase)
        {
            //原执行存储过程改为直接执行SQL
            var result = new RankReportCollection_BettingProfit_Sport();
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportJoinProfitSport").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
              .SetInt("@pageIndex", QueryBase.pageIndex)
              .SetInt("@pagePage", QueryBase.pageSize).List<RankInfo_BettingProfit_Sport>().ToList();

            string BonusSport_Total_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportJoinProfitSport_Total").SQL;
            var total = DB.CreateSQLQuery(BonusSport_Total_sql)
             .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
             .First<int>();
            result.RankInfoList = collection;
            result.TotalCount = total;
            return result;
        }

        /// <summary>
        /// 合买人气排行
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public RankReportCollection_RankInfo_HotTogether QueryRankInfoList_HotTogether(QueryBonusBase QueryBase)
        {
            //原执行存储过程改为直接执行SQL
            var result = new RankReportCollection_RankInfo_HotTogether();
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankInfoListHotTogether").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode).List<RankInfo_HotTogether>().ToList();
            result.RankInfoList = collection;
            result.TotalCount = collection == null ? 0 : collection.Count;
            return result;
        }

        /// <summary>
        /// 成功的战绩排行_竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public RankReportCollection_BettingProfit_Sport QueryRankInfoList_SuccessOrder_Sport(QueryBonusBase QueryBase)
        {
            //原执行存储过程改为直接执行SQL
            var result = new RankReportCollection_BettingProfit_Sport();
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankInfoListSuccessOrderSport").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode).List<RankInfo_BettingProfit_Sport>().ToList();
            result.RankInfoList = collection;
            result.TotalCount = collection == null ? 0 : collection.Count;
            return result;
        }

        /// <summary>
        /// 跟单排行榜
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public RankReportCollection_RankInfo_BeFollower QueryRankInfoList_BeFollowerCount(QueryBonusBase QueryBase)
        {
            var result = new RankReportCollection_RankInfo_BeFollower();
            if (BettingHelper.CheckSQLCondition(QueryBase.gameCode) || BettingHelper.CheckSQLCondition(QueryBase.gameType))
                throw new Exception("传入彩种或玩法含有特殊字符");
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            //Data_QueryRankInfoListBeFollowerCount
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankInfoListBeFollowerCount").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode).List<RankInfo_BeFollower>().ToList();
            result.RankInfoList = collection;
            result.TotalCount = collection == null ? 0 : collection.Count;
            return result;
        }

        /// <summary>
        ///  累积中奖排行榜 - 竞彩类
        /// </summary>
        public RankReportCollection_TotalBonus_Sport QueryRankReport_TotalBonus_Sport(QueryBonusBase QueryBase)
        {
            var result = new RankReportCollection_TotalBonus_Sport();
            QueryBase.pageIndex = QueryBase.pageIndex < 0 ? 0 : QueryBase.pageIndex;
            QueryBase.pageSize = QueryBase.pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : QueryBase.pageSize;
            string BonusSport_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportTotalBonusSport").SQL;
            var collection = DB.CreateSQLQuery(BonusSport_sql)
              .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
              .SetInt("@pageIndex", QueryBase.pageIndex)
              .SetInt("@pagePage", QueryBase.pageSize).List<RankInfo_TotalBonus_Sport>().ToList();

            string BonusSport_Total_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportTotalBonusSport_Total").SQL;
            var total = DB.CreateSQLQuery(BonusSport_Total_sql)
             .SetString("@fromDate", QueryBase.fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", QueryBase.toDate.ToString("yyyy-MM-dd"))
              .SetString("@gameCode", QueryBase.gameCode)
              .SetString("@gameType", QueryBase.gameType)
             .First<int>();
            result.RankInfoList = collection;
            result.TotalCount = total;
            return result;
        }


        /// <summary>
        /// 成功派奖列表
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string QueryPrizedIssuseList(string gameCode, string gameType, int length)
        {
            var query = from g in DB.CreateQuery<C_Game_Issuse>()
                        where g.GameCode == gameCode
                        && (gameType == "" || gameType == null || gameType == g.GameType)
                        && g.Status == (int)IssuseStatus.Stopped
                        orderby g.IssuseNumber descending
                        select g.IssuseNumber;
            return string.Join(",", query.Take(length).ToList());
        }

        /// <summary>
        /// 中奖查询，公共数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="issuseNumber"></param>
        /// <param name="completeData"></param>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public BonusOrderInfoCollection QueryBonusInfoList(string userId, string gameCode, string gameType, string issuseNumber, string completeData, string key, int pageIndex, int pageSize)
        {
            var result = new BonusOrderInfoCollection();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            string sql = SqlModule.UserSystemModule.FirstOrDefault(p => p.Key == "P_Order_QueryBonusOrderList").SQL;
            result.BonusOrderList = DB.CreateSQLQuery(sql)
                .SetString("@UserId", userId ?? "")
                .SetString("@GameCode", gameCode)
                .SetInt("@PageIndex", pageIndex)
                .SetInt("@PageSize", pageSize)
                .List<BonusOrderInfo>().ToList();
            result.TotalCount = result.BonusOrderList.Count();
            return result;

        }

        /// <summary>
        /// 中奖排行榜_按彩种查
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="topCount"></param>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public RankReportCollection_TotalBonus_Sport QueryRankReport_BonusByGameCode_All(DateTime fromDate, DateTime toDate, int topCount, string gameCode)
        {
            var Result = new RankReportCollection_TotalBonus_Sport();
            string QueryBonusInfo_sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryRankReportBonusByGameCodeAll").SQL;
            var BonusOrderList = DB.CreateSQLQuery(QueryBonusInfo_sql)
              .SetString("@fromDate", fromDate.ToString("yyyy-MM-dd"))
              .SetString("@toDate", toDate.ToString("yyyy-MM-dd"))
              .SetInt("@topCount", topCount)
              .SetString("@gameCode", gameCode).List<RankInfo_TotalBonus_Sport>();
            Result.RankInfoList = BonusOrderList;
            return Result;
        }
        #endregion

        #region 管理端接口
        /// <summary>
        /// 第三方游戏列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ThirdPartyGameCollection ThirdPartyGameDetail(ThirdPartyGameListParam param)
        {
            var query = DB.CreateQuery<C_Game_Transfer>().Where(p =>
           (param.GameType == -1 || p.GameType == param.GameType) &&
           (param.OrderId == null || param.OrderId == "" || p.OrderId == param.OrderId || p.ProviderSerialNo == param.OrderId) &&
           (param.Status == -1 || p.GameType == param.Status) &&
           (param.TransferType == -1 || p.TransferType == param.TransferType) &&
           (param.UserId == null || param.UserId == "" || p.UserId == param.UserId) &&
           p.RequestTime >= param.StartTime && p.RequestTime < param.EndTime)
           .OrderByDescending(p => p.RequestTime);
            var totalCount = query.Count();
            var list = query.Skip(param.PageIndex * param.PageSize).Take(param.PageSize).ToList().Select(p => new GameTransfer_ShowModel()
            {
                GameType = p.GameType,
                GameTypeShowName = ((MGGameType)p.GameType).GetRemark(),
                OrderId = p.OrderId,
                ProviderSerialNo = p.ProviderSerialNo,
                RequestMoney = p.RequestMoney,
                RequestTime = p.RequestTime,
                Status = p.Status,
                StatusShowName = ((FillMoneyStatus)p.Status).GetRemark(),
                TransferType = p.TransferType,
                TransferTypeShowName = ((GameTransferType)p.TransferType).GetRemark(),
                UpdateTime = p.UpdateTime,
                UserDisplayName = p.UserDisplayName,
                UserId = p.UserId
            }).ToList();
            return new ThirdPartyGameCollection()
            {
                List = list,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <returns></returns>
        public List<IndexReportForms> GetIndexReportForms()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1).Date;
            var firstDayByMonth= new DateTime(today.Year, today.Month, 1);
            //注册人数，实名人数，付费人数，当天充值金额，当天提款金额，当月的注册人数，当月的充值金额，当月的提款金额
            var registerSql = "Select Count(*) as TotalCount,ComeFrom from C_User_Register Where CreateTime >= @beginTime AND CreateTime < @endTime Group by ComeFrom";
            var realNameSql = "Select Count(*) as TotalCount,ComeFrom from C_User_Register c Inner join E_Authentication_RealName e on c.userId=e.userId where e.CreateTime >= @beginTime AND e.CreateTime < @endTime  group by c.ComeFrom";
            var paySql = "select Count(*) as TotalCount,ComeFrom from C_User_Register c inner join (select DISTINCT(userId) from E_Blog_Dynamic where CreateTime >= @beginTime AND CreateTime < @endTime ) t on c.userId=t.userId GROUP BY c.ComeFrom";
            var rechargeSql = @"select sum(ResponseMoney) as TotalMoney,c.ComeFrom from C_FillMoney f inner join C_User_Register c on f.userId = c.userId 
where f.status=1 and f.FillMoneyAgent !=80 and f.FillMoneyAgent !=90 
 and f.requestTime>=@beginTime and f.requestTime<@endTime group by c.ComeFrom";
            var withdrawSql = @"select sum(ResponseMoney) as TotalMoney,c.ComeFrom from C_Withdraw f inner join C_User_Register c on f.userId = c.userId 
where f.status=3 
 and f.requestTime>=@beginTime and f.requestTime<@endTime group by c.ComeFrom";

            //当天注册人数
            var registerToday = DB.CreateSQLQuery(registerSql)
                .SetString("beginTime", today.ToString("yyyy-MM-dd"))
                .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
                .List<IndexReportForms_FromCount>();
            //当月注册人数
            var registerMonth = DB.CreateSQLQuery(registerSql)
                .SetString("beginTime", firstDayByMonth.ToString("yyyy-MM-dd"))
                .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
                .List<IndexReportForms_FromCount>();
            //实名人数
            var realNameToday = DB.CreateSQLQuery(realNameSql)
                .SetString("beginTime", today.ToString("yyyy-MM-dd"))
                .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
                .List<IndexReportForms_FromCount>();
            //付费人数
            var payToday = DB.CreateSQLQuery(paySql)
                .SetString("beginTime", today.ToString("yyyy-MM-dd"))
                .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
                .List<IndexReportForms_FromCount>();
            //当日充值金额
            var rechargeToday= DB.CreateSQLQuery(rechargeSql)
                .SetString("beginTime", today.ToString("yyyy-MM-dd"))
                .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
                .List<IndexReportForms_FromTotalMoney>();
            //当日提款金额
            var withdrawToday = DB.CreateSQLQuery(withdrawSql)
               .SetString("beginTime", today.ToString("yyyy-MM-dd"))
               .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
               .List<IndexReportForms_FromTotalMoney>();
            //当月充值金额
            var rechargeMonth = DB.CreateSQLQuery(rechargeSql)
                .SetString("beginTime", firstDayByMonth.ToString("yyyy-MM-dd"))
                .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
                .List<IndexReportForms_FromTotalMoney>();
            //当月提款金额
            var withdrawMonth = DB.CreateSQLQuery(withdrawSql)
               .SetString("beginTime", firstDayByMonth.ToString("yyyy-MM-dd"))
               .SetString("endTime", tomorrow.ToString("yyyy-MM-dd"))
               .List<IndexReportForms_FromTotalMoney>();
            //INNER
            //IOS
            //LOCAL
            //NewAndroid
            //NewIOS
            //NewTOUCH
            //TOUCH
            var comeFromList = new List<string>()
            {
                "INNER",
                "IOS",
                "LOCAL",
                "NewAndroid",
                "NewIOS",
                "NewTOUCH",
                "TOUCH"
            };
            var result = new List<IndexReportForms>();
            foreach (var item in comeFromList)
            {
                var model = new IndexReportForms() { Source=item };
                //当天注册人数
                var register = registerToday.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (register == null) model.RegisterCountByDay = 0;
                else model.RegisterCountByDay = register.TotalCount;
                //当月注册人数
                var registerMItem = registerMonth.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (registerMItem == null) model.RegisterCountByDay = 0;
                else model.RegisterCountByDay = registerMItem.TotalCount;
                //实名人数
                var realName = realNameToday.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (realName == null) model.ReadNameCount = 0;
                else model.ReadNameCount = realName.TotalCount;
                //付费人数
                var pay = payToday.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (pay == null) model.PayCount = 0;
                else model.PayCount = pay.TotalCount;
                //当日充值金额
                var recharge = rechargeToday.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (recharge == null) model.RechargeMoneyByDay = 0m;
                else model.RechargeMoneyByDay = recharge.TotalMoney;
                //当日充值金额
                var withdraw = withdrawToday.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (withdraw == null) model.WithdrawalMoneyByDay = 0m;
                else model.WithdrawalMoneyByDay = withdraw.TotalMoney;
                //当月充值金额
                var rechargeMItem = rechargeToday.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (rechargeMItem == null) model.RechargeMoneyByMonth = 0m;
                else model.RechargeMoneyByMonth = rechargeMItem.TotalMoney;
                //当日充值金额
                var withdrawMItem = withdrawMonth.Where(p => p.ComeFrom == item).FirstOrDefault();
                if (withdrawMItem == null) model.WithdrawalMoneyByMonth = 0m;
                else model.WithdrawalMoneyByMonth = withdraw.TotalMoney;
                result.Add(model);
            }
            return result;
        }

        #endregion
        public List<LotteryGameInfo> QueryLotteryGameList()
        {
            var query = from g in DB.CreateQuery<C_Lottery_Game>()
                        orderby g.GameCode
                        select new LotteryGameInfo
                        {
                            DisplayName = g.DisplayName,
                            EnableStatus = (EnableStatus)g.EnableStatus,
                            GameCode = g.GameCode
                        };
            return query.ToList();
        }

        public List<E_A20150919_加奖配置> GetAddMoneyList()
        {
            return DB.CreateQuery<E_A20150919_加奖配置>().ToList();
        }
    }
}
