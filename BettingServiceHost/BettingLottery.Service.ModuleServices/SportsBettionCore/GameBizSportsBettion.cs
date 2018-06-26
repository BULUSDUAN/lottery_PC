using EntityModel.Communication;
using EntityModel.CoreModel.BetingEntities;
using Lottery.Kg.ORM.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BettingLottery.Service.ModuleServices.SportsBettionCore
{
    public class GameBizSportsBettion
    {
        public CommonActionResult Sports_Betting(Sports_BetingInfo info, string password, decimal redBagMoney, string userToken)
        {
            try
            {
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckJCRepeatBetting(userId, info);
                //检查投注内容,并获取投注注数
                var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //检查投注的比赛，并获取最早结束时间
                var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);

                string schemeId = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                schemeId = new Sports_Business().SportsBetting(info, userId, password, "Bet", totalCount, stopTime, redBagMoney);
                //}
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "足彩投注成功",
                };
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
            }

        }

    }
}
