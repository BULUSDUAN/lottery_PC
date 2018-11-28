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
    public class UserIdeaManager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddUserIdea(UserIdea userIdea)
        {
            userIdea.CreateTime = DateTime.Now;
            userIdea.UpdateTime = DateTime.Now;
            Add<UserIdea>(userIdea);
        }
        public void UpdateUserIdea(UserIdea userIdea)
        {
            userIdea.UpdateTime = DateTime.Now;
            Update<UserIdea>(userIdea);
        }
        public UserIdea GetUserIdeaById(int id)
        {
            return SingleByKey<UserIdea>(id);
        }
        public IList<UserIdea> QueryUserIdeaList(List<string> statusList, List<string> categoryList, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = Session.CreateCriteria<UserIdea>()
                .Add(Restrictions.InG<string>("Status", statusList))
                .Add(Restrictions.Between("CreateTime",starTime,endTime));
            if (categoryList.Count > 0)
            {
                query = query.Add(Restrictions.InG<string>("Category", categoryList));
            }
            totalCount = CriteriaTransformer.Clone(query).SetProjection(Projections.Count<UserIdea>(r => r.Id)).UniqueResult<int>();
            return query
                .AddOrder(Order.Desc("CreateTime"))
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .List<UserIdea>();
        }
        public List<UserIdeaInfo_Query> QueryMyUserIdeaList(string createUserId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from i in this.Session.Query<UserIdea>()
                        where i.CreateUserId == createUserId
                        orderby i.UpdateTime descending
                        select new UserIdeaInfo_Query
                        {
                            Category = string.IsNullOrEmpty(i.Category) ? string.Empty : i.Category,
                            CreateTime = i.CreateTime,
                            CreateUserDisplayName = i.CreateUserDisplayName,
                            CreateUserId = i.CreateUserId,
                            CreateUserMoibile = string.IsNullOrEmpty(i.CreateUserMoibile) ? string.Empty : i.CreateUserMoibile,
                            Description = string.IsNullOrEmpty(i.Description) ? string.Empty : i.Description,
                            Id = i.Id,
                            IsAnonymous = i.IsAnonymous,
                            Status = i.Status,
                            UpdateTime = i.UpdateTime,
                            UpdateUserDisplayName = string.IsNullOrEmpty(i.UpdateUserDisplayName) ? string.Empty : i.UpdateUserDisplayName,
                            UpdateUserId = string.IsNullOrEmpty(i.UpdateUserId) ? string.Empty : i.UpdateUserId,
                            ManageReply = string.IsNullOrEmpty(i.ManageReply) ? string.Empty : i.ManageReply,
                            PageOpenSpeed = i.PageOpenSpeed == null ? 0M : decimal.Parse(i.PageOpenSpeed.ToString()),
                            InterfaceBeautiful = i.InterfaceBeautiful == null ? 0M : decimal.Parse(i.InterfaceBeautiful.ToString()),
                            ComposingReasonable = i.ComposingReasonable == null ? 0M : decimal.Parse(i.ComposingReasonable.ToString()),
                            OperationReasonable = i.OperationReasonable == null ? 0M : decimal.Parse(i.OperationReasonable.ToString()),
                            ContentConveyDistinct = i.ContentConveyDistinct == null ? 0M : decimal.Parse(i.ContentConveyDistinct.ToString()),
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
