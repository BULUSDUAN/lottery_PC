using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class ArticleManager:DBbase
    {
        public List<ArticleInfo_Query> QueryNoStaticPathArticleList(int pageIndex, int pageSize, out int totalCount)
        {
     
            var query = from a in DB.CreateQuery<E_SiteMessage_Article_List>()
                        where a.StaticPath == ""
                        orderby a.CreateTime descending
                        select a;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(a=>new ArticleInfo_Query
            {
                Category = a.Category,
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
                StaticPath = a.StaticPath,
                Title = a.Title,
                UpdateTime = a.UpdateTime,
                UpdateUserDisplayName = a.UpdateUserDisplayName,
                UpdateUserKey = a.UpdateUserKey,
                NextStaticPath = a.NextStaticPath,
                PreStaticPath = a.PreStaticPath,
            }).ToList();
        }

        public E_SiteMessage_Article_List QueryArticle(string articleId)
        {
     
            return DB.CreateQuery<E_SiteMessage_Article_List>().Where(p => p.Id == articleId).FirstOrDefault();
        }

        public void UpdateArticle(E_SiteMessage_Article_List article)
        {
            article.UpdateTime = DateTime.Now;
            DB.GetDal<E_SiteMessage_Article_List>().Update(article);
        }
    }
}
