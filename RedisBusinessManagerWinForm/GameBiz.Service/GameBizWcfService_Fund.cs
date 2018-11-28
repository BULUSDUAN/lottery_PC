using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using GameBiz.Auth.Business;
using Common.Business;
using Common.Utilities;
//using Validation.Business.Mobile;
using GameBiz.Domain.Managers;

namespace GameBiz.Service
{
    public class GameBizWcfService_Fund : WcfService
    {
        public GameBizWcfService_Fund()
        {
            KnownTypeRegister.RegisterKnownTypes(CommunicationObjectGetter.GetCommunicationObjectTypes());
        }

        public CommonActionResult UserFillMoneyByUserId(UserFillMoneyAddInfo info, string userId, string agentId)
        {
            try
            {
                //! 执行扩展功能代码 - 启动事务前
                BusinessHelper.ExecPlugin<IRequestFillMoney_BeforeTranBegin>(new object[] { info, userId, userId });

                var orderId = string.Empty;
                //! 执行扩展功能代码 - 启动事务后
                BusinessHelper.ExecPlugin<IRequestFillMoney_AfterTranBegin>(new object[] { info, userId, userId });

                orderId = new FundBusiness().UserFillMoney(info, userId, userId, agentId);

                //! 执行扩展功能代码 - 提交事务前
                BusinessHelper.ExecPlugin<IRequestFillMoney_BeforeTranCommit>(new object[] { info, userId, userId });

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IRequestFillMoney_AfterTranCommit>(new object[] { info, userId, userId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "提交充值完成",
                    ReturnValue = string.Format("{0}|{1}", orderId, info.PayMoney),
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                BusinessHelper.ExecPlugin<IRequestFillMoney_OnError>(new object[] { info, userId, ex });

                throw new Exception("用户充值出现错误 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 用户充值
        /// </summary>
        public CommonActionResult UserFillMoney(UserFillMoneyAddInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            return UserFillMoneyByUserId(info, userId, "");
        }

        public CommonActionResult SetBalancePassword(string oldPassword, bool isSetPwd, string newPassword, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var biz = new FundBusiness();
                biz.SetBalancePassword(userId, oldPassword, isSetPwd, newPassword);

                BusinessHelper.ExecPlugin<IBalancePassword>(new object[] { userId, oldPassword, isSetPwd, newPassword });
                return new CommonActionResult { IsSuccess = true, Message = "操作资金密码完成" };
            }
            catch (Exception ex)
            {
                throw new Exception("操作资金密码出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 设置资金密码类型
        /// </summary>
        /// <param name="password"></param>
        /// <param name="placeList"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult SetBalancePasswordNeedPlace(string password, string placeList, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var innerList = new string[] { 
                "Bet"               // 投注
                , "Withdraw"        // 提现
                , "Transfer"        // 转账
                , "Red"             // 送红包
                , "CancelWithdraw"  // 取消提现
                , "CancelChase"     // 取消追号
                ,"BuyExperter"      //购买名家分析
                ,"ExchangeDouDou"   //豆豆兑换
            };
            if (placeList != "ALL")
            {
                var list = placeList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (list.Length > 0)
                {
                    foreach (var item in placeList.Split('|'))
                    {
                        if (!innerList.Contains(item))
                        {
                            throw new Exception("不支持设置资金密码类型 - " + item);
                        }
                    }
                }
            }
            try
            {
                var biz = new FundBusiness();
                biz.SetBalancePasswordNeedPlace(userId, password, placeList);
                return new CommonActionResult { IsSuccess = true, Message = "操作完成" };
            }
            catch (Exception ex)
            {
                throw new Exception("设置资金密码类型出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 完成订单充值
        /// </summary>
        public CommonActionResult CompleteFillMoneyOrder(string orderId, FillMoneyStatus status, decimal money, string code, string msg, string outerFlowId, string userToken)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //! 执行扩展功能代码 - 启动事务前
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranBegin>(new object[] { orderId, status, money, myId });

                //lock (UsefullHelper.moneyLocker)
                //{
                string userId;
                bool needContinue = false;
                FillMoneyAgentType agentType;
                var vipLevel = 0;
                //! 执行扩展功能代码 - 启动事务后
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranBegin>(new object[] { orderId, status, money, myId });
                needContinue = new FundBusiness().CompleteFillMoneyOrder(orderId, status, money, code, msg, outerFlowId, out agentType, out userId, out vipLevel);
                if (needContinue)
                {
                    //! 执行扩展功能代码 - 提交事务前
                    //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranCommit>(new object[] { orderId, status, agentType, money, userId });
                }
                if (needContinue)
                {
                    //! 执行扩展功能代码 - 提交事务后
                    BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, status, agentType, money, userId, vipLevel });
                }
                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "充值完成",
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, myId, ex });

                throw new Exception("完成充值出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 充值专员订单完成充值
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <param name="money"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="UserId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public CommonActionResult CompleteFillMoneyOrderByCzzy(string orderId, FillMoneyStatus status, decimal money, string code, string msg, string UserId, string type)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //! 执行扩展功能代码 - 启动事务前
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranBegin>(new object[] { orderId, status, money, myId });

                //lock (UsefullHelper.moneyLocker)
                //{
                string userId;
                bool needContinue = false;
                FillMoneyAgentType agentType;
                var vipLevel = 0;
                //! 执行扩展功能代码 - 启动事务后
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranBegin>(new object[] { orderId, status, money, myId });
                needContinue = new FundBusiness().CompleteFillMoneyOrderByCzzy(orderId, status, money, code, msg, UserId, out agentType, out userId, out vipLevel, type);
                if (needContinue)
                {
                    //! 执行扩展功能代码 - 提交事务前
                    //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranCommit>(new object[] { orderId, status, agentType, money, userId });
                }
                if (needContinue)
                {
                    //! 执行扩展功能代码 - 提交事务后
                    BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, status, agentType, money, userId, vipLevel });
                }
                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "充值完成",
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, myId, ex });

                throw new Exception("完成充值出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 支付宝充值免手续费
        /// </summary>
        public CommonActionResult CompleteFillMoneyOrderFeeFree(string orderId, FillMoneyStatus status, decimal money, string code, string msg, string outerFlowId, DateTime alipayTime, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //! 执行扩展功能代码 - 启动事务前
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranBegin>(new object[] { orderId, status, money, myId });

                //lock (UsefullHelper.moneyLocker)
                //{
                string userId;
                bool needContinue = false;
                FillMoneyAgentType agentType;
                var vipLevel = 0;
                //! 执行扩展功能代码 - 启动事务后
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranBegin>(new object[] { orderId, status, money, myId });
                needContinue = new FundBusiness().CompleteFillMoneyOrderFeeFree(orderId, status, money, code, msg, outerFlowId, alipayTime, out agentType, out userId, out vipLevel);
                if (needContinue)
                {
                    //! 执行扩展功能代码 - 提交事务前
                    //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranCommit>(new object[] { orderId, status, agentType, money, userId });
                }
                if (needContinue)
                {
                    //! 执行扩展功能代码 - 提交事务后
                    BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, status, agentType, money, userId, vipLevel });
                }
                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "充值完成",
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, myId, ex });

