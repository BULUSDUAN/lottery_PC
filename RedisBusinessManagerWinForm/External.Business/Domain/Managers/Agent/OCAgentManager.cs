using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using External.Domain.Entities.Agent;
using Common.Utilities;
using System.Collections;
using NHibernate.Linq;
using External.Core.Agnet;
using Common.Database.ORM;
using GameBiz.Domain.Entities;
using GameBiz.Core;
using System.Data;


namespace External.Domain.Managers.Agent
{
    public class OCAgentManager : GameBizEntityManagement
    {
        public void AddOCAgent(OCAgent entity)
        {
            this.Add(entity);
        }

        public void AddOCAgentRebate(OCAgentRebate entity)
        {
            this.Add(entity);
        }

        public void AddOCAgentPayDetail(OCAgentPayDetail entity)
        {
            this.Add(entity);
        }

        public void UpdateOCAgentRebate(OCAgentRebate entity)
        {
            this.Update(entity);
        }

        public void UpdateOCAgent(OCAgent entity)
        {
            this.Update(entity);
        }

        public OCAgent QueryOCAgent(string userId)
        {
            Session.Clear();
            return this.Session.Query<OCAgent>().FirstOrDefault(p => p.UserId == userId);
        }
        /// <summary>
        /// 根据店面编号查询店主id
        /// </summary>
        public OCAgent QueryUserIdBystoreId(string storeid)
        {
            Session.Clear();
            return this.Session.Query<OCAgent>().FirstOrDefault(p => p.StoreId == storeid);
        }

        public OCAgent QueryOCAgentByDomain(string domain)
        {
            Session.Clear();
            return this.Session.Query<OCAgent>().FirstOrDefault(p => p.CustomerDomain == domain);
        }
        public List<OCAgent> QueryOCAgentListByUserId(string userId)
        {
            Session.Clear();
            return this.Session.Query<OCAgent>().Where(p => p.ParentUserId == userId).ToList();
        }

        public List<UserRegister> QueryAgentSubUser(string agentId)
        {
            Session.Clear();
            return this.Session.Query<UserRegister>().Where(p => p.AgentId == agentId).ToList();
        }

        public List<OCAgent> QueryAllAgent()
        {
            Session.Clear();
            var query = from a in this.Session.Query<OCAgent>()
                        orderby a.StoreId descending
                        select new OCAgent
                        {
                            UserId = a.UserId,
                            StoreId = a.StoreId,
                            OCAgentCategory = a.OCAgentCategory,
                            ParentUserId = a.ParentUserId,
                            CustomerDomain = a.CustomerDomain,
                            CreateTime = a.CreateTime,
                        };
            return query.ToList();
        }

        /// <summary>
        /// 店面代理信息
        /// </summary>
        public List<StoreMessageInfo> QueryStoreMessageByuserId(string userId)
        {
            Session.Clear();
            var query = from a in this.Session.Query<OCAgent>()
                        join u in this.Session.Query<UserRegister>() on a.UserId equals u.UserId
                        where a.ParentUserId == userId && a.OCAgentCategory == OCAgentCategory.SportLotterySubAgent
                        select new StoreMessageInfo
                        {
                            UserId = a.UserId,
                            StoreId = a.StoreId,
                            UserName = u.DisplayName,
                            ParentUserId = a.ParentUserId,
                            OCAgentCategory = a.OCAgentCategory,
                            CustomerDomain = a.CustomerDomain,
                            CreateTime = a.CreateTime
                        };
            return query.ToList();
        }

