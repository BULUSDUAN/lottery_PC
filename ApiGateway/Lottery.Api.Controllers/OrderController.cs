using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.RequestModel;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Helper;
using KaSon.FrameWork.Helper.分析器工厂;
using Lottery.ApiGateway.Model.Enum;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static KaSon.FrameWork.Helper.JsonHelper;

namespace Lottery.Api.Controllers
{
    [Area("Order")]
    public class OrderController : BaseController
    {
        /// <summary>
        /// 查询订单开奖历史记录_100
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public LotteryServiceResponse QueryOrderHistoryRecord(LotteryServiceRequest entity)
        {
            try
            {
                //读取json数据
                var param = WebHelper.Decode(entity.Param);
                //读取json文件
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var fileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}.json", param.GameCode));
                var data = new GameWinNumber_InfoCollection();
                if (System.IO.File.Exists(fileFullName))
                {
                    var jsonData = System.IO.File.ReadAllText(fileFullName, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(jsonData.Trim()))
                    {
                        data = JsonHelper.Deserialize<GameWinNumber_InfoCollection>(jsonData);
                    }
                }

                var list = new List<object>();
                foreach (var item in data.List)
                {
                    list.Add(new
                    {
                        item.IssuseNumber,
                        item.WinNumber,
                        CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                    });
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单开奖历史记录成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询中奖列表_113
        /// </summary>
        public async Task<IActionResult> QueryBonusList([FromServices]IServiceProxyProvider _serviceProxyProvider,LotteryServiceRequest entity)
        {
            try
            {


                var p = WebHelper.Decode(entity.Param);
                if (string.IsNullOrEmpty(p.GameCode))
                    throw new Exception("彩种不能为空");
                //param.userToken = p.UserToken;
                string GameCode = p.GameCode.ToUpper();
                string gameType = p.GameType.ToUpper();
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string key = p.KeyWord;
                Dictionary<string, object> param = new Dictionary<string, object> {
                { "gameCode", GameCode },{"gameType", gameType },{ "pageIndex", pageIndex},{ "pageSize", pageSize},{ "key", key}
                };
                var list = new List<object>();
                var _issuseNumber = string.Empty;
                var _completeData = string.Empty;
                
                var bonusList = await _serviceProxyProvider.Invoke<BonusOrderInfoCollection>(param, "api/Order/QueryBonusInfoList");
                if (bonusList != null && bonusList.BonusOrderList.Count > 0)
                {
                    foreach (var item in bonusList.BonusOrderList)
                    {
                        list.Add(new
                        {
                            item.SchemeId,
                            BonusMoney = item.AfterTaxBonusMoney,
                            item.DisplayName,
                            item.UserId,
                            SchemeType = (int)item.SchemeType,
                            item.BetCount,
                            item.TotalMoney,
                            IssuseNumber = string.IsNullOrEmpty(item.IssuseNumber) ? string.Empty : item.IssuseNumber,
                        });
                    }
                }
                return Json(new LotteryServiceResponse { Code = ResponseCode.成功, Message = "查询中奖列表成功", MsgId = entity.MsgId, Value = list });                
            }            
            catch (Exception ex)
            {               
                return Json(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "查询中奖列表失败", MsgId = entity.MsgId, Value = null });
            }
        }
        /// <summary>
        /// 查询彩种当前开奖信息_128
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public LotteryServiceResponse QueryGameCurrBonusInfo(LotteryServiceRequest entity)
        {
            try
            {
                //读取json文件
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var fileFullName = Path.Combine(path, "lottery_new_open_numbers.json");
                var data = new GameWinNumber_InfoCollection();
                if (System.IO.File.Exists(fileFullName))
                {
                    var jsonData = System.IO.File.ReadAllText(fileFullName, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(jsonData.Trim()))
                    {
                        data = Deserialize<GameWinNumber_InfoCollection>(jsonData);
                    }
                }

                var list = new List<object>();
                foreach (var item in data.List)
                {
                    list.Add(new
                    {
                        item.Id,
                        item.WinNumber,
                        item.IssuseNumber,
                        item.GameType,
                        item.GameCode,
                        CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                    });
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询最新开奖成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询最新开奖失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 竞彩查询开奖历史_129
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryJingCaiBonusHistory([FromServices]IServiceProxyProvider _serviceProxyProvider,LotteryServiceRequest entity)
        {
            try
            {
                
                var p = WebHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string issuseNumber = p.IssuseNumber;
                string gameType = p.GameType;
                int pageIndex= p.PageIndex == null ? 0 : Convert.ToInt32(p.PageIndex);
                int pageSize= p.PageSize == null ? 0 : Convert.ToInt32(p.PageSize);
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                 { "gameCode", gameCode },{ "issuseNumber", issuseNumber },{ "gameType",gameType },{ "pageIndex", pageIndex },{ "pageSize",pageSize }
                };
                DateTime startTime = string.IsNullOrEmpty(p.StartTime) ? DateTime.Now : Convert.ToDateTime(p.StartTime);
                DateTime endTime = string.IsNullOrEmpty(p.EndTime) ? DateTime.Now : Convert.ToDateTime(p.EndTime);                
                if (string.IsNullOrEmpty(gameCode))
                    throw new Exception("传入彩种不能为空");
                var list = new List<object>();
                if (p.gameCode.ToUpper() == "JCZQ")
                {
                    //读取json
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var jczqFileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}_{1}.json", gameCode, endTime.ToString("yyyyMMdd")));
                    var result = new JCZQMatchResult_Collection();
                    if (System.IO.File.Exists(jczqFileFullName))
                    {
                        var jsonData = System.IO.File.ReadAllText(jczqFileFullName, Encoding.UTF8);
                        if (!string.IsNullOrEmpty(jsonData.Trim()))
                        {
                            result = Deserialize<JCZQMatchResult_Collection>(jsonData);
                        }
                    }

                    //var result = WCFClients.GameIssuseClient.QueryJCZQMatchResult(startTime, endTime, pageIndex, pageSize);
                    if (result != null && result.MatchResultList.Count > 0)
                    {
                        foreach (var item in result.MatchResultList)
                        {
                            list.Add(new
                            {
                                MatchId = item.MatchId,
                                MatchIdName = item.MatchIdName,
                                StartTime = ConvertHelper.ConvertDateTimeInt(item.StartTime),
                                LeagueId = item.LeagueId,
                                LeagueName = item.LeagueName,
                                LeagueColor = item.LeagueColor,
                                HomeTeamId = item.HomeTeamId,
                                HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName),
                                GuestTeamId = item.GuestTeamId,
                                GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("nbsp;", ""),
                                LetBall = item.LetBall,
                                WinOdds = item.WinOdds,
                                FlatOdds = item.FlatOdds,
                                LoseOdds = item.LoseOdds,
                                MatchState = item.MatchState,
                                HalfHomeTeamScore = item.HalfHomeTeamScore,
                                HalfGuestTeamScore = item.HalfGuestTeamScore,
                                FullHomeTeamScore = item.FullHomeTeamScore,
                                FullGuestTeamScore = item.FullGuestTeamScore,
                                SPF_Result = item.SPF_Result,
                                SPF_Name = ConvertHelper.ANTECODES_JCZQ("spf", item.SPF_Result),
                                SPF_SP = item.SPF_SP,
                                BRQSPF_Result = item.BRQSPF_Result,
                                BRQSPF_Name = ConvertHelper.ANTECODES_JCZQ("brqspf", item.BRQSPF_Result),
                                BRQSPF_SP = item.BRQSPF_SP,
                                ZJQ_Result = item.ZJQ_Result,
                                ZJQ_Name = ConvertHelper.ANTECODES_JCZQ("zjq", item.ZJQ_Result),
                                ZJQ_SP = item.ZJQ_SP,
                                BF_Result = item.BF_Result,
                                BF_Name = ConvertHelper.ANTECODES_JCZQ("bf", item.BF_Result),
                                BF_SP = item.BF_SP,
                                BQC_Result = item.BQC_Result,
                                BQC_Name = ConvertHelper.ANTECODES_JCZQ("bqc", item.BQC_Result),
                                BQC_SP = item.BQC_SP,
                                CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                            });
                        }
                    }
                }
                else if (p.gameCode.ToUpper() == "JCLQ")
                {
                    //读取json
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var jczqFileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}_{1}.json", gameCode, endTime.ToString("yyyyMMdd")));
                    var result = new JCLQMatchResult_Collection();
                    if (System.IO.File.Exists(jczqFileFullName))
                    {
                        var jsonData = System.IO.File.ReadAllText(jczqFileFullName, Encoding.UTF8);
                        if (!string.IsNullOrEmpty(jsonData.Trim()))
                        {
                            result = Deserialize<JCLQMatchResult_Collection>(jsonData);
                        }
                    }
                    //var result = WCFClients.GameIssuseClient.QueryJCLQMatchResult(startTime, endTime, pageIndex, pageSize);
                    if (result != null && result.MatchResultList.Count > 0)
                    {
                        foreach (var item in result.MatchResultList)
                        {
                            list.Add(new
                            {
                                MatchId = item.MatchId,
                                MatchIdName = item.MatchIdName,
                                StartTime = ConvertHelper.ConvertDateTimeInt(item.StartTime),
                                LeagueId = item.LeagueId,
                                LeagueName = item.LeagueName,
                                LeagueColor = item.LeagueColor,
                                HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName),
                                GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("nbsp;", ""),
                                MatchState = item.MatchState,
                                HomeTeamScore = item.HomeTeamScore,
                                GuestTeamScore = item.GuestTeamScore,
                                SF_Result = item.SF_Result,
                                SF_Name = ConvertHelper.ANTECODES_JCLQ("sf", item.SF_Result),
                                SF_SP = item.SF_SP,
                                RFSF_Result = item.RFSF_Result,
                                RFSF_Name = ConvertHelper.ANTECODES_JCLQ("rfsf", item.RFSF_Result),
                                RFSF_SP = item.RFSF_SP,
                                DXF_Result = item.DXF_Result,
                                DXF_Name = ConvertHelper.ANTECODES_JCLQ("dxf", item.DXF_Result),
                                DXF_SP = item.DXF_SP,
                                SFC_Result = item.SFC_Result,
                                SFC_Name = ConvertHelper.ANTECODES_JCLQ("sfc", item.SFC_Result),
                                SFC_SP = item.SFC_SP,
                                RFSF_Trend = item.RFSF_Trend,
                                DXF_Trend = item.DXF_Trend,
                                CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                            });
                        }
                    }
                }
                else if (p.gameCode.ToUpper() == "BJDC")
                {
                    if (string.IsNullOrEmpty(p.issuseNumber))
                        throw new Exception("传入期号不能为空");
                    var result = await _serviceProxyProvider.Invoke<BJDCMatchResultInfo_Collection>(param, "api/Order/QueryBJDC_MatchResultCollection");
                    if (result != null && result.ListInfo.Count > 0)
                    {
                        foreach (var item in result.ListInfo)
                        {
                            list.Add(new
                            {
                                BF_Result = item.BF_Result == null ? "" : item.BF_Result,
                                BF_Name = ConvertHelper.ANTECODES_BJDC("bf", item.BF_Result),
                                BF_SP = item.BF_SP>0 ? 0 : item.BF_SP,
                                BQC_Result = item.BQC_Result == null ? "" : item.BQC_Result,
                                BQC_Name = ConvertHelper.ANTECODES_BJDC("bqc", item.BQC_Result),
                                BQC_SP = item.BQC_SP>0 ? 0 : item.BQC_SP,
                                CreateTime = item.CreateTime,
                                FlatOdds = item.FlatOdds>0 ? 0 : item.FlatOdds,
                                GuestFull_Result = item.GuestFull_Result == null ? "" : item.GuestFull_Result,
                                GuestHalf_Result = item.GuestHalf_Result == null ? "" : item.GuestHalf_Result,
                                GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("nbsp;", ""),
                                HomeFull_Result = item.HomeFull_Result == null ? "" : item.HomeFull_Result,
                                HomeHalf_Result = item.HomeHalf_Result == null ? "" : item.HomeHalf_Result,
                                HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName),
                                Id = item.Id,
                                IssuseNumber = item.IssuseNumber,
                                LetBall = item.LetBall,
                                LoseOdds = item.LoseOdds,
                                MatchColor = item.MatchColor,
                                MatchName = item.MatchName,
                                MatchOrderId = item.MatchOrderId,
                                MatchStartTime = ConvertHelper.ConvertDateTimeInt(item.MatchStartTime),
                                MatchState = item.MatchState,
                                SPF_Result = item.SPF_Result == null ? "" : item.SPF_Result,
                                SPF_Name = ConvertHelper.ANTECODES_BJDC("spf", item.SPF_Result),
                                SPF_SP = item.SPF_SP,
                                SXDS_Result = item.SXDS_Result == null ? "" : item.SXDS_Result,
                                SXDS_Name = ConvertHelper.ANTECODES_BJDC("sxds", item.SXDS_Result),
                                SXDS_SP = item.SXDS_SP,
                                WinOdds = item.WinOdds,
                                ZJQ_Result = item.ZJQ_Result == null ? "" : item.ZJQ_Result,
                                ZJQ_Name = ConvertHelper.ANTECODES_BJDC("zjq", item.ZJQ_Result),
                                ZJQ_SP = item.ZJQ_SP
                            });
                        }
                    }
                }
                else if (p.gameCode.ToUpper() == "CTZQ")
                {
                    if (string.IsNullOrEmpty(p.gameType))
                        throw new Exception("玩法不能为空");
                    else if (string.IsNullOrEmpty(p.issuseNumber))
                        throw new Exception("期号不能为空");

                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var ctzqFileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}_{1}.json", p.gameType, p.issuseNumber));
                    var result = new CTZQMatch_PoolInfo_Collection();
                    if (System.IO.File.Exists(ctzqFileFullName))
                    {
                        var jsonData = System.IO.File.ReadAllText(ctzqFileFullName, Encoding.UTF8);
                        if (!string.IsNullOrEmpty(jsonData.Trim()))
                        {
                            result = Deserialize<CTZQMatch_PoolInfo_Collection>(jsonData);
                        }
                    }
                    foreach (var item in result.ListInfo)
                    {
                        list.Add(new
                        {
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            GuestTeamHalfScore = item.GuestTeamHalfScore,
                            GuestTeamId = item.GuestTeamId,
                            GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("nbsp;", ""),
                            GuestTeamScore = item.GuestTeamScore,
                            GuestTeamStanding = item.GuestTeamStanding,
                            HomeTeamHalfScore = item.HomeTeamHalfScore,
                            HomeTeamId = item.HomeTeamId,
                            HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName),
                            HomeTeamScore = item.HomeTeamScore,
                            HomeTeamStanding = item.HomeTeamStanding,
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            MatchResult = item.MatchResult == null ? "" : item.MatchResult,
                            MatchStartTime = ConvertHelper.ConvertDateTimeInt(item.MatchStartTime),
                            Mid = item.Mid,
                            OrderNumber = item.OrderNumber,
                            UpdateTime = ConvertHelper.ConvertDateTimeInt(item.UpdateTime),
                            BonusTime = item.BounsTime.Year < 1900 ? -1 : ConvertHelper.ConvertDateTimeInt(item.BounsTime),
                        });
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询开奖历史成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询历史开奖失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询账户明细_132
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryAccountDetial([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                

                var p = WebHelper.Decode(entity.Param);
                string viewType = p.ViewType;
                string userToken = p.UserToken;
                DateTime startTime = p.StartTime == null ? DateTime.Now : Convert.ToDateTime(p.StartTime);
                DateTime endTime = p.EndTime == null ? DateTime.Now : Convert.ToDateTime(p.EndTime);
                int days = p.Days;
                startTime = startTime.AddDays(-days).Date;
                int PageIndex = p.PageIndex??0;
                int pageSize = p.PageSize ?? 1;
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "viewType", viewType },{ "userToken", userToken },{ "startTime", startTime },{ "endTime", endTime },{ "pageIndex", PageIndex },{ "pageSize", pageSize }
                };
                if (string.IsNullOrEmpty(userToken))
                    throw new ArgumentException("您还未登陆");
                //endTime = endTime.AddDays(1);

