using System;
using System.Linq;
using System.Transactions;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using GameBiz.Auth.Business;
using Common.Utilities;
using System.Configuration;
using Common.Expansion;
using System.Collections.Generic;
using System.Dynamic;
using Common.Cryptography;
using System.Text;
using System.Threading;
using Common.JSON;
using System.Globalization;
using System.Diagnostics;
using GameBiz.Core.Ticket;
using System.IO;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Core : WcfService
    {
        #region 北单、竞彩投注

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

        private void CheckDisableGame(string gameCode, string gameType)
        {
            var status = new GameBusiness().LotteryGameToStatus(gameCode);
            if (status != Common.EnableStatus.Enable)
                throw new Exception("彩种暂时不能投注");
        }

        /// <summary>
        /// 足彩投注,用户保存的订单
        /// </summary>
        public CommonActionResult SaveOrderSportsBetting(Sports_BetingInfo info, string userToken)
        {
            // 验证用户身份及权限    
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckDisableGame(info.GameCode, info.GameType);
                BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);

                // 检查订单基本信息
                CheckSchemeOrder(info);

                string schemeId = new Sports_Business().SaveOrderSportsBetting(info, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "保存订单成功",
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("保存订单异常，请重试 ", ex);
            }
        }
        /// <summary>
        /// 足彩投注和追号
        /// </summary>
        public CommonActionResult Sports_BettingAndChase(Sports_BetingInfo info, string password, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var isSuceess = true;
                var t = this.Sports_Betting(info, password, redBagMoney, userToken);
                isSuceess = t.IsSuccess;
                var schemeId = string.Empty;
                var money = 0M;
                var array = t.ReturnValue.Split('|');
                if (array.Length == 2)
                {
                    schemeId = array[0];
                    money = decimal.Parse(array[1]);
                }
                if (isSuceess)
                {
                    SportsChase(schemeId);
                }
                return new CommonActionResult { IsSuccess = isSuceess, Message = "订单提交成功", ReturnValue = string.Format("{0}|{1}", schemeId, money) };
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 数字彩、传统足球投注

        /// <summary>
        /// 取消追号订单
        /// </summary>
        public CommonActionResult CancelChaseOrder(string schemeId)
        {
            try
            {
                new Sports_Business().CancelChaseOrder(schemeId);
                return new CommonActionResult(true, "取消追号订单成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public string QueryNoChaseOrder()
        {
            return new Sports_Business().QueryNoChaseOrder();
        }

        /// <summary>
        /// 欧洲杯投注
        /// </summary>
        public CommonActionResult BetOZB(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                //var checkError = CheckGeneralRepeatBetting(userId, info);
                //if (!string.IsNullOrEmpty(checkError))
                //    throw new LogicException(checkError);

                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().BetOZB(info, userId, balancePassword, "Bet", redBagMoney);
                //}

                return new CommonActionResult(true, "方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
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


        /// <summary>
        /// 世界杯投注
        /// </summary>
        public CommonActionResult BetSJB(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                //var checkError = CheckGeneralRepeatBetting(userId, info);
                //if (!string.IsNullOrEmpty(checkError))
                //    throw new LogicException(checkError);

                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().BetSJB(info, userId, balancePassword, "Bet", redBagMoney);
                //}

                return new CommonActionResult(true, "方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
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

        //private static Common.Log.ILogWriter logger = Common.Log.LogWriterGetter.GetLogWriter();
        /// <summary>
        /// 数字彩投注
        /// </summary>
        public CommonActionResult LotteryBetting(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");
                //var log = new List<string>();
                //log.Add("开始计时：" + userId);
                //var watch = new Stopwatch();
                //watch.Start();
                var checkError = CheckGeneralRepeatBetting(userId, info);
                if (!string.IsNullOrEmpty(checkError))
                    throw new LogicException(checkError);

                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().LotteryBetting(info, userId, balancePassword, "Bet", redBagMoney);
                //2017-12-4 更新用户推广
                BusinessHelper.ExecPlugin<IBettingLottery_AfterTranCommit>(new object[] { userId, info, info.SchemeId, keyLine });
                //}

                //watch.Stop();
                //log.Add("计时结束：" + keyLine);
                //log.Add("用时 " + watch.Elapsed.TotalMilliseconds);
                //logger.Write("LotteryBeting", userId + "-" + watch.Elapsed.TotalMilliseconds, Common.Log.LogType.Information, "投注", string.Join(Environment.NewLine, log.ToArray()));

                return new CommonActionResult(true, "数字彩投注方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
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
        /// <summary>
        /// 数字彩以代理商投注
        /// </summary>
        public CommonActionResult LotteryBettingWithAgent(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string agentToken, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckDisableGame(info.GameCode, info.AnteCodeList[0].GameType);

                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().LotteryBetting(info, userId, balancePassword, "Bet", redBagMoney);
                //}
                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IBettingLottery_AfterTranCommit>(new object[] { userId, info, schemeId, keyLine });
                return new CommonActionResult(true, "数字彩投注方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 数字彩 以userId 投注
        /// </summary>
        public CommonActionResult LotteryBettingWithUserId(LotteryBettingInfo info, string userId, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckDisableGame(info.GameCode, info.AnteCodeList[0].GameType);
                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().LotteryBetting(info, userId, "", "Bet", redBagMoney);
                //}
                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IBettingLottery_AfterTranCommit>(new object[] { userId, info, schemeId, keyLine });

                return new CommonActionResult(true, "数字彩投注方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public string WriteChaseOrderToDb()
        {
            try
            {
                return new Sports_Business().WriteChaseOrderToDb();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public string RepairChaseOrder(string gameCode, int maxCount)
        {
            try
            {
                return new Sports_Business().RepairChaseOrder(gameCode, maxCount);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 查询成功派奖列表
        /// </summary>
        public string QueryPrizedIssuseList(string gameCode, string gameType, int length, string userToken)
        {
            try
            {
                return new Sports_Business().QueryPrizedIssuseList(gameCode, gameType, length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询派奖失败列表
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="length"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public string QueryStopIssuseList(string gameCode, string gameType, int length, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryStopIssuseList(gameCode, gameType, length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存用户未购买订单
        /// </summary>
        public CommonActionResult SaveOrderLotteryBetting(LotteryBettingInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                //CheckDisableGame(info.GameCode, info.AnteCodeList[0].GameType);


                string keyLine;

                string schemeId = new Sports_Business().SaveOrderLotteryBetting(info, userId, out keyLine);

                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IBettingLottery_AfterTranCommit>(new object[] { userId, info, schemeId, keyLine });
                return new CommonActionResult(true, "保存订单成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询传统足球开奖结果
        /// </summary>
        public CTZQMatchInfo_Collection QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryCTZQMatchListByIssuseNumber(gameType, issuseNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 单式上传相关

        /// <summary>
        /// 单式上传
        /// </summary>
        public CommonActionResult SingleScheme(SingleSchemeInfo info, string password, decimal redBagMoney, string userToken)
        {
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            CheckSingleRepeatBetting(userId, info);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");


                //CheckDisableGame(info.GameCode, info.GameType);

                CheckSchemeOrder(info);

                var schemeId = new Sports_Business().SingleScheme(info, userId, password, redBagMoney);
                return new CommonActionResult(true, "单式方案提交成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
                //return new CommonActionResult
                //{
                //    IsSuccess = true,
                //    Message = "单式上传成功",
                //    ReturnValue = schemeId,
                //};
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("投注订单异常，请重试 ", ex);
            }
        }

        /// <summary>
        /// 单式上传，保存用户订单
        /// </summary>
        public CommonActionResult SaveOrderSingleScheme(SingleSchemeInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                //CheckDisableGame(info.GameCode, info.GameType);
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);

                CheckSchemeOrder(info);

                var schemeId = new Sports_Business().SaveOrderSingleScheme(info, userId);
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "保存订单成功",
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("保存订单异常，请重试 ", ex);
            }
        }
        /// <summary>
        /// 单式投注和追号
        /// </summary>
        public CommonActionResult SingleSchemeBettingAndChase(SingleSchemeInfo info, string password, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            CheckSingleRepeatBetting(userId, info);
            try
            {
                string schemeId;
                var isSuceess = true;
                var t = this.SingleScheme(info, password, redBagMoney, userToken);
                isSuceess = t.IsSuccess;
                schemeId = t.ReturnValue;
                //if (t.IsSuccess)
                //{
                //    t = SportsChase(schemeId);
                //    isSuceess = t.IsSuccess;
                //}

                return new CommonActionResult(isSuceess, isSuceess ? "单式方案提交成功" : "单式方案提交失败")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("投注订单异常，请重试 ", ex);
            }
        }
        /// <summary>
        /// 发起单式合买
        /// </summary>
        public CommonActionResult CreateSingleSchemeTogether(SingleScheme_TogetherSchemeInfo info, string balancePassword, string userToken)
        {
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.BettingInfo.GameCode.ToUpper());
            BusinessHelper.CheckGameCodeAndType(info.BettingInfo.GameCode, info.BettingInfo.GameType);
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckSingTogetherRepeatBetting(userId, info);//检查重复投注
                CheckSchemeOrder(info);
                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));
                //var list = new CacheDataBusiness().QuerySupperCreatorCollection();
                //if (list != null && list.FirstOrDefault(p => p.UserId == userId) != null)
                //    isTop = true;

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().CreateSingleSchemeTogether(info, 0, userId, balancePassword, sysGuarantees, isTop, out canChase, out stopTime, ref schemeInfo);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICreateTogether_AfterTranCommit>(new object[] { userId, schemeId, info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber, info.BettingInfo.TotalMoney, stopTime });

                //参与合买后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, schemeInfo.SoldCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });


                return new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("发起合买异常，请重试 ", ex);
            }
        }
        /// <summary>
        /// 查询单式上传全路径名
        /// </summary>
        public SingleScheme_AnteCodeQueryInfo QuerySingleSchemeFullFileName(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerySingleSchemeFullFileName(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 奖金优化投注方式

        /// <summary>
        /// 奖金优化投注
        /// </summary>
        public CommonActionResult YouHuaBet(Sports_BetingInfo info, string password, decimal realTotalMoney, decimal redBagMoney, string userToken)
        {
            try
            {
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());

                CheckJCRepeatBetting(userId, info, true);
                new Sports_Business().CheckYouHuaBetAttach(info.Attach, realTotalMoney, info.BettingCategory);

                //检查投注内容,并获取投注注数
                var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //检查投注的比赛，并获取最早结束时间
                var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);
                var schemeId = new Sports_Business().YouHuaBet(info, userId, password, realTotalMoney, totalCount, stopTime, redBagMoney);
                BusinessHelper.ExecPlugin<IBonusOptimize>(new object[] { userId, schemeId });

                //return new CommonActionResult
                //{
                //    IsSuccess = true,
                //    Message = "投注成功",
                //    ReturnValue = schemeId + "|" + realTotalMoney,
                //};
                return new CommonActionResult(true, "足彩投注方案提交成功")
                {
                    ReturnValue = schemeId + "|" + realTotalMoney,
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

        /// <summary>
        /// 奖金优化投注 和 追号
        /// </summary>
        public CommonActionResult YouHuaBetAndChase(Sports_BetingInfo info, string password, decimal realTotalMoney, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                string schemeId;
                var isSuceess = true;
                var t = this.YouHuaBet(info, password, realTotalMoney, redBagMoney, userToken);
                isSuceess = t.IsSuccess;
                schemeId = t.ReturnValue;
                //if (t.IsSuccess && !string.IsNullOrEmpty(schemeId))
                //{
                //    t = SportsChase(schemeId.Split('|')[0]);
                //    isSuceess = t.IsSuccess;
                //}

                return new CommonActionResult(isSuceess, isSuceess ? "足彩投注方案提交成功" : "足彩投注方案提交失败")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("投注订单异常，请重试 ", ex);
            }
        }

        /// <summary>
        /// 奖金优化合买
        /// </summary>
        public CommonActionResult CreateYouHuaSchemeTogether(Sports_TogetherSchemeInfo info, string balancePassword, decimal realTotalMoney, string userToken)
        {
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");
                CheckTogetherRepeatBetting(userId, info, true);//检查重复投注

                //CheckDisableGame(info.GameCode, string.Empty);

                // 检查订单基本信息
                CheckSchemeOrder(info);
                //最少认购5%
                var minMoney = realTotalMoney * 5 / 100;
                minMoney = Math.Ceiling(minMoney);
                if (info.Subscription * info.Price < minMoney)
                    throw new ArgumentException(string.Format("合买发起人认购金额必须大于等于{0}%，即：{1:N2}元。", 5, minMoney));

                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));
                //var list = new CacheDataBusiness().QuerySupperCreatorCollection();
                //if (list != null && list.FirstOrDefault(p => p.UserId == userId) != null)
                //    isTop = true;

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();
                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().CreateYouHuaTogether(info, 0, userId, balancePassword, sysGuarantees, isTop, realTotalMoney, out canChase, out stopTime, ref schemeInfo);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICreateTogether_AfterTranCommit>(new object[] { userId, schemeId, info.GameCode, info.GameType, info.IssuseNumber, realTotalMoney, stopTime });

                //参与合买后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, schemeInfo.SoldCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, realTotalMoney, schemeInfo.SchemeProgress });

                return new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + realTotalMoney,
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("发起合买异常，请重试 ", ex);
            }


        }
        #endregion

        #region 名家发布推荐

        public CommonActionResult PublishExperterScheme(Sports_BetingInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                CheckDisableGame(info.GameCode, info.GameType);

                // 检查订单基本信息
                CheckSchemeOrder(info);

                var schemeId = new Sports_Business().PublishExperterScheme(info, userId);
                //SportsChase(schemeId);
                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = "",
                    Message = "名家发布推荐成功",
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #endregion

        /// <summary>
        /// 订单投注失败返钱
        /// </summary>
        public CommonActionResult BetFail(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().BetFail(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败 " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 订单投注失败返钱 批量处理
        /// </summary>
        public CommonActionResult BetFails(string schemeId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().BetFail(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败 " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询等待追号的订单
        /// </summary>
        public string QuerySportsWaitForChaseSchemeIdArray(string gameCode, string issuseNumber, int returnCount)
        {
            try
            {
                return new Sports_Business().QuerySportsWaitForChaseSchemeIdArray(gameCode, issuseNumber, returnCount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 足彩追号
        /// </summary>
        public CommonActionResult SportsChase(string schemeId)
        {
            //lock (UsefullHelper.moneyLocker)
            //{
            var isRequestTicketSuccess = new Sports_Business().SportsChase(schemeId);
            BusinessHelper.ExecPlugin<IComplateBetting_AfterTranCommit>(new object[] { string.Empty, schemeId, isRequestTicketSuccess });
            //提交事务后
            return new CommonActionResult
            {
                IsSuccess = isRequestTicketSuccess,
                Message = isRequestTicketSuccess ? "请求出票成功" : "请求出票失败",
                ReturnValue = schemeId,
            };
            //}
        }

        /// <summary>
        /// 查询足彩方案信息
        /// </summary>
        public Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId)
        {
            try
            {
                return new Sports_Business().QuerySportsSchemeInfo(schemeId);

                //var cacheBiz = new CacheDataBusiness();
                //var info = cacheBiz.QuerySports_SchemeQueryInfo(schemeId);
                //if (info == null)
                //{
                //    info = new Sports_Business().QuerySportsSchemeInfo(schemeId);
                //    if (info != null && (info.BonusStatus == BonusStatus.Lose || (info.BonusStatus == BonusStatus.Win && info.IsPrizeMoney)))
                //    {
                //        cacheBiz.SaveSchemeInfoToXml(info);
                //    }
                //}
                //return info;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 合买中奖分配
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Sports_ComplateInfo QuerSportsComplateInfo(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerSportsComplateInfo(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 传统足球数字彩开奖
        /// </summary>
        public CommonActionResult LotteryOpen(string gameCode, string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().LotteryOpen(gameCode, issuseNumber);
                return new CommonActionResult(true, "开奖完成");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 足彩派奖
        /// </summary>
        public CommonActionResult SportsPrize(string schemeId, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney)
        {
            try
            {
                //lock (UsefullHelper.moneyLocker)
                //{
                new Sports_Business().SportsPrize(schemeId, isBonus ? BonusStatus.Win : BonusStatus.Lose, preTaxBonusMoney, afterTaxBonusMoney);
                //}
                return new CommonActionResult(true, "派奖完成");
            }
            catch (LogicException ex)
            {
                return new CommonActionResult(true, "派奖完成" + ex.Message);
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                throw new Exception(string.Format("订单{0} 派奖失败 - {1} ! ", schemeId, ex.Message), ex);
            }
        }

        /// <summary>
        /// 查询等待派钱的订单列表
        /// </summary>
        public Sports_SchemeQueryInfoCollection QueryWaitForPrizeMoneyOrderList(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryWaitForPrizeMoneyOrderList(startTime, endTime, gameCode, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询等待派钱的订单列表异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 足彩派钱
        /// </summary>
        public CommonActionResult SportsPrizeMoney(string schemeIdArray, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var array = schemeIdArray.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var list = new Sports_Business().SportsPrizeMoney(array);

                foreach (var item in list)
                {
                    BusinessHelper.ExecPlugin<IOrderPrizeMoney_AfterTranCommit>(new object[] {item.UserId, item.SchemeId, item.GameCode, item.GameType, item.IssuseNumber, 
                        item.TotalMoney, item.PreTaxBonusMoney, item.AfterTaxBonusMoney  });
                }

                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("方案派钱异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 发起足彩合买
        /// </summary>
        public CommonActionResult CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userToken)
        {
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            //栓查是否实名
            //if (!BusinessHelper.IsUserValidateRealName(userId))
            //    throw new LogicException("未实名认证用户不能购买彩票");
            //检查重复投注
            CheckTogetherRepeatBetting(userId, info);
            //CheckDisableGame(info.GameCode, info.GameType);

            // 检查订单基本信息
            CheckSchemeOrder(info);

            try
            {
                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));

                //var list = new CacheDataBusiness().QuerySupperCreatorCollection();
                //if (list != null && list.FirstOrDefault(p => p.UserId == userId) != null)
                //    isTop = true;


                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().CreateSportsTogether(info, 0, userId, balancePassword, sysGuarantees, isTop, out canChase, out stopTime, ref schemeInfo);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICreateTogether_AfterTranCommit>(new object[] { userId, schemeId, info.GameCode, info.GameType, info.IssuseNumber, info.TotalMoney, stopTime });

                //参与合买后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, schemeInfo.SoldCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });

                return new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }

            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("发起合买异常，请重试 ", ex);
            }
        }

        /// <summary>
        /// 发起合买_保存订单
        /// </summary>
        public CommonActionResult SaveOrder_CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigByKey("Site.Together.SystemGuarantees").ConfigValue);

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().SaveCreateSportsTogether(info, 0, userId, balancePassword, sysGuarantees, isTop);

                return new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询等待处理的合买订单
        /// </summary>
        public string QueryWaitToProcessingTogetherSchemeId(string gameCode, string stopTime)
        {
            try
            {
                return new Sports_Business().QueryWaitToProcessingTogetherSchemeId(gameCode, stopTime);
            }
            catch (Exception ex)
            {
                throw new Exception("查询等待处理的合买订单异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户是否已经参与了合买
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public bool IsUserJoinSportsTogether(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().IsUserJoinSportsTogether(schemeId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询足彩合买列表
        /// </summary>
        public Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherList(string key, string issuseNumber, string gameCode, string gameType,
            TogetherSchemeSecurity? security, SchemeBettingCategory? betCategory, TogetherSchemeProgress? progressState,
            decimal minMoney, decimal maxMoney, decimal minProgress, decimal maxProgress, string orderBy, int pageIndex, int pageSize, string userId)
        {
            try
            {
                //if (string.IsNullOrEmpty(userId) || !BusinessHelper.CheckIsShowHM(userId))//检查是否在白名单
                //    return new Sports_TogetherSchemeQueryInfoCollection();

                if (string.IsNullOrEmpty(orderBy))
                {
                    //orderBy = "ManYuan desc,ISTOP DESC, Progress desc,TotalMoney DESC,CreateTime desc";
                    //orderBy = "ManYuan desc,ISTOP DESC,ProgressStatus ASC,TotalMoney DESC,CreateTime desc, Progress asc";
                    orderBy = "JieZhi desc,ManYuan desc,ISTOP DESC, sold_guaran_Progress desc,TotalMoney DESC,CreateTime desc";
                }

                return new Sports_Business().QuerySportsTogetherList(key, issuseNumber, gameCode, gameType, security, betCategory, progressState, minMoney, maxMoney, minProgress, maxProgress, orderBy, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询足彩合买明细
        /// </summary>
        public Sports_TogetherSchemeQueryInfo QuerySportsTogetherDetail(string schemeId)
        {
            try
            {
                return new Sports_Business().QuerySportsTogetherDetail(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询足彩订单投注列表
        /// </summary>
        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerySportsOrderAnteCodeList(schemeId);

                //var cacheBiz = new CacheDataBusiness();
                //var collection = cacheBiz.QuerySports_AnteCodeQueryInfoCollection(schemeId);
                //if (collection == null)
                //{
                //    collection = new Sports_Business().QuerySportsOrderAnteCodeList(schemeId);
                //    var grp = collection.GroupBy(a => a.BonusStatus);
                //    if (grp.Count(g => g.Key != BonusStatus.Lose && g.Key != BonusStatus.Win) == 0 && collection.ToList().Count() > 0)
                //    {
                //        cacheBiz.SaveSchemeAnteCodeCollectionToXml(schemeId, collection);
                //    }
                //}
                //return collection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询订单票数据
        /// </summary>
        public Sports_TicketQueryInfoCollection QuerySportsTicketList(string schemeId, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var collection = new Sports_Business().QuerySchemeTicketList(schemeId, pageIndex, pageSize);

                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询参加合买的列表
        /// </summary>
        public Sports_TogetherJoinInfoCollection QuerySportsTogetherJoinList(string schemeId, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerySportsTogetherJoinList(schemeId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Sports_TogetherJoinInfoCollection QueryUserSportsTogetherJoinList(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserSportsTogetherJoinList(schemeId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询参与合买的最新中奖
        /// </summary>
        public Sports_TogetherJoinInfoCollection QueryNewBonusTogetherJoiner(int count, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryNewBonusTogetherJoiner(count);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 参与合买
        /// </summary>
        public CommonActionResult JoinSportsTogether(string schemeId, int buyCount, string joinPwd, string balancePassword, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            //bool isBet = new Sports_Business().UserIsBetting(userId);
            //if (!isBet)
            //    throw new Exception("对不起，网站已暂停彩票代购业务");
            //栓查是否实名
            //if (!BusinessHelper.IsUserValidateRealName(userId))
            //    throw new LogicException("未实名认证用户不能购买彩票");
            try
            {
                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();
                var canChase = new Sports_Business().JoinSportsTogether(schemeId, buyCount, userId, joinPwd, balancePassword, ref schemeInfo);

                //生成JsonData文件(合买大厅)
                //BusinessHelper.BuildJsonDataNotice("500");

                BusinessHelper.ExecPlugin<IAgentPayIn>(new object[] { schemeId });

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, buyCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });
                return new CommonActionResult(true, "参与合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 退出合买
        /// </summary>
        public CommonActionResult ExitTogether(string schemeId, int joinId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().ExitTogether(schemeId, joinId, userId);
                return new CommonActionResult(true, "撤销合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 撤销合买
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult CancelTogether(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().CancelTogether(schemeId, userId);
                return new CommonActionResult(true, "撤销合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 分析合买
        /// </summary>
        public CommonActionResult AnalysisSchemeTogether(string schemeId)
        {
            try
            {
                //var createUserId = string.Empty;
                //var gameCode = string.Empty;
                //var gameType = string.Empty;
                //var payBackMoney = 0M;
                //var canChase = new Sports_Business().AnalysisSchemeTogether(schemeId, out canPayBack, out createUserId, out gameCode, out gameType, out payBackMoney);
                new Sports_Business().AnalysisSchemeTogether(schemeId);
                //SportsChase(schemeId);//分析合买失败后直接撤单，不投注

                return new CommonActionResult(true, "分析合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception("分析合买异常 - " + ex.Message, ex);
            }
        }

        public string QueryFinishTogether()
        {
            try
            {
                return new Sports_Business().QueryFinishTogether();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CommonActionResult DoPayBackGuarantees(string schemeId)
        {
            try
            {
                new Sports_Business().DoPayBackGuarantees(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询中奖的战绩记录
        /// </summary>
        public UserBeedingListInfoCollection QueryBonusUserBeedingList(string userId)
        {
            try
            {
                return new Sports_Business().QueryBonusUserBeedingList(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserBeedingListInfo QueryUserBeedingListInfo(string userId, string gameCode, string gameType)
        {
            try
            {
                return new Sports_Business().QueryUserBeedingListInfo(userId, gameCode, gameType);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 查询用户战绩
        /// </summary>
        public UserBeedingListInfoCollection QueryUserBeedingList(string gameCode, string gameType, string userId, string userDisplayName, int pageIndex, int pageSize, QueryUserBeedingListOrderByProperty property, OrderByCategory category, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserBeedingList(gameCode, gameType, userId, userDisplayName, pageIndex, pageSize, property, category);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public TogetherFollowRecordInfoCollection QuerySucessFolloweRecord(string gameCode, long ruleId, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerySucessFolloweRecord(userId, ruleId, gameCode, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询 定制我的 跟单规则
        /// </summary>
        public TogetherFollowerRuleQueryInfoCollection QueryUserFollowRule(string gameCode, string gameType, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserFollowRule(false, userId, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询 我的定制 
        /// </summary>
        public TogetherFollowerRuleQueryInfoCollection QueryUserFollowRuleByCreater(string gameCode, string gameType, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserFollowRule(true, userId, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 定制向上调
        /// </summary>
        public CommonActionResult FollowRuleMoveUp(string createrUserId, string gameCode, string gameType, long ruleId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().FollowRuleMoveUp(createrUserId, gameCode, gameType, ruleId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 定制向下调
        /// </summary>
        public CommonActionResult FollowRuleMoveDown(string createrUserId, string gameCode, string gameType, long ruleId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().FollowRuleMoveDown(createrUserId, gameCode, gameType, ruleId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 将定制置顶
        /// </summary>
        public CommonActionResult FollowRuleSetTop(string createrUserId, string gameCode, string gameType, long ruleId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().FollowRuleSetTop(createrUserId, gameCode, gameType, ruleId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询定制我的跟单
        /// </summary>
        public TogetherFollowMeInfoCollection QueryUserBeFollowedReport(string gameCode, string gameType, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserBeFollowedReport(userId, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 定制合买跟单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult CustomTogetherFollower(TogetherFollowerRuleInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().CustomTogetherFollower(info);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ITogetherFollow_AfterTranCommit>(new object[] { info });
                return new CommonActionResult(true, "订制合买跟单成功");
            }
            catch (LogicException ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 编辑合买跟单
        /// </summary>
        public CommonActionResult EditTogetherFollower(TogetherFollowerRuleInfo info, long ruleId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().EditTogetherFollower(info, ruleId);
                return new CommonActionResult(true, "编辑跟单成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 退订跟单
        /// </summary>
        public CommonActionResult ExistTogetherFollower(long followerId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var rule = new Sports_Business().ExistTogetherFollower(followerId, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IExistTogetherFollow_AfterTranCommit>(new object[] { rule.CreaterUserId, rule.FollowerUserId, rule.GameCode, rule.GameType });
                return new CommonActionResult(true, "退订跟单成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询跟单信息
        /// </summary>
        public TogetherFollowerRuleQueryInfo QueryTogetherFollowerRuleInfo(string createUserId, string followerUserId, string gameCode, string gameType)
        {
            try
            {
                return new Sports_Business().QueryTogetherFollowerRuleInfo(createUserId, followerUserId, gameCode, gameType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 关注用户
        /// </summary>
        public CommonActionResult AttentionUser(string beAttentionUserId, string userToken)
        {
            // 验证用户身份及权限
            var currentUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().AttentionUser(currentUserId, beAttentionUserId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IAttention_AfterTranCommit>(new object[] { currentUserId, beAttentionUserId });
                return new CommonActionResult(true, "关注用户成功");
            }
            catch (LogicException ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 取消关注用户
        /// </summary>
        public CommonActionResult CancelAttentionUser(string beAttentionUserId, string userToken)
        {
            // 验证用户身份及权限
            var currentUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().CancelAttentionUser(currentUserId, beAttentionUserId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICancelAttention_AfterTranCommit>(new object[] { currentUserId, beAttentionUserId });
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询是否有关注
        /// </summary>
        public bool QueryIsAttention(string beAttentionUserId, string currentUserId)
        {
            try
            {
                return new Sports_Business().QueryIsAttention(currentUserId, beAttentionUserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据用户Id，查询我的关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public UserAttention_Collection QueryMyAttentionListByUserId(string userId, int pageIndex, int pageSize)
        {
            try
            {
                return new Sports_Business().QueryMyAttentionListByUserId(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询用户关注排行
        /// </summary>
        public UserAttentionSummaryInfoCollection QueryUserAttentionSummaryRank(int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserAttentionSummaryRank(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户关注列表
        /// </summary>
        public UserAttentionSummaryInfoCollection QueryUserAttentionList(string userId, int pageIndex, int pageSize, string userToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            }
            try
            {
                return new Sports_Business().QueryUserAttentionList(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询关注(被关注)用户列表
        /// </summary>
        public UserAttentionSummaryInfoCollection QueryAttentionUserList(string userId, int pageIndex, int pageSize, string userToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            }
            try
            {
                return new Sports_Business().QueryAttentionUserList(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 计算用户战绩
        /// </summary>
        public CommonActionResult ComputeUserBeedings(string complateDate)
        {
            try
            {
                new Sports_Business().ComputeUserBeedings(complateDate);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("计算用户战绩异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 计算用户幸运指数
        /// </summary>
        public CommonActionResult ComputeLucyUser()
        {
            try
            {
                new Sports_Business().ComputeLucyUser();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("计算用户幸运指数异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 计算用户中奖概率
        /// </summary>
        public CommonActionResult ComputeBonusPercent()
        {
            try
            {
                new Sports_Business().ComputeBonusPercent();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("计算中奖概率异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 修改用户战绩
        /// </summary>
        public CommonActionResult UpdateUserBeeding(string userId, string gameCode, string gameType, decimal successBettingMoney, decimal successAndBonusMoney, int successAndBonusCount,
            decimal failBettingMoney, decimal failAndBonusMoney, int failAndBonusCount, string userToken)
        {
            // 验证用户身份及权限
            var adminId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().UpdateUserBeeding(userId, gameCode, gameType, successBettingMoney, successAndBonusMoney, successAndBonusCount, failBettingMoney, failAndBonusMoney, failAndBonusCount);
                //修改缓存
                new CacheDataBusiness().UpdateProfileBeedingsByInput(userId, gameCode, gameType, successBettingMoney, successAndBonusMoney, successAndBonusCount, failBettingMoney, failAndBonusMoney, failAndBonusCount);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户当前订单列表
        /// </summary>
        public UserCurrentOrderInfoCollection QueryUserCurrentOrderList(string userId, string gameCode, string userToken, int pageIndex, int pageSize)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserCurrentOrderList(userId, gameCode, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户当前合买订单列表
        /// </summary>
        public UserCurrentOrderInfoCollection QueryUserCurrentTogetherOrderList(string userId, string gameCode, int pageIndex, int pageSize)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryUserCurrentTogetherOrderList(userId, gameCode, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据主键Id查询我的定制跟单信息
        /// </summary>
        public TogetherFollowerRuleQueryInfo QueryMyTogetherFollowerRuleById(long ruleId)
        {
            try
            {
                return new Sports_Business().QueryMyTogetherFollowerRuleById(ruleId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 奖期开奖  修改业务数据

        /// <summary>
        /// 奖期开奖
        /// 修改业务数据
        /// </summary>
        public CommonActionResult IssuseOpen(string gameCode, string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                new OpenBusiness().IssuseOpen(gameCode, issuseNumber);

                return new CommonActionResult(true, "奖期开奖完成");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}第{1}期奖期开奖失败 - {2} ! ", gameCode, issuseNumber, ex.Message), ex);
            }
        }

        #endregion

        #region 奖期派奖

        ///// <summary>
        ///// 奖期派奖
        ///// </summary>
        //public CommonActionResult IssusePrize(string gameCode, string issuseNumber, string winNumber, decimal prevTotalMoney, decimal afterTotalMoney, string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

        //    try
        //    {
        //        lock (UsefullHelper.moneyLocker)
        //        {
        //            PrizeBusiness prize = new PrizeBusiness();
        //            //修改期号信息
        //            prize.IssusePrize(gameCode, issuseNumber, winNumber);
        //            GameBizWcfServiceCache.RefreshWinNumberHistory(gameCode);
        //        }
        //        return new CommonActionResult(true, "奖期派奖完成");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("{0}第{1}期，奖期派奖失败 - {2} ! ", gameCode, issuseNumber, ex.Message), ex);
        //    }
        //}

        #endregion

        #region 订单手工返钱

        /// <summary>
        /// 订单手工返钱
        /// </summary>
        public CommonActionResult ManualPayForOrder(string orderId, decimal money, string msg, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //lock (UsefullHelper.moneyLocker)
                //{
                new PrizeBusiness().ManualPayForOrder(orderId, money, msg, userId);
                BusinessHelper.ExecPlugin<IManualPayForOrder>(new object[] { userId, orderId, money, msg });
                //}
                return new CommonActionResult(true, "订单手工返钱完成");
            }
            catch (Exception ex)
            {
                throw new Exception("订单手工返钱出错 -  " + ex.Message, ex);
            }
        }

        #endregion

        #region 手工处理订单相关

        /// <summary>
        /// 移动中的订单数据到完成订单数据
        /// </summary>
        public CommonActionResult MoveRunningOrderToComplateOrder(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().MoveRunningOrderToComplateOrder(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工设置订单中奖数据
        /// </summary>
        public CommonActionResult ManualSetOrderBonusMoney(string schemeId, decimal bonusMoney, int bonusCount, int hitMatchCount, string bonusCountDescription, string bonusCountDisplayName, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().ManualSetOrderBonusMoney(schemeId, bonusMoney, bonusCount, hitMatchCount, bonusCountDescription, bonusCountDisplayName);
                //删除缓存文件
                new CacheDataBusiness().DeleteSchemeInfoXml(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工设置订单为不中奖
        /// </summary>
        public CommonActionResult ManualSetOrderNotBonus(string schemeId, int hitMatchCount, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().ManualSetOrderNotBonus(schemeId, hitMatchCount);
                //删除缓存文件
                new CacheDataBusiness().DeleteSchemeInfoXml(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 删除订单缓存
        /// </summary>
        public CommonActionResult ManualDeleteOrderCache(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //删除缓存文件
                new CacheDataBusiness().DeleteSchemeInfoXml(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 修改订单状态为已派奖
        /// </summary>
        /// <param name="schemeId"></param>
        public CommonActionResult UpdateOrderPrizeMoney(string schemeId)
        {
            try
            {
                new Sports_Business().UpdateOrderPrizeMoney(schemeId);
                return new CommonActionResult(true, "修改订单派奖状态成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        private void CheckSchemeOrder(Sports_BetingInfo info)
        {
            if (info.AnteCodeList.Count == 0)
                throw new ArgumentException("未选择任何比赛或者投注号码");
            if (info.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.GameType != null && info.GameType.ToUpper() != "HH")
            {
                if (info.AnteCodeList != null)
                {
                    foreach (var item in info.AnteCodeList)
                    {
                        if (item.GameType != null)
                        {
                            if (item.GameType.ToUpper() != info.GameType.ToUpper())
                                throw new Exception("彩种玩法有误，应该是:" + BusinessHelper.FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + BusinessHelper.FormatGameType(info.GameCode, item.GameType));
                        }
                    }
                }
            }
        }
        private void CheckSchemeOrder(Sports_TogetherSchemeInfo info)
        {
            if (info.AnteCodeList.Count == 0)
                throw new ArgumentException("未选择任何比赛或者投注号码");
            if (info.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.TotalCount <= 0)
                throw new ArgumentException("合买总份数不能小于0");
            var allowGameCodeArray = "SSQ,DLT,FC3D,PL3,CTZQ,BJDC,JCZQ,JCLQ".Split(',');
            if (!allowGameCodeArray.Contains(info.GameCode.ToUpper()))
                throw new LogicException("当前彩种不支持合买投注");
            var maxDeduct = 10;
            if (info.BonusDeduct > maxDeduct || info.BonusDeduct < 0)
                throw new LogicException("合买提成有误，请确认后重新提交");
            if (info.BettingCategory != SchemeBettingCategory.YouHua)
            {
                var minMoney = info.TotalMoney * 5 / 100;
                minMoney = Math.Ceiling(minMoney);
                if (info.Subscription * info.Price < minMoney)
                    throw new ArgumentException(string.Format("合买发起人认购金额必须大于等于{0}%，即：{1:N2}元。", 5, minMoney));
            }
            //if (info.GameType != null && info.GameType.ToUpper() != "HH")
            //{
            //    if (info.AnteCodeList != null)
            //    {
            //        foreach (var item in info.AnteCodeList)
            //        {
            //            if (item.GameType != null)
            //            {
            //                if (item.GameType.ToUpper() != info.GameType.ToUpper())
            //                    throw new Exception("彩种玩法有误，应该是:" + BusinessHelper.FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + BusinessHelper.FormatGameType(info.GameCode, item.GameType));
            //            }
            //        }
            //    }
            //}

        }
        private void CheckSchemeOrder(SingleSchemeInfo info)
        {
            if (info.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.AllowCodes == null || info.AllowCodes.Length == 0)
                throw new ArgumentException("允许的投注号码不能为空");
            if (info.FileBuffer.Length == 0)
                throw new ArgumentException("上传的文件字节不能为0");
            //if (string.IsNullOrEmpty(info.AnteCodeFullFileName))
            //    throw new ArgumentException("上传的文件路径不能为空");
            if (info.GameType != null && info.GameType.ToUpper() != "HH")
            {
                if (info.AnteCodeList != null)
                {
                    foreach (var item in info.AnteCodeList)
                    {
                        if (item.GameType != null)
                        {
                            if (item.GameType.ToUpper() != info.GameType.ToUpper())
                                throw new Exception("彩种玩法有误，应该是:" + BusinessHelper.FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + BusinessHelper.FormatGameType(info.GameCode, item.GameType));
                        }
                    }
                }
            }
        }
        private void CheckSchemeOrder(SingleScheme_TogetherSchemeInfo info)
        {
            var allowGameCodeArray = "SSQ,DLT,FC3D,PL3,CTZQ,BJDC,JCZQ,JCLQ".Split(',');
            if (!allowGameCodeArray.Contains(info.BettingInfo.GameCode.ToUpper()))
                throw new LogicException("当前彩种不支持合买投注");
            var maxDeduct = 10;
            if (info.BonusDeduct > maxDeduct || info.BonusDeduct < 0)
                throw new LogicException("合买提成有误，请确认后重新提交");

            if (info.BettingInfo.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.BettingInfo.AllowCodes == null || info.BettingInfo.AllowCodes.Length == 0)
                throw new ArgumentException("允许的投注号码不能为空");
            //if (string.IsNullOrEmpty(info.BettingInfo.AnteCodeFullFileName))
            //    throw new ArgumentException("上传的文件路径不能为空");
            if (info.BettingInfo.FileBuffer.Length == 0)
                throw new ArgumentException("上传的文件字节不能为0");
            if (info.TotalCount <= 0)
                throw new ArgumentException("合买总份数不能小于0");

            var minMoney = info.TotalMoney * 5 / 100;
            minMoney = Math.Ceiling(minMoney);
            if (info.Subscription * info.Price < minMoney)
                throw new ArgumentException(string.Format("合买发起人认购金额必须大于等于{0}%，即：{1:N2}元。", 5, minMoney));


            if (info.BettingInfo.GameType != null && info.BettingInfo.GameType.ToUpper() != "HH")
            {
                if (info.BettingInfo.AnteCodeList != null)
                {
                    foreach (var item in info.BettingInfo.AnteCodeList)
                    {
                        if (item.GameType != null)
                        {
                            if (item.GameType.ToUpper() != info.BettingInfo.GameType.ToUpper())
                                throw new Exception("彩种玩法有误，应该是:" + BusinessHelper.FormatGameType(info.BettingInfo.GameCode, info.BettingInfo.GameType) + ",但实际是:" + BusinessHelper.FormatGameType(info.BettingInfo.GameCode, item.GameType));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查询投注号码
        /// </summary>
        public System.Data.DataSet QueryBettingAnteCode(DateTime startTime, DateTime endTime, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryBettingAnteCode(startTime, endTime);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户保存的订单信息
        /// </summary>
        public SaveOrder_LotteryBettingInfoCollection QuerySaveOrder_Lottery(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerySaveOrderLottery(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询未投注的订单
        /// </summary>
        public string QueryUnBetOrder(string time)
        {
            try
            {
                return new Sports_Business().QueryUnBetOrder(time);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 购买用户保存订单
        /// </summary>
        public CommonActionResult BettingUserSavedOrder(string schemeId, string balancePassword, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().BettingUserSavedOrder(schemeId, userId, balancePassword, redBagMoney);

                return new CommonActionResult(true, "投注成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 自动投注的用户保存订单
        /// </summary>
        public CommonActionResult AutoBetUserSaveOrder(string schemeId)
        {
            try
            {
                new Sports_Business().AutoBetUserSaveOrder(schemeId);
                return new CommonActionResult(true, "投注成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工修改中奖状态
        /// </summary>
        public CommonActionResult UpdateSchemeTicket(string ticketId, BonusStatus bonusStatus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().UpdateSchemeTicket(ticketId, bonusStatus, preTaxBonusMoney, afterTaxBonusMoney);
                return new CommonActionResult(true, "修改票数据成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 将接受到的通知信息存入表
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userToken"></param>
        public void AddReceiveNoticeLog(ReceiveNoticeLogInfo info)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            new Sports_Business().AddReceiveNoticeLog(info);
        }

        public string QueryReceiveNoticeList(int returnRecord = 0)
        {
            try
            {
                return new Sports_Business().QueryReceiveNoticeList(returnRecord);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 退票
        /// </summary>
        public CommonActionResult PayBackTicket(string ticketId)
        {
            try
            {
                var info = new Sports_Business().PayBackTicket(ticketId);

                BusinessHelper.ExecPlugin<IPayBackTicket>(new object[] { info.SchemeId, info.TicketId, info.TotalMoney });

                return new CommonActionResult(true, "退票完成");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 查询接收通知列表
        /// </summary>
        public ReceiveNoticeLogInfo_Collection QueryReceiveNoticeLogList(int noticeType, DateTime startTiem, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            return new Sports_Business().QueryReceiveNoticeLogList(noticeType, startTiem, endTime, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询历史接收通知列表
        /// </summary>
        public ReceiveNoticeLogInfo_Collection QueryComplateReceiveNoticeLogList(int noticeType, DateTime startTiem, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            return new Sports_Business().QueryComplateReceiveNoticeLogList(noticeType, startTiem, endTime, pageIndex, pageSize);
        }

        /// <summary>
        /// 重发通知
        /// </summary>
        public CommonActionResult ReSendNotice(string receiveId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().MoveComplateNotice(long.Parse(receiveId));
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string QueryWaitingTicket(int returnRecord)
        {
            try
            {
                return new Sports_Business().QueryWaitingTicket(returnRecord);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public CommonActionResult ExecCacheOrderChase(string schemeId)
        {
            try
            {
                this.SportsChase(schemeId);
                return new CommonActionResult(true, "执行完成");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public CommonActionResult TestWCFFunction(TestInfo info)
        {
            var txt = Encoding.UTF8.GetString(info.Buffer);

            return new CommonActionResult(true, txt);
        }

        #region 红人相关

        /// <summary>
        /// 查询红人的合买订单
        /// </summary>
        public TogetherHotUserInfoCollection QueryHotUserTogetherOrderList(string userId)
        {
            //if (string.IsNullOrEmpty(userId) || !BusinessHelper.CheckIsShowHM(userId))//检查是否在白名单
            //    return new TogetherHotUserInfoCollection();
            return new Sports_Business().QueryHotUserTogetherOrderList();
        }

        /// <summary>
        /// 添加红人
        /// </summary>
        public CommonActionResult AddTogetherHotUser(string userId)
        {
            try
            {
                new Sports_Business().AddTogetherHotUser(userId);
                return new CommonActionResult(true, "添加成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 删除合买红人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CommonActionResult DeleteTogetherHotUser(string userId)
        {
            try
            {
                new Sports_Business().DeleteTogetherHotUser(userId);
                return new CommonActionResult(true, "删除合买红人成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 方案快照查询 2014.11.24 dj

        /// <summary>
        /// 快照_查询竞彩投注信息
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public OrderSnapshotDetailInfo_JC_Collection QueryJCBettingSnapshotInfo(string schemeId, string gameCode)
        {
            try
            {
                return new Sports_Business().QueryJCBettingSnapshotInfo(schemeId, gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 快照_查询普通投注信息
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public OrderSnapshotDetailInfo_PT_Collection QueryPTBettingSnapshotInfo(string schemeId)
        {
            try
            {
                return new Sports_Business().QueryPTBettingSnapshotInfo(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 快照_查询追号投注信息
        /// </summary>
        /// <param name="KeyLine"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public OrderSnapshotDetailInfo_PT_Collection QueryChaseBettingSnapshotInfo(string KeyLine)
        {
            try
            {
                return new Sports_Business().QueryChaseBettingSnapshotInfo(KeyLine);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 快照_查询合买投注信息
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public OrderSnapshotDetailInfo_Together_Collection QueryTogetherBettingSnapshotInfo(string schemeId)
        {
            try
            {
                return new Sports_Business().QueryTogetherBettingSnapshotInfo(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// 查询传统足球奖等
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="issuseNumber">期号</param>
        /// <param name="hitMatch">场次</param>
        /// <returns></returns>
        public int GetHitMatchCount(string gameCode, string gameType, string issuseNumber, int hitMatch)
        {
            try
            {
                return new Sports_Business().GetHitMatchCount(gameCode, gameType, issuseNumber, hitMatch);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 投注测试函数


        /// <summary>
        /// (测试)足彩普通投注
        /// </summary>
        public CommonActionResult Test_SportsBetting(Sports_BetingInfo info, bool isVirtualOrder, string userId)
        {
            try
            {
                CheckDisableGame(info.GameCode, info.GameType);

                // 检查订单基本信息
                CheckSchemeOrder(info);

                string schemeId;
                //lock (UsefullHelper.moneyLocker)
                //{
                schemeId = new Sports_Business().Test_SportsBetting(info, userId, isVirtualOrder);
                this.SportsChase(schemeId);
                //}

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    //ReturnValue = schemeId,
                    Message = "足彩投注成功",
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// (测试)数字彩投注
        /// </summary>
        public CommonActionResult Test_LotteryBetting(LotteryBettingInfo info, bool isVirtualOrder, string userId)
        {
            try
            {
                CheckDisableGame(info.GameCode, info.AnteCodeList[0].GameType);

                string schemeId, keyLine;

                schemeId = new Sports_Business().Test_LotteryBetting(info, userId, isVirtualOrder, out keyLine);
                this.SportsChase(schemeId);

                return new CommonActionResult(true, "数字彩投注方案提交成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// (测试)发起合买
        /// </summary>
        /// <param name="info"></param>
        /// <param name="balancePassword"></param>
        /// <param name="userId"></param>
        /// <param name="isVirtualOrder"></param>
        /// <returns></returns>
        public CommonActionResult Test_CreateSportsTogether(Sports_TogetherSchemeInfo info, bool isVirtualOrder, string userId)
        {
            try
            {
                CheckDisableGame(info.GameCode, info.GameType);

                // 检查订单基本信息
                CheckSchemeOrder(info);

                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().Test_CreateSportsTogether(info, 0, userId, string.Empty, sysGuarantees, isTop, isVirtualOrder, out canChase, out stopTime, ref schemeInfo);
                this.SportsChase(schemeId);

                return new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 先发起后上传

        /// <summary>
        /// 先发起后上传_发起合买
        /// </summary>
        public CommonActionResult XianFaQiHSC_CreateSportsTogether(SingleScheme_TogetherSchemeInfo info, string balancePassword, string userToken)
        {
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            //检查用户余额是否足够
            BusinessHelper.CheckBalance(userId);
            CheckSingTogetherRepeatBetting(userId, info);//检查重复投注

            try
            {
                BusinessHelper.CheckGameCodeAndType(info.BettingInfo.GameCode, info.BettingInfo.GameType);

                CheckDisableGame(info.BettingInfo.GameCode, info.BettingInfo.GameType);
                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().XianFaQiHSC_CreateSportsTogether(info, 0, userId, balancePassword, sysGuarantees, isTop, out canChase, out stopTime, ref schemeInfo);

                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<ICreateTogether_AfterTranCommit>(new object[] { userId, schemeId, info.GameCode, info.GameType, info.IssuseNumber, info.TotalMoney, stopTime });

                ////参与合买后
                //BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, schemeInfo.SoldCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });

                return new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 先发起后上传
        /// </summary>
        public CommonActionResult XianFaQiHSC_JoinSportsTogether(string schemeId, int buyCount, string joinPwd, string balancePassword, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();
                var canChase = new Sports_Business().XianFaQiHSC_JoinSportsTogether(schemeId, buyCount, userId, joinPwd, balancePassword, ref schemeInfo);

                //生成JsonData文件(合买大厅)
                //BusinessHelper.BuildJsonDataNotice("500");

                //BusinessHelper.ExecPlugin<IAgentPayIn>(new object[] { schemeId });

                ////! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, buyCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });
                return new CommonActionResult(true, "参与合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 追加保底
        /// </summary>
        public CommonActionResult XianFaQi_AddGuarantees(string schemeId, int buyCount, string userId, string balancePassword)
        {
            try
            {
                var canChase = new Sports_Business().XianFaQi_AddGuarantees(schemeId, buyCount, userId, balancePassword);


                //生成JsonData文件(合买大厅)
                //BusinessHelper.BuildJsonDataNotice("500");

                return new CommonActionResult(true, "追加保底成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 先发起后上传_上传方案
        /// </summary>
        public CommonActionResult XianFaQi_UpLoadScheme(string schemeId, SingleScheme_TogetherSchemeInfo info, string userId)
        {
            try
            {
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.BettingInfo.GameCode.ToUpper());
                //检查用户余额是否足够
                BusinessHelper.CheckBalance(userId);
                CheckDisableGame(info.BettingInfo.GameCode, info.BettingInfo.GameType);
                var canChase = new Sports_Business().XianFaQi_UpLoadScheme(schemeId, info);
                return new CommonActionResult(true, "上传方案成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 分析先发起后上传合买
        /// </summary>
        public CommonActionResult AnalysisSchemeXianFaQiHSC(string schemeId)
        {
            try
            {
                //var canChase = new Sports_Business().AnalysisSchemeXianFaQiHSC(schemeId);
                return new CommonActionResult(true, "分析先发起后上传合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception("分析先发起后上传合买异常 - " + ex.Message, ex);
            }
        }
        public string QueryWaitToProcessingXianFaQiHSCOrderList(string gameCode, string stopTime)
        {
            try
            {
                return new Sports_Business().QueryWaitToProcessingXianFaQiHSCOrderList(gameCode, stopTime);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// 发送短信
        /// </summary>
        public bool SendMsg(string mobile, string content, string ip, int msgType, string userId, string schemeId, int msgId = 0)
        {
            try
            {
                return BusinessHelper.SendMsg(mobile, content, ip, msgType, userId, schemeId, msgId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询短信余额
        /// </summary>
        /// <returns></returns>
        public string GetSMSBalance()
        {
            try
            {
                return BusinessHelper.GetSMSBalance();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region APP相关函数

        /// <summary>
        /// 根据中奖状态查询投注列表
        /// </summary>
        public BettingOrderInfoCollection QueryOrderListByBonusState(string strSate, string userId, string strSchemeType, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new Sports_Business().QueryOrderListByBonusState(strSate, userId, strSchemeType, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// 查询不在投注时间段内订单，传统足球除外
        /// </summary>
        /// <returns></returns>
        public string QueryTicketAbnormalOrderId()
        {
            try
            {
                return new Sports_Business().QueryTicketAbnormalOrderId();
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 修改是否暂停投注标识
        /// </summary>
        public CommonActionResult UpdateCoreConfigInfoByKey(string configKey, string configValue)
        {
            try
            {
                new Sports_Business().UpdateCoreConfigInfo(configKey, configValue);
                return new CommonActionResult(true, "修改信息成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 根据key查询配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CoreConfigInfo QueryConfigByKey(string key)
        {
            try
            {
                return new Sports_Business().QueryConfigByKey(key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public HitMatchInfo GetHitMatchCount_YouHua(string gameCode, string gameType, int length)
        {
            try
            {
                return new Sports_Business().GetHitMatchCount_YouHua(gameCode, gameType, length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 控制频繁订单提交


        #region 检查普通订单

        /// <summary>
        /// 普通订单缓存数据
        /// </summary>
        private static Dictionary<string, LotteryBettingInfo> _bettingListInfo = new Dictionary<string, LotteryBettingInfo>();

        /// <summary>
        /// 检查普通订单频繁投注
        /// </summary>
        private string CheckGeneralRepeatBetting(string currUserId, LotteryBettingInfo info)
        {
            try
            {
                //todo:备用 info.IsSubmit = false;
                if (_bettingListInfo == null || !_bettingListInfo.ContainsKey(currUserId))
                {
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                var cacheInfo = _bettingListInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.SchemeSource == info.SchemeSource && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
                if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                {
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                if (!info.Equals(cacheInfo.Value))
                {
                    //不重复
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                //投注内容相同
                if (info.IsRepeat)
                {
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                if (timeSpan.TotalSeconds > 5)
                {
                    //大于间隔时间
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                return "Repeat";
            }
            catch (Exception)
            {
                _bettingListInfo.Clear();
                return string.Empty;
            }
        }

        #endregion

        #region 检查竞彩和优化

        /// <summary>
        /// 竞彩足球缓存数据
        /// </summary>
        //private static Dictionary<string, Sports_BetingInfo> _sportsBettingListInfo = new Dictionary<string, Sports_BetingInfo>();
        /// <summary>
        /// 竞彩足球缓存数据
        /// </summary>
        private static System.Collections.Concurrent.ConcurrentDictionary<string, Sports_BetingInfo> _sportsBettingListInfo = new System.Collections.Concurrent.ConcurrentDictionary<string, Sports_BetingInfo>();

        /// <summary>
        /// 检查竞彩订单频繁投注
        /// </summary>
        private void CheckJCRepeatBetting(string currUserId, Sports_BetingInfo info, bool isYouHua = false)
        {
            try
            {
                if (!_sportsBettingListInfo.ContainsKey(currUserId))
                {
                    info.CurrentBetTime = DateTime.Now;
                    _sportsBettingListInfo.TryAdd(currUserId, info);
                    return;
                }
            }
            catch (Exception)
            {

            }
            lock (_sportsBettingListInfo)
            {
                try
                {
                    Sports_BetingInfo value = _sportsBettingListInfo[currUserId];
                    if (isYouHua)//奖金优化
                    {
                        //不重复
                        if (!info.Equals(value))
                        {
                            _sportsBettingListInfo.TryRemove(currUserId, out value);
                            info.CurrentBetTime = DateTime.Now;
                            _sportsBettingListInfo.TryAdd(currUserId, info);
                            return;
                        }
                        //重复投注
                        if (value.Amount == info.Amount && value.GameCode.ToUpper() == info.GameCode.ToUpper() && value.PlayType == info.PlayType && value.TotalMoney == info.TotalMoney && value.Attach == info.Attach)
                        {
                            info.IsRepeat = true;
                        }
                        //重复投注
                        if (info.IsRepeat)
                        {
                            var timeSpan = DateTime.Now - value.CurrentBetTime;
                            if (timeSpan.TotalSeconds > 5)
                            {
                                //大于间隔时间
                                _sportsBettingListInfo.TryRemove(currUserId, out value);
                                info.CurrentBetTime = DateTime.Now;
                                _sportsBettingListInfo.TryAdd(currUserId, info);
                                return;
                            }
                            else
                            {
                                throw new LogicException("Repeat");
                            }
                        }
                    }
                    else
                    {
                        //不重复
                        if (!info.Equals(value))
                        {
                            _sportsBettingListInfo.TryRemove(currUserId, out value);
                            info.CurrentBetTime = DateTime.Now;
                            _sportsBettingListInfo.TryAdd(currUserId, info);
                            return;
                        }
                        //重复投注
                        if (value.Amount == info.Amount && value.GameCode.ToUpper() == info.GameCode.ToUpper() && value.PlayType == info.PlayType && value.TotalMoney == info.TotalMoney)
                        {
                            info.IsRepeat = true;
                        }
                        //重复投注
                        if (info.IsRepeat)
                        {
                            var timeSpan = DateTime.Now - value.CurrentBetTime;
                            if (timeSpan.TotalSeconds > 5)
                            {
                                //大于间隔时间
                                _sportsBettingListInfo.TryRemove(currUserId, out value);
                                info.CurrentBetTime = DateTime.Now;
                                _sportsBettingListInfo.TryAdd(currUserId, info);
                                return;
                            }
                            else
                            {
                                throw new LogicException("Repeat");
                            }
                        }
                    }
                }
                catch
                {
                    _sportsBettingListInfo.Clear();
                    return;
                }
            }
        }

        #endregion

        #region 检查普通单式

        /// <summary>
        /// 单式缓存数据
        /// </summary>
        private static Dictionary<string, SingleSchemeInfo> _singleSchemeListInfo = new Dictionary<string, SingleSchemeInfo>();

        /// <summary>
        /// 检查单式订单频繁投注
        /// </summary>
        private void CheckSingleRepeatBetting(string currUserId, SingleSchemeInfo info)
        {
            try
            {
                if (_singleSchemeListInfo == null || !_singleSchemeListInfo.ContainsKey(currUserId))
                {
                    info.CurrentBetTime = DateTime.Now;
                    _singleSchemeListInfo.Add(currUserId, info);
                    return;
                }
                var cacheInfo = _singleSchemeListInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.SchemeSource == info.SchemeSource && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
                if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                {
                    _singleSchemeListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _singleSchemeListInfo.Add(currUserId, info);
                    return;
                }
                if (!info.Equals(cacheInfo.Value))
                {
                    //不重复
                    _singleSchemeListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _singleSchemeListInfo.Add(currUserId, info);
                    return;
                }
                //投注内容相同
                if (info.IsRepeat)
                {
                    _singleSchemeListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _singleSchemeListInfo.Add(currUserId, info);
                    return;
                }
                var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                if (timeSpan.TotalSeconds > 5)
                {
                    //大于间隔时间
                    _singleSchemeListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _singleSchemeListInfo.Add(currUserId, info);
                    return;
                }
            }
            catch (Exception)
            {
                _singleSchemeListInfo.Clear();
                return;
            }
            throw new LogicException("Repeat");
        }
        #endregion

        #region 检查数字彩、竞彩、优化合买

        /// <summary>
        /// 合买订单缓存
        /// </summary>
        private static Dictionary<string, Sports_TogetherSchemeInfo> _togetherSchemeInfo = new Dictionary<string, Sports_TogetherSchemeInfo>();

        /// <summary>
        /// 检查合买订单频繁投注
        /// </summary>
        private void CheckTogetherRepeatBetting(string currUserId, Sports_TogetherSchemeInfo info, bool isYouHua = false)
        {
            if (_togetherSchemeInfo == null || !_togetherSchemeInfo.ContainsKey(currUserId))
            {
                info.CurrentBetTime = DateTime.Now;
                _togetherSchemeInfo.Add(currUserId, info);
                return;
            }

            if (isYouHua)//奖金优化
            {
                try
                {
                    var cacheInfo = _togetherSchemeInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.Amount == info.Amount && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.PlayType == info.PlayType && s.Value.TotalMoney == info.TotalMoney && s.Value.Attach == info.Attach);
                    if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    if (!info.Equals(cacheInfo.Value))
                    {
                        //不重复
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    //投注内容相同
                    if (info.IsRepeat)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                    if (timeSpan.TotalSeconds > 5)
                    {
                        //大于间隔时间
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                }
                catch (Exception)
                {
                    _togetherSchemeInfo.Clear();
                    return;
                }
                throw new LogicException("Repeat");
            }
            else
            {
                try
                {
                    var cacheInfo = _togetherSchemeInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.SchemeSource == info.SchemeSource && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
                    if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    if (!info.Equals(cacheInfo.Value))
                    {
                        //不重复
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    //投注内容相同
                    if (info.IsRepeat)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                    if (timeSpan.TotalSeconds > 5)
                    {
                        //大于间隔时间
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                }
                catch (Exception)
                {
                    _togetherSchemeInfo.Clear();
                    return;
                }
                throw new LogicException("Repeat");
            }
        }

        #endregion

        #region 单式合买

        /// <summary>
        /// 单式合买
        /// </summary>
        private static Dictionary<string, SingleScheme_TogetherSchemeInfo> _singTogether = new Dictionary<string, SingleScheme_TogetherSchemeInfo>();

        /// <summary>
        /// 检查合买订单频繁投注
        /// </summary>
        private void CheckSingTogetherRepeatBetting(string currUserId, SingleScheme_TogetherSchemeInfo info)
        {
            try
            {

                if (_singTogether == null || !_singTogether.ContainsKey(currUserId))
                {
                    info.BettingInfo.CurrentBetTime = DateTime.Now;
                    _singTogether.Add(currUserId, info);
                    return;
                }
                var cacheInfo = _singTogether.FirstOrDefault(s => s.Key == currUserId && s.Value.BettingInfo.GameCode == info.BettingInfo.GameCode.ToUpper() && s.Value.BettingInfo.SchemeSource == info.BettingInfo.SchemeSource && s.Value.BettingInfo.BettingCategory == info.BettingInfo.BettingCategory);
                if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                {
                    _singTogether.Remove(currUserId);
                    info.BettingInfo.CurrentBetTime = DateTime.Now;
                    _singTogether.Add(currUserId, info);
                    return;
                }
                if (!info.Equals(cacheInfo.Value))
                {
                    //不重复
                    _singTogether.Remove(currUserId);
                    info.BettingInfo.CurrentBetTime = DateTime.Now;
                    _singTogether.Add(currUserId, info);
                    return;
                }
                //投注内容相同
                if (info.BettingInfo.IsRepeat)
                {
                    _singTogether.Remove(currUserId);
                    info.BettingInfo.CurrentBetTime = DateTime.Now;
                    _singTogether.Add(currUserId, info);
                    return;
                }
                var timeSpan = DateTime.Now - cacheInfo.Value.BettingInfo.CurrentBetTime;
                if (timeSpan.TotalSeconds > 5)
                {
                    //大于间隔时间
                    _singTogether.Remove(currUserId);
                    info.BettingInfo.CurrentBetTime = DateTime.Now;
                    _singTogether.Add(currUserId, info);
                    return;
                }
            }
            catch (Exception)
            {
                _singTogether.Clear();
                return;
            }
            throw new LogicException("Repeat");
        }

        #endregion

        #endregion

        #region 宝单分享


        /// <summary>
        /// 宝单分享-创建宝单
        /// </summary>
        public CommonActionResult SaveOrderSportsBetting_DBFX(Sports_BetingInfo info, string userId)
        {
            try
            {
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());

                // 检查订单基本信息
                CheckSchemeOrder(info);

                string schemeId = new Sports_Business().SaveOrderSportsBetting_DBFX(info, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "保存订单成功",
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 宝单分享-抄单
        /// </summary>
        public CommonActionResult SportsBetting_BDFX(Sports_BetingInfo info, string password, string userId)
        {
            try
            {
                //检查用户余额是否足够
                BusinessHelper.CheckBalance(userId);

                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                //检查混合串关方式
                BusinessHelper.CheckHHPlayType(info.GameCode, info.GameType, info.PlayType, info.AnteCodeList);

                CheckJCRepeatBetting(userId, info);
                BusinessHelper.ExecPlugin<ICheckUserIsBetting_BeforeTranBegin>(new object[] { userId, info.TotalMoney });//检查当前用户是否可投注

                string schemeId = string.Empty;

                // 检查订单基本信息
                CheckSchemeOrder(info);
                //lock (UsefullHelper.moneyLocker)
                //{
                schemeId = new Sports_Business().SportsBetting_BDFX(info, userId, password, "Bet");
                //}
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });
                return new CommonActionResult { IsSuccess = true, Message = "订单提交成功", ReturnValue = string.Format("{0}|{1}", schemeId, info.TotalMoney) };
                //return new CommonActionResult
                //{
                //    IsSuccess = true,
                //    ReturnValue = schemeId + "|" + info.TotalMoney,
                //    Message = "足彩投注成功",
                //};
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
        ///// <summary>
        ///// 宝单分享-抄单
        ///// </summary>
        //public CommonActionResult Sports_BettingAndChase_BDFX(Sports_BetingInfo info, string password, string userId)
        //{
        //    // 验证用户身份及权限
        //    //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
        //    try
        //    {
        //        var isSuceess = true;
        //        var t = this.SportsBetting_BDFX(info, password, userId);
        //        isSuceess = t.IsSuccess;
        //        var schemeId = string.Empty;
        //        var money = 0M;
        //        var array = t.ReturnValue.Split('|');
        //        if (array.Length == 2)
        //        {
        //            schemeId = array[0];
        //            money = decimal.Parse(array[1]);
        //        }
        //        return new CommonActionResult { IsSuccess = isSuceess, Message = "订单提交成功", ReturnValue = string.Format("{0}|{1}", schemeId, money) };
        //    }
        //    catch (AggregateException ex)
        //    {
        //        throw new AggregateException(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //}
        /// <summary>
        /// 根据订单编号查询票数据
        /// </summary>
        public Sports_TicketQueryInfoCollection QueryTicketListBySchemeId(string schemeId)
        {
            try
            {
                return new Sports_Business().QueryTicketListBySchemeId(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion
        /// <summary>
        /// 字符串64位编码
        /// </summary>
        public string EncodeBase64(string source)
        {
            try
            {
                return BusinessHelper.EncodeBase64(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 字符串解码
        /// </summary>
        public string DecodeBase64(string source)
        {
            try
            {
                return BusinessHelper.DecodeBase64(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询网站服务项
        /// </summary>
        public UserSiteServiceInfo QueryUserSiteServiceById(int Id)
        {
            try
            {
                return new UserBusiness().QueryUserSiteServiceById(Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 修改网站服务项
        /// </summary>
        /// <param name="info"></param>
        public CommonActionResult UpdateSiteService(UserSiteServiceInfo info)
        {
            try
            {
                new UserBusiness().UpdateSiteService(info);
                return new CommonActionResult(true, "修改成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 根据用户编号查询服务项
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserSiteServiceInfo QueryUserSiteServiceByUserId(string userId)
        {
            try
            {
                return new UserBusiness().QueryUserSiteServiceByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 虚拟奖金优化投注
        /// </summary>
        public CommonActionResult VirtualOrderYouHuaBet(Sports_BetingInfo info, decimal realTotalMoney, string userToken)
        {
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());

            //对附件字符做验证
            new Sports_Business().CheckYouHuaBetAttach(info.Attach, realTotalMoney, info.BettingCategory);
            try
            {
                // 验证用户身份及权限
                BusinessHelper.ExecPlugin<ICheckUserIsBetting_BeforeTranBegin>(new object[] { userId, info.TotalMoney });//检查当前用户是否可投注

                //检查投注内容,并获取投注注数
                var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //检查投注的比赛，并获取最早结束时间
                var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);
                var schemeId = new Sports_Business().VirtualOrderYouHuaBet(info, userId, realTotalMoney, totalCount, stopTime);
                BusinessHelper.ExecPlugin<IBonusOptimize>(new object[] { userId, schemeId });
                return new CommonActionResult(true, "奖金优化投注方案提交成功")
                {
                    ReturnValue = schemeId + "|" + realTotalMoney,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #region 新流程设计

        #region 高频彩开启奖期

        private int GetDayIndex(DateTime date)
        {
            var dateIndex = 0;
            var currentDay = new DateTime(date.Year, 1, 1);
            for (var i = 0; i < date.DayOfYear; i++)
            {
                currentDay = currentDay.AddDays(1);
                if (CheckIsOpenDay(currentDay))
                {
                    dateIndex++;
                }
            }
            return dateIndex;
        }

        private bool CheckIsOpenDay(DateTime date)
        {
            var calendar = new ChineseLunisolarCalendar();
            var lMonth = calendar.GetMonth(date);
            var lDay = calendar.GetDayOfMonth(date);
            if (lMonth == 1 && lDay < 7)
            {
                return false;
            }
            else
            {
                date = date.AddDays(1);
                lMonth = calendar.GetMonth(date);
                lDay = calendar.GetDayOfMonth(date);
                if (lMonth == 1 && lDay == 1)
                {
                    return false;
                }
            }
            return true;
        }

        // 批量开启重庆时时彩奖期（每天120期。白天10分钟一期(上午10点开始)，夜场5分钟一期(22点开始)）
        public CommonActionResult OpenIssuseBatch_Fast_CQSSC(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "CQSSC";
            var bettingOffset = 90;
            var phases = new Dictionary<int, double>();
            phases.Add(23, 5);
            phases.Add(0, 475);
            phases.Add(96, 10);
            phases.Add(120, 5);
            var issuseFormat = "{0:yyyyMMdd}-{1,3:D3}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启江西时时彩奖期（每天84期。10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_JXSSC(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "JXSSC";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 540.5);
            phases.Add(84, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, (i, offset) =>
                {
                    if (i == 1)
                    {
                        return offset;
                    }
                    else if (i == 2)
                    {
                        return offset.AddSeconds(14);
                    }
                    else if (i % 5 == 2 && i > 5)
                    {
                        return offset.AddSeconds(10);
                    }
                    else
                    {
                        return offset.AddSeconds(11);
                    }
                });
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启山东11选5奖期（每天78期。12分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_SD11X5(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "SD11X5";
            var bettingOffset = 40;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 506);
            phases.Add(87, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启广东11选5奖期（每天70期。12分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_GD11X5(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "GD11X5";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 540);
            phases.Add(84, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启江西11选5奖期（每天78期。10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_JX11X5(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "JX11X5";
            var bettingOffset = 120;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 540);
            phases.Add(84, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启山东群英会奖期（每天40期。15分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_SDQYH(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "SDQYH";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(-1, 543);
            phases.Add(32, 15);
            phases.Add(-2, 178);
            phases.Add(40, 15);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启广东快乐十分奖期（每天84期。10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_GDKLSF(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "GDKLSF";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 510);
            phases.Add(84, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启广西快乐十分奖期（每天50期。10分钟销售，5分钟开奖，共15分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_GXKLSF(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "GXKLSF";
            var bettingOffset = 270;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 540);
            phases.Add(50, 15);
            var issuseFormat = "{0:yyyy}{2,3:D3}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                var dateIndex = GetDayIndex(date);
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, dayIndex: dateIndex);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启江苏快3奖期（每天82期。10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_JSKS(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "JSKS";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 510);
            phases.Add(82, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启辽林11选5奖期（每天83期。10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_LN11X5(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "LN11X5";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 520);
            phases.Add(83, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        //批量开启重庆11选5奖期（每天85期。10分钟一期）     
        public CommonActionResult OpenIssuseBatch_Fast_CQ11X5(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "CQ11X5";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 530);
            phases.Add(85, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        //批量开启重庆快乐十分奖期（每天97期。10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_CQKLSF(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "CQKLSF";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(1, 3);
            phases.Add(13, 10);
            phases.Add(0, 470);
            phases.Add(97, 10);

            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量湖南快乐十分奖期（全天84期,每10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_HNKLSF(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "HNKLSF";
            var bettingOffset = 30;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 543);
            phases.Add(84, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                var dateIndex = GetDayIndex(date);
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, dayIndex: dateIndex);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量吉林快3奖期（全天79期,每10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_JLK3(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "JLK3";
            var bettingOffset = 270;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 500);
            phases.Add(79, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}"; //"{0:yyyy}{2,3:D3}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                var dateIndex = GetDayIndex(date);
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, dayIndex: dateIndex);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量湖北快3奖期（全天78期,每10分钟一期）
        public CommonActionResult OpenIssuseBatch_Fast_HBK3(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "HBK3";
            var bettingOffset = 270;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 540);
            phases.Add(78, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}"; //"{0:yyyy}{2,3:D3}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                var dateIndex = GetDayIndex(date);
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, dayIndex: dateIndex);
            }
            return new CommonActionResult(true, "操作成功");
        }
        //山东快乐扑克3
        public CommonActionResult OpenIssuseBatch_Fast_SDKLPK3(DateTime dateFrom, DateTime dateTo)
        {
            var gameCode = "SDKLPK3";
            var bettingOffset = 40;
            var phases = new Dictionary<int, double>();
            phases.Add(0, 501);
            phases.Add(88, 10);
            var issuseFormat = "{0:yyyyMMdd}-{1,2:D2}"; //"{0:yyyy}{2,3:D3}-{1,2:D2}";

            var admin = new IssuseBusiness();
            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                var dateIndex = GetDayIndex(date);
                admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, dayIndex: dateIndex);
            }
            return new CommonActionResult(true, "操作成功");
        }
        #endregion

        #region 低频彩开启奖期

        // 批量开启双色球奖期（每周二、四、日开奖）
        public CommonActionResult OpenIssuseBatch_Slow_SSQ(int yearFrom, int yearTo)
        {
            var gameCode = "SSQ";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
                {
                    if ((d.DayOfWeek == DayOfWeek.Tuesday || d.DayOfWeek == DayOfWeek.Thursday || d.DayOfWeek == DayOfWeek.Sunday))
                    {
                        return CheckIsOpenDay(d);
                    }
                    return false;
                }, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启大乐透奖期（每周二、四、日开奖）
        public CommonActionResult OpenIssuseBatch_Slow_DLT(int yearFrom, int yearTo)
        {
            var gameCode = "DLT";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
               {
                   if ((d.DayOfWeek == DayOfWeek.Monday || d.DayOfWeek == DayOfWeek.Wednesday || d.DayOfWeek == DayOfWeek.Saturday))
                   {
                       return CheckIsOpenDay(d);
                   }
                   return false;
               }, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启七星彩奖期（每周二、五、日开奖）
        public CommonActionResult OpenIssuseBatch_Slow_QXC(int yearFrom, int yearTo)
        {
            var gameCode = "QXC";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
               {
                   if ((d.DayOfWeek == DayOfWeek.Tuesday || d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Sunday))
                   {
                       return CheckIsOpenDay(d);
                   }
                   return false;
               }, (d) => d.Date.AddHours(21).AddMinutes(30), 90);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启七乐彩奖期（每周二、五、日开奖）
        public CommonActionResult OpenIssuseBatch_Slow_QLC(int yearFrom, int yearTo)
        {
            var gameCode = "QLC";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
               {
                   if ((d.DayOfWeek == DayOfWeek.Monday || d.DayOfWeek == DayOfWeek.Wednesday || d.DayOfWeek == DayOfWeek.Friday))
                   {
                       return CheckIsOpenDay(d);
                   }
                   return false;
               }, (d) => d.Date.AddHours(21).AddMinutes(30), 90);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启东方6+1奖期（每周一、三、六开奖）
        public CommonActionResult OpenIssuseBatch_Slow_DF6J1(int yearFrom, int yearTo)
        {
            var gameCode = "DF6J1";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
               {
                   if ((d.DayOfWeek == DayOfWeek.Monday || d.DayOfWeek == DayOfWeek.Wednesday || d.DayOfWeek == DayOfWeek.Saturday))
                   {
                       return CheckIsOpenDay(d);
                   }
                   return false;
               }, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }

        #endregion

        #region 每日彩开启奖期

        // 批量开启福彩3D奖期（每日开奖）
        public CommonActionResult OpenIssuseBatch_Daily_FC3D(int yearFrom, int yearTo)
        {
            var gameCode = "FC3D";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启排列三奖期（每日开奖）
        public CommonActionResult OpenIssuseBatch_Daily_PL3(int yearFrom, int yearTo)
        {
            var gameCode = "PL3";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启排列五奖期（每日开奖）
        public CommonActionResult OpenIssuseBatch_Daily_PL5(int yearFrom, int yearTo)
        {
            var gameCode = "PL5";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启华东15选5奖期（每日开奖）
        public CommonActionResult OpenIssuseBatch_Daily_HD15X5(int yearFrom, int yearTo)
        {
            var gameCode = "HD15X5";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }
        // 批量开启广东好彩一奖期（每日开奖）
        public CommonActionResult OpenIssuseBatch_Daily_HC1(int yearFrom, int yearTo)
        {
            var gameCode = "HC1";
            var issuseFormat = "{0:yyyy}-{1,3:D3}";
            var admin = new IssuseBusiness();
            for (var year = yearFrom; year <= yearTo; year++)
            {
                admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
            }
            return new CommonActionResult(true, "操作成功");
        }

        #endregion

        /// <summary>
        /// 奖期派奖
        /// </summary>
        public CommonActionResult IssusePrize(string gameCode, string gameType, string issuseNumber, string winNumber)
        {
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                //奖期派奖
                new Sports_Business().LotteryIssusePrize(gameCode, gameType, issuseNumber, winNumber);
                watch.Stop();

                return new CommonActionResult(true, string.Format("奖期派奖完成,用时{0}毫秒", watch.Elapsed.TotalMilliseconds));
            }
            catch (Exception ex)
            {
                throw ex;
                //watch.Stop();
                //return new CommonActionResult(true, string.Format("{0},用时{1}毫秒", ex.Message, watch.Elapsed.TotalMilliseconds));
            }
        }

        /// <summary>
        /// 更新数字彩奖池
        /// </summary>
        public CommonActionResult UpdateBonusPool(string gameCode, string issuseNumber)
        {
            new TicketGatewayAdmin().UpdateBonusPool_SZC(gameCode, issuseNumber);
            return new CommonActionResult(true, "更新完成");
        }

        /// <summary>
        /// 查询未派奖的票，并执行派奖
        /// </summary>
        //public string QueryUnPrizeTicketAndDoPrize(string gameCode, string gameType, string issuseNumber, string winNumber, int count)
        //{
        //    return new Sports_Business().QueryUnPrizeTicketAndDoPrize(gameCode, gameType, issuseNumber, winNumber, count);
        //}

        /// <summary>
        /// 按彩种查询未派奖的票并执行派奖
        /// </summary>
        public string QueryUnPrizeTicketAndDoPrizeByGameCode(string gameCode, string gameType, int count)
        {
            return new Sports_Business().QueryUnPrizeTicketAndDoPrizeByGameCode(gameCode, gameType, count);
        }

        public CommonActionResult ManualPrizeOrder(string orderId)
        {
            new Sports_Business().ManualPrizeOrder(orderId);
            return new CommonActionResult(true, "票数据派奖完成");
        }
        public string QueryErrorTicketOrder(int count)
        {
            return new Sports_Business().QueryErrorTicketOrder(count);
        }
        public CommonActionResult ManualBet(string orderId)
        {
            new Sports_Business().ManualBet(orderId);
            return new CommonActionResult(true, "投注出票完成");
        }

        /// <summary>
        /// 查询未派奖的订单，并执行派奖
        /// </summary>
        public string QueryUnPrizeOrderAndDoPrize(string gameCode, int count)
        {
            return new Sports_Business().QueryUnPrizeOrderAndDoPrize(gameCode, count);
        }

        /// <summary>
        /// 查询未拆票的订单
        /// </summary>
        public string QueryUnSplitTicketsOrder(int count)
        {
            return new Sports_Business().QueryUnSplitTicketsOrder(count);
        }

        /// <summary>
        /// 拆行拆票
        /// </summary>
        public CommonActionResult DoSplitOrderTickets(string schemeId)
        {
            try
            {
                //var msg = new Sports_Business().DoSplitOrderTickets(schemeId);
                var msg = new Sports_Business().DoSplitOrderTicketsWithNoThread(schemeId);
                return new CommonActionResult(true, "执行完成 - " + msg);
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }

        }

        /// <summary>
        /// 拆行拆票
        /// </summary>
        public CommonActionResult DoSplitOrderTickets2(string obj, string input)
        {
            try
            {
                return new CommonActionResult(true, "执行完成 - ");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }

        }

        /// <summary>
        /// 自动更新可追号的订单状态
        /// </summary>
        public CommonActionResult UpdateCanChaseOrderStatus(string noChaseGameCodeStr, int afterSeconds)
        {
            try
            {
                var msg = new Sports_Business().UpdateCanChaseOrderStatus(noChaseGameCodeStr, afterSeconds);
                return new CommonActionResult(true, msg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 未开出对应彩种奖期开奖号，按订单本金归还
        /// </summary>
        public string QueryOrderAndFundOrder(string gameCode, string issuse)
        {
            new Sports_Business().LotteryIssusePrize(gameCode, string.Empty, issuse, "-");
            //new PrizeBusiness().IssusePrize(gameCode, issuse, string.Empty);
            return "奖期更新完成";
        }

        public CommonActionResult MoveComplateTicket(int count)
        {
            new Sports_Business().MoveComplateTicket(count);
            return new CommonActionResult(true, "移动完成");
        }

        /// <summary>
        /// 添加比赛，已有就返回球队名的图片地址
        /// </summary>
        public IndexMatch_Collection AddIndexMatch(string indexMatchList)
        {
            return new Sports_Business().AddIndexMatch(indexMatchList);
        }

        #endregion

        #region 原ticket操作

        /// <summary>
        /// 比赛采集调用
        /// </summary>
        public CommonActionResult HandleNotification(string text, string param, string innerKey, NoticeType noticeType)
        {
            try
            {
                new Sports_Business().UpdateLocalData(text, param, noticeType, innerKey);

                return new CommonActionResult(true, "处理数据成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }

        /// <summary>
        /// 根据彩种更新比赛为取消
        /// </summary>
        public CommonActionResult DoMatchCancel(string gameCode, string matchId, string issuse)
        {
            try
            {
                new Sports_Business().DoMatchCancel(gameCode, matchId, issuse);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "更新数据过程发生异常 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询禁赛列表
        /// </summary>
        public DisableMatchConfigInfoCollection QueryDisableMatchConfigList(string gameCode)
        {
            return new TicketGatewayAdmin().QueryDisableMatchConfigList(gameCode);
        }

        /// <summary>
        /// 手工更新竞彩足球SP数据
        /// </summary>
        public CommonActionResult UpdateOddsList_JCZQ_Manual()
        {
            try
            {
                new TicketGatewayAdmin().UpdateOddsList_JCZQ_Manual();

                return new CommonActionResult(true, "处理数据成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }

        /// <summary>
        /// 手工更新竞彩篮球SP数据
        /// </summary>
        public CommonActionResult UpdateOddsList_JCLQ_Manual()
        {
            try
            {
                new TicketGatewayAdmin().UpdateOddsList_JCLQ_Manual();

                return new CommonActionResult(true, "处理数据成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }

        /// <summary>
        /// 根据彩种派奖票数据
        /// </summary>
        public string GameCodePrizeTicket(string gameCode, int num)
        {
            try
            {
                var log = string.Empty;
                switch (gameCode.ToUpper())
                {
                    case "BJDC":
                        log = new TicketGatewayAdmin().PrizeBJDCTicket(num);
                        break;
                    case "JCZQ":
                        log = new TicketGatewayAdmin().PrizeJCZQTicket(num);
                        break;
                    case "JCLQ":
                        log = new TicketGatewayAdmin().PrizeJCLQTicket(num);
                        break;
                    default:
                        log = "派奖彩种异常" + gameCode.ToUpper();
                        break;
                }
                return log;
            }
            catch (Exception ex)
            {
                return "处理数据过程发生异常 - " + ex.Message;
            }
        }

        /// <summary>
        /// 根据订单号派奖
        /// </summary>
        public CommonActionResult PrizeTicket_OrderId(string gameCode, string orderId)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "BJDC":
                        new TicketGatewayAdmin().PrizeBJDCTicket_OrderId(orderId);
                        break;
                    case "JCZQ":
                        new TicketGatewayAdmin().PrizeJCZQTicket_OrderId(orderId);
                        break;
                    case "JCLQ":
                        new TicketGatewayAdmin().PrizeJCLQTicket_OrderId(orderId);
                        break;
                    case "CTZQ":
                        new Sports_Business().ManualPrizeOrder(orderId);
                        break;
                    default:
                        throw new Exception("派奖彩种异常" + gameCode.ToUpper());
                }
                string str = new Sports_Business().DoOrderPrize(orderId);
                return new CommonActionResult(true, str);
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }

        #endregion


        /// <summary>
        /// 发送生成JsonData文件通知
        /// </summary>
        public string BuildJsonDataNotice(string pageType)
        {
            try
            {
                return BusinessHelper.BuildJsonDataNotice(pageType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据Id查询首页比赛队伍
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IndexMatchInfo QueryIndexMatchInfo(int id)
        {
            try
            {
                return new Sports_Business().QueryIndexMatchInfo(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 修改首页比赛数据
        /// </summary>
        public CommonActionResult UpdateIndexMatch(int id, string imgPath)
        {
            try
            {
                new Sports_Business().UpdateIndexMatch(id, imgPath);
                return new CommonActionResult(true, "修改队伍图标成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询首页比赛数据列表
        /// </summary>
        public IndexMatch_Collection QueryIndexMatchCollection(string matchId, string hasImg, int pageIndex, int pageSize)
        {
            try
            {
                return new Sports_Business().QueryIndexMatchCollection(matchId, hasImg, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #region Redis相关调用

        #region 初始化Redis各类数据

        /// <summary>
        /// 初始化数字彩中奖规则
        /// </summary>
        public CommonActionResult LoadSZCBonusRule()
        {
            RedisMatchBusiness.LoadSZCBonusRule();
            return new CommonActionResult(true, "初始化完成");
        }

        /// <summary>
        /// 初始化数字彩奖池
        /// </summary>
        public CommonActionResult LoadSZCBonusPool()
        {
            RedisMatchBusiness.LoadSZCBonusPool();
            return new CommonActionResult(true, "初始化完成");
        }

        /// <summary>
        /// 初始化数字彩开奖号码
        /// </summary>
        public CommonActionResult LoadSZCWinNumber()
        {
            RedisMatchBusiness.LoadSZCWinNumber();
            return new CommonActionResult(true, "初始化完成");
        }

        /// <summary>
        /// 加载传统足球比赛结果
        /// </summary>
        public CommonActionResult LoadCTZQBonusPool()
        {
            RedisMatchBusiness.LoadCTZQBonusPool();
            return new CommonActionResult(true, "初始化完成");
        }

        /// <summary>
        /// 加载北京单场比赛结果
        /// </summary>
        public CommonActionResult LoadBJDCMatchResult()
        {
            RedisMatchBusiness.LoadBJDCMatchResult();
            return new CommonActionResult(true, "初始化完成");
        }

        /// <summary>
        /// 加载竞彩篮球比赛结果
        /// </summary>
        public CommonActionResult LoadJCLQMatchResult()
        {
            RedisMatchBusiness.LoadJCLQMatchResult();
            return new CommonActionResult(true, "初始化完成");
        }

        /// <summary>
        /// 加载竞彩足球比赛结果
        /// </summary>
        public CommonActionResult LoadJCZQMatchResult()
        {
            RedisMatchBusiness.LoadJCZQMatchResult();
            return new CommonActionResult(true, "初始化完成");
        }

        #endregion

        #region 订单拆票

        /// <summary>
        /// 订单拆票
        /// </summary>
        public string SplitOrderTicket(string gameCode, int doMaxOrderCount)
        {
            return RedisOrderBusiness.SplitOrderTicket(gameCode, doMaxOrderCount);
        }
        public string SplitOrderTicketByKey(string gameCode, string fullKey, int doMaxOrderCount)
        {
            return RedisOrderBusiness.SplitOrderTicket(gameCode, fullKey, doMaxOrderCount);
        }


        /// <summary>
        /// 订单拆票
        /// </summary>
        public string SplitOrderTicket_Single(string gameCode, int doMaxOrderCount)
        {
            return RedisOrderBusiness.SplitOrderTicket_Single(gameCode, doMaxOrderCount);
        }

        /// <summary>
        /// 追号订单拆票
        /// </summary>
        public string SplitChaseOrderTicket(string gameCode)
        {
            return RedisOrderBusiness.SplitChaseOrderTicket(gameCode);
        }

        /// <summary>
        /// 查询SQL中等待拆票的订单号
        /// </summary>
        public string QuerySQLUnSplitTicketsOrder()
        {
            return RedisOrderBusiness.QueryUnSplitTicketsOrder();
        }

        /// <summary>
        /// 把SQL中的订单加入Redis待拆票库中
        /// </summary>
        public string AddSQLOrderToWaitSplitList(string schemeId)
        {
            return RedisOrderBusiness.AddOrderToWaitSplitList(schemeId);
        }

        /// <summary>
        /// 查询已出票未派奖的订单
        /// </summary>
        public string QuerySQLUnPrizeOrder()
        {
            return RedisOrderBusiness.QueryUnPrizeOrder();
        }

        /// <summary>
        /// 添加SQL订单数据到Redis库中
        /// </summary>
        public string AddSQLOrderToRunningOrder(string schemeId)
        {
            return RedisOrderBusiness.AddToRunningOrder(schemeId);
        }

        #endregion

        #region 派奖

        /// <summary>
        /// 数字彩派奖
        /// </summary>
        public string PrizeOrder_SZC(string gameCode, int pageSize, int doMaxCount)
        {
            return RedisOrderBusiness.PrizeOrder_SZC(gameCode, pageSize, doMaxCount);
        }

        public string PrizeOrder_SZC_ByKey(string gameCode, string currentIssuseNumber, string fullKey)
        {
            return RedisOrderBusiness.PrizeOrder_SZC_ByKey(gameCode, currentIssuseNumber, fullKey);
        }

        public string PrizeOrder_SZC_Page(string gameCode, string currentIssuseNumber, string fullKey, string runingOrderList)
        {
            return RedisOrderBusiness.PrizeOrder_SZC_Page(gameCode, currentIssuseNumber, fullKey, runingOrderList);
        }

        /// <summary>
        /// 传统足球派奖
        /// </summary>
        public string PrizeOrder_CTZQ(string gameType, int pageSize, int doMaxCount)
        {
            return RedisOrderBusiness.PrizeOrder_CTZQ(gameType, pageSize, doMaxCount);
        }

        /// <summary>
        /// 北京单场派奖
        /// </summary>
        public string PrizeOrder_BJDC(int pageSize, int doMaxCount)
        {
            return RedisOrderBusiness.PrizeOrder_BJDC(pageSize, doMaxCount);
        }

        /// <summary>
        /// 竞彩篮球派奖
        /// </summary>
        public string PrizeOrder_JCLQ(int pageSize, string fullKey)
        {
            return RedisOrderBusiness.PrizeOrder_JCLQ(pageSize, fullKey);
        }

        /// <summary>
        /// 竞彩足球派奖
        /// </summary>
        public string PrizeOrder_JCZQ(int pageSize, string fullKey)
        {
            return RedisOrderBusiness.PrizeOrder_JCZQ(pageSize, fullKey);
        }

        /// <summary>
        /// 欧洲杯派奖
        /// </summary>
        public string PrizeOrder_OZB(int pageSize, int doMaxCount)
        {
            return RedisOrderBusiness.PrizeOrder_OZB(pageSize, doMaxCount);
        }

        /// <summary>
        /// 世界杯派奖
        /// </summary>
        public string PrizeOrder_SJB(int pageSize, int doMaxCount)
        {
            return RedisOrderBusiness.PrizeOrder_SJB(pageSize, doMaxCount);
        }

        /// <summary>
        /// 订单手工派奖
        /// </summary>
        public CommonActionResult Redis_ManualPrizeOrder(string gameCode, string schemeId)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "JCZQ":
                        RedisOrderBusiness.ManualPrizeOrder_JCZQ(schemeId);
                        break;
                    case "JCLQ":
                        RedisOrderBusiness.ManualPrizeOrder_JCLQ(schemeId);
                        break;
                    case "BJDC":
                        RedisOrderBusiness.ManualPrizeOrder_BJDC(schemeId);
                        break;
                    default:
                        throw new Exception("不支持的彩种编码：" + gameCode);
                }
                return new CommonActionResult(true, "派奖完成");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }

        #endregion

        #region 加载未来奖期数据

        public CommonActionResult LoadNextIssuseList()
        {
            try
            {
                var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                foreach (var gameCode in gameCodeArray)
                {
                    try
                    {
                        RedisMatchBusiness.LoadNextIssuseListByOfficialStopTime(gameCode);
                        RedisMatchBusiness.LoadNextIssuseListByLocalStopTime(gameCode);
                    }
                    catch (Exception)
                    {
                    }
                }
                return new CommonActionResult(true, "加载完成");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.ToString());
            }
        }

        #endregion


        #endregion
    }
}
