using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Core;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Business;

namespace GameBiz.Domain.Managers
{
    public class TogetherHotUserManager : GameBizEntityManagement
    {
        public void AddTogetherHotUser(TogetherHotUser entity)
        {
            this.Add(entity);
        }
        public void UpdateTogetherHotUser(TogetherHotUser entity)
        {
            this.Update(entity);
        }
        public TogetherHotUser TogetherHotUserById(string userId)
        {
            Session.Clear();
            return Session.Query<TogetherHotUser>().FirstOrDefault(p => p.UserId == userId);
        }

        public List<TogetherHotUser> QueryTogetherHotUserList()
        {
            Session.Clear();
            return Session.Query<TogetherHotUser>().ToList();
        }
        public void DeleteTogetherHotUser(TogetherHotUser entity)
        {
            this.Delete<TogetherHotUser>(entity);
        }

        /// <summary>
        /// 查询是否有该红人
        /// </summary>
        public int QueryTogether(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<TogetherHotUser>().Where(p => p.UserId == userId).Count();
        }

        #region 关于合买红人的订单

        public List<TogetherHotUserOrderInfo> QueryTogetherHotUserOrderInfo(string[] userIdArray)
        {
            Session.Clear();
            var query = from r in this.Session.Query<Sports_Together>()
                        where userIdArray.Contains(r.CreateUserId)
                        && (r.ProgressStatus == TogetherSchemeProgress.Finish || r.ProgressStatus == TogetherSchemeProgress.SalesIn || r.ProgressStatus == TogetherSchemeProgress.Standard)
                        && r.StopTime >= DateTime.Now
                        select new TogetherHotUserOrderInfo
                        {
                            CreateUserId = r.CreateUserId,
                            CreateTime = r.CreateTime,
                            GameCode = r.GameCode,
                            GameType = r.GameType,
                            PlayType = r.PlayType,
                            Progress = r.Progress,
                            ProgressStatus = r.ProgressStatus,
                            SchemeId = r.SchemeId,
                            StopTime = r.StopTime,
                            TotalMoney = r.TotalMoney,
                        };
            return query.ToList();
        }


        /// <summary>
        /// 查询红人合买列表
        /// </summary>
        public List<TogetherHotUserInfo> QueryTogetherHotUserInfo()
        {
            Session.Clear();
            var query = from t in this.Session.Query<TogetherHotUser>()
                        join pu in this.Session.Query<Blog_ProfileBonusLevel>() on t.UserId equals pu.UserId
                        join u in this.Session.Query<UserRegister>() on t.UserId equals u.UserId
                        join a in this.Session.Query<UserAttentionSummary>() on t.UserId equals a.UserId
                        orderby t.CreateTime ascending
                        select new TogetherHotUserInfo
                        {
                            AttentionUserCount = a.BeAttentionUserCount,
                            CreateTime = u.CreateTime,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = u.UserId,
                            MaxLevelName = pu.MaxLevelName,
                            WeeksWinMoney = t.WeeksWinMoney,
                        };
            return query.ToList();
        }

        #endregion

    }
}
