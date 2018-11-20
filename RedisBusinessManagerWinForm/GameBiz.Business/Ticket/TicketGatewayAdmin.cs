using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;
using Common.XmlAnalyzer;
using Common;
using System.Threading;
using Common.Net;
using Common.JSON;
using Common.Lottery;
using Common.Log;
using Common.Net.SMS;
using System.IO;
using Common.Utilities;
using Common.Communication;
using GameBiz.Core.Ticket;
using GameBiz.Business.Domain.Managers.Ticket;
using GameBiz.Business.Domain.Entities.Ticket;
using GameBiz.Core;
using GameBiz.Domain.Managers;

namespace GameBiz.Business
{
    public partial class TicketGatewayAdmin
    {
        //// 订单派奖错误时，发送短信的手机号
        //private static string _mobile_onprizeordererror = new Common_ConfigManager().GetGatewayConfigByPath("Setting.MobileOnPrizeOrderError");
        //// 竞技彩票扣税的基础金额
        //private static decimal _taxBaseMoney_Sport = new Common_ConfigManager().GetGatewayConfigByPath<decimal>("Setting.TaxBaseMoney_Sport");
        //// 竞技彩票扣税的税率
        //private static decimal _taxRatio_Sport = new Common_ConfigManager().GetGatewayConfigByPath<decimal>("Setting.TaxRatio_Sport");
        //// 北单官方扣点 - 65%
        //private static decimal _reduceRatio_BJDC = new Common_ConfigManager().GetGatewayConfigByPath<decimal>("Setting.ReduceRatio_BJDC");
        ////派奖上线金额
        //private static decimal _prizedMaxMoney = new Common_ConfigManager().GetGatewayConfigByPath<decimal>("PrizedMaxMoney");

        private static string _baseDir;
        public static void SetMatchConfigBaseDir(string dir)
        {
            _baseDir = dir;
        }
        //public UserInfoCollection QueryUserInfoList()
        //{
        //    var list = new UserInfoCollection();
        //    var manager = new Common_UserManager();
        //    foreach (var item in manager.QueryCommon_UserList())
        //    {
        //        list.Add(new UserInfo
        //        {
        //            BonusBalance = item.BonusBalance,
        //            DefaultGateway = item.DefaultGateway,
        //            DisplayName = item.DisplayName,
        //            CommissionBalance = item.CommissionBalance,
        //            CreateTime = item.CreateTime,
        //            CreditBalance = item.CreditBalance,
        //            CurrentCreditBalance = item.CurrentCreditBalance,
        //            EnableBalance = item.EnableBalance,
        //            NoticeMobile = item.NoticeMobile,
        //            NoticeUrl = item.NoticeUrl,
        //            UserId = item.UserId,
        //            UserName = item.UserName,
        //            UserType = item.UserType == UserType.OuterAgent ? "外部代理商" : item.UserType == UserType.InnerAdmin ? "内部管理员" : "第三方的代理",
        //            WarnBalance = item.WarnBalance
        //        });
        //    }
        //    return list;
        //}
        //public UserInfoCollection QueryUserInfoToAgentList()
        //{
        //    var list = new UserInfoCollection();
        //    var manager = new Common_UserManager();
        //    foreach (var item in manager.QueryCommon_UserListToAgent())
        //    {
        //        list.Add(new UserInfo
        //        {
        //            BonusBalance = item.BonusBalance,
        //            DefaultGateway = item.DefaultGateway,
        //            DisplayName = item.DisplayName,
        //            CommissionBalance = item.CommissionBalance,
        //            CreateTime = item.CreateTime,
        //            CreditBalance = item.CreditBalance,
        //            CurrentCreditBalance = item.CurrentCreditBalance,
        //            EnableBalance = item.EnableBalance,
        //            NoticeMobile = item.NoticeMobile,
        //            NoticeUrl = item.NoticeUrl,
        //            UserId = item.UserId,
        //            UserName = item.UserName,
        //            UserType = item.UserType == UserType.OuterAgent ? "外部代理商" : item.UserType == UserType.InnerAdmin ? "内部管理员" : "第三方的代理",
        //            WarnBalance = item.WarnBalance
        //        });
        //    }
        //    return list;
        //}
        //public UserInfo LoginAdmin(string loginName, string password)
        //{
        //    var info = new UserInfo();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var manager = new Common_UserManager();
        //        var roleManager = new Common_RoleManager();
        //        var user = manager.LoginNoCache(loginName, password);

        //        ObjectConvert.ConverEntityToInfo<Common_User, UserInfo>(user, ref info);

        //        info.FunctionList = roleManager.QueryRoleFunctionIdList(user.UserId);
        //        info.IsAdmin = roleManager.CheckIsAdmin(user.UserId);
        //        tran.CommitTran();
        //    }

        //    return info;
        //}
        ///// <summary>
        ///// 查询出票中心代理帐号相关信息
        ///// </summary>
        //public UserInfo GetUserToAgentId(string userId)
        //{
        //    var manager = new Common_UserManager();
        //    var entity = manager.GetUser(userId);
        //    if (entity == null)
        //        throw new Exception("查询代理出错！");
        //    return new UserInfo()
        //    {
        //        BonusBalance = entity.BonusBalance,
        //        CommissionBalance = entity.CommissionBalance,
        //        DefaultGateway = entity.DefaultGateway,
        //        DisplayName = entity.DisplayName,
        //        CreateTime = entity.CreateTime,
        //        CreditBalance = entity.CreditBalance,
        //        CurrentCreditBalance = entity.CurrentCreditBalance,
        //        EnableBalance = entity.EnableBalance,
        //        NoticeMobile = entity.NoticeMobile,
        //        NoticeUrl = entity.NoticeUrl,
        //        UserId = entity.UserId,
        //        UserName = entity.UserName,
        //        UserToken = entity.UserToken,
        //        UserType = entity.UserType == UserType.OuterAgent ? "外部代理商" : entity.UserType == UserType.InnerAdmin ? "内部管理员" : "第三方的代理",
        //        WarnBalance = entity.WarnBalance
        //    };
        //}
        //// 添加通知数据
        //public List<string> AddNotice(Common_NoticeManager noticeManager, NoticeType noticeType, string text, Common_User agent, string innerKey)
        //{
        //    if (agent == null) return new List<string>();
        //    if (string.IsNullOrEmpty(text)) return new List<string>();
        //    if (string.IsNullOrEmpty(agent.NoticeUrl)) return new List<string>();
        //    //var sign = string.Format("{0}{1}{2}{3}", agent.UserId, (int)noticeType, text, agent.Password);
        //    //if (agent.UserType == UserType.ThirdPartyAgent)
        //    var sign = string.Format("{0}{1}{2}", agent.UserId, (int)noticeType, text);
        //    sign = Encipherment.MD5(sign).ToUpper();
        //    var postData = string.Format("nAgent={0}&nType={1}&nValue={2}&nSign={3}", agent.UserId, (int)noticeType, text, sign);
        //    var urlList = agent.NoticeUrl.Split('|');
        //    var noticeIdList = new List<string>();
        //    if (string.IsNullOrEmpty(postData))
        //        return noticeIdList;

        //    foreach (var item in urlList)
        //    {
        //        if (string.IsNullOrEmpty(item)) continue;
        //        var noticeEntity = new Common_Notice_Running()
        //        {
        //            NoticeType = noticeType,
        //            InnerKey = innerKey,
        //            ReceiveAgentId = agent.UserId,
        //            ReceiveUrlRoot = item,
        //            PostDataString = postData,
        //        };
        //        noticeManager.AddNotice(noticeEntity);
        //        noticeIdList.Add(noticeEntity.SId);
        //    }
        //    return noticeIdList;
        //}
        //public IssuseInfoCollection QueryIssuseCollection(string gameCode, string[] issuseNumberArray)
        //{
        //    var entityList = new Ticket_IssuseManager().QueryIssuseList(gameCode, issuseNumberArray);
        //    var collection = new IssuseInfoCollection();
        //    ObjectConvert.ConvertEntityListToInfoList<IList<Ticket_Issuse>, Ticket_Issuse, IssuseInfoCollection, IssuseInfo>(entityList, ref collection, () => new IssuseInfo());
        //    return collection;
        //}

        //#region 配置相关
        //public RequestTicketConfigInfoCollection QueryRequestTicketConfigList(RequestTicketConfigCategory? category, string userId)
        //{
        //    var list = new RequestTicketConfigInfoCollection();
        //    var manager = new RequestTicketConfigManager();
        //    var totalCount = 0;
        //    list.ConfigList = manager.QueryRequestTicketConfigList(category, userId, out totalCount);
        //    list.TotalCount = totalCount;
        //    return list;
        //}

        //public RequestTicketConfigInfo QueryRequestTicketConfig(int id)
        //{
        //    var userManager = new RequestTicketConfigManager();
        //    var config = userManager.QueryRequestTicketConfig(id);
        //    if (config == null)
        //        throw new Exception("未找到删除的数据！");
        //    return new RequestTicketConfigInfo
        //    {
        //        Amount = config.Amount,
        //        Category = config.Category,
        //        GameCode = config.GameCode,
        //        GameType = config.GameType,
        //        GateWay = config.GateWay,
        //        UpdateTime = config.UpdateTime,
        //        UserId = config.UserId,
        //        Id = config.Id
        //    };
        //}
        //public void DeleteRequestTicketConfig(int id)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var userManager = new RequestTicketConfigManager();
        //        var config = userManager.QueryRequestTicketConfig(id);
        //        if (config == null)
        //            throw new Exception("未找到删除的数据！");
        //        userManager.DeleteRequestTicketConfig(config);
        //        tran.CommitTran();
        //    }
        //}
        //public void AddOrUpdateRequestTicketConfig(RequestTicketConfigInfo info)
        //{
        //    var manager = new RequestTicketConfigManager();
        //    manager.AddOrUpdateRequestTicketConfig(info);
        //}
        //#endregion

        ///// <summary>
        ///// 对代理商发出通知
        ///// </summary>
        //public void SendNotification(params string[] noticeIdArray)
        //{
        //    if (noticeIdArray.Length == 0) return;
        //    foreach (var noticeId in noticeIdArray)
        //    {
        //        if (string.IsNullOrEmpty(noticeId)) continue;
        //        var notice = new Common_NoticeManager().GetRunningNotice(noticeId);
        //        if (notice == null) continue;
        //        //var agent = new Common_UserManager(DbAccessHelper.DbAccess).GetUser(notice.ReceiveAgentId);
        //        var noticeUrl = notice.ReceiveUrlRoot;
        //        if (string.IsNullOrEmpty(noticeUrl)) continue;

        //        StringBuilder msgBuilder = new StringBuilder();
        //        msgBuilder.Append("开始发送：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-->");
        //        msgBuilder.Append("-->" + noticeUrl);
        //        msgBuilder.Append("-->" + notice.PostDataString);
        //        bool success = DoSendNotificationByPost(noticeUrl, notice.PostDataString, notice.SuccessFlag, 0, notice.MaxRetryTimes, notice.RetryTimeSpan, ref msgBuilder);
        //        msgBuilder.AppendLine();

        //        notice.SendStatus = success ? ResultStatus.Successful : ResultStatus.Failed;
        //        notice.SendedTimes += 1;
        //        notice.Remark += msgBuilder.ToString();
        //        notice.LastRetryTime = DateTime.Now;

        //        //去掉次数限制
        //        //if (notice.SendedTimes >= notice.MaxRetryTimes)
        //        //    success = true;

        //        using (var tran = new TicketBusinessManagement())
        //        {
        //            tran.BeginTran();

        //            var noticeTranManager = new Common_NoticeManager();
        //            if (success)
        //            {
        //                try
        //                {
        //                    var completedNotice = new Common_Notice_Completed(notice);
        //                    //! 将通知数据从临时表转移到历史记录表
        //                    noticeTranManager.AddCompletedNotice(completedNotice);

        //                }
        //                catch (Exception ex)
        //                {
        //                    //noticeTranManager.UpdateNotice(notice);
        //                }
        //                finally
        //                {
        //                    //x 删除临时表（进行中）的通知数据
        //                    noticeTranManager.DeleteRunningNotice(notice);
        //                }
        //            }
        //            else
        //            {
        //                noticeTranManager.UpdateNotice(notice);
        //            }

        //            tran.CommitTran();
        //        }
        //    }
        //}
        ///// <summary>
        ///// 对指定奖期进行派奖
        ///// </summary>
        ///// <returns>返回对应通知编号</returns>
        //public string[] PrizeIssuse(string gameCode, string issuseNumber, string winNumber, string gameType = "")
        //{
        //    #region 验证中奖号码的有效性

        //    var winNumberAnalyzer = AnalyzerFactory.GetWinNumberAnalyzer(gameCode, gameType);
        //    string errMsg;
        //    if (!winNumberAnalyzer.CheckWinNumber(winNumber, out errMsg))
        //    {
        //        throw new WinNumberFormatException(gameCode, winNumber, errMsg);
        //    }

        //    #endregion
        //    var isTrue = false;
        //    var issuseManager = new Ticket_IssuseManager();
        //    var orderManager = new Ticket_OrderManager();
        //    var userManager = new Common_UserManager();
        //    var issuse = new Ticket_Issuse();
        //    var issuse_CTZQ = new CTZQ_Issuse();

        //    #region 验证奖期数据
        //    if (gameCode.ToUpper() == "CTZQ")
        //    {
        //        issuse_CTZQ = issuseManager.GetIssuse_CTZQ(gameType, issuseNumber);
        //        if (issuse_CTZQ == null)
        //            throw new ArgumentException(string.Format("指定奖期\"{0}-{1}\"不存在", gameCode, issuseNumber));
        //    }
        //    else
        //    {
        //        issuse = issuseManager.GetIssuse(gameCode, gameType, issuseNumber);
        //        if (issuse == null)
        //            throw new ArgumentException(string.Format("指定奖期\"{0}-{1}\"不存在", gameCode, issuseNumber));
        //    }