                throw new Exception("完成充值出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工充值
        /// </summary>
        public CommonActionResult ManualFillMoney(UserFillMoneyAddInfo info, string userId, string userToken)
        {
            // 验证用户身份及权限
            var requestUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var orderId = string.Empty;
                var vipLevel = 0;
                //lock (UsefullHelper.moneyLocker)
                //{
                orderId = new FundBusiness().ManualFillMoney(info, userId, requestUserId, out vipLevel);

                BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, FillMoneyStatus.Success, FillMoneyAgentType.ManualFill, info.RequestMoney, userId, vipLevel });
                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工充值完成",
                    ReturnValue = orderId,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("手工充值出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工打款
        /// </summary>
        public CommonActionResult ManualAddMoney(string keyLine, decimal money, AccountType accountType, string userId, string message, string userToken)
        {
            // 验证用户身份及权限
            var requestUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var orderId = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                if (string.IsNullOrEmpty(keyLine))
                {
                    keyLine = BusinessHelper.GetManualFillMoneyId();
                }
                new FundBusiness().ManualHandleMoney(keyLine, keyLine, money, accountType, Common.PayType.Payin, userId, message, requestUserId);
                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工打款完成",
                    ReturnValue = orderId,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("手工打款出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工扣款
        /// </summary>
        public CommonActionResult ManualDeductMoney(string keyLine, decimal money, AccountType accountType, string userId, string message, string userToken)
        {
            // 验证用户身份及权限
            var requestUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var orderId = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                if (string.IsNullOrEmpty(keyLine))
                {
                    keyLine = BusinessHelper.GetManualFillMoneyId();
                }
                new FundBusiness().ManualHandleMoney(keyLine, keyLine, money, accountType, Common.PayType.Payout, userId, message, requestUserId);

                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工扣款完成",
                    ReturnValue = orderId,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("手工扣款出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工完成充值
        /// </summary>
        public CommonActionResult ManualCompleteFillMoneyOrder(string orderId, FillMoneyStatus status, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            string userId = "";
            try
            {
                //! 执行扩展功能代码 - 启动事务前
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranBegin>(new object[] { orderId, status, userId });
                var vipLevel = 0;
                //lock (UsefullHelper.moneyLocker)
                //{
                FillMoneyAgentType agentType;
                decimal money;

                //! 执行扩展功能代码 - 启动事务后
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranBegin>(new object[] { orderId, status, userId });
                userId = new FundBusiness().ManualCompleteFillMoneyOrder(orderId, status, out agentType, out money, out vipLevel, myId);

                //! 执行扩展功能代码 - 提交事务前
                //BusinessHelper.ExecPlugin<ICompleteFillMoney_BeforeTranCommit>(new object[] { orderId, status, agentType, money, userId });
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, status, agentType, money, userId, vipLevel });
                //}
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工完成充值成功",
                };
            }
            catch (LogicException ex)
            {
                //! 执行扩展功能代码 - 发生异常
                BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, userId, ex });
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "充值订单重复处理",
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, userId, ex });

