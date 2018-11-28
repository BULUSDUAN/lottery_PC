using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 代理相关业务
    /// </summary>
    public class OCAgentBusiness : DBbase
    {
        /// <summary>
        /// 后台会员详情，更新经销商
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agentId"></param>
        public void UpdateUserAgentId(string userId, string agentId)
        {
            //开启事务
            DB.Begin();
            try
            {
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
                                }
                            }
                        }
                        //修改当前用户下级相关联用户的返点
                        UpdateRelationChildRebate(userId, oldAgentId, agentId, reg.ParentPath);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
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
        /// <summary>
        /// 添加代理
        /// </summary>
        public void AddOCAgent(OCAgentCategory category, string parentUserId, string userId, CPSMode cpsmode)
        {
                DB.Begin();
            try
            {
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
                        switch ((OCAgentCategory)parentAgent.OCAgentCategory)
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
                agentManager.AddOCAgent(new P_OCAgent()
                {
                    CreateTime = DateTime.Now,
                    OCAgentCategory = (int)category,
                    ParentUserId = parentUserId,
                    UserId = userId,
                    CustomerDomain = string.Empty,
                    ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath,
                    CPSMode = category == OCAgentCategory.Company ? (int)cpsmode : parentAgent.CPSMode,
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
                        agentManager.AddOCAgentRebate(new P_OCAgent_Rebate()
                        {
                            CreateTime = DateTime.Now,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            UserId = userId,
                            Rebate = item.SubUserRebate,
                            SubUserRebate = 0M,
                            RebateType = item.RebateType,
                            CPSMode = category == OCAgentCategory.Company ? (int)cpsmode : parentAgent.CPSMode,
                        });
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询代理销售充值注册量明细汇总
        /// </summary>
        public OCAagentDetailInfoCollection QueryAgentDetail(string agentId, string gameCode, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, bool isRecharge)
        {
            var collection = new OCAagentDetailInfoCollection();
            collection = new OCAgentManager().QueryAgentDetail(agentId, gameCode, starTime, endTime, pageIndex, pageSize, isRecharge);
            return collection;
        }

        /// <summary>
        /// 对已派奖订单返点
        /// </summary>
        public void AgentPayIn_CompateOrder(string schemeId)
        {
            string currentUserId = string.Empty;
            decimal currentBetMoney = 0M;
            string currentGameCode = string.Empty;
            bool currentIsAgent = false;
            DB.Begin();
            try
            {
                //查询订单信息
                var sportsManager = new Sports_Manager();
            var manager = new SchemeManager();
            var order = sportsManager.QuerySports_Order_Complate(schemeId);
            if (order == null)
                throw new LogicException(string.Format("找不到已派奖订单 ：{0} ", schemeId));
            if (order.IsPayRebate)
                throw new LogicException(string.Format("订单{0}已执行返点", schemeId));
            if (order.TicketStatus != (int)TicketStatus.Ticketed)
                throw new LogicException("订单未出票完成，不能返点");

            var orderDetail = sportsManager.QueryOrderDetailBySchemeId(schemeId);
            if (orderDetail == null)
                throw new LogicException(string.Format("找不到订单 ：{0} ", schemeId));

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
                    if (order.SchemeType == (int)SchemeType.TogetherBetting)
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
                        totalPayRebateMoney = PayOrderRebate(agentManager, user, schemeId, userId, (SchemeType)order.SchemeType, gameCode, gameType, order.TotalMoney, realMoney, 0, rebateType);
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
                DB.Commit();
                currentUserId = order.UserId;
                currentBetMoney = order.SuccessMoney;
                currentGameCode = order.GameCode;

                //计算代理销量
                CalculationAgentSales(currentUserId, currentGameCode, currentBetMoney, currentIsAgent, 0);
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }

        //计算用户自身返点
        public decimal PayOrderRebate(OCAgentManager manager, C_User_Register currentUser, string schemeId, string orderUserId, SchemeType schemeType,
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
                if (agentRebate.CPSMode != (int)CPSMode.PayRebate)
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

                manager.AddOCAgentPayDetail(new P_OCAgent_PayDetail
                {
                    CreateTime = DateTime.Now,
                    GameCode = gameCode,
                    GameType = gameType,
                    PayMoney = payMoney,
                    OrderTotalMoney = totalOrderMoney,
                    Rebate = currentPayRebate,
                    SchemeId = schemeId,
                    SchemeType = (int)schemeType,
                    CPSMode = (int)CPSMode.PayRebate,
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
                //if (user != null)
                //{
                //    var ocAgentSales = balanceManager.QueryOCAgentReportSales(userId, gameCode);
                //    if (ocAgentSales == null)
                //    {
                //        ocAgentSales = new OCAgentReportSales();
                //        ocAgentSales.UserId = userId;
                //        ocAgentSales.ParentUserId = user.AgentId;
                //        ocAgentSales.ParentUserIdPath = user.ParentPath;
                //        ocAgentSales.TotalSales = currentBetMoney;
                //        if (index != 0)
                //        {
                //            ocAgentSales.TotalAgentSales = isAgent ? currentBetMoney : 0M;
                //            ocAgentSales.TotalUserSales = !isAgent ? currentBetMoney : 0M;
                //        }
                //        else
                //        {
                //            ocAgentSales.TotalAgentSales = 0M;
                //            ocAgentSales.TotalUserSales = 0M;
                //            ocAgentSales.TotalCurrentUserSales = currentBetMoney;
                //        }
                //        ocAgentSales.GameCode = gameCode;
                //        ocAgentSales.CreateTime = DateTime.Now;
                //        balanceManager.AddOCAgentReportSales(ocAgentSales);
                //    }
                //    else
                //    {
                //        var parentUser = balanceManager.QueryUserRegister(userId);
                //        ocAgentSales.UserId = userId;
                //        ocAgentSales.ParentUserId = user.AgentId;
                //        ocAgentSales.ParentUserIdPath = user.ParentPath;
                //        ocAgentSales.TotalSales += currentBetMoney;
                //        if (index != 0)
                //        {
                //            ocAgentSales.TotalAgentSales += isAgent ? currentBetMoney : 0M;
                //            ocAgentSales.TotalUserSales += !isAgent ? currentBetMoney : 0M;
                //        }
                //        else
                //            ocAgentSales.TotalCurrentUserSales += currentBetMoney;
                //        //else
                //        //{
                //        //    ocAgentSales.TotalAgentSales = 0M;
                //        //    ocAgentSales.TotalUserSales = 0M;
                //        //}
                //        ocAgentSales.GameCode = gameCode;
                //        //ocAgentSales.CreateTime = DateTime.Now;
                //        balanceManager.UpdateOCAgentReportSales(ocAgentSales);
                //    }
                //}
                index++;
                CalculationAgentSales(user.AgentId, gameCode, currentBetMoney, user.IsAgent, index);
            }
            catch
            {
            }
        }
    }
}