        //    #endregion

        //    #region 处理中奖数据

        //    var agentList = userManager.QueryNoticeAgentList();
        //    var orderList = orderManager.QueryWaitPrizeOrderListByIssuse(gameCode, issuseNumber, gameType);
        //    foreach (var runningOrder in orderList)
        //    {
        //        try
        //        {
        //            if (runningOrder.TicketStatus == OrderStatus.TickeRunning)
        //                continue;
        //            switch (runningOrder.BettingCategory)
        //            {
        //                //case SchemeBettingCategory.GeneralBetting:
        //                //    PrizeOrder(runningOrder, winNumber);
        //                //    break;
        //                //case SchemeBettingCategory.SingleBetting:
        //                //    PrizeSingleOrder(runningOrder, winNumber);
        //                //    break;
        //                case SchemeBettingCategory.GeneralBetting:
        //                case SchemeBettingCategory.SingleBetting:
        //                    PrizeOrder(runningOrder, winNumber, agentList.ToList());
        //                    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if ((ex.Message.Contains("未更新奖池数据") || ex.Message.Contains("此奖等无人中奖")) && (gameCode.ToUpper() == "SSQ" || gameCode.ToUpper() == "DLT"))
        //                isTrue = true;
        //            var msg = "订单派奖错误 - " + gameCode + "." + gameType + "." + issuseNumber + "：" + ex.Message;
        //            ex = new Exception(msg, ex);
        //            LogWriterGetter.GetLogWriter().Write("Error", "PrizeOrder", ex);
        //            // 发送异常短信
        //            try
        //            {
        //                new Thread(() =>
        //                {
        //                    var valus = new Common_ConfigManager().GetGatewayConfigByPath<string>("HandleAbnormalPhone");
        //                    var SMSAGentList = valus.Split('|');
        //                    foreach (var item in SMSAGentList)
        //                    {
        //                        SendSMS_VeeSing(msg, item);
        //                    }
        //                }).Start();
        //            }
        //            catch { }
        //        }
        //    }

        //    #endregion

        //    if (isTrue)
        //    {
        //        var noticeIdList = new List<string>();
        //        issuse.WinNumber = winNumber;
        //        issuse.Status = IssuseStatus.Awarding;
        //        issuseManager.UpdateIssuse(issuse);
        //        return noticeIdList.ToArray();
        //    }

        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        #region 更新奖期状态

        //        if (gameCode.ToUpper() == "CTZQ")
        //        {
        //            issuse_CTZQ.WinNumber = winNumber;
        //            issuse_CTZQ.Status = IssuseStatus.Complete;
        //            issuseManager.UpdateIssuse_CTZQ(issuse_CTZQ);
        //        }
        //        else
        //        {
        //            issuse.WinNumber = winNumber;
        //            issuse.Status = IssuseStatus.Complete;
        //            issuseManager.UpdateIssuse(issuse);
        //        }

        //        #endregion

        //        //var userManager = new Common_UserManager(tran);
        //        var bonusManager = new Ticket_BonusManager();
        //        var noticeManager = new Common_NoticeManager();

        //        #region 添加通知数据

        //        //var agentList = userManager.QueryNoticeAgentList();
        //        var noticeIdList = new List<string>();
        //        foreach (var agent in agentList)
        //        {
        //            var issuseId = gameCode + "|" + issuseNumber;
        //            //var text = gameCode == "CTZQ" ? string.Format("{0}_{1}_{2}_{3}_{4}_{5}", gameCode, gameType, issuseNumber, winNumber, totalBeforeTaxBonusMoeny, totalAfterTaxBonusMoeny)
        //            //   : string.Format("{0}_{1}_{2}_{3}_{4}", gameCode, issuseNumber, winNumber, totalBeforeTaxBonusMoeny, totalAfterTaxBonusMoeny);


        //            var text = string.Format("{0}_{1}_{2}_{3}", gameCode, gameType, issuseNumber, winNumber);
        //            //添加中奖金额
        //            //agent.BonusBalance += totalAfterTaxBonusMoeny;
        //            //userManager.UpdateUser(agent);
        //            //CapitalSubsidiary.PayMoney(issuseId, agent.UserId, Core.PayType.Bonus, totalAfterTaxBonusMoeny);

        //            noticeIdList.AddRange(AddNotice(noticeManager, NoticeType.BonusNotice_New, text, agent, issuseId));
        //        }

        //        #endregion

        //        tran.CommitTran();

        //        return noticeIdList.ToArray();
        //    }
        //}

        ///// <summary>
        ///// 当期所有订单退款
        ///// </summary>
        //public string[] IssuseAllOrderRefund(string gameCode, string issuseNumber, string winNumber, string gameType = "")
        //{
        //    var isTrue = false;
        //    var issuseManager = new Ticket_IssuseManager();
        //    var orderManager = new Ticket_OrderManager();
        //    var userManager = new Common_UserManager();

        //    var issuse = issuseManager.GetIssuse(gameCode, gameType, issuseNumber);
        //    if (issuse == null)
        //        throw new ArgumentException(string.Format("指定奖期\"{0}-{1}\"不存在", gameCode, issuseNumber));

        //    #region 处理中奖数据

        //    var agentList = userManager.QueryNoticeAgentList();
        //    var orderList = orderManager.QueryWaitPrizeOrderListByIssuse(gameCode, issuseNumber, gameType);
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        foreach (var runningOrder in orderList)
        //        {
        //            try
        //            {
        //                if (runningOrder.TicketStatus == OrderStatus.TickeRunning)
        //                    continue;



        //                var prizeTicketList = new List<PrizeTicketListInfo>();
        //                var prizeOrder = new PrizeOrderInfo();
        //                var bonusManager = new Ticket_BonusManager();
        //                var historyManager = new Common_GatewayHistoryManager();

        //                runningOrder.BonusStatus = BonusStatus.Lose;
        //                runningOrder.BonusTime = DateTime.Now;
        //                var bonusLevel_order = new Dictionary<int, int>();

        //                //数字彩票数据
        //                IList<GetWay_LocHistory> locTicketList = new List<GetWay_LocHistory>();
        //                if (runningOrder.TicketGateway.Split('|').Contains("LOCAL"))
        //                {
        //                    locTicketList = historyManager.GetLocHistoryByOrderId(runningOrder.OrderId, string.Empty);
        //                }

        //                var totalHitCount = 0;
        //                var totalPreTaxBonusMoney = -1M;
        //                var totalAfterTaxBonusMoney = -1M;
        //                var firstOrTwoMoney = 0M;
        //                #region 计算票列表的中奖数据

        //                #region 虚拟订单处理
        //                //票商的票数据
        //                foreach (var ticket in locTicketList)
        //                {
        //                    if (ticket.TicketStatus != TicketStatus.Successful)
        //                        continue;

        //                    var preTaxBonusMoney = -1M;
        //                    var afterTaxBonusMoney = -1M;
        //                    var hitCount = 0;

        //                    totalHitCount += hitCount;

        //                    ticket.BonusStatus = preTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //                    ticket.BonusMoneyPrevTax = preTaxBonusMoney;
        //                    ticket.BonusMoneyAfterTax = afterTaxBonusMoney;

        //                    historyManager.UpdateGatewayLocHistory(ticket);
        //                    prizeTicketList.Add(new PrizeTicketListInfo
        //                    {
        //                        TicketId = ticket.TicketId,
        //                        AfterTaxBonusMoney_Ticket = afterTaxBonusMoney,
        //                        PreTaxBonusMoney_Ticket = preTaxBonusMoney
        //                    });
        //                }
        //                #endregion

        //                #endregion

        //                prizeOrder.OrderId = runningOrder.OrderId;
        //                prizeOrder.PreTaxBonusMoney = totalPreTaxBonusMoney;
        //                prizeOrder.AfterTaxBonusMoney = totalAfterTaxBonusMoney;
        //                prizeOrder.PrizeTicketListInfo = prizeTicketList;

        //                runningOrder.HitCount = totalHitCount;
        //                runningOrder.BonusMoneyBeforeTax = totalPreTaxBonusMoney;
        //                runningOrder.BonusMoneyAfterTax = totalAfterTaxBonusMoney;

        //                runningOrder.BonusStatus = totalPreTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //                //当双色球大乐透中一二等奖取奖池金额时不给用户加钱
        //                var money = (runningOrder.GameCode.ToUpper() == "SSQ" || runningOrder.GameCode.ToUpper() == "DLT") ? totalPreTaxBonusMoney - firstOrTwoMoney : totalPreTaxBonusMoney;
        //                var prizedOrderEntity = new Ticket_Order_Prized();
        //                prizedOrderEntity.LoadByRunning(runningOrder);
        //                orderManager.AddPrizedOrder(prizedOrderEntity);    // 将订单从临时表转移到历史记录表
        //                orderManager.DeleteRunningOrder(runningOrder);  // 删除临时表（进行中）的订单数据

        //                var orderAllLManager = new Common_OrderManager();
        //                var orderALEntity = orderAllLManager.GetRunningOrder(runningOrder.OrderId);
        //                orderALEntity.PrizedTime = DateTime.Now;
        //                orderALEntity.BonusMoneyPrevTax = totalPreTaxBonusMoney;
        //                orderALEntity.BonusMoneyAfterTax = totalAfterTaxBonusMoney;
        //                orderAllLManager.UpdateCommon_OrderAllList(orderALEntity);


        //                var orderAgent = agentList.FirstOrDefault(p => p.UserId == runningOrder.AgentId);
        //                if (orderAgent == null)
        //                    throw new Exception("没有找到该订单中的代理-" + runningOrder.AgentId);

        //                //添加通知数据
        //                var noticeManager = new Common_NoticeManager();
        //                var num = 100;
        //                var count = 1;
        //                count = prizeOrder.PrizeTicketListInfo.Count / num;
        //                if (prizeOrder.PrizeTicketListInfo.Count % num > 0)
        //                    count++;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    var str = JsonSerializer.Serialize<PrizeOrderInfo>(new PrizeOrderInfo
        //                    {
        //                        OrderId = prizeOrder.OrderId,
        //                        AfterTaxBonusMoney = prizeOrder.AfterTaxBonusMoney,
        //                        PreTaxBonusMoney = prizeOrder.PreTaxBonusMoney,
        //                        TicketCount = prizeOrder.PrizeTicketListInfo.Count,
        //                        PrizeTicketListInfo = prizeOrder.PrizeTicketListInfo.Skip(num * i).Take(num).ToList()
        //                    });
        //                    //添加通知数据
        //                    AddNotice(noticeManager, NoticeType.BonusNotice_Order, str, orderAgent, runningOrder.OrderId);
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                if ((ex.Message.Contains("未更新奖池数据") || ex.Message.Contains("此奖等无人中奖")) && (gameCode.ToUpper() == "SSQ" || gameCode.ToUpper() == "DLT"))
        //                    isTrue = true;
        //                var msg = "订单派奖错误 - " + gameCode + "." + gameType + "." + issuseNumber + "：" + ex.Message;
        //                ex = new Exception(msg, ex);
        //                LogWriterGetter.GetLogWriter().Write("Error", "PrizeOrder", ex);
        //            }
        //        }

        //        tran.CommitTran();
        //    }

        //    #endregion

        //    if (isTrue)
        //    {
        //        var noticeIdList = new List<string>();
        //        issuse.WinNumber = winNumber;
        //        issuse.Status = IssuseStatus.Awarding;
        //        issuseManager.UpdateIssuse(issuse);
        //        return noticeIdList.ToArray();
        //    }

        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        #region 更新奖期状态

        //        issuse.WinNumber = winNumber;
        //        issuse.Status = IssuseStatus.Complete;
        //        issuseManager.UpdateIssuse(issuse);

        //        #endregion

        //        var bonusManager = new Ticket_BonusManager();
        //        var noticeManager = new Common_NoticeManager();

        //        #region 添加通知数据

        //        var noticeIdList = new List<string>();
        //        foreach (var agent in agentList)
        //        {
        //            var issuseId = gameCode + "|" + issuseNumber;
        //            var text = string.Format("{0}_{1}_{2}_{3}", gameCode, gameType, issuseNumber, winNumber);
        //            noticeIdList.AddRange(AddNotice(noticeManager, NoticeType.BonusNotice_New, text, agent, issuseId));
        //        }

        //        #endregion

        //        tran.CommitTran();

        //        return noticeIdList.ToArray();
        //    }
        //}

        ///// <summary>
        ///// 单式上传派奖
        ///// </summary>
        //private void PrizeSingleOrder(Ticket_Order_Running runningOrder, string winNumber)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var orderManager = new Ticket_OrderManager();
        //        var bonusManager = new Ticket_BonusManager();

        //        runningOrder.BonusStatus = BonusStatus.Lose;
        //        runningOrder.BonusTime = DateTime.Now;
        //        var bonusLevel_order = new Dictionary<int, int>();
        //        runningOrder.TicketList = orderManager.QueryRunningTicketListByOrderId(runningOrder.AgentId, runningOrder.OrderId);
        //        foreach (var ticket in runningOrder.TicketList)
        //        {
        //            ticket.BonusStatus = BonusStatus.Lose;
        //            ticket.BonusTime = DateTime.Now;
        //            var bonusLevel_ticket = new Dictionary<int, int>();

