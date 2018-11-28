using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Auth.Business;
using GameBiz.Core;
using GameBiz.Business;
using System.Configuration;
using Common.Utilities;
using Common.Business;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Core
    {
        public CommonActionResult BJDCBetting(BJDCBettingInfo info, string password, string userToken)
        {
            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "W", "C010", out userId);
            try
            {
                //! 执行扩展功能代码 - 启动事务前
                Plugin.Core.PluginRunner.ExecPlugin<IBJDCBettingInfo_BeforeTranBegin>(info);
                string schemeId;
                lock (UsefullHelper.moneyLocker)
                {
                    using (var biz = new GameBizBusinessManagement())
                    {
                        biz.BeginTran();

                        //! 执行扩展功能代码 - 启动事务后
                        Plugin.Core.PluginRunner.ExecPlugin<IBJDCBettingInfo_AfterTranBegin>(info);

                        schemeId = new BJDCBusiness().BJDCBetting(info, userId, password);

                        //! 执行扩展功能代码 - 提交事务前
                        Plugin.Core.PluginRunner.ExecPlugin<IBJDCBettingInfo_BeforeTranCommit>(new object[] { info, schemeId });

                        //? 提交分布式事务
                        biz.CommitTran();
                    }
                    //! 执行扩展功能代码 - 提交事务后
                    Plugin.Core.PluginRunner.ExecPlugin<IBJDCBettingInfo_AfterTranCommit>(new object[] { info, schemeId });
                }
                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId,
                    Message = " 北京单场投注成功",
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                Plugin.Core.PluginRunner.ExecPlugin<IBJDCBettingInfo_OnError>(new object[] { info, ex });
                throw new Exception("北京单场投注失败 - " + ex.Message, ex);
            }
        }

        public string QueryBJDCWaitForChaseSchemeIdArray(string userToken)
        {
            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "R", "C040", out userId);
            try
            {
                return new BJDCBusiness().QueryBJDCWaitForChaseSchemeIdArray();
            }
            catch (Exception ex)
            {
                throw new Exception("北京单场查询等待投注的订单信息错误 - " + ex.Message, ex);
            }
        }

        public CommonActionResult BJDCChase(string orderId, string userToken)
        {
            var orderIdArray = orderId.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (orderIdArray.Length != 2)
                throw new Exception("北京单场传入OrderId格式不正确:" + orderId);

            SchemeType schemeType;
            decimal issuseMoney;
            string orderUserId;
            var schemeId = orderIdArray[0];
            var issuseNumber = orderIdArray[1];
            var isRequestTicketSuccess = true;
            var errMsg = "";
            var gameCode = "BJDC";
            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "W", "C041", out userId);

            try
            {
                //开始事务之前
                Plugin.Core.PluginRunner.ExecPlugin<IChase_BeforeTranBegin>(new object[] { gameCode, issuseNumber, schemeId });


                lock (UsefullHelper.moneyLocker)
                {
                    using (var biz = new GameBizBusinessManagement())
                    {
                        biz.BeginTran();

                        //开始事务之后
                        Plugin.Core.PluginRunner.ExecPlugin<IChase_AfterTranBegin>(new object[] { gameCode, issuseNumber, schemeId });

                        var agentToken = ConfigurationManager.AppSettings["GatewayAgentToken"];
                        new BJDCBusiness().BJDCChase(schemeId, agentToken, out schemeType, out issuseMoney, out orderUserId, out isRequestTicketSuccess, out errMsg);

                        //提交事务前
                        Plugin.Core.PluginRunner.ExecPlugin<IChase_BeforeTranCommit>(new object[] { gameCode, issuseNumber, schemeId, schemeType, issuseMoney, orderUserId });

                        biz.CommitTran();
                    }

                    //提交事务后
                    Plugin.Core.PluginRunner.ExecPlugin<IChase_AfterTranCommit>(new object[] { gameCode, issuseNumber, schemeId, schemeType, issuseMoney, orderUserId });
                    return new CommonActionResult
                    {
                        IsSuccess = isRequestTicketSuccess,
                        Message = isRequestTicketSuccess ? "请求出票成功" : "请求出票失败 - " + errMsg,
                        ReturnValue = orderId,
                    };
                }
            }
            catch (Exception ex)
            {
                Plugin.Core.PluginRunner.ExecPlugin<IChase_OnError>(new object[] { gameCode, issuseNumber, schemeId, ex });
                throw new Exception("北京单场追号失败 - " + ex.Message, ex);
            }
        }

        public CommonActionResult UpdateBJDCTicketResult(string schemeId, string ticketId, bool isSuccess, string userToken)
        {
            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "W", "C040", out userId);
            try
            {
                lock (UsefullHelper.moneyLocker)
                {
                    using (var biz = new GameBizBusinessManagement())
                    {
                        biz.BeginTran();

                        new BJDCBusiness().UpdateBJDCTicketResult(schemeId, ticketId, isSuccess);

                        biz.CommitTran();
                    }
                }
                return new CommonActionResult(true, "更新出票状态成功");
            }
            catch (Exception ex)
            {
                throw new Exception("北京单场更新出票结果失败 - " + ex.Message, ex);
            }
        }

        public CommonActionResult BJDCQueryTicket(string orderId, string userToken)
        {
            string[] orderArray = orderId.Split('|');
            if (orderArray.Length != 2)
                throw new Exception("北京单场传入OrderId格式不正确:" + orderId);

            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "W", "C040", out userId);
            try
            {
                string msg = string.Empty;
                lock (UsefullHelper.moneyLocker)
                {
                    using (var biz = new GameBizBusinessManagement())
                    {
                        biz.BeginTran();

                        var agentToken = ConfigurationManager.AppSettings["GatewayAgentToken"];
                        msg = new BJDCBusiness().BJDCQueryTicket(orderArray[0], orderArray[1], agentToken);

                        biz.CommitTran();
                    }
                }
                return new CommonActionResult(true, msg);
            }
            catch (Exception ex)
            {
                throw new Exception("北京单场票查询失败 - " + ex.Message, ex);
            }
        }

        public CommonActionResult BJDCPrize(string schemeId, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, string userToken)
        {
            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "W", "C040", out userId);
            try
            {
                //! 执行扩展功能代码 - 启动事务前
                Plugin.Core.PluginRunner.ExecPlugin<IPrizeOrder_BeforeTranBegin>(new object[] { schemeId, isBonus, preTaxBonusMoney, afterTaxBonusMoney });

                lock (UsefullHelper.moneyLocker)
                {
                    var result = new PrizeResult();
                    using (var biz = new GameBizBusinessManagement())
                    {
                        biz.BeginTran();

                        //! 执行扩展功能代码 - 启动事务后
                        Plugin.Core.PluginRunner.ExecPlugin<IPrizeOrder_AfterTranBegin>(new object[] { schemeId, isBonus, preTaxBonusMoney, afterTaxBonusMoney });

                        result = new BJDCBusiness().BJDCPrize(schemeId, isBonus ? BonusStatus.Win : BonusStatus.Lose, preTaxBonusMoney, afterTaxBonusMoney);
                        //! 执行扩展功能代码 - 启动提交前
                        Plugin.Core.PluginRunner.ExecPlugin<IPrizeOrder_BeforeTranCommit>(new object[] { result });

                        biz.CommitTran();
                    }
                    //if (isBonus)
                    //{
                    //    GameBizWcfServiceCache.RefreshLastBonusList(result.GameCode);
                    //}
                    //! 执行扩展功能代码 - 启动事务后
                    Plugin.Core.PluginRunner.ExecPlugin<IPrizeOrder_AfterTranCommit>(new object[] { result });
                }

                return new CommonActionResult(true, "派奖完成");
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                Plugin.Core.PluginRunner.ExecPlugin<IPrizeOrder_OnError>(new object[] { schemeId, isBonus, preTaxBonusMoney, afterTaxBonusMoney, ex });
                throw new Exception(string.Format("订单{0} 派奖失败 - {1} ! ", schemeId, ex.Message), ex);
            }
        }

        public BJDCSchemeQueryInfo QueryBJDCSchemeInfo(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            string userId;
            GameBizAuthBusiness.ValidateAuthentication(userToken, "R", "C050", out userId);
            try
            {
                return new BJDCBusiness().QueryBJDCSchemeInfo(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询北京单场订单信息失败 - " + ex.Message, ex);
            }
        }

    }
}
