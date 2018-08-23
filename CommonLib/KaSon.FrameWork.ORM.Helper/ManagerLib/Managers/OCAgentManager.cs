using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class OCAgentManager:DBbase
    {
        public void AddOCAgent(P_OCAgent entity)
        {
            DB.GetDal<P_OCAgent>().Add(entity);
        }

        public void UpdateOCAgent(P_OCAgent entity)
        {
            DB.GetDal<P_OCAgent>().Update(entity);
        }

        public void AddOCAgentPayDetail(P_OCAgent_PayDetail entity)
        {
            DB.GetDal<P_OCAgent_PayDetail>().Add(entity);
        }

        public P_OCAgent QueryOCAgent(string userId)
        {
        
            return DB.CreateQuery<P_OCAgent>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// 查询用户所有的返点配置
        /// </summary>
        public List<P_OCAgent_Rebate> QueryOCAgentRebateList(string userId)
        {
          
            return DB.CreateQuery<P_OCAgent_Rebate>().Where(p => p.UserId == userId).OrderByDescending(p => p.CreateTime).ToList();
        }

        /// <summary>
        /// 新增用户返点配置
        /// </summary>
        public void AddOCAgentRebate(P_OCAgent_Rebate OCAgentRebate) {

             DB.GetDal<P_OCAgent_Rebate>().Add(OCAgentRebate);

        }

        /// <summary>
        /// 修改用户返点
        /// </summary>
        /// <param name="rebate"></param>
        public void UpdateOCAgentRebate(P_OCAgent_Rebate rebate) {

            DB.GetDal<P_OCAgent_Rebate>().Update(rebate);
        }

        /// <summary>
        /// 根据代理ID查询注册用户列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<C_User_Register> QueryAgentSubUser(string agentId)
        {
           
            return DB.CreateQuery<C_User_Register>().Where(p => p.AgentId == agentId).ToList();
        }

        /// <summary>
        /// 查询代理对应彩种的返点
        /// </summary>
        public P_OCAgent_Rebate QueryOCAgentDefaultRebateByRebateType(string userId, string gameCode, string gameType, int rebateType)
        {
           
            return DB.CreateQuery<P_OCAgent_Rebate>().OrderByDescending(p => p.CreateTime).Where(p => p.UserId == userId && p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType).FirstOrDefault();
        }
        /// <summary>
        /// 查询代理对应彩种的返点
        /// </summary>
        public P_OCAgent_Rebate QueryOCAgentDefaultRebate(string userId, string gameCode, string gameType, CPSMode mode)
        {
          
            return DB.CreateQuery<P_OCAgent_Rebate>().Where(p => p.CPSMode == (int)mode && p.UserId == userId && p.GameCode == gameCode && (gameType == "" || p.GameType == gameType)).FirstOrDefault();
        }
        public int QueryOCAgentPayDetailCount(string schemeId)
        {
           
            return DB.CreateQuery<P_OCAgent_PayDetail>().Where(p => p.SchemeId == schemeId).Count();
        }

        /// <summary>
        /// 店面代理信息
        /// </summary>
        public List<StoreMessageInfo> QueryStoreMessageByuserId(string userId)
        {
           
            var query = from a in DB.CreateQuery<P_OCAgent>()
                        join u in DB.CreateQuery<C_User_Register>() on a.UserId equals u.UserId
                        where a.ParentUserId == userId && a.OCAgentCategory == (int)OCAgentCategory.SportLotterySubAgent
                        select new StoreMessageInfo
                        {
                            UserId = a.UserId,
                            StoreId = a.StoreId,
                            UserName = u.DisplayName,
                            ParentUserId = a.ParentUserId,
                            OCAgentCategory = (OCAgentCategory)a.OCAgentCategory,
                            CustomerDomain = a.CustomerDomain,
                            CreateTime = a.CreateTime
                        };
            return query.ToList();
        }
        /// <summary>
        /// 根据店面编号查询店主id
        /// </summary>
        public P_OCAgent QueryUserIdBystoreId(string storeid)
        {
            
            return DB.CreateQuery<P_OCAgent>().Where(p => p.StoreId == storeid).FirstOrDefault();
        }

        public P_OCAgent QueryOCAgentByDomain(string domain)
        {
          
            return DB.CreateQuery<P_OCAgent>().Where(p => p.CustomerDomain == domain).FirstOrDefault();
        }

        /// <summary>
        /// 查询用户是否有返点
        /// </summary>
        public List<OCAgentRebateInfo> QueryUserRebateList(string userId)
        {
           
            var query = from a in DB.CreateQuery<P_OCAgent_Rebate>()
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

        /// <summary>
        /// 查询用户所有的返点配置
        /// </summary>
        public List<OCAgentRebateInfo> QueryOCAgentRebateInfoList(string userId)
        {
            
            var query = from a in DB.CreateQuery<P_OCAgent_Rebate>()
                        where a.UserId == userId
                        select new OCAgentRebateInfo
                        {
                            GameCode = a.GameCode,
                            GameType = a.GameType,
                            Rebate = a.Rebate,
                            SubUserRebate = a.SubUserRebate,
                            UserId = a.UserId,
                            RebateType = a.RebateType == 0 ? 0 : a.RebateType,
                            CreateTime = a.CreateTime,
                        };
            return query.ToList();
        }

        public List<OCAgentPayDetailInfo> QueryOCAgentPayDetailList(DateTime fromDate, DateTime toDate, string userId, int pageIndex, int pageSize, out int totalCount)
        {
          
            var query = from d in DB.CreateQuery<P_OCAgent_PayDetail>()
                        join u in DB.CreateQuery<C_User_Register>() on d.OrderUser equals u.UserId
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
                            SchemeType = (SchemeType)d.SchemeType,
                        };
            totalCount = query.Count();
            return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 发起中，未返点的订单
        /// </summary>
        public List<SubUserNoPayRebateOrderInfo> QuerySubUserCreateingOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out int userCount, out decimal totalMoney)
        {
            starTime = starTime.Date;
            endTime = endTime.AddDays(1).Date;
           
       

            var query = from o in DB.CreateQuery<C_Sports_Order_Running>()
                        join r in DB.CreateQuery<C_User_Register>() on o.UserId equals r.UserId
                        where o.CreateTime >= starTime && o.CreateTime < endTime
                        && (string.Empty == key || (r.UserId == key || r.DisplayName == key))
                        && o.AgentId == userId && (o.TicketStatus == (int)TicketStatus.Ticketing || o.TicketStatus == (int)TicketStatus.PrintTicket) && o.ProgressStatus == (int)ProgressStatus.Running
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
                            TicketStatus = (TicketStatus)o.TicketStatus,
                        };

            totalCount = query.Count();
            userCount = totalCount == 0 ? 0 : query.GroupBy(p => p.UserId).Select(p => new { Key = p.Key, Count = p.Count() }).ToList().Count;
            totalMoney = totalCount == 0 ? 0 : query.Sum(p => p.TotalMonery);
            return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<P_OCAgent_Rebate> QueryLowerAgentListByParentId(string parentId)
        {
           

            string strSql = "select r.CreateTime,isnull(r.GameCode,'')GameCode,isnull(r.GameType,'')GameType,r.Id,isnull(r.Rebate,0)Rebate,isnull(r.SubUserRebate,0)SubUserRebate,u.UserId from C_User_Register u left join P_OCAgent_Rebate r on u.UserId=r.UserId where u.ParentPath like '%" + parentId + "%'";
            var query = DB.CreateSQLQuery(strSql).List<object>();
            List<P_OCAgent_Rebate> ocList = new List<P_OCAgent_Rebate>();
            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    P_OCAgent_Rebate entity = new P_OCAgent_Rebate();
                    entity.CreateTime = Convert.ToDateTime(array[0]);
                    entity.GameCode = Convert.ToString(array[1]);
                    entity.GameType = Convert.ToString(array[2]);
                    entity.Id = Convert.ToInt32(array[3]);
                    entity.Rebate = Convert.ToDecimal(array[4]);
                    entity.SubUserRebate = Convert.ToDecimal(array[6]);
                    entity.UserId = Convert.ToString(array[6]);
                    ocList.Add(entity);
                }
            }

            return ocList;
        }

        public List<C_User_Register> QueryLowerAgentByUserId(string userId)
        {
            return DB.CreateQuery<C_User_Register>().Where(s => s.AgentId == userId && s.IsAgent == true).ToList();//20150819 一米要求过滤一级用户
            //return Session.Query<UserRegister>().Where(s => s.AgentId == userId).ToList();
        }
        #region 测试
        public List<C_User_Register> Test_QueryUserRegisterList()
        {
           
            return DB.CreateQuery<C_User_Register>().Where(s => s.AgentId != string.Empty).ToList();
        }
        public C_User_Register Test_QueryUserRegister(string userId)
        {
         
            return DB.CreateQuery<C_User_Register>().Where(s => s.UserId == userId).FirstOrDefault();
        }
        public void UpdateUserRigister(C_User_Register entity)
        {
            DB.GetDal<C_User_Register>().Update(entity);
        }
        #endregion

        /// <summary>
        /// 查询代理资金明细
        /// </summary>
        public List<OCAgentPayDetailInfo> QueryAgentRebateAndBonusDetail(string agentId, string schemeid, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, int cpsMode, int schemeType, out int totalCount)
        {
          
            var query = from d in DB.CreateQuery<P_OCAgent_PayDetail>()
                        join u in DB.CreateQuery<C_User_Register>() on d.OrderUser equals u.UserId
                        where (string.IsNullOrEmpty(agentId) || d.PayInUserId == agentId)
                        && (string.IsNullOrEmpty(schemeid) || d.SchemeId == schemeid)
                         && d.CreateTime >= starTime && d.CreateTime < endTime
                         && (cpsMode == -1 || d.CPSMode == (int)(CPSMode)cpsMode)
                         && (schemeType == -1 || d.SchemeType == (int)(SchemeType)schemeType)
                        select new OCAgentPayDetailInfo
                        {
                            CPSMode = (CPSMode)d.CPSMode,
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
                            SchemeType = (SchemeType)d.SchemeType,
                            TotalMoney = d.OrderTotalMoney
                        };

            totalCount = query.Count();
            return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        //public List<AgentLottoTopInfo> QueryLowerAgentSaleListByUserId(string agentId, DateTime startTime, DateTime endTime)
        //{
        //    Session.Clear();
        //    List<AgentLottoTopInfo> listInfo = new List<AgentLottoTopInfo>();
        //    startTime = startTime.Date;
        //    endTime = endTime.AddDays(1).Date;
        //    //原始
        //    string strSql = "select sum(tab.CTZQ) CTZQ,sum(tab.JCZQ)JCZQ,sum(tab.JCLQ)JCLQ,sum(tab.SZC)SZC,sum(tab.GPC)GPC,SUM(tab.BJDC) BJDC,sum(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney,UserId,DisplayName from (select r.UserId,r.DisplayName,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC from  C_OrderDetail o inner join C_User_Register r on o.UserId=r.UserId  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.BetTime>=:StartTime and o.BetTime<:EndTime  and r.UserId in (select UserId from C_User_Register where ParentPath  like '%" + agentId + "%' or UserId='" + agentId + "')  group by r.UserId,r.DisplayName) tab group by UserId,DisplayName";
        //    var result = DB.CreateSQLQuery(strSql)
        //                      //.SetParameterList("UserId", agentId)
        //                      .SetDateTime("StartTime", startTime)
        //                      .SetDateTime("EndTime", endTime)
        //                      .List();



        //    //20150925优化后SQl
        //    //string strSql = "select CTZQ,JCZQ,JCLQ,SZC,GPC,BJDC,(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney,utab.UserId,utab.DisplayName from  ( select o.UserId,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC from  C_OrderDetail o  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.BetTime>=:StartTime and o.BetTime<:EndTime   group by o.UserId) tab inner join  (select UserId,DisplayName from C_User_Register where ParentPath  like '%" + agentId + "%' or UserId='" + agentId + "') utab on tab.UserId=utab.UserId";

        //    //string strSql = "select CTZQ,JCZQ,JCLQ,SZC,GPC,BJDC,(tab.CTZQ+tab.JCZQ+tab.JCLQ+tab.SZC+tab.GPC+tab.BJDC) as totalMoney,tab.UserId,tab.DisplayName from  ( select o.UserId,SUM((case o.GameCode when 'CTZQ' then o.CurrentBettingMoney else 0 end)) CTZQ,SUM((case o.GameCode when 'JCZQ' then o.CurrentBettingMoney  else 0 end)) JCZQ,SUM((case o.GameCode when 'JCLQ' then o.CurrentBettingMoney else 0 end)) JCLQ,SUM((case o.GameCode when 'DLT' then o.CurrentBettingMoney when 'SSQ' then o.CurrentBettingMoney when 'FC3D' then o.CurrentBettingMoney when 'PL3' then o.CurrentBettingMoney else 0 end)) SZC,SUM((case o.GameCode when 'CQSSC' then o.CurrentBettingMoney when 'JX11X5' then o.CurrentBettingMoney else 0 end)) GPC,SUM((case o.GameCode when 'BJDC' then o.CurrentBettingMoney else 0 end)) BJDC,u.DisplayName from  C_OrderDetail o inner join C_User_Register u on o.UserId=u.UserId  where  IsVirtualOrder=0 and o.TicketStatus=90 and o.BetTime>=:StartTime and o.BetTime<:EndTime and (u.ParentPath like '%" + agentId + "%' or u.UserId='" + agentId + "')   group by o.UserId,u.DisplayName ) tab";
        //    //var result = Session.CreateSQLQuery(strSql)
        //    //                  .SetDateTime("StartTime", startTime)
        //    //                  .SetDateTime("EndTime", endTime)
        //    //                  .List();
        //    if (result != null && result.Count > 0)
        //    {
        //        foreach (var item in result)
        //        {
        //            AgentLottoTopInfo info = new AgentLottoTopInfo();
        //            var array = item as object[];
        //            info.CTZQ = Convert.ToDecimal(array[0]);
        //            info.JCZQ = Convert.ToDecimal(array[1]);
        //            info.JCLQ = Convert.ToDecimal(array[2]);
        //            info.SZC = Convert.ToDecimal(array[3]);
        //            info.BJDC = Convert.ToDecimal(array[4]);
        //            info.GPC = Convert.ToDecimal(array[5]);
        //            info.TotalMoney = Convert.ToDecimal(array[6]);
        //            info.UserId = Convert.ToString(array[7]);
        //            info.DisplayName = Convert.ToString(array[8]);
        //            listInfo.Add(info);
        //        }

        //    }
        //    return listInfo;
        //}

        /// <summary>
        /// 佣金管理，查询结算报表
        /// </summary>
        //public List<AgentPayDetailReportInfo> QueryAgentPayDetailReportInfo(string userId, DateTime fromDate, DateTime toDate)
        //{

        //    // 通过数据库存储过程进行查询
        //    var query = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Agent_GetPayDetailReport").SQL;
        //    var query_info= DB.CreateSQLQuery(query)
        //        .SetString("pAgentId", userId)
        //        .SetString("fromDate", fromDate.ToString("yyyy-MM-dd"))
        //        .SetString("toDate", toDate.ToString("yyyy-MM-dd"));
        //    DataTable dt = query_info.List<AgentPayDetailReportInfo>().;
        //    var list = new List<AgentPayDetailReportInfo>(); 
        //    foreach (DataRow row in dt.)
        //    {
        //        list.Add(new AgentPayDetailReportInfo
        //        {
        //            CommissionMoney = row["CommissionMoney"] == DBNull.Value ? 0M : decimal.Parse(row["CommissionMoney"].ToString()),
        //            GameCode = row["GameCode"] == DBNull.Value ? string.Empty : row["GameCode"].ToString(),
        //            GameType = row["GameType"] == DBNull.Value ? string.Empty : row["GameType"].ToString(),
        //            TotalMoney = row["TotalMoney"] == DBNull.Value ? 0M : decimal.Parse(row["TotalMoney"].ToString()),
        //            UserId = row["UserId"] == DBNull.Value ? string.Empty : row["UserId"].ToString()
        //        });
        //    }
        //    return list;
        //    //return Common.Database.ORM.ORMHelper.DataTableToList<AgentPayDetailReportInfo>(dt);
        //}
    }
}