        //            //单式上传
        //            var singleOrder = new BJDC_OrderManager().GetSingleSchemeOrder(runningOrder.OrderId);
        //            if (singleOrder == null)
        //                throw new Exception(string.Format("未查询到订单{0}的单式上传信息", runningOrder.OrderId));
        //            var selectMatchIdArray = singleOrder.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //            var allowCodeArray = singleOrder.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //            //var codeText = File.ReadAllText(singleOrder.AnteCodeFullFileName, Encoding.UTF8);
        //            //var codeText = Encoding.UTF8.GetString(singleOrder.FileBuffer);
        //            var matchIdList = new List<string>();
        //            var codeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(singleOrder.FileBuffer, runningOrder.GameType, allowCodeArray, out matchIdList);
        //            var hitCount = 0;
        //            foreach (var code in codeList)
        //            {
        //                var anteCode = new Ticket_AnteCode_Running
        //                {
        //                    BonusStatus = BonusStatus.Lose,
        //                    BonusTime = DateTime.Now,
        //                    AnteNumber = string.Join(",", code.ToArray()),
        //                };
        //                var bonusLevel_ante = new Dictionary<int, int>();
        //                var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(runningOrder.GameCode, runningOrder.GameType);
        //                var bonusLevelList = analyzer.CaculateBonus(anteCode.AnteNumber, winNumber);
        //                var t = AnalyzerFactory.GetHitCount(anteCode.AnteNumber, winNumber);
        //                if (t > hitCount)
        //                    hitCount = t;
        //                foreach (var level in bonusLevelList)
        //                {
        //                    var rule = GetBonusRule(runningOrder.GameCode, runningOrder.GameType, level);
        //                    var money = rule.BonusMoney;
        //                    if (money == -1M)
        //                    {
        //                        // 取奖池
        //                        var pool = bonusManager.GetBonusPool(runningOrder.GameCode, runningOrder.GameType, runningOrder.IssuseNumber, level.ToString());
        //                        if (pool == null)
        //                        {
        //                            throw new Exception("未更新奖池数据 - " + runningOrder.GameCode + "." + runningOrder.GameType + " - " + pool.BonusLevelDisplayName);
        //                        }
        //                        if (pool.BonusCount <= 0)
        //                        {
        //                            throw new Exception("此奖等无人中奖 - " + runningOrder.GameCode + "." + runningOrder.GameType + " - " + pool.BonusLevelDisplayName);
        //                        }
        //                        money = pool.BonusMoney;
        //                    }
        //                    var bonusMoney = money * runningOrder.Amount;
        //                    var taxMoney = 0M;
        //                    if (money >= _taxBaseMoney_Sport)
        //                    {
        //                        taxMoney = money * _taxRatio_Sport * runningOrder.Amount;
        //                    }
        //                    HandleBonusOrder(runningOrder, ticket, anteCode, level, bonusMoney, taxMoney, bonusLevel_order, bonusLevel_ticket, bonusLevel_ante);
        //                }
        //                anteCode.BonusCountDescription = GetBonusLevelDescription(bonusLevel_ante);
        //                anteCode.BonusCountDisplayName = GetBonusLevelDisplayName(bonusLevel_ante, runningOrder.GameCode, runningOrder.GameType);
        //            }

        //            ticket.BonusCountDescription = GetBonusLevelDescription(bonusLevel_ticket);
        //            ticket.BonusCountDisplayName = GetBonusLevelDisplayName(bonusLevel_ticket, runningOrder.GameCode, ticket.GameType);
        //            ticket.HitCount = hitCount;
        //        }
        //        runningOrder.HitCount = runningOrder.TicketList.Max(t => t.HitCount);
        //        if (runningOrder.TicketList.GroupBy(t => t.GameType).Count() == 1)
        //        {
        //            runningOrder.BonusCountDescription = GetBonusLevelDescription(bonusLevel_order);
        //            runningOrder.BonusCountDisplayName = GetBonusLevelDisplayName(bonusLevel_order, runningOrder.GameCode, runningOrder.TicketList[0].GameType);
        //        }
        //        else
        //        {
        //            runningOrder.BonusCountDescription = "-1";
        //            runningOrder.BonusCountDisplayName = "混合玩法 未统计中奖注数";
        //        }
        //        var prizedOrderEntity = new Ticket_Order_Prized();
        //        prizedOrderEntity.LoadByRunning(runningOrder);
        //        orderManager.AddPrizedOrder(prizedOrderEntity);    // 将订单从临时表转移到历史记录表
        //        orderManager.DeleteRunningOrder(runningOrder);  // 删除临时表（进行中）的订单数据

        //        tran.CommitTran();
        //    }
        //}

        ///// <summary>
        ///// 对指定已派奖的订单进行重新派奖
        ///// </summary>
        //private void PrizeOrder(string orderId, string winNumber, List<Common_User> agentList)
        //{
        //    var orderManager = new Ticket_OrderManager();

        //    var orderPrized = orderManager.GetPrizedOrder(orderId);
        //    if (orderPrized != null)
        //    {
        //        var historyManager = new Common_GatewayHistoryManager();
        //        var locTicketList = historyManager.GetLocHistoryByOrderId(orderPrized.OrderId, string.Empty);
        //        var prizeTicketList = new List<PrizeTicketListInfo>();
        //        #region 虚拟订单处理

        //        var totalHitCount = 0;
        //        var totalPreTaxBonusMoney = 0M;
        //        var totalAfterTaxBonusMoney = 0M;
        //        var firstOrTwoMoney = 0M;

        //        var bonusManager = new Ticket_BonusManager();
        //        //票商的票数据
        //        foreach (var ticket in locTicketList)
        //        {
        //            var preTaxBonusMoney = 0M;
        //            var afterTaxBonusMoney = 0M;
        //            var hitCount = 0;

        //            ComputeTicketBonus(bonusManager, ticket.GameCode, ticket.GameType, ticket.IsAppend, ticket.LocBetContent, ticket.IssuseNumber, winNumber, ticket.Multiple, out hitCount, out preTaxBonusMoney, out afterTaxBonusMoney, out firstOrTwoMoney);
        //            totalHitCount += hitCount;

        //            ticket.BonusStatus = preTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //            ticket.BonusMoneyPrevTax = preTaxBonusMoney;
        //            ticket.BonusMoneyAfterTax = afterTaxBonusMoney;
        //            totalPreTaxBonusMoney += preTaxBonusMoney;
        //            totalAfterTaxBonusMoney += afterTaxBonusMoney;

        //            historyManager.UpdateGatewayLocHistory(ticket);
        //            prizeTicketList.Add(new PrizeTicketListInfo
        //            {
        //                TicketId = ticket.TicketId,
        //                AfterTaxBonusMoney_Ticket = afterTaxBonusMoney,
        //                PreTaxBonusMoney_Ticket = preTaxBonusMoney
        //            });
        //        }
        //        #endregion

        //        var orderAgent = agentList.FirstOrDefault(p => p.UserId == orderPrized.AgentId);
        //        if (orderAgent == null)
        //            throw new Exception("没有找到该订单中的代理-" + orderPrized.AgentId);

        //        //添加通知数据
        //        var noticeManager = new Common_NoticeManager();
        //        var num = 100;
        //        var count = 1;
        //        count = prizeTicketList.Count / num;
        //        if (prizeTicketList.Count % num > 0)
        //            count++;
        //        for (int i = 0; i < count; i++)
        //        {
        //            var str = JsonSerializer.Serialize<PrizeOrderInfo>(new PrizeOrderInfo
        //            {
        //                OrderId = orderPrized.OrderId,
        //                AfterTaxBonusMoney = totalAfterTaxBonusMoney,
        //                PreTaxBonusMoney = totalPreTaxBonusMoney,
        //                TicketCount = prizeTicketList.Count,
        //                PrizeTicketListInfo = prizeTicketList.Skip(num * i).Take(num).ToList()
        //            });
        //            //添加通知数据
        //            AddNotice(noticeManager, NoticeType.BonusNotice_Order, str, orderAgent, orderPrized.OrderId);
        //        }
        //    }

        //}

        ///// <summary>
        ///// 对指定奖期进行派奖
        ///// </summary>
        //private void PrizeOrder(Ticket_Order_Running runningOrder, string winNumber, List<Common_User> agentList)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var prizeTicketList = new List<PrizeTicketListInfo>();
        //        var prizeOrder = new PrizeOrderInfo();
        //        var orderManager = new Ticket_OrderManager();
        //        var bonusManager = new Ticket_BonusManager();
        //        var historyManager = new Common_GatewayHistoryManager();

        //        runningOrder.BonusStatus = BonusStatus.Lose;
        //        runningOrder.BonusTime = DateTime.Now;
        //        var bonusLevel_order = new Dictionary<int, int>();

        //        //数字彩票数据
        //        IList<GetWay_LocHistory> locTicketList = new List<GetWay_LocHistory>();
        //        if (runningOrder.TicketGateway.Split('|').Contains("LOCAL"))
        //        {
        //            locTicketList = historyManager.GetLocHistoryByOrderId(runningOrder.OrderId, string.Empty);
        //        }

        //        var totalHitCount = 0;
        //        var totalPreTaxBonusMoney = 0M;
        //        var totalAfterTaxBonusMoney = 0M;
        //        var firstOrTwoMoney = 0M;
        //        #region 计算票列表的中奖数据

        //        #region 虚拟订单处理
        //        //票商的票数据
        //        foreach (var ticket in locTicketList)
        //        {
        //            if (ticket.TicketStatus != TicketStatus.Successful)
        //                continue;

        //            var preTaxBonusMoney = 0M;
        //            var afterTaxBonusMoney = 0M;
        //            var hitCount = 0;

        //            ComputeTicketBonus(bonusManager, ticket.GameCode, ticket.GameType, ticket.IsAppend, ticket.LocBetContent, ticket.IssuseNumber, winNumber, ticket.Multiple, out hitCount, out preTaxBonusMoney, out afterTaxBonusMoney, out firstOrTwoMoney);
        //            totalHitCount += hitCount;

        //            ticket.BonusStatus = preTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //            ticket.BonusMoneyPrevTax = preTaxBonusMoney;
        //            ticket.BonusMoneyAfterTax = afterTaxBonusMoney;
        //            totalPreTaxBonusMoney += preTaxBonusMoney;
        //            totalAfterTaxBonusMoney += afterTaxBonusMoney;

        //            historyManager.UpdateGatewayLocHistory(ticket);
        //            prizeTicketList.Add(new PrizeTicketListInfo
        //            {
        //                TicketId = ticket.TicketId,
        //                AfterTaxBonusMoney_Ticket = afterTaxBonusMoney,
        //                PreTaxBonusMoney_Ticket = preTaxBonusMoney
        //            });
        //        }
        //        #endregion

        //        #endregion

        //        prizeOrder.OrderId = runningOrder.OrderId;
        //        prizeOrder.PreTaxBonusMoney = totalPreTaxBonusMoney;
        //        prizeOrder.AfterTaxBonusMoney = totalAfterTaxBonusMoney;
        //        prizeOrder.PrizeTicketListInfo = prizeTicketList;

        //        runningOrder.HitCount = totalHitCount;
        //        runningOrder.BonusMoneyBeforeTax = totalPreTaxBonusMoney;
        //        runningOrder.BonusMoneyAfterTax = totalAfterTaxBonusMoney;

        //        runningOrder.BonusStatus = totalPreTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //        //当双色球大乐透中一二等奖取奖池金额时不给用户加钱
        //        var money = (runningOrder.GameCode.ToUpper() == "SSQ" || runningOrder.GameCode.ToUpper() == "DLT") ? totalPreTaxBonusMoney - firstOrTwoMoney : totalPreTaxBonusMoney;
        //        var prizedOrderEntity = new Ticket_Order_Prized();
        //        prizedOrderEntity.LoadByRunning(runningOrder);
        //        if (orderManager.GetPrizedOrder(prizedOrderEntity.OrderId) == null)
        //            orderManager.AddPrizedOrder(prizedOrderEntity);    // 将订单从临时表转移到历史记录表
        //        orderManager.DeleteRunningOrder(runningOrder);  // 删除临时表（进行中）的订单数据

        //        var orderAllLManager = new Common_OrderManager();
        //        //if(orderAllLManager.GetRunningOrder(runningOrder.OrderId))
        //        var orderALEntity = orderAllLManager.GetRunningOrder(runningOrder.OrderId);
        //        orderALEntity.PrizedTime = DateTime.Now;
        //        orderALEntity.BonusMoneyPrevTax = totalPreTaxBonusMoney;
        //        orderALEntity.BonusMoneyAfterTax = totalAfterTaxBonusMoney;
        //        orderAllLManager.UpdateCommon_OrderAllList(orderALEntity);


        //        var orderAgent = agentList.FirstOrDefault(p => p.UserId == runningOrder.AgentId);
        //        if (orderAgent == null)
        //            throw new Exception("没有找到该订单中的代理-" + runningOrder.AgentId);

        //        //添加通知数据
        //        var noticeManager = new Common_NoticeManager();
        //        var num = 100;
        //        var count = 1;
        //        count = prizeOrder.PrizeTicketListInfo.Count / num;
        //        if (prizeOrder.PrizeTicketListInfo.Count % num > 0)
        //            count++;
        //        for (int i = 0; i < count; i++)
        //        {
        //            var str = JsonSerializer.Serialize<PrizeOrderInfo>(new PrizeOrderInfo
        //            {
        //                OrderId = prizeOrder.OrderId,
        //                AfterTaxBonusMoney = prizeOrder.AfterTaxBonusMoney,
        //                PreTaxBonusMoney = prizeOrder.PreTaxBonusMoney,
        //                TicketCount = prizeOrder.PrizeTicketListInfo.Count,
        //                PrizeTicketListInfo = prizeOrder.PrizeTicketListInfo.Skip(num * i).Take(num).ToList()
        //            });
        //            //添加通知数据
        //            AddNotice(noticeManager, NoticeType.BonusNotice_Order, str, orderAgent, runningOrder.OrderId);
        //        }


