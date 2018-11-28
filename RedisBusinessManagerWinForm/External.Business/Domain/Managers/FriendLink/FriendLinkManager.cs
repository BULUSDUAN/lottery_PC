using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using External.Core;
using External.Domain.Entities.FriendLink;
using NHibernate.Linq;
using GameBiz.Business;
using External.Core.FriendLinks;

namespace External.Domain.Managers.Authentication
{
    public class FriendLinkManager : GameBizEntityManagement
    {
        public void AddFriendLinks(FriendLinks entity)
        {
            this.Add<FriendLinks>(entity);
        }

        public void DeleteFriendLink(FriendLinks entity)
        {
            this.Delete<FriendLinks>(entity);
        }

        public FriendLinks QueryFriendLink(int id)
        {
            Session.Clear();
            return Session.Query<FriendLinks>().FirstOrDefault(p => p.Id == id);
        }

        public void UpdateFriendLink(FriendLinks entity)
        {
            this.Update<FriendLinks>(entity);
        }
        /// <summary>
        /// 友情链接全部查询
        /// </summary>
        public List<FriendLinksInfo> QueryYQLinks(bool Isfriendship)
        {
            Session.Clear();
            var query = from s in Session.Query<FriendLinks>()
                        where s.Isfriendship == Isfriendship
                        orderby s.IndexLink ascending
                        select new FriendLinksInfo
                        {
                            Id = s.Id,
                            IndexLink = s.IndexLink,
                            InnerText = s.InnerText,
                            LinkUrl = s.LinkUrl,
                            IsFriendShip = s.Isfriendship,
                        };
            return query.ToList();
        }
    }
}
