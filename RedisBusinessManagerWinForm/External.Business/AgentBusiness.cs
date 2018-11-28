using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Domain.Entities.Agent;
using External.Domain.Managers.Agent;
using External.Core.Agnet;
using System.Collections;
using GameBiz.Core;
using GameBiz.Business;
using Common.Utilities;
using System.Diagnostics;
using System.Data;
using Common.Database.ORM;
using GameBiz.Domain.Entities;
using GameBiz.Auth.Business;
using Common.XmlAnalyzer;
using GameBiz.Domain.Managers;
using GameBiz.Business.Domain.Managers;
using External.Core.Login;
namespace External.Business
{
    public class AgentBusiness : IRegister_AfterTranCommit//,IAgentPayIn
    {
        private DataTable agentReturnPointCollection;
        public DataTable AgentReturnPointCollection
        {
            get
            {
                if (agentReturnPointCollection == null)
                {
                    agentReturnPointCollection = new DataTable();
                    agentReturnPointCollection.Columns.Add("GameCode", typeof(String));
                    agentReturnPointCollection.Columns.Add("GameType", typeof(String));
                    agentReturnPointCollection.Columns.Add("AgentId", typeof(String));
                    agentReturnPointCollection.Columns.Add("SetLevel", typeof(Int32));
                    agentReturnPointCollection.Columns.Add("ReturnPoint", typeof(Decimal));
                    agentReturnPointCollection.Columns.Add("PAgentId", typeof(String));
                    agentReturnPointCollection.Columns.Add("PSetLevel", typeof(Int32));
                    agentReturnPointCollection.Columns.Add("PReturnPoint", typeof(Decimal));
                }
                return agentReturnPointCollection;
            }
        }

        /// <summary>
        /// 添加代理用户
        /// </summary>
        public void AddAgentUser(string existUserId, int agentLevel, string upUserId)
        {
            var agentManager = new AgentManager();
            var userBiz = new UserBusiness();
            var existUser = userBiz.GetRegisterById(existUserId);
            if (existUser == null)
            {
                throw new Exception("未能找到用户");
            }
            if (existUser.IsAgent)
            {
                throw new Exception("该用户已经是代理商");
            }
            if (!string.IsNullOrWhiteSpace(upUserId))
            {
                var upUser = userBiz.GetRegisterById(upUserId);
                if (upUser == null)
                {
                    throw new Exception("未能找到用户");
                }
                upUserId = upUser.UserId;
            }

            existUser.AgentId = string.IsNullOrWhiteSpace(upUserId) ? "" : upUserId;
            existUser.IsAgent = true;
            agentManager.UpdateAgentUser(existUser);

            var authBiz = new GameBizAuthBusiness();
            authBiz.AddUserRoles(existUserId, new string[] { "Agent" });

            AddAgentInitialInfoPoint(existUserId, agentLevel, upUserId);
        }

        public void AddAgentInitialInfoPoint(string existUserId, int agentLevel, string upUserId)
        {
            var agentManager = new AgentManager();
            //var list = agentManager.GetAgentReturnPointList(upUserId);
            var list = agentManager.GetAgentReturnPointListByUserId(upUserId);
            var pointList = new List<AgentReturnPointReality>();

            var pointOld = agentManager.GetAgentReturnPointList(existUserId);
            if (pointOld.Count > 0)
            {
                return;
            }

            foreach (var item in list)
            {
                var pointMy = item.ReturnPoint;
                var maxReturnPoint = GetMaxReturnPoint(item.GameCode, item.GameType);
                pointMy = pointMy > maxReturnPoint ? maxReturnPoint : pointMy;
                pointMy = pointMy < 0 ? 0 : pointMy;
                var pointLower = pointMy - GetReservReturnPoint(item.GameCode, item.GameType);
                pointLower = pointLower < 0 ? 0 : pointLower;

                pointList.Add(new AgentReturnPointReality()
                {
                    UserId = existUserId,
                    GameCode = item.GameCode,
                    GameType = item.GameType,
                    MyPoint = pointMy,
                    MyUpTime = DateTime.Now,

                    LowerPoint = pointLower,
                    LowerUpTime = DateTime.Now
                });
            }

            agentManager.AddAgentReturnPoint(pointList.ToArray());
        }

        public UserRegister GetRegisterById(string agentId)
        {
            var userBiz = new UserBusiness();
            var existUser = userBiz.GetRegisterById(agentId);
            if (existUser == null)
            {
                throw new Exception("未能找到用户");
            }
            return existUser;
        }

