using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using NHibernate.Criterion;
using GameBiz.Auth.Domain.Entities;
using System.Collections;
using NHibernate.Linq;
using GameBiz.Core;

namespace GameBiz.Business.Domain.Managers
{
    public class InnerMailManager : GameBiz.Business.GameBizEntityManagement
    {
        #region 20151226
        //public void AddInnerMail(InnerMail mail)
        //{
        //    mail.SendTime = DateTime.Now;
        //    mail.UpdateTime = DateTime.Now;
        //    Add<InnerMail>(mail);
        //}
        //public void UpdateInnerMail(InnerMail mail)
        //{
        //    mail.UpdateTime = DateTime.Now;
        //    Update<InnerMail>(mail);
        //}
        //public void AddSendAddress(InnerMailSendAddress address)
        //{
        //    Add<InnerMailSendAddress>(address);
        //}
        //public InnerMail GetInnerMailById(string mailId)
        //{
        //    return SingleByKey<InnerMail>(mailId);
        //}
        //public void AddReadRecord(InnerMailReadRecord readRecord)
        //{
        //    readRecord.UpdateTime = DateTime.Now;
        //    Add<InnerMailReadRecord>(readRecord);
        //} 
        #endregion
        public void AddMoibleSMSSendRecord(MoibleSMSSendRecord entity)
        {
            this.Add(entity);
        }

        #region 20151226
        //public void UpdateReadRecord(InnerMailReadRecord readRecord)
        //{
        //    readRecord.UpdateTime = DateTime.Now;
        //    Update<InnerMailReadRecord>(readRecord);
        //} 
        #endregion

        public void UpdateSiteMessageScene(SiteMessageScene entity)
        {
            this.Update(entity);
        }

        #region 20151226
        //public InnerMailReadRecord GetReadRecord(InnerMail mail, SystemUser user)
        //{
        //    Session.Clear();
        //    return Session.CreateCriteria<InnerMailReadRecord>()
        //        .Add(Restrictions.Eq("Mail", mail))
        //        .Add(Restrictions.Eq("Receiver", user))
        //        .UniqueResult<InnerMailReadRecord>();
        //} 

        //public InnerMailReadRecord GetReadRecord(InnerMail mail, SystemUser user)
        //{
        //    Session.Clear();
        //    return Session.CreateCriteria<InnerMailReadRecord>()
        //        .Add(Restrictions.Eq("Mail", mail))
        //        .Add(Restrictions.Eq("Receiver", user))
        //        .UniqueResult<InnerMailReadRecord>();
        //} 
        #endregion

        /// <summary>
        /// 获取指定站内信包含接收者的数量
        /// </summary>
        /// <param name="innerMailId">站内信编号</param>
        /// <param name="userId">用户编号</param>
        /// <returns>包含数量</returns>
        public int GetMailContainsReceiverCount(string innerMailId, string userId)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            //return Session.GetNamedQuery("P_GetMailContainsReceiverCount")
            //    .SetString("InnerMailId", innerMailId)
            //    .SetString("ReceiverId", userId)
            //    .UniqueResult<int>();

            //var query = Session.Query<SiteMessageInnerMailListNew>().Where(s => s.MailId == innerMailId && s.ReceiverId == "U:"+userId);
            var query = Session.Query<SiteMessageInnerMailListNew>().Where(s => s.MailId == innerMailId && (s.ReceiverId == userId || s.ReceiverId == "U:" + userId));
            if (query != null) return query.Count();
            return 0;
        }
        public int GetUnreadMailCount(string userId)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            //return Session.GetNamedQuery("P_QueryUnreadInnerMailCount_ByReceiverId")
            //    .SetString("ReceiverId", userId)
            //    .UniqueResult<int>();

