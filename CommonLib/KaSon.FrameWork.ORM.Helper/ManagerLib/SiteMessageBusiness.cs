using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.Common.Utilities;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SiteMessageBusiness : DBbase
    {
        public UserIdeaInfo_QueryCollection QueryMyUserIdeaList(string createUserId, int pageIndex, int pageSize)
        {
            var result = new UserIdeaInfo_QueryCollection();
            var manager = new UserIdeaManager();
            var totalCount = 0;
            result.UserIdeaList = manager.QueryMyUserIdeaList(createUserId, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            return result;
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

        public void UpdateArticleStaticPath(string articleId, string staticPath, string preId, string nextId)
        {

            DB.Begin();
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
            DB.Commit();

        }

        public void AddSysOperationLog(string userId, string operUserId, string menuName, string desc)
        {
            var manager = new SysLogManager();
            manager.AddSysOperationLog(new C_Sys_OperationLog
            {
                UserId = userId,
                OperUserId = operUserId,
                CreateTime = DateTime.Now,
                Description = desc,
                MenuName = menuName,
            });
        }

        public SysOperationLog_Collection QuerySysOperationList(string menuName, string userId, string operUserId, DateTime startTime, DateTime endTimen, int pageIndex, int pageSize)
        {
            var manager = new SysLogManager();
            int totalCount = 0;
            SysOperationLog_Collection collection = new SysOperationLog_Collection();
            var result = manager.QuerySysOperationList(menuName, userId, operUserId, startTime, endTimen, pageIndex, pageSize, out totalCount);
            collection.TotalCount = totalCount;
            collection.LogInfoList = result;
            return collection;
        }

        /// <summary>
        /// 查询后台管理的公告列表
        /// </summary>
        public BulletinInfo_Collection QueryManagementBulletins(string key, EnableStatus status, int priority, int isPutTop, int pageIndex, int pageSize)
        {
            var bulletinManager = new BulletinManager();
            {
                int totalCount;
                var list = bulletinManager.QueryAdminBulletinList(key, status, priority, isPutTop, pageIndex, pageSize, out totalCount);
                var collection = new BulletinInfo_Collection
                {
                    TotalCount = totalCount,
                };
                collection.BulletinList = list;
                return collection;
            }
        }

        public void UpdateBulletinStatus(long id, EnableStatus status, string updateBy)
        {
            var bulletinManager = new BulletinManager();
            var entity = bulletinManager.GetBulletinById(id);
            entity.Status = (int)status;
            entity.UpdateBy = updateBy;
            bulletinManager.UpdateBulletin(entity);
        }

        /// <summary>
        /// 发布公告
        /// </summary>
        public void PublishBulletin(E_SiteMessage_Bulletin_List bulletin, string createBy)
        {
            var bulletinManager = new BulletinManager();
            bulletin.CreateBy = createBy;
            bulletin.UpdateBy = createBy;
            bulletinManager.AddBulletin(bulletin);
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        public void UpdateBulletin(E_SiteMessage_Bulletin_List bulletin, string updateBy)
        {
            var bulletinManager = new BulletinManager();
            var entity = bulletinManager.GetBulletinById(bulletin.Id);
                bulletin.CreateTime = entity.CreateTime;
                bulletin.CreateBy = entity.CreateBy;
                bulletin.UpdateBy = updateBy;
                bulletinManager.UpdateBulletin(bulletin);
        }

        #region 文章相关
        //public IList QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize, out int totalCount)
        //{
        //    var manager = new ArticleManager();
        //    var list = manager.QueryArticleList(key, gameCode, category, pageIndex, pageSize, out totalCount);
        //    return list;
        //}
        public void UpdateArticleIndex(Dictionary<string, int> indexCollection)
        {
            var manager = new ArticleManager();
            manager.UpdateArticleIndex(indexCollection);
        }

        public void DeleteArticle(string articleId)
        {
            var manager = new ArticleManager();
            var entity = manager.GetArticleById(articleId);
            if (entity == null)
            {
                throw new ArgumentException("指定要删除的文章不存在");
            }
            manager.DeleteArticle(entity);
        }

        public E_SiteMessage_Article_List QueryArticleInfoById(string articleId, bool isAddReadCount)
        {
            var manager = new ArticleManager();
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
            return entity;
        }
        public string SubmitArticle(E_SiteMessage_Article_List article)
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
            //var entity = new Article
            //{
            //    Id = id,
            //    GameCode = article.GameCode,
            //    Title = article.Title,
            //    KeyWords = article.KeyWords,
            //    DescContent = article.DescContent,
            //    Description = content,
            //    IsRedTitle = article.IsRedTitle,
            //    Category = article.Category,
            //    ShowIndex = 0,
            //    ReadCount = 0,
            //    CreateTime = DateTime.Now,
            //    CreateUserKey = article.CreateUserKey,
            //    CreateUserDisplayName = article.CreateUserDisplayName,
            //    UpdateTime = DateTime.Now,
            //    UpdateUserKey = article.CreateUserKey,
            //    UpdateUserDisplayName = article.CreateUserDisplayName,
            //    PreId = lastId,
            //    PreTitle = lastTitle,
            //    PreStaticPath = lastStaticPath,
            //    StaticPath = "",
            //};
            article.Id = id;
            article.GameCode = article.GameCode;
            article.ShowIndex = 0;
            article.ReadCount = 0;
            article.PreId = lastId;
            article.PreTitle = lastTitle;
            article.PreStaticPath = lastStaticPath;
            article.StaticPath = "";
            article.Description = content;
            article.CreateTime = DateTime.Now;
            article.UpdateTime = DateTime.Now;
            manager.AddArticle(article);
            return id;
        }
        public void UpdateArticle(E_SiteMessage_Article_List article)
        {
            var manager = new ArticleManager();
            {
                var entity = manager.GetArticleById(article.Id);
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
        }

        private string DeepReplaceContent(string content, List<E_SiteMessage_KeywordOfArticle> keyWords)
        {
            var keys = keyWords.OrderBy(p => p.KeyWords.Length).ToList();
            foreach (var item in keys)
            {
                if (!content.Contains(item.KeyWords)) continue;
                content = content.Replace(item.KeyWords, FomartLink(item.KeyWords, item.Link));
            }
            return content;
        }
        private string FomartLink(string keyWords, string link)
        {
            return string.Format("<a href='{0}' title='{1}' target='_blank'>{1}</a>", link, keyWords);
        }
        #endregion
        #region 广告相关
        /// <summary>
        /// 删除广告图
        /// </summary>
        public void DeleteBanner(int bannerId)
        {
            var manager = new BulletinManager();
            var entity = manager.QueryBannerManager(bannerId);
            manager.DeleteBanner(entity);
        }

        public SiteMessageBannerInfo_Collection QuerySiteMessageBannerCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var manager = new BulletinManager();
            SiteMessageBannerInfo_Collection collection = new SiteMessageBannerInfo_Collection();
            int totalCount;
            collection.ListInfo = manager.QuerySiteMessageBannerCollection(title, startTime, endTime, pageIndex, pageSize, out totalCount);
            collection.TotalCount = totalCount;
            return collection;
        }
        public void UpdateBannerInfo(E_Sitemessage_Banner info)
        {
            if (info == null || info.BannerId <= 0)
            {
                throw new Exception("当前广告信息无效！");
            }
            var manager = new BulletinManager();
            var entity = manager.GetBannerManagerInfo(info.BannerId.ToString());
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
        public void AddBannerInfo(E_Sitemessage_Banner info)
        {
            if (info == null)
            {
                throw new Exception("当前广告信息无效！");
            }
            var manager = new BulletinManager();
            info.CreateTime = DateTime.Now;
            manager.AddBannerInfo(info);
        }
        #endregion
    }
}