        /// <summary>
        /// 查询代理用户
        /// </summary>
        public IList<AgentUserInfo> GetAgentUserByKeyword(string keyword, int isAgent, string pAgentId, int pageIndex, int pageSize, out int totalCount)
        {
            var manager = new AgentManager();
            return manager.GetAgentUserByKeyword(keyword, isAgent, pAgentId, pageIndex, pageSize, out totalCount);
        }
        public IList<AgentCommissionDetailInfo> GetAgentCommissionDetailList(string pAgentId, string userId, string displayName, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, int applyState, out int totalCount)
        {
            var manager = new AgentManager();
            return manager.GetAgentCommissionDetailList(pAgentId, userId, displayName, fromDate, toDate, pageIndex, pageSize, applyState, out  totalCount);
        }
        public IList<AgentCommissionDetailInfo> GetCommissionReport(string userId, string displayName, int applyState, DateTime fromDate, DateTime toDate, string pAgentId, out decimal saleTotal, out decimal actualCommissionTotal)
        {
            var manager = new AgentManager();
            return manager.GetCommissionReport(userId, displayName, fromDate, toDate, pAgentId, applyState, out saleTotal, out actualCommissionTotal);
        }
        public IList<AgentCommissionApplyInfo> GetAgentCommissionApplyList(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, int applyState, string pAgentId, string userId, out int totalCount)
        {
            var manager = new AgentManager();
            return manager.GetAgentCommissionApplyList(fromDate, toDate, pageIndex, pageSize, applyState, pAgentId, userId, out  totalCount);
        }

        public IList<AgentLottoTopInfo> QueryAgentLottoTopList(string agentId, string userId, string displayName, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount)
        {
            var manager = new AgentManager();
            return manager.QueryAgentLottoTopList(agentId, userId, displayName, fromDate, toDate, pageIndex, pageSize, out  totalCount);
        }

        public IList<AgentFillMoneyTopInfo> QueryAgentFillMoneyTopList(string agentId, string userId, string displayName, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount)
        {
            var manager = new AgentManager();
            return manager.QueryAgentFillMoneyTopList(agentId, userId, displayName, fromDate, toDate, pageIndex, pageSize, out  totalCount);
        }

        public IList<AgentSchemeInfo> GetAgentScheme(string agentId, string userId, string displayName, int progressStatus, int ticketStatus, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount, out int totalUser, out int totalScheme, out decimal totalMoney1)
        {
            var manager = new AgentManager();
            return manager.GetAgentScheme(agentId, userId, displayName, progressStatus, ticketStatus, fromDate, toDate, pageIndex, pageSize, out  totalCount, out  totalUser, out  totalScheme, out  totalMoney1);
        }

        public AgentCommissionDetailCollection GetAgentGetCommissionReportList(string gameCode, string gameType, string pAgentId, string userId, string displayName, int category, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            var manager = new AgentManager();
            return manager.GetAgentGetCommissionReportList(gameCode, gameType, pAgentId, userId, displayName, category, fromDate, toDate, pageIndex, pageSize);
        }

        public AgentWithdrawRecordCollection GetAgentWithdrawRecord(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            var manager = new AgentManager();
            return manager.GetAgentWithdrawRecord(userId, fromDate, toDate, pageIndex, pageSize);
        }


        /// <summary>
        /// 申请结算佣金
        /// </summary>
        public void AddAgentCommissionApply(DateTime toDate, decimal deduction, string agentId)
        {
            var manager = new AgentManager();
            var applyTime = GetCommissionApplyTime(agentId);
            if (toDate > applyTime[1])
            {
                throw new Exception("结算时间不能大于" + applyTime[1].ToString("yyyy-MM-dd"));
            }
            if (toDate < applyTime[0])
            {
                throw new Exception("结算时间不能小于" + applyTime[0].ToString("yyyy-MM-dd"));
            }

            decimal commission = 0;
            decimal dealSale = 0;
            var agentCommissionApplyList = manager.GetCommissionReportByFromTimeAndToDateAndPAgentId(applyTime[0], toDate.AddDays(+1), agentId, 0);
            if (agentCommissionApplyList.Count > 0)
            {
                foreach (var item in agentCommissionApplyList)
                {
                    item.ApplyState = 1;
                }
                commission = agentCommissionApplyList.Sum(o => o.BeforeCommission);
                commission = commission * (100 - deduction) / 100;
                dealSale = agentCommissionApplyList.Sum(o => o.Sale);
            }

            manager.UpdateAgentCommissionDetail(agentCommissionApplyList);

            manager.AddAgentCommissionApply(new AgentCommissionApply()
            {
                ID = UsefullHelper.UUID(),
                RequestTime = DateTime.Now,
                FromTime = applyTime[0],
                ToTime = toDate,
                RequestUserId = agentId,
                RequestCommission = commission,
                ResponseCommission = null,
                ResponseUserId = null,
                DealSale = dealSale,
                ResponseTime = null,
                ApplyState = 1,
                Remark = null
            });
        }

        public void UpdateAgentCommissionApply(string applyid, string adminId)
        {
            var manager = new AgentManager();
            var commissionApply = manager.GetAgentCommissionApply(applyid);
            if (commissionApply == null)
            {
                throw new Exception("未能找到订单");
            }
            string remark = string.Format("成功提取佣金-金额￥{1:N2}", applyid, commissionApply.RequestCommission);
            commissionApply.ResponseCommission = commissionApply.RequestCommission;
            commissionApply.ResponseUserId = adminId;
            commissionApply.ResponseTime = DateTime.Now;
            commissionApply.ApplyState = 2;
            commissionApply.Remark = remark;
            manager.UpdateAgentCommissionApply(commissionApply);

            //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_Commission, applyid, applyid, false, string.Empty,
            //    string.Empty, commissionApply.RequestUserId, AccountType.Commission, commissionApply.RequestCommission, remark);

            BusinessHelper.Payin_To_Balance(AccountType.Commission, BusinessHelper.FundCategory_Commission, commissionApply.RequestUserId, applyid
                , commissionApply.RequestCommission, remark);

        }

