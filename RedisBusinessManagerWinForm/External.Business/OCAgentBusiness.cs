using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using Common.Communication;
using External.Domain.Managers.Agent;
using External.Domain.Entities.Agent;
using GameBiz.Business;
using GameBiz.Domain.Entities;
using GameBiz.Auth.Business;
using External.Core.Agnet;
using External.Domain.Managers.Login;
using GameBiz.Auth.Domain.Managers;
using Common;

namespace External.Business
{
    /// <summary>
    /// 代理相关业务
    /// </summary>
    public class OCAgentBusiness : IPayBackTicket, IAgentPayIn
    {
        #region 实现接口方法
        /// <summary>
        /// 返点接口(未派奖订单)
        /// </summary>
        public void AgentPayIn(string schemeId)
        {
            string currentUserId = string.Empty;
            decimal currentBetMoney = 0M;
            string currentGameCode = string.Empty;
            bool currentIsAgent = false;

            //查询订单信息
            var sportsManager = new Sports_Manager();
            var manager = new SchemeManager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));
            if (order.IsPayRebate)
                throw new LogicException(string.Format("订单{0}已执行返点", schemeId));
            if (order.TicketStatus != TicketStatus.Ticketed)
                throw new LogicException("订单未出票完成，不能返点");

            var orderDetail = sportsManager.QueryOrderDetailBySchemeId(schemeId);
            if (orderDetail == null)
                throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));

            string msg = string.Empty;
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                if (order.IsVirtualOrder)
                {
                    //虚订单，只修改状态
                    order.IsPayRebate = true;
                    sportsManager.UpdateSports_Order_Running(order);
                }
                else
                {
                    //真实订单，处理返点数据
                    var gameCode = order.GameCode.ToUpper();
                    var gameType = order.GameType.ToUpper();
                    var userId = order.UserId;

                    
                    //合买判断
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        var main = sportsManager.QuerySports_Together(schemeId);
                        if (main == null)
                        {
                            msg = string.Format("找不到合买订单:{0}", schemeId);
                            //throw new Exception(string.Format("找不到合买订单:{0}", schemeId));
                        }
                        //if (main.ProgressStatus != TogetherSchemeProgress.Finish)
                        //    throw new Exception(string.Format("合买订单:{0} 状态不正确", schemeId));
                        var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                        if (sysJoinEntity != null && sysJoinEntity.RealBuyCount > 0)
                        {
                            msg = "网站参与保底，不返点";
                            //throw new Exception("网站参与保底，不返点");
                        }

                        if (main.SoldCount + main.Guarantees < main.TotalCount)
                            throw new Exception("订单未满员，不执行返点");
                        //realMoney -= main.SystemGuarantees * main.Price;
                    }

                    var realMoney = 0M; ;
                    var totalPayRebateMoney = 0M;
                    var agentManager = new OCAgentManager();
                    if (string.IsNullOrEmpty(msg))
                    {
                        //没有异常，执行返点
                        var noGameTypeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                        if (noGameTypeArray.Contains(gameCode))
                            gameType = string.Empty;

                        //真实投注金额，订单成功金额
                        realMoney = order.TotalMoney;
                        //查询用户自身返点
                        var balanceManager = new UserBalanceManager();
                        var user = balanceManager.QueryUserRegister(userId);
                        currentIsAgent = user.IsAgent;

                        //去掉红包参与金额
                        var redBagJoinMoney = order.RedBagMoney;// new FundManager().QuerySchemeRedBagTotalJoinMoney(schemeId);
                        //var redBagJoinMoney = new FundManager().QuerySchemeRedBagTotalJoinMoney(schemeId);
                        realMoney -= redBagJoinMoney;
                        //递归调用
                        int rebateType = 0;
                        var arrGameCode = new string[] { "JCZQ", "JCLQ", "BJDC" };
                        if (!string.IsNullOrEmpty(order.PlayType) && arrGameCode.Contains(order.GameCode))
                        {
                            if (order.PlayType == "1_1")
                                rebateType = 1;
                        }
                        totalPayRebateMoney = PayOrderRebate(agentManager, user, schemeId, userId, order.SchemeType, gameCode, gameType, order.TotalMoney, realMoney, 0, rebateType);
                    }

                    order.IsPayRebate = true;
                    //order.RealPayRebateMoney = realMoney;
                    var agentRebate = agentManager.QueryOCAgentDefaultRebate(userId, gameCode, gameType, CPSMode.PayRebate);
                    if (agentRebate != null)
                    {
                        //自身有返点
                        var payMoney = realMoney * agentRebate.Rebate / 100;
                        if (payMoney > 0)
                        {
                            order.RealPayRebateMoney = payMoney;
                            orderDetail.RealPayRebateMoney = payMoney;
                        }
                    }
                    order.TotalPayRebateMoney = totalPayRebateMoney;
                    order.TicketLog += msg;
                    sportsManager.UpdateSports_Order_Running(order);

                    orderDetail.TotalPayRebateMoney = totalPayRebateMoney;
                    manager.UpdateOrderDetail(orderDetail);
                }

                biz.CommitTran();
                currentUserId = order.UserId;
                currentBetMoney = order.SuccessMoney;
                currentGameCode = order.GameCode;
            }
            //计算代理销量
            CalculationAgentSales(currentUserId, currentGameCode, currentBetMoney, currentIsAgent, 0);
        }
        /// <summary>
        /// 完成出票
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var agentManager = new OCAgentManager();
                var existCount = agentManager.QueryOCAgentPayDetailCount(schemeId);
                if (existCount > 0)
                    throw new LogicException(string.Format("订单{0}已返利", schemeId));

                //查询订单信息
                var sportsManager = new Sports_Manager();
                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));

                if (!string.IsNullOrEmpty(order.ExtensionOne))
                {
                    var activityType = order.ExtensionOne.Split('_');
                    if (activityType.Length != 2)
                        throw new LogicException("选择活动拆分数据错误！");

                    if (activityType[0] != "3X1")
                        throw new LogicException("选择活动错误！");

                    int activ = int.Parse(activityType[1]);
                    if (ActivityType.AddAward == (ActivityType)activ || ActivityType.Rebate == (ActivityType)activ)
                        throw new LogicException("该订单参与了加奖或者参与了返利!");
                }
                if (order.SchemeType == SchemeType.TogetherBetting)
                {
                    var main = sportsManager.QuerySports_Together(schemeId);
                    if (main.SystemGuarantees <= 0 && !bool.Parse(new CacheDataBusiness().QueryCoreConfigByKey("IsTogetherFandian").ConfigValue))
                        throw new LogicException("合买不返利");
                }

                var gameCode = order.GameCode;
                var gameType = order.GameType;
                var noGameTypeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                if (noGameTypeArray.Contains(gameCode))
                    gameType = string.Empty;

                //查询用户自身返点
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);

                //递归调用 (20150613 dj 返点已移植到代理平台)
                //PayOrderRebate(agentManager, user, schemeId, userId, order.SchemeType, gameCode, gameType, totalMoney, totalMoney - totalErrorMoney, 0);

                biz.CommitTran();
            }
        }

        //计算用户自身返点
        public decimal PayOrderRebate(OCAgentManager manager, UserRegister currentUser, string schemeId, string orderUserId, SchemeType schemeType,
            string gameCode, string gameType, decimal totalOrderMoney, decimal realMoney, decimal payedRebate, int rebateType)
        {
            var totalPayRebateMoney = 0M;
            if (realMoney <= 0M)
                return totalPayRebateMoney;
            if (currentUser == null)
                return totalPayRebateMoney;
            if (currentUser.UserId == currentUser.AgentId)
                return totalPayRebateMoney;

            //var agentRebate = manager.QueryOCAgentDefaultRebate(currentUser.UserId, gameCode, gameType);
            var agentRebate = manager.QueryOCAgentDefaultRebateByRebateType(currentUser.UserId, gameCode, gameType, rebateType);
            if (agentRebate != null)
            {
                if (agentRebate.CPSMode != CPSMode.PayRebate)
                    return totalPayRebateMoney;
                //自身有返点
                var currentPayRebate = (agentRebate.Rebate - payedRebate);
                var payMoney = realMoney * currentPayRebate / 100;
                var remark = string.Format("订单{0}出票成功,返利{1:N2}%，共{2:N2}元。", schemeId, currentPayRebate, payMoney);
                payedRebate = agentRebate.Rebate;
                totalPayRebateMoney += payMoney;
                if (payMoney <= 0)
                {
                    if (string.IsNullOrEmpty(currentUser.AgentId))
                        return totalPayRebateMoney;
                    var balance = new UserBalanceManager();
                    var p = balance.QueryUserRegister(currentUser.AgentId);
                    return totalPayRebateMoney + PayOrderRebate(manager, p, schemeId, orderUserId, schemeType, gameCode, gameType, totalOrderMoney, realMoney, payedRebate, rebateType);
                }
                //添加佣金
                //BusinessHelper.Payin_To_Balance(AccountType.CPS, BusinessHelper.FundCategory_SchemeDeduct, currentUser.UserId,
                //    schemeId, payMoney, remark);
                BusinessHelper.Payin_To_Balance(AccountType.Commission, BusinessHelper.FundCategory_SchemeDeduct, currentUser.UserId,
                    schemeId, payMoney, remark);

                manager.AddOCAgentPayDetail(new OCAgentPayDetail
                {
                    CreateTime = DateTime.Now,
                    GameCode = gameCode,
                    GameType = gameType,
                    PayMoney = payMoney,
                    OrderTotalMoney = totalOrderMoney,
                    Rebate = currentPayRebate,
                    SchemeId = schemeId,
                    SchemeType = schemeType,
                    CPSMode = CPSMode.PayRebate,
                    PayInUserId = currentUser.UserId,
                    OrderUser = orderUserId,
                    Remark = remark,
                });

                //刷新余额
                BusinessHelper.RefreshRedisUserBalance(currentUser.UserId);
            }
            //查询上级
            if (string.IsNullOrEmpty(currentUser.AgentId))
                return totalPayRebateMoney;

            var balanceManager = new UserBalanceManager();
            var parent = balanceManager.QueryUserRegister(currentUser.AgentId);

            //递归
            return totalPayRebateMoney + PayOrderRebate(manager, parent, schemeId, orderUserId, schemeType, gameCode, gameType, totalOrderMoney, realMoney, payedRebate, rebateType);
        }

        //退还用户返点
        private void PayBackRebate(OCAgentManager manager, UserRegister currentUser, string schemeId, string orderUserId, SchemeType schemeType, string gameCode, string gameType, decimal totalOrderMoney, decimal payBackMoney, decimal payedRebate)
        {
            if (currentUser == null)
                return;

            var agentRebate = manager.QueryOCAgentDefaultRebate(currentUser.UserId, gameCode, gameType, CPSMode.PayRebate);
            if (agentRebate != null)
            {
                //自身有返点
                var currentPayRebate = (agentRebate.Rebate - payedRebate);
                var payMoney = payBackMoney * currentPayRebate / 100;
                var remark = string.Format("订单{0}退票{1:N2}元,返利{2:N2}%，共{3:N2}元。", schemeId, payBackMoney, currentPayRebate, payMoney);
                payedRebate = agentRebate.Rebate;

                try
                {
                    //退还佣金
                    BusinessHelper.Payout_To_End(AccountType.CPS, BusinessHelper.FundCategory_TicketFailed, currentUser.UserId,
                        schemeId, payMoney, remark);
                }
                catch (Exception ex)
                {
                    var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    writer.Write("EXEC_Plugin_PayBackRebate_Error_", "退还佣金", ex);
                }

                manager.AddOCAgentPayDetail(new OCAgentPayDetail
                {
                    CreateTime = DateTime.Now,
                    GameCode = gameCode,
                    GameType = gameType,
                    PayMoney = payMoney,
                    CPSMode = GameBiz.Core.CPSMode.PayRebate,
                    OrderTotalMoney = totalOrderMoney,
                    OrderUser = orderUserId,
                    Rebate = currentPayRebate,
                    SchemeId = schemeId,
                    SchemeType = schemeType,
                    PayInUserId = currentUser.UserId,
                    Remark = remark,
                });
            }
            //查询上级
            if (string.IsNullOrEmpty(currentUser.AgentId))
                return;

            var balanceManager = new UserBalanceManager();
            var parent = balanceManager.QueryUserRegister(currentUser.AgentId);

            //递归
            PayBackRebate(manager, parent, schemeId, orderUserId, schemeType, gameCode, gameType, totalOrderMoney, payBackMoney, payedRebate);
        }

        /// <summary>
        /// 退票
        /// </summary>
        public void PayBack(string schemeId, string ticketId, decimal ticketMoney)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var ticket = sportsManager.QueryTicket(ticketId);
                if (ticket == null)
                    throw new LogicException(string.Format("找不到票号:{0}", ticketId));

                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));

                var gameCode = order.GameCode;
                var gameType = order.GameType;
                var noGameTypeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                if (noGameTypeArray.Contains(gameType))
                    gameType = string.Empty;

                var agentManager = new OCAgentManager();
                //查询用户自身返点
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(order.UserId);
                //递归调用
                PayBackRebate(agentManager, user, schemeId, order.UserId, order.SchemeType, gameCode, gameType, order.TotalMoney, ticketMoney, 0);

                biz.CommitTran();
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IPayBackTicket":
                        PayBack((string)paraList[0], (string)paraList[1], (decimal)paraList[2]);
                        break;
                    //case "IComplateTicket":
                    //    ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
                    //    break;
                    case "IAgentPayIn":
                        AgentPayIn((string)paraList[0]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_OCAgentBusiness_Error_", type, ex);
            }
            return null;
        }

        #endregion

        /// <summary>
        /// 对已派奖订单返点
        /// </summary>
        public void AgentPayIn_CompateOrder(string schemeId)
        {
            string currentUserId = string.Empty;
            decimal currentBetMoney = 0M;
            string currentGameCode = string.Empty;
            bool currentIsAgent = false;

            //查询订单信息
            var sportsManager = new Sports_Manager();
            var manager = new SchemeManager();
            var order = sportsManager.QuerySports_Order_Complate(schemeId);
            if (order == null)
                throw new LogicException(string.Format("找不到已派奖订单 ：{0} ", schemeId));
            if (order.IsPayRebate)
                throw new LogicException(string.Format("订单{0}已执行返点", schemeId));
            if (order.TicketStatus != TicketStatus.Ticketed)
                throw new LogicException("订单未出票完成，不能返点");

            var orderDetail = sportsManager.QueryOrderDetailBySchemeId(schemeId);
            if (orderDetail == null)
                throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));

            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                if (order.IsVirtualOrder)
                {
                    //虚订单，只修改状态
                    order.IsPayRebate = true;
                    sportsManager.UpdateSports_Order_Complate(order);
                }
                else
                {
                    //真实订单，处理返点数据
                    var gameCode = order.GameCode.ToUpper();
                    var gameType = order.GameType.ToUpper();
                    var userId = order.UserId;

                    var msg = string.Empty;
                    //合买判断
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        var main = sportsManager.QuerySports_Together(schemeId);
                        if (main == null)
                        {
                            msg = string.Format("找不到合买订单:{0}", schemeId);
                            //throw new Exception(string.Format("找不到合买订单:{0}", schemeId));
                        }
                        //if (main.ProgressStatus != TogetherSchemeProgress.Finish)
                        //    throw new Exception(string.Format("合买订单:{0} 状态不正确", schemeId));
                        var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                        if (sysJoinEntity != null && sysJoinEntity.RealBuyCount > 0)
                        {
                            msg = "网站参与保底，不返点";
                            //throw new Exception("网站参与保底，不返点");
                        }

                        if (main.SoldCount + main.Guarantees < main.TotalCount)
                            throw new Exception("订单未满员，不执行返点");
                        //realMoney -= main.SystemGuarantees * main.Price;
                    }

                    var realMoney = 0M; ;
                    var totalPayRebateMoney = 0M;
                    var agentManager = new OCAgentManager();
                    if (string.IsNullOrEmpty(msg))
                    {
                        //没有异常，执行返点
                        var noGameTypeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                        if (noGameTypeArray.Contains(gameCode))
                            gameType = string.Empty;

                        //真实投注金额，订单成功金额
                        realMoney = order.TotalMoney;
                        //查询用户自身返点
                        var balanceManager = new UserBalanceManager();
                        var user = balanceManager.QueryUserRegister(userId);
                        currentIsAgent = user.IsAgent;

                        //去掉红包参与金额
                        var redBagJoinMoney = order.RedBagMoney;// new FundManager().QuerySchemeRedBagTotalJoinMoney(schemeId);
                        //var redBagJoinMoney = new FundManager().QuerySchemeRedBagTotalJoinMoney(schemeId);
                        realMoney -= redBagJoinMoney;
                        //递归调用
                        int rebateType = 0;
                        var arrGameCode = new string[] { "JCZQ", "JCLQ", "BJDC" };
                        if (!string.IsNullOrEmpty(order.PlayType) && arrGameCode.Contains(order.GameCode))
                        {
                            if (order.PlayType == "1_1")
                                rebateType = 1;
                        }
                        totalPayRebateMoney = PayOrderRebate(agentManager, user, schemeId, userId, order.SchemeType, gameCode, gameType, order.TotalMoney, realMoney, 0, rebateType);
                    }

                    order.IsPayRebate = true;
                    //order.RealPayRebateMoney = realMoney;
                    var agentRebate = agentManager.QueryOCAgentDefaultRebate(userId, gameCode, gameType, CPSMode.PayRebate);
                    if (agentRebate != null)
                    {
                        //自身有返点
                        var payMoney = realMoney * agentRebate.Rebate / 100;
                        if (payMoney > 0)
                        {
                            order.RealPayRebateMoney = payMoney;
                            orderDetail.RealPayRebateMoney = payMoney;
                        }
                    }
                    order.TotalPayRebateMoney = totalPayRebateMoney;
                    order.TicketLog += msg;
                    sportsManager.UpdateSports_Order_Complate(order);

                    orderDetail.TotalPayRebateMoney = totalPayRebateMoney;
                    manager.UpdateOrderDetail(orderDetail);
                }

                biz.CommitTran();
                currentUserId = order.UserId;
                currentBetMoney = order.SuccessMoney;
                currentGameCode = order.GameCode;
            }
            //计算代理销量
            CalculationAgentSales(currentUserId, currentGameCode, currentBetMoney, currentIsAgent, 0);
        }

        /// <summary>
        /// 查询等待返点的订单，并执行返点
        /// 包括普通订单 和 合买订单
        /// </summary>
        public string QueryWaitPayRebate()
        {
            var log = new List<string>();
            var successCount = 0;
            var failCount = 0;
            var manager = new Sports_Manager();
            foreach (var schemeId in manager.QueryWaitPayRebateRunningOrder())
            {
                try
                {
                    AgentPayIn(schemeId);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    log.Add(string.Format("订单{0} 执行返点异常：{1}", schemeId, ex.Message));
                }
            }
            log.Insert(0, string.Format("成功执行返点：{0}条，失败执行返点：{1}条", successCount, failCount));
            return string.Join(Environment.NewLine, log.ToArray());
        }

        /// <summary>
        /// 执行订单返点，已派奖订单
        /// </summary>
        public string DoPayRebate_ComplateOrder()
        {
            var log = new List<string>();
            var successCount = 0;
            var failCount = 0;
            var manager = new Sports_Manager();
            foreach (var schemeId in manager.QueryWaitPayRebateComplateOrder())
            {
                try
                {
                    AgentPayIn_CompateOrder(schemeId);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    log.Add(string.Format("订单{0} 执行返点异常：{1}", schemeId, ex.Message));
                }
            }
            log.Insert(0, string.Format("成功执行返点：{0}条，失败执行返点：{1}条", successCount, failCount));
            return string.Join(Environment.NewLine, log.ToArray());
        }

        /// <summary>
        /// 执行订单返点
        /// </summary>
        public void DoPayRebate(string schemeId)
        {
            AgentPayIn(schemeId);
        }

        /// <summary>
        /// 添加代理
        /// </summary>
        public void AddOCAgent(OCAgentCategory category, string parentUserId, string userId, CPSMode cpsmode)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                if (parentUserId == userId)
                    throw new Exception("不能将自己添加为自己的上级代理");
                var balanceManager = new UserBalanceManager();
                var parent = balanceManager.QueryUserRegister(parentUserId);

                if (category != OCAgentCategory.Company && parent == null && category != OCAgentCategory.Company && !string.IsNullOrEmpty(parentUserId))
                    throw new LogicException(string.Format("上级用户{0}不存在", parentUserId));

                var user = balanceManager.QueryUserRegister(userId);
                if (user == null)
                    throw new LogicException(string.Format("用户{0}不存在", userId));

                if (category != OCAgentCategory.Company)
                {
                    if (string.IsNullOrEmpty(user.AgentId))
                        throw new Exception("用户不属于您的下级用户");
                    if (user.AgentId != parentUserId)
                        throw new Exception("该用户非您发展的用户");
                }

                var agentManager = new OCAgentManager();
                var userAgent = agentManager.QueryOCAgent(userId);
                if (userAgent != null)
                    throw new Exception(string.Format("用户{0}已经是代理", userId));
                var parentAgent = agentManager.QueryOCAgent(parentUserId);
                if (category != OCAgentCategory.Company)
                {
                    if (parentAgent == null)
                        throw new Exception("上级用户不是代理");
                    if (category != OCAgentCategory.SportLotteryAgent)
                    {
                        switch (parentAgent.OCAgentCategory)
                        {
                            case OCAgentCategory.Company:
                                if (category == OCAgentCategory.Company)
                                    throw new LogicException("添加代理类型不正确，不能为公司类型");
                                break;
                            case OCAgentCategory.Market:
                                if (category == OCAgentCategory.Company)
                                    throw new LogicException("添加代理类型不正确，不能为公司类型");
                                break;
                            case OCAgentCategory.GeneralAgent:
                                if (category != OCAgentCategory.GeneralAgent)
                                    throw new LogicException("添加代理类型不正确，只能添加普通代理");
                                break;
                            default:
                                break;
                        }
                    }
                }
                user.AgentId = parentUserId;
                user.IsAgent = true;
                user.ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath;
                balanceManager.UpdateUserRegister(user);

                var authBiz = new GameBizAuthBusiness();
                var userManager = new UserManager();
                var strRole = userManager.QueryUserRoleIdsByUserId(userId);
                if (!string.IsNullOrEmpty(strRole))
                {
                    var array_Role = strRole.Split(new string[] { "%item%" }, StringSplitOptions.RemoveEmptyEntries);
                    if (array_Role != null && array_Role.Length > 0)
                    {
                        if (!array_Role.Contains("Agent"))
                            authBiz.AddUserRoles(userId, new string[] { "Agent" });
                    }
                    else
                        authBiz.AddUserRoles(userId, new string[] { "Agent" });
                }
                else
                    authBiz.AddUserRoles(userId, new string[] { "Agent" });
                //保存代理上下级关系
                agentManager.AddOCAgent(new OCAgent
                {
                    CreateTime = DateTime.Now,
                    OCAgentCategory = category,
                    ParentUserId = parentUserId,
                    UserId = userId,
                    CustomerDomain = string.Empty,
                    ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath,
                    CPSMode = category == OCAgentCategory.Company ? cpsmode : parentAgent.CPSMode,
                });
                //返点配置
                var rebateList = agentManager.QueryOCAgentRebateList(parentUserId);
                foreach (var item in rebateList)
                {
                    if (item.SubUserRebate < 0M)
                        throw new Exception("添加下级代理前，请先设置上级代理的下级默认返点");


                    var currAgent = agentManager.QueryOCAgentDefaultRebateByRebateType(userId, item.GameCode, item.GameType, item.RebateType);
                    if (currAgent == null)
                    {
                        agentManager.AddOCAgentRebate(new OCAgentRebate
                        {
                            CreateTime = DateTime.Now,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            UserId = userId,
                            Rebate = item.SubUserRebate,
                            SubUserRebate = 0M,
                            RebateType = item.RebateType,
                            CPSMode = category == OCAgentCategory.Company ? cpsmode : parentAgent.CPSMode,
                        });
                    }
                    //else
                    //{
                    //    currAgent.Rebate = item.SubUserRebate;
                    //    currAgent.SubUserRebate = 0M;
                    //    currAgent.RebateType = item.RebateType;
                    //    agentManager.UpdateOCAgentRebate(currAgent);
                    //}


                    //foreach (var itemAgent in currAgent)
                    //{

                    //    if (currAgent != null && !string.IsNullOrEmpty(itemAgent.UserId))
                    //    {
                    //        if (itemAgent.Rebate == 0)
                    //        {
                    //            agentManager.UpdateOCAgentRebate(itemAgent);
                    //        }
                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                }

                biz.CommitTran();
            }
        }


        /// <summary>
        /// 添加代理
        /// </summary>
        public void AddOCAgent_New_CPS(OCAgentCategory category, string parentUserId, string userId, CPSMode cpsmode, string channelName)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                if (parentUserId == userId)
                    throw new Exception("不能将自己添加为自己的上级代理");
                var balanceManager = new UserBalanceManager();
                var parent = balanceManager.QueryUserRegister(parentUserId);
                //if (category != OCAgentCategory.Company && parent == null && category != OCAgentCategory.Company && !string.IsNullOrEmpty(parentUserId))
                //    throw new LogicException(string.Format("上级用户{0}不存在", parentUserId));

                var user = balanceManager.QueryUserRegister(userId);
                if (user == null)
                    throw new LogicException(string.Format("用户{0}不存在", userId));

                if (string.IsNullOrEmpty(parentUserId) && !string.IsNullOrEmpty(user.AgentId))
                    throw new LogicException(string.Format("用户{0}已经是{1}的下级用户", userId, user.AgentId));

                if (!string.IsNullOrEmpty(parentUserId))
                {
                    if (user.AgentId != parentUserId)
                        throw new Exception("该用户非您发展的用户");
                }

                //if (category != OCAgentCategory.Company && !string.IsNullOrEmpty(parentUserId))
                //{
                //    if (string.IsNullOrEmpty(user.AgentId))
                //        throw new Exception("用户不属于您的下级用户");
                //    if (user.AgentId != parentUserId)
                //        throw new Exception("该用户非您发展的用户");
                //}

                var agentManager = new OCAgentManager();
                var userAgent = agentManager.QueryOCAgent(userId);
                if (userAgent != null)
                    throw new Exception(string.Format("用户{0}已经是代理或者推广员", userId));
                var parentAgent = agentManager.QueryOCAgent(parentUserId);

                if (!string.IsNullOrEmpty(parentUserId))
                {
                    if (parentAgent == null)
                        throw new LogicException("上级用户不是代理");
                    if (parentAgent.CPSMode != cpsmode)
                        throw new LogicException("上级返点模式跟添加用户选择不一致");
                    switch (parentAgent.OCAgentCategory)
                    {
                        case OCAgentCategory.Company:
                            if (category == OCAgentCategory.Company)
                                throw new LogicException("添加代理类型不正确，不能为总代理");
                            break;
                        case OCAgentCategory.GeneralAgent:
                            if (category != OCAgentCategory.Extension)
                                throw new LogicException("添加代理类型不正确，只能添加推广员");
                            break;
                        case OCAgentCategory.Extension:
                            if (category == OCAgentCategory.Extension)
                                throw new LogicException("添加代理类型不正确，上级已经是推广员");
                            break;
                        default:
                            break;
                    }
                }
                user.AgentId = parentUserId;
                user.IsAgent = true;
                user.ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath;
                balanceManager.UpdateUserRegister(user);

                //保存代理上下级关系
                agentManager.AddOCAgent(new OCAgent
                {
                    CreateTime = DateTime.Now,
                    OCAgentCategory = category,
                    ParentUserId = parentUserId,
                    UserId = userId,
                    CustomerDomain = string.Empty,
                    ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath,
                    CPSMode = cpsmode,
                    ChannelName = channelName
                });
                //返点配置
                var rebateList = agentManager.QueryOCAgentRebateList(parentUserId);
                foreach (var item in rebateList)
                {
                    if (item.SubUserRebate < 0M)
                        throw new Exception("添加下级代理前，请先设置上级代理的下级默认返点");

                    agentManager.AddOCAgentRebate(new OCAgentRebate
                    {
                        CreateTime = DateTime.Now,
                        GameCode = item.GameCode,
                        GameType = item.GameType,
                        UserId = userId,
                        Rebate = item.SubUserRebate,
                        SubUserRebate = 0M,
                        RebateType = item.RebateType,
                        CPSMode = cpsmode
                    });
                }

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 店面代理信息
        /// </summary>
        public StoreMessageInfoCollection QueryStoreMessageByusreId(string userId)
        {
            var list = new StoreMessageInfoCollection();
            var agentManager = new OCAgentManager();
            list.AddRange(agentManager.QueryStoreMessageByuserId(userId));
            return list;
        }
        /// <summary>
        /// 查询代理类型
        /// </summary>
        public OCAgent QueryAgentType(string userid)
        {
            var agentManager = new OCAgentManager();
            var userAgent = agentManager.QueryOCAgent(userid);
            return userAgent;
        }
        /// <summary>
        /// 根据店面编号查询店主id
        /// </summary>
        public string QueryUserIdBystoreId(string storeid)
        {
            var agentManager = new OCAgentManager();
            var userAgent = agentManager.QueryUserIdBystoreId(storeid);
            return userAgent.UserId;
        }

        /// <summary>
        /// 代理自定义推广域名
        /// </summary>
        public void SetOCAgentCustomerDomain(string userId, string domain)
        {
            #region 验证自定义域名规范

            if (domain.Length < 3 || domain.Length > 16)
                throw new Exception("链接长度只允许在3-16位之间");

            var allowList = new List<int>();
            for (int i = 97; i <= 122; i++)
            {
                allowList.Add(i);
            }
            for (int i = 48; i <= 57; i++)
            {
                allowList.Add(i);
            }
            //是否在允许范围内
            foreach (var item in domain)
            {
                if (!allowList.Contains((int)item))
                    throw new Exception(string.Format("自定义链接中包含不允许的字符:{0}", item));
            }
            //是否为纯数字
            allowList = new List<int>();
            for (int i = 97; i <= 122; i++)
            {
                allowList.Add(i);
            }
            var containLetter = false;
            foreach (var item in domain)
            {
                if (allowList.Contains((int)item))
                {
                    containLetter = true;
                    break;
                }
            }
            if (!containLetter)
                throw new Exception("自定义链接不支持纯数字");

            #endregion

            var manager = new OCAgentManager();
            var agent = manager.QueryOCAgent(userId);
            if (agent == null)
                throw new Exception(string.Format("用户:{0}不是代理。", userId));
            if (!string.IsNullOrEmpty(agent.CustomerDomain))
                throw new Exception("代理已设置过自定义链接:" + agent.CustomerDomain);

            var domainFormat = "http://{0}.vip.iqucai.com";
            domain = string.Format(domainFormat, domain);
            var existAgent = manager.QueryOCAgentByDomain(domain);
            if (existAgent != null)
                throw new Exception(string.Format("您设置的自定义链接 {0} 已被其它用户抢先使用，请更换其它链接", domain));

            agent.CustomerDomain = domain;
            manager.UpdateOCAgent(agent);
        }

        /// <summary>
        /// 根据用户查询自定义域名
        /// </summary>
        public string QueryCustmerDomainByUserId(string userId)
        {
            var manager = new OCAgentManager();
            var agent = manager.QueryOCAgent(userId);
            return agent == null ? string.Empty : agent.CustomerDomain;
        }

        /// <summary>
        /// 根据自定义域名查询用户编号
        /// </summary>
        public string QueryAgentUserIdByCustomerDomain(string domain)
        {
            var manager = new OCAgentManager();
            var agent = manager.QueryOCAgentByDomain(domain);
            return agent == null ? string.Empty : agent.UserId;
        }
        /// <summary>
        /// 查询店面编号
        /// </summary>
        public string QueryStoreIdByUrl(string domain)
        {
            var manager = new OCAgentManager();
            var agent = manager.QueryOCAgentByDomain(domain);
            return agent == null ? string.Empty : agent.StoreId;
        }

        /// <summary>
        /// 设置代理 的下级用户默认返点(设置新用户返点)
        /// setString 的每一项 由三部分组成  GameCode:GameType:Rebate|GameCode:GameType:Rebate
        /// </summary>
        public void SetOCAgentRebate(string userId, string setString)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var agentManager = new OCAgentManager();
                var rebateList = agentManager.QueryOCAgentRebateList(userId);

                var array = setString.Split('|');
                foreach (var item in array)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    var configArray = item.Split(':');
                    //if (configArray.Length != 3)
                    //    continue;

                    var gameCode = configArray[0];
                    var gameType = configArray[1];
                    var rebateValue = decimal.Parse(configArray[2]);
                    var rebateType = int.Parse(configArray[3]);

                    //是否开启最大返点控制
                    var isEnableSetMaxPayRabate = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableSetMaxPayRabate").ConfigValue);
                    if (isEnableSetMaxPayRabate)
                    {
                        //代理允许的最大返点
                        var AllowSetMaxPayRabate = Convert.ToDecimal(new CacheDataBusiness().QueryCoreConfigByKey("AllowSetMaxPayRabate").ConfigValue);
                        if (rebateValue > AllowSetMaxPayRabate)
                            throw new Exception("设置返点不能高于" + AllowSetMaxPayRabate + "%");
                    }
                    var maxRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("MaxReturnPoint").ConfigValue);
                    if (rebateValue > maxRebate)
                        throw new Exception(string.Format("设置的返点不能大于系统最大返点{0}", maxRebate));

                    var rebate = rebateList.FirstOrDefault(p => p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType);
                    if (rebate == null)
                    {
                        //添加
                        //todo:
                        //agentManager.AddOCAgentRebate(new OCAgentRebate {

                        //});
                    }
                    else
                    {
                        //修改 
                        //最小保留返点
                        var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
                        if ((rebate.Rebate - rebateValue) < keepRebate)
                            throw new Exception(string.Format("用户自身保留的返点不能低于{0}%", keepRebate));

                        rebate.SubUserRebate = rebateValue;
                        rebate.CreateTime = DateTime.Now;
                        agentManager.UpdateOCAgentRebate(rebate);
                    }


                }

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 修改代理或用户的返点（配置代理或用户返点）
        /// setString 的每一项 由三部分组成  GameCode:GameType:Rebate|GameCode:GameType:Rebate
        /// </summary>
        public void UpdateOCAgentRebate(string parentUserId, string userId, string setString)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                EditOCAgentRebate(parentUserId, userId, setString);

                biz.CommitTran();
            }
        }
        public void EditOCAgentRebate(string parentUserId, string userId, string setString)
        {
            //查询用户实名认证和手机认证
            //var mobile = new MobileAuthenticationBusiness().GetAuthenticatedMobile(userId);
            //if (mobile == null || !mobile.IsSettedMobile)
            //    throw new Exception("用户未手机认证，不能配置返点");
            //var realName = new RealNameAuthenticationBusiness().GetAuthenticatedRealName(userId);
            //if (realName == null || !realName.IsSettedRealName)
            //    throw new Exception("用户未实名认证，不能配置返点");


            var agentManager = new OCAgentManager();
            var parentRebateList = agentManager.QueryOCAgentRebateList(parentUserId);
            var userRebateList = agentManager.QueryOCAgentRebateList(userId);

            //var userAgent = agentManager.QueryOCAgent(userId);
            var array = setString.Split('|');
            foreach (var item in array)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                var configArray = item.Split(':');
                if (configArray.Length != 4)
                    continue;

                var gameCode = configArray[0];
                var gameType = configArray[1];
                var rebateValue = decimal.Parse(configArray[2]);
                var rebateType = int.Parse(configArray[3]);
                var IsEnableSetMaxPayRabate = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableSetMaxPayRabate").ConfigValue);
                var AllowSetMaxPayRabate = Convert.ToDecimal(new CacheDataBusiness().QueryCoreConfigByKey("AllowSetMaxPayRabate").ConfigValue);
                if (IsEnableSetMaxPayRabate && !string.IsNullOrEmpty(parentUserId))
                {
                    if (rebateValue > AllowSetMaxPayRabate)
                        throw new Exception("设置返点不能高于" + AllowSetMaxPayRabate + "%");
                }
                //if (rebateValue <= 0M)
                if (rebateValue < 0M || rebateValue > 100M)
                    throw new Exception("返点必须大于0小于100");

                var parent = parentRebateList.OrderByDescending(p => p.CreateTime).FirstOrDefault(p => p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType);
                if (parent == null)
                    continue;

                if (rebateValue > parent.Rebate && !string.IsNullOrEmpty(parentUserId))
                    throw new Exception("设置的返点不能大于上级返点");
                var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
                if ((parent.Rebate - rebateValue) < keepRebate && !string.IsNullOrEmpty(parentUserId))
                    throw new Exception(string.Format("用户自身保留的返点不能低于{0}%", keepRebate));

                //查询用户返点数据
                var rebate = userRebateList.OrderByDescending(p => p.CreateTime).FirstOrDefault(p => p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType);
                if (rebate == null)
                {
                    //user 为普通用户
                    //添加返点数据
                    agentManager.AddOCAgentRebate(new OCAgentRebate
                    {
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = gameType,
                        UserId = userId,
                        Rebate = rebateValue,
                        SubUserRebate = 0,
                        RebateType = rebateType,
                    });
                }
                else
                {
                    //user 为代理用户
                    //修改返点数据
                    //if (rebateValue < rebate.Rebate)
                    //    throw new Exception("不能降低用户的返点");
                    rebate.Rebate = rebateValue;
                    rebate.CreateTime = DateTime.Now;
                    agentManager.UpdateOCAgentRebate(rebate);
                }
                //更新代理返点的同时更新他的下级返点
                UpdateLowerAgentRebate(userId, gameCode, gameType, rebateValue, rebateType);
            }
        }

        /// <summary>
        /// 更新代理返点的同时更新他的下级返点
        /// </summary>
        public void UpdateLowerAgentRebate(string agentId, string gameCode, string gameType, decimal rebate, int rebateType)
        {
            var ocAgentMannger = new OCAgentManager();
            //查询全部下级代理
            var agentList = ocAgentMannger.QueryAgentSubUser(agentId);
            if (agentList == null || agentList.Count == 0)
                return;
            var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
            foreach (var agent in agentList)
            {
                var agenrRebate = ocAgentMannger.QueryOCAgentDefaultRebateByRebateType(agent.UserId, gameCode, gameType, rebateType);
                if (agenrRebate == null)
                    continue;
                if (rebate > agenrRebate.Rebate || (rebate == agenrRebate.Rebate && keepRebate == 0M))
                    continue;
                agenrRebate.Rebate = rebate - keepRebate;
                agenrRebate.CreateTime = DateTime.Now;
                ocAgentMannger.UpdateOCAgentRebate(agenrRebate);

                UpdateLowerAgentRebate(agent.UserId, gameCode, gameType, agenrRebate.Rebate, rebateType);
            }
        }

        /// <summary>
        /// 查询用户是否有返点
        /// </summary>
        public OCAgentRebateInfoCollection QueryUserRebateList(string userId)
        {
            var list = new OCAgentRebateInfoCollection();
            var agentManager = new OCAgentManager();
            list.AddRange(agentManager.QueryUserRebateList(userId));
            return list;
        }

        /// <summary>
        /// 查询代理用户返点列表
        /// </summary>
        public OCAgentRebateInfoCollection QueryUserRebate(string userId)
        {
            var list = new OCAgentRebateInfoCollection();
            var agentManager = new OCAgentManager();
            list.AddRange(agentManager.QueryOCAgentRebateInfoList(userId));
            return list;
        }

        /// <summary>
        /// 佣金记录
        /// </summary>
        public OCAgentPayDetailInfoCollection QueryOCAgentPayDetailList(DateTime fromDate, DateTime toDate, string userId, int pageIndex, int pageSize)
        {
            var list = new OCAgentPayDetailInfoCollection();
            var totalCount = 0;
            var agentManager = new OCAgentManager();
            list.DetailList.AddRange(agentManager.QueryOCAgentPayDetailList(fromDate, toDate, userId, pageIndex, pageSize, out totalCount));
            list.TotalCount = totalCount;
            return list;
        }

        /// <summary>
        /// 佣金记录 查询结算报表
        /// </summary>
        public AgentPayDetailReportInfoCollection QueryAgentPayDetailReportInfo(string userId, DateTime fromDate, DateTime toDate)
        {
            var list = new AgentPayDetailReportInfoCollection();
            var agentManager = new OCAgentManager();
            //agentManager.QueryAgentPayDetailReportInfo(userId, fromDate, toDate);
            list.List = agentManager.QueryAgentPayDetailReportInfo(userId, fromDate, toDate);
            return list;
        }

        public SubUserNoPayRebateOrderInfoCollection QuerySubUserCreateingOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var list = new SubUserNoPayRebateOrderInfoCollection();
            int totalCount = 0;
            int userCount = 0;
            decimal totalMoney = 0M;
            var agentManager = new OCAgentManager();
            if (!string.IsNullOrEmpty(key))
            {
                var manager = new UserBalanceManager();
                var user = manager.GetUserRegister(key);
                if (user == null || user.AgentId.Trim() != userId.Trim())
                {
                    list.TotalCount = 0;
                    list.UserCount = 0;
                    list.TotalMoney = 0;
                    return list;
                }
            }
            list.List.AddRange(agentManager.QuerySubUserCreateingOrderList(key, userId, starTime, endTime, pageIndex, pageSize, out totalCount, out  userCount, out   totalMoney));
            list.TotalCount = totalCount;
            list.UserCount = userCount;
            list.TotalMoney = totalMoney;
            return list;
        }
        public SubUserNoPayRebateOrderInfoCollection QuerySubUserNoPayRebateOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var list = new SubUserNoPayRebateOrderInfoCollection();
            int totalCount = 0;
            int userCount = 0;
            decimal totalMoney = 0M;
            var agentManager = new OCAgentManager();

            if (!string.IsNullOrEmpty(key))
            {
                var manager = new UserBalanceManager();
                var user = manager.GetUserRegister(key);
                if (user == null || user.AgentId.Trim() != userId.Trim())
                {
                    list.TotalCount = 0;
                    list.UserCount = 0;
                    list.TotalMoney = 0;
                    return list;
                }
            }
            list.List.AddRange(agentManager.QuerySubUserNoPayRebateOrderList(key, userId, starTime, endTime, pageIndex, pageSize, out totalCount, out  userCount, out   totalMoney));
            list.TotalCount = totalCount;
            list.UserCount = userCount;
            list.TotalMoney = totalMoney;
            return list;
        }

        public SubUserPayRebateOrderInfoCollection QuerySubUserPayRebateOrderList(string key, string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var list = new SubUserPayRebateOrderInfoCollection();
            int totalCount = 0;
            int totalUserCount = 0;
            decimal totalMoney = 0M;
            decimal totalRealPayRebateMoney = 0M;
            var agentManager = new OCAgentManager();
            list.List.AddRange(agentManager.QuerySubUserPayRebateOrderList(key, userId, starTime, endTime, pageIndex, pageSize,
                out totalCount, out  totalUserCount, out   totalMoney, out totalRealPayRebateMoney));
            list.TotalCount = totalCount;
            list.UserCount = totalUserCount;
            list.TotalMoney = totalMoney;
            list.TotalRealPayRebateMoney = totalRealPayRebateMoney;
            return list;
        }

        public OCAgentInfoCollection QuerySubAgentList(string userId)
        {
            var list = new OCAgentInfoCollection();


            return list;
        }
        /// <summary>
        /// 后台会员详情，更新经销商
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agentId"></param>
        public void UpdateUserAgentId(string userId, string agentId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var agentManager = new OCAgentManager();
                var manager = new UserBalanceManager();
                string oldAgentId = string.Empty;
                var reg = manager.GetUserRegister(userId);
                var ocAgent = agentManager.QueryOCAgent(agentId);
                if (reg == null)
                    throw new Exception("不存在此用户：" + userId);
                else if (ocAgent == null)
                    throw new Exception("上级用户不是代理商");
                var parentAgent = manager.GetUserRegister(agentId);

                oldAgentId = reg.AgentId;
                reg.AgentId = agentId;
                reg.ParentPath = parentAgent.ParentPath + "/" + agentId;
                manager.UpdateUserRegister(reg);

                if (ocAgent != null)
                {
                    var currAgent = agentManager.QueryOCAgent(userId);
                    if (currAgent != null)
                    {
                        currAgent.ParentUserId = agentId;
                        currAgent.ParentPath = ocAgent.ParentPath + "/" + agentId;
                        agentManager.UpdateOCAgent(currAgent);
                    }
                }

                var currUserRebateList = agentManager.QueryOCAgentRebateList(userId);
                if (currUserRebateList != null && currUserRebateList.Count > 0)
                {
                    var parentRebateList = agentManager.QueryOCAgentRebateList(agentId);
                    if (parentRebateList != null && parentRebateList.Count > 0)
                    {
                        var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
                        foreach (var item in currUserRebateList)
                        {
                            var rebate = parentRebateList.FirstOrDefault(s => s.UserId == agentId && s.GameCode == item.GameCode && s.GameType == item.GameType);
                            if (rebate != null)
                            {
                                var result = item.Rebate - (rebate.Rebate - keepRebate);//判断：当自身返点高于上级减去系统保留返点时，则用上级返点
                                if (result >= 0)
                                {
                                    item.Rebate = (rebate.Rebate >= 0 ? rebate.Rebate : 0);
                                    if (item.SubUserRebate > rebate.Rebate)
                                        item.SubUserRebate = rebate.Rebate;
                                    else
                                        item.SubUserRebate = rebate.SubUserRebate >= 0 ? rebate.SubUserRebate : 0;
                                    agentManager.UpdateOCAgentRebate(item);
                                    //修改当前用户下级相关联用户的返点(20150617 dj 修改)
                                    //UpdateRelationChildRebate(userId, item.Rebate, item.GameCode, item.GameType);
                                }
                            }
                        }

                        //修改当前用户下级相关联用户的返点
                        UpdateRelationChildRebate(userId, oldAgentId, agentId, reg.ParentPath);
                    }
                }

                biz.CommitTran();
            }
        }
        public void UpdateRelationChildRebate(string currUserId, string oldAgentId, string newAgentId, string path)
        {
            var manager = new OCAgentManager();
            var agentManager = new OCAgentManager();
            var parentUser = agentManager.QueryOCAgentRebateList(currUserId);
            var xjUser = agentManager.QueryLowerAgentListByParentId(currUserId);
            List<string> strUserIds = new List<string>();
            foreach (var item in xjUser)
            {
                var currRebate = parentUser.FirstOrDefault(s => s.GameCode == item.GameCode && (item.GameType == string.Empty || s.GameType == item.GameType));
                if (currRebate != null)
                {
                    if (item.Rebate > currRebate.Rebate)
                    {
                        item.Rebate = currRebate.Rebate;
                        if (item.SubUserRebate > currRebate.Rebate)
                            item.SubUserRebate = currRebate.Rebate;
                        else
                            item.SubUserRebate = item.SubUserRebate >= 0 ? item.SubUserRebate : 0;
                        agentManager.UpdateOCAgentRebate(item);
                    }
                }
                if (!strUserIds.Contains(item.UserId))
                {
                    strUserIds.Add(item.UserId);
                    UserBalanceManager userManager = new UserBalanceManager();
                    var userRegist = userManager.GetUserRegister(item.UserId);
                    if (userRegist != null)
                    {

                        if (!string.IsNullOrEmpty(userRegist.ParentPath))
                        {
                            //userRegist.ParentPath = userRegist.ParentPath.Replace(oldAgentId, newAgentId);
                            //userManager.UpdateUserRegister(userRegist);

                            var parentPath = path + "/";
                            parentPath = parentPath.Replace(currUserId, "");
                            parentPath = parentPath.Replace("//", "/");
                            int index = userRegist.ParentPath.IndexOf(currUserId);
                            var newPath = parentPath + userRegist.ParentPath.Substring(index, userRegist.ParentPath.Length - (index));
                            userRegist.ParentPath = newPath;
                            userManager.UpdateUserRegister(userRegist);
                        }

                    }
                }
            }

        }

        #region 查询代理总销量


        public AgentLottoTopCollection QueryLowerAgentSaleByUserId(string agentId, string userId, string userDisplayName, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new OCAgentManager())
            {
                AgentLottoTopCollection collection = new AgentLottoTopCollection();
                collection.TotalCount = 0;
                string[] array_SqlFilter = new string[] { "'", "--", ";", "=", "+", "\\", "[", "]", ">", "<", "?" };
                if (array_SqlFilter.Contains(userId) || array_SqlFilter.Contains(userDisplayName))
                    throw new Exception("查询条件不能有特殊字符");

                #region 新逻辑
                //System.Diagnostics.Stopwatch myWatch = System.Diagnostics.Stopwatch.StartNew();
                var userList = manager.QueryLowerAgentByUserId(agentId);
                foreach (var u_item in userList)
                {

                    AgentLottoTopInfo info = new AgentLottoTopInfo();
                    //var listData = manager.Test_QueryLowerAgentSaleListByUserId(u_item.UserId, startTime, endTime);//目前查询，包含当前代理本身

                    var listData = manager.QueryLowerAgentSaleListByUserId(u_item.UserId, startTime, endTime);//目前查询，包含当前代理本身
                    foreach (var item in listData)
                    {
                        info.BJDC += item.BJDC;
                        info.CTZQ += item.CTZQ;
                        info.JCLQ += item.JCLQ;
                        info.JCZQ += item.JCZQ;
                        info.SZC += item.SZC;
                        info.TotalMoney += item.TotalMoney;
                    }
                    //for (int i = 0; i < listData.Count; i++)
                    //{
                    //    info.BJDC += listData[i].BJDC;
                    //    info.CTZQ += listData[i].CTZQ;
                    //    info.JCLQ += listData[i].JCLQ;
                    //    info.JCZQ += listData[i].JCZQ;
                    //    info.SZC += listData[i].SZC;
                    //    info.TotalMoney += listData[i].TotalMoney;
                    //}
                    info.UserId = u_item.UserId;
                    info.DisplayName = u_item.DisplayName;
                    collection.AgentLottoTopList.Add(info);
                }
                //myWatch.Stop();
                //Common.Log.ILogWriter writeLog = Common.Log.LogWriterGetter.GetLogWriter();
                //writeLog.Write("代理推广销量查询执行时间", myWatch.ElapsedMilliseconds.ToString(), Common.Log.LogType.Information, "花费时间：" + myWatch.ElapsedMilliseconds.ToString(), "时间:" + myWatch.ElapsedMilliseconds.ToString());
                #endregion

                if (collection != null && collection.AgentLottoTopList != null && collection.AgentLottoTopList.Count > 0)
                {
                    collection.AgentLottoTopList = collection.AgentLottoTopList.Where(s => (s.UserId == userId || userId == string.Empty) && (s.DisplayName == userDisplayName || userDisplayName == string.Empty)).OrderByDescending(s => s.TotalMoney).ToList();
                    collection.TotalCount = collection.AgentLottoTopList.Count;
                    collection.AgentLottoTopList = collection.AgentLottoTopList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                }
                return collection;
            }
        }
        public AgentLottoTopCollection QueryNewLowerAgentSaleByUserId(string agentId, string userId, string userDisplayName, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new OCAgentManager())
            {
                AgentLottoTopCollection collection = new AgentLottoTopCollection();
                collection.TotalCount = 0;
                var list = manager.QueryNewLowerAgentSaleListByUserId(agentId, startTime, endTime);
                if (list != null && list.Count > 0)
                {
                    var _list = list.Where(s => (userId == string.Empty || s.UserId == userId) && (userDisplayName == string.Empty || s.DisplayName == userDisplayName)).ToList();
                    collection.AgentLottoTopList = _list;
                    collection.TotalCount = _list.Count;
                    collection.AgentLottoTopList = collection.AgentLottoTopList.Skip(pageSize * pageIndex).Take(pageSize).ToList();
                }
                return collection;
            }
        }

        #endregion

        #region 后台辅助功能

        Dictionary<int, string> strList = new Dictionary<int, string>();
        int count = 0;
        public void CreateUserParentPath()
        {
            var ocAgentManager = new OCAgentManager();
            var ocAgentList = ocAgentManager.Test_QueryUserRegisterList();
            foreach (var item in ocAgentList)
            {
                count = 0;
                strList.Clear();
                strList.Add(count++, item.AgentId);
                FindParentPath(item.AgentId);
                if (strList != null)
                {
                    if (strList.Count > 2)
                    {
                        string str1 = string.Empty;
                    }
                    var newList = strList.OrderByDescending(s => s.Key).ToList();

                    string str = string.Empty;
                    if (newList.Count == 1 && item.UserId == newList[0].Value)
                        str = "/0";
                    else
                        str = "/0/" + string.Join("/", newList.Select(s => s.Value));
                    var entity = ocAgentManager.Test_QueryUserRegister(item.UserId);
                    if (entity != null)
                    {
                        entity.ParentPath = str;
                        ocAgentManager.UpdateUserRigister(entity);
                    }
                }
            }
        }
        public void FindParentPath(string userId)
        {
            var ocAgentManager = new OCAgentManager();

            var currAgentList = ocAgentManager.Test_QueryUserRegister(userId);
            if (currAgentList == null || string.IsNullOrEmpty(currAgentList.AgentId) || (Convert.ToInt32(currAgentList.AgentId) == Convert.ToInt32(currAgentList.UserId)))
            {
                return;
            }
            strList.Add(count++, currAgentList.AgentId);
            FindParentPath(currAgentList.AgentId);
        }

        #endregion
        /// <summary>
        /// 计算代理销量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentBetMoney"></param>
        /// <param name="isAgent"></param>
        public void CalculationAgentSales(string userId, string gameCode, decimal currentBetMoney, bool isAgent, int index = 0)
        {
            if (string.IsNullOrEmpty(userId) || currentBetMoney <= 0 || string.IsNullOrEmpty(gameCode))
                return;
            try
            {
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);
                if (user != null)
                {
                    var ocAgentSales = balanceManager.QueryOCAgentReportSales(userId, gameCode);
                    if (ocAgentSales == null)
                    {
                        ocAgentSales = new OCAgentReportSales();
                        ocAgentSales.UserId = userId;
                        ocAgentSales.ParentUserId = user.AgentId;
                        ocAgentSales.ParentUserIdPath = user.ParentPath;
                        ocAgentSales.TotalSales = currentBetMoney;
                        if (index != 0)
                        {
                            ocAgentSales.TotalAgentSales = isAgent ? currentBetMoney : 0M;
                            ocAgentSales.TotalUserSales = !isAgent ? currentBetMoney : 0M;
                        }
                        else
                        {
                            ocAgentSales.TotalAgentSales = 0M;
                            ocAgentSales.TotalUserSales = 0M;
                            ocAgentSales.TotalCurrentUserSales = currentBetMoney;
                        }
                        ocAgentSales.GameCode = gameCode;
                        ocAgentSales.CreateTime = DateTime.Now;
                        balanceManager.AddOCAgentReportSales(ocAgentSales);
                    }
                    else
                    {
                        var parentUser = balanceManager.QueryUserRegister(userId);
                        ocAgentSales.UserId = userId;
                        ocAgentSales.ParentUserId = user.AgentId;
                        ocAgentSales.ParentUserIdPath = user.ParentPath;
                        ocAgentSales.TotalSales += currentBetMoney;
                        if (index != 0)
                        {
                            ocAgentSales.TotalAgentSales += isAgent ? currentBetMoney : 0M;
                            ocAgentSales.TotalUserSales += !isAgent ? currentBetMoney : 0M;
                        }
                        else
                            ocAgentSales.TotalCurrentUserSales += currentBetMoney;
                        //else
                        //{
                        //    ocAgentSales.TotalAgentSales = 0M;
                        //    ocAgentSales.TotalUserSales = 0M;
                        //}
                        ocAgentSales.GameCode = gameCode;
                        //ocAgentSales.CreateTime = DateTime.Now;
                        balanceManager.UpdateOCAgentReportSales(ocAgentSales);
                    }
                }
                index++;
                CalculationAgentSales(user.AgentId, gameCode, currentBetMoney, user.IsAgent, index);
            }
            catch
            {
            }
        }


        /// <summary>
        /// 查询代理资金明细
        /// </summary>
        public OCAgentPayDetailInfoCollection QueryAgentRebateAndBonusDetail(string agentId, string schemeid, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, int cpsMode, int schemeType)
        {
            var collection = new OCAgentPayDetailInfoCollection();
            var totalCount = 0;
            var infoList = new OCAgentManager().QueryAgentRebateAndBonusDetail(agentId, schemeid, starTime, endTime, pageIndex, pageSize, cpsMode, schemeType, out totalCount);
            collection.TotalCount = totalCount;
            collection.DetailList = infoList;
            return collection;
        }

        /// <summary>
        /// 结算分红金额
        /// </summary>
        public void SettlementBonus(string userId, decimal bonusMoney)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var balanceManager = new UserBalanceManager();
                var userBalance = balanceManager.QueryUserBalance(userId);
                if (userBalance == null)
                    throw new Exception("没有找到该用户");

                if (userBalance.CPSBalance < bonusMoney)
                    throw new Exception("结算金额大于可结算金额");
                var orderId = Guid.NewGuid().ToString("N");

                var fundManager = new FundManager();
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = BusinessHelper.FundCategory_SettlementBonus,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.CPS,
                    PayMoney = bonusMoney,
                    PayType = PayType.Payout,
                    Summary = "结算分红-->" + bonusMoney,
                    UserId = userId,
                    BeforeBalance = userBalance.CPSBalance,
                    AfterBalance = userBalance.CPSBalance - bonusMoney,
                    OperatorId = userId,
                });

                userBalance.CPSBalance = userBalance.CPSBalance - bonusMoney;
                balanceManager.UpdateUserBalance(userBalance);

                biz.CommitTran();
            }
        }


        /// <summary>
        /// 查询代理销售充值注册量明细汇总
        /// </summary>
        public OCAagentDetailInfoCollection QueryAgentDetail(string agentId, string gameCode, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var collection = new OCAagentDetailInfoCollection();
            var totalCount = 0;
            var totalBuyMoney = 0M;
            var totalFillMoney = 0M;
            var totalBounsMoney = 0M;
            var totalRedBagAwardsMoney = 0M;
            var totalBonusAwardsMoney = 0M;
            var totalWithdrawalsMoney = 0M;
            collection.List = new OCAgentManager().QueryAgentDetail(agentId, gameCode, starTime, endTime, pageIndex, pageSize, out totalCount, out totalBuyMoney, out totalBounsMoney, out totalRedBagAwardsMoney, out totalBonusAwardsMoney, out totalFillMoney, out totalWithdrawalsMoney);
            collection.TotalCount = totalCount;
            collection.TotalBuyMoney = totalBuyMoney;
            collection.TotalFillMoney = totalFillMoney;
            collection.TotalBounsMoney = totalBounsMoney;
            collection.TotalRedBagAwardsMoney = totalRedBagAwardsMoney;
            collection.TotalBonusAwardsMoney = totalBonusAwardsMoney;
            collection.TotalWithdrawalsMoney = totalWithdrawalsMoney;
            return collection;
        }

        /// <summary>
        /// 更新渠道名称
        /// </summary>
        public void UpdateChannelName(string userId, string channelName)
        {
            var manager = new OCAgentManager();
            var userEntity = manager.QueryOCAgent(userId);
            if (userEntity == null)
                throw new Exception("没有查询到该用户-" + userId);
            userEntity.ChannelName = channelName;
            manager.UpdateOCAgent(userEntity);
        }
    }
}