        //        tran.CommitTran();
        //    }

        //}

        ///// <summary>
        ///// 计算一张票的中奖数据
        ///// </summary>
        //private void ComputeTicketBonus(Ticket_BonusManager manager, string gameCode, string gameType, string isAppend, string betContent, string issuseNumber, string winNumber, int betAmount,
        //    out int hitCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney, out decimal firstOrTwoMoney)
        //{
        //    firstOrTwoMoney = 0M;
        //    preTaxBonusMoney = 0M;
        //    afterTaxBonusMoney = 0M;
        //    var HCount = 0;
        //    foreach (var item in betContent.Split('/'))
        //    {
        //        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, gameType);
        //        var bonusLevelList = analyzer.CaculateBonus(item, winNumber);
        //        HCount += AnalyzerFactory.GetHitCount(item, winNumber);

        //        foreach (var level in bonusLevelList)
        //        {
        //            var isTrue = false;
        //            var appendMoney = 0M;
        //            var rule = GetBonusRule(gameCode, gameType, level);
        //            var money = rule.BonusMoney;
        //            if (money == -1M)
        //            {
        //                // 取奖池
        //                var pool = manager.GetBonusPool(gameCode, (gameCode.ToUpper() == "SSQ" || gameCode.ToUpper() == "DLT") ? "" : gameType, issuseNumber, level.ToString());
        //                if (pool == null)
        //                {
        //                    throw new Exception("未更新奖池数据 - " + gameCode + "." + gameType);
        //                }
        //                if (pool.BonusCount <= 0)
        //                {
        //                    throw new Exception("此奖等无人中奖 - " + gameCode + "." + gameType);
        //                }
        //                money = pool.BonusMoney;
        //                isTrue = true;
        //            }

        //            if (level <= 3 && int.Parse(isAppend) == 1)
        //                appendMoney = money * 0.6M;
        //            else if (level > 3 && level <= 5 && int.Parse(isAppend) == 1)
        //                appendMoney = money * 0.5M;

        //            var bonusMoney = (money + appendMoney) * betAmount;
        //            preTaxBonusMoney += bonusMoney;
        //            var taxMoney = 0M;
        //            if ((money + appendMoney) >= _taxBaseMoney_Sport)
        //            {
        //                taxMoney = (money + appendMoney) * _taxRatio_Sport * betAmount;
        //            }
        //            afterTaxBonusMoney += bonusMoney - taxMoney;
        //            if (isTrue)
        //                firstOrTwoMoney += afterTaxBonusMoney;

        //            //var bonusMoney = money * betAmount;
        //            //preTaxBonusMoney += bonusMoney;
        //            //var taxMoney = 0M;
        //            //if (money >= _taxBaseMoney_Sport)
        //            //{
        //            //    taxMoney = money * _taxRatio_Sport * betAmount;
        //            //}
        //            //afterTaxBonusMoney += bonusMoney - taxMoney;
        //            //if (isTrue)
        //            //    firstOrTwoMoney += afterTaxBonusMoney;
        //        }
        //    }
        //    hitCount = HCount;
        //}

        //#region 私有辅助函数
        //private void HandleBonusOrder(Ticket_Order_Running order, Ticket_Ticket_Running ticket, Ticket_AnteCode_Running ante, int level, decimal bonusMoney, decimal taxMoney
        //   , Dictionary<int, int> bonusLevel_order, Dictionary<int, int> bonusLevel_ticket, Dictionary<int, int> bonusLevel_ante)
        //{
        //    ante.BonusCount++;
        //    ticket.BonusCount++;
        //    order.BonusCount++;

        //    ante.BonusStatus = BonusStatus.Win;
        //    ticket.BonusStatus = BonusStatus.Win;
        //    order.BonusStatus = BonusStatus.Win;

        //    // TODO: 处理扣税
        //    ante.BonusMoneyBeforeTax += bonusMoney;
        //    ante.BonusMoneyAfterTax += bonusMoney - taxMoney;
        //    ticket.BonusMoneyBeforeTax += bonusMoney;
        //    ticket.BonusMoneyAfterTax += bonusMoney - taxMoney;
        //    order.BonusMoneyBeforeTax += bonusMoney;
        //    order.BonusMoneyAfterTax += bonusMoney - taxMoney;

        //    ante.BonusTime = DateTime.Now;
        //    ticket.BonusTime = DateTime.Now;
        //    order.BonusTime = DateTime.Now;

        //    if (bonusLevel_ante.ContainsKey(level))
        //    {
        //        bonusLevel_ante[level]++;
        //    }
        //    else
        //    {
        //        bonusLevel_ante.Add(level, 1);
        //    }
        //    if (bonusLevel_ticket.ContainsKey(level))
        //    {
        //        bonusLevel_ticket[level]++;
        //    }
        //    else
        //    {
        //        bonusLevel_ticket.Add(level, 1);
        //    }
        //    if (bonusLevel_order.ContainsKey(level))
        //    {
        //        bonusLevel_order[level]++;
        //    }
        //    else
        //    {
        //        bonusLevel_order.Add(level, 1);
        //    }
        //}
        //private string GetBonusLevelDescription(Dictionary<int, int> levelList)
        //{
        //    if (levelList.Count == 0) return "-1";
        //    var str = "";
        //    foreach (var key in levelList.Keys)
        //    {
        //        str += key + "|" + levelList[key] + ";";
        //    }
        //    return str;
        //}
        //private string GetBonusLevelDisplayName(Dictionary<int, int> levelList, string gameCode, string gameType)
        //{
        //    if (levelList.Count == 0) return "未中奖";
        //    var str = "";
        //    foreach (var key in levelList.Keys)
        //    {
        //        var rule = GetBonusRule(gameCode, gameType, key);
        //        str += rule.BonusLevelDisplayName + "|" + levelList[key] + "注" + ";";
        //    }
        //    return str;
        //}
        //private decimal GetAfterTaxBonusMoney(decimal beforeTaxBonus)
        //{
        //    return beforeTaxBonus;
        //}
        //private bool DoSendNotificationByPost(string url, string postData, string successFlag, int index, int retryTimes, int retryTimeSpan, ref StringBuilder msg)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(url)) return true;
        //        postData = postData.Replace("+", "%2B");
        //        string response = PostManager.Post(url, postData, Encoding.UTF8);
        //        if (!((string.IsNullOrWhiteSpace(successFlag) && (response == "1" || response.Equals("true", StringComparison.OrdinalIgnoreCase)))
        //            || (response.Equals(successFlag, StringComparison.OrdinalIgnoreCase))))
        //        {
        //            throw new Exception(string.Format("未返回期待的成功标识：{0}。实际值为：{1}。", successFlag, response));
        //        }
        //        msg.Append(string.Format("-->第 {0}次发送通知成功", index));
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        msg.Append(string.Format("-->第 {0}次发送通知失败", index));
        //        msg.Append("-->" + ex.Message);

        //        if (++index > retryTimes)
        //        {
        //            return false;
        //        }
        //        Thread.Sleep(retryTimeSpan);
        //        return DoSendNotificationByPost(url, postData, successFlag, index, retryTimes, retryTimeSpan, ref msg);
        //    }
        //}
        private string ReadFileString(string fileName)
        {
            string strResult = PostManager.Get(fileName, Encoding.UTF8);

            if (!string.IsNullOrEmpty(strResult))
            {
                if (strResult.ToLower().StartsWith("var"))
                {
                    string[] strArray = strResult.Split('=');
                    if (strArray != null && strArray.Length == 2)
                    {
                        if (strArray[1].ToString().Trim().EndsWith(";"))
                        {
                            return strArray[1].ToString().Trim().TrimEnd(';');
                        }
                        return strArray[1].ToString().Trim();
                    }
                }
            }
            return strResult;
        }
        //private BonusRuleInfo GetBonusRule(string gameCode, string gameType, int bonusLevel)
        //{
        //    var ruleList = new Common_ConfigManager().QueryBonusRuleListByGameType(gameCode, gameType);
        //    foreach (var rule in ruleList)
        //    {
        //        if (rule.BonusLevel == bonusLevel)
        //        {
        //            return rule;
        //        }
        //    }
        //    throw new BonusException("未定义此中奖规则：" + gameCode + "-" + gameType + " - " + bonusLevel);
        //}
        //public List<string> AddNotice(string text, string innerKey, NoticeType noticeType)
        //{
        //    var list = new List<string>();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var issuseManager = new Ticket_IssuseManager();
        //        var userManager = new Common_UserManager();
        //        var noticeManager = new Common_NoticeManager();

        //        var agentList = userManager.QueryNoticeAgentList();
        //        foreach (var item in agentList)
        //        {
        //            list.AddRange(this.AddNotice(noticeManager, noticeType, text, item, innerKey));
        //        }

        //        // 提交事务
        //        tran.CommitTran();
        //    }
        //    return list;
        //}
        //#endregion

        //#region 竞彩辅助函数

        private IList<T> GetOddsList_JingCai<T>(string gameCode, string gameType, string flag)
            where T : JingCaiMatchBase
        {
            var fileName = string.Format(@"{3}/{0}/{1}_SP{2}.json", gameCode, gameType, flag, _baseDir);
            //if (!File.Exists(fileName))
            //{
            //    throw new ArgumentException("竞彩赔率数据文件不存在 - " + gameCode + "." + gameType);
            //}
            var json = ReadFileString(fileName);
            var resultList = JsonSerializer.Deserialize<List<T>>(json);
            return resultList;
        }
        //private IList<JingCai_MatchInfo> GetMatchList_JingCai(string gameCode, string matchData)
        //{
        //    var fileName = "";
        //    if (!string.IsNullOrEmpty(matchData))
        //    {
        //        fileName = string.Format("{2}/{0}/{1}/Match_List.json", gameCode, matchData, _baseDir);
        //    }
        //    else
        //    {
        //        fileName = string.Format("{1}/{0}/Match_List.json", gameCode, _baseDir);
        //    }
        //    //if (!File.Exists(fileName))
        //    //{
        //    //    throw new ArgumentException("竞彩比赛数据文件不存在");
        //    //}
        //    var json = ReadFileString(fileName);
        //    var resultList = JsonSerializer.Deserialize<List<JingCai_MatchInfo>>(json);
        //    return resultList;
        //}
        //private IList<JingCai_MatchResultInfo> GetMatchResultList_JingCai(string gameCode, string matchData)
        //{
        //    var fileName = "";
        //    if (!string.IsNullOrEmpty(matchData))
        //    {
        //        fileName = string.Format(@"{2}\{0}\{1}\Match_Result_List.json", gameCode, matchData, _baseDir);

        //    }
        //    else
        //    {
        //        fileName = string.Format(@"{1}\{0}\Match_Result_List.json", gameCode, _baseDir);
        //    }
        //    //if (!File.Exists(fileName))
        //    //{
        //    //    throw new ArgumentException("竞彩比赛结果数据不存在或尚未开奖");
        //    //}
        //    var json = ReadFileString(fileName);
        //    var resultList = JsonSerializer.Deserialize<List<JingCai_MatchResultInfo>>(json);
        //    return resultList;
        //}

        //#endregion

        ///// <summary>
        ///// 开启奖期
        ///// </summary>
        //public string[] OpenIssuse(GatewayIssuseCollection collection, string gameCode)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var issuseManager = new Ticket_IssuseManager();
        //        var userManager = new Common_UserManager();
        //        var noticeManager = new Common_NoticeManager();

        //        //var gameInfo = GameListAnalyzer.GetAllGameList().FirstOrDefault(g => g.GameCode.Equals(gameCode, StringComparison.OrdinalIgnoreCase));
        //        var gameInfo = new Common_ConfigManager().GetAllGameList().FirstOrDefault(g => g.GameCode.Equals(gameCode, StringComparison.OrdinalIgnoreCase));
        //        var list = new List<Ticket_Issuse>();
        //        var noticeList = new List<string>();
        //        var issuseStart = "";
        //        ObjectConvert.ConvertInfoListToEntityList<GatewayIssuseCollection, GatewayIssuse, List<Ticket_Issuse>, Ticket_Issuse>(collection, ref list
        //            , () => new Ticket_Issuse()
        //            , (info, entity) =>
        //            {
        //                if (info.StartTime >= info.BettingStopTime || info.BettingStopTime > info.OfficialStopTime)
        //                {
        //                    throw new ArgumentException(string.Format("奖期时间设置错误 - 玩法：{0}；期号：{1}；开始时间：{2:yyyy-MM-dd HH:mm:ss}；投注截至：{3:yyyy-MM-dd HH:mm:ss}；官方截至：{4:yyyy-MM-dd HH:mm:ss}", info.GameCode, info.IssuseNumber, info.StartTime, info.BettingStopTime, info.OfficialStopTime));
        //                }
        //                if (issuseStart == "")
        //                {
        //                    issuseStart = info.IssuseNumber;
        //                }
        //                entity.IssuseId = info.GameCode + "|" + info.IssuseNumber;
        //                entity.GameCode = info.GameCode;
        //                entity.Status = IssuseStatus.Running;
        //                entity.IssuseDate = info.OfficialStopTime.ToString(gameInfo.CacheDateFormat);

