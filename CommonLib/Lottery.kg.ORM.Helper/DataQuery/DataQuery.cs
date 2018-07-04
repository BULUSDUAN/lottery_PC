using EntityModel;
using GameBiz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using System.Linq;

namespace Lottery.Kg.ORM.Helper
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
            var list= query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            var returnModel = new ActivityListInfoCollection();
            if (list != null && list.Count > 0)
            {
                returnModel.List.AddRange(list);
            }
            returnModel.TotalCount = totalCount;
            return returnModel;
        }
        #endregion

        #region LotteryGameManager
        public GameIssuse QueryCurrentNewIssuseInfo(string gameCode, string gameType)
        {
            var sql = SqlModule.DataModule.FirstOrDefault(x => x.Key == "Data_QueryCurrentNewIssuseInfo").SQL;
            var model = DB.CreateSQLQuery(sql)
              .SetString("@GameCode", gameCode)
              .SetString("@GameType", gameType)
              .List<GameIssuse>().FirstOrDefault();
            return model;
        }
        #endregion
    }
}