            var query = Session.Query<SiteMessageInnerMailListNew>().Where(s => s.HandleType == 0 && (s.ReceiverId == userId||s.ReceiverId == "U:"+userId));
            if (query != null) return query.Count();
            return 0;
        }

        #region 20151226
        ///// <summary>
        ///// 根据接收人查询邮件列表
        ///// </summary>
        ///// <param name="userId">用户编号</param>
        ///// <returns>包含数量</returns>
        //public IList QueryInnerMailList_ByReceiverId(string userId, int pageIndex, int pageSize, out int totalCount)
        //{
        //    Session.Clear();
        //    pageIndex = pageIndex < 0 ? 0 : pageIndex;
        //    pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
        //    // 通过数据库存储过程进行查询
        //    Dictionary<string, object> outputs;
        //    var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryInnerMailList_ByReceiverId"));
        //    query = query.AddInParameter("ReceiverId", userId);
        //    query = query.AddInParameter("PageIndex", pageIndex);
        //    query = query.AddInParameter("PageSize", pageSize);
        //    query = query.AddOutParameter("TotalCount", "Int32");
        //    var list = query.List(out outputs);
        //    totalCount = (int)outputs["TotalCount"];
        //    return list;
        //} 
        #endregion


        #region 20151226
        ///// <summary>
        ///// 根据接收人查询邮件列表
        ///// </summary>
        ///// <param name="userId">用户编号</param>
        ///// <returns>包含数量</returns>
        //public IList QueryUnReadInnerMailList_ByReceiverId(string userId, int pageIndex, int pageSize, int handleType, out int totalCount)
        //{
        //    Session.Clear();
        //    pageIndex = pageIndex < 0 ? 0 : pageIndex;
        //    //pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
        //    // 通过数据库存储过程进行查询
        //    Dictionary<string, object> outputs;
        //    var query = CreateOutputQuery(Session.GetNamedQuery("P_SiteMessage_QueryUnReadInnerMailList_ByHandleType"));
        //    query = query.AddInParameter("ReceiverId", userId);
        //    query = query.AddInParameter("PageIndex", pageIndex);
        //    query = query.AddInParameter("PageSize", pageSize);
        //    query = query.AddInParameter("HandleType", handleType);
        //    query = query.AddOutParameter("TotalCount", "Int32");
        //    var list = query.List(out outputs);
        //    totalCount = (int)outputs["TotalCount"];
        //    return list;
        //} 
        #endregion

        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }

        public SiteMessageScene QuerySiteMessageScene(string key)
        {
            this.Session.Clear();

            var result = Session.Query<SiteMessageScene>().Where(s => s.SceneKey == key).ToList();
//            var sql = string.Format(@"select [Id]
//                                          ,[SceneKey]
//                                          ,[SceneName]
//                                          ,[MsgCategory]
//                                          ,[MsgTemplateTitle]
//                                          ,[MsgTemplateContent]
//                                          ,[MsgTemplateParams]
//                                          ,[CreateTime]  from [E_SiteMessage_SiteMessageScene] where [SceneKey]='{0}'", key);
//            var result = Session.CreateSQLQuery(sql).List();
            //RoleInfo_QueryCollection collection = new RoleInfo_QueryCollection();
            if (result != null && result.Count > 0)
            {
                return result[0];
                //foreach (var item in result)
                //{
                //    //var array = item as object[];
                //    SiteMessageScene info = new SiteMessageScene
                //    {
                //        Id = int.Parse(array[0].ToString()),
                //        SceneKey = array[1].ToString(),
                //        SceneName = array[2].ToString(),
                //        MsgCategory = (GameBiz.Core.SiteMessageCategory)int.Parse(array[3].ToString()),
                //        MsgTemplateTitle = array[4].ToString(),
                //        MsgTemplateContent = array[5].ToString(),
                //        MsgTemplateParams = array[6].ToString(),
                //        CreateTime = DateTime.Parse(array[7].ToString()),
                //    };
                //    return info;
                //}
            }
            return null;

            //return this.Session.Query<SiteMessageScene>().FirstOrDefault(p => p.SceneKey == key);
        }

        public string QueryMobileByUserId(string userId)
        {
            this.Session.Clear();
            //var sql = string.Format("select [Mobile] from [E_Authentication_Mobile] where UserId='{0}'", userId);
            return this.Session.CreateSQLQuery("select [Mobile] from [E_Authentication_Mobile] where UserId=:userId").SetString("userId", userId).UniqueResult<string>();
        }

        //public SiteMessageTemplate QuerySiteMessageTemplate(int tempId)
        //{
        //    this.Session.Clear();
        //    return this.Session.Query<SiteMessageTemplate>().FirstOrDefault(p => p.Id == tempId);
        //}

        public List<GameBiz.Core.SiteMessageSceneInfo> QuerySiteNoticeConfig()
        {
            this.Session.Clear();
            var query = from s in this.Session.Query<SiteMessageScene>()
                        orderby s.Id ascending
                        select new GameBiz.Core.SiteMessageSceneInfo
                        {
                            CreateTime = s.CreateTime,
                            Id = s.Id,
                            MsgCategory = s.MsgCategory,
                            MsgTemplateContent = s.MsgTemplateContent,
                            MsgTemplateParams = s.MsgTemplateParams,
                            MsgTemplateTitle = s.MsgTemplateTitle,
                            SceneKey = s.SceneKey,
                            SceneName = s.SceneName,
                        };
            return query.ToList();
        }

        public string QuerySiteMessageTags()
        {
            this.Session.Clear();
            var query = from t in this.Session.Query<SiteMessageTags>()
                        orderby t.CreateTime ascending
                        select string.Format("{0}={1}", t.TagKey, t.TagName);
            return string.Join("^", query.ToArray());
        }

        public List<GameBiz.Core.MoibleSMSSendRecordInfo> QuerySMSSendRecordList(string userId, string mobileNumber, DateTime startTime, DateTime endTime, string status, int pageIndex, int pageSize, out int totalCount)
        {
            this.Session.Clear();
            var query = from l in this.Session.Query<MoibleSMSSendRecord>()
                        where (string.IsNullOrEmpty(userId) || l.UserId == userId)
                        && (string.IsNullOrEmpty(mobileNumber) || l.Mobile == mobileNumber)
                        && (string.IsNullOrEmpty(status) || l.SendStatus == status)
                        && (l.CreateTime >= startTime && l.CreateTime <= endTime)
                        orderby l.CreateTime descending
                        select new GameBiz.Core.MoibleSMSSendRecordInfo
                        {
                            CreateTime = l.CreateTime,
                            Id = l.Id,
                            Mobile = l.Mobile,
                            SendStatus = l.SendStatus,
                            SMSContent = l.SMSContent,
                            UserId = l.UserId,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        #region 新站内信


        public void AddSiteMessageInnerMailListNew(SiteMessageInnerMailListNew entity)
        {
            this.Add<SiteMessageInnerMailListNew>(entity);
        }
        public void UpdateSiteMessageInnerMailListNew(SiteMessageInnerMailListNew entity)
        {
            this.Add<SiteMessageInnerMailListNew>(entity);
        }
        public SiteMessageInnerMailListNew QuerySiteMessageInnerMailListNewByMailId(string mailId)
        {
            Session.Clear();
            return Session.Query<SiteMessageInnerMailListNew>().FirstOrDefault(s => s.MailId == mailId);
        }
        /// <summary>
        /// 根据接收人查询邮件列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns>包含数量</returns>
        public SiteMessageInnerMailListNew_Collection QueryInnerMailList_ByReceiverId(string userId, int pageIndex, int pageSize)
        {
            Session.Clear();
            SiteMessageInnerMailListNew_Collection collection = new SiteMessageInnerMailListNew_Collection();
            collection.TotalCount = 0;
            //var query = from m in Session.Query<SiteMessageInnerMailListNew>()
            //            where m.ReceiverId == "U:"+userId
            //            select new SiteMessageInnerMailListNewInfo
            //                {
            //                    HandleType = m.HandleType,
            //                    MailId = m.MailId,
            //                    MsgContent = m.MsgContent,
            //                    ReadTime = m.ReadTime,
            //                    ReceiverId = m.ReceiverId,
            //                    SenderId = m.SenderId,
            //                    SendTime = m.SendTime,
            //                    Title = m.Title,
            //                };
            var query = from m in Session.Query<SiteMessageInnerMailListNew>()
                        where (m.ReceiverId == userId || m.ReceiverId == "U:" + userId)
                        && m.HandleType!=InnerMailHandleType.Deleted
                        select new SiteMessageInnerMailListNewInfo
                        {
                            HandleType = m.HandleType,
                            MailId = m.MailId,
                            MsgContent = m.MsgContent,
                            ReadTime = m.ReadTime,
                            ReceiverId = m.ReceiverId,
                            SenderId = m.SenderId,
                            SendTime = m.SendTime,
                            Title = m.Title,
                        };
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.MailList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }
        /// <summary>
        /// 根据接收人查询邮件列表
        /// </summary>
        public SiteMessageInnerMailListNew_Collection QueryUnReadInnerMailList_ByReceiverId(string userId, int pageIndex, int pageSize, int handleType)
        {
            Session.Clear();
            SiteMessageInnerMailListNew_Collection collection = new SiteMessageInnerMailListNew_Collection();
            collection.TotalCount = 0;
            var query = from m in Session.Query<SiteMessageInnerMailListNew>()
                        where (m.ReceiverId == userId ||
                        m.ReceiverId == "U:" + userId)
                        && m.HandleType == (InnerMailHandleType)handleType
                        select new SiteMessageInnerMailListNewInfo
                        {
                            HandleType = m.HandleType,
                            MailId = m.MailId,
                            MsgContent = m.MsgContent,
                            ReadTime = m.ReadTime,
                            ReceiverId = m.ReceiverId,
                            SenderId = m.SenderId,
                            SendTime = m.SendTime,
                            Title = m.Title,
                        };
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.MailList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }
        public int GetUserInnerMailCount(string userId)
        {
            Session.Clear();
            var query = Session.Query<SiteMessageInnerMailListNew>().Where(s => (s.ReceiverId == userId||s.ReceiverId == "U:"+ userId)&&s.HandleType!=InnerMailHandleType.Deleted);
            if (query != null) return query.Count();
            return 0;
        }
        public string QueryUserIdByRoleId(string roleId)
        {
            Session.Clear();
            string strUserIds = string.Empty;
            var sql = "select userId from C_Auth_UserRole where RoleId=:roleId";
            var result = Session.CreateSQLQuery(sql)
                              .SetString("roleId", roleId).List();
            if (result != null)
            {
                foreach (var item in result)
                {
                    if (item == null || string.IsNullOrEmpty(item.ToString()))
                        continue;
                    strUserIds += item.ToString() + "|";
                }
            }
            if (!string.IsNullOrEmpty(strUserIds))
                strUserIds.TrimEnd('|');
            return strUserIds;
        }


        #endregion
    }
}
