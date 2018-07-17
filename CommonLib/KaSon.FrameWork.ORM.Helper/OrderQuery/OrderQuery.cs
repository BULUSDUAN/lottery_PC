using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.Redis;
using EntityModel.RequestModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.ORM.Helper.WinNumber;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace KaSon.FrameWork.ORM.Helper.OrderQuery
{
    public class OrderQuery : DBbase
    {
        /// <summary>
        /// 中奖查询
        /// </summary>
        /// <param name="Model">请求实体</param>
        /// <returns></returns>
        public BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model)
        {
            var result = new BonusOrderInfoCollection();
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            try
            {
                string sql = SqlModule.UserSystemModule.FirstOrDefault(p => p.Key == "P_Order_QueryBonusOrderList").SQL;
                result.BonusOrderList = DB.CreateSQLQuery(sql)
                    .SetString("@UserId", Model.userId??"")
                    .SetString("@GameCode", Model.gameCode)
                    .SetInt("@PageIndex", Model.pageIndex)
                    .SetInt("@PageSize", Model.pageSize)
                    .List<BonusOrderInfo>().ToList();
                return result;
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
                        //select r
                        select new { r, m };
            var queryResult = query.ToList().Select(b => new BJDCMatchResultInfo
            {
                BF_Result = b.r.BF_Result == null ? "" : b.r.BF_Result,
                BF_SP = b.r.BF_SP == null ? 0 : b.r.BF_SP,
                BQC_Result = b.r.BQC_Result == null ? "" : b.r.BQC_Result,
                BQC_SP = b.r.BQC_SP == null ? 0 : b.r.BQC_SP,
                CreateTime = b.r.CreateTime,
                FlatOdds = b.m.FlatOdds == null ? 0 : b.m.FlatOdds,
                GuestFull_Result = b.r.GuestFull_Result == null ? "" : b.r.GuestFull_Result,
                GuestHalf_Result = b.r.GuestHalf_Result == null ? "" : b.r.GuestHalf_Result,
                GuestTeamName = b.m.GuestTeamName,
                HomeFull_Result = b.r.HomeFull_Result == null ? "" : b.r.HomeFull_Result,
                HomeHalf_Result = b.r.HomeHalf_Result == null ? "" : b.r.HomeHalf_Result,
                HomeTeamName = b.m.HomeTeamName,
                Id = b.r.Id,
                IssuseNumber = b.r.IssuseNumber,
                LetBall = b.m.LetBall,
                LoseOdds = b.m.LoseOdds,
                MatchColor = b.m.MatchColor,
                MatchName = b.m.MatchName,
                MatchOrderId = b.r.MatchOrderId,
                MatchStartTime = b.m.MatchStartTime,
                MatchState = b.r.MatchState,
                SPF_Result = b.r.SPF_Result == null ? "" : b.r.SPF_Result,
                SPF_SP = b.r.SPF_SP,
                SXDS_Result = b.r.SXDS_Result == null ? "" : b.r.SXDS_Result,
                SXDS_SP = b.r.SXDS_SP,
                WinOdds = b.m.WinOdds,
                ZJQ_Result = b.r.ZJQ_Result == null ? "" : b.r.ZJQ_Result,
                ZJQ_SP = b.r.ZJQ_SP
            });

            if (queryResult != null)
            {
                BJDCMatchResultInfo_Collection list = new BJDCMatchResultInfo_Collection();
                list.ListInfo = queryResult.Skip(pageIndex * pageSize).Take(pageSize).ToList();
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
                                                && (accountArray.Length == 0 || accountArray.Contains(f.AccountType))
                                                select new C_Fund_Detail
                                                {
                                                    AccountType = f.AccountType,
                                                    AfterBalance = f.AfterBalance,
                                                    BeforeBalance = f.BeforeBalance,
                                                    Category = f.Category,
                                                    CreateTime = f.CreateTime,
                                                    Id = f.Id,
                                                    KeyLine = f.KeyLine,
                                                    OperatorId = f.OperatorId,
                                                    OrderId = f.OrderId,
                                                    PayMoney = f.PayMoney,
                                                    PayType = f.PayType,
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
                                                        && (accountArray.Length == 0 || accountArray.Contains(l.AccountType))
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

                    var payInList = list.Where(p => p.PayType == (int)EntityModel.Enum.PayType.Payin).ToList();
                    var payOutList = list.Where(p => p.PayType == (int)EntityModel.Enum.PayType.Payout).ToList();
                    collection.TotalPayinCount = payInList.Count();
                    collection.TotalPayinMoney = payInList.Count() <= 0 ? 0 : payInList.Sum(p => p.PayMoney);
                    collection.TotalPayoutCount = payOutList.Count();
                    collection.TotalPayoutMoney = payOutList.Count() <= 0 ? 0 : payOutList.Sum(p => p.PayMoney);
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



            //查询账户类型 和 类别
            //var AccountTyleList = Model.accountTypeList.Split('|');
            //查询资金明细
            string AccountDetail_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_AccountDetail").SQL;
            if (string.IsNullOrEmpty(userId))
            {
                userId = "";
                AccountDetail_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_AccountDetailNoUserID").SQL;
            }          
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
        public FillMoneyQueryInfoCollection QueryMyFillMoneyList(QueryFillMoneyListParam Model)
        {
            UserAuthentication Auth = new UserAuthentication();
            var userId = Auth.ValidateUserAuthentication(Model.userToken);

            var Collection = new FillMoneyQueryInfoCollection();
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            if (Model.pageSize == -1)
                Model.pageSize = int.MaxValue;
            else
                Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;

            var agentTypeList = string.Format("{0}", string.Join(',', (Model.agentTypeList??"").Split('|', StringSplitOptions.RemoveEmptyEntries))).ToString();
            var statusList = string.Format("{0}", string.Join(',', (Model.statusList??"").Split('|', StringSplitOptions.RemoveEmptyEntries))).ToString();
            var sourceList = string.Format("{0}", string.Join(',', (Model.sourceList??"").Split('|', StringSplitOptions.RemoveEmptyEntries))).ToString();
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
                .SetString("@StatusList",Model.statusList)
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
            var state = -1;
            if (Model.bonusStatus != null)
            {
                state = (int)Model.bonusStatus;
            }
            var Collection = new MyBettingOrderInfoCollection();
            string Count_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_MyBettingOrder").SQL;
            Collection = DB.CreateSQLQuery(Count_sql)
                .SetString("@userId", userId)
                .SetInt("@BonusStatus", state)
                .SetString("@GameCode", Model.gameCode)
                .SetString("@FromDate", Model.startTime.Value.ToString("yyyy-MM-dd") ?? "")
                .SetString("@ToDate", Model.endTime.Value.ToString("yyyy-MM-dd") ?? "").First<MyBettingOrderInfoCollection>();

            string MyBettingOrdePage_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_MyBettingOrderPage").SQL;
            Collection.OrderList = DB.CreateSQLQuery(MyBettingOrdePage_sql)
                .SetString("@userId", userId)
                .SetInt("@BonusStatus", state)
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
            //WithdrawAgentType? agent = null;
            int? agent = null;
            int sortType = -1;
            var query = from r in DB.CreateQuery<C_Withdraw>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where (userId == "" || r.UserId == userId)
                        && r.RequestTime >= Model.startTime && r.RequestTime < Model.endTime
                        && (Model.status == null || r.Status == Model.status)
                        && (orderId == "" || r.BankCode == orderId)
                        && (agent == null || r.WithdrawAgent == agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney <= maxMoney)
                        select new { r, u };            
            var queryResult= query.ToList().Select(b=>new Withdraw_QueryInfo
                        {
                            BankCardNumber = b.r.BankCardNumber,
                            BankCode = b.r.BankCode,
                            BankName = b.r.BankName,
                            BankSubName = b.r.BankSubName,
                            CityName = b.r.CityName,
                            OrderId = b.r.OrderId,
                            ProvinceName = b.r.ProvinceName,
                            RequestMoney = b.r.RequestMoney,
                            RequestTime = b.r.RequestTime,
                            ResponseTime = b.r.ResponseTime,
                            ResponseMoney = b.r.ResponseMoney,
                            WithdrawAgent = b.r.WithdrawAgent,
                            Status = b.r.Status,
                            ResponseMessage = b.r.ResponseMessage,
                            RequesterDisplayName = b.u.DisplayName,
                            RequesterUserKey = b.u.UserId,
                        });
            Collection.WinCount = queryResult.Where(p => p.Status == (int)WithdrawStatus.Success).Count();
            Collection.RefusedCount = queryResult.Where(p => p.Status == (int)WithdrawStatus.Refused).Count();

            Collection.TotalWinMoney = Collection.WinCount == 0 ? 0M : queryResult.Where(p => p.Status == (int)WithdrawStatus.Success).Sum(p => p.RequestMoney);
            Collection.TotalRefusedMoney = Collection.RefusedCount == 0 ? 0M : queryResult.Where(p => p.Status == (int)WithdrawStatus.Refused).Sum(p => p.RequestMoney);
            Collection.TotalCount = queryResult.Count();
            Collection.TotalMoney = queryResult.Count() == 0 ? 0M : queryResult.Sum(p => p.RequestMoney);
            Collection.TotalResponseMoney = Collection.WinCount == 0 ? 0M : queryResult.Where(p => p.ResponseMoney.HasValue == true).Sum(p => p.ResponseMoney.Value);

            if (sortType == -1)
                queryResult = queryResult.OrderBy(p => p.RequestTime);
            if (sortType == 0)
                queryResult = queryResult.OrderBy(p => p.RequestMoney);
            if (sortType == 1)
                queryResult = queryResult.OrderByDescending(p => p.RequestMoney);

            if (Model.pageSize == -1)
            { Collection.WithdrawList = queryResult.ToList(); return Collection; }

            Collection.WithdrawList = queryResult.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
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
                .SetInt("@BonusStatus",(int)Model.bonus)
                .SetInt("@PageIndex", Model.pageIndex)
                .SetInt("@PageSize", Model.pageSize)
                .List<TogetherOrderInfo>();
            return collection;
        }
        /// <summary>
        /// 查询指定用户参与的合买订单
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TogetherOrderInfoCollection QueryJoinTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model)
        {
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            var collection = new TogetherOrderInfoCollection();
            string sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryJoinTogetherOrderCount").SQL;
            collection = DB.CreateSQLQuery(sql)
                .SetString("@UserId",Model.userId)
               .SetString("@GameCode", Model.gameCode)
               .SetString("@DateFrom", Model.startTime.ToString("yyyy-MM-dd"))
               .SetString("@DateTo", Model.endTime.ToString("yyyy-MM-dd")).First<TogetherOrderInfoCollection>();

            string page_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryJoinTogetherOrderPage").SQL;
            collection.OrderList = DB.CreateSQLQuery(page_sql)
                 .SetString("@UserId", Model.userId)
                .SetString("@DateFrom", Model.startTime.ToString("yyyy-MM-dd"))
                .SetString("@DateTo", Model.endTime.ToString("yyyy-MM-dd"))
                .SetString("@GameCode", Model.gameCode)
                .SetInt("@BonusStatus", (int)Model.bonus)
                .SetInt("@PageIndex", Model.pageIndex)
                .SetInt("@PageSize", Model.pageSize).List<TogetherOrderInfo>();
            return collection;
        }
        /// <summary>
        /// 从Redis查询出合买订单数据
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherListFromRedis(QuerySportsTogetherListFromRedisParam Model)
        {
            var redisKey_TogetherList = RedisKeys.Key_Core_Togegher_OrderList;
            //生成列表
            var list = new List<Sports_TogetherSchemeQueryInfo>();
            var redisList = new List<StackExchange.Redis.RedisValue>(); //RedisHelper.QuerySportsTogetherListFromRedis(redisKey_TogetherList).Result;
            foreach (var item in redisList)
            {
                try
                {
                    if (!item.HasValue) continue;
                    var t = JsonHelper.Deserialize<Sports_TogetherSchemeQueryInfo>(item.ToString());
                    list.Add(t);
                }
                catch (Exception)
                {
                }
            }

            //查询列表
            var seC = !Model.security.HasValue ? -1 : (int)Model.security.Value;
            var betC = !Model.betCategory.HasValue ? -1 : (int)Model.betCategory.Value;
            var strPro = !Model.progressState.HasValue ? "10|20|30" : ((int)Model.progressState.Value).ToString();
            var arrProg = strPro.Split('|');
            if (!string.IsNullOrEmpty(Model.gameCode))
                Model.gameCode = Model.gameCode.ToUpper();
            if (!string.IsNullOrEmpty(Model.gameType))
                Model.gameType = Model.gameType.ToUpper();
            var cache = new Sports_TogetherSchemeQueryInfoCollection();
            var query = from s in list
                        where arrProg.Contains(Convert.ToInt32(s.ProgressStatus).ToString())
                          && (betC == -1 || Convert.ToInt32(s.SchemeBettingCategory) == betC)
                          && (Model.issuseNumber == "" || s.IssuseNumber == Model.issuseNumber)
                          && (s.StopTime >= DateTime.Now)
                          && (Model.gameCode == "" || s.GameCode == Model.gameCode)
                          && (Model.gameType == "" || s.GameType == Model.gameType)
                          && (Model.minMoney == -1 || s.TotalMoney >= Model.minMoney)
                          && (Model.maxMoney == -1 || s.TotalMoney <= Model.maxMoney)
                          && (Model.minProgress == -1 || s.Progress >= Model.minProgress)
                          && (Model.maxProgress == -1 || s.Progress <= Model.maxProgress)
                          && (seC == -1 || Convert.ToInt32(s.Security) == seC)
                          && (Model.key == "" || s.CreateUserId == Model.key || s.SchemeId == Model.key || s.CreaterDisplayName == Model.key)
                        select s;
            cache.TotalCount = query.Count();
            cache.List = query.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
            return cache;
        }
        /// <summary>
        /// 按keyline查询追号列表
        /// </summary>
        /// <param name="keyLine"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string keyLine, string userToken)
        {
            try
            {
                var collection = new BettingOrderInfoCollection();
                string sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryOrderListByChaseKeyLine").SQL;
                collection.OrderList = DB.CreateSQLQuery(sql).List<BettingOrderInfo>();
                if (collection != null && collection.OrderList != null && collection.OrderList.Count > 0)
                {
                    collection.TotalCount = collection.OrderList.Count;
                    collection.TotalOrderMoney = collection.OrderList.Sum(o => o.TotalMoney);
                    collection.TotalBuyMoney = collection.OrderList.Sum(o => o.CurrentBettingMoney);
                    collection.TotalPreTaxBonusMoney = collection.OrderList.Sum(o => o.PreTaxBonusMoney);
                    collection.TotalAfterTaxBonusMoney = collection.OrderList.Sum(o => o.AfterTaxBonusMoney);
                    collection.TotalAddMoney = collection.OrderList.Sum(o => o.AddMoney);
                    collection.TotalUserCount = 1;
                }
                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("查询追号列表失败 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询指定订单的投注号码列表
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId)
        {
            string sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryAnteCodeListBySchemeId").SQL;
            var collection = new BettingAnteCodeInfoCollection();
            collection.AnteCodeList = DB.CreateSQLQuery(sql).List<BettingAnteCodeInfo>();
            return collection;
        }
        /// <summary>
        /// 查询足彩合买明细
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public Sports_TogetherSchemeQueryInfo QuerySportsTogetherDetail(string schemeId)
        {
            var orderDetail = DB.CreateQuery<C_OrderDetail>().Where(x => x.SchemeId == schemeId).FirstOrDefault();
            if (orderDetail == null)
                throw new Exception(string.Format("没有查询到方案{0}的orderDetail信息", schemeId));

            var info = (orderDetail.ProgressStatus == (int)ProgressStatus.Complate
              || orderDetail.ProgressStatus == (int)ProgressStatus.Aborted
              || orderDetail.ProgressStatus == (int)ProgressStatus.AutoStop) ? QueryComplateSportsTogetherDetail(schemeId) : QueryRunningSportsTogetherDetail(schemeId);
            if (info == null)
                throw new Exception(string.Format("没有查询到方案{0}的信息", schemeId));
            return info;
        }
        public Sports_TogetherSchemeQueryInfo QueryComplateSportsTogetherDetail(string schemeId)
        {
            var query = from t in DB.CreateQuery<C_Sports_Together>()
                        join u in DB.CreateQuery<UserRegister>() on t.CreateUserId equals u.UserId
                        join r in DB.CreateQuery<C_Sports_Order_Complate>() on t.SchemeId equals r.SchemeId
                        join b in DB.CreateQuery<C_User_Beedings>() on t.CreateUserId equals b.UserId
                        where t.SchemeId == schemeId && t.GameCode == b.GameCode && t.GameType == b.GameType
                        select new { t, u, r, b };
            var queryResult = query.ToList().Select(z => new Sports_TogetherSchemeQueryInfo
            {
                BonusDeduct = z.t.BonusDeduct,
                CreateUserId = z.t.CreateUserId,
                CreaterDisplayName = z.u.DisplayName,
                CreaterHideDisplayNameCount = z.u.HideDisplayNameCount,
                Description = z.t.Description,
                GameDisplayName = KaSon.FrameWork.Common.Utilities.ConvertHelper.FormatGameCode(z.t.GameCode),
                GameTypeDisplayName = KaSon.FrameWork.Common.Utilities.ConvertHelper.FormatGameType(z.t.GameCode, z.t.GameType),
                Guarantees = z.t.Guarantees,
                PlayType = z.t.PlayType,
                Price = z.t.Price,
                SchemeDeduct = z.t.SchemeDeduct,
                SchemeSource = (SchemeSource)z.t.SchemeSource,
                Security = (TogetherSchemeSecurity)z.t.Security,
                StopTime = z.t.StopTime,
                Subscription = z.t.Subscription,
                Title = z.t.Title,
                TotalCount = z.t.TotalCount,
                TotalMoney = z.t.TotalMoney,
                SchemeId = z.t.SchemeId,
                JoinPwd = z.t.JoinPwd,
                Progress = z.t.Progress,
                ProgressStatus = (TogetherSchemeProgress)z.t.ProgressStatus,
                SystemGuarantees = z.t.SystemGuarantees,
                GameCode = z.t.GameCode,
                GameType = z.t.GameType,
                SoldCount = z.t.SoldCount,
                TotalMatchCount = z.t.TotalMatchCount,
                Amount = z.r.Amount,
                BetCount = z.r.BetCount,
                PreTaxBonusMoney = z.r.PreTaxBonusMoney,
                AfterTaxBonusMoney = z.r.AfterTaxBonusMoney,
                BonusStatus = (BonusStatus)z.r.BonusStatus,
                BonusCount = z.r.BonusCount,
                CreateTime = z.t.CreateTime,
                IsPrizeMoney = z.r.IsPrizeMoney,
                TicketStatus = (TicketStatus)z.r.TicketStatus,
                IssuseNumber = z.r.IssuseNumber,
                AddMoney = z.r.AddMoney,
                AddMoneyDescription = z.r.AddMoneyDescription,
                IsVirtualOrder = z.r.IsVirtualOrder,
                HitMatchCount = z.r.HitMatchCount,
                SchemeBettingCategory = (SchemeBettingCategory)z.r.SchemeBettingCategory,
                JoinUserCount = z.t.JoinUserCount,
                Attach = z.r.Attach,
                MinBonusMoney = z.r.MinBonusMoney,
                MaxBonusMoney = z.r.MaxBonusMoney,
                ExtensionOne = z.r.ExtensionOne,
                GoldCrownCount = z.b.GoldCrownCount,
                GoldCupCount = z.b.GoldCupCount,
                GoldDiamondsCount = z.b.GoldDiamondsCount,
                GoldStarCount = z.b.GoldStarCount,
                SilverCrownCount = z.b.SilverCrownCount,
                SilverCupCount = z.b.SilverCupCount,
                SilverDiamondsCount = z.b.SilverDiamondsCount,
                SilverStarCount = z.b.SilverStarCount,
                IsAppend = z.r.IsAppend == null ? false : z.r.IsAppend,
                TicketTime = z.r.TicketTime,
            });
            var info = queryResult.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = DB.CreateQuery<C_Game_Issuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }
        public Sports_TogetherSchemeQueryInfo QueryRunningSportsTogetherDetail(string schemeId)
        {
            var query = from t in DB.CreateQuery<C_Sports_Together>()
                        join u in DB.CreateQuery<UserRegister>() on t.CreateUserId equals u.UserId
                        join r in DB.CreateQuery<C_Sports_Order_Running>() on t.SchemeId equals r.SchemeId
                        join b in DB.CreateQuery<C_User_Beedings>() on t.CreateUserId equals b.UserId
                        where t.SchemeId == schemeId && t.GameCode == b.GameCode && t.GameType == b.GameType
                        select new { t, u, r, b };
                     var queryResult=query.ToList().Select(z=> new Sports_TogetherSchemeQueryInfo
                        {
                            BonusDeduct = z.t.BonusDeduct,
                            CreateUserId = z.t.CreateUserId,
                            CreaterDisplayName = z.u.DisplayName,
                            CreaterHideDisplayNameCount = z.u.HideDisplayNameCount,
                            Description = z.t.Description,
                            GameDisplayName = KaSon.FrameWork.Common.Utilities.ConvertHelper.FormatGameCode(z.t.GameCode),
                            GameTypeDisplayName = KaSon.FrameWork.Common.Utilities.ConvertHelper.FormatGameType(z.t.GameCode, z.t.GameType),
                            Guarantees = z.t.Guarantees,
                            PlayType = z.t.PlayType,
                            Price = z.t.Price,
                            SchemeDeduct = z.t.SchemeDeduct,
                            SchemeSource = (SchemeSource)z.t.SchemeSource,
                            Security = (TogetherSchemeSecurity)z.t.Security,
                            StopTime = z.t.StopTime,
                            Subscription = z.t.Subscription,
                            Title = z.t.Title,
                            TotalCount = z.t.TotalCount,
                            TotalMoney = z.t.TotalMoney,
                            SchemeId = z.t.SchemeId,
                            JoinPwd = z.t.JoinPwd,
                            Progress = z.t.Progress,
                            ProgressStatus = (TogetherSchemeProgress)z.t.ProgressStatus,
                            SystemGuarantees = z.t.SystemGuarantees,
                            GameCode = z.t.GameCode,
                            GameType = z.t.GameType,
                            SoldCount = z.t.SoldCount,
                            TotalMatchCount = z.t.TotalMatchCount,
                            Amount = z.r.Amount,
                            BetCount = z.r.BetCount,
                            PreTaxBonusMoney = z.r.PreTaxBonusMoney,
                            AfterTaxBonusMoney = z.r.AfterTaxBonusMoney,
                            WinNumber = string.Empty,
                            BonusStatus = (BonusStatus)z.r.BonusStatus,
                            BonusCount = 0,
                            CreateTime = z.t.CreateTime,
                            IsPrizeMoney = false,
                            TicketStatus = (TicketStatus)z.r.TicketStatus,
                            IssuseNumber = z.r.IssuseNumber,
                            AddMoney = 0M,
                            AddMoneyDescription = string.Empty,
                            IsVirtualOrder = z.r.IsVirtualOrder,
                            HitMatchCount = z.r.HitMatchCount,
                            SchemeBettingCategory = (SchemeBettingCategory)z.r.SchemeBettingCategory,
                            JoinUserCount = z.t.JoinUserCount,
                            Attach = z.r.Attach,
                            MinBonusMoney = z.r.MinBonusMoney,
                            MaxBonusMoney = z.r.MaxBonusMoney,
                            ExtensionOne = z.r.ExtensionOne,
                            GoldCrownCount = z.b.GoldCrownCount,
                            GoldCupCount = z.b.GoldCupCount,
                            GoldDiamondsCount = z.b.GoldDiamondsCount,
                            GoldStarCount = z.b.GoldStarCount,
                            SilverCrownCount = z.b.SilverCrownCount,
                            SilverCupCount = z.b.SilverCupCount,
                            SilverDiamondsCount = z.b.SilverDiamondsCount,
                            SilverStarCount = z.b.SilverStarCount,
                            IsAppend = z.r.IsAppend == null ? false : z.r.IsAppend,
                            TicketTime = z.r.TicketTime,

                        });
            var info = queryResult.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = DB.CreateQuery<C_Game_Issuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }
        public Sports_TogetherJoinInfoCollection QuerySportsTogetherJoinList(string schemeId, int pageIndex, int pageSize,string UserToken)
        {
            var result = new Sports_TogetherJoinInfoCollection();
            var totalCount = 0;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = (from j in DB.CreateQuery<C_Sports_TogetherJoin>()
                         join u in DB.CreateQuery<UserRegister>() on j.JoinUserId equals u.UserId
                         where j.SchemeId == schemeId && j.JoinSucess == true
                         orderby j.JoinType ascending
                         select new { j, u });
                 var queryResult=query.ToList().Select(b=> new Sports_TogetherJoinInfo
                         {
                             BuyCount = b.j.BuyCount,
                             RealBuyCount = b.j.RealBuyCount,
                             IsSucess = b.j.JoinSucess,
                             JoinDateTime = b.j.CreateTime,
                             JoinType = (TogetherJoinType)b.j.JoinType,
                             Price = b.j.Price,
                             UserDisplayName = b.u.DisplayName,
                             HideDisplayNameCount = b.u.HideDisplayNameCount,
                             UserId = b.u.UserId,
                             JoinId = b.j.Id,
                             SchemeId = b.j.SchemeId,
                             BonusMoney = b.j.PreTaxBonusMoney,
                         }).ToList();
            totalCount = queryResult.Count();
            if (pageIndex == -1 && pageSize == -1)
            {
                result.List = queryResult.ToList();
                return result;
            }
            var list = queryResult.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            result.TotalCount = totalCount;
            result.List.AddRange(list);
            return result;
        }
        public bool IsUserJoinSportsTogether(string schemeId, string userToken)
        {
            UserAuthentication Auth = new UserAuthentication();
            var userId = Auth.ValidateUserAuthentication(userToken);
            int flag = DB.CreateQuery<C_Sports_TogetherJoin>().Count(p => p.SchemeId == schemeId && p.JoinUserId == userId && p.JoinSucess);
            return flag > 0;
        }
        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId)
        {
            var result = new Sports_AnteCodeQueryInfoCollection();
            var codeList = QuerySportsAnteCodeBySchemeId(schemeId);
            var issuseList = new List<C_Game_Issuse>();
            foreach (var item in codeList)
            {
                if (item.GameCode == "BJDC")
                {
                    #region BJDC

                    var match = QueryBJDC_Match(string.Format("{0}|{1}", item.IssuseNumber, item.MatchId));
                    var matchResult = QueryBJDC_MatchResult(string.Format("{0}|{1}", item.IssuseNumber, item.MatchId));
                    //matchResult.MatchState
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        halfResult = string.Format("{0}:{1}", matchResult.HomeHalf_Result, matchResult.GuestHalf_Result);
                        fullResult = string.Format("{0}:{1}", matchResult.HomeFull_Result, matchResult.GuestFull_Result);
                        matchState = matchResult.MatchState;
                        switch (item.GameType)
                        {
                            case "SPF":
                                caiguo = matchResult.SPF_Result;
                                matchResultSp = matchResult.SPF_SP;
                                break;
                            case "ZJQ":
                                caiguo = matchResult.ZJQ_Result;
                                matchResultSp = matchResult.ZJQ_SP;
                                break;
                            case "SXDS":
                                caiguo = matchResult.SXDS_Result;
                                matchResultSp = matchResult.SXDS_SP;
                                break;
                            case "BF":
                                caiguo = matchResult.BF_Result;
                                matchResultSp = matchResult.BF_SP;
                                break;
                            case "BQC":
                                caiguo = matchResult.BQC_Result;
                                matchResultSp = matchResult.BQC_SP;
                                break;
                        }
                    }
                    result.List.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = match.IssuseNumber,
                        LeagueId = string.Empty,
                        LeagueName = match.MatchName,
                        LeagueColor = match.MatchColor,
                        MatchId = item.MatchId,
                        MatchIdName = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.GuestTeamName,
                        IsDan = item.IsDan,
                        StartTime = match.MatchStartTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = item.Odds,
                        LetBall = match.LetBall,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        MatchState = matchState,
                        WinNumber = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "JCZQ")
                {
                    #region JCZQ

                    var match = QueryJCZQ_Match(item.MatchId);
                    var matchResult = QueryJCZQ_MatchResult(item.MatchId);
                    //matchResult .MatchState
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        halfResult = string.Format("{0}:{1}", matchResult.HalfHomeTeamScore, matchResult.HalfGuestTeamScore);
                        fullResult = string.Format("{0}:{1}", matchResult.FullHomeTeamScore, matchResult.FullGuestTeamScore);
                        matchState = matchResult.MatchState;
                        switch (item.GameType.ToUpper())
                        {
                            case "SPF":
                                caiguo = matchResult.SPF_Result;
                                matchResultSp = matchResult.SPF_SP;
                                break;
                            case "BRQSPF":
                                caiguo = matchResult.BRQSPF_Result;
                                matchResultSp = matchResult.BRQSPF_SP;
                                break;
                            case "ZJQ":
                                caiguo = matchResult.ZJQ_Result;
                                matchResultSp = matchResult.ZJQ_SP;
                                break;
                            case "BF":
                                caiguo = matchResult.BF_Result;
                                matchResultSp = matchResult.BF_SP;
                                break;
                            case "BQC":
                                caiguo = matchResult.BQC_Result;
                                matchResultSp = matchResult.BQC_SP;
                                break;
                        }
                    }
                    result.List.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = string.Empty,
                        LeagueId = match.LeagueId.ToString(),
                        LeagueName = match.LeagueName,
                        LeagueColor = match.LeagueColor,
                        MatchId = match.MatchId,
                        MatchIdName = match.MatchIdName,
                        HomeTeamId = match.HomeTeamId.ToString(),
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = match.GuestTeamId.ToString(),
                        GuestTeamName = match.GuestTeamName,
                        IsDan = item.IsDan,
                        StartTime = match.StartDateTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = item.Odds,
                        LetBall = match.LetBall,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        MatchState = matchState,
                        //XmlHeader = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "JCLQ")
                {
                    #region JCLQ

                    var match = QueryJCLQ_Match(item.MatchId);
                    var matchResult = QueryJCLQ_MatchResult(item.MatchId);
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        //halfResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestHalf_Result);
                        fullResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestScore);
                        matchState = matchResult.MatchState;
                        switch (item.GameType.ToUpper())
                        {
                            case "SF":
                                caiguo = matchResult.SF_Result;
                                matchResultSp = matchResult.SF_SP;
                                break;
                            case "RFSF":
                                caiguo = matchResult.RFSF_Result;
                                matchResultSp = matchResult.RFSF_SP;
                                break;
                            case "SFC":
                                caiguo = matchResult.SFC_Result;
                                matchResultSp = matchResult.SFC_SP;
                                break;
                            case "DXF":
                                caiguo = matchResult.DXF_Result;
                                matchResultSp = matchResult.DXF_SP;
                                break;
                        }
                    }
                    result.List.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = string.Empty,
                        LeagueId = match.LeagueId.ToString(),
                        LeagueName = match.LeagueName,
                        LeagueColor = match.LeagueColor,
                        MatchId = match.MatchId,
                        MatchIdName = match.MatchIdName,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.GuestTeamName,
                        IsDan = item.IsDan,
                        StartTime = match.StartDateTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = item.Odds,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        MatchState = matchState,
                        WinNumber = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "CTZQ")
                {
                    #region CTZQ

                    result.List.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = item.IssuseNumber,
                        LeagueId = string.Empty,
                        LeagueName = string.Empty,
                        LeagueColor = string.Empty,
                        MatchId = string.Empty,
                        MatchIdName = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = string.Empty,
                        GuestTeamId = string.Empty,
                        GuestTeamName = string.Empty,
                        IsDan = item.IsDan,
                        StartTime = DateTime.Now,
                        HalfResult = string.Empty,
                        FullResult = string.Empty,
                        MatchResult = string.Empty,
                        MatchResultSp = 0M,
                        CurrentSp = item.Odds,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        WinNumber = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "JCSJBGJ" || item.GameCode == "JCYJ")
                {
                    var match = GetSJBMatch(item.GameCode, int.Parse(item.AnteCode));
                    result.List.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.Team,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.Team,
                        IsDan = item.IsDan,
                        CurrentSp = item.Odds,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        StartTime = DateTime.Now,
                    });
                    continue;
                }

                var c = issuseList.FirstOrDefault(p => p.GameCode == item.GameCode && p.IssuseNumber == item.IssuseNumber);
                if (c == null)
                {
                    c = QueryGameIssuse(item.GameCode, item.IssuseNumber);
                    issuseList.Add(c);
                }
                result.List.Add(new Sports_AnteCodeQueryInfo
                {
                    AnteCode = item.AnteCode,
                    IssuseNumber = item.IssuseNumber,
                    BonusStatus = (BonusStatus)item.BonusStatus,
                    CurrentSp = item.Odds,
                    IsDan = item.IsDan,
                    GameType = item.GameType,
                    WinNumber = c == null ? string.Empty : string.IsNullOrEmpty(c.WinNumber) ? string.Empty : c.WinNumber,
                    StartTime = DateTime.Now,
                });
            }
            return result;
        }
        public List<C_Sports_AnteCode> QuerySportsAnteCodeBySchemeId(string schemeId)
        {
            var list = (from a in DB.CreateQuery<C_Sports_AnteCode>()
                        where a.SchemeId == schemeId
                        select a
                       ).ToList();
            if (list == null || list.Count <= 0)
                list = (from a in DB.CreateQuery<C_Sports_AnteCode_History>()
                        where a.SchemeId == schemeId
                        select new C_Sports_AnteCode
                        {
                            AnteCode = a.AnteCode,
                            BonusStatus = a.BonusStatus,
                            CreateTime = a.CreateTime,
                            GameCode = a.GameCode,
                            GameType = a.GameType,
                            Id = a.Id,
                            IsDan = a.IsDan,
                            IssuseNumber = a.IssuseNumber,
                            MatchId = a.MatchId,
                            Odds = a.Odds,
                            PlayType = a.PlayType,
                            SchemeId = a.SchemeId,

                        }).ToList();
            return list;
        }
        public C_BJDC_Match QueryBJDC_Match(string id)
        {
            return DB.CreateQuery<C_BJDC_Match>().FirstOrDefault(p => p.Id == id);
        }
        public C_BJDC_MatchResult QueryBJDC_MatchResult(string id)
        {
            return DB.CreateQuery<C_BJDC_MatchResult>().FirstOrDefault(p => p.Id == id);
        }
        public C_JCZQ_Match QueryJCZQ_Match(string matchId)
        {
            return DB.CreateQuery<C_JCZQ_Match>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public C_JCZQ_MatchResult QueryJCZQ_MatchResult(string matchId)
        {
            return DB.CreateQuery<C_JCZQ_MatchResult>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public C_JCLQ_Match QueryJCLQ_Match(string matchId)
        {
            return DB.CreateQuery<C_JCLQ_Match>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public C_JCLQ_MatchResult QueryJCLQ_MatchResult(string matchId)
        {
            return DB.CreateQuery<C_JCLQ_MatchResult>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public C_SJB_Match GetSJBMatch(string gameType, int matchId)
        {
            return DB.CreateQuery<C_SJB_Match>().FirstOrDefault(p => p.GameType == gameType && p.MatchId == matchId);
        }
        public C_Game_Issuse QueryGameIssuse(string gameCode, string issuseNumber)
        {
            return DB.CreateQuery<C_Game_Issuse>().FirstOrDefault(p => p.GameCode == gameCode && p.IssuseNumber == issuseNumber);
        }

        public Issuse_QueryInfo QueryIssuseInfo(string gameCode, string gameType, string issuseNumber)
        {
            var issuse = (from g in DB.CreateQuery<C_Game_Issuse>()
                          where g.GameCode == gameCode
                          && g.IssuseNumber == issuseNumber
                          && (gameType == "" || g.GameType == gameType)
                          select g).FirstOrDefault();
            if (issuse == null) return new Issuse_QueryInfo { Status = IssuseStatus.OnSale };
            return new Issuse_QueryInfo
            {
                CreateTime = issuse.CreateTime,
                GameCode_IssuseNumber = issuse.GameCode_IssuseNumber,
                Game = new GameInfo
                {
                    //DisplayName = issuse.Game.DisplayName,
                    GameCode = issuse.GameCode
                },
                GatewayStopTime = issuse.GatewayStopTime,
                IssuseNumber = issuse.IssuseNumber,
                LocalStopTime = issuse.LocalStopTime,
                OfficialStopTime = issuse.OfficialStopTime,
                StartTime = issuse.StartTime,
                Status = (IssuseStatus)issuse.Status,
                WinNumber = issuse.WinNumber,
            };
        }
        public Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId)
        {
            var orderDetail = DB.CreateQuery<C_OrderDetail>().FirstOrDefault(o => o.SchemeId == schemeId);
            if (orderDetail == null) return null;
            var info = (orderDetail.ProgressStatus == (int)ProgressStatus.Complate
                || orderDetail.ProgressStatus == (int)ProgressStatus.Aborted
                || orderDetail.ProgressStatus == (int)ProgressStatus.AutoStop) ? QuerySports_Order_ComplateInfo(schemeId) : QuerySports_Order_RunningInfo(schemeId);
            if (info == null)
                throw new Exception(string.Format("没有查询到方案{0}的信息", schemeId));
            return info;
        }
        public Sports_SchemeQueryInfo QuerySports_Order_ComplateInfo(string schemeId)
        {
            var query = from r in DB.CreateQuery<C_Sports_Order_Complate>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where r.SchemeId == schemeId
                        select new { r, u };
            var queryResult = query.ToList().Select(b => new Sports_SchemeQueryInfo
            {
                UserId = b.u.UserId,
                UserDisplayName = b.u.DisplayName,
                HideDisplayNameCount = b.u.HideDisplayNameCount,
                GameDisplayName = Common.Utilities.ConvertHelper.FormatGameCode(b.r.GameCode),
                GameCode = b.r.GameCode,
                Amount = b.r.Amount,
                BonusStatus = (BonusStatus)b.r.BonusStatus,
                CreateTime = b.r.CreateTime,
                GameType = b.r.GameType,
                GameTypeDisplayName =KaSon.FrameWork.Common.Utilities. ConvertHelper.FormatGameType(b.r.GameCode, b.r.GameType),
                IssuseNumber = b.r.IssuseNumber,
                PlayType = b.r.PlayType,
                ProgressStatus = (ProgressStatus)b.r.ProgressStatus,
                SchemeId = b.r.SchemeId,
                SchemeType = (SchemeType)b.r.SchemeType,
                TicketId = b.r.TicketId,
                TicketLog = b.r.TicketLog,
                TicketStatus = (TicketStatus)b.r.TicketStatus,
                TotalMatchCount = b.r.TotalMatchCount,
                TotalMoney = b.r.TotalMoney,
                BetCount = b.r.BetCount,
                PreTaxBonusMoney = b.r.PreTaxBonusMoney,
                AfterTaxBonusMoney = b.r.AfterTaxBonusMoney,
                BonusCount = b.r.BonusCount,
                IsPrizeMoney = b.r.IsPrizeMoney,
                Security = (TogetherSchemeSecurity)b.r.Security,
                IsVirtualOrder = b.r.IsVirtualOrder,
                StopTime = b.r.StopTime,
                HitMatchCount = b.r.HitMatchCount,
                AddMoney = b.r.AddMoney,
                AddMoneyDescription = b.r.AddMoneyDescription,
                SchemeBettingCategory = (SchemeBettingCategory)b.r.SchemeBettingCategory,
                TicketProgress = b.r.TicketProgress,
                DistributionWay = (AddMoneyDistributionWay)b.r.DistributionWay,
                Attach = b.r.Attach,
                MaxBonusMoney = b.r.MaxBonusMoney,
                MinBonusMoney = b.r.MinBonusMoney,
                ExtensionOne = b.r.ExtensionOne,
                IsAppend = b.r.IsAppend == null ? false : b.r.IsAppend,
                ComplateDateTime = b.r.ComplateDateTime,
                BetTime = b.r.BetTime,
                SchemeSource = (SchemeSource)b.r.SchemeSource,
                RedBagMoney = b.r.RedBagMoney,
                TicketTime = b.r.TicketTime,
                RedBagAwardsMoney = b.r.AddMoneyDescription == "70" ? b.r.AddMoney : 0,
                BonusAwardsMoney = b.r.AddMoneyDescription == "10" ? b.r.AddMoney : 0,
            });
            var info = queryResult.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = DB.CreateQuery<C_Game_Issuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }
        public Sports_SchemeQueryInfo QuerySports_Order_RunningInfo(string schemeId)
        {
            var query = from r in DB.CreateQuery<C_Sports_Order_Running>()
                        join u in DB.CreateQuery<UserRegister>() on r.UserId equals u.UserId
                        where r.SchemeId == schemeId
                        select new { r, u };
            var queryResult = query.ToList().Select(b => new Sports_SchemeQueryInfo
            {
                UserId = b.u.UserId,
                UserDisplayName = b.u.DisplayName,
                HideDisplayNameCount = b.u.HideDisplayNameCount,
                GameCode = b.r.GameCode,
                Amount = b.r.Amount,
                BonusStatus = (BonusStatus)b.r.BonusStatus,
                CreateTime = b.r.CreateTime,
                GameType = b.r.GameType,
                IssuseNumber = b.r.IssuseNumber,
                PlayType = b.r.PlayType,
                ProgressStatus = (ProgressStatus)b.r.ProgressStatus,
                SchemeId = b.r.SchemeId,
                SchemeType = (SchemeType)b.r.SchemeType,
                TicketId = b.r.TicketId,
                TicketLog = b.r.TicketLog,
                TicketStatus = (TicketStatus)b.r.TicketStatus,
                TotalMatchCount = b.r.TotalMatchCount,
                TotalMoney = b.r.TotalMoney,
                BetCount = b.r.BetCount,
                GameDisplayName = KaSon.FrameWork.Common.Utilities.ConvertHelper.FormatGameCode(b.r.GameCode),
                GameTypeDisplayName = KaSon.FrameWork.Common.Utilities.ConvertHelper.FormatGameType(b.r.GameCode, b.r.GameType),
                AfterTaxBonusMoney = 0M,
                PreTaxBonusMoney = 0M,
                BonusCount = 0,
                WinNumber = string.Empty,
                IsPrizeMoney = false,
                Security = (TogetherSchemeSecurity)b.r.Security,
                IsVirtualOrder = b.r.IsVirtualOrder,
                StopTime = b.r.StopTime,
                HitMatchCount = b.r.HitMatchCount,
                AddMoney = 0M,
                AddMoneyDescription = string.Empty,
                SchemeBettingCategory = (SchemeBettingCategory)b.r.SchemeBettingCategory,
                TicketProgress = b.r.TicketProgress,
                DistributionWay = AddMoneyDistributionWay.Average,
                Attach = b.r.Attach,
                MaxBonusMoney = b.r.MaxBonusMoney,
                MinBonusMoney = b.r.MinBonusMoney,
                ExtensionOne = b.r.ExtensionOne,
                IsAppend = b.r.IsAppend == null ? false : b.r.IsAppend,
                BetTime = b.r.BetTime,
                SchemeSource = (SchemeSource)b.r.SchemeSource,
                TicketTime = b.r.TicketTime,
                RedBagMoney = b.r.RedBagMoney,
            }); 
            var info = queryResult.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = DB.CreateQuery<C_Game_Issuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }
        /// <summary>
        /// 查询我的定制  或 定制我的
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TogetherFollowerRuleQueryInfoCollection QueryUserFollowRule(QueryUserFollowRuleParam Model)
        {
            UserAuthentication Auth = new UserAuthentication();
            Model.userId = Auth.ValidateUserAuthentication(Model.userToken);
            var collection = new TogetherFollowerRuleQueryInfoCollection();
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            var queryResult = new List<TogetherFollowerRuleQueryInfo>();
            if (Model.byFollower)
            {
                var query = (from f in DB.CreateQuery<C_Together_FollowerRule>()
                             join u in DB.CreateQuery<UserRegister>() on f.CreaterUserId equals u.UserId
                             where (Model.gameCode == "" || f.GameCode == Model.gameCode)
                             && (Model.gameType == "" || f.GameType == Model.gameType)
                             && (Model.userId == "" || f.FollowerUserId == Model.userId)
                             select new { f, u });
                queryResult.AddRange(query.ToList().Select(b => new TogetherFollowerRuleQueryInfo
                {
                    RuleId = b.f.Id,
                    BonusMoney = b.f.TotalBonusMoney,
                    BuyMoney = b.f.TotalBetMoney,
                    CancelNoBonusSchemeCount = b.f.CancelNoBonusSchemeCount,
                    CancelWhenSurplusNotMatch = b.f.CancelWhenSurplusNotMatch,
                    CreaterUserId = b.f.CreaterUserId,
                    CreateTime = b.f.CreateTime,
                    FollowerCount = b.f.FollowerCount,
                    FollowerIndex = b.f.FollowerIndex,
                    FollowerPercent = b.f.FollowerPercent,
                    FollowerUserId = b.f.FollowerUserId,
                    GameCode = b.f.GameCode,
                    GameType = b.f.GameType,
                    IsEnable = b.f.IsEnable,
                    MaxSchemeMoney = b.f.MaxSchemeMoney,
                    MinSchemeMoney = b.f.MinSchemeMoney,
                    SchemeCount = b.f.SchemeCount,
                    StopFollowerMinBalance = b.f.StopFollowerMinBalance,
                    UserId = b.u.UserId,
                    UserDisplayName = b.u.DisplayName,
                    HideDisplayNameCount = b.u.HideDisplayNameCount,
                }));
            }
            else
            {
                var query = (from f in DB.CreateQuery<C_Together_FollowerRule>()
                             join u in DB.CreateQuery<UserRegister>() on f.FollowerUserId equals u.UserId
                             where (Model.gameCode == "" || f.GameCode == Model.gameCode)
                             && (Model.gameType == "" || f.GameType == Model.gameType)
                             && (Model.userId == "" || f.CreaterUserId == Model.userId)
                             orderby f.FollowerIndex ascending
                             select new { f, u });
                queryResult.AddRange(query.ToList().Select(b => new TogetherFollowerRuleQueryInfo
                {
                    RuleId = b.f.Id,
                    BonusMoney = b.f.TotalBonusMoney,
                    BuyMoney = b.f.TotalBetMoney,
                    CancelNoBonusSchemeCount = b.f.CancelNoBonusSchemeCount,
                    CancelWhenSurplusNotMatch = b.f.CancelWhenSurplusNotMatch,
                    CreaterUserId = b.f.CreaterUserId,
                    CreateTime = b.f.CreateTime,
                    FollowerCount = b.f.FollowerCount,
                    FollowerIndex = b.f.FollowerIndex,
                    FollowerPercent = b.f.FollowerPercent,
                    FollowerUserId = b.f.FollowerUserId,
                    GameCode = b.f.GameCode,
                    GameType = b.f.GameType,
                    IsEnable = b.f.IsEnable,
                    MaxSchemeMoney = b.f.MaxSchemeMoney,
                    MinSchemeMoney = b.f.MinSchemeMoney,
                    SchemeCount = b.f.SchemeCount,
                    StopFollowerMinBalance = b.f.StopFollowerMinBalance,
                    UserId = b.u.UserId,
                    UserDisplayName = b.u.DisplayName,
                    HideDisplayNameCount = b.u.HideDisplayNameCount,
                }));
            }
            #region
            //var query = Model.byFollower ? (from f in DB.CreateQuery<C_Together_FollowerRule>()
            //                                join u in DB.CreateQuery<UserRegister>() on f.CreaterUserId equals u.UserId
            //                                where (Model.gameCode == string.Empty || f.GameCode == Model.gameCode)
            //                                && (Model.gameType == string.Empty || f.GameType == Model.gameType)
            //                                && (Model.userId == string.Empty || f.FollowerUserId == Model.userId)
            //                                select new TogetherFollowerRuleQueryInfo
            //                                {
            //                                    RuleId = f.Id,
            //                                    BonusMoney = f.TotalBonusMoney,
            //                                    BuyMoney = f.TotalBetMoney,
            //                                    CancelNoBonusSchemeCount = f.CancelNoBonusSchemeCount,
            //                                    CancelWhenSurplusNotMatch = f.CancelWhenSurplusNotMatch,
            //                                    CreaterUserId = f.CreaterUserId,
            //                                    CreateTime = f.CreateTime,
            //                                    FollowerCount = f.FollowerCount,
            //                                    FollowerIndex = f.FollowerIndex,
            //                                    FollowerPercent = f.FollowerPercent,
            //                                    FollowerUserId = f.FollowerUserId,
            //                                    GameCode = f.GameCode,
            //                                    GameType = f.GameType,
            //                                    IsEnable = f.IsEnable,
            //                                    MaxSchemeMoney = f.MaxSchemeMoney,
            //                                    MinSchemeMoney = f.MinSchemeMoney,
            //                                    SchemeCount = f.SchemeCount,
            //                                    StopFollowerMinBalance = f.StopFollowerMinBalance,
            //                                    UserId = u.UserId,
            //                                    UserDisplayName = u.DisplayName,
            //                                    HideDisplayNameCount = u.HideDisplayNameCount,
            //                                }) :
            //                        (from f in DB.CreateQuery<C_Together_FollowerRule>()
            //                         join u in DB.CreateQuery<UserRegister>() on f.FollowerUserId equals u.UserId
            //                         where (Model.gameCode == string.Empty || f.GameCode == Model.gameCode)
            //                         && (Model.gameType == string.Empty || f.GameType == Model.gameType)
            //                         && (Model.userId == string.Empty || f.CreaterUserId == Model.userId)
            //                         orderby f.FollowerIndex ascending
            //                         select new TogetherFollowerRuleQueryInfo
            //                         {
            //                             RuleId = f.Id,
            //                             BonusMoney = f.TotalBonusMoney,
            //                             BuyMoney = f.TotalBetMoney,
            //                             CancelNoBonusSchemeCount = f.CancelNoBonusSchemeCount,
            //                             CancelWhenSurplusNotMatch = f.CancelWhenSurplusNotMatch,
            //                             CreaterUserId = f.CreaterUserId,
            //                             CreateTime = f.CreateTime,
            //                             FollowerCount = f.FollowerCount,
            //                             FollowerIndex = f.FollowerIndex,
            //                             FollowerPercent = f.FollowerPercent,
            //                             FollowerUserId = f.FollowerUserId,
            //                             GameCode = f.GameCode,
            //                             GameType = f.GameType,
            //                             IsEnable = f.IsEnable,
            //                             MaxSchemeMoney = f.MaxSchemeMoney,
            //                             MinSchemeMoney = f.MinSchemeMoney,
            //                             SchemeCount = f.SchemeCount,
            //                             StopFollowerMinBalance = f.StopFollowerMinBalance,
            //                             UserId = u.UserId,
            //                             UserDisplayName = u.DisplayName,
            //                             HideDisplayNameCount = u.HideDisplayNameCount,
            //                         });
            #endregion
            collection.TotalCount = queryResult.Count();
            collection.List = queryResult.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
            return collection;
        }
        /// <summary>
        ///  查询跟单信息
        /// </summary>
        /// <param name="createrUserId"></param>
        /// <param name="followerUserId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public TogetherFollowerRuleQueryInfo QueryTogetherFollowerRuleInfo(string createrUserId, string followerUserId, string gameCode, string gameType)
        {
            var query = from t in DB.CreateQuery<C_Together_FollowerRule>()
                        join u in DB.CreateQuery<UserRegister>() on t.CreaterUserId equals u.UserId
                        where t.CreaterUserId == createrUserId && t.FollowerUserId == followerUserId && t.GameCode == gameCode && t.GameType == gameType
                        select new { t, u };
            var queryResult = query.ToList().Select(b => new TogetherFollowerRuleQueryInfo
            {
                CancelNoBonusSchemeCount = b.t.CancelNoBonusSchemeCount,
                CancelWhenSurplusNotMatch = b.t.CancelWhenSurplusNotMatch,
                CreaterUserId = b.t.CreaterUserId,
                FollowerCount = b.t.FollowerCount,
                FollowerPercent = b.t.FollowerPercent,
                FollowerUserId = b.t.FollowerUserId,
                GameCode = b.t.GameCode,
                GameType = b.t.GameType,
                IsEnable = b.t.IsEnable,
                MaxSchemeMoney = b.t.MaxSchemeMoney,
                MinSchemeMoney = b.t.MinSchemeMoney,
                SchemeCount = b.t.SchemeCount,
                StopFollowerMinBalance = b.t.StopFollowerMinBalance,
                UserDisplayName = b.u.DisplayName,
                HideDisplayNameCount = b.u.HideDisplayNameCount,
                RuleId = b.t.Id,
            });
            if (query != null) return queryResult.FirstOrDefault();
            return new TogetherFollowerRuleQueryInfo();
        }
        public TotalSingleTreasure_Collection QueryTodayBDFXList(QueryTodayBDFXList Model)
        {

            string orderBy = "bdfxcreatetime";
            string desc = "desc";
            if (!string.IsNullOrEmpty(Model.strOrderBy))
            {
                var array = Model.strOrderBy.ToLower().Split('|');
                if (array != null && array.Length > 1)
                {
                    orderBy = array[0].ToString();
                    desc = array[1].ToString();
                }
            }
            Model.startTime = Model.startTime.Date;
            Model.endTime = Model.endTime.AddDays(1).Date;
            var bdfxList = QueryTodayBDFXList_manage(Model, desc, orderBy);
            if (!string.IsNullOrEmpty(Model.currentUserId) && bdfxList != null && bdfxList.TotalCount > 0)
            {
                var userIdList = QueryBeConcernedUserIdList(Model.currentUserId);
                var singleTraList = bdfxList.TotalSingleTreasureList.Where(s => userIdList.ToArray().Contains(s.UserId)).ToList();
                bdfxList.TotalSingleTreasureList = singleTraList.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
                bdfxList.TotalCount = singleTraList.Count;
                return bdfxList;
            }
            bdfxList.TotalSingleTreasureList = bdfxList.TotalSingleTreasureList.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
            return bdfxList;

        }
        public TotalSingleTreasure_Collection QueryTodayBDFXList_manage(QueryTodayBDFXList Model, string desc, string orderBy)
        {
            TotalSingleTreasure_Collection collection = new TotalSingleTreasure_Collection();
            collection.TotalCount = 0;

            //计算上周时间
            var currTime = DateTime.Now;
            int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
            if (currTime.DayOfWeek != 0)
                currTime = currTime.AddDays(-day);
            else
                currTime = currTime.AddDays(-6);
            var sTime = currTime.AddDays(-7).Date;
            var eTime = currTime.Date;
            using (DB)
            {
                string tempTable_sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_TempOrderRunning_Complate_table").SQL;
                DB.CreateSQLQuery(tempTable_sql);
                if (Model.isMyBD == "1")
                {
                    string CountSql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_MyBDCount").SQL;
                    collection = DB.CreateSQLQuery(CountSql)
                        .SetString("@GameCode", Model.gameCode)
                        .SetString("@UserName", Model.userName)
                        .SetString("@UserId", Model.userId).First<TotalSingleTreasure_Collection>();
                    string pageSql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_MyBDPage").SQL;
                    collection.TotalSingleTreasureList = DB.CreateSQLQuery(pageSql)
                        .SetString("@Desc", desc)
                        .SetString("@OrderBy", orderBy)
                        .SetString("@GameCode", Model.gameCode)
                        .SetString("@UserName", Model.userName)
                        .SetString("@UserId", Model.userId)
                        .SetString("@LastweekStartTime", Model.startTime.ToString())
                        .SetString("@LastweekEndTime", Model.endTime.ToString())
                        .List<TotalSingleTreasureInfo>();

                }
                else
                {
                    string CountSql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_NotMyBDCount").SQL;
                    collection = DB.CreateSQLQuery(CountSql)
                        .SetString("@GameCode", Model.gameCode)
                        .SetString("@UserName", Model.userName)
                        .SetString("@UserId", Model.userId).First<TotalSingleTreasure_Collection>();
                    string pageSql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_NotMyBDPage").SQL;
                    collection.TotalSingleTreasureList = DB.CreateSQLQuery(pageSql)
                        .SetString("@Desc", desc)
                        .SetString("@OrderBy", orderBy)
                        .SetString("@GameCode", Model.gameCode)
                        .SetString("@UserName", Model.userName)
                        .SetString("@UserId", Model.userId)
                        .SetString("@LastweekStartTime", Model.startTime.ToString())
                        .SetString("@LastweekEndTime", Model.endTime.ToString())
                        .List<TotalSingleTreasureInfo>();
                }
            }
            if (collection.TotalCount > 0)
            {
                var arrSchemeId = from o in collection.TotalSingleTreasureList select o.SchemeId;
                var anteCodeList = QueryAnteCodeList(arrSchemeId.ToArray());
                collection.AnteCodeList.AddRange(anteCodeList);
            }
            return collection;
        }
        public List<AnteCodeInfo> QueryAnteCodeList(string[] arrSchemeId)
        {
            var query = from a in DB.CreateQuery<C_Sports_AnteCode>()
                        where arrSchemeId.Contains(a.SchemeId)
                        select new AnteCodeInfo
                        {
                            AnteCode = a.AnteCode,
                            GameType = a.GameType,
                            GameCode = a.GameCode,
                            IsDan = a.IsDan,
                            IssuseNumber = a.IssuseNumber,
                            MatchId = a.MatchId,
                            PlayType = a.PlayType,
                            SchemeId = a.SchemeId,
                        };
            if (query != null && query.Count() > 0)
                return query.ToList();
            return new List<AnteCodeInfo>();
        }
        public List<string> QueryBeConcernedUserIdList(string concernedUserId)
        {
            return (from o in DB.CreateQuery<C_SingleTreasure_Attention>().Where(s => s.ConcernedUserId == concernedUserId) select o.BeConcernedUserId).ToList();
        }
        /// <summary>
        /// 查询昨日牛人
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string QueryYesterdayNR(DateTime startTime, DateTime endTime, int count)
        {
            startTime = startTime.Date.AddDays(-1);
            endTime = endTime.Date;
            string strSql = "select top " + count + " t.UserId,t.DisplayName from(select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,u.UserId,u.DisplayName from C_TotalSingleTreasure t inner join C_User_Register u on t.UserId=u.UserId where  t.CreateTime>=:StartTime and t.CreateTime<:EndTime and t.IsBonus=1 group by u.UserId,u.DisplayName	)t where  t.CurrProfitRate>=0 order by t.CurrProfitRate desc";
            var query = DB.CreateSQLQuery(strSql)
                .SetString("StartTime", startTime.ToString())
                .SetString("EndTime", endTime.ToString())
                .List<QueryYesterdayNRModel>();

            string str = string.Empty;
            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    str += item.UserId + "|" + item.DisplayName + "%";
                }
            }
            if (!string.IsNullOrEmpty(str))
                str = str.TrimEnd('%');
            return str;
        }
        /// <summary>
        /// 查询宝单作者主页
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strIsBonus"></param>
        /// <param name="currentTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public TotalSingleTreasure_Collection QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize)
        {
            using (DB)
            {
                TotalSingleTreasure_Collection collection = new TotalSingleTreasure_Collection
                {
                    TotalCount = 0
                };
                int _bonusStatus = 0;
                if (!string.IsNullOrEmpty(strIsBonus) && strIsBonus == "1")
                {
                    _bonusStatus = 20;
                }
                else if (!string.IsNullOrEmpty(strIsBonus) && strIsBonus == "0")
                {
                    _bonusStatus = 30;
                }
                string tablesql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_TempTableComplate").SQL;
                DB.CreateSQLQuery(tablesql);
                string sqlCount = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryBDFXAutherHomePageCount").SQL;
                collection = DB.CreateSQLQuery(sqlCount)
                                            .SetString("UserId", userId)
                                            .SetString("StrIsBonus", strIsBonus)
                                            .SetInt("_bonusStatus", _bonusStatus)
                                            .SetString("CurrentTime", currentTime).First<TotalSingleTreasure_Collection>();

                string PageSql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryBDFXAutherHomePageCount").SQL;
                collection.TotalSingleTreasureList = DB.CreateSQLQuery(PageSql)
                                            .SetString("UserId", userId)
                                            .SetString("StrIsBonus", strIsBonus)
                                            .SetInt("_bonusStatus", _bonusStatus)
                                            .SetString("CurrentTime", currentTime)
                                            .SetInt("PageIndex", pageIndex)
                                            .SetInt("PageSize", pageSize)
                                            .List<TotalSingleTreasureInfo>();
                if (collection.TotalCount > 0)
                {
                    var arrSchemeId = from o in collection.TotalSingleTreasureList select o.SchemeId;
                    var anteCodeList = this.QueryAnteCodeList(arrSchemeId.ToArray());
                    collection.AnteCodeList.AddRange(anteCodeList);
                }
                return collection;
            }
        }
        /// <summary>
        /// 查询关注(关注总数、被关注总数、晒单总数等)
        /// </summary>
        public ConcernedInfo QueryConcernedByUserId(string bdfxUserId, string currUserId, string startTime, string endTime)
        {

            var sTime = new DateTime();
            var eTime = new DateTime();
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                startTime = startTime.Replace('.', '-');
                endTime = endTime.Replace('.', '-');
                sTime = Convert.ToDateTime(DateTime.Now.Year + "-" + startTime).Date;
                eTime = Convert.ToDateTime(DateTime.Now.Year + "-" + endTime).AddDays(1).Date;
            }
            else if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                var currTime = DateTime.Now;
                int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
                if (currTime.DayOfWeek != 0)
                    currTime = currTime.AddDays(-day);
                else
                    currTime = currTime.AddDays(-6);
                sTime = currTime.AddDays(-7).Date;
                eTime = currTime.Date;
            }
            return QueryConcernedByUserId_low(bdfxUserId, currUserId, sTime, eTime);
        }
        public ConcernedInfo QueryConcernedByUserId_low(string bdfxUserId, string currUserId, DateTime startTime, DateTime endTime)
        {
            ConcernedInfo info = new ConcernedInfo();
            //查询关注信息
            string strSql = "select u.UserId,u.DisplayName,isnull(att.BeConcernedUserCount,0)BeConcernedUserCount,isnull(att.ConcernedUserCount,0)ConcernedUserCount,isnull(att.SingleTreasureCount,0)SingleTreasureCount from C_User_Register u left join C_SingleTreasure_AttentionSummary att on u.UserId=att.UserId  where u.UserId=@BDFXUserId";
            info = DB.CreateSQLQuery(strSql)
                            .SetString("@BDFXUserId", bdfxUserId).First<ConcernedInfo>();
            //查询是否已关注
            strSql = "select count(1) GZ from C_SingleTreasure_Attention at where at.BeConcernedUserId=@BDFXUserId and at.ConcernedUserId=@CurrUserId";
            info.IsGZ = DB.CreateSQLQuery(strSql)
                         .SetString("@BDFXUserId", bdfxUserId)
                         .SetString("@CurrUserId", currUserId)
                         .First<bool>();
            #region 暂时屏蔽
            ////查询近段时间盈利率
            //var endTime = DateTime.Now.Date;
            //var startTime = endTime.AddDays(-7);
            //strSql = "select tab.rowNumber,isnull(tab.currDay,'')CurrDay,isnull(tab.ProfitRate,0)ProfitRate from(select ROW_NUMBER() over(order by CONVERT(varchar(10),CreateTime,120)) rowNumber, CONVERT(varchar(10),CreateTime,120) currDay,t.ProfitRate from C_TotalSingleTreasure t where CONVERT(varchar(10),CreateTime,120)>=:StartTime and CONVERT(varchar(10),CreateTime,120) <:EndTime and UserId=:BDFXUserId group by CONVERT(varchar(10),CreateTime,120),t.ProfitRate ) tab";
            //query = Session.CreateSQLQuery(strSql)
            //             .SetDateTime("StartTime", startTime)
            //             .SetDateTime("EndTime", endTime)
            //             .SetString("BDFXUserId", bdfxUserId)
            //             .List();
            //if (query != null && query.Count > 0)
            //{
            //    foreach (var item in query)
            //    {
            //        var array = item as object[];
            //        NearTimeProfitRateInfo nInfo = new NearTimeProfitRateInfo();
            //        nInfo.RowNumber = Convert.ToInt32(array[0]);
            //        nInfo.CurrDate = Convert.ToString(array[1]);
            //        nInfo.ProfitRate = Convert.ToDecimal(array[2]);
            //        info.NearTimeProfitRateCollection.NearTimeProfitRateList.Add(nInfo);
            //    }
            //}

            ////查询上周排行,根据当前时间，计算出上个星期的时间段
            //var currTime = DateTime.Now;
            //int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
            //if (currTime.DayOfWeek != 0)
            //    currTime = currTime.AddDays(-day);
            //else
            //    currTime = currTime.AddDays(-6);
            //startTime = currTime.AddDays(-7).Date;
            //endTime = currTime.Date;
            //strSql = "select tt.rownumber from (select ROW_NUMBER() over(order by sum(ProfitRate) desc) rownumber,UserId from C_TotalSingleTreasure t where CreateTime>=:StartTime and CreateTime<:EndTime group by UserId)tt where UserId=:BDFXUserId";
            //query = Session.CreateSQLQuery(strSql)
            //             .SetDateTime("StartTime", startTime)
            //             .SetDateTime("EndTime", endTime)
            //             .SetString("BDFXUserId", bdfxUserId)
            //             .List();
            //if (query != null && query.Count > 0)
            //    info.RankNumber = Convert.ToInt32(query[0]); 
            #endregion
            //查询近段时间盈利率
            var nInfo = QueryNearTimeProfitRate(bdfxUserId);
            info.NearTimeProfitRateCollection = new NearTimeProfitRate_Collection();
            info.NearTimeProfitRateCollection.NearTimeProfitRateList.AddRange(nInfo);
            //查询上周排行,根据当前时间，计算出上个星期的时间段
            info.RankNumber = QueryRankNumber(bdfxUserId);
            return info;
        }
        public List<NearTimeProfitRateInfo> QueryNearTimeProfitRate(string bdfxUserId)
        {
            //查询近段时间盈利率
            List<NearTimeProfitRateInfo> ListInfo = new List<NearTimeProfitRateInfo>();
            var endTime = DateTime.Now.Date.AddDays(1);
            var startTime = endTime.AddDays(-7);
            //20151008
            //var strSql = "select tab.rowNumber,isnull(tab.currDay,'')CurrDay,isnull(tab.CurrProfitRate,0)CurrProfitRate from(select ROW_NUMBER() over(order by CONVERT(varchar(10),CreateTime,120)) rowNumber, CONVERT(varchar(10),CreateTime,120) currDay,(case t.IsBonus when 0 then 0 when 1 then((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) else 0 end) CurrProfitRate from C_TotalSingleTreasure t where CONVERT(varchar(10),CreateTime,120)>=:StartTime and CONVERT(varchar(10),CreateTime,120) <:EndTime and UserId=:BDFXUserId group by CONVERT(varchar(10),CreateTime,120) ,t.UserId,t.IsBonus) tab";
            var strSql = "select tab.rowNumber,isnull(tab.currDay,'')CurrDay,isnull(tab.CurrProfitRate,0)CurrProfitRate from ( select ROW_NUMBER() over(order by CONVERT(varchar(10),CreateTime,120)) rowNumber, CONVERT(varchar(10),CreateTime,120) currDay, (SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney) CurrProfitRate  from C_TotalSingleTreasure t where CONVERT(varchar(10),CreateTime,120)>=@StartTime and CONVERT(varchar(10),CreateTime,120) <@EndTime and UserId=@BDFXUserId and IsBonus=1 group by CONVERT(varchar(10),CreateTime,120) ,t.UserId ) tab";
            var query = DB.CreateSQLQuery(strSql)
                           .SetString("StartTime", startTime.ToString())
                           .SetString("EndTime", endTime.ToString())
                           .SetString("BDFXUserId", bdfxUserId)
                           .List<NearTimeProfitRateInfo>();
            if (ListInfo == null || !ListInfo.Any())
            {
                for (int i = 1; i <= 7; i++)
                {
                    NearTimeProfitRateInfo nInfo = new NearTimeProfitRateInfo();
                    nInfo.RowNumber = i;
                    nInfo.CurrDate = string.Empty;
                    nInfo.CurrProfitRate = 0;
                    ListInfo.Add(nInfo);
                }
            }
            return ListInfo;
        }
        public int QueryRankNumber(string bdfxUserId)
        {
            //查询上周排行,根据当前时间，计算出上个星期的时间段

            //计算上周时间
            var currTime = DateTime.Now;
            int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
            if (currTime.DayOfWeek != 0)
                currTime = currTime.AddDays(-day);
            else
                currTime = currTime.AddDays(-6);
            var sTime = currTime.AddDays(-7).Date;
            var eTime = currTime.Date;

            var strSql = "select tt.LastweekRank from (select ROW_NUMBER() over(order by sum(CurrProfitRate) desc) LastweekRank,lastTab.UserId from (select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,UserId from C_TotalSingleTreasure t where CreateTime>=@StartTime and CreateTime<@EndTime and t.IsBonus=1 group by UserId	)	lastTab group by UserId		)tt where tt.UserId=@BDFXUserId";
            var query = DB.CreateSQLQuery(strSql)
                          .SetString("StartTime", sTime.ToString())
                          .SetString("EndTime", eTime.ToString())
                          .SetString("BDFXUserId", bdfxUserId)
                          .First<int>();
            return query;
        }
        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="bgzUserId"></param>
        public CommonActionResult BDFXAttention(string currUserId, string bgzUserId)
        {
            CommonActionResult result = new CommonActionResult();
            try
            {               
                DB.Begin();
                #region 关注
                if (string.IsNullOrEmpty(currUserId))
                    throw new Exception("关注人编号不能为空");
                else if (string.IsNullOrEmpty(bgzUserId))
                    throw new Exception("被关注人编号不能为空");
                var singleTreasureAttention = QuerySingleTreasureAttentionByUserId(bgzUserId, currUserId);
                if (singleTreasureAttention != null && !string.IsNullOrEmpty(singleTreasureAttention.ConcernedUserId))
                    throw new Exception("您已经关注了他");
                if (currUserId == bgzUserId)
                    throw new Exception("不能关注自己");
                singleTreasureAttention = new C_SingleTreasure_Attention();
                singleTreasureAttention.BeConcernedUserId = bgzUserId;
                singleTreasureAttention.ConcernedUserId = currUserId;
                singleTreasureAttention.CreateTime = DateTime.Now;
                AddSingleTreasureAttention(singleTreasureAttention);
                //修改被关注者信息
                var BGZSummary = QuerySingleTreasureAttentionSummaryByUserId(bgzUserId);
                if (BGZSummary != null && !string.IsNullOrEmpty(BGZSummary.UserId))
                {
                    BGZSummary.BeConcernedUserCount += 1;
                    BGZSummary.UpdateTime = DateTime.Now;
                    UpdateSingleTreasureAttentionSummary(BGZSummary);
                }
                else
                {
                    BGZSummary = new C_SingleTreasure_AttentionSummary();
                    BGZSummary.BeConcernedUserCount = 1;
                    BGZSummary.ConcernedUserCount = 0;
                    BGZSummary.SingleTreasureCount = 0;
                    BGZSummary.UpdateTime = DateTime.Now;
                    BGZSummary.UserId = bgzUserId;
                    AddSingleTreasureAttentionSummary(BGZSummary);
                }
                //修改关注者信息
                var GZSummary = QuerySingleTreasureAttentionSummaryByUserId(currUserId);
                if (GZSummary != null && !string.IsNullOrEmpty(GZSummary.UserId))
                {
                    GZSummary.ConcernedUserCount += 1;
                    GZSummary.UpdateTime = DateTime.Now;
                    UpdateSingleTreasureAttentionSummary(GZSummary);
                }
                else
                {
                    GZSummary = new C_SingleTreasure_AttentionSummary();
                    GZSummary.BeConcernedUserCount = 0;
                    GZSummary.ConcernedUserCount = 1;
                    GZSummary.SingleTreasureCount = 0;
                    GZSummary.UpdateTime = DateTime.Now;
                    GZSummary.UserId = currUserId;
                    AddSingleTreasureAttentionSummary(GZSummary);
                }
                #endregion
                DB.Commit();
                result.IsSuccess = true;
                result.Message = "关注成功";
                result.ReturnValue = "1";
                return result;
            }
            catch (Exception)
            {
                DB.Rollback();
                result.IsSuccess = false;
                result.Message = "关注失败";
                result.ReturnValue = "0";
                return result;
            }
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="bgzUserId"></param>
        public CommonActionResult BDFXCancelAttention(string currUserId, string bgzUserId)
        {
            CommonActionResult result = new CommonActionResult();
            try
            {
                #region 取消关注
                DB.Begin();
                var singleTreasureAttention = QuerySingleTreasureAttentionByUserId(bgzUserId, currUserId);
                if (singleTreasureAttention == null || string.IsNullOrEmpty(singleTreasureAttention.ConcernedUserId))
                    throw new Exception("您还未关注他");
                DeleteSingleTreasureAttention(singleTreasureAttention);
                //修改被关注者信息
                var BGZSummary = QuerySingleTreasureAttentionSummaryByUserId(bgzUserId);
                if (BGZSummary != null && !string.IsNullOrEmpty(BGZSummary.UserId))
                {
                    BGZSummary.BeConcernedUserCount -= 1;
                    BGZSummary.UpdateTime = DateTime.Now;
                    UpdateSingleTreasureAttentionSummary(BGZSummary);
                }

                //修改关注者信息
                var GZSummary = QuerySingleTreasureAttentionSummaryByUserId(currUserId);
                if (GZSummary != null && !string.IsNullOrEmpty(GZSummary.UserId))
                {
                    GZSummary.ConcernedUserCount -= 1;
                    GZSummary.UpdateTime = DateTime.Now;
                    UpdateSingleTreasureAttentionSummary(GZSummary);
                }
                DB.Commit();
                #endregion
                result.IsSuccess = true;
                result.Message = "取消关注成功";
                result.ReturnValue = "1";
                return result;
            }
            catch (Exception)
            {
                DB.Rollback();
                result.IsSuccess = false;
                result.Message = "取消关注失败";
                result.ReturnValue = "0";
                return result;
            }                        
        }
        
        public C_SingleTreasure_Attention QuerySingleTreasureAttentionByUserId(string beConcernedUserId, string concernedUserId)
        {           
            return DB.CreateQuery<C_SingleTreasure_Attention>().FirstOrDefault(s => s.BeConcernedUserId == beConcernedUserId && s.ConcernedUserId == concernedUserId);
        }
        public void AddSingleTreasureAttention(C_SingleTreasure_Attention entity)
        {
            DB.GetDal<C_SingleTreasure_Attention>().Add(entity);
        }
        public C_SingleTreasure_AttentionSummary QuerySingleTreasureAttentionSummaryByUserId(string userId)
        {
            return DB.CreateQuery<C_SingleTreasure_AttentionSummary>().FirstOrDefault(s => s.UserId == userId);
        }
        public void UpdateSingleTreasureAttentionSummary(C_SingleTreasure_AttentionSummary entity)
        {
            DB.GetDal<C_SingleTreasure_AttentionSummary>().Update(entity);
        }
        public void AddSingleTreasureAttentionSummary(C_SingleTreasure_AttentionSummary entity)
        {
            DB.GetDal<C_SingleTreasure_AttentionSummary>().Add(entity);
        }
        public void DeleteSingleTreasureAttention(C_SingleTreasure_Attention entity)
        {
            DB.GetDal<C_SingleTreasure_Attention>().Delete(entity);
        }
        /// <summary>
        /// 查询高手排行/我的关注
        /// </summary>
        public BDFXGSRank_Collection QueryGSRankList(string startTime, string endTime, string currUserId, string isMyGZ)
        {
            startTime = startTime.Replace('.', '-');
            endTime = endTime.Replace('.', '-');
            var sTime = Convert.ToDateTime(DateTime.Now.Year + "-" + startTime).Date;
            var eTime = Convert.ToDateTime(DateTime.Now.Year + "-" + endTime).AddDays(1).Date;
            if (!string.IsNullOrEmpty(isMyGZ))
                isMyGZ = isMyGZ.ToLower();
            return QueryGSRankList(sTime, eTime, currUserId, isMyGZ);
        }
        public BDFXGSRank_Collection QueryGSRankList(DateTime startTime, DateTime endTime, string currUserId, string isMyGZ)
        {            
            var sTime = startTime.AddDays(-7).Date;
            var eTime = startTime.Date;
            BDFXGSRank_Collection collection = new BDFXGSRank_Collection();
            string sql = string.Empty;
            if (!string.IsNullOrEmpty(isMyGZ) && isMyGZ == "true")
            {
                sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryMyFollow").SQL;
                collection.RankList= DB.CreateSQLQuery(sql)
                    .SetString("@StartTime", startTime.ToString())
                    .SetString("@EndTime", endTime.ToString())
                    .SetString("@CurrUserId", currUserId)
                    .SetString("@LastweekStartTime", sTime.ToString())
                    .SetString("@LastweekEndTime", eTime.ToString())
                    .List<BDFXGSRankInfo>();
            }
            else
            {
                sql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryGSRankList").SQL;
                collection.RankList = DB.CreateSQLQuery(sql)
                    .SetString("@StartTime", startTime.ToString())
                    .SetString("@EndTime", endTime.ToString())
                    .SetString("@CurrUserId", currUserId)
                    .SetString("IsMyGZ", isMyGZ)
                    .SetString("@LastweekStartTime", sTime.ToString())
                    .SetString("@LastweekEndTime", eTime.ToString())
                    .List<BDFXGSRankInfo>();
            }           
            return collection;
        }
        /// <summary>
        /// 查询宝单详情
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public BDFXOrderDetailInfo QueryBDFXOrderDetailBySchemeId(string schemeId)
        {
            var orderDetailInfo = QueryBDFXOrderDetailBySchemeId(schemeId);
            if (orderDetailInfo != null && !string.IsNullOrEmpty(orderDetailInfo.SchemeId))
            {
                orderDetailInfo.AnteCodeCollection = new Sports_AnteCodeQueryInfoCollection();
                orderDetailInfo.AnteCodeList = new List<AnteCodeInfo>();
                orderDetailInfo.NearTimeProfitRateCollection = new NearTimeProfitRate_Collection();
                var anteCodeCollection = QuerySportsOrderAnteCodeList(schemeId);
                if (anteCodeCollection != null)
                    orderDetailInfo.AnteCodeCollection = anteCodeCollection;
                var nearTimeProfitInfo = QueryNearTimeProfitRate(orderDetailInfo.UserId);
                if (nearTimeProfitInfo != null)
                    orderDetailInfo.NearTimeProfitRateCollection.NearTimeProfitRateList.AddRange(nearTimeProfitInfo);

                var currTime = DateTime.Now;
                int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
                if (currTime.DayOfWeek != 0)
                    currTime = currTime.AddDays(-day);
                else
                    currTime = currTime.AddDays(-6);
                var startTime = currTime.AddDays(-7).Date;
                var endTime = currTime.Date;
                var rankNumber =QueryRankNumber(orderDetailInfo.UserId);
                orderDetailInfo.RankNumber = rankNumber;
                var anteCodeList = QueryAnteCodeListBySchemeId_manage(schemeId);
                if (anteCodeList != null && anteCodeList.Count > 0)
                    orderDetailInfo.AnteCodeList = anteCodeList.ToList();
            }

            return orderDetailInfo;
        }
        public List<AnteCodeInfo> QueryAnteCodeListBySchemeId_manage(string schemeId)
        {
            var query = from a in DB.CreateQuery<C_Sports_AnteCode>()
                        where a.SchemeId == schemeId
                        select new { a };
            var queryResult = query.ToList().Select(b => new AnteCodeInfo
            {
                AnteCode = b.a.AnteCode,
                GameType = b.a.GameType,
                GameCode = b.a.GameCode,
                IsDan = b.a.IsDan,
                IssuseNumber = b.a.IssuseNumber,
                MatchId = b.a.MatchId,
                PlayType = b.a.PlayType,
                SchemeId = b.a.SchemeId,
            });
            if (query != null && query.Count() > 0)
                return queryResult.ToList();
            return new List<AnteCodeInfo>();
        }

        public BettingOrderInfoCollection QueryMyChaseOrderList(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var collection = new BettingOrderInfoCollection();
            string Countsql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryMyChaseOrderListCount").SQL;
            collection.TotalCount = DB.CreateSQLQuery(Countsql)
                .SetString("@GameCode", gameCode)
                .SetString("@UserId", userId)
                .SetString("@FromDate", startTime.ToString("yyyy-MM-dd"))
                .SetString("@ToDate", endTime.AddDays(1).ToString("yyyy-MM-dd")).First<int>();

            string PageSql = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "Debug_QueryMyChaseOrderListPage").SQL;
            collection.OrderList = DB.CreateSQLQuery(PageSql)
                 .SetString("@GameCode", gameCode)
                .SetString("@UserId", userId)
                .SetString("@FromDate", startTime.ToString("yyyy-MM-dd"))
                .SetString("@ToDate", endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .SetInt("@PageIndex", pageIndex)
                .SetInt("@PageSize", pageSize).List<BettingOrderInfo>();
           
            return collection;
        }
        public MyOrderListInfoCollection QueryMyOrderListInfo(QueryMyOrderListInfoParam Model)
        {
            var UserId = new UserAuthentication().ValidateUserAuthentication(Model.userToken);
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            var collection = new MyOrderListInfoCollection();
            var query = from d in DB.CreateQuery<C_OrderDetail>()
                        where d.UserId == UserId
                        && (Model.gameCode == string.Empty || d.GameCode == Model.gameCode)
                        && (Model.bonusStatus == null || d.BonusStatus == (int)Model.bonusStatus)
                        && (Model.schemeType == null || d.SchemeType == (int)Model.schemeType)
                        && (d.CreateTime >= Model.startTime && d.CreateTime < Model.endTime)
                        select new { d };
            var queryResult = query.ToList().Select(b => new MyOrderListInfo
            {
                AddMoney = b.d.AddMoney,
                Amount = b.d.Amount,
                AfterTaxBonusMoney = b.d.AfterTaxBonusMoney,
                BetTime = b.d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                BonusAwardsMoney = b.d.BonusAwardsMoney,
                BonusStatus = (BonusStatus)b.d.BonusStatus,
                GameCode = b.d.GameCode,
                GameType = b.d.GameType,
                GameTypeName = b.d.GameTypeName,
                IssuseNumber = b.d.CurrentIssuseNumber,
                PreTaxBonusMoney = b.d.PreTaxBonusMoney,
                ProgressStatus = (ProgressStatus)b.d.ProgressStatus,
                RedBagAwardsMoney = b.d.RedBagAwardsMoney,
                SchemeBettingCategory = (SchemeBettingCategory)b.d.SchemeBettingCategory,
                SchemeId = b.d.SchemeId,
                SchemeSource = (SchemeSource)b.d.SchemeSource,
                SchemeType = (SchemeType)b.d.SchemeType,
                TicketStatus = (TicketStatus)b.d.TicketStatus,
                TotalMoney = b.d.TotalMoney,
            });
            collection.List = queryResult.Skip(Model.pageIndex * Model.pageSize).Take(Model.pageSize).ToList();
            return collection;
        }

        public MyOrderListInfo QueryMyOrderDetailInfo(string schemeId)
        {
            var query = from d in DB.CreateQuery<C_OrderDetail>()
                        where d.SchemeId == schemeId
                        select new { d };
            var queryResult = query.ToList().Select(b => new MyOrderListInfo
            {
                AddMoney = b.d.AddMoney,
                Amount = b.d.Amount,
                AfterTaxBonusMoney = b.d.AfterTaxBonusMoney,
                BetTime = b.d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                BonusAwardsMoney = b.d.BonusAwardsMoney,
                BonusStatus = (BonusStatus)b.d.BonusStatus,
                GameCode = b.d.GameCode,
                GameType = b.d.GameType,
                GameTypeName = b.d.GameTypeName,
                IssuseNumber = b.d.CurrentIssuseNumber,
                PreTaxBonusMoney = b.d.PreTaxBonusMoney,
                ProgressStatus = (ProgressStatus)b.d.ProgressStatus,
                RedBagAwardsMoney = b.d.RedBagAwardsMoney,
                SchemeBettingCategory = (SchemeBettingCategory)b.d.SchemeBettingCategory,
                SchemeId = b.d.SchemeId,
                SchemeSource = (SchemeSource)b.d.SchemeSource,
                SchemeType = (SchemeType)b.d.SchemeType,
                TicketStatus = (TicketStatus)b.d.TicketStatus,
                TotalMoney = b.d.TotalMoney,
                StopAfterBonus = b.d.StopAfterBonus,
            });
            return queryResult.FirstOrDefault();
        }
        public List<LotteryNewBonusInfo> QueryLotteryNewBonusInfoList(int count)
        {
            var query = from b in DB.CreateQuery<E_LotteryNewBonus>()
                        orderby b.CreateTime descending
                        select new { b };
               var queryResult=query.ToList().Select(z=> new LotteryNewBonusInfo
                        {
                            AfterTaxBonusMoney = z.b.AfterTaxBonusMoney,
                            Amount = z.b.Amount,
                            CreateTime = z.b.CreateTime,
                            GameCode = z.b.GameCode,
                            GameType = z.b.GameType,
                            HideUserDisplayNameCount = z.b.HideUserDisplayNameCount,
                            IssuseNumber = z.b.IssuseNumber,
                            PlayType = z.b.PlayType,
                            PreTaxBonusMoney = z.b.PreTaxBonusMoney,
                            SchemeId = z.b.SchemeId,
                            TotalMoney = z.b.TotalMoney,
                            UserDisplayName = z.b.UserDisplayName,
                        });
            return queryResult.Take(count).ToList();
        }

        /// <summary>
        /// 查询开奖号码
        /// </summary>
        /// <param name="gameType">仅传统足球需要传玩法</param>
        /// <returns></returns>
        public GameWinNumber_InfoCollection QueryGameWinNumber(string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "CQ11X5":
                        return new LotteryDataBusiness_CQ11X5().QueryCQ11X5_GameWinNumber(pageIndex, pageSize);
                    case "CQKLSF":
                        return new LotteryDataBusiness_CQKLSF().QueryCQKLSF_GameWinNumber(pageIndex, pageSize);
                    case "CQSSC":
                        return new LotteryDataBusiness_CQSSC().QueryCQSSC_GameWinNumber(pageIndex, pageSize);
                    case "DF6J1":
                        return new LotteryDataBusiness_DF6_1().QueryDF6_1_GameWinNumber(pageIndex, pageSize);
                    case "DLT":
                        return new LotteryDataBusiness_DLT().QueryDLT_GameWinNumber(pageIndex, pageSize);
                    case "FC3D":
                        return new LotteryDataBusiness_FC3D().QueryFC3D_GameWinNumber(pageIndex, pageSize);
                    case "GD11X5":
                        return new LotteryDataBusiness_GD11X5().QueryGD11X5_GameWinNumber(pageIndex, pageSize);
                    case "GDKLSF":
                        return new LotteryDataBusiness_GDKLSF().QueryGDKLSF_GameWinNumber(pageIndex, pageSize);
                    case "HBK3":
                        return new LotteryDataBusiness_HBK3().QueryHBK3_GameWinNumber(pageIndex, pageSize);
                    case "HC1":
                        return new LotteryDataBusiness_HC1().QueryHC1_GameWinNumber(pageIndex, pageSize);
                    case "HD15X5":
                        return new LotteryDataBusiness_HD15X5().QueryHD15X5_GameWinNumber(pageIndex, pageSize);
                    case "HNKLSF":
                        return new LotteryDataBusiness_HNKLSF().QueryHNKLSF_GameWinNumber(pageIndex, pageSize);
                    case "JLK3":
                        return new LotteryDataBusiness_JLK3().QueryJLK3_GameWinNumber(pageIndex, pageSize);
                    case "JSKS":
                        return new LotteryDataBusiness_JSK3().QueryJSK3_GameWinNumber(pageIndex, pageSize);
                    case "JX11X5":
                        return new LotteryDataBusiness_JX11X5().QueryJX11X5_GameWinNumber(pageIndex, pageSize);
                    case "JXSSC":
                        return new LotteryDataBusiness_JXSSC().QueryJXSSC_GameWinNumber(pageIndex, pageSize);
                    case "LN11X5":
                        return new LotteryDataBusiness_LN11X5().QueryLN11X5_GameWinNumber(pageIndex, pageSize);
                    case "PL3":
                        return new LotteryDataBusiness_PL3().QueryPL3_GameWinNumber(pageIndex, pageSize);
                    case "PL5":
                        return new LotteryDataBusiness_PL5().QueryPL5_GameWinNumber(pageIndex, pageSize);
                    case "QLC":
                        return new LotteryDataBusiness_QLC().QueryQLC_GameWinNumber(pageIndex, pageSize);
                    case "QXC":
                        return new LotteryDataBusiness_QXC().QueryQXC_GameWinNumber(pageIndex, pageSize);
                    case "SDQYH":
                        return new LotteryDataBusiness_SDQYH().QuerySDQYH_GameWinNumber(pageIndex, pageSize);
                    case "SSQ":
                        return new LotteryDataBusiness_SSQ().QuerySSQ_GameWinNumber(pageIndex, pageSize);
                    case "SD11X5":
                        return new LotteryDataBusiness_YDJ11().QueryYDJ11_GameWinNumber(pageIndex, pageSize);
                    case "SDKLPK3":
                        return new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_GameWinNumber(pageIndex, pageSize);
                    case "CTZQ_T14C":
                    case "CTZQ_TR9":
                    case "CTZQ_T6BQC":
                    case "CTZQ_T4CJQ":
                        return new LotteryDataBusiness_CTZQ(gameCode).QueryCTZQ_GameWinNumber(pageIndex, pageSize);
                    default:
                        break;
                }
                throw new Exception("没有匹配的彩种: " + gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询号码数据 - " + ex.Message, ex);
            }
        }
        public GameWinNumber_InfoCollection QueryGameWinNumberByDate(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "CQSSC":
                        return new LotteryDataBusiness_CQSSC().QueryCQSSC_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "JX11X5":
                        return new LotteryDataBusiness_JX11X5().QueryJX11X5_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "DLT":
                        return new LotteryDataBusiness_DLT().QueryDLT_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "SSQ":
                        return new LotteryDataBusiness_SSQ().QuerySSQ_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "FC3D":
                        return new LotteryDataBusiness_FC3D().QueryFC3D_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "PL3":
                        return new LotteryDataBusiness_PL3().QueryPL3_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "SD11X5":
                        return new LotteryDataBusiness_YDJ11().QueryYDJ11_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "GD11X5":
                        return new LotteryDataBusiness_GD11X5().QueryGD11X5_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "GDKLSF":
                        return new LotteryDataBusiness_GDKLSF().QueryGDKLSF_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "JSKS":
                        return new LotteryDataBusiness_JSK3().QueryJSK3_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "SDKLPK3":
                        return new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "CTZQ_T14C":
                    case "CTZQ_TR9":
                    case "CTZQ_T6BQC":
                    case "CTZQ_T4CJQ":
                        return new LotteryDataBusiness_CTZQ(gameCode).QueryCTZQ_GameWinNumber(startTime, endTime, pageIndex, pageSize);


                    default:
                        break;
                }
                throw new Exception("没有匹配的彩种: " + gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询号码数据 - " + ex.Message, ex);
            }
        }

        public GameWinNumber_InfoCollection QueryGameWinNumberByDateDesc(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "CQSSC":
                        return new LotteryDataBusiness_CQSSC().QueryCQSSC_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "JX11X5":
                        return new LotteryDataBusiness_JX11X5().QueryJX11X5_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "DLT":
                        return new LotteryDataBusiness_DLT().QueryDLT_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "SSQ":
                        return new LotteryDataBusiness_SSQ().QuerySSQ_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "FC3D":
                        return new LotteryDataBusiness_FC3D().QueryFC3D_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "PL3":
                        return new LotteryDataBusiness_PL3().QueryPL3_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    case "SD11X5":
                        return new LotteryDataBusiness_YDJ11().QueryYDJ11_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "GD11X5":
                        return new LotteryDataBusiness_GD11X5().QueryGD11X5_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "GDKLSF":
                        return new LotteryDataBusiness_GDKLSF().QueryGDKLSF_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "JSKS":
                        return new LotteryDataBusiness_JSK3().QueryJSK3_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "SDKLPK3":
                        return new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize);
                    case "CTZQ_T14C":
                    case "CTZQ_TR9":
                    case "CTZQ_T6BQC":
                    case "CTZQ_T4CJQ":
                        return new LotteryDataBusiness_CTZQ(gameCode).QueryCTZQ_GameWinNumber(startTime, endTime, pageIndex, pageSize);
                    default:
                        break;
                }
                throw new Exception("没有匹配的彩种: " + gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询号码数据 - " + ex.Message, ex);
            }
        }
        public GameWinNumber_InfoCollection QueryAllGameNewWinNumber(string gameString)
        {
            //gameString:SSQ|DLT|FC3D
            var collection = new GameWinNumber_InfoCollection();
            var array = gameString.Split('|');
            foreach (var item in array)
            {
                var list = QueryGameWinNumber(item, 0, 1);
                collection.List.AddRange(list.List);
            }
            return collection;
        }

        public GameWinNumber_Info QueryNewWinNumber(string gameCode)
        {
            var list = QueryGameWinNumber(gameCode, 0, 1);
            if (list.List.Count == 0) return new GameWinNumber_Info();
            return list.List[0];
        }
        public KJGameIssuse QueryKJGameIssuse(string gameCode, string issuseNumber)
        {
            return DB.CreateQuery<KJGameIssuse>().FirstOrDefault(p => p.GameCode == gameCode && p.IssuseNumber == issuseNumber);
        }
        public List<LotteryIssuse_QueryInfo> QueryAllGameCurrentIssuse(bool byOfficial)
        {           
            var list = new List<LotteryIssuse_QueryInfo>();
            var sql = string.Format(@"select g.GameCode,g.IssuseNumber,g.LocalStopTime,g.OfficialStopTime, convert(int, c.ConfigValue)ConfigValue
                        from (
                        SELECT GameCode,min(IssuseNumber)IssuseNumber,min(OfficialStopTime)OfficialStopTime,min(LocalStopTime)LocalStopTime
                          FROM [C_Game_Issuse]
                          where gamecode in ('ssq','dlt','fc3d','pl3','cqssc','jx11x5')
                          and {0}>getdate()
                          group by gamecode
                          ) as g
                          left join [C_Core_Config] c on 'Site.GameDelay.'+g.GameCode=c.configkey", byOfficial ? "OfficialStopTime" : "LocalStopTime");

            var array = DB.CreateSQLQuery(sql).List<LotteryIssuse_QueryInfo>();
            if (array == null)
                return list;
            var schemeIdList = new List<string>();
            return list;
        }
        public BJDCIssuseInfo QueryBJDCCurrentIssuseInfo()
        {
            var query = from b in DB.CreateQuery<C_BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select new { b };
            var queryResult = query.ToList().Select(z => new BJDCIssuseInfo
            {
                IssuseNumber = z.b.IssuseNumber,
                MinLocalStopTime = z.b.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MinMatchStartTime = z.b.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            });
            return queryResult.FirstOrDefault();
        }
        public CTZQMatchInfo_Collection QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber, string userToken)
        {
            var collection = new CTZQMatchInfo_Collection();
            //var UserId = new UserAuthentication().ValidateUserAuthentication(userToken);
            var query = (from s in DB.CreateQuery<C_CTZQ_Match>()
                         where s.GameType == gameType && s.IssuseNumber == issuseNumber
                         select new { s });
            collection.ListInfo = query.ToList().Select(b => new CTZQMatchInfo
            {
                GameCode = b.s.GameCode,
                GameType = b.s.GameType,
                GuestTeamHalfScore = b.s.GuestTeamHalfScore,
                GuestTeamId = b.s.GuestTeamId,
                GuestTeamName = b.s.GuestTeamName,
                GuestTeamScore = b.s.GuestTeamScore,
                GuestTeamStanding = b.s.GuestTeamStanding.ToString(),
                HomeTeamHalfScore = b.s.HomeTeamHalfScore,
                HomeTeamId = b.s.HomeTeamId,
                HomeTeamName = b.s.HomeTeamName,
                HomeTeamScore = b.s.HomeTeamScore,
                HomeTeamStanding = b.s.HomeTeamStanding.ToString(),
                Id = b.s.Id,
                IssuseNumber = b.s.IssuseNumber,
                MatchId = b.s.MatchId,
                MatchName = b.s.MatchName,
                MatchResult = b.s.MatchResult == null ? "" : b.s.MatchResult,
                MatchStartTime = b.s.MatchStartTime,
                Mid = b.s.Mid,
                OrderNumber = b.s.OrderNumber,
                UpdateTime = b.s.UpdateTime,
            }).ToList();
            return collection;
        }
    }
}