        //                noticeList.Add(string.Format("{0}#{1:yyyy-MM-dd HH:mm:ss}#{2:yyyy-MM-dd HH:mm:ss}#{3:yyyy-MM-dd HH:mm:ss}", info.IssuseNumber, info.StartTime, info.BettingStopTime, info.OfficialStopTime));
        //            });
        //        issuseManager.AddIssuseList(list);

        //        #region 添加通知数据

        //        var totalCount = collection.Count;
        //        var noticeText = string.Join("|", noticeList);
        //        var agentList = userManager.QueryNoticeAgentList();
        //        var noticeIdList = new List<string>();
        //        var text = string.Format("{0}_{1}_{2}", gameCode, totalCount, noticeText);
        //        foreach (var agent in agentList)
        //        {
        //            noticeIdList.AddRange(AddNotice(noticeManager, NoticeType.IssuseNotice, text, agent, gameCode + "_" + issuseStart + "_" + totalCount));
        //        }

        //        if (gameCode.ToUpper().Equals("PL3") || gameCode.ToUpper().Equals("DLT"))
        //        {
        //            List<OpenIssuseInfo> issuseList = new List<OpenIssuseInfo>();
        //            foreach (var item in noticeList)
        //            {
        //                var strL = item.Split('#');
        //                issuseList.Add(new OpenIssuseInfo
        //                {
        //                    Lottery_Id = gameCode.ToUpper() == "PL3" ? "D3" : "T001",
        //                    Issue = strL[0].Substring(2).Replace("-", ""),
        //                    Start_Timestamp = Convert.ToDateTime(strL[1]),
        //                    Print_Start_Timestamp = Convert.ToDateTime(strL[1]),
        //                    End_Timestamp = Convert.ToDateTime(strL[2]),
        //                    Sys_End_Timestamp = Convert.ToDateTime(strL[3]),
        //                });
        //            }
        //            var content = JsonSerializer.Serialize(issuseList);
        //        }
        //        #endregion

        //        tran.CommitTran();

        //        return noticeIdList.ToArray();
        //    }
        //}
        //// 批量开启高频彩奖期
        //public string[] OpenIssuseBatch_Fast(string gameCode, DateTime date, int bettingOffset, Func<DateTime, bool> checkIsOpenDay, Dictionary<int, double> phases, string issuseFormat, Func<int, DateTime, DateTime> eachIssuseOffsetHander = null, int dayIndex = 0)
        //{
        //    var i = 1;
        //    var collection = new GatewayIssuseCollection();
        //    var offset = date.Date;
        //    foreach (var phase in phases)
        //    {
        //        if (phase.Key <= 0)
        //        {
        //            offset = offset.AddMinutes(phase.Value);
        //            continue;
        //        }
        //        for (; i <= phase.Key; i++)
        //        {
        //            if (eachIssuseOffsetHander != null)
        //            {
        //                offset = eachIssuseOffsetHander(i, offset);
        //            }
        //            var issuseNumber = string.Format(issuseFormat, date, i, dayIndex);
        //            var info = new GatewayIssuse
        //            {
        //                GameCode = gameCode,
        //                IssuseNumber = issuseNumber,
        //            };
        //            //info.StartTime = startTime;
        //            info.StartTime = offset;
        //            offset = offset.AddMinutes(phase.Value);
        //            info.OfficialStopTime = offset;
        //            info.BettingStopTime = offset;
        //            //info.BettingStopTime = offset.AddSeconds(-bettingOffset);

        //            collection.Add(info);
        //        }
        //    }
        //    return OpenIssuse(collection, gameCode);
        //}
        //// 批量开启低频彩以及每日彩奖期，如 双色球、福彩3D
        //public string[] OpenIssuseBatch_Slow(string gameCode, int year, string issuseFormat, Func<DateTime, bool> checkIsOpenDay, Func<DateTime, DateTime> getOfficialStopTime, int bettingStopTimeOffsetMinutes)
        //{
        //    var startDate = new DateTime(year, 1, 1);
        //    var endDate = new DateTime(year + 1, 1, 1);
        //    var i = 1;
        //    var startTime = startDate;
        //    var collection = new GatewayIssuseCollection();
        //    for (var date = startDate; date < endDate; date = date.AddDays(1))
        //    {
        //        if (checkIsOpenDay(date))
        //        {
        //            var issuseNumber = string.Format(issuseFormat, date, i++);
        //            var info = new GatewayIssuse
        //            {
        //                GameCode = gameCode,
        //                IssuseNumber = issuseNumber,
        //            };
        //            info.StartTime = startTime;
        //            info.OfficialStopTime = getOfficialStopTime(date);
        //            info.BettingStopTime = info.OfficialStopTime;
        //            //info.BettingStopTime = info.OfficialStopTime.AddMinutes(-bettingStopTimeOffsetMinutes);

        //            collection.Add(info);

        //            startTime = info.OfficialStopTime.AddHours(3);
        //        }
        //    }
        //    return OpenIssuse(collection, gameCode);
        //}

        ///// <summary>
        ///// 删除奖期（按时间）
        ///// </summary>
        //public string[] DeleteIssuse(string gameCode, DateTime startTime, DateTime endTime)
        //{
        //    var noticeIdList = new List<string>();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var issuseManager = new Ticket_IssuseManager();
        //        var userManager = new Common_UserManager();
        //        var noticeManager = new Common_NoticeManager();

        //        var deleteIssuseList = new List<string>();
        //        var issuseList = issuseManager.QueryIssuseListByOfficialStopTime(gameCode, startTime, endTime);
        //        foreach (var item in issuseList)
        //        {
        //            deleteIssuseList.Add(item.IssuseNumber);
        //            issuseManager.DeleteIssuse(item);
        //        }

        //        //通知格式：GameCode_期号|期号|期  如：CQSSC_20140419-001|20140419-002|20140419-003
        //        var text = string.Format("{0}_{1}", gameCode, string.Join("|", deleteIssuseList));
        //        var agentList = userManager.QueryNoticeAgentList();
        //        foreach (var agent in agentList)
        //        {
        //            noticeIdList.AddRange(AddNotice(noticeManager, NoticeType.DeleteIssuse, text, agent, string.Format("{0}_{1}_{2}", gameCode, startTime, endTime)));
        //        }

        //        tran.CommitTran();
        //    }
        //    return noticeIdList.ToArray();
        //}

        ///// <summary>
        ///// 删除奖期（按期号）
        ///// </summary>
        //public string[] DeleteIssuseToIssuse(string gameCode, string issuseNumber)
        //{
        //    var noticeIdList = new List<string>();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var issuseManager = new Ticket_IssuseManager();
        //        var userManager = new Common_UserManager();
        //        var noticeManager = new Common_NoticeManager();

        //        var deleteIssuseList = new List<string>();
        //        var issuseEntity = issuseManager.QueryIssuseListByIssuse(gameCode, issuseNumber);
        //        if (issuseEntity == null)
        //            throw new Exception("没有找到该彩种对应期号数据！");

        //        deleteIssuseList.Add(issuseEntity.IssuseNumber);
        //        issuseManager.DeleteIssuse(issuseEntity);

        //        //通知格式：GameCode_期号|期号|期  如：CQSSC_20140419-001|20140419-002|20140419-003
        //        var text = string.Format("{0}_{1}", gameCode, string.Join("|", deleteIssuseList));
        //        var agentList = userManager.QueryNoticeAgentList();
        //        foreach (var agent in agentList)
        //        {
        //            noticeIdList.AddRange(AddNotice(noticeManager, NoticeType.DeleteIssuse, text, agent, string.Format("{0}_{1}", gameCode, issuseNumber)));
        //        }

        //        tran.CommitTran();
        //    }
        //    return noticeIdList.ToArray();
        //}

        //#region  通知相关
        //public NoticeInfoCollection QueryWaitingNoticeCollection(int pageIndex, int pageSize)
        //{
        //    var manager = new Common_NoticeManager();
        //    int totalCount;
        //    var noticeList = manager.QueryWaitingNoticeList(pageIndex, pageSize, out totalCount);

        //    var collection = new NoticeInfoCollection
        //    {
        //        TotalCount = totalCount,
        //        NoticeList = noticeList,
        //    };
        //    return collection;
        //}
        //public NoticeInfoCollection QueryCompletedNoticeCollection(string innerKey, NoticeType? noticeType, string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var manager = new Common_NoticeManager();
        //    int totalCount;
        //    var notice = -1;
        //    if (noticeType.HasValue)
        //        notice = (int)noticeType.Value;
        //    var noticeList = manager.QueryCompletedNoticeList(innerKey, notice, agentId, startTime, endTime, pageIndex, pageSize, out totalCount);

        //    var collection = new NoticeInfoCollection
        //    {
        //        TotalCount = totalCount,
        //        NoticeList = noticeList,
        //    };
        //    return collection;
        //}
        ///// <summary>
        ///// 重发通知
        ///// </summary>
        //public void ResendNotification(string noticeId)
        //{
        //    if (string.IsNullOrEmpty(noticeId)) throw new WcfException("通知Id异常！");
        //    var notice = new Common_NoticeManager().GetCompletedNotice(noticeId);
        //    if (notice == null) throw new WcfException("没有该通知ID成功记录！");
        //    //var agent = new Common_UserManager(DbAccessHelper.DbAccess).GetUser(notice.ReceiveAgentId);
        //    var noticeUrl = notice.ReceiveUrlRoot;
        //    if (string.IsNullOrEmpty(noticeUrl)) throw new WcfException("通知地址异常！");

        //    StringBuilder msgBuilder = new StringBuilder();
        //    msgBuilder.Append("开始发送：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-->");
        //    msgBuilder.Append("-->" + noticeUrl);
        //    msgBuilder.Append("-->" + notice.PostDataString);
        //    bool success = DoSendNotificationByPost(noticeUrl, notice.PostDataString, notice.SuccessFlag, 0, notice.MaxRetryTimes, notice.RetryTimeSpan, ref msgBuilder);
        //    msgBuilder.AppendLine();
        //}
        //public string GetWaitingNoticeRemark(string noticeId)
        //{
        //    var notice = new Common_NoticeManager().GetRunningNotice(noticeId);
        //    if (notice == null)
        //    {
        //        throw new ArgumentException("通知不存在或者通知已经发送成功");
        //    }
        //    return notice.Remark;
        //}
        //public string GetCompletedNoticeRemark(string noticeId)
        //{
        //    var notice = new Common_NoticeManager().GetCompletedNotice(noticeId);
        //    if (notice == null)
        //    {
        //        throw new ArgumentException("通知不存在或者通知已经发送成功");
        //    }
        //    return notice.Remark;
        //}
        //public void DeleteNotification(string noticeId)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var noticeTranManager = new Common_NoticeManager();
        //        var notice = noticeTranManager.GetRunningNotice(noticeId);
        //        if (notice == null)
        //        {
        //            throw new ArgumentException("通知不存在或者通知已经发送成功");
        //        }
        //        noticeTranManager.DeleteRunningNotice(notice);

        //        tran.CommitTran();
        //    }
        //}
        //#endregion


        //#region 客户端相关
        ///// <summary>
        ///// 添加客户端用户
        ///// </summary>
        //public void AddClientUser(string loginName, string displayName, string password)
        //{
        //    var manager = new Common_UserManager();
        //    var userId = manager.GetMaxUserId();

        //    //UserType=Opertioner,Admin
        //    password = Encipherment.MD5(password);
        //    manager.AddCommon_ClientUser(new Common_ClientUser
        //    {
        //        CreateTime = DateTime.Now,
        //        CurrentToken = string.Empty,
        //        UserStatus = EnableStatus.Enable,
        //        UserId = userId,
        //        DisplayName = displayName,
        //        LoginName = loginName,
        //        Password = password,
        //        PartnerId = string.Empty,
        //        UserType = "Opertioner",
        //    });
        //}
        ///// <summary>
        ///// 更新客户端用户信息
        ///// </summary>
        //public void UpdateClientUser(string userId, string displayName, string password)
        //{
        //    var manager = new Common_UserManager();
        //    var entity = manager.GetClientUser(userId);
        //    if (entity == null)
        //        throw new Exception(string.Format("查询用户{0}出错", userId));

        //    entity.DisplayName = displayName;
        //    if (!string.IsNullOrEmpty(password))
        //    {
        //        password = Encipherment.MD5(password);
        //        entity.Password = password;
        //    }
        //    manager.UpdateCommon_ClientUser(entity);
        //}

        ///// <summary>
        ///// 查询奖期
        ///// </summary>
        ///// <returns></returns>
        //public CTZQ_Issuse_ListInfoCollection QueryCTZQ_IssuseInfoList(string gameCode, string gameType)
        //{
        //    var colletion = new CTZQ_Issuse_ListInfoCollection();
        //    var manager = new CTZQ_MatchManager();
        //    colletion.AddRange(manager.QueryCTZQ_Issuse_List(gameCode, gameType));
        //    return colletion;
        //}

        ///// <summary>
        ///// 竞猜足球SP获取
        ///// </summary>
        //public JCZQ_SPInfoCollection GetJCZQAllSp()
        //{
        //    var collection = new JCZQ_SPInfoCollection();
        //    var manager = new JCZQ_OddsManager();
        //    collection.AddRange(manager.GetJCZQAllSp());
        //    return collection;
        //}

        ///// <summary>
        ///// 竞猜足球SP获取
        ///// </summary>
        //public JCLQ_SPInfoCollection GetJCLQAllSp()
        //{
        //    var collection = new JCLQ_SPInfoCollection();
        //    var manager = new JCLQ_OddsManager();
        //    collection.AddRange(manager.GetJCLQAllSp());
        //    return collection;
        //}

        //#endregion