                var list = new List<object>();
                if (p.viewType.ToUpper() == "ZHMX")
                {
                    string accountType = p.AccoountType;
                    if (string.IsNullOrEmpty(accountType))
                        accountType = string.Empty;
                    param.Add("accountType", accountType);
                    param.Add("categoryList", "");
                    var FundDetails = await _serviceProxyProvider.Invoke<UserFundDetailCollection>(param, "api/Order/QueryMyFundDetailList");
                    if (FundDetails != null && FundDetails.FundDetailList.Count > 0)
                    {
                        foreach (var item in FundDetails.FundDetailList)
                        {
                            var StrAccountType = ConvertHelper.GetAccountType((int)item.AccountType);
                            list.Add(new
                            {
                                Id = item.Id,
                                UserId = item.UserId,
                                AccountType = item.AccountType,
                                StrAccountType = StrAccountType,
                                AfterBalance = item.AfterBalance,
                                BeforeBalance = item.BeforeBalance,
                                Category = item.Category + (StrAccountType == "充值" ? "" : "|" + ConvertHelper.GetAccountType((int)item.AccountType)),
                                KeyLine = item.KeyLine,
                                OperatorId = item.OperatorId,
                                OrderId = item.OrderId,
                                PayMoney = item.PayMoney,
                                PayType = item.PayType,
                                Summary = item.Summary,
                                CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                                TotalPayinCount = FundDetails.TotalPayinCount,
                                TotalPayinMoney = FundDetails.TotalPayinMoney,
                                TotalPayoutCount = FundDetails.TotalPayinMoney,
                                TotalPayoutMoney = FundDetails.TotalPayoutMoney,
                                SchemeAddress = ConvertHelper.GetDomain() + "/user/scheme/" + item.OrderId,
                            });
                        }
                    }
                }
                else if (p.viewType.ToUpper() == "CZJL")
                {
                    param.Add("statusList", "1");
                    var FillMoneyCollection = await _serviceProxyProvider.Invoke<FillMoneyQueryInfoCollection>(param, "api/Order/QueryFillMoneyList");
                    if (FillMoneyCollection != null && FillMoneyCollection.FillMoneyList.Count > 0)
                    {
                        foreach (var item in FillMoneyCollection.FillMoneyList)
                        {
                            list.Add(new
                            {
                                FillMoneyAgentType = item.FillMoneyAgent,
                                StrFillMoneyAgentType = ConvertHelper.GetFillMoneyAgentType(item.FillMoneyAgent),
                                OrderId = item.OrderId,
                                RequestTime = ConvertHelper.ConvertDateTimeInt(item.RequestTime),
                                ResponseTime = ConvertHelper.ConvertDateTimeInt(item.ResponseTime.Value),
                                FillMoneyStatus = item.Status,
                                StrFillMoneyStatus = ConvertHelper.GetFillMoneyStatus(item.Status),
                                RequestMoney = item.RequestMoney,
                                TotalCount = FillMoneyCollection.TotalCount,
                                TotalRequestMoney = FillMoneyCollection.TotalRequestMoney,
                                TotalResponseMoney = FillMoneyCollection.TotalResponseMoney,
                            });
                        }
                    }
                }
                else if (p.viewType.ToUpper() == "GCJL")
                {
                    //OrderQueryType orderType = (OrderQueryType)p.OrderType;
                    var result = await _serviceProxyProvider.Invoke<MyBettingOrderInfoCollection>(param, "api/Order/QueryMyBettingOrderList");
                    if (result != null && result.OrderList != null)
                    {
                        foreach (var item in result.OrderList)
                        {
                            list.Add(new
                            {
                                BuyTime = ConvertHelper.ConvertDateTimeInt(item.BuyTime),
                                BuyMoney = item.BuyMoney,
                                SchemeBettingCategory = item.SchemeBettingCategory,
                                StrSchemeBettingCategory = ConvertHelper.GetSchemeBettingCategory(item.SchemeBettingCategory),
                                SchemeId = item.SchemeId,
                                TotalCount = result.TotalCount,
                                TotalBuyMoney = result.TotalBuyMoney,
                                TotalBonusMoney = result.TotalBonusMoney,
                            });
                        }
                    }
                }
                else if (p.viewType.ToUpper() == "ZJJL")
                {
                    param.Add("accountType", "10");
                    param.Add("categoryList", "奖金");
                    var result =await _serviceProxyProvider.Invoke<UserFundDetailCollection>(param, "api/Order/QueryMyFundDetailList");
                    if (result != null && result.FundDetailList != null)
                    {
                        foreach (var item in result.FundDetailList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                UserId = item.UserId,
                                AccountType = item.AccountType,
                                AfterBalance = item.AfterBalance,
                                BeforeBalance = item.BeforeBalance,
                                Category = item.Category,
                                KeyLine = item.KeyLine,
                                OperatorId = item.OperatorId,
                                OrderId = item.OrderId,
                                PayMoney = item.PayMoney,
                                PayType = item.PayType,
                                Summary = item.Summary,
                                CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                                TotalPayinCount = result.TotalPayinCount,
                                TotalPayinMoney = result.TotalPayinMoney,
                                TotalPayoutCount = result.TotalPayoutCount,
                                TotalPayoutMoney = result.TotalPayoutMoney,
                            });
                        }
                    }
                }
                else if (p.viewType.ToUpper() == "TKJL")
                {
                    param.Add("WithdrawStatus", WithdrawStatus.Success);
                    var result = await _serviceProxyProvider.Invoke<Withdraw_QueryInfoCollection>(param, "api/Order/QueryMyWithdrawList");
                    if (result != null && result.WithdrawList != null)
                    {
                        foreach (var item in result.WithdrawList)
                        {
                            list.Add(new
                            {
                                BankCardNumber = item.BankCardNumber,
                                BankCode = item.BankCode,
                                BankName = item.BankName,
                                BankSubName = item.BankSubName,
                                CityName = item.CityName,
                                OrderId = item.OrderId,
                                ProvinceName = item.ProvinceName,
                                RequestMoney = item.RequestMoney,
                                RequestTime = ConvertHelper.ConvertDateTimeInt(item.RequestTime),
                                ResponseTime = ConvertHelper.ConvertDateTimeInt(item.ResponseTime.Value),
                                ResponseMoney = item.ResponseMoney,
                                WithdrawAgent = item.WithdrawAgent,
                                StrWithdrawAgent = ConvertHelper.WithdrawAgentTypeName(item.WithdrawAgent),
                                Status = item.Status,
                                StrStatus = ConvertHelper.GetWithdrawStatus(item.Status),
                                ResponseMessage = item.ResponseMessage,
                                RequesterDisplayName = item.RequesterDisplayName,
                                RequesterUserKey = item.RequesterUserKey,
                                TotalMoney = result.TotalMoney,
                                TotalRefusedMoney = result.TotalRefusedMoney,
                                TotalResponseMoney = result.TotalResponseMoney,
                                TotalWinMoney = result.TotalWinMoney,
                            });
                        }
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询账户明细成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询账户明细出错",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 根据中奖状态查询投注记录_133
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryOrderListByBonusState([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userId = p.UserId;
                int state = p.BonusStatus;
                //int schemeType = p.SchemeType;
                int days = p.ViewDay;
                DateTime startTime = DateTime.Now.AddDays(-days);
                DateTime endTime = DateTime.Now;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string userToken = p.UserToken;
                int orderQueryType = p.OrderQueryType;
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "userId", userId },{ "state", (BonusStatus?)state },{ "days", days },{ "startTime", startTime },{ "endTime", endTime },
                    { "pageIndex", pageIndex },{ "pageSize", pageSize },{ "userToken", userToken }
                };
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userToken))
                    throw new ArgumentException("您还未登陆，请登陆后查询");

                var list = new List<object>();
                if (orderQueryType == 1)
                {
                    //代购
                    var orderList = await _serviceProxyProvider.Invoke<MyBettingOrderInfoCollection>(param, "api/Order/QueryMyBettingOrderList");
                    if (orderList != null && orderList.OrderList != null)
                    {
                        foreach (var item in orderList.OrderList)
                        {
                            list.Add(new
                            {
                                SchemeId = item.SchemeId,
                                GameCode = item.GameCode,
                                GameName = ConvertHelper.GameName(item.GameCode, item.GameType),
                                GameTypeName = item.GameTypeName,
                                CreatorDisplayName = item.CreatorDisplayName,
                                IssuseNumber = item.IssuseNumber,
                                CurrentBettingMoney = item.BuyMoney,
                                BetTime = ConvertHelper.ConvertDateTimeInt(item.BetTime),
                                SchemeType = item.SchemeType,
                                TicketStatus = item.TicketStatus,
                                ProgressStatus = item.ProgressStatus,
                                PreTaxBonusMoney = item.PreTaxBonusMoney,
                                AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                                BonusStatus = item.BonusStatus,
                                AddMoney = item.AddMoney,
                                StrBonusStatus = ConvertHelper.BonusStatusName(item.BonusStatus),
                                StrOrderStateName = ConvertHelper.GetOrderStatusName(item.SchemeType, item.ProgressStatus, item.TicketStatus, item.BonusStatus, true, true, false),
                                TotalMoney = item.TotalMoney,
                                CreateTime = ConvertHelper.ConvertDateTimeInt(item.BuyTime)
                            });
                        }
                    }
                }
                if (orderQueryType == 2)
                {
                    //发起的合买
                    var createList = await _serviceProxyProvider.Invoke<TogetherOrderInfoCollection>(param, "api/Order/QueryCreateTogetherOrderListByUserId");
                    foreach (var item in createList.OrderList)
                    {
                        list.Add(new
                        {
                            SchemeId = item.SchemeId,
                            GameCode = item.GameCode,
                            GameName = ConvertHelper.GameName(item.GameCode, item.GameType),
                            GameTypeName = item.GameTypeName,
                            CreatorDisplayName = item.CreatorDisplayName,
                            IssuseNumber = item.IssuseNumber,
                            CurrentBettingMoney = item.TotalMoney,
                            BetTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                            SchemeType = item.SchemeType,
                            TicketStatus = item.TicketStatus,
                            ProgressStatus = item.ProgressStatus,
                            PreTaxBonusMoney = item.PreTaxBonusMoney,
                            AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                            BonusStatus = item.BonusStatus,
                            AddMoney = item.AddMoney,
                            StrBonusStatus = ConvertHelper.BonusStatusName(item.BonusStatus),
                            StrOrderStateName = ConvertHelper.GetOrderStatusName(item.SchemeType, item.ProgressStatus, item.TicketStatus, item.BonusStatus, true, true, false),
                            TotalMoney = item.TotalMoney,
                            CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime)
                        });
                    }
                }
                if (orderQueryType == 3)
                {
                    //参与的合买
                    var joinList = await _serviceProxyProvider.Invoke<TogetherOrderInfoCollection>(param, "api/Order/QueryJoinTogetherOrderListByUserId");
                    foreach (var item in joinList.OrderList)
                    {
                        list.Add(new
                        {
                            SchemeId = item.SchemeId,
                            GameCode = item.GameCode,
                            GameName = ConvertHelper.GameName(item.GameCode, item.GameType),
                            GameTypeName = item.GameTypeName,
                            CreatorDisplayName = item.CreatorDisplayName,
                            IssuseNumber = item.IssuseNumber,
                            CurrentBettingMoney = item.TotalMoney,
                            BetTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                            SchemeType = item.SchemeType,
                            TicketStatus = item.TicketStatus,
                            ProgressStatus = item.ProgressStatus,
                            PreTaxBonusMoney = item.PreTaxBonusMoney,
                            AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                            BonusStatus = item.BonusStatus,
                            AddMoney = item.AddMoney,
                            StrBonusStatus = ConvertHelper.BonusStatusName(item.BonusStatus),
                            StrOrderStateName = ConvertHelper.GetOrderStatusName(item.SchemeType, item.ProgressStatus, item.TicketStatus, item.BonusStatus, true, true, false),
                            TotalMoney = item.TotalMoney,
                            CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime)
                        });
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询投注记录成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询投注记录失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            
        }
        /// <summary>
        /// 查询合买跟单_137
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QuerySportsTogetherList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                var gameCode = p.GameCode;
                var gameType = p.GameType;
                var pageNo = p.Pageindex;
                var PageSize = p.PageSize;
                var orderBy = p.orderBy;
                var sortType = p.sortType;
                string userToken = p.UserToken;
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "gameCode", gameCode },{ "key", "" },
                    { "gameType", gameType }, { "issuseNumber", "" },
                    { "pageIndex", pageNo }, { "TogetherSchemeSecurity", null },
                    { "PageSize", PageSize }, { "betCategory", null },
                    { "orderBy", orderBy }, { "TogetherSchemeProgress", null },
                    { "sortType", sortType },{ "minMoney", -1 },
                    { "maxMoney", -1 },{ "minProgress", -1 },
                    { "maxProgress", -1 }
                };
                string userId = string.Empty;
                if (!string.IsNullOrEmpty(userToken))
                {
                    param.Add("userToken", userToken);
                    var userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");


                    if (userInfo != null)
                        userId = userInfo.UserId;
                }

                if (orderBy == "Progress")
                    orderBy = "ManYuan desc, Progress " + sortType + ",TotalMoney DESC,ISTOP DESC";
                else if (orderBy == "TotalMoney")
                    orderBy = "ManYuan desc,TotalMoney " + sortType + ", Progress DESC,ISTOP DESC";

                var list = new List<object>();
                
                //Sports_TogetherSchemeQueryInfoCollection result1 = WCFClients.GameQueryClient.QueryJoinTogetherOrderListByUserId(userId, null, gameCode, ViewBag.Begin, ViewBag.End, pageNo, PageSize);
                Sports_TogetherSchemeQueryInfoCollection result = await _serviceProxyProvider.Invoke<Sports_TogetherSchemeQueryInfoCollection>(param, "api/Order/QuerySportsTogetherListFromRedis");
                if (result != null && result.List.Count > 0)
                {
                    foreach (var item in result.List)
                    {
                        list.Add(new
                        {
                            SchemeId = item.SchemeId,
                            SchemeSource = item.SchemeSource,
                            IsTop = item.IsTop,
                            Title = item.Title,
                            Description = item.Description,
                            CreateUserId = item.CreateUserId,
                            CreaterDisplayName = item.CreaterDisplayName,
                            CreaterHideDisplayNameCount = item.CreaterHideDisplayNameCount,
                            JoinPwd = item.JoinPwd,
                            Security = item.Security,
                            TotalMoney = item.TotalMoney,
                            TotalCount = item.TotalCount,
                            SoldCount = item.SoldCount,
                            SurplusCount = item.SurplusCount,
                            JoinUserCount = item.JoinUserCount,
                            Price = item.Price,
                            BonusDeduct = item.BonusDeduct,
                            SchemeDeduct = item.SchemeDeduct,
                            Subscription = item.Subscription,
                            Guarantees = item.Guarantees,
                            SystemGuarantees = item.SystemGuarantees,
                            Progress = item.Progress,
                            ProgressStatus = item.ProgressStatus,
                            GameCode = item.GameCode,
                            GameDisplayName = item.GameDisplayName,
                            GameType = item.GameType,
                            GameTypeDisplayName = item.GameTypeDisplayName,
                            PlayType = item.PlayType,
                            StopTime = ConvertHelper.ConvertDateTimeInt(item.StopTime).ToString(),
                            TotalMatchCount = item.TotalMatchCount,
                            GoldCrownCount = item.GoldCrownCount,
                            GoldCupCount = item.GoldCupCount,
                            GoldDiamondsCount = item.GoldDiamondsCount,
                            GoldStarCount = item.GoldStarCount,
                            SilverCrownCount = item.SilverCrownCount,
                            SilverCupCount = item.SilverCupCount,
                            SilverDiamondsCount = item.SilverDiamondsCount,
                            SilverStarCount = item.SilverStarCount,
                            TicketStatus = item.TicketStatus,
                            SchemeBettingCategory = item.SchemeBettingCategory,
                        });
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询合买大厅成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询合买大厅失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询中奖榜单_138
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>

        public async Task<LotteryServiceResponse> QueryRankReport([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                var type = p.type;
                var time = p.time;
                var BeginTime = DateTime.Now.AddDays(-time);
                var EndTime = DateTime.Now.AddDays(1);
                var gameCode = p.GameCode;
                var gameType = p.GameType;
                var pageIndex = p.PageIndex;
                var pageSize = p.PageSize;
                List<object> list = new List<object>();
                if (type == "ljzj")
                {
                    //RankReportCollection_TotalBonus_Sport result = WCFClients.GameQueryClient.QueryRankReport_TotalBonus_Sport(BeginTime, EndTime, gameCode, gameType, pageIndex, pageSize);
                    //从jsonData查询数据
                    RankReportCollection_TotalBonus_Sport result = new RankReportCollection_TotalBonus_Sport();
                    var url = "/jsonData/bonus/" + type + "_" + gameCode + "" + (string.IsNullOrEmpty(gameType) ? string.Empty : "_" + gameType) + "/" + time + "_bonus.json";
                    var jsonData = GetJsonData(url);
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        result = JsonHelper.Deserialize<RankReportCollection_TotalBonus_Sport>(jsonData);
                        if (result != null && result.RankInfoList.Count > 0)
                            result.RankInfoList = result.RankInfoList.Skip((int)pageIndex * (int)pageSize).Take((int)pageSize).ToList(); /*result.RankInfoList.AsParallel().Skip(pageIndex * pageSize).Take(pageSize).ToList();*/
                        else result = new RankReportCollection_TotalBonus_Sport();
                    }
                    if (result != null && result.RankInfoList != null && result.RankInfoList.Count > 0)
                    {
                        foreach (var item in result.RankInfoList)
                        {
                            list.Add(new
                            {
                                BonusMoney = item.BonusMoney,
                                TotalOrderCount = item.TotalOrderCount,
                                TotalOrderMoney = item.TotalOrderMoney,
                                ProfitMoney = item.ProfitMoney,
                                UserId = item.UserId,
                                UserDisplayName = item.UserDisplayName,
                                UserHideDisplayNameCount = item.UserHideDisplayNameCount
                            });
                        }
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询中奖榜单成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询中奖榜单失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 新接口_查询订单详情_144
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryNewOrderDetailBySchemeId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new ArgumentException("您还未登录！");
                string schemeId = p.SchemeId;
                if (string.IsNullOrEmpty(schemeId))
                    throw new ArgumentException("订单号不能为空！");

                if (schemeId.StartsWith("CHASE"))
                {
                    return await QueryCHASEOrderDetail(_serviceProxyProvider, entity, schemeId, userToken);
                }
                else if (schemeId.StartsWith("TSM"))
                {
                    return await QueryTMSOrderDetail(_serviceProxyProvider, entity, schemeId, userToken);
                }
                else
                {
                    return await QueryGeneralOrderDetail(_serviceProxyProvider, entity, schemeId, userToken);
                }
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询订单详情失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }

        }
        /// <summary>
        /// 查看追号订单详情
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryCHASEOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider,LotteryServiceRequest entity, string schemeId, string userToken)
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "schemeId", schemeId },
                { "userToken", userToken }
            };
            var schemeInfo =await _serviceProxyProvider.Invoke<BettingOrderInfoCollection>(param, "api/Order/QueryBettingOrderListByChaseKeyLine");
            if (schemeInfo.OrderList.Count == 0)
                throw new Exception("追号方案不包括投注期信息");
            var firstIssuse = schemeInfo.OrderList[0];
            var userInfo =await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");

            var status = schemeInfo.OrderList.Min(t => t.ProgressStatus);
            var codeList = new List<object>();
            if (firstIssuse.Security == TogetherSchemeSecurity.Public
                || (firstIssuse.Security == TogetherSchemeSecurity.CompletePublic && status != ProgressStatus.Running && status != ProgressStatus.Waitting) || (firstIssuse.UserId == userInfo.UserId))
            {
                param.Clear();
                param.Add("SchemeId", firstIssuse.SchemeId);
                param.Add("userToken", userToken);
                var anteCodeList =await _serviceProxyProvider.Invoke<BettingAnteCodeInfoCollection>(param, "api/Order/QueryAnteCodeListBySchemeId");
                foreach (var code in anteCodeList.AnteCodeList)
                {
                    var betCount = AnalyzerFactory.GetAntecodeAnalyzer(code.GameCode, code.GameType).AnalyzeAnteCode(code.AnteCode);
                    codeList.Add(new
                    {
                        IssuseNumber = code.IssuseNumber,
                        GameTypeName = code.GameTypeName,
                        GameType = code.GameType,
                        GameName = code.GameName,
                        GameCode = code.GameCode,
                        AnteCode = code.AnteCode,
                        SchemeId = code.SchemeId,
                        BetCount = betCount,
                        CurrBetMoney = betCount * 2,
                    });
                }
            }
            var issuseList = new List<object>();
            foreach (var item in schemeInfo.OrderList)
            {
                issuseList.Add(new
                {
                    WinNumber = item.WinNumber,
                    IssuseNumber = item.IssuseNumber,
                    BettingMoney = item.CurrentBettingMoney,
                    Amount = item.Amount,
                    AddMoney = item.AddMoney,
                    AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                    BonusStatus = item.BonusStatus,
                    TicketStatus = item.TicketStatus,
                    ProgressStatus = item.ProgressStatus,
                    IsVirtualOrder = item.IsVirtualOrder,
                    SchemeType = item.SchemeType,
                    SchemeId = item.SchemeId,
                });
            }
            var result = new
            {
                ChaseId = schemeId,
                CurrSchemeId = firstIssuse.SchemeId,
                GameCode = firstIssuse.GameCode,
                GameDisplayName = firstIssuse.GameName,
                GameTypeDisplayName = firstIssuse.GameTypeName,
                UserId = firstIssuse.UserId,
                UserDisplayName = firstIssuse.CreatorDisplayName,
                HideDisplayNameCount = firstIssuse.HideDisplayNameCount,
                CreateTime = ConvertHelper.ConvertDateTimeInt(firstIssuse.CreateTime),
                TotalMoney = schemeInfo.TotalOrderMoney,
                TotalBuyMoney = schemeInfo.TotalBuyMoney,
                BonusMoney = schemeInfo.TotalAfterTaxBonusMoney,
                ChaseCount = schemeInfo.OrderList.Count,
                Security = firstIssuse.Security,
                SchemeSource = firstIssuse.SchemeSource,
                CurrIssuseBonusMoney = firstIssuse.AfterTaxBonusMoney,
                CurrIssuseBonusStatus = firstIssuse.BonusStatus,
                SchemeBettingCategory = (int)firstIssuse.SchemeBettingCategory,
                CodeList = codeList,
                IssuseList = issuseList,
                SchemeType = firstIssuse.SchemeType,
                IsVirtualOrder = firstIssuse.IsVirtualOrder,
                StopAfterBonus = firstIssuse.StopAfterBonus,
            };

            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询订单明细成功",
                MsgId = entity.MsgId,
                Value = result,
            };
        }
        /// <summary>
        /// 查看合买订单详情
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryTMSOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity, string schemeId, string userToken)
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                {"schemeId",schemeId },{"userToken",userToken },{ "PageIndex",0},{ "PageSize",100},{"MaxPageSize",200 }
            };            
            var schemeInfo =await _serviceProxyProvider.Invoke<Sports_TogetherSchemeQueryInfo>(param, "'api/Order/QuerySportsTogetherDetail");
            var userInfo =await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");
            var join = await _serviceProxyProvider.Invoke<Sports_TogetherJoinInfoCollection>(param, "api/Order/QuerySportsTogetherJoinList");

            var joinList = new List<object>();
            foreach (var item in join.List)
            {
                joinList.Add(new
                {
                    UserId = item.UserId,
                    UserDisplayName =ConvertHelper.HideUserName(item.UserDisplayName, item.HideDisplayNameCount),
                    HideDisplayNameCount = item.HideDisplayNameCount,
                    SchemeId = item.SchemeId,
                    RealBuyCount = item.RealBuyCount,
                    Price = item.Price,
                    JoinType = ConvertHelper.FomartJoinType(item.JoinType),
                    JoinId = item.JoinId,
                    JoinDateTime = ConvertHelper.ConvertDateTimeInt(item.JoinDateTime),
                    IsSucess = item.IsSucess,
                    BuyCount = item.BuyCount,
                    BonusMoney = item.BonusMoney,
                });
            }

            var codeList = new List<object>();
            if (schemeInfo.Security == TogetherSchemeSecurity.Public
               || (schemeInfo.Security == TogetherSchemeSecurity.CompletePublic && schemeInfo.StopTime <= DateTime.Now)
               || schemeInfo.CreateUserId == userInfo.UserId
                || (schemeInfo.Security == TogetherSchemeSecurity.JoinPublic && await _serviceProxyProvider.Invoke<bool>(param, "api/Order/IsUserJoinSportsTogether")))
            {
                var anteCodeList = await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                //codeList = GetCodeList(anteCodeList);
                codeList =await GetCodeList_GSAPP(_serviceProxyProvider,anteCodeList, schemeInfo.GameCode, schemeInfo.Amount);

            }

            var result = new
            {
                SchemeId = schemeInfo.SchemeId,
                GameCode = schemeInfo.GameCode,
                GameDisplayName = schemeInfo.GameDisplayName,
                GameType = schemeInfo.GameType,
                GameTypeDisplayName = schemeInfo.GameTypeDisplayName,
                UserId = schemeInfo.CreateUserId,
                UserDisplayName = schemeInfo.CreaterDisplayName,
                HideDisplayNameCount = schemeInfo.CreaterHideDisplayNameCount,
                MatchCount = schemeInfo.TotalMatchCount,
                PlayType = schemeInfo.PlayType,
                TotalMoney = schemeInfo.TotalMoney,
                TotalCount = schemeInfo.TotalCount,
                SoldCount = schemeInfo.SoldCount,
                StopTime =ConvertHelper.ConvertDateTimeInt(schemeInfo.StopTime),
                Price = schemeInfo.Price,
                Progress = schemeInfo.Progress,
                JoinPwd = schemeInfo.JoinPwd,
                BetCount = schemeInfo.BetCount,
                Amount = schemeInfo.Amount,
                BonusDeduct = schemeInfo.BonusDeduct,
                IssuseNumber = schemeInfo.IssuseNumber,
                SchemeDeduct = schemeInfo.SchemeDeduct,
                Subscription = schemeInfo.Subscription,
                Guarantees = schemeInfo.Guarantees,
                WinNumber = schemeInfo.WinNumber,
                CreateTime = ConvertHelper.ConvertDateTimeInt(schemeInfo.CreateTime),
                HitMatchCount = schemeInfo.HitMatchCount,
                BonusCount = schemeInfo.BonusCount,
                BonusMoney = schemeInfo.AfterTaxBonusMoney,
                AddMoney = schemeInfo.AddMoney,
                AddMoneyDescription = schemeInfo.AddMoneyDescription,
                TicketStatus = schemeInfo.TicketStatus,
                TogetherProgressStatus = schemeInfo.ProgressStatus,
                BonusStatus = schemeInfo.BonusStatus,
                Security = schemeInfo.Security,
                SchemeSource = schemeInfo.SchemeSource,
                SchemeBettingCategory = (int)schemeInfo.SchemeBettingCategory,
                IsPrizeMoney = schemeInfo.IsPrizeMoney,
                Title = schemeInfo.Title,
                Description = schemeInfo.Description,
                CodeList = codeList,
                JoinList = joinList,
                ServiceTime = ConvertHelper.ConvertDateTimeInt(DateTime.Now),
            };

            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询订单明细成功",
                MsgId = entity.MsgId,
                Value = result,
            };
        }
        /// <summary>
        /// 查询普通订单详情
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryGeneralOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider,LotteryServiceRequest entity, string schemeId, string userToken)
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                {"schemeId",schemeId },{"userToken",userToken }
            };
            var schemeInfo =await _serviceProxyProvider.Invoke<Sports_SchemeQueryInfo>(param, "api/Order/QuerySportsSchemeInfo");
            var userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");


            var codeList = new List<object>();
            if (schemeInfo.Security == TogetherSchemeSecurity.Public
               || (schemeInfo.Security == TogetherSchemeSecurity.CompletePublic && schemeInfo.StopTime <= DateTime.Now)
               || schemeInfo.UserId == userInfo.UserId)
            {
                if (schemeInfo.Security != TogetherSchemeSecurity.FirstMatchStopPublic)
                {
                    var anteCodeList =await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                    codeList =await GetCodeList_GSAPP(_serviceProxyProvider,anteCodeList, schemeInfo.GameCode, schemeInfo.Amount);
                }
                else if ((schemeInfo.Security == TogetherSchemeSecurity.FirstMatchStopPublic && schemeInfo.StopTime <= DateTime.Now))
                {
                    var anteCodeList =await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                    codeList =await GetCodeList_GSAPP(_serviceProxyProvider,anteCodeList, schemeInfo.GameCode, schemeInfo.Amount);
                }
            }
            string[] array_GameCode = new string[] { "JCZQ", "JCLQ", "CTZQ", "BJDC" };
            string winNumber = string.Empty;
            if (!array_GameCode.Contains(schemeInfo.GameCode))
            {
                Dictionary<string, object> Issuse_param = new Dictionary<string, object>()
                {
                    {"gameCode",schemeInfo.GameCode },{"gameType",string.Empty },{"issuseNumber",schemeInfo.IssuseNumber }
                };
                var IssuseInfo =await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(Issuse_param, "api/Order/QueryIssuseInfo");
                if (IssuseInfo != null)
                    winNumber = IssuseInfo.WinNumber;
            }
            //if (schemeInfo.GameCode.ToUpper() == "SJB")
            //{

            //}
            var result = new
            {
                SchemeId = schemeInfo.SchemeId,
                GameCode = schemeInfo.GameCode,
                GameDisplayName = schemeInfo.GameDisplayName,
                GameType = schemeInfo.GameType,
                GameTypeDisplayName = schemeInfo.GameTypeDisplayName,
                UserId = schemeInfo.UserId,
                UserDisplayName = schemeInfo.UserDisplayName,
                HideDisplayNameCount = schemeInfo.HideDisplayNameCount,
                MatchCount = schemeInfo.TotalMatchCount,
                PlayType = schemeInfo.PlayType,
                CreateTime = ConvertHelper.ConvertDateTimeInt(schemeInfo.CreateTime),
                TotalMoney = schemeInfo.TotalMoney,
                BonusMoney = schemeInfo.AfterTaxBonusMoney,
                AddMoney = schemeInfo.AddMoney,
                AddMoneyDescription = schemeInfo.AddMoneyDescription,
                TicketStatus = (int)schemeInfo.TicketStatus,
                ProgressStatus = (int)schemeInfo.ProgressStatus,
                BonusStatus = (int)schemeInfo.BonusStatus,
                Amount = schemeInfo.Amount,
                BetCount = schemeInfo.BetCount,
                SchemeSource = schemeInfo.SchemeSource,
                IssuseNumber = schemeInfo.IssuseNumber,
                WinNumber = winNumber,
                Security = schemeInfo.Security,
                Attach = schemeInfo.Attach,
                SchemeBettingCategory = (int)schemeInfo.SchemeBettingCategory,
                AfterTaxBonusMoney = schemeInfo.AfterTaxBonusMoney,
                IsPrizeMoney = schemeInfo.IsPrizeMoney,
                SchemeType = schemeInfo.SchemeType,
                IsVirtualOrder = schemeInfo.IsVirtualOrder,
                CodeList = codeList,
            };
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询订单详情成功",
                MsgId = entity.MsgId,
                Value = result,
            };
        }
        private async Task<List<object>> GetCodeList_GSAPP([FromServices]IServiceProxyProvider _serviceProxyProvider, Sports_AnteCodeQueryInfoCollection code, string gameCode, int amount)
        {
            var issuseNumber = code.List.Count > 0 ? code.List[0].IssuseNumber : string.Empty;
            var codeList = new List<object>();
            foreach (var item in code.List)
            {
                string[] array_GameCode = new string[] { "JCZQ", "JCLQ", "CTZQ", "BJDC" };
                var betCount = 0;
                if (gameCode.ToUpper() == "CTZQ")
                {
                    var result = Json_CTZQ.GetMatchListToOrderDetail(item.IssuseNumber, item.GameType, item.AnteCode);
                    if (result != null)
                    {
                        foreach (var _item in result)
                        {
                            codeList.Add(new
                            {
                                AnteCode = _item.AnteCode,
                                BonusStatus = (int)item.BonusStatus,
                                CurrentSp = item.CurrentSp,
                                FullResult = _item.FullResult,
                                GameType = item.GameType,
                                GuestTeamId = _item.GuestTeamId,
                                GuestTeamName = _item.GuestTeamName.Replace("nbsp;", ""),
                                HalfResult = _item.HalfResult,
                                HomeTeamId = _item.HomeTeamId,
                                HomeTeamName = _item.HomeTeamName,
                                IsDan = _item.IsDan,
                                IssuseNumber = _item.IssuseNumber,
                                LeagueColor = _item.LeagueColor,
                                LeagueName = _item.LeagueName,
                                LetBall = _item.LetBall,
                                MatchId = _item.MatchId,
                                MatchIdName = _item.MatchIdName,
                                MatchResult = _item.MatchResult,
                                MatchResultSp = item.MatchResultSp,
                                MatchState = _item.MatchState,
                                StartTime = _item.StartTime,
                                Amount = amount,
                                BetCount = betCount,
                                OrderNumber = _item.OrderNumber,
                                Detail_RF = AnalyticalCurrentSp(item.CurrentSp, "RF"),
                                Detail_YSZF = AnalyticalCurrentSp(item.CurrentSp, "YSZF"),
                            });
                        }
                    }
                }
                if (gameCode.ToUpper() == "SJB")
                {
                    Dictionary<string,object> param = new Dictionary<string, object>
                    {
                        {"gameCode",gameCode },{"gameType","" },{"issuseNumber",issuseNumber }
                    };
                    var issuseInfo =await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Order/QueryIssuseInfo");
                    if (!array_GameCode.Contains(gameCode))
                    {
                        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, item.GameType);
                        betCount = analyzer.AnalyzeAnteCode(item.AnteCode);
                    }

                    codeList.Add(new
                    {
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)item.BonusStatus,
                        CurrentSp = Json_JCZQ.GetAnteCode(item.GameType, item.AnteCode, item.CurrentSp),
                        FullResult = item.FullResult,
                        GameType = item.GameType,
                        GuestTeamId = item.GuestTeamId,
                        GuestTeamName = item.GuestTeamName,
                        HalfResult = item.HalfResult,
                        HomeTeamId = item.HomeTeamId,
                        HomeTeamName = item.HomeTeamName,
                        IsDan = item.IsDan,
                        IssuseNumber = item.IssuseNumber,
                        LeagueColor = item.LeagueColor,
                        //LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LetBall = item.LetBall,
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchResult = issuseInfo == null ? string.Empty : issuseInfo.WinNumber,
                        MatchResultSp = item.MatchResultSp,
                        MatchState = item.MatchState,
                        StartTime = ConvertHelper.ConvertDateTimeInt(Convert.ToDateTime(item.StartTime)),
                        Amount = amount,
                        BetCount = betCount,
                        Detail_RF = AnalyticalCurrentSp(item.CurrentSp, "RF"),
                        Detail_YSZF = AnalyticalCurrentSp(item.CurrentSp, "YSZF"),
                    });
                }
                else
                {
                    Dictionary<string, object> param = new Dictionary<string, object>
                    {
                        {"gameCode",gameCode },{"gameType","" },{"issuseNumber",issuseNumber }
                    };
                    //数字彩
                    var issuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Order/QueryIssuseInfo");
                    if (!array_GameCode.Contains(gameCode))
                    {
                        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, item.GameType);
                        betCount = analyzer.AnalyzeAnteCode(item.AnteCode);
                    }

                    codeList.Add(new
                    {
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)item.BonusStatus,
                        CurrentSp = item.CurrentSp,
                        FullResult = item.FullResult,
                        GameType = item.GameType,
                        GuestTeamId = item.GuestTeamId,
                        GuestTeamName = item.GuestTeamName,
                        HalfResult = item.HalfResult,
                        HomeTeamId = item.HomeTeamId,
                        HomeTeamName = item.HomeTeamName,
                        IsDan = item.IsDan,
                        IssuseNumber = item.IssuseNumber,
                        LeagueColor = item.LeagueColor,
                        //LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LetBall = item.LetBall,
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchResult = issuseInfo == null ? string.Empty : issuseInfo.WinNumber,
                        MatchResultSp = item.MatchResultSp,
                        MatchState = item.MatchState,
                        StartTime = ConvertHelper.ConvertDateTimeInt(Convert.ToDateTime(item.StartTime)),
                        Amount = amount,
                        BetCount = betCount,
                        Detail_RF = AnalyticalCurrentSp(item.CurrentSp, "RF"),
                        Detail_YSZF = AnalyticalCurrentSp(item.CurrentSp, "YSZF"),
                    });
                }
            }
            return codeList;
        }
        private string AnalyticalCurrentSp(string currentSp, string code)
        {
            if (string.IsNullOrEmpty(currentSp) || string.IsNullOrEmpty(code))
                return string.Empty;
            var array = currentSp.Split(',');
            if (array != null && array.Length > 0)
            {
                foreach (var item in array)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var tempArr = item.Split('|');
                        if (tempArr != null && tempArr.Length > 1)
                        {
                            if (code.ToUpper() == tempArr[0].ToUpper())
                                return (tempArr[1]);
                        }
                    }
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 查询定制跟单列表_150
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryAutofollowList([FromServices]IServiceProxyProvider _serviceProxyProvider,LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("您还未登录，请登录！");
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    {"gameCode",gameCode },{"gameType",gameType },{"pageIndex",pageIndex },{"pageSize",pageSize },{"userToken",userToken }
                };
                var followList =await _serviceProxyProvider.Invoke<TogetherFollowerRuleQueryInfoCollection>(param, "api/Order/QueryUserFollowRule");
                var list = new List<object>();
                if (followList != null && followList.TotalCount > 0)
                {
                    foreach (var item in followList.List)
                    {
                        list.Add(new
                        {
                            RuleId = item.RuleId,
                            BonusMoney = item.BonusMoney,
                            BuyMoney = item.BuyMoney,
                            CancelNoBonusSchemeCount = item.CancelNoBonusSchemeCount,
                            CancelWhenSurplusNotMatch = item.CancelWhenSurplusNotMatch,
                            CreaterUserId = item.CreaterUserId,
                            CreateTime = item.CreateTime,
                            FollowerCount = item.FollowerCount,
                            FollowerIndex = item.FollowerIndex,
                            FollowerPercent = item.FollowerPercent,
                            FollowerUserId = item.FollowerUserId,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            IsEnable = item.IsEnable,
                            MaxSchemeMoney = item.MaxSchemeMoney,
                            MinSchemeMoney = item.MinSchemeMoney,
                            SchemeCount = item.SchemeCount,
                            StopFollowerMinBalance = item.StopFollowerMinBalance,
                            UserId = item.UserId,
                            UserDisplayName = item.UserDisplayName,
                            HideDisplayNameCount = item.HideDisplayNameCount,
                        });
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询定制跟单列表成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询跟单信息_152
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryTogetherFollowerRule([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string createUserId = p.CreateUserId;
                string followerUserId = p.FollowerUserId;
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("您还未登录，请登录！");
                Dictionary<string, object> param = new Dictionary<string, object>() {
                    {"createrUserId",createUserId },{"followerUserId", followerUserId},{"gameCode",gameCode },{ "gameType",gameType}
                };
                var result =await _serviceProxyProvider.Invoke<TogetherFollowerRuleQueryInfo>(param, "api/Order/QueryTogetherFollowerRuleInfo");
                var list = new List<object>();
                if (result != null && !string.IsNullOrEmpty(result.CreaterUserId))
                {
                    list.Add(new
                    {
                        CancelNoBonusSchemeCount = result.CancelNoBonusSchemeCount,
                        CancelWhenSurplusNotMatch = result.CancelWhenSurplusNotMatch,
                        CreaterUserId = result.CreaterUserId,
                        FollowerCount = result.FollowerCount,
                        FollowerPercent = result.FollowerPercent,
                        FollowerUserId = result.FollowerUserId,
                        GameCode = result.GameCode,
                        GameType = result.GameType,
                        IsEnable = result.IsEnable,
                        MaxSchemeMoney = result.MaxSchemeMoney,
                        MinSchemeMoney = result.MinSchemeMoney,
                        SchemeCount = result.SchemeCount,
                        StopFollowerMinBalance = result.StopFollowerMinBalance,
                        UserDisplayName = result.UserDisplayName,
                        HideDisplayNameCount = result.HideDisplayNameCount,
                        RuleId = result.RuleId,
                    });
                }
                if (list != null && list.Count > 0)
                {
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询跟单信息成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    };
                }
                else
                {
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = "未查询到跟单信息",
                        MsgId = entity.MsgId,
                        Value = list,
                    };
                }
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询今日宝单_153
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryTodayBDFXList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userName = p.UserName;
                string strOrderBy = p.StrOrderBy;
                string currUserId = p.CurrentUserId;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    {"userId","" },{"userName",userName },{"gameCode","" },{"orderBy",strOrderBy },
                    { "desc",currUserId },{"startTime",DateTime.Now },{ "endTime",DateTime.Now},
                    { "isMyBD",string.Empty},{"pageIndex",pageIndex },{"pageSize",pageSize }
                };
                var todayBDList =await _serviceProxyProvider.Invoke<TotalSingleTreasure_Collection>(param, "api/Order/QueryTodayBDFXList");
                Dictionary<string, object> paranNR = new Dictionary<string, object>()
                {
                    { "startTime",DateTime.Now},{"endTime",DateTime.Now },{"count",3 }
                };
                var dayNR = await _serviceProxyProvider.Invoke<string>(paranNR, "api/Order/QueryYesterdayNR");
                List<object> list = new List<object>();
                if (todayBDList != null && todayBDList.TotalCount > 0)
                {
                    foreach (var item in todayBDList.TotalSingleTreasureList)
                    {
                        var currAnCodeList = todayBDList.AnteCodeList.Where(s => s.SchemeId == item.SchemeId).ToList();
                        list.Add(new
                        {
                            AnteCodeList = currAnCodeList,
                            AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                            BDFXCreateTime = item.BDFXCreateTime,
                            BetCount = item.BetCount,
                            Commission = item.Commission,
                            CurrentBetMoney = item.CurrentBetMoney,
                            CurrProfitRate = item.CurrProfitRate,
                            ExpectedBonusMoney = item.ExpectedBonusMoney,
                            ExpectedReturnRate = item.ExpectedReturnRate,
                            FirstMatchStopTime =ConvertHelper.ConvertDateTimeInt(item.FirstMatchStopTime),
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            IsComplate = item.IsComplate,
                            IssuseNumber = item.IssuseNumber,
                            LastMatchStopTime = ConvertHelper.ConvertDateTimeInt(item.LastMatchStopTime),
                            LastweekProfitRate = item.LastweekProfitRate,
                            ProfitRate = item.ProfitRate,
                            SchemeId = item.SchemeId,
                            Security = item.Security,
                            SingleTreasureDeclaration = item.SingleTreasureDeclaration,
                            TotalBonusMoney = item.TotalBonusMoney,
                            TotalBuyCount = item.TotalBuyCount,
                            TotalBuyMoney = item.TotalBuyMoney,
                            TotalMatchCount = item.TotalMatchCount,
                            UserId = item.UserId,
                            UserName = item.UserName,
                            StrNR = dayNR,
                        });
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询今日宝单成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询宝单作者首页_154
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> QueryBDFXAutherHomePage([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userId = p.UserId;
                string strAward = p.StrAward;//-1:查询可购买；1：查询中奖；0：查询未中奖；当为空时，查询全部
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("您还未登录，请先登录。");
                var result = new TotalSingleTreasure_Collection();
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "userId",userId},{"pageIndex",pageIndex },{"pageSize",pageSize }
                };
                if (!string.IsNullOrEmpty(strAward) && strAward == "-1")
                {
                    param.Add("strIsBonus", "");
                    param.Add("currentTime", DateTime.Now.ToString());
                    result = await _serviceProxyProvider.Invoke<TotalSingleTreasure_Collection>(param, "api/Order/QueryBDFXAutherHomePage");
                }
                else
                {
                    param.Add("strIsBonus", strAward);
                    param.Add("currentTime", "");
                    result = await _serviceProxyProvider.Invoke<TotalSingleTreasure_Collection>(param, "api/Order/QueryBDFXAutherHomePage");
                }
                List<object> list = new List<object>();
                if (result != null && result.TotalCount > 0)
                {
                    foreach (var item in result.TotalSingleTreasureList)
                    {
                        var currAnCodeList = result.AnteCodeList.Where(s => s.SchemeId == item.SchemeId).ToList();
                        list.Add(new
                        {
                            AnteCodeList = currAnCodeList,
                            UserId = item.UserId,
                            UserName = item.UserName,
                            SingleTreasureDeclaration = item.SingleTreasureDeclaration,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            IssuseNumber = item.IssuseNumber,
                            ExpectedReturnRate = item.ExpectedReturnRate,
                            Commission = item.Commission,
                            Security = item.Security,
                            TotalBuyCount = item.TotalBuyCount,
                            TotalBuyMoney = item.TotalBuyMoney,
                            AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                            FirstMatchStopTime = ConvertHelper.ConvertDateTimeInt(item.FirstMatchStopTime),
                            LastMatchStopTime = ConvertHelper.ConvertDateTimeInt(item.LastMatchStopTime),
                            ProfitRate = item.ProfitRate,
                            SchemeId = item.SchemeId,
                            TotalBonusMoney = item.TotalBuyMoney,
                            ExpectedBonusMoney = item.ExpectedBonusMoney,
                            BetCount = item.BetCount,
                            TotalMatchCount = item.TotalMatchCount,
                            IsComplate = item.IsComplate,
                            CurrentBetMoney = item.CurrentBetMoney,
                            CurrProfitRate = item.CurrProfitRate,
                            TotalCount = result.TotalCount,
                        });
                    }
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询宝单作者主页成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
    }
}
