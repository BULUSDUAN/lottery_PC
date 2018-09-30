using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
     public class SiteMessageBusiness:DBbase
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
    }
}