        //public void HandlePrizeOrder(string gameCode, string orderId)
        //{
        //    var tiketId = orderId.Split('|');
        //    if (tiketId.Length < 3 && gameCode != "CTZQ")
        //        return;
        //    var num = 0;
        //    switch (gameCode)
        //    {
        //        case "BJDC":
        //            var collection_BJDC = IsPrizeOrder_BJDC(tiketId[0]);
        //            num = 1;
        //            if (collection_BJDC == null || collection_BJDC.Count() == 0)
        //                return;
        //            try
        //            {
        //                num = 2;
        //                var noticeId = PrizeOrder_BJDC(tiketId[1], tiketId[0], collection_BJDC, _prizedMaxMoney);
        //                num = 3;
        //                // 开启线程发送通知
        //                new Thread(() => SendNotification(noticeId.ToArray())).Start();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(tiketId[0] + "订单派奖失败!" + num + ex.Message);
        //            }
        //            break;
        //        case "JCLQ":
        //            var collection_JCLQ = IsPrizeOrder_JCLQ(tiketId[0], tiketId[2]);
        //            if (collection_JCLQ == null || collection_JCLQ.Count() == 0)
        //                return;
        //            try
        //            {
        //                var noticeId = PrizeOrder_JCLQ(tiketId[1], tiketId[0], collection_JCLQ, _prizedMaxMoney);
        //                // 开启线程发送通知
        //                new Thread(() => SendNotification(noticeId.ToArray())).Start();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(tiketId[0] + "订单派奖失败!错误" + ex.Message);
        //            }
        //            break;
        //        case "JCZQ":
        //            var collection_JCZQ = IsPrizeOrder_JCZQ(tiketId[0], tiketId[2]);
        //            if (collection_JCZQ == null || collection_JCZQ.Count() == 0)
        //                return;
        //            try
        //            {
        //                var noticeId = PrizeOrder_JCZQ(tiketId[1], tiketId[0], collection_JCZQ, _prizedMaxMoney);
        //                // 开启线程发送通知
        //                new Thread(() => SendNotification(noticeId.ToArray())).Start();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(tiketId[0] + "订单派奖失败! 错误" + ex.Message);
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //private bool CanPrize_CTZQ(string gameType, int totalBonusCount, string winNumber)
        //{
        //    if (gameType.Equals("T14C") || gameType.Equals("TR9"))
        //    {
        //        return totalBonusCount > 0;
        //    }
        //    else
        //    {
        //        return !winNumber.Contains("*");
        //    }
        //}
        ///// <summary>
        ///// 查询按照代理商订单列表  （彩种、投注时间段）
        ///// </summary>
        //public Common_OrderAllListInfoCollection QueryCommon_OrderAllListToBetTime(string gameCode, string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var manager = new Common_OrderManager();
        //    int totalCount;
        //    decimal totalMoneyCount = 0M;
        //    decimal totalMoneyWinCount = 0M;
        //    decimal totalMoneyLoseCount = 0M;
        //    decimal bonusMoneyPrevTaxCount = 0M;
        //    decimal bonusMoneyAfterTaxCount = 0M;
        //    var orderList = manager.QueryCommon_OrderAllListToBetTime(gameCode, agentId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalMoneyCount, out totalMoneyWinCount, out totalMoneyLoseCount, out bonusMoneyPrevTaxCount, out bonusMoneyAfterTaxCount);
        //    var collection = new Common_OrderAllListInfoCollection
        //    {
        //        TotolCount = totalCount,
        //        TotalMoneyCount = totalMoneyCount,
        //        TotalMoneyWinCount = totalMoneyWinCount,
        //        TotalMoneyLoseCount = totalMoneyLoseCount,
        //        BonusMoneyAfterTaxCount = bonusMoneyPrevTaxCount,
        //        BonusMoneyPrevTaxCount = bonusMoneyAfterTaxCount,
        //        Common_OrderAllListInfo = orderList
        //    };

        //    return collection;
        //}
        ///// <summary>
        ///// 查询按代理已经结算的订单 （彩种、派奖时间）	
        ///// </summary>
        //public Common_OrderAllListInfoCollection QueryCommon_OrderAllListToPrizedTime(string gameCode, string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var manager = new Common_OrderManager();
        //    int totalCount;
        //    decimal totalMoneyCount = 0M;
        //    decimal totalMoneyWinCount = 0M;
        //    decimal totalMoneyLoseCount = 0M;
        //    decimal bonusMoneyPrevTaxCount = 0M;
        //    decimal bonusMoneyAfterTaxCount = 0M;
        //    var orderList = manager.QueryCommon_OrderAllListToPrizedTime(gameCode, agentId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalMoneyCount, out totalMoneyWinCount, out totalMoneyLoseCount, out bonusMoneyPrevTaxCount, out bonusMoneyAfterTaxCount);
        //    var collection = new Common_OrderAllListInfoCollection
        //    {
        //        TotolCount = totalCount,
        //        TotalMoneyCount = totalMoneyCount,
        //        TotalMoneyWinCount = totalMoneyWinCount,
        //        TotalMoneyLoseCount = totalMoneyLoseCount,
        //        BonusMoneyAfterTaxCount = bonusMoneyPrevTaxCount,
        //        BonusMoneyPrevTaxCount = bonusMoneyAfterTaxCount,
        //        Common_OrderAllListInfo = orderList
        //    };

        //    return collection;
        //}
        //public void MoneyConversion(string userId, decimal money, ConversionFunds conversionFunds)
        //{
        //    var manager = new Common_UserManager();
        //    var entity = manager.GetUser(userId);
        //    if (entity == null)
        //        throw new Exception("查询代理出错！");
        //    if (money <= 0M)
        //        throw new Exception("转用资金错误，必须大于0。");
        //    if (conversionFunds == ConversionFunds.General)
        //    {
        //        entity.CommissionBalance -= money;
        //        if (entity.CommissionBalance < 0M)
        //            throw new Exception("转用金额大于佣金金额。");
        //        var balance = entity.CreditBalance - entity.CurrentCreditBalance + money;
        //        entity.CurrentCreditBalance = (balance) > 0 ? entity.CreditBalance : entity.CurrentCreditBalance + money;
        //        entity.EnableBalance += (balance) > 0 ? balance : 0;
        //        manager.UpdateUser(entity);
        //    }
        //    if (conversionFunds == ConversionFunds.Bonus)
        //    {
        //        entity.BonusBalance -= money;
        //        if (entity.BonusBalance < 0M)
        //            throw new Exception("转用金额大于奖金金额。");
        //        var balance = entity.CreditBalance - entity.CurrentCreditBalance + money;
        //        entity.CurrentCreditBalance = (balance) > 0 ? entity.CreditBalance : entity.CurrentCreditBalance + money;
        //        entity.EnableBalance += (balance) > 0 ? balance : 0;
        //        manager.UpdateUser(entity);
        //    }
        //}

        ///// <summary>
        ///// 查询进行中的订单
        ///// </summary>
        //public OrderWaitInfoCollection QueryOderWaitList(string gameCode)
        //{
        //    var collection = new OrderWaitInfoCollection();
        //    if (gameCode.Equals("BJDC", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var maager = new BJDC_OrderManager();
        //        var entity_BJDC = maager.QueryOderWaitList();
        //        if (entity_BJDC != null)
        //            collection.AddRange(entity_BJDC);
        //    }
        //    else if (gameCode.Equals("JCZQ", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var maager = new JCZQ_OrderManager();
        //        var entity_JCZQ = maager.QueryOderWaitList();
        //        if (entity_JCZQ != null)
        //            collection.AddRange(entity_JCZQ);
        //    }
        //    else if (gameCode.Equals("JCLQ", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var maager = new JCLQ_OrderManager();
        //        var entity_JCLQ = maager.QueryOderWaitList();
        //        if (entity_JCLQ != null)
        //            collection.AddRange(entity_JCLQ);
        //    }
        //    else
        //    {
        //        var maager = new Ticket_OrderManager();
        //        var entity = maager.QueryOderWaitList();
        //        if (entity != null)
        //            collection.AddRange(entity);
        //    }
        //    return collection;
        //}

        //public string GetGatewayConfigByPath(string path)
        //{
        //    return new Common_ConfigManager().GetGatewayConfigByPath(path);
        //}

        public bool CanPrize_CTZQ(string gameType, int totalBonusCount, string winNumber)
        {
            if (gameType.Equals("T14C") || gameType.Equals("TR9"))
            {
                return totalBonusCount > 0;
            }
            else
            {
                return !winNumber.Contains("*");
            }
        }

        public void UpdateBonusPool_SZC(string gameCode, string issuseNumber)
        {
            var bonusPoolList = GetBonusPoolList_SZC(gameCode, issuseNumber);
            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();

                var bonusManager = new Ticket_BonusManager();
                foreach (var info in bonusPoolList.GradeList)
                {
                    var entity = bonusManager.GetBonusPool(gameCode, "", issuseNumber, info.Grade);
                    if (entity == null)
                    {
                        entity = new Ticket_BonusPool
                        {
                            Id = string.Format("{0}|{1}|{2}", bonusPoolList.GameCode, bonusPoolList.IssuseNumber, info.Grade),
                            GameCode = gameCode,
                            GameType = "",
                            IssuseNumber = bonusPoolList.IssuseNumber,
                            BonusLevel = info.Grade,
                            BonusCount = info.BonusCount,
                            BonusLevelDisplayName = info.GradeName,
                            BonusMoney = info.BonusMoney,
                            WinNumber = bonusPoolList.WinNumber,
                            CreateTime = DateTime.Now,
                        };
                        bonusManager.AddBonusPool(entity);
                    }
                }

                tran.CommitTran();
            }

            #region 对依赖奖池派奖的奖期派奖
            //var issuse = new Ticket_IssuseManager();
            //var iss_Entity = issuse.QueryIssuseListByIssuse(gameCode, issuseNumber);
            //if (iss_Entity != null && iss_Entity.Status == IssuseStatus.Awarding && !string.IsNullOrEmpty(iss_Entity.WinNumber))
            //{
            //    var noticeIdList = PrizeIssuse(gameCode, issuseNumber, bonusPoolList.WinNumber);
            //    try
            //    {
            //        new Thread(() =>
            //        {
            //            try
            //            {
            //                foreach (var noticeId in noticeIdList)
            //                {
            //                    // 开启线程发送通知
            //                    //new Thread(() => admin.SendNotification(noticeId)).Start();
            //                    SendNotification(noticeId);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Common.Log.LogWriterGetter.GetLogWriter().Write("TicketGateWayAdmin", "UpdateBonusPool_SZC", Common.Log.LogType.Information, "test", ex.ToString());
            //            }

            //        }).Start();
            //    }
            //    catch (Exception ex)
            //    {
            //        Common.Log.LogWriterGetter.GetLogWriter().Write("TicketGateWayAdmin2", "UpdateBonusPool_SZC", Common.Log.LogType.Information, "test", ex.ToString());
            //    }
            //    //foreach (var noticeId in noticeIdList)
            //    //{
            //    //    // 开启线程发送通知
            //    //    new Thread(() => SendNotification(noticeId)).Start();
            //    //}
            //}
            #endregion

        }

        private SZC_BonusPoolInfo GetBonusPoolList_SZC(string gameCode, string issuseNumber)
        {
            var fileName = string.Format(@"{2}\{0}\{0}_{1}.json", gameCode, issuseNumber, _baseDir);
            //if (!File.Exists(fileName))
            //{
            //    throw new ArgumentException("奖池不存在或尚未开奖");
            //}
            var json = ReadFileString(fileName);
            var resultList = JsonSerializer.Deserialize<SZC_BonusPoolInfo>(json);
            return resultList;
        }

        ///// <summary>
        ///// 查询为出票成功却奖期结束的订单
        ///// </summary>
        ///// <returns></returns>
        //public Common_OrderAllListInfoCollection QueryTicketingOrderList()
        //{
        //    var collection = new Common_OrderAllListInfoCollection();
        //    var maager = new Ticket_OrderManager();
        //    var ticketingOrderList = maager.QueryTicketingOrderList();
        //    foreach (var item in ticketingOrderList)
        //    {
        //        collection.Common_OrderAllListInfo.Add(item);
        //    }
        //    var ticketingOrderList_CTZQ = maager.QueryTicketingOrderList_CTZQ();
        //    foreach (var item in ticketingOrderList_CTZQ)
        //    {
        //        collection.Common_OrderAllListInfo.Add(item);
        //    }
        //    return collection;
        //}

        ///// <summary>
        ///// 发送短信
        ///// </summary>
        //public void SendProxySMS(string noticeMobile, string content)
        //{
        //    //SMSAgent.Name|SMSAgent.UserId|SMSAgent.Password|SMSAgent.Attach|无限通短信
        //    var valus = new Common_ConfigManager().GetGatewayConfigByPath("SMSAGent");
        //    var SMSAGentList = valus.Split('|');
        //    var sender = SMSSenderFactory.GetSMSSenderInstance(new SMSConfigInfo
        //    {
        //        AgentName = SMSAGentList[0].ToUpper(),
        //        UserName = SMSAGentList[1],
        //        Password = SMSAGentList[2],
        //        Attach = SMSAGentList[3],
        //    });
        //    if (string.IsNullOrEmpty(noticeMobile) || string.IsNullOrEmpty(content)) return;
        //    foreach (var item in noticeMobile.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
        //    {
        //        //SMSSenderFactory.SendSMS(item, content);
        //        sender.SendSMS(item, content, SMSAGentList[3]);
        //    }
        //}

