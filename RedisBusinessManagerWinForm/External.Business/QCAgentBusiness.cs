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

namespace External.Business
{
    /// <summary>
    /// 代理相关业务(分红模式)
    /// </summary>
    public class QCAgentBusiness : IOrderPrize_AfterTranCommit
    {
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var order = sportsManager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new LogicException(string.Format("自动分红，没有找到订单{0}", schemeId));
                //if (order.IsPayRebate)
                //    throw new LogicException(string.Format("订单{0}已分红", schemeId));
                var bonusAwardsMoney = 0M;
                if (order.AddMoneyDescription.Contains("10"))
                {
                    bonusAwardsMoney = order.AddMoney;
                }
                var bonusMoney = (order.TotalMoney - order.RedBagMoney - order.AfterTaxBonusMoney + bonusAwardsMoney);
                if (order.TicketStatus == TicketStatus.Ticketed && !order.IsVirtualOrder)
                {
                    var msg = string.Empty;
                    //合买判断
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        var main = sportsManager.QuerySports_Together(schemeId);
                        if (main == null)
                        {
                            msg = string.Format("找不到合买订单:{0}", schemeId);
                        }
                        var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                        if (sysJoinEntity != null && sysJoinEntity.RealBuyCount > 0)
                        {
                            msg = "网站参与保底，不分红";
                        }

                        if (main.SoldCount + main.Guarantees < main.TotalCount)
                            throw new Exception("订单未满员，不执行分红");
                    }

                    if (string.IsNullOrEmpty(msg))
                    {
                        var userManager = new UserBalanceManager();
                        var ocAgentManager = new OCAgentManager();
                        var orderGameType = string.Empty;
                        if (new string[] { "CTZQ", "BJDC", "JCZQ", "JCLQ" }.Contains(order.GameCode))
                        {
                            orderGameType = order.GameType;
                        }

                        PayOrderBonus(userManager, ocAgentManager, order.SchemeId, order.SchemeType, order.UserId, order.GameCode, orderGameType, order.UserId, bonusMoney, 0);
                    }
                }

                order.IsPayRebate = true;
                sportsManager.UpdateSports_Order_Complate(order);

                biz.CommitTran();
            }
        }

        private void PayOrderBonus(UserBalanceManager userManager, OCAgentManager ocAgentManager, string schemeId, SchemeType schemeType, string orderUserId,
            string gameCode, string gameType, string currentUserId, decimal bonusMoney, decimal doPercent)
        {
            var user = userManager.QueryUserRegister(currentUserId);
            if (user == null)
                return;

            var rebate = doPercent;
            var bonus = ocAgentManager.QueryOCAgentDefaultRebate(currentUserId, gameCode, gameType, CPSMode.PayBonus);
            if (bonus != null && bonus.Rebate > 0M)
            {
                rebate = bonus.Rebate - doPercent;
                if (rebate < 0)
                    return;
                var giveBonus = bonusMoney * rebate / 100;
                if (giveBonus != 0)
                {
                    var remark = string.Format("订单{0}分红{1}元。", schemeId, giveBonus);
                    ocAgentManager.AddOCAgentPayDetail(new OCAgentPayDetail
                    {
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = gameType,
                        OrderTotalMoney = bonusMoney,
                        OrderUser = orderUserId,
                        PayInUserId = currentUserId,
                        PayMoney = giveBonus,
                        Rebate = rebate,
                        Remark = remark,
                        SchemeId = schemeId,
                        SchemeType = schemeType,
                        CPSMode = CPSMode.PayBonus,
                    });
                    if (giveBonus > 0)
                    {
                        //添加佣金
                        BusinessHelper.Payin_To_Balance(AccountType.CPS, BusinessHelper.FundCategory_SchemeBonus, currentUserId,
                            schemeId, giveBonus, remark);
                    }
                    else
                    {
                        //添加佣金
                        BusinessHelper.Payout_To_End_CPS(AccountType.CPS, BusinessHelper.FundCategory_SchemeBonus, currentUserId,
                            schemeId, decimal.Negate(giveBonus), remark);
                    }


                }
            }

            if (string.IsNullOrEmpty(user.AgentId))
                return;

            //递归
            PayOrderBonus(userManager, ocAgentManager, schemeId, schemeType, orderUserId, gameCode, gameType, user.AgentId, bonusMoney, rebate);
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                writer.Write("EXEC_Plugin_QCAgentBusiness_Error_", type, ex);
            }
            return null;
        }
    }
}