        /// <summary>
        /// 查询代理对应彩种的返点
        /// </summary>
        public OCAgentRebate QueryOCAgentDefaultRebate(string userId, string gameCode, string gameType, CPSMode mode)
        {
            Session.Clear();
            return this.Session.Query<OCAgentRebate>().FirstOrDefault(p => p.CPSMode == mode && p.UserId == userId && p.GameCode == gameCode && (gameType == "" || p.GameType == gameType));
        }
        /// <summary>
        /// 查询代理对应彩种的返点
        /// </summary>
        public List<OCAgentRebate> QueryOCAgentDefaultRebateList(string userId, string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<OCAgentRebate>().Where(p => p.UserId == userId && p.GameCode == gameCode && p.GameType == gameType).ToList();
        }
        /// <summary>
        /// 查询代理对应彩种的返点
        /// </summary>
        public OCAgentRebate QueryOCAgentDefaultRebateByRebateType(string userId, string gameCode, string gameType, int rebateType)
        {
            Session.Clear();
            return this.Session.Query<OCAgentRebate>().OrderByDescending(p => p.CreateTime).FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType);
        }

        /// <summary>
        /// 查询用户所有的返点配置
        /// </summary>
        public List<OCAgentRebate> QueryOCAgentRebateList(string userId)
        {
            Session.Clear();
            return this.Session.Query<OCAgentRebate>().Where(p => p.UserId == userId).OrderByDescending(p => p.CreateTime).ToList();
        }

        /// <summary>
        /// 查询用户所有的返点配置
        /// </summary>
        public List<OCAgentRebateInfo> QueryOCAgentRebateInfoList(string userId)
        {
            Session.Clear();
            var query = from a in this.Session.Query<OCAgentRebate>()
                        where a.UserId == userId
                        select new OCAgentRebateInfo
                        {
                            GameCode = a.GameCode,
                            GameType = a.GameType,
                            Rebate = a.Rebate,
                            SubUserRebate = a.SubUserRebate,
                            UserId = a.UserId,
                            RebateType = a.RebateType == null ? 0 : a.RebateType,
                            CreateTime = a.CreateTime,
                        };
            return query.ToList();
        }

        /// <summary>
        /// 查询用户是否有返点
        /// </summary>
        public List<OCAgentRebateInfo> QueryUserRebateList(string userId)
        {
            Session.Clear();
            var query = from a in this.Session.Query<OCAgentRebate>()
                        where a.UserId == userId
                        && a.Rebate > 0
                        select new OCAgentRebateInfo
                        {
                            GameCode = a.GameCode,
                            GameType = a.GameType,
                            Rebate = a.Rebate,
                            SubUserRebate = a.SubUserRebate,
                            UserId = a.UserId
                        };
            return query.ToList();
        }

        public int QueryOCAgentPayDetailCount(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<OCAgentPayDetail>().Count(p => p.SchemeId == schemeId);
        }