        ///// <summary>
        ///// 发送短信VeeSing
        ///// </summary>
        //public void SendSMS_VeeSing(string content, string noticeMobile)
        //{
        //    //短信接入商名称|短信接入商用户号|短信接入商密码|短信接入商附加信息
        //    var valus = new Common_ConfigManager().GetGatewayConfigByPath<string>("SMSAGentVeeSing");
        //    var SMSAGentList = valus.Split('|');
        //    //VeeSing|cf_xunticaibb|cai_baob1213.14|http://121.199.16.178

        //    var result = Common.Net.SMS.SMSSenderFactory.GetSMSSenderInstance(new Common.Net.SMS.SMSConfigInfo
        //    {
        //        AgentName = SMSAGentList[0],
        //        Attach = SMSAGentList[3],
        //        Password = SMSAGentList[2],
        //        UserName = SMSAGentList[1]
        //    }).SendSMS(noticeMobile, content, SMSAGentList[3]);

        //}

        //public int ExecutionSql(string sqlUrl)
        //{
        //    return new Common_UserManager().Monitor_SQl(sqlUrl);
        //}

        //#region 竞彩比赛玩法对应开启关闭

        ///// <summary>
        ///// 添加竞彩禁赛信息
        ///// </summary>
        //public void AddDisableMatchConfig(string gameCode, string matchId, DateTime matchStartTime, string privilegesType)
        //{
        //    var manager = new Common_ConfigManager();
        //    var entity = manager.QueryDisableMatchConfigToMatchId(matchId);
        //    if (entity == null)
        //    {
        //        manager.AddDisableMatchConfig(new Common_DisableMatchConfig
        //        {
        //            GameCode = gameCode,
        //            CreateTime = DateTime.Now,
        //            MatchId = matchId,
        //            MatchStartTime = matchStartTime,
        //            PrivilegesType = privilegesType
        //        });
        //    }
        //    else
        //    {
        //        entity.PrivilegesType = privilegesType;
        //        manager.UpdateDisableMatchConfig(entity);
        //    }
        //}

        /// <summary>
        /// 查询禁赛列表
        /// </summary>
        /// <returns></returns>
        public DisableMatchConfigInfoCollection QueryDisableMatchConfigList(string gameCode)
        {
            var collection = new DisableMatchConfigInfoCollection();
            switch (gameCode.ToUpper())
            {
                case "BJDC":
                    var manager_BJDC = new BJDCMatchManager();
                    collection.AddRange(manager_BJDC.QueryBJDC_DisableMatchConfigList());
                    break;
                case "JCZQ":
                    var manager_JCZQ = new JCZQMatchManager();
                    collection.AddRange(manager_JCZQ.QueryJCZQ_DisableMatchConfigList());
                    break;
                case "JCLQ":
                    var manager_JCLQ = new JCLQMatchManager();
                    collection.AddRange(manager_JCLQ.QueryJCLQ_DisableMatchConfigList());
                    break;
                default:
                    break;
            }

            return collection;
        }

        //#endregion


        //#region 配置相关

        ///// <summary>
        ///// 查询配置列表
        ///// </summary>
        //public CoreConfigInfoCollection QueryCoreConfigList()
        //{
        //    var collection = new CoreConfigInfoCollection();
        //    var maager = new Common_ConfigManager();
        //    collection.AddRange(maager.QueryAllCoreConfig());
        //    return collection;
        //}

        ///// <summary>
        ///// 更新配置
        ///// </summary>
        //public void UpdateCoreConfig(int id, string configValue, string userId)
        //{
        //    var maager = new Common_ConfigManager();
        //    var entity = maager.QueryCommon_Config(id);
        //    if (entity == null)
        //        throw new Exception("没有找到相应的配置");
        //    entity.ConfigValue = configValue;
        //    entity.CreateTime = DateTime.Now;
        //    entity.OperationLog += string.Format(">>{0}在{1}将该配置修改为'{2}'", userId, DateTime.Now, configValue);
        //    maager.UpdateCoreConfig(entity);
        //    maager.UpdateCacheConfig();
        //}

        //#endregion

        //#region 权限相关

        //public bool CheckIsAdmin(string userId)
        //{
        //    try
        //    {
        //        return new Common_RoleManager().CheckIsAdmin(userId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new AuthException("用户身份验证失败", ex);
        //    }
        //}

        //public RoleInfo_QueryCollection GetSystemRoleCollection()
        //{
        //    using (var roleManager = new Common_RoleManager())
        //    {
        //        var list = roleManager.QueryRoleList();

        //        var collection = new RoleInfo_QueryCollection();
        //        ObjectConvert.ConvertEntityListToInfoList<IList<Common_SystemRole>, Common_SystemRole, RoleInfo_QueryCollection, RoleInfo_Query>(list, ref collection, () => new RoleInfo_Query());
        //        return collection;
        //    }
        //}

        //public RoleInfo_Query GetSystemRoleById(string roleId)
        //{
        //    using (var roleManager = new Common_RoleManager())
        //    {
        //        var role = roleManager.GetRoleById(roleId);

        //        var info = new RoleInfo_Query();
        //        ObjectConvert.ConverEntityToInfo<Common_SystemRole, RoleInfo_Query>(role, ref info);
        //        foreach (var function in role.FunctionList)
        //        {
        //            if (function.Status == EnableStatus.Enable)
        //            {
        //                var item = new RoleFunctionInfo
        //                {
        //                    FunctionId = function.FunctionId,
        //                    Mode = function.Mode,
        //                };
        //                info.FunctionList.Add(item);
        //            }
        //        }
        //        return info;
        //    }
        //}

        ////public FunctionCollection QueryConfigFunctionCollection(RoleType roleType)
        ////{
        ////    using (var roleManager = new Common_RoleManager())
        ////    {
        ////        var list = roleManager.QueryConfigFunctionList(roleType);

        ////        var collection = new FunctionCollection();
        ////        ObjectConvert.ConvertEntityListToInfoList<IList<Common_Function>, Common_Function, FunctionCollection, FunctionInfo>(list, ref collection, () => new FunctionInfo());
        ////        return collection;
        ////    }
        ////}

        //public void UpdateSystemRole(RoleInfo_Update roleInfo)
        //{
        //    using (var biz = new TicketBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        using (var roleManager = new Common_RoleManager())
        //        {
        //            var role = roleManager.GetRoleById(roleInfo.RoleId);
        //            if (role == null)
        //            {
        //                throw new ArgumentException("指定编号的角色不存在 - " + roleInfo.RoleId);
        //            }
        //            role.RoleId = roleInfo.RoleId;
        //            role.RoleName = roleInfo.RoleName;
        //            roleManager.UpdateRole(role);

        //            foreach (var item in roleInfo.AddFunctionList)
        //            {
        //                var roleFunction = roleManager.GetRoleFunction(role, item.FunctionId);
        //                if (roleFunction != null)
        //                {
        //                    throw new ArgumentException("添加权限到角色错误 - 已经包含权限\"" + roleFunction.Function.FunctionId + " - " + roleFunction.Function.DisplayName + "\"");
        //                }
        //                roleFunction = new Common_RoleFunction
        //                {
        //                    Role = role,
        //                    FunctionId = item.FunctionId,
        //                    Function = roleManager.LoadFunctionById(item.FunctionId),
        //                    Status = EnableStatus.Enable,
        //                    Mode = item.Mode,
        //                };
        //                roleManager.AddRoleFunction(roleFunction);
        //            }
        //            foreach (var item in roleInfo.ModifyFunctionList)
        //            {
        //                var roleFunction = roleManager.GetRoleFunction(role, item.FunctionId);
        //                if (roleFunction == null)
        //                {
        //                    throw new ArgumentException("修改权限错误 - 此角色尚未包含权限\"" + roleFunction.Function.FunctionId + " - " + roleFunction.Function.DisplayName + "\"");
        //                }
        //                roleFunction.Mode = item.Mode;
        //                roleManager.UpdateRoleFunction(roleFunction);
        //            }
        //            foreach (var item in roleInfo.RemoveFunctionList)
        //            {
        //                var roleFunction = roleManager.GetRoleFunction(role, item.FunctionId);
        //                if (roleFunction == null)
        //                {
        //                    throw new ArgumentException("移除权限错误 - 此角色尚未包含权限\"" + roleFunction.Function.FunctionId + " - " + roleFunction.Function.DisplayName + "\"");
        //                }
        //                roleManager.DeleteRoleFunction(roleFunction);
        //            }
        //        }

        //        biz.CommitTran();
        //    }
        //}

        //public void AddSystemRole(RoleInfo_Add roleInfo)
        //{
        //    using (var biz = new TicketBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var roleManager = new Common_RoleManager())
        //        {
        //            var role = roleManager.GetRoleById(roleInfo.RoleId);
        //            if (role != null)
        //            {
        //                throw new ArgumentException("指定编号的角色已经存在 - " + role.RoleId);
        //            }
        //            role = new Common_SystemRole
        //            {
        //                RoleId = roleInfo.RoleId,
        //                RoleName = roleInfo.RoleName,
        //                IsAdmin = roleInfo.IsAdmin,
        //                IsInner = false,
        //                RoleType = roleInfo.RoleType,
        //                FunctionList = new List<Common_RoleFunction>(),
        //            };
        //            if (!role.IsAdmin)
        //            {
        //                foreach (var item in roleInfo.FunctionList)
        //                {
        //                    var roleFunction = new Common_RoleFunction
        //                    {
        //                        Role = role,
        //                        FunctionId = item.FunctionId,
        //                        Function = roleManager.LoadFunctionById(item.FunctionId),
        //                        Status = EnableStatus.Enable,
        //                        Mode = item.Mode,
        //                    };
        //                    role.FunctionList.Add(roleFunction);
        //                }
        //                var list = roleManager.QueryFixFunctionList(roleInfo.RoleType);
        //                foreach (var item in list)
        //                {
        //                    var roleFunction = new Common_RoleFunction
        //                    {
        //                        Role = role,
        //                        FunctionId = item.FunctionId,
        //                        Function = item,
        //                        Status = EnableStatus.Enable,
        //                        Mode = "RW",
        //                    };
        //                    role.FunctionList.Add(roleFunction);
        //                }
        //            }
        //            roleManager.AddRole(role);
        //        }
        //        biz.CommitTran();
        //    }
        //}

        //public RoleInfo_QueryCollection QueryRoleCollection()
        //{
        //    using (var manage = new Common_UserManager())
        //    {
        //        return manage.QueryRoleCollection();
        //    }
        //}

        //public string QueryUserRoleIdsByUserId(string userId)
        //{
        //    using (var manage = new Common_UserManager())
        //    {
        //        return manage.QueryUserRoleIdsByUserId(userId);
        //    }
        //}

        //public void RegisterUser(Common_SystemUser user, string[] roleIds)
        //{
        //    if (roleIds.Length == 0)
        //    {
        //        throw new AuthException("必须指定角色");
        //    }
        //    using (var userManager = new Common_UserManager())
        //    {

        //        user.UserId = user.UserId;
        //        var roleList = userManager.GetRoleListByIds(roleIds);
        //        if (roleList.Count != roleIds.Length)
        //        {
        //            throw new AuthException("指定的角色可能不存在 - " + string.Join(",", roleIds));
        //        }
        //        user.RoleList = roleList;
        //        userManager.AddSystemUser(user);
        //    }
        //}

        //public void AddUser(Common_User user)
        //{
        //    var manager = new Common_UserManager();
        //    var entity = manager.ReLoadUser(user.UserId);
        //    if (entity == null)
        //    {
        //        manager.AddCommon_User(user);
        //    }

        //}

        //public void UpdateUserName(string userId, string newUserName)
        //{
        //    using (var manager = new Common_UserManager())
        //    {
        //        var user = manager.GetUser(userId);
        //        user.UserId = newUserName;
        //        user.UserName = newUserName;
        //        var pass = AuthAnalyzer.GetUserPassWord(user.UserToken);
        //        user.UserToken = AuthAnalyzer.GetUserToken(userId, pass);
        //        manager.UpdateUser(user);
        //    }
        //}

        //public void AddUserRoles(string userId, string[] roleIds)
        //{
        //    using (var userManager = new Common_UserManager())
        //    {
        //        var user = userManager.GetUserById(userId);
        //        if (user == null)
        //        {
        //            throw new ArgumentException("指定的用户不存在。");
        //        }
        //        NHibernate.NHibernateUtil.Initialize(user.RoleList);
        //        var roleList = userManager.GetRoleListByIds(roleIds);
        //        foreach (var role in roleList)
        //        {
        //            user.RoleList.Add(role);
        //        }
        //        userManager.UpdateSystemUser(user);
        //    }
        //}

        //public void RemoveUserRoles(string userId, string[] roleIds)
        //{
        //    using (var userManager = new Common_UserManager())
        //    {
        //        var user = userManager.GetUserById(userId);
        //        if (user == null)
        //        {
        //            throw new ArgumentException("指定的用户不存在。");
        //        }
        //        NHibernate.NHibernateUtil.Initialize(user.RoleList);
        //        foreach (var id in roleIds)
        //        {
        //            foreach (var role in user.RoleList)
        //            {
        //                if (role.RoleId == id)
        //                {
        //                    user.RoleList.Remove(role);
        //                    break;
        //                }
        //            }
        //        }
        //        if (user.RoleList.Count == 0)
        //        {
        //            throw new AuthException("用户必须指定至少一个角色");
        //        }
        //        userManager.UpdateSystemUser(user);
        //    }
        //}

        //#endregion


        //#region 新流程

        //private static List<string> _isPrintTicketSuccess = new List<string>();

