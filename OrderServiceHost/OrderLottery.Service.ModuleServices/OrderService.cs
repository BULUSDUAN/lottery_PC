using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using KaSon.FrameWork.Helper;
using Lottery.Kg.ORM.Helper;
using OrderLottery.Service.IModuleServices;
using OrderLottery.Service.ModuleBaseServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Enum;

namespace OrderLottery.Service.ModuleServices
{
    [ModuleName("Order")]
    public class OrderService:DBbase, IOrderService
    {
        IKgLog log = null;
        public OrderService()
        {
            log = new Log4Log();
        }

        /// <summary>
        /// 中奖查询
        /// </summary>
        /// <param name="Model">请求实体</param>
        /// <returns></returns>
        public BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model)
        {
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            try
            {
                string sql = SqlModule.UserSystemModule.FirstOrDefault(p => p.Key == "P_Order_QueryBonusOrderList").SQL;
                var query = DB.CreateSQLQuery(sql)
                    .SetString("@UserId", Model.userId)
                    .SetString("@GameCode", Model.gameCode)
                    .SetString("@GameType", Model.gameType)
                    .SetString("@IssuseNumber", Model.issuseNumber)
                    .SetInt("@CompleteData", Model.completeData)
                    .SetString("@Key_UID_UName_SchemeId", Model.key)
                    .SetInt("@PageIndex", Model.pageIndex)
                    .SetInt("@PageSize", Model.pageSize)
                    .SetInt("@TotalCount", 0);
                return query as BonusOrderInfoCollection;
            }
            catch (Exception ex)
            {
                return null;                
            }            
        }