        public void UpdateAgentCommissionApplyByDecline(string applyId, string adminId)
        {
            var manager = new AgentManager();
            var commissionApply = manager.GetAgentCommissionApply(applyId);
            if (commissionApply == null)
            {
                throw new Exception("未能找到订单");
            }
            commissionApply.ResponseCommission = commissionApply.RequestCommission;
            commissionApply.ResponseUserId = adminId;
            commissionApply.ResponseTime = DateTime.Now;
            commissionApply.ApplyState = 3;
            commissionApply.Remark = "";
            manager.UpdateAgentCommissionApply(commissionApply);

            var agentCommissionApplyList = manager.GetCommissionReportByFromTimeAndToDateAndPAgentId(commissionApply.FromTime, commissionApply.ToTime, commissionApply.RequestUserId, 1);

            foreach (var item in agentCommissionApplyList)
            {
                item.ApplyState = 0;
            }
            manager.UpdateAgentCommissionDetail(agentCommissionApplyList);
        }

        //public AgentReturnPointCollection GetAgentReturnPointByAgentId(string agentIdFrom, string agentIdTo, bool isUpAndLower)
        //{
        //    var manager = new AgentManager();
        //    return manager.GetAgentReturnPointList(agentIdFrom, agentIdTo, isUpAndLower);

        //    //IList<AgentReturnPoint> list = manager.GetAgentReturnPointByAgentIdFromAndAgentIdTo(agentIdFrom, agentIdTo);
        //    //IList<AgentReturnPointInfo> collection = new List<AgentReturnPointInfo>();
        //    //ObjectConvert.ConvertEntityListToInfoList<IList<AgentReturnPoint>, AgentReturnPoint, IList<AgentReturnPointInfo>, AgentReturnPointInfo>(list, ref collection, () => new AgentReturnPointInfo());
        //    //return new AgentReturnPointCollection()
        //    //{
        //    //    AgentReturnPointList = collection
        //    //};
        //}

        public IList<AgentReturnPointRealityInfo> GetAgentReturnPointByHighAndLow(string userId)
        {
            var manager = new AgentManager();
            return manager.GetAgentReturnPointByHighAndLow(userId);
        }
        public IList<AgentReturnPointInitialInfo> GetAgentReturnPointList(string agentId)
        {
            var manager = new AgentManager();

            var list = manager.GetAgentReturnPointList(agentId);

            IList<AgentReturnPointInitialInfo> collection = new List<AgentReturnPointInitialInfo>();

            ObjectConvert.ConvertEntityListToInfoList<IList<AgentReturnPointReality>, AgentReturnPointReality
                , IList<AgentReturnPointInitialInfo>, AgentReturnPointInitialInfo>(list, ref collection, () => new AgentReturnPointInitialInfo());
            return collection;
        }

        //public AgentReturnPointCollection GetMyReturnPoint(string userId)
        //{
        //    var user = GetRegisterById(userId);
        //    var manager = new AgentManager();
        //    if (string.IsNullOrWhiteSpace(user.AgentId))
        //    {
        //        user.AgentId = "-";
        //    }

        //    IList<AgentReturnPoint> list = manager.GetAgentReturnPointByAgentIdFromAndAgentIdTo(user.AgentId, user.UserId);
        //    IList<AgentReturnPointInfo> collection = new List<AgentReturnPointInfo>();
        //    ObjectConvert.ConvertEntityListToInfoList<IList<AgentReturnPoint>, AgentReturnPoint, IList<AgentReturnPointInfo>, AgentReturnPointInfo>(list, ref collection, () => new AgentReturnPointInfo());
        //    return new AgentReturnPointCollection()
        //    {
        //        AgentReturnPointList = collection
        //    };
        //}

        public DateTime[] GetCommissionApplyTime(string pAgentId)
        {
            var manager = new AgentManager();
            var result = manager.AgentCommissionApplyByUserId(pAgentId, 2);//applyState=2  2表示已经完结的
            var toTime = DateTime.Now.Date.AddDays(-1);
            if (result == null)
            {
                return new DateTime[2] { DateTime.Parse("2012-10-01"), toTime };
            }
            return new DateTime[2] { result.ToTime.AddDays(+1), toTime };
        }

