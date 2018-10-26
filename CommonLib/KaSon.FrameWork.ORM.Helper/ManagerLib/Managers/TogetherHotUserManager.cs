using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
   public class TogetherHotUserManager:DBbase
    {
        /// <summary>
        /// 查询红人合买列表
        /// </summary>
        public List<TogetherHotUserInfo> QueryTogetherHotUserInfo()
        {
         
            var query = from t in DB.CreateQuery<C_TogetherHotUser>()
                        join pu in DB.CreateQuery<E_Blog_ProfileBonusLevel>() on t.UserId equals pu.UserId
                        join u in DB.CreateQuery<C_User_Register>() on t.UserId equals u.UserId
                        join a in DB.CreateQuery<C_User_Attention_Summary>() on t.UserId equals a.UserId
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

        public List<TogetherHotUserOrderInfo> QueryTogetherHotUserOrderInfo(string[] userIdArray)
        {
          
            var query = from r in DB.CreateQuery<C_Sports_Together>()
                        where userIdArray.Contains(r.CreateUserId)
                        && (r.ProgressStatus == (int)TogetherSchemeProgress.Finish || r.ProgressStatus == (int)TogetherSchemeProgress.SalesIn || r.ProgressStatus == (int)TogetherSchemeProgress.Standard)
                        && r.StopTime >= DateTime.Now
                        select new TogetherHotUserOrderInfo
                        {
                            CreateUserId = r.CreateUserId,
                            CreateTime = r.CreateTime,
                            GameCode = r.GameCode,
                            GameType = r.GameType,
                            PlayType = r.PlayType,
                            Progress = r.Progress,
                            ProgressStatus = (TogetherSchemeProgress)r.ProgressStatus,
                            SchemeId = r.SchemeId,
                            StopTime = r.StopTime,
                            TotalMoney = r.TotalMoney,
                        };
            return query.ToList();
        }
        public void AddTogetherHotUser(C_TogetherHotUser entity)
        {
           DB.GetDal<C_TogetherHotUser>().Add(entity);
        }
        /// <summary>
        /// 查询是否有该红人
        /// </summary>
        public int QueryTogether(string userId)
        {
            return DB.CreateQuery<C_TogetherHotUser>().Where(p => p.UserId == userId).Count();
        }
        public C_TogetherHotUser TogetherHotUserById(string userId)
        {
            return DB.CreateQuery<C_TogetherHotUser>().Where(p => p.UserId == userId).FirstOrDefault();
        }
        public void DeleteTogetherHotUser(C_TogetherHotUser entity)
        {
            DB.GetDal<C_TogetherHotUser>().Delete(entity);
        }
    }
}