        /// <summary>
        /// 北京单场查询开奖结果
        /// </summary>
        /// <param name="issuseNumber">期号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        public BJDCMatchResultInfo_Collection QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize)
        {
            var query = from r in DB.CreateQuery<C_BJDC_MatchResult>()
                        join m in DB.CreateQuery<C_BJDC_Match>() on r.Id equals m.Id
                        where r.IssuseNumber == issuseNumber
                        orderby r.Id descending
                        select new BJDCMatchResultInfo
                        {
                            BF_Result = r.BF_Result == null ? "" : r.BF_Result,
                            BF_SP = r.BF_SP == null ? 0 : r.BF_SP,
                            BQC_Result = r.BQC_Result == null ? "" : r.BQC_Result,
                            BQC_SP = r.BQC_SP == null ? 0 : r.BQC_SP,
                            CreateTime = r.CreateTime,
                            FlatOdds = m.FlatOdds == null ? 0 : m.FlatOdds,
                            GuestFull_Result = r.GuestFull_Result == null ? "" : r.GuestFull_Result,
                            GuestHalf_Result = r.GuestHalf_Result == null ? "" : r.GuestHalf_Result,
                            GuestTeamName = m.GuestTeamName,
                            HomeFull_Result = r.HomeFull_Result == null ? "" : r.HomeFull_Result,
                            HomeHalf_Result = r.HomeHalf_Result == null ? "" : r.HomeHalf_Result,
                            HomeTeamName = m.HomeTeamName,
                            Id = r.Id,
                            IssuseNumber = r.IssuseNumber,
                            LetBall = m.LetBall,
                            LoseOdds = m.LoseOdds,
                            MatchColor = m.MatchColor,
                            MatchName = m.MatchName,
                            MatchOrderId = r.MatchOrderId,
                            MatchStartTime = m.MatchStartTime,
                            MatchState = r.MatchState,
                            SPF_Result = r.SPF_Result == null ? "" : r.SPF_Result,
                            SPF_SP = r.SPF_SP,
                            SXDS_Result = r.SXDS_Result == null ? "" : r.SXDS_Result,
                            SXDS_SP = r.SXDS_SP,
                            WinOdds = m.WinOdds,
                            ZJQ_Result = r.ZJQ_Result == null ? "" : r.ZJQ_Result,
                            ZJQ_SP = r.ZJQ_SP
                        };
            if (query != null && query.Count() > 0)
            {
                BJDCMatchResultInfo_Collection list = new BJDCMatchResultInfo_Collection();
                list.ListInfo = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                return list;
            }
            else
            {
                return new BJDCMatchResultInfo_Collection();
            }            
        }

        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public UserFundDetailCollection QueryMyFundDetailList(QueryUserFundDetailParam Model)
        {
            UserAuthentication Auth = new UserAuthentication();
            var userId = Auth.ValidateUserAuthentication(Model.userToken);
            var v = ConfigHelper.ConfigInfo["QueryUserFundDetailFromCache"].ToString();
            var collection = new UserFundDetailCollection();
            if (!string.IsNullOrEmpty(v))
            {
                if (bool.Parse(v))
                {
                    var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "FundDetail", userId);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    var list = new List<C_Fund_Detail>();
                    var accountArray = Model.accountTypeList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToArray();
                    while (Model.fromDate < Model.toDate)
                    {
                        try
                        {
                            //从缓存中读取指定日期的数据
                            var dateStr = Model.fromDate.ToString("yyyyMMdd");
                            if (dateStr == DateTime.Today.ToString("yyyyMMdd"))
                            {
                                //当天的查数据库
                                var todayList = from f in DB.CreateQuery<C_Fund_Detail>()
                                                            where f.UserId == userId
                                                            && (f.CreateTime >= DateTime.Today && f.CreateTime < DateTime.Today.AddDays(1))
                                                            && (accountArray.Length == 0 || accountArray.Contains((int)f.AccountType))
                                                            select new C_Fund_Detail
                                                            {
                                                                AccountType =(int)f.AccountType,
                                                                AfterBalance = f.AfterBalance,
                                                                BeforeBalance = f.BeforeBalance,
                                                                Category = f.Category,
                                                                CreateTime = f.CreateTime,
                                                                Id = f.Id,
                                                                KeyLine = f.KeyLine,
                                                                OperatorId = f.OperatorId,
                                                                OrderId = f.OrderId,
                                                                PayMoney = f.PayMoney,
                                                                PayType = (int)f.PayType,
                                                                Summary = f.Summary,
                                                                UserId = f.UserId,
                                                            };
                                list.AddRange(todayList);
                            }
                            else
                            {
                                var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", dateStr));
                                if (System.IO.File.Exists(filePath))
                                {
                                    //有缓存文件
                                    var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                                    if (!string.IsNullOrEmpty(content))
                                    {
                                        //文件内容不为空
                                        var currentList = JsonHelper.Deserialize<List<C_Fund_Detail>>(content);
                                        var querylist = from l in currentList
                                                    where (Model.keyLine == string.Empty || l.KeyLine == Model.keyLine)
                                                    && (accountArray.Length == 0 || accountArray.Contains((int)l.AccountType))
                                                    //&& (categoryArray.Length == 0 || categoryArray.Contains(l.Category))
                                                    select l;
                                        list.AddRange(querylist.ToList());
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        Model.fromDate = Model.fromDate.AddDays(1);
                    }

                    var payInList = list.Where(p => p.PayType == (int)PayType.Payin).ToList();
                    var payOutList = list.Where(p => p.PayType == (int)PayType.Payout).ToList();
                    collection.TotalPayinCount = payInList.Count;
                    collection.TotalPayinMoney = payInList.Count <= 0 ? 0 : payInList.Sum(p => p.PayMoney);
                    collection.TotalPayoutCount = payOutList.Count;
                    collection.TotalPayoutMoney = payOutList.Count <= 0 ? 0 : payOutList.Sum(p => p.PayMoney);
                    collection.FundDetailList = list.OrderByDescending(p => p.CreateTime).Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
                    return collection;
                }
            }
            return QueryUserFundDetail(Model, userId);
        }
        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserFundDetailCollection QueryUserFundDetail(QueryUserFundDetailParam Model, string userId)
        {
            var collection = new UserFundDetailCollection();
            userId = string.IsNullOrEmpty(userId) ? string.Empty : userId;
            Model.keyLine = string.IsNullOrEmpty(Model.keyLine) ? string.Empty : Model.keyLine;
            Model.accountTypeList = string.IsNullOrEmpty(Model.accountTypeList) ? string.Empty : Model.accountTypeList;
            Model.categoryList = string.IsNullOrEmpty(Model.categoryList) ? string.Empty : Model.categoryList;
            Model.toDate = Model.toDate.AddDays(1).Date;
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            if (Model.pageSize < 10000)
                Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;

            int totalPayinCount = 0;
            decimal totalPayinMoney = 0M;
            int totalPayoutCount = 0;
            decimal totalPayoutMoney = 0M;

            if (string.IsNullOrEmpty(userId))
            {
                return new UserFundDetailCollection();
            }

            //查询账户类型 和 类别
            //var AccountTyleList = Model.accountTypeList.Split('|');
            //查询资金明细
            string AccountDetail_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_AccountDetail").SQL;
            var AccountDetail_query = DB.CreateSQLQuery(AccountDetail_sql).SetString("@UserId", userId)
                .SetString("@StartTime", Model.fromDate.ToString())
                .SetString("@EndTime", Model.toDate.ToString())
                .SetInt("@PageIndex", Model.pageIndex)
                .SetInt("@PageSize", Model.pageSize).List<C_Fund_Detail>();
            //收入条数和金额
            string IncomeAndMoney_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_IncomeAndMoney").SQL;
            var IncomeAndMoney_query = DB.CreateSQLQuery(IncomeAndMoney_sql)
                .SetString("@UserId", userId)
                .SetString("@StartTime", Model.fromDate.ToString())
                .SetString("@EndTime", Model.toDate.ToString()).First<PayTypeDetail>();
            //支出条数和金额
            string OutAndMoney_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_OutAndMoney").SQL;
            var OutAndMoney_query = DB.CreateSQLQuery(OutAndMoney_sql)
                 .SetString("@UserId", userId)
                .SetString("@StartTime", Model.fromDate.ToString())
                .SetString("@EndTime", Model.toDate.ToString()).First<PayTypeDetail>();

            //整合数据
            collection.FundDetailList = AccountDetail_query;
            totalPayinCount = IncomeAndMoney_query.PayCount;
            totalPayinMoney = IncomeAndMoney_query.TotalPayMoney;
            totalPayoutCount = OutAndMoney_query.PayCount;
            totalPayoutMoney = OutAndMoney_query.TotalPayMoney;
            return collection;
        }
        /// <summary>
        /// 查询我的充值提现
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public FillMoneyQueryInfoCollection QueryFillMoneyList(QueryFillMoneyListParam Model)
        {
            UserAuthentication Auth = new UserAuthentication();
            var userId = Auth.ValidateUserAuthentication(Model.userToken);

            var Collection = new FillMoneyQueryInfoCollection();
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            if (Model.pageSize == -1)
                Model.pageSize = int.MaxValue;
            else
                Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;

            var agentTypeList = string.Format("{0}", string.Join(',', Model.agentTypeList.Split('|', StringSplitOptions.RemoveEmptyEntries))).ToString();
            var statusList = string.Format("{0}", string.Join(',', Model.statusList.Split('|', StringSplitOptions.RemoveEmptyEntries))).ToString();
            var sourceList = string.Format("{0}", string.Join(',', Model.sourceList.Split('|', StringSplitOptions.RemoveEmptyEntries))).ToString();
            string sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_TotalRequestMoney").SQL;           
            sql = string.Format(sql, agentTypeList, statusList, sourceList);
            Collection = DB.CreateSQLQuery(sql)
                .SetString("@UserId", userId)
                .SetString("@AgentList", agentTypeList)
                .SetString("@StatusList", statusList)
                .SetString("@SourceList", sourceList)
                .SetString("@StartTime", Model.startTime.ToString("yyyy-MM-dd"))
                .SetString("@EndTime", Model.endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .SetString("@OrderId", Model.OrderId).First<FillMoneyQueryInfoCollection>();

            string TotalResponseMoney_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_TotalResponseMoney").SQL;
            TotalResponseMoney_sql = string.Format(TotalResponseMoney_sql, agentTypeList, sourceList);
            Collection = DB.CreateSQLQuery(TotalResponseMoney_sql)
                .SetString("@UserId", userId)
                .SetString("@AgentList", agentTypeList)
                .SetString("@SourceList", sourceList)
                .SetString("@StartTime", Model.startTime.ToString("yyyy-MM-dd"))
                .SetString("@EndTime", Model.endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .SetString("@OrderId", Model.OrderId).First<FillMoneyQueryInfoCollection>();

            string FillMoneyPage_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_FillMoneyPage").SQL;
            FillMoneyPage_sql = string.Format(FillMoneyPage_sql, agentTypeList, statusList, sourceList);
            Collection.FillMoneyList = DB.CreateSQLQuery(FillMoneyPage_sql)
                .SetString("@UserId", userId)
                .SetString("@AgentList", agentTypeList)
                .SetString("@SourceList", sourceList)
                .SetString("@StartTime", Model.startTime.ToString("yyyy-MM-dd"))
                .SetString("@EndTime", Model.endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .SetString("@OrderId", Model.OrderId)
                .SetInt("@PageIndex", Model.pageIndex)
                .SetInt("@PageSize", Model.pageSize).List<FillMoneyQueryInfo>();
            return Collection;
        }
        /// <summary>
        /// 查询我的投注记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public MyBettingOrderInfoCollection QueryMyBettingOrderList(QueryMyBettingOrderParam Model)
        {
            UserAuthentication Auth = new UserAuthentication();
            var userId = Auth.ValidateUserAuthentication(Model.userToken);
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;

            var Collection = new MyBettingOrderInfoCollection();
            string Count_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_MyBettingOrder").SQL;
            Collection = DB.CreateSQLQuery(Count_sql)
                .SetString("@userId", userId)
                .SetInt("@BonusStatus", (int)Model.bonusStatus)
                .SetString("@GameCode", Model.gameCode)
                .SetString("@FromDate", Model.startTime.Value.ToString("yyyy-MM-dd") ?? "")
                .SetString("@ToDate", Model.endTime.Value.ToString("yyyy-MM-dd") ?? "").First<MyBettingOrderInfoCollection>();

            string MyBettingOrdePage_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_MyBettingOrderPage").SQL;
            Collection.OrderList = DB.CreateSQLQuery(MyBettingOrdePage_sql)
                .SetString("@userId", userId)
                .SetInt("@BonusStatus", (int)Model.bonusStatus)
                .SetString("@GameCode", Model.gameCode)
                .SetString("@FromDate", Model.startTime.Value.ToString("yyyy-MM-dd") ?? "")
                .SetString("@ToDate", Model.endTime.Value.ToString("yyyy-MM-dd") ?? "")
                .SetInt("@PageIndex", Model.pageIndex)
                .SetInt("@PageSize", Model.pageSize).List<MyBettingOrderInfo>();
            return Collection;

        }
        /// <summary>
        /// 查询提现记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Withdraw_QueryInfoCollection QueryMyWithdrawList(QueryMyWithdrawParam Model)
        {
            UserAuthentication Auth = new UserAuthentication();
            var userId = Auth.ValidateUserAuthentication(Model.userToken);
            var statusList = new List<int>();
            if (Model.status.HasValue) statusList.Add((int)Model.status.Value);
            var Collection = new Withdraw_QueryInfoCollection();
            Model.endTime = Model.endTime.AddDays(1).Date;
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;

            string orderId = string.Empty;
            decimal minMoney = -1;
            decimal maxMoney = -1;
            WithdrawAgentType? agent=null;
            int sortType = -1;
            var query = from r in DB.CreateQuery<C_Withdraw>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where (userId == string.Empty || r.UserId == userId)
                        && r.RequestTime >=Model.startTime && r.RequestTime < Model.endTime
                        && (Model.status == null || r.Status == (int)Model.status)
                        && (orderId == string.Empty || r.BankCode == orderId)
                        && (agent == null || r.WithdrawAgent == (int)agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney <= maxMoney)
                        select new Withdraw_QueryInfo
                        {
                            BankCardNumber = r.BankCardNumber,
                            BankCode = r.BankCode,
                            BankName = r.BankName,
                            BankSubName = r.BankSubName,
                            CityName = r.CityName,
                            OrderId = r.OrderId,
                            ProvinceName = r.ProvinceName,
                            RequestMoney = r.RequestMoney,
                            RequestTime = r.RequestTime,
                            ResponseTime = r.ResponseTime,
                            ResponseMoney = r.ResponseMoney,
                            WithdrawAgent = (WithdrawAgentType)r.WithdrawAgent,
                            Status = (WithdrawStatus)r.Status,
                            ResponseMessage = r.ResponseMessage,
                            RequesterDisplayName = u.DisplayName,
                            RequesterUserKey = u.UserId,
                        };
            Collection.WinCount = query.Where(p => p.Status == WithdrawStatus.Success).Count();
            Collection.RefusedCount = query.Where(p => p.Status == WithdrawStatus.Refused).Count();

            Collection.TotalWinMoney = Collection.WinCount == 0 ? 0M : query.Where(p => p.Status == WithdrawStatus.Success).Sum(p => p.RequestMoney);
            Collection.TotalRefusedMoney = Collection.RefusedCount == 0 ? 0M : query.Where(p => p.Status == WithdrawStatus.Refused).Sum(p => p.RequestMoney);
            Collection.TotalCount = query.Count();
            Collection.TotalMoney = query.Count() == 0 ? 0M : query.Sum(p => p.RequestMoney);
            Collection.TotalResponseMoney = Collection.WinCount == 0 ? 0M : query.Where(p => p.ResponseMoney.HasValue == true).Sum(p => p.ResponseMoney.Value);

            if (sortType == -1)
                query = query.OrderBy(p => p.RequestTime);
            if (sortType == 0)
                query = query.OrderBy(p => p.RequestMoney);
            if (sortType == 1)
                query = query.OrderByDescending(p => p.RequestMoney);

            if (Model.pageSize == -1)
            { Collection.WithdrawList = query.ToList(); return Collection; }

            Collection.WithdrawList=query.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
            return Collection;
        }
        /// <summary>
        /// 查询指定用户创建的合买订单列表
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TogetherOrderInfoCollection QueryCreateTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model)
        {
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            var collection = new TogetherOrderInfoCollection();
            string sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryCreateTogetherOrderCount").SQL;
             collection = DB.CreateSQLQuery(sql)
                .SetString("@UserId", Model.userId)
                .SetString("@DateFrom", Model.startTime.ToString("yyyy-MM-dd"))
                .SetString("@DateTo", Model.endTime.ToString("yyyy-MM-dd"))
                .SetString("@GameCode", Model.gameCode).First<TogetherOrderInfoCollection>();

            string Page_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryCreateTogetherOrderPage").SQL;
            collection.OrderList = DB.CreateSQLQuery(Page_sql)
                .SetString("@UserId", Model.userId)
                .SetString("@DateFrom", Model.startTime.ToString("yyyy-MM-dd"))
                .SetString("@DateTo", Model.endTime.ToString("yyyy-MM-dd"))
                .SetString("@GameCode", Model.gameCode)
                .SetInt("@PageIndex",Model.pageIndex)
                .SetInt("@PageSize",Model.pageSize)
                .List<TogetherOrderInfo>();
            return collection;
        }
    }
}