        public void AddAgentCommissionDetailByTicketing(string userId, string gameCode, string gameType, decimal money, decimal deduction, string schemeId, SchemeType schemeType, DateTime complateTime, AgentCaculateHistory history)
        {
            try
            {
                gameType = ConvertGameType(gameCode, gameType);
                var manager = new AgentManager();
                int category = 0;
                string agentId = userId;
                while (!string.IsNullOrWhiteSpace(agentId))
                {
                    category += 1;
                    AgentCloseReturnPointInfo info;
                    var rows = AgentReturnPointCollection.Select("AgentId='" + agentId + "' and GameCode='" + gameCode + "' and GameType='" + gameType + "'");
                    if (rows.Length == 0)
                    {
                        info = manager.GetAgentReturnPointByUserId(agentId, gameCode, gameType);
                        if (info == null)
                        {
                            throw new Exception("没有返点信息");
                        }
                        AgentReturnPointCollection.Rows.Add
                        (
                            info.GameCode,
                            info.GameType,
                            info.AgentId,
                            info.SetLevel,
                            info.ReturnPoint,
                            info.PAgentId,
                            info.PSetLevel,
                            info.PReturnPoint
                        );
                    }
                    else
                    {
                        info = ORMHelper.ConvertDataRowToEntity<AgentCloseReturnPointInfo>(rows[0]);
                    }

                    if (string.IsNullOrWhiteSpace(info.PAgentId))
                    {
                        break;
                    }

                    decimal initialPoint = info.PReturnPoint;
                    decimal lowerPoint = info.ReturnPoint;
                    decimal actualPoint = initialPoint - lowerPoint;
                    if (actualPoint < 0)
                    {
                        throw new Exception("实际返点小于0");
                    }
                    if (actualPoint == 0)
                    {
                        break;
                    }
                    agentId = info.PAgentId;

                    var beforeCommission = money * actualPoint / 100;
                    var actualCommission = beforeCommission * (100 - deduction) / 100;

                    history.TotalCommisionMoney += actualCommission;

                    //需要添加用户佣金明细资金
                    manager.AddAgentCommissionDetail(new AgentCommissionDetail()
                    {
                        GameCode = gameCode,
                        GameType = ConvertGameType(gameCode, gameType),
                        ApplyState = 0,
                        CreateTime = DateTime.Now,
                        PAgentId = agentId,//给上级用户
                        UserId = userId,//传个来的用户
                        Category = category,
                        Sale = money,
                        InitialPoint = initialPoint,//上级用户的返点
                        LowerPoint = lowerPoint,//传个来的用户的返点
                        ActualPoint = actualPoint,  // 0,//上级用户的返点 - 传个来的用户的返点
                        Deduction = deduction,
                        BeforeCommission = beforeCommission,
                        ActualCommission = actualCommission,
                        //代购方案￥100.00 返点2.00%=￥2.00-扣量5%=￥1.90///去除扣量2012-11-23 23：15：51
                        Remark = string.Format("{0}￥{1:N2} 返点{2:#.##}%=￥{3:N2}"
                        , schemeType == SchemeType.TogetherBetting ? "成功发起合买" : "代购方案", money, actualPoint, beforeCommission),
                        DetailKeyword = schemeId,
                        ComplateDateTime = complateTime
                    });

                    userId = agentId;
                }

                //需要添加用户资金
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AgentCommissionClose(string createByUserId)
        {
            var manager = new AgentManager();
            var history = manager.GetAgentCaculateHistoryByDesc();
            if (history == null)
            {
                history = new AgentCaculateHistory()
                {
                    CaculateTimeFrom = DateTime.Parse("2012-10-01"),
                    CaculateTimeTo = DateTime.Now
                };
            }
            else
            {
                history = new AgentCaculateHistory()
                {
                    CaculateTimeFrom = history.CaculateTimeTo,
                    CaculateTimeTo = DateTime.Now
                };
            }

            if (history.CaculateTimeFrom >= history.CaculateTimeTo)
            {
                return;
            }

            bool tag = true;
            int totalCount = 0;
            int pageIndex = 0;
            int pageSize = 100;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                var waitingOrderList = manager.QueryAgentWaitingCommissionOrderList(history.CaculateTimeFrom, history.CaculateTimeTo, pageIndex, pageSize, out totalCount);

                foreach (var item in waitingOrderList)
                {
                    try
                    {
                        AddAgentCommissionDetailByTicketing(item.UserId, item.GameCode, item.GameType, item.TotalBuyMoney, 5, item.SchemeId, item.SchemeType, item.ComplateTime, history);
                        history.TotalOrderCount += 1;
                        history.TotalOrderMoney += item.TotalMoney;
                        history.TotalBuyMoney += item.TotalBuyMoney;
                    }
                    catch (Exception ex)
                    {
                        history.ErrorCount += 1;
                        history.ErrorOrderMoney += item.TotalMoney;
                        history.ErrorBuyMoney += item.TotalBuyMoney;
                        history.ErrorSchemeIdList += item.SchemeId + ";";
                        var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    }
                }

                pageIndex += 1;
                if (((totalCount + pageSize - 1) / pageSize) <= pageIndex)
                {
                    tag = false;
                }
            }
            while (tag);

            stopwatch.Stop();

            history.MillisecondSpan = stopwatch.ElapsedMilliseconds;
            history.CreateBy = createByUserId;
            history.CreateTime = DateTime.Now;
            history.TotalAgentCount = ORMHelper.DataTableToList<AgentCloseReturnPointInfo>(AgentReturnPointCollection).GroupBy(o => o.AgentId).Count();
            manager.AddAgentCaculateHistory(history);



        }

        public string ConvertGameType(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "bjdc":
                case "jczq":
                case "jclq":
                case "ctzq":
                    return gameType;
                default:
                    return "";
            }
        }
        private static object agentLock = new object();
        public void AgentPayIn(string userId, string gameCode, string gameType, decimal money, string schemeId, SchemeType schemeType)
        {
            try
            {
                if (SchemeType.TogetherBetting == schemeType)
                {
                    var sportsManager = new Sports_Manager();
                    var main = sportsManager.QuerySports_Together(schemeId);
                    if (main.SystemGuarantees <= 0 && !bool.Parse(new CacheDataBusiness().QueryCoreConfigByKey("IsTogetherFandian").ConfigValue))
                        return;
                }

                var manager = new AgentManager();
                var isExistScheme = manager.GetRebateDetail(schemeId);
                if (!isExistScheme)
                    return;
                //lock (agentLock)
                //{
                using (var biz = new GameBiz.Business.GameBizBusinessManagement())
                {
                    biz.BeginTran();

                    gameType = ConvertGameType(gameCode, gameType);

                    var info = manager.GetAgentReturnPointByUserId(userId, gameCode, gameType);
                    if (info != null)
                    {
                        decimal commission = money * info.ReturnPoint / 100;
                        string remark = string.Format("方案{0}出票成功，返利{1:N2}%，共{2:N2}元。", schemeId, info.ReturnPoint, commission);
                        if (commission > 0)
                        {
                            //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_SchemeDeduct, schemeId, schemeId, false, string.Empty, string.Empty, userId,
                            //    AccountType.Common, commission, remark);

                            BusinessHelper.Payin_To_Balance(AccountType.Commission, BusinessHelper.FundCategory_SchemeDeduct, userId, schemeId, commission, remark, RedBagCategory.Partner);
                        }

                        manager.AddRebateDetail(new RebateDetail()
                        {
                            SchemeId = schemeId,
                            SchemeType = schemeType,
                            UserId = userId,
                            CreateTime = DateTime.Now,
                            GameCode = gameCode,
                            GameType = gameType,
                            Point = info.ReturnPoint,
                            PayinMoney = commission,
                            Remark = remark
                        });
                    }

                    AgentPayInByReturnPoint(userId, gameCode, gameType, money, 0, 0, schemeId, schemeType);

                    biz.CommitTran();
                }
                //}
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_AgentPayIn_Error_2222222222", userId, ex);
                //throw ex;
            }
        }

