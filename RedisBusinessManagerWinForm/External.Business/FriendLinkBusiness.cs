using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Domain.Managers.Authentication;
using External.Domain.Entities.FriendLink;
using External.Core.FriendLinks;

namespace External.Business
{
    public class FriendLinkBusiness
    {
        /// <summary>
        ///  添加友情链接
        /// </summary>
        public void AddFriendLink(int indexLink, string linkUrl, string innerText, bool isfriendship)
        {
            var manager = new FriendLinkManager();
            manager.AddFriendLinks(new FriendLinks()
            {
                IndexLink = indexLink,
                LinkUrl = linkUrl,
                InnerText = innerText,
                Isfriendship = isfriendship,
                CreateTime = DateTime.Now,
            });
            _cacheLinkList.Clear();
        }

        public void UpdateFriendLink(int id, int indexLink, string linkUrl, string innerText)
        {
            var manager = new FriendLinkManager();
            var entity = manager.QueryFriendLink(id);
            entity.IndexLink = indexLink;
            entity.LinkUrl = linkUrl;
            entity.InnerText = innerText;

            manager.UpdateFriendLink(entity);
            _cacheLinkList.Clear();
        }

        /// <summary>
        /// 删除友情链接
        /// </summary>
        public void DeleteFriendLink(int id)
        {
            var manager = new FriendLinkManager();
            var link = manager.QueryFriendLink(id);
            manager.DeleteFriendLink(link);

            _cacheLinkList.Clear();
        }

        private static List<FriendLinksInfo> _cacheLinkList = new List<FriendLinksInfo>();
        public FriendLinksInfooCollection QueryLinks(bool Isfriendship)
        {
            var result = new FriendLinksInfooCollection();

            var cache = _cacheLinkList.Where(p => p.IsFriendShip == Isfriendship).ToList();
            if (cache.Count != 0)
            {
                result.AddRange(cache);
            }
            else
            {
                var news = new FriendLinkManager().QueryYQLinks(Isfriendship);
                result.AddRange(news);
                _cacheLinkList.AddRange(news);
            }
            return result;
        }
    }
}
