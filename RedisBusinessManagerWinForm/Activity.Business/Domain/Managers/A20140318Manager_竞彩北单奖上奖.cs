using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Activity.Business;
using GameBiz.Business;
using NHibernate.Linq;
using A20140318_竞彩北单奖上奖 = Activity.Domain.Entities.A20140318_竞彩北单奖上奖;

namespace Activity.Domain.Managers
{
    public class A20140318Manager_竞彩北单奖上奖 : GameBizEntityManagement
    {
        public void AddRecord(A20140318_竞彩北单奖上奖 entity)
        {
            Add(entity);
        }

        public IList<A20140318_竞彩北单奖上奖> QueryA20140318_竞彩北单奖上奖(string userId, int schemeType, string schemeId, string gameCode, string gameType, string issueNumber, int pageIndex, int pageSize, out int totalCount)
        {
            var predicate = PredicateExtensions.True<A20140318_竞彩北单奖上奖>();
            if (!string.IsNullOrEmpty(userId.Trim()))
            {
                predicate = predicate.And(o => o.UserId == userId.Trim());
            }
            if (schemeType != 0)
            {
                predicate = predicate.And(o => Convert.ToInt32(o.SchemeType) == schemeType);
            }
            if (!string.IsNullOrEmpty(schemeId.Trim()))
            {
                predicate = predicate.And(o => o.SchemeId == schemeId.Trim());
            }
            if (!string.IsNullOrEmpty(gameCode.Trim()))
            {
                predicate = predicate.And(o => o.GameCode.ToLower() == gameCode.Trim().ToLower());
            }
            if (!string.IsNullOrEmpty(gameType.Trim()))
            {
                predicate = predicate.And(o => o.GameType == gameType.Trim());
            }
            if (!string.IsNullOrEmpty(issueNumber.Trim()))
            {
                predicate = predicate.And(o => o.IssuseNumber == issueNumber.Trim());
            }
            var list = this.Session.Query<A20140318_竞彩北单奖上奖>().Where(predicate);
            totalCount = list.Count();
            return list.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