        public List<OCAgentPayDetailInfo> QueryOCAgentPayDetailList(DateTime fromDate, DateTime toDate, string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from d in this.Session.Query<OCAgentPayDetail>()
                        join u in this.Session.Query<UserRegister>() on d.OrderUser equals u.UserId
                        where (d.CreateTime >= fromDate && d.CreateTime < toDate)
                        && (string.Empty == userId || d.PayInUserId == userId)
                        select new OCAgentPayDetailInfo
                        {
                            CreateTime = d.CreateTime,
                            GameCode = d.GameCode,
                            GameType = d.GameType,
                            TotalMoney = d.OrderTotalMoney,
                            OrderUser = d.OrderUser,
                            OrderUserDisplayName = u.DisplayName,
                            PayMoney = d.PayMoney,
                            Rebate = d.Rebate,
                            Remark = d.Remark,
                            SchemeId = d.SchemeId,
                            SchemeType = d.SchemeType,
                        };
            totalCount = query.Count();
            return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 佣金管理，查询结算报表
        /// </summary>
        public List<AgentPayDetailReportInfo> QueryAgentPayDetailReportInfo(string userId, DateTime fromDate, DateTime toDate)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetPayDetailReport"))
                .AddInParameter("pAgentId", userId)
                .AddInParameter("fromDate", fromDate)
                .AddInParameter("toDate", toDate);
            var dt = query.GetDataTable();
            var list = new List<AgentPayDetailReportInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AgentPayDetailReportInfo
                {
                    CommissionMoney = row["CommissionMoney"] == DBNull.Value ? 0M : decimal.Parse(row["CommissionMoney"].ToString()),
                    GameCode = row["GameCode"] == DBNull.Value ? string.Empty : row["GameCode"].ToString(),
                    GameType = row["GameType"] == DBNull.Value ? string.Empty : row["GameType"].ToString(),
                    TotalMoney = row["TotalMoney"] == DBNull.Value ? 0M : decimal.Parse(row["TotalMoney"].ToString()),
                    UserId = row["UserId"] == DBNull.Value ? string.Empty : row["UserId"].ToString()
                });
            }
            return list;
            //return Common.Database.ORM.ORMHelper.DataTableToList<AgentPayDetailReportInfo>(dt);
        }

        /// <summary>
        /// 发起中，未返点的订单
        /// </summary>
        public List<SubUserNoPayRebateOrderInfo> QuerySubUserCreateingOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out int userCount, out  decimal totalMoney)
        {
            starTime = starTime.Date;
            endTime = endTime.AddDays(1).Date;
            Session.Clear();
            //var query = from o in this.Session.Query<Sports_Order_Running>()
            //            join t in this.Session.Query<Sports_Together>() on o.SchemeId equals t.SchemeId
            //            join r in this.Session.Query<UserRegister>() on o.UserId equals r.UserId
            //            where o.CreateTime >= starTime && o.CreateTime < endTime
            //            && (string.Empty == key || (r.UserId == key || r.DisplayName == key))
            //            && o.AgentId == userId
            //            && o.IsPayRebate == false
            //            && o.IsVirtualOrder == false

            //            select new SubUserNoPayRebateOrderInfo
            //            {
            //                UserId = o.UserId,
            //                SchemeId = o.SchemeId,
            //                GameCode = o.GameCode,
            //                Progress = t.Progress,
            //                DisplayName = r.DisplayName,
            //                TotalMonery = o.TotalMoney,
            //                CreateTime = o.CreateTime,
            //            };

            var query = from o in this.Session.Query<Sports_Order_Running>()
                        join r in this.Session.Query<UserRegister>() on o.UserId equals r.UserId
                        where o.CreateTime >= starTime && o.CreateTime < endTime
                        && (string.Empty == key || (r.UserId == key || r.DisplayName == key))
                        && o.AgentId == userId && (o.TicketStatus == TicketStatus.Ticketing || o.TicketStatus == TicketStatus.PrintTicket) && o.ProgressStatus == ProgressStatus.Running
                        && o.IsPayRebate == false
                        && o.IsVirtualOrder == false

                        select new SubUserNoPayRebateOrderInfo
                        {
                            UserId = o.UserId,
                            SchemeId = o.SchemeId,
                            GameCode = o.GameCode,
                            Progress = o.TicketProgress,
                            DisplayName = r.DisplayName,
                            TotalMonery = o.TotalMoney,
                            CreateTime = o.CreateTime,
                            HideDisplayNameCount = r.HideDisplayNameCount,
                            TicketStatus = o.TicketStatus,
                        };

            totalCount = query.Count();
            userCount = totalCount == 0 ? 0 : query.GroupBy(p => p.UserId).Select(p => new { Key = p.Key, Count = p.Count() }).ToList().Count;
            totalMoney = totalCount == 0 ? 0 : query.Sum(p => p.TotalMonery);
            return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 未返点方案
        /// </summary>
        public List<SubUserNoPayRebateOrderInfo> QuerySubUserNoPayRebateOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize,
            out int totalCount, out int totalUserCount, out  decimal totalMoney)
        {
            starTime = starTime.Date;
            endTime = endTime.AddDays(1).Date;
            Session.Clear();
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QuerySubUserNoPayRebateOrderList"))
                .AddInParameter("key", key)
                .AddInParameter("agentId", userId)
                .AddInParameter("starTime", starTime)
                .AddInParameter("endTime", endTime)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32")
                .AddOutParameter("totalUserCount", "Int32")
                .AddOutParameter("totalMoney", "Decimal");
            var list = query.List(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalUserCount = UsefullHelper.GetDbValue<int>(outputs["totalUserCount"]);
            totalMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalMoney"]);

            var result = new List<SubUserNoPayRebateOrderInfo>();
            foreach (var item in list)
            {
                var array = item as object[];
                SubUserNoPayRebateOrderInfo info = new SubUserNoPayRebateOrderInfo();
                info.UserId = Convert.ToString(array[1]);
                info.DisplayName = Convert.ToString(array[2]);
                info.SchemeId = Convert.ToString(array[3]);
                info.Progress = Convert.ToDecimal(array[4]);
                info.GameCode = Convert.ToString(array[5]);
                info.TotalMonery = Convert.ToDecimal(array[6]);
                info.CreateTime = Convert.ToDateTime(array[7]);
                info.HideDisplayNameCount = Convert.ToInt32(array[8]);
                result.Add(info);


                //result.Add(new SubUserNoPayRebateOrderInfo
                //{
                //    UserId = UsefullHelper.GetDbValue<string>(array[1]),
                //    DisplayName = UsefullHelper.GetDbValue<string>(array[2]),
                //    SchemeId = UsefullHelper.GetDbValue<string>(array[3]),
                //    Progress = UsefullHelper.GetDbValue<decimal>(array[4]),
                //    GameCode = UsefullHelper.GetDbValue<string>(array[5]),
                //    TotalMonery = UsefullHelper.GetDbValue<decimal>(array[6]),
                //    CreateTime = UsefullHelper.GetDbValue<DateTime>(array[7]),
                //    HideDisplayNameCount=Convert.ToInt32(array[8]),
                //});
            }
            return result;
        }

        /// <summary>
        /// 已返点方案
        /// </summary>
        public List<SubUserPayRebateOrderInfo> QuerySubUserPayRebateOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize,
            out int totalCount, out int totalUserCount, out  decimal totalMoney, out decimal totalRealPayRebateMoney)
        {
            Session.Clear();
            starTime = starTime.Date;
            endTime = endTime.AddDays(1).Date;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QuerySubUserPayRebateOrderList"))
                .AddInParameter("key", key)
                .AddInParameter("agentId", userId)
                .AddInParameter("starTime", starTime)
                .AddInParameter("endTime", endTime)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32")
                .AddOutParameter("totalUserCount", "Int32")
                .AddOutParameter("totalMoney", "Decimal")
                .AddOutParameter("totalRealPayRebateMoney", "Decimal");
            var list = query.List(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalUserCount = UsefullHelper.GetDbValue<int>(outputs["totalUserCount"]);
            totalMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalMoney"]);
            totalRealPayRebateMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRealPayRebateMoney"]);

            var result = new List<SubUserPayRebateOrderInfo>();
            foreach (var item in list)
            {
                var array = item as object[];
                result.Add(new SubUserPayRebateOrderInfo
                {
                    UserId = UsefullHelper.GetDbValue<string>(array[1]),
                    DisplayName = UsefullHelper.GetDbValue<string>(array[2]),
                    SchemeId = UsefullHelper.GetDbValue<string>(array[3]),
                    GameCode = UsefullHelper.GetDbValue<string>(array[4]),
                    CreateTime = UsefullHelper.GetDbValue<DateTime>(array[5]),
                    TotalMoney = UsefullHelper.GetDbValue<decimal>(array[6]),
                    RealPayRebateMoney = UsefullHelper.GetDbValue<decimal>(array[7]),
                    HideDisplayNameCount = Convert.ToInt32(array[8]),
                });
            }
            return result;
        }

        #region 查询代理销量

        public List<UserRegister> QueryLowerAgentByUserId(string userId)
        {
            return Session.Query<UserRegister>().Where(s => s.AgentId == userId && s.IsAgent == true).ToList();//20150819 一米要求过滤一级用户
            //return Session.Query<UserRegister>().Where(s => s.AgentId == userId).ToList();
        }
        public UserRegister QueryLowerAgentByUserId(string userId, string agentId)
        {
            return Session.Query<UserRegister>().FirstOrDefault(s => s.AgentId == agentId && s.UserId == userId);
        }
        public List<string> QueryLowerAgentList(string userId)
        {
            Session.Clear();
            List<string> strList = new List<string>();
            string strSql = "select UserId from C_User_Register where ParentPath like '%" + userId + "%'";
            var result = Session.CreateSQLQuery(strSql).List();
            if (result != null)
            {
                foreach (var item in result)
                {
                    strList.Add(item.ToString());
                }
            }
            return strList;
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<AgentLottoTopInfo> Test_QueryLowerAgentSaleListByUserId(string agentId, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            List<AgentLottoTopInfo> listInfo = new List<AgentLottoTopInfo>();
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryLowerAgentSaleList"))
                .AddInParameter("AgentId", agentId)
                .AddInParameter("StartTime", startTime)
                .AddInParameter("EndTime", endTime)
                .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    AgentLottoTopInfo info = new AgentLottoTopInfo();
                    info.CTZQ = Convert.ToDecimal(row["CTZQ"]);
                    info.JCZQ = Convert.ToDecimal(row["JCZQ"]);
                    info.JCLQ = Convert.ToDecimal(row["JCLQ"]);
                    info.SZC = Convert.ToDecimal(row["SZC"]);
                    info.BJDC = Convert.ToDecimal(row["BJDC"]);
                    info.GPC = Convert.ToDecimal(row["GPC"]);
                    info.TotalMoney = Convert.ToDecimal(row["TotalMoney"]);
                    info.UserId = Convert.ToString(row["UserId"]);
                    info.DisplayName = Convert.ToString(row["DisplayName"]);
                    listInfo.Add(info);
                }
            }
            return listInfo;
        }
        public List<AgentLottoTopInfo> QueryLowerAgentSaleListByUserId(string agentId, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            List<AgentLottoTopInfo> listInfo = new List<AgentLottoTopInfo>();
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            //原始
            string strSql = "select sum(tab.CTZQ) CTZQ,sum(tab.JCZQ)JCZQ,sum(tab.JCLQ)JCLQ,sum(tab.SZC)SZC,sum(tab.GPC)GPC,SUM(tab.BJDC) BJDC,sum(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney,UserId,DisplayName from (select r.UserId,r.DisplayName,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC from  C_OrderDetail o inner join C_User_Register r on o.UserId=r.UserId  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.BetTime>=:StartTime and o.BetTime<:EndTime  and r.UserId in (select UserId from C_User_Register where ParentPath  like '%" + agentId + "%' or UserId='" + agentId + "')  group by r.UserId,r.DisplayName) tab group by UserId,DisplayName";
            var result = Session.CreateSQLQuery(strSql)
                //.SetParameterList("UserId", agentId)
                              .SetDateTime("StartTime", startTime)
                              .SetDateTime("EndTime", endTime)
                              .List();



            //20150925优化后SQl
            //string strSql = "select CTZQ,JCZQ,JCLQ,SZC,GPC,BJDC,(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney,utab.UserId,utab.DisplayName from  ( select o.UserId,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC from  C_OrderDetail o  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.BetTime>=:StartTime and o.BetTime<:EndTime   group by o.UserId) tab inner join  (select UserId,DisplayName from C_User_Register where ParentPath  like '%" + agentId + "%' or UserId='" + agentId + "') utab on tab.UserId=utab.UserId";

            //string strSql = "select CTZQ,JCZQ,JCLQ,SZC,GPC,BJDC,(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney,tab.UserId,tab.DisplayName from  ( select o.UserId,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC,u.DisplayName from  C_OrderDetail o inner join C_User_Register u on o.UserId=u.UserId  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.BetTime>=:StartTime and o.BetTime<:EndTime and (u.ParentPath like '%" + agentId + "%' or u.UserId='" + agentId + "')   group by o.UserId,u.DisplayName ) tab";
            //var result = Session.CreateSQLQuery(strSql)
            //                  .SetDateTime("StartTime", startTime)
            //                  .SetDateTime("EndTime", endTime)
            //                  .List();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    AgentLottoTopInfo info = new AgentLottoTopInfo();
                    var array = item as object[];
                    info.CTZQ = Convert.ToDecimal(array[0]);
                    info.JCZQ = Convert.ToDecimal(array[1]);
                    info.JCLQ = Convert.ToDecimal(array[2]);
                    info.SZC = Convert.ToDecimal(array[3]);
                    info.BJDC = Convert.ToDecimal(array[4]);
                    info.GPC = Convert.ToDecimal(array[5]);
                    info.TotalMoney = Convert.ToDecimal(array[6]);
                    info.UserId = Convert.ToString(array[7]);
                    info.DisplayName = Convert.ToString(array[8]);
                    listInfo.Add(info);
                }

            }
            return listInfo;
        }
        /// <summary>
        /// 推广销量新版查询(试用)
        /// </summary>
        /// <returns></returns>
        public List<AgentLottoTopInfo> QueryNewLowerAgentSaleListByUserId(string agentId, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            List<AgentLottoTopInfo> collection = new List<AgentLottoTopInfo>();
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            string strSql = "select tabl.CTZQ,tabl.JCZQ,tabl.JCLQ,tabl.SZC,tabl.GPC,tabl.BJDC,(tabl.CTZQ+tabl.JCZQ+tabl.JCLQ+tabl.SZC+tabl.GPC+tabl.BJDC) totalMoney,tabl.UserId,tabl.DisplayName from  ( select SUM((case p.GameCode when'CTZQ' then p.TotalSales else 0 end)) CTZQ,SUM((case p.GameCode when'JCZQ' then p.TotalSales else 0 end)) JCZQ,SUM((case p.GameCode when'JCLQ' then p.TotalSales else 0 end)) JCLQ,SUM((case p.GameCode when 'DLT' then p.TotalSales when 'SSQ' then p.TotalSales when 'FC3D' then p.TotalSales when 'PL3' then p.TotalSales else 0 end)) SZC,SUM((case p.GameCode when 'CQSSC' then p.TotalSales when 'JX11X5' then p.TotalSales else 0 end)) GPC,SUM((case p.GameCode when'BJDC' then p.TotalSales else 0 end)) BJDC,u.UserId,u.DisplayName from P_OCAgent_ReportSales p inner join C_User_Register u on p.UserId=p.UserId where ParentUserId=:AgentId and p.CreateTime>=:StartTime and p.CreateTime<:EndTime group by u.UserId,u.DisplayName ) tabl";
            var result = Session.CreateSQLQuery(strSql)
                                .SetString("AgentId", agentId)
                                .SetDateTime("StartTime", startTime)
                                .SetDateTime("EndTime", endTime)
                                .List();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    AgentLottoTopInfo info = new AgentLottoTopInfo();
                    var array = item as object[];
                    info.CTZQ = Convert.ToDecimal(array[0]);
                    info.JCZQ = Convert.ToDecimal(array[1]);
                    info.JCLQ = Convert.ToDecimal(array[2]);
                    info.SZC = Convert.ToDecimal(array[3]);
                    info.BJDC = Convert.ToDecimal(array[4]);
                    info.GPC = Convert.ToDecimal(array[5]);
                    info.TotalMoney = Convert.ToDecimal(array[6]);
                    info.UserId = Convert.ToString(array[7]);
                    info.DisplayName = Convert.ToString(array[8]);
                    collection.Add(info);
                }

            }
            return collection;
        }
        public AgentLottoTopInfo QueryLowerAgentSaleByUserId(string[] arrayUserId, DateTime startTime, DateTime endTime)
        {
            Session.Clear();

            AgentLottoTopInfo info = new AgentLottoTopInfo();
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            string strSql = "select sum(tab.CTZQ) CTZQ,sum(tab.JCZQ)JCZQ,sum(tab.JCLQ)JCLQ,sum(tab.SZC)SZC,sum(tab.GPC)GPC,SUM(tab.BJDC) BJDC,sum(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney from (select r.UserId,r.DisplayName,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC from  C_OrderDetail o inner join C_User_Register r on o.UserId=r.UserId  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.UserId in (:UserId) and o.BetTime>=:StartTime and o.BetTime<:EndTime group by r.UserId,r.DisplayName) tab";

            var result = Session.CreateSQLQuery(strSql)
                                .SetParameterList("UserId", arrayUserId)
                                .SetDateTime("StartTime", startTime)
                                .SetDateTime("EndTime", endTime)
                                .List();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    info.CTZQ = Convert.ToDecimal(array[0]);
                    info.JCZQ = Convert.ToDecimal(array[1]);
                    info.JCLQ = Convert.ToDecimal(array[2]);
                    info.SZC = Convert.ToDecimal(array[3]);
                    info.BJDC = Convert.ToDecimal(array[4]);
                    info.GPC = Convert.ToDecimal(array[5]);
                    info.TotalMoney = Convert.ToDecimal(array[6]);
                }

            }
            return info;
        }

        public List<OCAgentRebate> QueryLowerAgentListByParentId(string parentId)
        {
            Session.Clear();

            string strSql = "select r.CreateTime,isnull(r.GameCode,'')GameCode,isnull(r.GameType,'')GameType,r.Id,isnull(r.Rebate,0)Rebate,isnull(r.SubUserRebate,0)SubUserRebate,u.UserId from C_User_Register u left join P_OCAgent_Rebate r on u.UserId=r.UserId where u.ParentPath like '%" + parentId + "%'";
            var query = Session.CreateSQLQuery(strSql).List();
            List<OCAgentRebate> ocList = new List<OCAgentRebate>();
            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    OCAgentRebate entity = new OCAgentRebate();
                    entity.CreateTime = Convert.ToDateTime(array[0]);
                    entity.GameCode = Convert.ToString(array[1]);
                    entity.GameType = Convert.ToString(array[2]);
                    entity.Id = Convert.ToInt64(array[3]);
                    entity.Rebate = Convert.ToDecimal(array[4]);
                    entity.SubUserRebate = Convert.ToDecimal(array[6]);
                    entity.UserId = Convert.ToString(array[6]);
                    ocList.Add(entity);
                }
            }

            return ocList;
        }

        public List<OCAgentRebate> QueryOcAgentRebateList(decimal rebate, string[] arrayUserId, string gameCode, string gameType)
        {
            Session.Clear();
            return Session.Query<OCAgentRebate>().Where(s => s.Rebate > rebate && arrayUserId.Contains(s.UserId) && s.GameCode == gameCode && (s.GameType == gameType || gameType == string.Empty)).ToList();
        }

        #endregion

        #region 测试


        public List<OCAgent> Test_QueryAllAgent()
        {
            Session.Clear();
            return Session.Query<OCAgent>().ToList();
        }
        public OCAgent Test_QueryAllAgent(string userId)
        {
            Session.Clear();
            return Session.Query<OCAgent>().FirstOrDefault(s => s.UserId == userId);
        }
        public List<UserRegister> Test_QueryUserRegisterList()
        {
            Session.Clear();
            return Session.Query<UserRegister>().Where(s => s.AgentId != string.Empty).ToList();
        }
        public UserRegister Test_QueryUserRegister(string userId)
        {
            Session.Clear();
            return Session.Query<UserRegister>().FirstOrDefault(s => s.UserId == userId);
        }
        public void UpdateUserRigister(UserRegister entity)
        {
            this.Update<UserRegister>(entity);
        }


        #endregion


        /// <summary>
        /// 查询代理资金明细
        /// </summary>
        public List<OCAgentPayDetailInfo> QueryAgentRebateAndBonusDetail(string agentId, string schemeid, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, int cpsMode, int schemeType, out int totalCount)
        {
            Session.Clear();
            var query = from d in Session.Query<OCAgentPayDetail>()
                        join u in this.Session.Query<UserRegister>() on d.OrderUser equals u.UserId
                        where (string.IsNullOrEmpty(agentId) || d.PayInUserId == agentId)
                        && (string.IsNullOrEmpty(schemeid) || d.SchemeId == schemeid)
                         && d.CreateTime >= starTime && d.CreateTime < endTime
                         && (cpsMode == -1 || d.CPSMode == (CPSMode)cpsMode)
                         && (schemeType == -1 || d.SchemeType == (SchemeType)schemeType)
                        select new OCAgentPayDetailInfo
                        {
                            CPSMode = d.CPSMode,
                            CreateTime = d.CreateTime,
                            GameCode = d.GameCode,
                            GameType = d.GameType,
                            HandlPeople = d.HandlPeople,
                            OrderUser = d.OrderUser,
                            PayInUserId = d.PayInUserId,
                            OrderUserDisplayName = u.DisplayName,
                            PayMoney = d.PayMoney,
                            Rebate = d.Rebate,
                            Remark = d.Remark,
                            SchemeId = d.SchemeId,
                            SchemeType = d.SchemeType,
                            TotalMoney = d.OrderTotalMoney
                        };

            totalCount = query.Count();
            return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<OCAagentDetailInfo> QueryAgentDetail(string agentId, string gameCode, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalBuyMoney
            , out decimal totalBounsMoney, out decimal totalRedBagAwardsMoney, out decimal totalBonusAwardsMoney, out decimal totalFillMoney, out decimal totalWithdrawalsMoney)
        {
            Session.Clear();
            starTime = starTime.Date;
            endTime = endTime.AddDays(1).Date;
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryAgentDetail"))
                .AddInParameter("agentId", agentId)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("starTime", starTime)
                .AddInParameter("endTime", endTime)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize);
            var ds = query.GetDataSet();
            totalFillMoney = ds.Tables[0].Rows[0]["TotalFillMoney"].ToString() == string.Empty ? 0M : (decimal)ds.Tables[0].Rows[0]["TotalFillMoney"];
            totalBuyMoney = ds.Tables[1].Rows[0]["TotalBuyMoney"].ToString() == string.Empty ? 0M : (decimal)ds.Tables[1].Rows[0]["TotalBuyMoney"];

            totalBounsMoney = ds.Tables[1].Rows[0]["TotalBounsMoney"].ToString() == string.Empty ? 0M : (decimal)ds.Tables[1].Rows[0]["TotalBounsMoney"];
            totalRedBagAwardsMoney = ds.Tables[1].Rows[0]["TotalRedBagAwardsMoney"].ToString() == string.Empty ? 0M : (decimal)ds.Tables[1].Rows[0]["TotalRedBagAwardsMoney"];
            totalBonusAwardsMoney = ds.Tables[1].Rows[0]["TotalBonusAwardsMoney"].ToString() == string.Empty ? 0M : (decimal)ds.Tables[1].Rows[0]["TotalBonusAwardsMoney"];

            totalCount = ds.Tables[2].Rows[0]["TotalCount"].ToString() == string.Empty ? 0 : (int)ds.Tables[2].Rows[0]["TotalCount"];
            totalWithdrawalsMoney = ds.Tables[4].Rows[0]["TotalWithdrawalsMoney"].ToString() == string.Empty ? 0M : (decimal)ds.Tables[4].Rows[0]["TotalWithdrawalsMoney"];
            return ORMHelper.DataTableToInfoList<OCAagentDetailInfo>(ds.Tables[3]).ToList();
        }
    }
}
