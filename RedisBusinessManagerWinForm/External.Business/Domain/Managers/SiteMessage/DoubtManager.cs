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

namespace External.Domain.Managers.SiteMessage
{
    public class DoubtManager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddDoubt(Doubt doubt)
        {
            doubt.CreateTime = DateTime.Now;
            doubt.UpdateTime = DateTime.Now;
            Add<Doubt>(doubt);
        }
        public void UpdateDoubt(Doubt doubt)
        {
            doubt.UpdateTime = DateTime.Now;
            Update<Doubt>(doubt);
        }
        public void UpdateDoubtIndex(Dictionary<string, int> indexCollection)
        {
            foreach (var item in indexCollection)
            {
                var hql = "update Doubt d set d.ShowIndex=? where d.Id=?";
                var query = Session.CreateQuery(hql)
                    .SetInt32(0, item.Value)
                    .SetString(1, item.Key);
                query.ExecuteUpdate();
            }
            Session.Flush();
        }
        public void DeleteDoubt(Doubt doubt)
        {
            Delete<Doubt>(doubt);
        }
        public Doubt GetDoubtById(string doubtId)
        {
            return SingleByKey<Doubt>(doubtId);
        }
        public void AddUpDownRecord(UpDownRecord record)
        {
            record.CreateTime = DateTime.Now;
            Add<UpDownRecord>(record);
        }
        public void DeleteUpDownRecord(string doubtId)
        {
            Session.Delete("from UpDownRecord r where r.DoubtId='" + doubtId + "'");
        }
        public UpDownRecord GetUpDownRecord(string userId, string doubtId)
        {
            Session.Clear();
            return Session.CreateCriteria<UpDownRecord>()
                .Add(Restrictions.Eq("UserId", userId))
                .Add(Restrictions.Eq("DoubtId", doubtId))
                .UniqueResult<UpDownRecord>();
        }
        public IList QueryDoubtList_Admin(string key, string category, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryDoubtList_Admin"));
            query = query.AddInParameter("Key", key);
            query = query.AddInParameter("Category", category);
            query = query.AddInParameter("PageIndex", pageIndex);
            query = query.AddInParameter("PageSize", pageSize);
            query = query.AddOutParameter("TotalCount", "Int32");
            var list = query.List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }
        public IList QueryDoubtList_Web(string key, string category, string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryDoubtList_Web"));
            query = query.AddInParameter("Key", key);
            query = query.AddInParameter("Category", category);
            query = query.AddInParameter("UserId", userId);
            query = query.AddInParameter("PageIndex", pageIndex);
            query = query.AddInParameter("PageSize", pageSize);
            query = query.AddOutParameter("TotalCount", "Int32");
            var list = query.List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }
    }
}
