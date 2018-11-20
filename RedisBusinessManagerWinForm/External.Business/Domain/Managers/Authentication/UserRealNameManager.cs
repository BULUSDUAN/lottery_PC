using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Auth.Domain.Entities;
using NHibernate.Criterion;
using External.Domain.Entities.Authentication;
using NHibernate.Criterion;
using GameBiz.Business;
using NHibernate.Linq;
using External.Core.Authentication;
using NHibernate.Linq;

namespace External.Domain.Managers.Authentication
{
    public class UserRealNameManager : GameBiz.Business.GameBizEntityManagement
    {
        public UserRealName GetUserRealName(string userId)
        {
            return GetByKey<UserRealName>(userId);
        }
        public UserRealName GetOtherUserCard(string cardType, string cardNumber, string userId)
        {
            Session.Clear();
            var list = Session.CreateCriteria<UserRealName>()
                .Add(Restrictions.Eq("CardType", cardType))
                .Add(Restrictions.Eq("IdCardNumber", cardNumber))
                .Add(Restrictions.Not(Restrictions.Eq("UserId", userId)))
                .List<UserRealName>();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        public void AddUserRealName(UserRealName realName)
        {
            realName.CreateTime = DateTime.Now;
            realName.UpdateTime = DateTime.Now;
            Add<UserRealName>(realName);
        }
        public void UpdateUserRealName(UserRealName realName)
        {
            realName.UpdateTime = DateTime.Now;
            Update<UserRealName>(realName);
        }

        public void DeleteUserRealName(UserRealName entity)
        {
            this.Delete<UserRealName>(entity);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }

        public int QueryIdCardNumber(string idCardNumber)
        {
            Session.Clear();
            var count = this.Session.Query<UserRealName>().Count(p => p.IdCardNumber == idCardNumber);
            return count;
        }
        public bool IsRealName(string name, string cardNumber)
        {
            return Session.Query<UserRealName>().Where(s => s.RealName == name && s.IdCardNumber == cardNumber).ToList().Count > 0 ? true : false;
        }
        public UserRealName_Collection QueryUserRealNameCollection()
        {
            UserRealName_Collection collection = new UserRealName_Collection();
            collection.TotalCount = 0;
            var query = from i in Session.Query<UserRealName>()
                        select new UserRealNameInfo
                        {
                            AuthFrom = i.AuthFrom,
                            CardType = i.CardType,
                            IdCardNumber = i.IdCardNumber,
                            RealName = i.RealName,
                            UserId = i.UserId,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                collection.RealNameList = query.ToList();
            }
            return collection;

        }
        public UserRealName GetRealNameInfoByName(string realName, string cardNumber)
        {
            var query = Session.Query<UserRealName>().Where(s => s.RealName == realName && s.IdCardNumber == cardNumber);
            if (query != null && query.Count() > 0)
            {
                var resutl = query.FirstOrDefault(s => s.IsSettedRealName == true);
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.FirstOrDefault(s => s.IsSettedRealName == false);
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }

        public UserRealName QueryUserRealName(string idCard)
        {
            this.Session.Clear();
            return this.Session.Query<UserRealName>().FirstOrDefault(p => p.IdCardNumber == idCard && p.IsSettedRealName == true);
        }

        public UserRealName QueryUserRealNameByName(string name)
        {
            this.Session.Clear();
            return this.Session.Query<UserRealName>().FirstOrDefault(p => p.RealName == name && p.IsSettedRealName == true);
        }
    }
}