        ///// <summary>
        ///// 订单打票成功发送通知
        ///// </summary>
        //public void PrintCompletedOrderToTicketList(string gameCode)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var ticketSuccess = new List<string>();
        //        var PrintTicketSuccess = new List<string>();
        //        var orderAllStrSql = string.Empty;
        //        var manager = new Common_GatewayHistoryManager();
        //        var ticketList = manager.PrintCompletedOrderToTicketList(gameCode);
        //        var orderidList = ticketList.GroupBy(p => p.OrderId).Select(o => o.Key).ToList();

        //        #region 判断是否出票 打票  并发送通知
        //        foreach (var item in orderidList)
        //        {
        //            var order_TicketList = ticketList.Where(p => p.OrderId == item).ToList();

        //            if (order_TicketList.Count <= 0)
        //                continue;

        //            if (order_TicketList.Where(p => p.TicketStatus == TicketStatus.Successful || p.TicketStatus == TicketStatus.Failed).Count() == order_TicketList.Count)
        //            {
        //                var winMoney = order_TicketList.Where(p => p.TicketStatus == TicketStatus.Successful).Sum(s => s.BetMoney);
        //                var loseMoney = order_TicketList.Where(p => p.TicketStatus == TicketStatus.Failed).Sum(s => s.BetMoney);
        //                var ticketWinCount = order_TicketList.Where(p => p.TicketStatus == TicketStatus.Successful).Count();
        //                var ticketLoseCount = order_TicketList.Where(p => p.TicketStatus == TicketStatus.Failed).Count();
        //                orderAllStrSql += string.Format("update Common_OrderAllList set WinMoney={0},LoseMoney={1},TicketWinCount={2},TicketLoseCount={3},TicketStatus=0 where OrderId='{4}' {5}"
        //                    , winMoney, loseMoney, ticketWinCount, ticketLoseCount, item, Environment.NewLine);
        //                //添加通知数据
        //                var userManager = new Common_UserManager();
        //                var agent = userManager.GetUser(order_TicketList.First().AgentId);
        //                var noticeManager = new Common_NoticeManager();
        //                var num = 100;
        //                var count = 1;
        //                count = order_TicketList.Count / num;
        //                if (order_TicketList.Count % num > 0)
        //                    count++;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    var str = JsonSerializer.Serialize<List<SuccessTicketListInfo>>(order_TicketList.Skip(num * i).Take(num).ToList());
        //                    str = string.Format("{0}{1}", order_TicketList.Count.ToString("D5"), str);
        //                    //添加通知数据
        //                    AddNotice(noticeManager, NoticeType.TicketSuccess_New, str, agent, item);
        //                }
        //                ticketSuccess.Add(item);
        //                if (_isPrintTicketSuccess.Contains(item))
        //                    _isPrintTicketSuccess.Remove(item);
        //                continue;
        //            }
        //            else if (order_TicketList.Where(p => p.TicketStatus == TicketStatus.Successful || p.TicketStatus == TicketStatus.Failed || p.TicketStatus == TicketStatus.Pending).Count() == order_TicketList.Count)
        //            {
        //                if (!_isPrintTicketSuccess.Contains(item))
        //                {
        //                    //添加通知数据
        //                    var userManager = new Common_UserManager();
        //                    var agent = userManager.GetUser(order_TicketList.First().AgentId);
        //                    var noticeManager = new Common_NoticeManager();
        //                    //添加通知数据
        //                    AddNotice(noticeManager, NoticeType.PrintTicketSuccess, item, agent, item);
        //                    PrintTicketSuccess.Add(item);
        //                    _isPrintTicketSuccess.Add(item);
        //                }
        //            }
        //        }
        //        #endregion

        //        #region 对出票打票完成的订单做更新处理
        //        if (gameCode.Equals("BJDC", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var BJDC_manager = new BJDC_OrderManager();
        //            if (ticketSuccess.Count > 0)
        //                BJDC_manager.UpdateTicketSuccessOrder_BJDC(ticketSuccess.ToArray());
        //            if (PrintTicketSuccess.Count > 0)
        //                BJDC_manager.UpdatePrintTicketSuccessOrder_BJDC(PrintTicketSuccess.ToArray());
        //        }
        //        else if (gameCode.Equals("JCLQ", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var JCLQ_manager = new JCLQ_OrderManager();
        //            if (ticketSuccess.Count > 0)
        //                JCLQ_manager.UpdateTicketSuccessOrder_JCLQ(ticketSuccess.ToArray());
        //            if (PrintTicketSuccess.Count > 0)
        //                JCLQ_manager.UpdatePrintTicketSuccessOrder_JCLQ(PrintTicketSuccess.ToArray());
        //        }
        //        else if (gameCode.Equals("JCZQ", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var JCZQ_manager = new JCZQ_OrderManager();
        //            if (ticketSuccess.Count > 0)
        //                JCZQ_manager.UpdateTicketSuccessOrder_JCZQ(ticketSuccess.ToArray());
        //            if (PrintTicketSuccess.Count > 0)
        //                JCZQ_manager.UpdatePrintTicketSuccessOrder_JCZQ(PrintTicketSuccess.ToArray());
        //        }
        //        else
        //        {
        //            var ticket_Manager = new Ticket_OrderManager();
        //            if (ticketSuccess.Count > 0)
        //                ticket_Manager.UpdateTicketSuccessOrder(ticketSuccess.ToArray());
        //            if (PrintTicketSuccess.Count > 0)
        //                ticket_Manager.UpdatePrintTicketSuccessOrder(PrintTicketSuccess.ToArray());
        //        }
        //        #endregion

        //        //更新总订单表的订单状态
        //        if (!string.IsNullOrEmpty(orderAllStrSql))
        //            manager.UpdateOrderAllState(orderAllStrSql);

        //        tran.CommitTran();
        //    }
        //}

        //#endregion

        //#region 数字彩自动服务派奖

        //public void PrizeOrder_SZC(string gameCode)
        //{
        //    var OrderList = new Ticket_OrderManager().QueryIsPrizeOrder(gameCode);
        //    foreach (var order in OrderList)
        //    {
        //        var orderArray = order.Split('_');
        //        if (orderArray.Length != 3) continue;
        //        PrizeByOrderAndWinNumber(orderArray[0], orderArray[2]);
        //    }
        //}

        //public void PrizeByOrderAndWinNumber(string orderId, string winNumber)
        //{
        //    var orderRunnin = new Ticket_OrderManager().GetRunningOrder(orderId);
        //    var agentList = new Common_UserManager().QueryNoticeAgentList();
        //    if (orderRunnin != null)
        //    {
        //        //未派奖
        //        PrizeOrder(orderRunnin, winNumber, agentList.ToList());
        //    }
        //    else
        //    {
        //        //已派奖
        //        PrizeOrder(orderId, winNumber, agentList.ToList());
        //    }
        //}

        //public void PrizeOrder(string orderId)
        //{
        //    var str = new Ticket_OrderManager().QueryOrderWinNumber(orderId);
        //    var order_winArray = str.Split('_');
        //    if (order_winArray.Length != 2)
        //        throw new Exception("查询订单开奖号不正确：" + str);

        //    if (string.IsNullOrEmpty(order_winArray[0]) || string.IsNullOrEmpty(order_winArray[1]))
        //        throw new Exception("订单号或开奖号为空");

        //    PrizeByOrderAndWinNumber(order_winArray[0], order_winArray[1]);
        //}

        //#endregion


        ///// <summary>
        ///// 一场订单派奖移动订单Running表重先派奖
        ///// </summary>
        //public JCZQ_Order_Running HandlePrizeOrder_JCZQ(string orderId)
        //{
        //    var order = new JCZQ_Order_Running();
        //    if (string.IsNullOrEmpty(orderId))
        //        return order;
        //    var manager = new JCZQ_OrderManager();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var jczq_P = manager.GetCompletedOrderToOrderId(orderId);
        //        if (jczq_P != null)
        //        {
        //            //var count = new Common_GatewayHistoryManager().GetHistoryTicketMachineCount(orderId);
        //            manager.DeletePrizedOrder_Error(jczq_P);
        //            order = new JCZQ_Order_Running
        //            {
        //                CompositeId = jczq_P.CompositeId,
        //                AgentId = jczq_P.AgentId,
        //                OrderId = jczq_P.OrderId,
        //                TicketGateway = jczq_P.TicketGateway,
        //                GameCode = jczq_P.GameCode,
        //                GameType = jczq_P.GameType,
        //                PlayType = jczq_P.PlayType,
        //                TotalMoney = jczq_P.TotalMoney,
        //                TotalBetCount = jczq_P.TotalBetCount,
        //                Amount = jczq_P.Amount,
        //                Attach = jczq_P.Attach,
        //                Price = jczq_P.Price,
        //                RequestTime = jczq_P.RequestTime,
        //                TotalMatchCount = jczq_P.TotalMatchCount,
        //                BettingCategory = jczq_P.BettingCategory,
        //                TicketStatus = jczq_P.TicketStatus,
        //                TotalTicketCount = -1,
        //                SuccessCount = -1,
        //                Deadline = DateTime.Now
        //            };
        //            manager.AddRunningOrder_Error(order);
        //        }

        //        tran.CommitTran();
        //    }
        //    return order;
        //}
        //public JCLQ_Order_Running HandlePrizeOrder_JCLQ(string orderId)
        //{
        //    var order = new JCLQ_Order_Running();
        //    if (string.IsNullOrEmpty(orderId))
        //        return order;
        //    var manager = new JCLQ_OrderManager();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var jclq_P = manager.GetCompletedOrderToOrderId(orderId);
        //        if (jclq_P != null)
        //        {
        //            manager.DeletePrizedOrder_Error(jclq_P);
        //            order = new JCLQ_Order_Running
        //            {
        //                CompositeId = jclq_P.CompositeId,
        //                AgentId = jclq_P.AgentId,
        //                OrderId = jclq_P.OrderId,
        //                TicketGateway = jclq_P.TicketGateway,
        //                GameCode = jclq_P.GameCode,
        //                GameType = jclq_P.GameType,
        //                PlayType = jclq_P.PlayType,
        //                TotalMoney = jclq_P.TotalMoney,
        //                TotalBetCount = jclq_P.TotalBetCount,
        //                Amount = jclq_P.Amount,
        //                Attach = jclq_P.Attach,
        //                Price = jclq_P.Price,
        //                RequestTime = jclq_P.RequestTime,
        //                TotalMatchCount = jclq_P.TotalMatchCount,
        //                BettingCategory = jclq_P.BettingCategory,
        //                TicketStatus = jclq_P.TicketStatus,
        //                TotalTicketCount = -1,
        //                SuccessCount = -1,
        //                Deadline = DateTime.Now
        //            };
        //            manager.AddRunningOrder_Error(order);
        //        }

        //        tran.CommitTran();
        //    }
        //    return order;
        //}
        //public Ticket_Order_Running HandlePrizeOrder_SZC(string orderId)
        //{
        //    var order = new Ticket_Order_Running();
        //    if (string.IsNullOrEmpty(orderId))
        //        return order;
        //    var manager = new Ticket_OrderManager();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var szc = manager.GetCompletedOrderToOrderId(orderId);
        //        if (szc != null)
        //        {
        //            manager.DeletePrizedOrder_Error(szc);
        //            order = new Ticket_Order_Running
        //            {
        //                CompositeId = szc.CompositeId,
        //                AgentId = szc.AgentId,
        //                OrderId = szc.OrderId,
        //                TicketGateway = szc.TicketGateway,
        //                GameCode = szc.GameCode,
        //                GameType = szc.GameType,
        //                IssuseNumber = szc.IssuseNumber,
        //                TotalMoney = szc.TotalMoney,
        //                TotalBetCount = szc.TotalBetCount,
        //                Amount = szc.Amount,
        //                Attach = szc.Attach,
        //                Price = szc.Price,
        //                RequestTime = szc.RequestTime,
        //                TicketStatus = szc.TicketStatus,
        //                BettingCategory = szc.BettingCategory,
        //                TotalTicketCount = -1,
        //                SuccessCount = -1,
        //            };
        //            manager.AddRunningOrder_Error(order);
        //        }

        //        tran.CommitTran();
        //    }
        //    return order;
        //}
        //public BJDC_Order_Running HandlePrizeOrder_BJDC(string orderId)
        //{
        //    var order = new BJDC_Order_Running();
        //    if (string.IsNullOrEmpty(orderId))
        //        return order;
        //    var manager = new BJDC_OrderManager();
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var bjdc_P = manager.GetCompletedOrderToOrderId(orderId);
        //        if (bjdc_P != null)
        //        {
        //            manager.DeletePrizedOrder_Error(bjdc_P);
        //            order = new BJDC_Order_Running
        //            {
        //                CompositeId = bjdc_P.CompositeId,
        //                AgentId = bjdc_P.AgentId,
        //                OrderId = bjdc_P.OrderId,
        //                TicketGateway = bjdc_P.TicketGateway,
        //                GameCode = bjdc_P.GameCode,
        //                GameType = bjdc_P.GameType,
        //                PlayType = bjdc_P.PlayType,
        //                TotalMoney = bjdc_P.TotalMoney,
        //                TotalBetCount = bjdc_P.TotalBetCount,
        //                Amount = bjdc_P.Amount,
        //                Attach = bjdc_P.Attach,
        //                Price = bjdc_P.Price,
        //                RequestTime = bjdc_P.RequestTime,
        //                TotalMatchCount = bjdc_P.TotalMatchCount,
        //                BettingCategory = bjdc_P.BettingCategory,
        //                TicketStatus = bjdc_P.TicketStatus,
        //                TotalTicketCount = -1,
        //                SuccessCount = -1,
        //            };
        //            manager.AddRunningOrder_Error(order);
        //        }

        //        tran.CommitTran();
        //    }

        //    return order;
        //}
    }
}