        public void AgentPayInByReturnPoint(string userId, string gameCode, string gameType, decimal money, decimal deduction, int category, string schemeId, SchemeType schemeType)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            category += 1;
            var manager = new AgentManager();
            var info = manager.GetAgentReturnPointByUserId(userId, gameCode, gameType);
            if (info != null)
            {
                decimal initialPoint = info.PReturnPoint;
                decimal lowerPoint = info.ReturnPoint;
                decimal actualPoint = initialPoint - lowerPoint;
                if (actualPoint > 0)
                {
                    var beforeCommission = money * actualPoint / 100;
                    var actualCommission = beforeCommission * (100 - deduction) / 100;

                    string remark = string.Format("方案{0}出票成功，返利{1:N2}%，共{2:N2}元。", schemeId, actualPoint, beforeCommission);
                    //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_SchemeDeduct, schemeId, schemeId, false, string.Empty, string.Empty
                    //    , info.PAgentId, AccountType.Commission, beforeCommission, remark);

                    BusinessHelper.Payin_To_Balance(AccountType.Commission, BusinessHelper.FundCategory_SchemeDeduct, info.PAgentId, schemeId, beforeCommission, remark);


                    //需要添加用户佣金明细资金
                    manager.AddAgentCommissionDetail(new AgentCommissionDetail()
                    {
                        GameCode = gameCode,
                        GameType = ConvertGameType(gameCode, gameType),
                        ApplyState = 1,
                        CreateTime = DateTime.Now,
                        PAgentId = info.PAgentId,//给上级用户
                        UserId = userId,//传个来的用户
                        Category = category,
                        Sale = money,
                        InitialPoint = initialPoint,//上级用户的返点
                        LowerPoint = lowerPoint,//传个来的用户的返点
                        ActualPoint = actualPoint,  // 0,//上级用户的返点 - 传个来的用户的返点
                        Deduction = deduction,
                        BeforeCommission = beforeCommission,
                        ActualCommission = actualCommission,
                        //代购方案￥100.00 返点2.00%=￥2.00-扣量5%=￥1.90///去除扣量2012-11-23 23：15：51
                        Remark = string.Format("{0}￥{1:N2} 返点{2:#.##}%=￥{3:N2}"
                        , schemeType == SchemeType.TogetherBetting ? "成功发起合买" : "代购方案", money, actualPoint, beforeCommission),
                        DetailKeyword = schemeId,
                        ComplateDateTime = DateTime.Now
                    });

                }
            }

