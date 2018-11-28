using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain;
using Common.Utilities;
using Common.Business;
using System.Collections;
using Common;
using External.Core.SiteMessage;
using External.Domain.Entities.SiteMessage;
using External.Domain.Managers.SiteMessage;
using External.Core;
using GameBiz.Core;
using GameBiz.Domain.Entities;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using Common.Communication;
using External.Domain.Managers.Agent;
using External.Domain.Entities.Agent;

namespace External.Business
{
    public class SiteMessageBusiness
    {
        #region 公告

        /// <summary>
        /// 发布公告
        /// </summary>
        public void PublishBulletin(BulletinInfo_Publish bulletin, string createBy)
        {
            var entity = new Bulletin();
            ObjectConvert.ConverInfoToEntity<BulletinInfo_Publish, Bulletin>(bulletin, ref entity);
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var bulletinManager = new BulletinManager())
                {
                    entity.CreateBy = createBy;
                    entity.UpdateBy = createBy;
                    bulletinManager.AddBulletin(entity);
                }
                biz.CommitTran();
            }
        }
        /// <summary>
        /// 修改公告
        /// </summary>
        public void UpdateBulletin(long id, BulletinInfo_Publish bulletin, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var bulletinManager = new BulletinManager())
                {
                    var entity = bulletinManager.GetBulletinById(id);
                    ObjectConvert.ConverInfoToEntity<BulletinInfo_Publish, Bulletin>(bulletin, ref entity);
                    entity.UpdateBy = updateBy;
                    bulletinManager.UpdateBulletin(entity);
                }
                biz.CommitTran();
            }
        }
        public void UpdateBulletinStatus(long id, EnableStatus status, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var bulletinManager = new BulletinManager())
                {
                    var entity = bulletinManager.GetBulletinById(id);
                    entity.Status = status;
                    entity.UpdateBy = updateBy;
                    bulletinManager.UpdateBulletin(entity);
                }
                biz.CommitTran();
            }
        }
        /// <summary>
        /// 根据公告编号，查询公告详情
        /// </summary>
        public BulletinInfo_Query QueryBulletinDetailById(long bulletinId)
        {
            using (var bulletinManager = new BulletinManager())
            {
                return bulletinManager.QueryBulletinDetailById(bulletinId);
            }
        }
        /// <summary>
        /// 查询前台显示的公告列表
        /// </summary>
        public BulletinInfo_Collection QueryDisplayBulletins(BulletinAgent agent, int pageIndex, int pageSize)
        {
            using (var bulletinManager = new BulletinManager())
            {
                int totalCount;
                var list = bulletinManager.QueryDispayBulletinList(agent, pageIndex, pageSize, out totalCount);
                var collection = new BulletinInfo_Collection
                {
                    TotalCount = totalCount,
                };
                collection.LoadList(list);
                return collection;
            }
        }
        /// <summary>
        /// 查询后台管理的公告列表
        /// </summary>
        public BulletinInfo_Collection QueryManagementBulletins(string key, EnableStatus status, int priority, int isPutTop, int pageIndex, int pageSize)
        {
            using (var bulletinManager = new BulletinManager())
            {
                int totalCount;
                var list = bulletinManager.QueryAdminBulletinList(key, status, priority, isPutTop, pageIndex, pageSize, out totalCount);
                var collection = new BulletinInfo_Collection
                {
                    TotalCount = totalCount,
                };
                collection.LoadList(list);
                return collection;
            }
        }

        #endregion

        #region 用户建议

        public void SubmitUserIdea(UserIdeaInfo_Add userIdea)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                using (var manager = new UserIdeaManager())
                {
                    manager.AddUserIdea(new UserIdea
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
                        ContentConveyDistinct = userIdea.ContentConveyDistinct,
                    });
                }

                biz.CommitTran();
            }
        }
        public void HandleUserIdea(int id, string status, string updateUserId, string updateUserDisplayName, string managerReply)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserIdeaManager())
                {
                    var idea = manager.GetUserIdeaById(id);
                    idea.Status = status;
                    idea.UpdateUserId = updateUserId;
                    idea.UpdateUserDisplayName = updateUserDisplayName;
                    idea.ManageReply = managerReply;
                    manager.UpdateUserIdea(idea);
                }
                biz.CommitTran();
            }
        }
        public UserIdeaInfo_QueryCollection QueryUserIdeaList(List<string> statusList, List<string> categoryList, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new UserIdeaManager())
            {
                int totalCount;
                var list = manager.QueryUserIdeaList(statusList, categoryList, starTime, endTime, pageIndex, pageSize, out totalCount);
                var collection = new UserIdeaInfo_QueryCollection
                {
                    TotalCount = totalCount,
                };
                IList<UserIdeaInfo_Query> infoList = new List<UserIdeaInfo_Query>();
                ObjectConvert.ConvertEntityListToInfoList<IList<UserIdea>, UserIdea, IList<UserIdeaInfo_Query>, UserIdeaInfo_Query>(list, ref infoList, () => new UserIdeaInfo_Query());
                collection.UserIdeaList = infoList;
                return collection;
            }
        }

        public UserIdeaInfo_QueryCollection QueryMyUserIdeaList(string createUserId, int pageIndex, int pageSize)
        {
            var result = new UserIdeaInfo_QueryCollection();
            var manager = new UserIdeaManager();
            var totalCount = 0;
            result.UserIdeaList = manager.QueryMyUserIdeaList(createUserId, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        #endregion

        #region 常见问题

        public string SubmitDoubt(DoubtInfo_Add doubt)
        {
            var id = Guid.NewGuid().ToString("N");
            var entity = new Doubt
            {
                Id = id,
                Title = doubt.Title,
                Description = doubt.Description,
                Category = doubt.Category,
                ShowIndex = 0,
                UpCount = 0,
                DownCount = 0,
                CreateTime = DateTime.Now,
                CreateUserKey = doubt.CreateUserKey,
                CreateUserDisplayName = doubt.CreateUserDisplayName,
                UpdateTime = DateTime.Now,
                UpdateUserKey = doubt.CreateUserKey,
                UpdateUserDisplayName = doubt.CreateUserDisplayName,
            };
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new DoubtManager())
                {
                    manager.AddDoubt(entity);
                }
                biz.CommitTran();
            }
            return id;
        }
        public void UpdateDoubt(string doubtId, DoubtInfo_Add doubt)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new DoubtManager())
                {
                    var entity = manager.GetDoubtById(doubtId);
                    if (entity == null)
                    {
                        throw new ArgumentException("指定编号的问题不存在");
                    }
                    entity.Title = doubt.Title;
                    entity.Description = doubt.Description;
                    entity.Category = doubt.Category;
                    entity.UpdateTime = DateTime.Now;
                    entity.UpdateUserKey = doubt.CreateUserKey;
                    entity.UpdateUserDisplayName = doubt.CreateUserDisplayName;
                    manager.UpdateDoubt(entity);
                }
                biz.CommitTran();
            }
        }
        public void UpdateDoubtIndex(Dictionary<string, int> indexCollection)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new DoubtManager())
                {
                    manager.UpdateDoubtIndex(indexCollection);
                }
                biz.CommitTran();
            }
        }
        public void DeleteDoubt(string doubtId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new DoubtManager())
                {
                    var entity = manager.GetDoubtById(doubtId);
                    if (entity == null)
                    {
                        throw new ArgumentException("指定编号的问题不存在");
                    }
                    manager.DeleteUpDownRecord(doubtId);
                    manager.DeleteDoubt(entity);
                }
                biz.CommitTran();
            }
        }
        public void UpDownDoubt(string doubtId, string userId, string operation)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new DoubtManager())
                {
                    var entity = manager.GetDoubtById(doubtId);
                    if (entity == null)
                    {
                        throw new ArgumentException("指定编号的问题不存在");
                    }
                    var record = manager.GetUpDownRecord(userId, doubtId);
                    if (record != null)
                    {
                        throw new ArgumentException("不能重复操作");
                    }
                    if (operation.ToUpper() == "UP")
                    {
                        entity.UpCount++;
                    }
                    else
                    {
                        entity.DownCount++;
                    }
                    manager.UpdateDoubt(entity);
                    record = new UpDownRecord
                    {
                        UserId = userId,
                        DoubtId = doubtId,
                        UpDown = operation,
                        CreateTime = DateTime.Now,
                    };
                    manager.AddUpDownRecord(record);
                }
                biz.CommitTran();
            }
        }
        public DoubtInfo_Query QueryDoubtInfoById(string doubtId)
        {
            using (var manager = new DoubtManager())
            {
                var entity = manager.GetDoubtById(doubtId);

                var info = new DoubtInfo_Query();
                ObjectConvert.ConverEntityToInfo<Doubt, DoubtInfo_Query>(entity, ref info);
                return info;
            }
        }
        public IList QueryDoubtList_Admin(string key, string category, int pageIndex, int pageSize, out int totalCount)
        {
            using (var manager = new DoubtManager())
            {
                var list = manager.QueryDoubtList_Admin(key, category, pageIndex, pageSize, out totalCount);
                return list;
            }
        }
        public IList QueryDoubtList_Web(string key, string category, string userId, int pageIndex, int pageSize, out int totalCount)
        {
            using (var manager = new DoubtManager())
            {
                var list = manager.QueryDoubtList_Web(key, category, userId, pageIndex, pageSize, out totalCount);
                return list;
            }
        }

        #endregion

        #region 文章资讯

        public void UpdateArticleStaticPath(string articleId, string staticPath, string preId, string nextId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ArticleManager();
                var entity = manager.QueryArticle(articleId);
                if (entity == null)
                    throw new ArgumentException("指定编号的文章不存在");
                var pre = manager.QueryArticle(preId);
                var next = manager.QueryArticle(nextId);

                entity.StaticPath = staticPath;
                entity.PreStaticPath = pre == null ? string.Empty : pre.StaticPath;
                entity.NextStaticPath = next == null ? string.Empty : next.StaticPath;
                manager.UpdateArticle(entity);

                if (pre != null)
                {
                    pre.NextStaticPath = staticPath;
                    pre.NextId = entity.Id;
                    pre.NextTitle = entity.Title.Length > 50 ? entity.Title.Substring(0, 50) : entity.Title;
                    manager.UpdateArticle(pre);
                }
                if (next != null)
                {
                    next.PreStaticPath = staticPath;
                    next.PreId = entity.Id;
                    next.PreTitle = entity.Title.Length > 50 ? entity.Title.Substring(0, 50) : entity.Title;
                    manager.UpdateArticle(next);
                }

                biz.CommitTran();
            }
        }

        private string FomartLink(string keyWords, string link)
        {
            return string.Format("<a href='{0}' title='{1}' target='_blank'>{1}</a>", link, keyWords);
        }

        private string DeepReplaceContent(string content, List<KeywordOfArticle> keyWords)
        {
            var keys = keyWords.OrderBy(p => p.KeyWords.Length).ToList();
            foreach (var item in keys)
            {
                if (!content.Contains(item.KeyWords)) continue;

                //var deepKeyWordsList = keys.Where(p => p.KeyWords.Contains(item.KeyWords) && p.KeyWords != item.KeyWords).ToList();
                //var needReplace = true;
                //foreach (var deep in deepKeyWordsList)
                //{
                //    if (content.Contains(">" + deep.KeyWords) || content.Contains(deep.KeyWords + "<"))
                //    {
                //        needReplace = false;
                //        break;
                //    }
                //}

                //while (content.IndexOf(item.KeyWords) > -1)
                //{
                //    var temp = content.Substring(content.IndexOf(item.KeyWords), item.KeyWords.Length+1);
                //    if (temp.Contains(">") || temp.Contains("<"))
                //        continue;
                //}

                content = content.Replace(item.KeyWords, FomartLink(item.KeyWords, item.Link));
            }
            return content;
            //foreach (var item in keyWords)
            //{
            //    //文章不包括key
            //    if (!content.Contains(item.KeyWords)) continue;

            //    //找出上级key
            //    var deepKeyWordsList = keyWords.Where(p => p.KeyWords.Contains(item.KeyWords) && p.KeyWords != item.KeyWords).ToList();
            //    if (deepKeyWordsList.Count == 0)
            //    {
            //        //没有更高级的key , 替换当前key
            //        return content.Replace(item.KeyWords, FomartLink(item.KeyWords, item.Link));
            //    }
            //    else
            //    {
            //        //还有更高级的key
            //        //content = content.Replace(item.KeyWords, FomartLink(item.KeyWords, item.Link));
            //        content = DeepReplaceContent(content, deepKeyWordsList);
            //    }
            //}
            //return content;
        }

        public string SubmitArticle(ArticleInfo_Add article)
        {
            var id = BusinessHelper.GetArticleId();
            var manager = new ArticleManager();
            var keyWordsList = manager.QuerKeywordOfArticle();
            var content = DeepReplaceContent(article.Description, keyWordsList);

            var tagList = new List<string>();
            tagList.Add("script");
            content = UsefullHelper.ReplaceHtmlTag(content, tagList);

            var staticPath = string.Format("/statichtml/zixun/details/{0}/{1}.html", DateTime.Now.ToString("yyyyMMdd"), id);
            var lastId = string.Empty;
            var lastTitle = string.Empty;
            var lastStaticPath = string.Empty;
            var last = manager.QueryLastArticle(article.Category);
            if (last != null)
            {
                lastId = last.Id;
                lastTitle = last.Title.Length > 50 ? last.Title.Substring(0, 50) : last.Title;
                lastStaticPath = last.StaticPath;

                last.NextId = id;
                last.NextTitle = article.Title.Length > 50 ? article.Title.Substring(0, 50) : article.Title;
                last.NextStaticPath = staticPath;
                manager.UpdateArticle(last);
            }

            var entity = new Article
            {
                Id = id,
                GameCode = article.GameCode,
                Title = article.Title,
                KeyWords = article.KeyWords,
                DescContent = article.DescContent,
                Description = content,
                IsRedTitle = article.IsRedTitle,
                Category = article.Category,
                ShowIndex = 0,
                ReadCount = 0,
                CreateTime = DateTime.Now,
                CreateUserKey = article.CreateUserKey,
                CreateUserDisplayName = article.CreateUserDisplayName,
                UpdateTime = DateTime.Now,
                UpdateUserKey = article.CreateUserKey,
                UpdateUserDisplayName = article.CreateUserDisplayName,
                PreId = lastId,
                PreTitle = lastTitle,
                PreStaticPath = lastStaticPath,
                StaticPath = "",
            };

            manager.AddArticle(entity);
            return id;
        }
        public void UpdateArticle(string articleId, ArticleInfo_Add article)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new ArticleManager())
                {
                    var entity = manager.GetArticleById(articleId);
                    if (entity == null)
                    {
                        throw new ArgumentException("指定编号的文章不存在");
                    }

                    var keyWordsList = manager.QuerKeywordOfArticle();
                    var content = DeepReplaceContent(article.Description, keyWordsList);

                    var tagList = new List<string>();
                    tagList.Add("script");
                    content = UsefullHelper.ReplaceHtmlTag(content, tagList);

                    entity.Title = article.Title;
                    entity.KeyWords = article.KeyWords;
                    entity.DescContent = article.DescContent;
                    entity.GameCode = article.GameCode;
                    entity.Description = content;
                    entity.IsRedTitle = article.IsRedTitle;
                    entity.Category = article.Category;
                    entity.UpdateTime = DateTime.Now;
                    entity.UpdateUserKey = article.CreateUserKey;
                    entity.UpdateUserDisplayName = article.CreateUserDisplayName;
                    manager.UpdateArticle(entity);
                }
                biz.CommitTran();
            }
        }
        public void UpdateArticleIndex(Dictionary<string, int> indexCollection)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new ArticleManager())
                {
                    manager.UpdateArticleIndex(indexCollection);
                }
                biz.CommitTran();
            }
        }
        public void DeleteArticle(string articleId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new ArticleManager())
                {
                    var entity = manager.GetArticleById(articleId);
                    if (entity == null)
                    {
                        throw new ArgumentException("指定要删除的文章不存在");
                    }
                    manager.DeleteArticle(entity);
                }
                biz.CommitTran();
            }
        }
        public ArticleInfo_Query QueryArticleInfoById(string articleId, bool isAddReadCount)
        {
            using (var manager = new ArticleManager())
            {
                var entity = manager.GetArticleById(articleId);
                if (entity == null)
                {
                    throw new ArgumentException("指定编号的文章不存在");
                }
                if (isAddReadCount)
                {
                    entity.ReadCount++;
                    manager.UpdateArticle(entity);
                }
                var info = new ArticleInfo_Query();
                ObjectConvert.ConverEntityToInfo<Article, ArticleInfo_Query>(entity, ref info);
                return info;
            }
        }
        public IList QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize, out int totalCount)
        {
            using (var manager = new ArticleManager())
            {
                var list = manager.QueryArticleList(key, gameCode, category, pageIndex, pageSize, out totalCount);
                return list;
            }
        }

        public ArticleInfo_QueryCollection QueryNoStaticPathArticleList(int pageIndex, int pageSize)
        {
            var result = new ArticleInfo_QueryCollection();
            var manager = new ArticleManager();
            int totalCount = 0;
            result.ArticleList = manager.QueryNoStaticPathArticleList(pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        #endregion

        #region 查询角色



        #endregion

        #region 广告管理

        public SiteMessageBannerInfo_Collection QuerySiteMessageBannerCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new BulletinManager())
            {
                SiteMessageBannerInfo_Collection collection = new SiteMessageBannerInfo_Collection();
                int totalCount;
                collection.ListInfo = manager.QuerySiteMessageBannerCollection(title, startTime, endTime, pageIndex, pageSize, out totalCount);
                return collection;
            }
        }
        public void UpdateBannerInfo(SiteMessageBannerInfo info)
        {
            if (info == null || info.BannerId <= 0)
            {
                throw new Exception("当前广告信息无效！");
            }
            using (var manager = new BulletinManager())
            {
                SiteMessageBanner entity = new SiteMessageBanner();
                entity = manager.GetBannerManagerInfo(info.BannerId.ToString());
                if (entity == null)
                {
                    throw new Exception("未查询到广告信息！");
                }
                entity.BannerIndex = info.BannerIndex;
                entity.BannerTitle = info.BannerTitle;
                entity.BannerType = info.BannerType;
                entity.ImageUrl = info.ImageUrl;
                entity.IsEnable = info.IsEnable;
                entity.JumpUrl = info.JumpUrl;
                manager.UpdateBannerInfo(entity);
            }

        }
        public void AddBannerInfo(SiteMessageBannerInfo info)
        {
            if (info == null)
            {
                throw new Exception("当前广告信息无效！");
            }
            using (var manager = new BulletinManager())
            {
                SiteMessageBanner entity = new SiteMessageBanner();
                ObjectConvert.ConverInfoToEntity(info, ref entity);
                entity.CreateTime = DateTime.Now;
                manager.AddBannerInfo(entity);
            }
        }
        public SiteMessageBannerInfo GetBannerManagerInfo(string bannerId)
        {
            using (var manager = new BulletinManager())
            {
                var entity = manager.GetBannerManagerInfo(bannerId);
                if (entity == null)
                {
                    throw new Exception("未查询到广告数据！");
                }
                SiteMessageBannerInfo info = new SiteMessageBannerInfo();
                ObjectConvert.ConverEntityToInfo(entity, ref info);
                return info;
            }
        }
        /// <summary>
        /// 前台查询广告信息
        /// </summary>
        /// <param name="bannerType"></param>
        /// <param name="returnRecord"></param>
        /// <returns></returns>
        public SiteMessageBannerInfo_Collection QuerySitemessageBanngerList_Web(BannerType bannerType, int returnRecord = 10)
        {
            using (var manager = new BulletinManager())
            {
                SiteMessageBannerInfo_Collection collection = new SiteMessageBannerInfo_Collection();
                collection.ListInfo = manager.QuerySitemessageBanngerList_Web(bannerType, returnRecord);
                return collection;
            }
        }

        /// <summary>
        /// 删除广告图
        /// </summary>
        public void DeleteBanner(int bannerId)
        {
            var manager = new BulletinManager();
            var entity = manager.QueryBannerManager(bannerId);
            manager.DeleteBanner(entity);
        }

        #endregion

        #region 网站优惠活动

        public void UpdateSiteActivity(SiteActivityInfo info)
        {
            if (info == null || info.Id <= 0)
            {
                throw new Exception("当前活动信息无效！");
            }
            using (var manager = new SiteActivityManager())
            {
                SiteActivity entity = new SiteActivity();
                entity = manager.QuerySiteActivity(info.Id);
                if (entity == null)
                {
                    throw new Exception("未查询到活动信息！");
                }
                entity.ArticleUrl = info.ArticleUrl;
                //entity.ImageUrl = info.ImageUrl;
                entity.Titile = info.Titile;
                entity.StartTime = info.StartTime;
                entity.EndTime = info.EndTime;

                manager.UpdateSiteActivity(entity);
            }
        }

        public void AddSiteActivity(SiteActivityInfo info)
        {
            if (info == null)
            {
                throw new Exception("当前广告信息无效！");
            }
            using (var manager = new SiteActivityManager())
            {
                SiteActivity entity = new SiteActivity();
                ObjectConvert.ConverInfoToEntity(info, ref entity);
                manager.AddSiteActivity(entity);
            }
        }

        /// <summary>
        /// 查询所有活动配置
        /// </summary>
        public SiteActivityInfo_Collection QueryAllSiteActivity()
        {
            using (var manager = new SiteActivityManager())
            {
                SiteActivityInfo_Collection collection = new SiteActivityInfo_Collection();
                collection.ListInfo = manager.QueryAllSiteActivity();
                return collection;
            }
        }

        /// <summary>
        /// 查询某一个活动配置
        /// </summary>
        public SiteActivityInfo QuerySiteActivityInfo(int id)
        {
            using (var manager = new SiteActivityManager())
            {
                return manager.QuerySiteActivityInfo(id);
            }
        }

        /// <summary>
        /// 删除广告图
        /// </summary>
        public void DeleteSiteActivity(int id)
        {
            var manager = new SiteActivityManager();
            var entity = manager.QuerySiteActivity(id);
            manager.DeleteSiteActivity(entity);
        }

        #endregion

        #region 手工返点

        /// <summary>
        /// 返点接口
        /// </summary>
        public void ManualAgentPayIn(string schemeId)
        {
            var order = new SchemeManager().QueryOrderDetail(schemeId);
            if (order == null)
                throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));
            if (order.TicketStatus != TicketStatus.Ticketed)
                throw new LogicException("订单未出票完成，不能返点");
            if (order.IsVirtualOrder)
                throw new LogicException("虚拟订单不能返点");

            string currentUserId = string.Empty;
            decimal currentBetMoney = 0M;
            string currentGameCode = string.Empty;
            bool currentIsAgent = false;
            OCAgentBusiness busi = new OCAgentBusiness();
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                //查询订单信息
                var sportsManager = new Sports_Manager();

                //真实订单，处理返点数据
                var gameCode = order.GameCode.ToUpper();
                var gameType = order.GameType.ToUpper();
                var userId = order.UserId;

                var msg = string.Empty;
                //合买判断
                if (order.SchemeType == SchemeType.TogetherBetting)
                {
                    var main = sportsManager.QuerySports_Together(schemeId);
                    if (main == null)
                    {
                        msg = string.Format("找不到合买订单:{0}", schemeId);
                        //throw new Exception(string.Format("找不到合买订单:{0}", schemeId));
                    }
                    //if (main.ProgressStatus != TogetherSchemeProgress.Finish)
                    //    throw new Exception(string.Format("合买订单:{0} 状态不正确", schemeId));
                    var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                    if (sysJoinEntity != null && sysJoinEntity.RealBuyCount > 0)
                    {
                        msg = "网站参与保底，不返点";
                        //throw new Exception("网站参与保底，不返点");
                    }

                    if (main.SoldCount + main.Guarantees < main.TotalCount)
                        throw new Exception("订单未满员，不执行返点");
                    //realMoney -= main.SystemGuarantees * main.Price;
                }

                var realMoney = 0M; ;
                var totalPayRebateMoney = 0M;
                var agentManager = new OCAgentManager();
                if (string.IsNullOrEmpty(msg))
                {
                    //没有异常，执行返点
                    var noGameTypeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3" };
                    if (noGameTypeArray.Contains(gameCode))
                        gameType = string.Empty;

                    //真实投注金额，订单成功金额
                    realMoney = order.TotalMoney;
                    //查询用户自身返点
                    var balanceManager = new UserBalanceManager();
                    var user = balanceManager.QueryUserRegister(userId);
                    currentIsAgent = user.IsAgent;

                    //去掉红包参与金额
                    var redBagJoinMoney = order.RedBagMoney;// new FundManager().QuerySchemeRedBagTotalJoinMoney(schemeId);
                    realMoney -= redBagJoinMoney;
                    //递归调用
                    int rebateType = 0;
                    var arrGameCode = new string[] { "JCZQ", "JCLQ", "BJDC" };
                    if (!string.IsNullOrEmpty(order.PlayType) && arrGameCode.Contains(order.GameCode))
                    {
                        if (order.PlayType == "1_1")
                            rebateType = 1;
                    }
                    totalPayRebateMoney = busi.PayOrderRebate(agentManager, user, schemeId, userId, order.SchemeType, gameCode, gameType, order.TotalMoney, realMoney, 0, rebateType);
                }

                biz.CommitTran();
            }
            //计算代理销量
            busi.CalculationAgentSales(currentUserId, currentGameCode, currentBetMoney, currentIsAgent, 0);
        }

        #endregion

        #region 系统操作日志记录

        public void AddSysOperationLog(string userId, string operUserId, string menuName, string desc)
        {
            using (var manager = new SysLogManager())
            {
                manager.AddSysOperationLog(new External.Domain.Entities.AdminMenu.SysOperationLog
                {
                    UserId = userId,
                    OperUserId = operUserId,
                    CreateTime = DateTime.Now,
                    Description = desc,
                    MenuName = menuName,
                });
            }
        }

        public SysOperationLog_Collection QuerySysOperationList(string menuName, string userId, string operUserId, DateTime startTime, DateTime endTimen, int pageIndex, int pageSize)
        {
            using (var manager = new SysLogManager())
            {
                int totalCount = 0;
                SysOperationLog_Collection collection = new SysOperationLog_Collection();
                var result = manager.QuerySysOperationList(menuName, userId, operUserId, startTime, endTimen, pageIndex, pageSize, out totalCount);
                collection.TotalCount = totalCount;
                collection.LogInfoList = result;
                return collection;
            }
        }

        #endregion

        #region 优化函数

        public ArticleInfo_QueryCollection QueryArticleList_YouHua(string category, string gameCode, int pageIndex, int pageSize)
        {
            using (var manager = new ArticleManager())
            {
                if (string.IsNullOrEmpty(category))
                    throw new Exception("未查询到文章类别");
                var array = category.Split('|');
                var gameCodeArray = gameCode.Split('|');
                var list = manager.QueryArticleList_YouHua(array, gameCodeArray, pageIndex, pageSize);
                return list;
            }
        }

        /// <summary>
        /// 查询最新中奖
        /// </summary>
        public LotteryNewBonusInfoCollection QueryLotteryNewBonusInfoList(int count)
        {
            var r = new LotteryNewBonusInfoCollection();
            var list = new SiteActivityManager().QueryLotteryNewBonusInfoList(count);
            r.AddRange(list);
            return r;
        }

        #endregion
    }
}