                throw new Exception("手工完成充值 出现错误 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询充值订单
        /// </summary>
        public FillMoneyQueryInfo QueryFillMoneyOrder(string orderId, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new FundBusiness().QueryFillMoney(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询充值订单异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询我的余额
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserBalanceInfo QueryMyBalance(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            if (userId == "admin")
                return null;
            try
            {
                var fund = new FundBusiness();
                var entity = fund.QueryUserBalance(userId);
                //return new FundBusiness().QueryUserBalance(userId);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的余额出错 - " + ex.Message, ex);
            }
        }

        public UserBalanceInfo QueryUserBalance(string userId)
        {
            try
            {
                var fund = new FundBusiness();
                var entity = fund.QueryUserBalance(userId);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的余额出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询我的冻结明细
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserBalanceFreezeCollection QueryMyBalanceFreezeList(int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new FundBusiness().QueryUserBalanceFreezeListByUser(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的冻结明细出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户冻结明细
        /// </summary>
        public UserBalanceFreezeCollection QueryUserBalanceFreezeListByUser(string userId, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new FundBusiness().QueryUserBalanceFreezeListByUser(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户冻结明细出错 - " + ex.Message, ex);
            }
        }

        #region 用户奖金账户提现

        public CheckWithdrawResult RequestWithdraw_Step1(string userId, decimal requestMoney)
        {
            try
            {
                return new FundBusiness().RequestWithdraw_Step1(userId, requestMoney);
            }
            catch (Exception ex)
            {
                throw new Exception("申请提现出错 - " + ex.Message, ex);
            }
        }

        public CommonActionResult RequestWithdraw_Step2(Withdraw_RequestInfo info, string userId, string password)
        {
            try
            {
                new FundBusiness().RequestWithdraw_Step2(info, userId, password);
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "申请提现成功",
                };
            }
            catch (Exception ex)
            {
                throw new Exception("申请提现出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 成功提现
        /// </summary>
        public CommonActionResult CompleteWithdraw(string orderId, string responseMsg, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //lock (UsefullHelper.moneyLocker)
                //{
                using (var tran = new GameBizBusinessManagement())
                {
                    tran.BeginTran();
                    //! 执行扩展功能代码 - 启动事务后
                    BusinessHelper.ExecPlugin<ICompleteWithdraw_AfterTranBegin>(new object[] { orderId });
                    new FundBusiness().CompleteWithdraw(orderId, responseMsg, userId);

                    //! 执行扩展功能代码 - 提交事务前
                    BusinessHelper.ExecPlugin<ICompleteWithdraw_BeforeTranCommit>(new object[] { orderId });
                    tran.CommitTran();
                }
                //}
                return new CommonActionResult(true, "完成提现成功");
            }
            catch (Exception ex)
            {
                throw new Exception("完成提现出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 通过UserId成功提现（内部用）
        /// </summary>
        public CommonActionResult CompleteWithdrawByUserId(string orderId, string responseMsg, string userId)
        {

            try
            {
                //lock (UsefullHelper.moneyLocker)
                //{
                using (var tran = new GameBizBusinessManagement())
                {
                    tran.BeginTran();
                    //! 执行扩展功能代码 - 启动事务后
                    BusinessHelper.ExecPlugin<ICompleteWithdraw_AfterTranBegin>(new object[] { orderId });
                    new FundBusiness().CompleteWithdraw(orderId, responseMsg, userId);

                    //! 执行扩展功能代码 - 提交事务前
                    BusinessHelper.ExecPlugin<ICompleteWithdraw_BeforeTranCommit>(new object[] { orderId });
                    tran.CommitTran();
                }
                //}
                return new CommonActionResult(true, "完成提现成功");
            }
            catch (Exception ex)
            {
                throw new Exception("完成提现出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 提交代付改变订单状态
        /// </summary>
        public CommonActionResult ReuqestWithdraw(string orderId, string userToken, string agendid)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new FundBusiness().ReuqestWithdraw(orderId, userId, agendid);
                return new CommonActionResult(true, "提交代付成功");
            }
            catch (Exception ex)
            {
                throw new Exception("提交代付改变订单状态 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 自动代付失败；返回请求中
        /// </summary>
        public CommonActionResult ReuqestWithdraw2(string orderId, string userToken, string agendid)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new FundBusiness().ReuqestWithdraw2(orderId, userId, agendid, WithdrawStatus.Requesting);
                return new CommonActionResult(true, "提交代付成功");
            }
            catch (Exception ex)
            {
                throw new Exception("提交代付改变订单状态 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 拒绝提现
        /// </summary>
        public CommonActionResult RefusedWithdraw(string orderId, string refusedMsg, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //lock (UsefullHelper.moneyLocker)
                //{
                using (var tran = new GameBizBusinessManagement())
                {
                    tran.BeginTran();
                    //! 执行扩展功能代码 - 启动事务后
                    BusinessHelper.ExecPlugin<IRefuseWithdraw_AfterTranBegin>(new object[] { orderId });
                    new FundBusiness().RefusedWithdraw(orderId, refusedMsg, userId);

                    //! 执行扩展功能代码 - 提交事务前
                    BusinessHelper.ExecPlugin<IRefuseWithdraw_BeforeTranCommit>(new object[] { orderId });
                    tran.CommitTran();
                }
                //}
                return new CommonActionResult(true, "拒绝提现成功");
            }
            catch (Exception ex)
            {
                throw new Exception("拒绝提现出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 拒绝提现
        /// </summary>
        public CommonActionResult RefusedWithdrawByUserId(string orderId, string refusedMsg, string userId)
        {

            try
            {
                //lock (UsefullHelper.moneyLocker)
                //{
                using (var tran = new GameBizBusinessManagement())
                {
                    tran.BeginTran();
                    //! 执行扩展功能代码 - 启动事务后
                    BusinessHelper.ExecPlugin<IRefuseWithdraw_AfterTranBegin>(new object[] { orderId });
                    new FundBusiness().RefusedWithdraw(orderId, refusedMsg, userId);

                    //! 执行扩展功能代码 - 提交事务前
                    BusinessHelper.ExecPlugin<IRefuseWithdraw_BeforeTranCommit>(new object[] { orderId });
                    tran.CommitTran();
                }
                //}
                return new CommonActionResult(true, "拒绝提现成功");
            }
            catch (Exception ex)
            {
                throw new Exception("拒绝提现出错 - " + ex.Message, ex);
            }
        }


        #region 4.后台查询

        /// <summary>
        /// 查询提现记录
        /// </summary>
        public Withdraw_QueryInfo GetWithdrawById(string orderId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new FundBusiness().GetWithdrawById(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询提现记录列表
        /// </summary>
        public Withdraw_QueryInfoCollection QueryWithdrawList(string userKey, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, int pageIndex, int pageSize, string orderId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new FundBusiness().QueryWithdrawList(userKey, agent, status, minMoney, maxMoney, startTime, endTime, sortType, pageIndex, pageSize, orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 5.会员中心查询
        /// <summary>
        /// 查询我的提现记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Withdraw_QueryInfo GetMyWithdrawById(string orderId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var order = new FundBusiness().GetWithdrawById(orderId);
                if (order.RequesterUserKey != userId)
                {
                    throw new Exception("不允许查看别人的提现订单");
                }
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据orderid查询提现记录
        /// </summary>
        /// <param name="orderId"></param>
        /// 
        /// <returns></returns>
        public Withdraw_QueryInfo GetWithdrawByOrderId(string orderId)
        {
            try
            {
                var order = new FundBusiness().GetWithdrawById(orderId);

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        public Withdraw_QueryInfoCollection QueryMyWithdrawList(WithdrawStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {

            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new FundBusiness().QueryWithdrawList(userId, null, status, -1, -1, startTime, endTime, -1, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的提现记录列表 - " + ex.Message, ex);
            }
        }
        #endregion

        #endregion


        /// <summary>
        /// 赠送红包
        /// </summary>
        /// <param name="userIdToList">收红包的用户列表，以“|”分隔，每个用户以“#”分隔Id和红包金额。如：001#2|002#3.5|003#20</param>
        /// <param name="password">输入的资金密码，如果未设置资金密码，或者未配置赠送红包需资金密码，此项被忽略</param>
        /// <param name="message">赠送者留言</param>
        public CommonActionResult GiveRedMoney(string userIdToList, string password, string message, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var dict = new Dictionary<string, decimal>();
                foreach (var item in userIdToList.Split('|'))
                {
                    dict.Add(item.Split('#')[0], decimal.Parse(item.Split('#')[1]));
                }
                var biz = new FundBusiness();
                biz.GiveRed(userId, dict, password, message);
                return new CommonActionResult { IsSuccess = true, Message = "赠送红包完成" };
            }
            catch (Exception ex)
            {
                throw new Exception("赠送红包出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 奖金转通用账户
        /// </summary>
        /// <param name="password"></param>
        /// <param name="transferMoney"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult TransferBonus2CommonBalance(string password, decimal transferMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var biz = new FundBusiness();
                //biz.TransferAccount(userId, password, transferMoney, AccountType.Bonus, AccountType.Common);

                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<ITransferMoney_AfterTranCommit>(new object[] { userId, transferMoney, AccountType.Bonus, AccountType.Common });
                return new CommonActionResult { IsSuccess = true, Message = "操作成功" };
            }
            catch (Exception ex)
            {
                throw new Exception("奖金转通用账户失败 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 佣金转通用
        /// </summary>
        public CommonActionResult CommissionToCommon(string password, decimal transferMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var biz = new FundBusiness();
                //biz.TransferAccount(userId, password, transferMoney, AccountType.Commission, AccountType.Common);

                ////! 执行扩展功能代码 - 提交事务后
                //Plugin.Core.PluginRunner.ExecPlugin<ITransferMoney_AfterTranCommit>(new object[] { userId, transferMoney, AccountType.Bonus, AccountType.Common });

                return new CommonActionResult { IsSuccess = true, Message = "操作成功" };
            }
            catch (Exception ex)
            {
                throw new Exception("佣金转通用账户失败 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询银行卡信息
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BankCardInfo QueryBankCard(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new BankCardBusiness().BankCardById(userId);
            }
            catch (LogicException)
            {
                return new BankCardInfo { };
            }
            catch (Exception ex)
            {
                throw new LogicException(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据用户编号查询银行卡信息
        /// </summary>
        public BankCardInfo QueryBankCardByUserId(string userId, string userToken)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new BankCardBusiness().BankCardById(userId);
            }
            catch (LogicException)
            {
                return new BankCardInfo { };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 增加银行卡信息
        /// </summary>
        /// <param name="bankCard"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult AddBankCard(BankCardInfo bankCard, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var entity = new BankCardManager().BankCardByCode(bankCard.BankCardNumber);
                if (entity != null)
                {
                    throw new Exception("该银行卡号已经被其他用户绑定，请选择其它银行卡号");
                }
                if (string.IsNullOrEmpty(bankCard.UserId) || bankCard.UserId == null || bankCard.UserId.Length == 0)
                    bankCard.UserId = userId;

                var bankcarduser = new BankCardManager().BankCardById(userId);
                if (bankcarduser != null)
                    throw new Exception("您已绑定了银行卡，请不要重复绑定！");
                new BankCardBusiness().AddBankCard(bankCard);
                new CacheDataBusiness().ClearUserBindInfoCache(userId);
                //绑定银行卡之后实现接口
                BusinessHelper.ExecPlugin<IAddBankCard>(new object[] { bankCard.UserId, bankCard.BankCardNumber, bankCard.BankCode, bankCard.BankName, bankCard.BankSubName, bankCard.CityName, bankCard.ProvinceName, bankCard.RealName });
                return new CommonActionResult(true, "添加银行卡信息成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加银行卡信息出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 修改银行卡信息
        /// </summary>
        public CommonActionResult UpdateBankCard(BankCardInfo bankCard, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                new BankCardBusiness().UpdateBankCard(bankCard, bankCard.UserId);
                return new CommonActionResult(true, "修改银行卡信息成功");
            }
            catch (Exception ex)
            {
                throw new Exception("修改银行卡信息出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 解除银行卡绑定
        /// </summary>
        /// <returns></returns>
        public CommonActionResult CancelBankCard(string userId)
        {
            // 验证用户身份及权限
            // var Id = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                new BankCardBusiness().CancelBankCard(userId);
                return new CommonActionResult(true, "银行解除绑定成功");
            }
            catch (Exception ex)
            {
                throw new Exception("银行卡解除出错 - " + ex.Message, ex);
            }
        }

        private string GetRandomMobileValidateCode()
        {
            var validateCode = "8888";
            if (!UsefullHelper.IsInTest)
            {
                // 生成随机密码
                Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
                validateCode = random.Next(1000, 9999).ToString();
            }
            return validateCode;
        }
        private int GetDelay(int delay)
        {
            if (UsefullHelper.IsInTest)
            {
                return 0;
            }
            return delay;
        }
        private int GetMaxTimes(int time)
        {
            if (UsefullHelper.IsInTest)
            {
                return 100000;
            }
            return time;
        }

        #region 财务人员设置

        public CommonActionResult FinanceSetting(string opeType, FinanceSettingsInfo info, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                new UserBusiness().FinanceSetting(opeType, info, userId);
                if (opeType.ToLower() != "delete")
                {
                    return new CommonActionResult(true, "保存数据成功！");
                }
                else
                {
                    return new CommonActionResult(true, "删除数据成功！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public FinanceSettingsInfo_Collection GetFinanceSettingsCollection(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                return new UserBusiness().GetFinanceSettingsCollection(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetCaiWuOperator(string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                return new UserBusiness().GetCaiWuOperator();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FinanceSettingsInfo GetFinanceSettingsByFinanceId(string FinanceId, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                return new UserBusiness().GetFinanceSettingsByFinanceId(FinanceId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 保存或者修改礼品配置
        /// </summary>
        /// <param name="info"></param>
        /// <param name="opeType"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult SaveOrUpdateActivityPrizeConfig(ActivityPrizeConfigInfo info, string opeType, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                info.CreatorId = userId;
                //new UserIntegralBusiness().SaveOrUpdateActivityPrizeConfig(info, opeType);
                return new CommonActionResult(true, "保存数据成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 删除礼品配置项
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult DeleteActivityPrizeConfig(int activityId, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                //new UserIntegralBusiness().DeleteActivityPrizeConfig(activityId);
                return new CommonActionResult(true, "删除数据成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询礼品配置列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActivityPrizeConfigInfo_Collection QueryActivityPrizeConfigCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                //return new UserIntegralBusiness().QueryActivityPrizeConfigCollection(title, startTime, endTime, pageIndex, pageSize);
                return new ActivityPrizeConfigInfo_Collection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据Id获取礼品配置信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public ActivityPrizeConfigInfo GetActivityPrizeConfigById(int activityId, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                //return new UserIntegralBusiness().GetActivityPrizeConfig(activityId);
                return new ActivityPrizeConfigInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// 查询某人成长值的赠送记录
        /// </summary>
        public UserGrowthDetailInfoCollection QueryUserGrowthDetailList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new FundBusiness().QueryUserGrowthDetailList(userId, starTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询记录列表 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户成长值列表
        /// </summary>
        public UserGrowthDetailInfoCollection QueryUserGrowList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new FundBusiness().QueryUserGrowList(userId, starTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 首页发送推广短信

        /// <summary>
        /// 首页发送推广短息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public CommonActionResult AddSendMsgHistoryRecord(SendMsgHistoryRecordInfo info)
        {
            try
            {
                new UserBusiness().AddSendMsgHistoryRecord(info);
                return new CommonActionResult(true, "发送短信成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        /// <summary>
        /// 查询短信发送列表
        /// </summary>
        public SendMsgHistoryRecord_Collection QueryHistoryRecordCollection(string mobile, string status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new UserBusiness().QueryHistoryRecordCollection(mobile, status, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        public UserFundDetailCollection QueryFundDetailByDateTime(DateTime date)
        {
            return new FundBusiness().QueryFundDetailByDateTime(date);
        }

        public UserFundDetailCollection QueryFundDetailByDateTimeUserId(DateTime date, string userId)
        {
            return new FundBusiness().QueryFundDetailByDateTime(date, userId);
        }
        public CommonActionResult BuildFundDetailByDate(string json, string userId, string date)
        {
            new FundBusiness().BuildFundDetailByDate(json, userId, date);
            return new CommonActionResult(true, "生成成功");
        }

        public CommonActionResult BuildFundDetailCacheByDateTime(DateTime date)
        {
            new FundBusiness().BuildFundDetailCacheByDateTime(date);
            return new CommonActionResult(true, "调用完成");
        }

        public CommonActionResult ManualBuildFundDetailCache(DateTime date)
        {
            new FundBusiness().DoBuildFundDetailCacheByDateTime(date);
            return new CommonActionResult(true, "调用完成");
        }

        public decimal QueryUserTotalFillMoney(string userId)
        {
            return new FundBusiness().QueryUserTotalFillMoney(userId);
        }

        public decimal QueryUserTotalWithdrawMoney(string userId)
        {
            return new FundBusiness().QueryUserTotalWithdrawMoney(userId);
        }
    }
}
