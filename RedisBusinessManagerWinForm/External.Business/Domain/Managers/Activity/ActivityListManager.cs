using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Common.Utilities;
using System.Collections;
using NHibernate.Linq;
using External.Core.Agnet;
using Common.Database.ORM;
using GameBiz.Domain.Entities;
using GameBiz.Core;
using External.Domain.Entities.Activity;
using External.Core.Task;


namespace External.Domain.Managers.Activity
{
    public class ActivityListManager : GameBizEntityManagement
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        public List<ActivityListInfo> QueryActivInfoList(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from a in this.Session.Query<ActivityList>()
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
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询某活动
        /// </summary>
        public ActivityListInfo QueryActivInfo(string activeName)
        {
            Session.Clear();
            var query = from a in this.Session.Query<ActivityList>()
                        where a.ActiveName == activeName
                        select new ActivityListInfo()
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
            return query.FirstOrDefault();
        }
    }
}