            string PAgentId = string.Empty;
            try
            {
                var user = GetRegisterById(userId);
                PAgentId = user.AgentId;
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_AgentPayInByReturnPoint_Error_", userId, ex);
                throw ex;
            }
            AgentPayInByReturnPoint(PAgentId, gameCode, gameType, money, deduction, category, schemeId, schemeType);

        }

        public void AfterRegisterTranCommit(string regType, string userId)
        {
            var entity = GetRegisterById(userId);
            AddAgentInitialInfoPoint(userId, 2, entity.AgentId);
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IAgentPayIn":
                        AgentPayIn((string)paraList[0], (string)paraList[1], (string)paraList[2], (decimal)paraList[3], (string)paraList[4], (SchemeType)paraList[5]);
                        break;
                    case "IRegister_AfterTranCommit":
                        AfterRegisterTranCommit((string)paraList[0], (string)paraList[1]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_AgentBusiness_Error_", type, ex);
            }

            return null;
        }

        public bool CheckIsApply(string userId)
        {
            var manager = new AgentManager();
            var result = manager.AgentCommissionApplyByUserId(userId);
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public string GetUserCount(string userId)
        {
            var manager = new AgentManager();
            return string.Format("{0}|{1}", manager.GetUserCount(userId, false), manager.GetUserCount(userId, true));
        }

        public void SearchReturnPoint(IList<AgentReturnPointRealityInfo> highLowList, string gameCode, string gameType, decimal returnPoint)
        {
            try
            {
                var temp = highLowList.FirstOrDefault(o => o.GameCode == gameCode && o.GameType == gameType);
                if (temp != null)
                {
                    decimal i = temp.UpPoint - GetReservReturnPoint(gameCode, gameType);
                    if (returnPoint > i)
                    {
                        throw new Exception(GameName(gameCode, gameType) + "下级不能大于上级的返点:" + i.ToString("#.##"));
                    }
                }
                else
                {
                    throw new Exception(GameName(gameCode, gameType) + "上级未设置返点");
                }
                temp = highLowList.FirstOrDefault(o => o.GameCode == gameCode && o.GameType == gameType);
                if (temp != null)
                {
                    decimal i = temp.LowerPoint;//+ GetReservReturnPoint(gameCode, gameType);
                    if (returnPoint < i)
                    {
                        throw new Exception(GameName(gameCode, gameType) + "不能小于下级的返点:" + i.ToString("#.##"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public decimal GetReservReturnPoint(string gameCode, string gameType)
        {
            var returnpoint = "";
            if (string.IsNullOrEmpty(gameType))
                returnpoint = "ReservReturnPoint." + gameCode;
            else
                returnpoint = "ReservReturnPoint." + gameCode + "." + gameType;
            var config = QueryCoreConfigByKey().Where(p => p.ConfigKey == returnpoint).FirstOrDefault();
            if (string.IsNullOrEmpty(config.ConfigValue))
                return 0;
            else
                return decimal.Parse(config.ConfigValue);
            //var xElement = SettingConfigAnalyzer.GetConfigXElementByKey("ReservReturnPoint");
            //var element = xElement.Elements("Point").FirstOrDefault(o => o.Attribute("GameCode").Value == gameCode && o.Attribute("GameType").Value == gameType);
            //if (element == null)
            //{
            //    return 0;
            //}
            //return decimal.Parse(element.Attribute("Value").Value);
        }
        public decimal GetMaxReturnPoint(string gameCode, string gameType)
        {
            //最大返点
            var key = "";
            if (string.IsNullOrEmpty(gameType))
                key = "MaxReturnPoint." + gameCode;
            else
                key = "MaxReturnPoint." + gameCode + "." + gameType;
            var config = QueryCoreConfigByKey().Where(p => p.ConfigKey == key).FirstOrDefault();
            //var maxReturnPoint = string.IsNullOrEmpty(config.ConfigValue) ? 0 : decimal.Parse(config.ConfigValue);
            if (string.IsNullOrEmpty(config.ConfigValue))
                return 0;
            else
                return decimal.Parse(config.ConfigValue);
            //var xElement = SettingConfigAnalyzer.GetConfigXElementByKey("MaxReturnPoint");
            //var element = xElement.Elements("Point").FirstOrDefault(o => o.Attribute("GameCode").Value == gameCode && o.Attribute("GameType").Value == gameType);
            //if (element == null)
            //{
            //    return 8;
            //}
            //return decimal.Parse(element.Attribute("Value").Value);
        }
        public void SearchMaxReturnPoint(string gameCode, string gameType, decimal returnPoint)
        {
            var maxReturnPoint = GetMaxReturnPoint(gameCode, gameType);
            if (returnPoint > maxReturnPoint)
            {
                throw new Exception(GameName(gameCode, gameType) + "所设置的返点不能大于系统指定的" + maxReturnPoint.ToString("#.##") + "点");
            }
        }

        public bool SearchIsLowDropPoint()
        {
            //var xElement = SettingConfigAnalyzer.GetConfigXElementByKey("IsLowDropPoint");
            //var element = xElement.Attribute("Value").Value;
            //if (string.IsNullOrWhiteSpace(element))
            //{
            //    return true;
            //}
            //return bool.Parse(element);
            var config = QueryCoreConfigByKey().Where(p => p.ConfigKey == "IsLowDropPoint").FirstOrDefault();
            return bool.Parse(config.ConfigValue);
        }

        public void SearchIsLowDropPoint(IList<AgentReturnPointInitialInfo> currentPoint, string gameCode, string gameType, decimal returnPoint)
        {
            var temp = currentPoint.FirstOrDefault(o => o.GameCode == gameCode && o.GameType == gameType);
            if (temp != null)
            {
                if (temp.MyPoint > returnPoint)
                {
                    throw new Exception(GameName(gameCode, gameType) + "所设置的返点不能降低");
                }
            }
        }

        /// <summary>
        /// 根据彩种编码（玩法类型）获取彩种（玩法）名称
        /// </summary>
        /// <param name="gamecode">彩种编码</param>
        /// <param name="type">玩法编码，可为空</param>
        /// <returns>彩种（玩法）名称</returns>
        public string GameName(string gamecode, string type = "")
        {
            if (string.IsNullOrEmpty(gamecode))
            {
                return "";
            }
            type = string.IsNullOrEmpty(type) ? gamecode : type;
            //根据彩种编号获取彩种名称
            switch (gamecode.ToLower())
            {
                case "cqssc": return "时时彩";
                case "jxssc": return "新时时彩";
                case "sd11x5": return "老11选5";
                case "gd11x5": return "新11选5";
                case "jx11x5": return "11选5";
                case "pl3": return "排列三";
                case "fc3d": return "福彩3D";
                case "ssq": return "双色球";
                case "qxc": return "七星彩";
                case "qlc": return "七乐彩";
                case "dlt": return "大乐透";
                case "sdqyh": return "群英会";
                case "gdklsf": return "快乐十分";
                case "gxklsf": return "广西快乐十分";
                case "jsks": return "江苏快3";
                case "jczq":
                    switch (type.ToLower())
                    {
                        case "spf": return "竞彩胜平负";
                        case "bf": return "竞彩比分";
                        case "zjq": return "竞彩总进球数";
                        case "bqc": return "竞彩半全场";
                        case "hh": return "竞彩混合过关";
                        default: return "竞彩足球";
                    }
                case "jclq":
                    switch (type.ToLower())
                    {
                        case "sf": return "篮球胜负";
                        case "rfsf": return "篮球让分胜负";
                        case "sfc": return "篮球胜分差";
                        case "dxf": return "篮球大小分";
                        case "hh": return "篮球混合过关";
                        default: return "竞彩篮球";
                    }
                case "ctzq":
                    switch (type.ToLower())
                    {
                        case "t14c": return "14场胜负";
                        case "tr9": return "任9场";
                        case "t6bqc": return "6场半全";
                        case "t4cjq": return "4场进球";
                        default: return "传统足球";
                    }
                case "bjdc":
                    switch (type.ToLower())
                    {
                        case "sxds": return "单场上下单双";
                        case "spf": return "单场胜平负";
                        case "zjq": return "单场总进球";
                        case "bf": return "单场比分";
                        case "bqc": return "单场半全场";
                        default: return "北京单场";
                    }
                default: return gamecode;
            }
        }
        public void CheckIsItsUser(string agentId, string userId)
        {
            var manager = new AgentManager();
            var entity = manager.GetUserByAgentIdAndUserId(agentId, userId);
            if (entity == null)
            {
                throw new Exception("只能修改自己的下线用户");
            }
        }
        /// <summary>
        /// 返点配置
        /// </summary>
        public void UpdateAgentReturnPoint(List<AgentReturnPointRealityInfo> list, string userId)
        {
            using (var agentManager = new AgentManager())
            {
                var agentReturnPoint = agentManager.GetAgentReturnPointList(userId);
                var reality = new List<AgentReturnPointReality>();
                foreach (var item in list)
                {
                    var temp = agentReturnPoint.FirstOrDefault(o => o.GameCode == item.GameCode && (item.GameType == string.Empty || o.GameType == item.GameType));
                    if (temp == null)
                    {
                        agentManager.AddAgentReturnPoint(new AgentReturnPointReality
                        {
                            UserId = userId,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            MyPoint = item.MyPoint,
                            MyUpTime = DateTime.Now,
                        });
                    }
                    else
                    {
                        temp.MyPoint = item.MyPoint == 0 ? temp.MyPoint : item.MyPoint;
                        temp.MyUpTime = DateTime.Now;
                        agentManager.UpdateAgentReturnPoint(temp);
                    }
                }
            }
        }

        //public void UpdateAgentInitialPoint(List<AgentReturnPointInitialInfo> list, string agentId)
        //{
        //    using (var agentManager = new AgentManager())
        //    {
        //        var agentReturnPoint = agentManager.GetAgentReturnPointList(agentId);
        //        var reality = new List<AgentReturnPointReality>();
        //        foreach (var item in list)
        //        {
        //            var temp = agentReturnPoint.FirstOrDefault(o => o.GameCode == item.GameCode && o.GameType == item.GameType);
        //            if (temp == null)
        //            {
        //                agentManager.AddAgentReturnPoint(new AgentReturnPointReality
        //                {
        //                    UserId = agentId,
        //                    GameCode = item.GameCode,
        //                    GameType = item.GameType,
        //                    LowerPoint = item.LowerPoint,
        //                    LowerUpTime = DateTime.Now
        //                });
        //            }
        //            else
        //            {
        //                temp.LowerPoint = item.LowerPoint;
        //                temp.LowerUpTime = DateTime.Now;
        //                agentManager.UpdateAgentReturnPoint(temp);
        //            }
        //        }
        //    }
        //}

        public void UpdateAgentInitialPoint(List<AgentReturnPointInfo> list, string agentId)
        {
            using (var agentManager = new AgentManager())
            {
                var agentReturnPoint = agentManager.GetAgentReturnPointListByUserId(agentId);
                var reality = new List<AgentReturnPoint>();
                foreach (var item in list)
                {
                    var temp = agentReturnPoint.FirstOrDefault(o => o.GameCode == item.GameCode && (item.GameType == string.Empty || o.GameType == item.GameType));
                    if (temp == null)
                    {
                        agentManager.AddAgentReturnInitialPoint(new AgentReturnPoint
                        {
                            AgentIdFrom = agentId,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            SetLevel = -1,
                            AgentIdTo = "",
                            ReturnPoint = item.ReturnPoint

                        });
                        //agentManager.AddAgentReturnPoint(new AgentReturnPointInfo
                        //{
                        //    AgentIdFrom =
                        //    UserId = agentId,
                        //    GameCode = item.GameCode,
                        //    GameType = item.GameType,
                        //    LowerPoint = item.LowerPoint,
                        //    LowerUpTime = DateTime.Now
                        //});
                    }
                    else
                    {
                        temp.ReturnPoint = item.ReturnPoint == null ? temp.ReturnPoint : item.ReturnPoint;
                        //temp.LowerUpTime = DateTime.Now;
                        agentManager.UpdateAgentReturnInitialPoint(temp);
                    }
                }
            }
        }
        /// <summary>
        /// 查询新用户返点
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IList<AgentReturnPointInfo> GetAgentReturnPointListByAgentId(string agentId)
        {
            var manager = new AgentManager();

            var list = manager.GetAgentReturnPointListByUserId(agentId);


            IList<AgentReturnPointInfo> collection = new List<AgentReturnPointInfo>();

            ObjectConvert.ConvertEntityListToInfoList<IList<AgentReturnPoint>, AgentReturnPoint
                , IList<AgentReturnPointInfo>, AgentReturnPointInfo>(list, ref collection, () => new AgentReturnPointInfo());
            return collection;
            // return list;
        }
        public string CheckIsUserConcern(string userId, string agentId)
        {
            var agentManager = new AgentManager();
            return agentManager.CheckIsUserConcern(userId, agentId);
        }

        public void AToBCommon(string goUserId, string toUserId, decimal toMoney, string password)
        {
            //var orderId = BusinessHelper.GetTransferId();
            //// 资金转出
            //BusinessHelper.Payout_2End(BusinessHelper.FundCategory_TransferFrom, orderId, orderId, true, "Transfer", password, goUserId, AccountType.Common, toMoney
            //    , string.Format("资金转出，赠送{0}，转出金额：￥{1:N2}", toUserId, toMoney));

            //// 资金转入
            //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_TransferTo, orderId, orderId, false, "", "", toUserId, AccountType.Common, toMoney
            //    , string.Format("资金转入，从{0}得到赠送，转入金额：￥{1:N2}", goUserId, toMoney));
        }

        /// <summary>
        /// 查询销量排行
        /// </summary>
        public AgentCommDetailByTotalSaleCollection GettCommDetailByTotalSale(DateTime fromDate, DateTime toDate)
        {
            var agentManager = new AgentManager();
            return agentManager.GettCommDetailByTotalSale(fromDate, toDate);
        }

        public List<CoreConfigInfo> QueryCoreConfigByKey()
        {

            using (var agentManager = new UserIntegralManager())
            {
                return agentManager.QueryAllCoreConfig();
            }
        }

        public UserQueryInfoCollection QueryAgentUserManagerList(DateTime createFrom, DateTime createTo, string keyType, string keyValue, int pageIndex, int pageSize)
        {
            using (var manager = new AgentManager())
            {
                int totalCount;
                var list = manager.QueryAgentUserManagerList(createFrom, createTo, keyType, keyValue, pageIndex, pageSize, out totalCount);
                var collection = new UserQueryInfoCollection
                {
                    TotalCount = totalCount
                };
                collection.LoadList(list);
                return collection;
            }
        }
    }
}
