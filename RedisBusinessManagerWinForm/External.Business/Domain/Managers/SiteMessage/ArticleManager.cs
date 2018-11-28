using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain;
using Common.Business;
using NHibernate.Linq;
using Common;
using GameBiz.Domain.Entities;
using NHibernate.Criterion;
using System.Collections;
using NHibernate;
using External.Domain.Entities.SiteMessage;
using GameBiz.Business;
using External.Core.SiteMessage;

namespace External.Domain.Managers.SiteMessage
{
    public class ArticleManager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddArticle(Article article)
        {
            article.CreateTime = DateTime.Now;
            article.UpdateTime = DateTime.Now;
            Add<Article>(article);
        }
        public void UpdateArticle(Article article)
        {
            article.UpdateTime = DateTime.Now;
            Update<Article>(article);
        }
        public Article QueryArticle(string articleId)
        {
            Session.Clear();
            return this.Session.Query<Article>().FirstOrDefault(p => p.Id == articleId);
        }
        public void UpdateArticleIndex(Dictionary<string, int> indexCollection)
        {
            foreach (var item in indexCollection)
            {
                var hql = "update Article d set d.ShowIndex=? where d.Id=?";
                var query = Session.CreateQuery(hql)
                    .SetInt32(0, item.Value)
                    .SetString(1, item.Key);
                query.ExecuteUpdate();
            }
            Session.Flush();
        }
        public void DeleteArticle(Article article)
        {
            Delete<Article>(article);
        }
        public Article GetArticleById(string articleId)
        {
            return GetByKey<Article>(articleId);
        }
        public IList QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryArticleList"));
            query = query.AddInParameter("Key", key);
            query = query.AddInParameter("GameCode", gameCode);
            query = query.AddInParameter("Category", category);
            query = query.AddInParameter("PageIndex", pageIndex);
            query = query.AddInParameter("PageSize", pageSize);
            query = query.AddOutParameter("TotalCount", "Int32");
            var list = query.List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }


        public List<ArticleInfo_Query> QueryNoStaticPathArticleList(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from a in this.Session.Query<Article>()
                        where a.StaticPath == ""
                        orderby a.CreateTime descending
                        select new ArticleInfo_Query
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
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public Article QueryLastArticle(string category)
        {
            Session.Clear();
            return Session.Query<Article>().Where(p => p.Category == category).OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<KeywordOfArticle> QuerKeywordOfArticle()
        {
            Session.Clear();
            return Session.Query<KeywordOfArticle>().Where(p => p.IsEnable && p.KeyWords != string.Empty && p.Link != string.Empty).ToList();
        }

        #region 优化函数

        public ArticleInfo_QueryCollection QueryArticleList_YouHua(string[] array_category, string[] gameCodeArray, int pageIndex, int pageSize)
        {
            Session.Clear();
            ArticleInfo_QueryCollection collection = new ArticleInfo_QueryCollection();
            // 通过数据库存储过程进行查询
            var query = from a in Session.Query<Article>()
                        where array_category.Contains(a.Category)
                        && gameCodeArray.Contains(a.GameCode)
                        orderby a.CreateTime descending
                        select new ArticleInfo_Query
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
                        };
            collection.TotalCount = query.Count();
            collection.ArticleList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return collection;

        }

        #endregion

    }
}
