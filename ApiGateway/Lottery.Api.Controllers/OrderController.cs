using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.LotteryJsonInfo;
using EntityModel.Redis;
using EntityModel.RequestModel;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Common.Utilities;
using Lottery.Api.Controllers.CommonFilterActtribute;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaSon.FrameWork.Common.ExceptionEx;
using System.Diagnostics;
using EntityModel.ExceptionExtend;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class OrderController : BaseController
    {
        /// <summary>
        /// 查询订单开奖历史记录_100
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult QueryOrderHistoryRecord(LotteryServiceRequest entity)
        {
            try
            {
                //读取json数据
                var param = JsonHelper.Decode(entity.Param);
                string gamecode = param.GameCode;
                //读取json文件
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var fileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}.json", gamecode));
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
                if (data != null && data.List != null && data.List.Count > 0)
                {
                    foreach (var item in data.List)
                    {
                        list.Add(new
                        {
                            item.IssuseNumber,
                            item.WinNumber,
                            CreateTime = item.CreateTime,
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单开奖历史记录成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询中奖列表_113
        /// </summary>
        public async Task<IActionResult> QueryBonusList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string GameCode = p.GameCode;
                string gameType = p.GameType;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string key = p.KeyWord;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("用户未登录");
                if (string.IsNullOrEmpty(GameCode))
                    throw new Exception("彩种不能为空");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["Model"] = new QueryBonusInfoListParam() { userId = userid, gameCode = GameCode, gameType = gameType, pageIndex = pageIndex, pageSize = pageSize, UserToken = userToken };
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
                return Json(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.ToGetMessage() + "●" + ex.ToString(), MsgId = entity.MsgId, Value = ex.ToGetMessage() });
            }
        }
        /// <summary>
        /// 查询彩种当前开奖信息_128
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult QueryGameCurrBonusInfo(LotteryServiceRequest entity)
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
                        data = JsonHelper.Deserialize<GameWinNumber_InfoCollection>(jsonData);
                    }
                }

                var list = new List<object>();
                if (data != null && data.List != null && data.List.Any())
                {
                    foreach (var item in data.List)
                    {
                        list.Add(new
                        {
                            item.Id,
                            item.WinNumber,
                            item.IssuseNumber,
                            item.GameType,
                            item.GameCode,
                            CreateTime = item.CreateTime,
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询最新开奖成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 竞彩查询开奖历史_129
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryJingCaiBonusHistory([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {

                var p = JsonHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string issuseNumber = p.IssuseNumber;
                string gameType = p.GameType;
                int pageIndex = p.PageIndex == null ? 0 : Convert.ToInt32(p.PageIndex);
                int pageSize = p.PageSize == null ? 0 : Convert.ToInt32(p.PageSize);
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "issuseNumber", issuseNumber },{ "pageIndex", pageIndex },{ "pageSize",pageSize }
                };
                string starttime = p.StartTime;
                bool Boolstarttime = string.IsNullOrEmpty(starttime);
                DateTime startTime = Boolstarttime ? DateTime.Now : Convert.ToDateTime(p.StartTime);
                string endtime = p.EndTime;
                bool boolendtime = string.IsNullOrEmpty(endtime);
                DateTime endTime = boolendtime ? DateTime.Now : Convert.ToDateTime(p.EndTime);
                if (string.IsNullOrEmpty(gameCode))
                    throw new LogicException("传入彩种不能为空");
                var list = new List<object>();
                if (gameCode.ToUpper() == "JCZQ")
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
                            result = JsonHelper.Deserialize<JCZQMatchResult_Collection>(jsonData);
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
                                StartTime = item.StartTime,
                                LeagueId = item.LeagueId,
                                LeagueName = item.LeagueName,
                                LeagueColor = item.LeagueColor,
                                HomeTeamId = item.HomeTeamId,
                                HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
                                GuestTeamId = item.GuestTeamId,
                                GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
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
                                CreateTime = item.CreateTime,
                            });
                        }
                    }
                }
                else if (gameCode.ToUpper() == "JCLQ")
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
                            result = JsonHelper.Deserialize<JCLQMatchResult_Collection>(jsonData);
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
                                StartTime = item.StartTime,
                                LeagueId = item.LeagueId,
                                LeagueName = item.LeagueName,
                                LeagueColor = item.LeagueColor,
                                HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
                                GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
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
                                CreateTime = item.CreateTime,
                            });
                        }
                    }
                }
                else if (gameCode.ToUpper() == "BJDC")
                {
                    if (string.IsNullOrEmpty(issuseNumber))
                        throw new LogicException("传入期号不能为空");
                    var result = await _serviceProxyProvider.Invoke<BJDCMatchResultInfo_Collection>(param, "api/Order/QueryBJDC_MatchResultCollection");
                    if (result != null && result.ListInfo.Count > 0)
                    {
                        foreach (var item in result.ListInfo)
                        {
                            list.Add(new
                            {
                                BF_Result = item.BF_Result == null ? "" : item.BF_Result,
                                BF_Name = ConvertHelper.ANTECODES_BJDC("bf", item.BF_Result),
                                BF_SP = item.BF_SP > 0 ? 0 : item.BF_SP,
                                BQC_Result = item.BQC_Result == null ? "" : item.BQC_Result,
                                BQC_Name = ConvertHelper.ANTECODES_BJDC("bqc", item.BQC_Result),
                                BQC_SP = item.BQC_SP > 0 ? 0 : item.BQC_SP,
                                CreateTime = item.CreateTime,
                                FlatOdds = item.FlatOdds > 0 ? 0 : item.FlatOdds,
                                GuestFull_Result = item.GuestFull_Result == null ? "" : item.GuestFull_Result,
                                GuestHalf_Result = item.GuestHalf_Result == null ? "" : item.GuestHalf_Result,
                                GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
                                HomeFull_Result = item.HomeFull_Result == null ? "" : item.HomeFull_Result,
                                HomeHalf_Result = item.HomeHalf_Result == null ? "" : item.HomeHalf_Result,
                                HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
                                Id = item.Id,
                                IssuseNumber = item.IssuseNumber,
                                LetBall = item.LetBall,
                                LoseOdds = item.LoseOdds,
                                MatchColor = item.MatchColor,
                                MatchName = item.MatchName,
                                MatchOrderId = item.MatchOrderId,
                                MatchStartTime = item.MatchStartTime,
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
                else if (gameCode.ToUpper() == "CTZQ")
                {
                    if (string.IsNullOrEmpty(gameType))
                        throw new LogicException("玩法不能为空");
                    else if (string.IsNullOrEmpty(issuseNumber))
                        throw new LogicException("期号不能为空");

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
                            result = JsonHelper.Deserialize<CTZQMatch_PoolInfo_Collection>(jsonData);
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
                            GuestTeamName = ConvertHelper.DeleteHtml(item.GuestTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
                            GuestTeamScore = item.GuestTeamScore,
                            GuestTeamStanding = item.GuestTeamStanding,
                            HomeTeamHalfScore = item.HomeTeamHalfScore,
                            HomeTeamId = item.HomeTeamId,
                            HomeTeamName = ConvertHelper.DeleteHtml(item.HomeTeamName).Replace("&nbsp;", "").Replace("nbsp;", ""),
                            HomeTeamScore = item.HomeTeamScore,
                            HomeTeamStanding = item.HomeTeamStanding,
                            Id = item.Id,
                            IssuseNumber = item.IssuseNumber,
                            MatchId = item.MatchId,
                            MatchName = item.MatchName,
                            MatchResult = item.MatchResult == null ? "" : item.MatchResult,
                            MatchStartTime = item.MatchStartTime,
                            Mid = item.Mid,
                            OrderNumber = item.OrderNumber,
                            UpdateTime = item.UpdateTime,
                            BonusTime = item.BounsTime.Year < 1900 ? null : item.BounsTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询开奖历史成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询账户明细_132
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryAccountDetial([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string viewType = p.ViewType;
                string userToken = p.UserToken;
                string starttime = p.StartTime;
                bool Boolstarttime = string.IsNullOrEmpty(starttime);
                string endtime = p.EndTime;
                bool Boolendtime = string.IsNullOrEmpty(endtime);
                DateTime startTime = Boolstarttime ? DateTime.Now : Convert.ToDateTime(p.StartTime);
                DateTime endTime = Boolendtime ? DateTime.Now.AddDays(1).Date : Convert.ToDateTime(p.EndTime);
                int days = p.Days;
                startTime = startTime.AddDays(-days).Date;
                int PageIndex = p.PageIndex ?? 0;
                int pageSize = p.PageSize ?? 1;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登陆");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                var list = new List<object>();
                if (viewType.ToUpper() == "ZHMX")
                {
                    string accountType = p.AccoountType;
                    if (string.IsNullOrEmpty(accountType))
                        accountType = string.Empty;
                    var Model = new QueryUserFundDetailParam() { viewtype = viewType, userid = userid, fromDate = startTime, toDate = endTime, pageIndex = PageIndex, pageSize = pageSize, accountTypeList = accountType };
                    param["Model"] = Model;
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
                                CreateTime = item.CreateTime,
                                TotalPayinCount = FundDetails.TotalPayinCount,
                                TotalPayinMoney = FundDetails.TotalPayinMoney,
                                TotalPayoutCount = FundDetails.TotalPayinMoney,
                                TotalPayoutMoney = FundDetails.TotalPayoutMoney,
                                SchemeAddress = ConvertHelper.GetDomain() + "/user/scheme/" + item.OrderId,
                            });
                        }
                    }
                }
                else if (viewType.ToUpper() == "CZJL")
                {
                    var Model = new QueryFillMoneyListParam() { userid = userid, startTime = startTime, endTime = endTime, pageIndex = PageIndex, pageSize = pageSize, statusList = "1" };
                    param["Model"] = Model;
                    var FillMoneyCollection = await _serviceProxyProvider.Invoke<FillMoneyQueryInfoCollection>(param, "api/Order/QueryMyFillMoneyList");
                    if (FillMoneyCollection != null && FillMoneyCollection.FillMoneyList.Count > 0)
                    {
                        foreach (var item in FillMoneyCollection.FillMoneyList)
                        {
                            list.Add(new
                            {
                                FillMoneyAgentType = item.FillMoneyAgent,
                                StrFillMoneyAgentType = ConvertHelper.GetFillMoneyAgentType(item.FillMoneyAgent),
                                OrderId = item.OrderId,
                                RequestTime = item.RequestTime,
                                ResponseTime = item.ResponseTime.Value,
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
                else if (viewType.ToUpper() == "GCJL")
                {
                    var Model = new QueryMyBettingOrderParam() { UserID = userid, startTime = startTime, endTime = endTime, pageIndex = PageIndex, pageSize = pageSize, bonusStatus = null, gameCode = "" };
                    param["Model"] = Model;
                    var result = await _serviceProxyProvider.Invoke<MyBettingOrderInfoCollection>(param, "api/Order/QueryMyBettingOrderList");
                    if (result != null && result.OrderList != null)
                    {
                        foreach (var item in result.OrderList)
                        {
                            list.Add(new
                            {
                                BuyTime = item.BuyTime,
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
                else if (viewType.ToUpper() == "ZJJL")
                {
                    var Model = new QueryUserFundDetailParam() { userid = userid, fromDate = startTime, toDate = endTime, pageIndex = PageIndex, pageSize = pageSize, categoryList = "奖金", accountTypeList = "10" };
                    param["Model"] = Model;
                    var result = await _serviceProxyProvider.Invoke<UserFundDetailCollection>(param, "api/Order/QueryMyFundDetailList");
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
                                CreateTime = item.CreateTime,
                                TotalPayinCount = result.TotalPayinCount,
                                TotalPayinMoney = result.TotalPayinMoney,
                                TotalPayoutCount = result.TotalPayoutCount,
                                TotalPayoutMoney = result.TotalPayoutMoney,
                            });
                        }
                    }
                }
                else if (viewType.ToUpper() == "TKJL")
                {
                    var Model = new QueryMyWithdrawParam() { userid = userid, startTime = startTime, endTime = endTime, pageIndex = PageIndex, pageSize = pageSize, status = (int)WithdrawStatus.Success };
                    param["Model"] = Model;
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
                                RequestTime = item.RequestTime.Value,
                                ResponseTime = item.ResponseTime.Value,
                                ResponseMoney = item.ResponseMoney,
                                WithdrawAgent = item.WithdrawAgent,
                                StrWithdrawAgent = ConvertHelper.WithdrawAgentTypeName((WithdrawAgentType)item.WithdrawAgent),
                                Status = item.Status,
                                StrStatus = ConvertHelper.GetWithdrawStatus((WithdrawStatus)item.Status),
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询账户明细成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 根据中奖状态查询投注记录_133
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryOrderListByBonusState([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userId = p.UserId;
                int state = p.bonusStatus;
                //int schemeType = p.SchemeType;
                int days = p.ViewDay;
                DateTime startTime = Convert.ToDateTime("2015-01-01 00:00:00");
                DateTime endTime = DateTime.Now.AddDays(1).Date;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string userToken = p.UserToken;
                int orderQueryType = p.OrderQueryType;
                string gamecode = p.GameCode;
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登陆，请登陆后查询");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var list = new List<object>();
                if (orderQueryType == 1)
                {
                    var Model = new QueryMyBettingOrderParam() { UserID = userid, bonusStatus = (BonusStatus)state, gameCode = gamecode ?? "", startTime = startTime, endTime = endTime, pageIndex = pageIndex, pageSize = pageSize };
                    param["Model"] = Model;
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
                                GameType = item.GameType,
                                GameTypeName = item.GameTypeName,
                                CreatorDisplayName = item.CreatorDisplayName,
                                IssuseNumber = item.IssuseNumber,
                                CurrentBettingMoney = item.BuyMoney,
                                BetTime = item.BetTime,
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
                                CreateTime = item.BuyTime
                            });
                        }
                    }
                }
                if (orderQueryType == 2)
                {
                    var Model = new QueryCreateTogetherOrderParam() { userId = userId, startTime = startTime, endTime = endTime, gameCode = gamecode, pageIndex = pageIndex, pageSize = pageSize, bonus = (BonusStatus)state };
                    param["Model"] = Model;
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
                            BetTime = item.CreateTime,
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
                            CreateTime = item.CreateTime
                        });
                    }
                }
                if (orderQueryType == 3)
                {
                    var Model = new QueryCreateTogetherOrderParam() { pageIndex = pageIndex, pageSize = pageSize, gameCode = gamecode, userId = userId, startTime = startTime, endTime = endTime, bonus = (BonusStatus)state };
                    param["Model"] = Model;
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
                            BetTime = item.CreateTime,
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
                            CreateTime = item.CreateTime
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询投注记录成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }

        }
        /// <summary>
        /// 查询合买跟单_137
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QuerySportsTogetherList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = p.GameCode;
                var gameType = p.GameType;
                var pageNo = p.Pageindex;
                var PageSize = p.PageSize;
                var orderBy = p.orderBy;
                var sortType = p.sortType;
                string userToken = p.UserToken;
                Dictionary<string, object> param = new Dictionary<string, object>();
                string userId = string.Empty;
                if (!string.IsNullOrEmpty(userToken))
                {
                    userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                    //param.Add("userToken", userToken);
                    //var userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");
                    //if (userInfo != null)
                    //    userId = userInfo.UserId;
                }

                if (orderBy == "Progress")
                    orderBy = "ManYuan desc, Progress " + sortType + ",TotalMoney DESC,ISTOP DESC";
                else if (orderBy == "TotalMoney")
                    orderBy = "ManYuan desc,TotalMoney " + sortType + ", Progress DESC,ISTOP DESC";

                var list = new List<object>();
                param.Clear();
                var Model = new QuerySportsTogetherListFromRedisParam() { security = null, betCategory = null, progressState = null, gameCode = gameCode, gameType = gameType, issuseNumber = "", minMoney = -1, maxMoney = -1, minProgress = -1, maxProgress = -1 };
                param["Model"] = Model;
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
                            StopTime = item.StopTime,
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询合买大厅成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询中奖榜单_138
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>

        public async Task<IActionResult> QueryRankReport([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询中奖榜单成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 新接口_查询订单详情_144
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryNewOrderDetailBySchemeId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;

                string schemeId = p.SchemeId;
                if (string.IsNullOrEmpty(schemeId))
                    throw new LogicException("订单号不能为空！");

                if (schemeId.StartsWith("CHASE"))
                {
                    if (string.IsNullOrEmpty(userToken))
                        throw new LogicException("您还未登录！");
                    return await QueryCHASEOrderDetail(_serviceProxyProvider, entity, schemeId, userToken);
                }
                else if (schemeId.StartsWith("TSM"))
                {
                    return await QueryTMSOrderDetail(_serviceProxyProvider, entity, schemeId, userToken);
                }
                else
                {
                    if (string.IsNullOrEmpty(userToken))
                        throw new LogicException("您还未登录！");
                    return await QueryGeneralOrderDetail(_serviceProxyProvider, entity, schemeId, userToken);
                }
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
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
        public async Task<IActionResult> QueryCHASEOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity, string schemeId, string userToken = "")
        {
            try
            {
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "schemeId", schemeId }
            };
                var schemeInfo = await _serviceProxyProvider.Invoke<BettingOrderInfoCollection>(param, "api/Order/QueryBettingOrderListByChaseKeyLine");
                if (schemeInfo.OrderList.Count == 0)
                    throw new LogicException("追号方案不包括投注期信息");
                var firstIssuse = schemeInfo.OrderList[0];
                //var userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");

                var status = schemeInfo.OrderList.Min(t => t.ProgressStatus);
                var codeList = new List<object>();
                if (firstIssuse.Security == TogetherSchemeSecurity.Public
                    || (firstIssuse.Security == TogetherSchemeSecurity.CompletePublic && status != ProgressStatus.Running && status != ProgressStatus.Waitting) || (firstIssuse.UserId == userid))
                {
                    param.Clear();
                    param.Add("SchemeId", firstIssuse.SchemeId);
                    var anteCodeList = await _serviceProxyProvider.Invoke<BettingAnteCodeInfoCollection>(param, "api/Order/QueryAnteCodeListBySchemeId");
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
                    CreateTime = firstIssuse.CreateTime,
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

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单明细成功",
                    MsgId = entity.MsgId,
                    Value = result,
                });
            }
            catch (Exception ex)
            {

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }

        }
        /// <summary>
        /// 查看合买订单详情
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryTMSOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity, string schemeId, string userToken = "")
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>
            {
                {"schemeId",schemeId }
            };
                var schemeInfo = await _serviceProxyProvider.Invoke<Sports_TogetherSchemeQueryInfo>(param, "api/Order/QuerySportsTogetherDetail");
                param.Clear();
                string userid = string.Empty;
                //var userInfo = new LoginInfo();
                var flag = false;
                if (!string.IsNullOrEmpty(userToken))
                {
                    //param.Add("userToken", userToken);
                    //userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");
                    userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                    param.Add("schemeId", schemeId);
                    param.Add("userId", userid);
                    flag = await _serviceProxyProvider.Invoke<bool>(param, "api/Order/IsUserJoinSportsTogether");
                }

                var codeList = new List<object>();
                if (schemeInfo.Security == TogetherSchemeSecurity.Public
                   || (schemeInfo.Security == TogetherSchemeSecurity.CompletePublic && schemeInfo.StopTime <= DateTime.Now)
                   || schemeInfo.CreateUserId == userid
                    || (schemeInfo.Security == TogetherSchemeSecurity.JoinPublic && flag))
                {
                    var anteCodeList = await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                    //codeList = GetCodeList(anteCodeList);
                    codeList = await GetCodeList_GSAPP(_serviceProxyProvider, anteCodeList, schemeInfo.GameCode, schemeInfo.Amount);

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
                    //SurplusCount = schemeInfo.SurplusCount,
                    SoldCount = schemeInfo.SoldCount,
                    StopTime = schemeInfo.StopTime,
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
                    CreateTime = schemeInfo.CreateTime,
                    HitMatchCount = schemeInfo.HitMatchCount,
                    BonusCount = schemeInfo.BonusCount,
                    BonusMoney = schemeInfo.AfterTaxBonusMoney,
                    AddMoney = schemeInfo.AddMoney,
                    JoinUserCount = schemeInfo.JoinUserCount,
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
                    //JoinList = joinList,
                    ServiceTime = DateTime.Now,
                };

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单明细成功",
                    MsgId = entity.MsgId,
                    Value = result,
                });
            }
            catch (Exception ex)
            {

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }

        }
        /// <summary>
        /// 查询普通订单详情
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryGeneralOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity, string schemeId, string userToken = "")
        {
            try
            {


                Dictionary<string, object> param = new Dictionary<string, object>
            {
                {"schemeId",schemeId }
            };
                var schemeInfo = await _serviceProxyProvider.Invoke<Sports_SchemeQueryInfo>(param, "api/Order/QuerySportsSchemeInfo");
                string userId = string.Empty;
                param.Clear();
                //var userInfo = new LoginInfo();
                if (!string.IsNullOrEmpty(userToken))
                {
                    userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                    //param.Add("userToken", userToken);
                    //userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");
                }
                param.Clear();
                //|| string.IsNullOrEmpty(userToken)
                var codeList = new List<object>();
                if (schemeInfo.Security == TogetherSchemeSecurity.Public
                   || (schemeInfo.Security == TogetherSchemeSecurity.CompletePublic && schemeInfo.StopTime <= DateTime.Now)
                   || schemeInfo.UserId == userId)
                {
                    param["schemeId"] = schemeId;
                    if (schemeInfo.Security != TogetherSchemeSecurity.FirstMatchStopPublic)
                    {
                        var anteCodeList = await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                        codeList = await GetCodeList_GSAPP(_serviceProxyProvider, anteCodeList, schemeInfo.GameCode, schemeInfo.Amount);
                    }
                    else if ((schemeInfo.Security == TogetherSchemeSecurity.FirstMatchStopPublic && schemeInfo.StopTime <= DateTime.Now))
                    {
                        var anteCodeList = await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                        codeList = await GetCodeList_GSAPP(_serviceProxyProvider, anteCodeList, schemeInfo.GameCode, schemeInfo.Amount);
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
                    var IssuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(Issuse_param, "api/Order/QueryIssuseInfo");
                    if (IssuseInfo != null)
                        winNumber = IssuseInfo.WinNumber;
                }
                //if (schemeInfo.GameCode.ToUpper() == "SJB")
                //{

                //}
                var GameTypeDisplayName = string.IsNullOrEmpty(schemeInfo.GameTypeDisplayName) ? "" : schemeInfo.GameTypeDisplayName
                    .Replace("胜负任9", "任选9")
                    .Replace("14场胜负", "胜负14场")
                    .Replace("6场半全场", "六场半全场")
                    .Replace("4场进球", "四场进球彩");
                var result = new
                {
                    SchemeId = schemeInfo.SchemeId,
                    GameCode = schemeInfo.GameCode,
                    GameDisplayName = schemeInfo.GameDisplayName,
                    GameType = schemeInfo.GameType,
                    GameTypeDisplayName = GameTypeDisplayName,
                    GameName = ConvertHelper.GameName(schemeInfo.GameCode, schemeInfo.GameType),
                    UserId = schemeInfo.UserId,
                    UserDisplayName = ConvertHelper.HideUserName(schemeInfo.UserDisplayName, schemeInfo.HideDisplayNameCount),
                    HideDisplayNameCount = schemeInfo.HideDisplayNameCount,
                    MatchCount = schemeInfo.TotalMatchCount,
                    PlayType = schemeInfo.PlayType,
                    CreateTime = schemeInfo.CreateTime,
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单详情成功",
                    MsgId = entity.MsgId,
                    Value = result,
                });
            }
            catch (Exception ex)
            {

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        public async Task<List<object>> GetCodeList_GSAPP([FromServices]IServiceProxyProvider _serviceProxyProvider, Sports_AnteCodeQueryInfoCollection code, string gameCode, int amount, bool IsChase = false)
        {

            var issuseNumber = code.List.Count > 0 ? code.List[0].IssuseNumber : string.Empty;
            var codeList = new List<object>();
            var issuseInfoList = new List<Issuse_QueryInfo>();
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
                                GameTypeDisplayName = string.IsNullOrEmpty(item.GameType) ? "" : BettingHelper.FormatGameType(gameCode, item.GameType),
                                GameType = item.GameType,
                                GuestTeamId = _item.GuestTeamId,
                                GuestTeamName = _item.GuestTeamName.Replace("&nbsp;", "").Replace("nbsp;", ""),
                                HalfResult = _item.HalfResult,
                                HomeTeamId = _item.HomeTeamId,
                                HomeTeamName = _item.HomeTeamName.Replace("&nbsp;", "").Replace("nbsp;", ""),
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
                    Dictionary<string, object> param = new Dictionary<string, object>
                    {
                        {"gameCode",gameCode },{"gameType","" },{"issuseNumber",issuseNumber }
                    };
                    //var issuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Order/QueryIssuseInfo");
                    var issuseInfo = new Issuse_QueryInfo();
                    if (issuseInfoList.Exists(p => p.IssuseNumber == issuseNumber)) issuseInfo = issuseInfoList.FirstOrDefault(p => p.IssuseNumber == issuseNumber);
                    else
                    {
                        issuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Order/QueryIssuseInfo");
                        issuseInfoList.Add(issuseInfo);
                    }
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
                        GameTypeDisplayName = BettingHelper.FormatGameType(gameCode, item.GameType),
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
                        StartTime = Convert.ToDateTime(item.StartTime),
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
                    //var issuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Order/QueryIssuseInfo");
                    var issuseInfo = new Issuse_QueryInfo();
                    if (IsChase)
                    {
                        if (issuseInfoList.Exists(p => p.IssuseNumber == issuseNumber)) issuseInfo = issuseInfoList.FirstOrDefault(p => p.IssuseNumber == issuseNumber);
                        else
                        {
                            issuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Order/QueryIssuseInfo");
                            issuseInfoList.Add(issuseInfo);
                        }
                    }
                    if (!array_GameCode.Contains(gameCode))
                    {
                        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, item.GameType);
                        betCount = analyzer.AnalyzeAnteCode(item.AnteCode);
                    }

                    codeList.Add(new
                    {
                        AnteCode = item.AnteCode,
                        AnteCodeDetail = BettingHelper.BackToDetailByAnteCode(gameCode.ToUpper(), item.GameType, item.AnteCode, item.CurrentSp),
                        BonusStatus = (int)item.BonusStatus,
                        CurrentSp = item.CurrentSp,
                        FullResult = item.FullResult,
                        GameTypeDisplayName = BettingHelper.FormatGameType(gameCode, item.GameType),
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
                        StartTime = Convert.ToDateTime(item.StartTime),
                        Amount = amount,
                        BetCount = betCount,
                        Detail_RF = AnalyticalCurrentSp(item.CurrentSp, "RF"),
                        Detail_YSZF = AnalyticalCurrentSp(item.CurrentSp, "YSZF"),
                    });
                }
            }
            return codeList;


        }

        /// <summary>
        /// 查询定制跟单列表_150
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryAutofollowList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string userToken = p.UserToken;
                bool byFollower = p.byFollower;//true=查询我的定制,false=查询定制我的
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录，请登录！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                var Model = new QueryUserFollowRuleParam() { userId = userId, pageIndex = pageIndex, pageSize = pageSize, gameCode = gameCode, gameType = gameType, byFollower = byFollower };
                param["Model"] = Model;
                var followList = await _serviceProxyProvider.Invoke<TogetherFollowerRuleQueryInfoCollection>(param, "api/Order/QueryUserFollowRule");
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询定制跟单列表成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询定制跟单列表出错" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询跟单信息_152
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryTogetherFollowerRule([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string createUserId = p.CreateUserId;
                string followerUserId = p.FollowerUserId;
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录，请登录！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>() {
                    {"createrUserId",createUserId },{"followerUserId", followerUserId},{"gameCode",gameCode },{ "gameType",gameType}
                };
                var result = await _serviceProxyProvider.Invoke<TogetherFollowerRuleQueryInfo>(param, "api/Order/QueryTogetherFollowerRuleInfo");
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
                //if (list != null && list.Count > 0)
                //{
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询跟单信息成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
                //}
                //else
                //{
                //    return Json(new LotteryServiceResponse
                //    {
                //        Code = ResponseCode.失败,
                //        Message = "未查询到跟单信息" + "●" + ex.ToString(),
                //        MsgId = entity.MsgId,
                //        Value = list,
                //    });
                //}
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询跟单信息失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询今日宝单_153
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryTodayBDFXList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userName = p.UserName;
                string strOrderBy = p.StrOrderBy;
                string currUserId = p.CurrentUserId;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string UserId = p.UserId;
                string GameCode = p.GameCode;
                string isMyBD = p.isMyBD;
                Dictionary<string, object> param = new Dictionary<string, object>();
                var Model = new QueryTodayBDFXList() { isMyBD = isMyBD, userName = userName, gameCode = GameCode, userId = UserId, strOrderBy = strOrderBy, startTime = DateTime.Now, endTime = DateTime.Now, currentUserId = currUserId, pageIndex = pageIndex, pageSize = pageSize, };
                param["Model"] = Model;
                var todayBDList = await _serviceProxyProvider.Invoke<TotalSingleTreasure_Collection>(param, "api/Order/QueryTodayBDFXList");
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
                            FirstMatchStopTime = item.FirstMatchStopTime,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            IsComplate = item.IsComplate,
                            IssuseNumber = item.IssuseNumber,
                            LastMatchStopTime = item.LastMatchStopTime,
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询今日宝单成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询今日宝单失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询宝单作者首页_154
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryBDFXAutherHomePage([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userId = p.UserId;
                string strAward = p.StrAward;//-1:查询可购买；1：查询中奖；0：查询未中奖；当为空时，查询全部
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                if (string.IsNullOrEmpty(userId))
                    throw new LogicException("您还未登录，请先登录。");
                var result = new TotalSingleTreasure_Collection();
                string currenttime = string.Empty;
                if (!string.IsNullOrEmpty(strAward) && strAward == "-1")
                {
                    currenttime = DateTime.Now.ToString();
                    strAward = string.Empty;
                }
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "userId",userId},{"pageIndex",pageIndex },{"pageSize",pageSize },{"strIsBonus", strAward},{"currentTime", currenttime }
                };
                result = await _serviceProxyProvider.Invoke<TotalSingleTreasure_Collection>(param, "api/Order/QueryBDFXAutherHomePage");
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
                            FirstMatchStopTime = item.FirstMatchStopTime,
                            LastMatchStopTime = item.LastMatchStopTime,
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
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询宝单作者主页成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询保单作者首页失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询关注(关注总数、被关注总数、晒单总数等)_155
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryConcernedByUserId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userId = p.UserId;
                string bdfxUserId = p.BDFXUserId;
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "bdfxUserId", bdfxUserId },{"currUserId",string.IsNullOrEmpty(userId) ? "" : userId },{"startTime","" },{"endTime","" }
                };
                var result = await _serviceProxyProvider.Invoke<ConcernedInfo>(param, "api/Order/QueryConcernedByUserId");
                List<object> list = new List<object>();
                if (result != null)
                {
                    list.Add(new
                    {
                        BeConcernedUserCount = result.BeConcernedUserCount,
                        ConcernedUserCount = result.ConcernedUserCount,
                        IsGZ = result.IsGZ,
                        NearTimeProfitRateCollection = result.NearTimeProfitRateCollection,
                        RankNumber = result.RankNumber,
                        SingleTreasureCount = result.SingleTreasureCount,
                        UserId = result.UserId,
                        UserName = result.UserName,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 宝单分享_关注和取消关注_156
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> BDFXAttentionAndCancel([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string gzType = p.GZType;//1:关注；0:取消关注
                string userToken = p.UserToken;
                string currentUserId = p.CurrentUserId;
                string bgzUserId = p.BGZUserId;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录,请先登录!");
                else if (string.IsNullOrEmpty(gzType))
                    throw new LogicException("关注类型错误");
                else if (string.IsNullOrEmpty(currentUserId))
                    throw new LogicException("用户编号不能为空！");
                else if (string.IsNullOrEmpty(bgzUserId))
                    throw new LogicException("被关注用户编号不能为空！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "currUserId",currentUserId},{"bgzUserId",bgzUserId }
                };
                if (gzType == "1")//关注
                {
                    var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Order/BDFXAttention");
                    return Json(new LotteryServiceResponse
                    {
                        Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                        Message = result.Message,
                        MsgId = entity.MsgId,
                        Value = result.Message,
                    });
                }
                else if (gzType == "0")//取消关注
                {
                    var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Order/BDFXCancelAttention");
                    return Json(new LotteryServiceResponse
                    {
                        Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                        Message = result.Message,
                        MsgId = entity.MsgId,
                        Value = result.Message,
                    });
                }
                throw new LogicException("传入关注类型错误");
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 宝单分享_高手排行_157
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryGSRankList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string startWeek = p.StartWeek;
                string endWeek = p.EndWeek;
                string currentUserId = p.CurrentUserId;
                string myGZ = p.MYGZ;//是否为我的关注；"传值为true:查询我的关注,传值为空，查询高手排行"
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    {"startTime",startWeek },{"endTime",endWeek },{"currUserId",currentUserId },{"isMyGZ",myGZ }
                };
                var result = await _serviceProxyProvider.Invoke<BDFXGSRank_Collection>(param, "api/Order/QueryGSRankList");
                List<object> list = new List<object>();
                if (result != null)
                {
                    foreach (var item in result.RankList)
                    {
                        list.Add(new
                        {
                            BeConcernedUserCount = item.BeConcernedUserCount,
                            IsGZ = item.IsGZ,
                            CurrProfitRate = item.CurrProfitRate,
                            RankNumber = item.RankNumber,
                            SchemeId = item.SchemeId,
                            SingleTreasureCount = item.SingleTreasureCount,
                            UserId = item.UserId,
                            UserName = item.UserName,
                            LastweekRank = item.LastweekRank,
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询我的宝单_158
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryMyBDFXList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string currUserId = p.CurrentUserId;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string GameCode = p.GameCode;
                string userName = p.userName;
                string isMyBD = p.isMyBD;
                if (string.IsNullOrEmpty(currUserId))
                    throw new LogicException("您还未登录，请先登录。");
                Dictionary<string, object> param = new Dictionary<string, object>();

                var Model = new QueryTodayBDFXList() { isMyBD = isMyBD, userName = userName, gameCode = GameCode, strOrderBy = "", startTime = DateTime.Parse("2015-06-06"), endTime = DateTime.Now, currentUserId = currUserId, pageIndex = pageIndex, pageSize = pageSize };
                param["Model"] = Model;
                var myBDFXList = await _serviceProxyProvider.Invoke<TotalSingleTreasure_Collection>(param, "api/Order/QueryTodayBDFXList");
                List<object> list = new List<object>();
                if (myBDFXList != null && myBDFXList.TotalCount > 0)
                {
                    foreach (var item in myBDFXList.TotalSingleTreasureList)
                    {
                        var currAnteCodeList = myBDFXList.AnteCodeList.Where(s => s.SchemeId == item.SchemeId).ToList();
                        list.Add(new
                        {
                            AnteCodeList = currAnteCodeList,
                            AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                            BDFXCreateTime = item.BDFXCreateTime,
                            BetCount = item.BetCount,
                            Commission = item.Commission,
                            CurrentBetMoney = item.CurrentBetMoney,
                            CurrProfitRate = item.CurrProfitRate,
                            ExpectedBonusMoney = item.ExpectedBonusMoney,
                            ExpectedReturnRate = item.ExpectedReturnRate,
                            FirstMatchStopTime = item.FirstMatchStopTime,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            IsComplate = item.IsComplate,
                            IssuseNumber = item.IssuseNumber,
                            LastMatchStopTime = item.LastMatchStopTime,
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
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询我的关注成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询宝单详情_160
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryBDFXOrderDetailBySchemeId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string schemeId = p.SchemeId;
                if (string.IsNullOrEmpty(schemeId))
                    throw new LogicException("订单号不能为空");
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    {"schemeId",schemeId }
                };
                var result = await _serviceProxyProvider.Invoke<BDFXOrderDetailInfo>(param, "api/Order/QueryBDFXOrderDetailBySchemeId");
                List<object> list = new List<object>();
                if (result != null)
                {
                    list.Add(new
                    {
                        AfterTaxBonusMoney = result.AfterTaxBonusMoney,
                        Amount = result.Amount,
                        AnteCodeCollection = result.AnteCodeCollection,
                        AnteCodeList = result.AnteCodeList,
                        BetCount = result.BetCount,
                        Commission = result.Commission,
                        CurrentBetMoney = result.CurrentBetMoney,
                        CurrProfitRate = result.CurrProfitRate,
                        ExpectedBonusMoney = result.ExpectedBonusMoney,
                        ExpectedReturnRate = result.ExpectedReturnRate,
                        FirstMatchStopTime = result.FirstMatchStopTime,
                        GameCode = result.GameCode,
                        GameType = result.GameType,
                        IsComplate = result.IsComplate,
                        IssuseNumber = result.IssuseNumber,
                        LastMatchStopTime = result.LastMatchStopTime,
                        NearTimeProfitRateCollection = result.NearTimeProfitRateCollection,
                        PlayType = result.PlayType,
                        ProfitRate = result.ProfitRate,
                        RankNumber = result.RankNumber,
                        SchemeBettingCategory = result.SchemeBettingCategory,
                        SchemeId = result.SchemeId,
                        Security = result.Security,
                        SingleTreasureDeclaration = result.SingleTreasureDeclaration,
                        TicketStatus = result.TicketStatus,
                        TotalBonusMoney = result.TotalBonusMoney,
                        TotalBuyCount = result.TotalBuyCount,
                        TotalBuyMoney = result.TotalBuyMoney,
                        TotalMatchCount = result.TotalMatchCount,
                        UserId = result.UserId,
                        UserName = result.UserName,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询宝单详情成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询用户订单记录，包括分类数据（2017）_200
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryUserOrderList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                BonusStatus? bonusStatus = null;
                SchemeType? schemeType = null;
                DateTime startTime = Convert.ToDateTime(p.StartTime);
                DateTime endTime = Convert.ToDateTime(p.EndTime);
                int pageIndex = Convert.ToInt32(p.PageIndex);
                int pageSize = Convert.ToInt32(p.PageSize);
                int ProgressStatusType = 0;
                if (p.ProgressStatusType != null)
                {
                    ProgressStatusType = Convert.ToInt32(p.ProgressStatusType);
                }
                string _gameCode = p.GameCode;
                string _bonusStatus = p.BonusStatus;
                if (!string.IsNullOrEmpty(_bonusStatus))
                {
                    bonusStatus = (BonusStatus?)int.Parse(_bonusStatus);
                }
                string _schemeType = p.SchemeType;
                if (!string.IsNullOrEmpty(_schemeType))
                {
                    schemeType = (SchemeType?)int.Parse(_schemeType);
                }
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    {"gameCode",_gameCode },
                    { "startTime",startTime},
                    { "endTime",endTime},
                    { "pageIndex",pageIndex},
                    { "pageSize",pageSize },
                    { "userId",userId },
                    { "ProgressStatusType",ProgressStatusType }
                };
                if (_schemeType == "2")
                {
                    var result = await _serviceProxyProvider.Invoke<BettingOrderInfoCollection>(param, "api/Order/QueryMyChaseOrderList");
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询投注记录成功",
                        MsgId = entity.MsgId,
                        Value = result,
                    });
                }
                else
                {
                    param.Clear();
                    var Model = new QueryMyOrderListInfoParam() { userId = userId, pageIndex = pageIndex, pageSize = pageSize, gameCode = _gameCode, bonusStatus = bonusStatus, schemeType = schemeType, startTime = startTime, endTime = endTime };
                    param["Model"] = Model;
                    var result = await _serviceProxyProvider.Invoke<MyOrderListInfoCollection>(param, "api/Order/QueryMyOrderListInfo");
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询投注记录成功",
                        MsgId = entity.MsgId,
                        Value = result,
                    });
                }
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询订单详细数据，包括普通、合买、追号(2017)_201
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryUserOrderDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录！");
                string schemeId = p.SchemeId;
                if (string.IsNullOrEmpty(schemeId))
                    throw new LogicException("订单号不能为空！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                MyOrderListInfo orderInfo = null;
                Sports_AnteCodeQueryInfoCollection anteCodeList = null;
                var togetherInfo = new object();
                var joinTogetherUserList = new object();
                if (schemeId.StartsWith("CHASE"))
                {
                    //追号
                    Dictionary<string, object> param = new Dictionary<string, object>()
                    {
                    {"keyLine",schemeId }
                    };
                    var schemeInfo = await _serviceProxyProvider.Invoke<BettingOrderInfoCollection>(param, "api/Order/QueryBettingOrderListByChaseKeyLine");
                    if (schemeInfo.OrderList.Count == 0)
                        throw new LogicException("追号方案不包括投注期信息");
                    var firstIssuse = schemeInfo.OrderList[0];
                    param.Add("schemeId", firstIssuse.SchemeId);
                    orderInfo = await _serviceProxyProvider.Invoke<MyOrderListInfo>(param, "api/Order/QueryMyOrderDetailInfo");
                    anteCodeList = await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                    var codeList = await GetCodeList_GSAPP(_serviceProxyProvider, anteCodeList, orderInfo.GameCode, orderInfo.Amount, true);

                    var result = new
                    {
                        SchemeId = schemeId,
                        GameCode = orderInfo.GameCode,
                        GameTypeName = orderInfo.GameTypeName,
                        SchemeType = orderInfo.SchemeType,
                        SchemeSource = orderInfo.SchemeSource,
                        SchemeBettingCategory = orderInfo.SchemeBettingCategory,
                        TotalMoney = orderInfo.TotalMoney,
                        Amount = orderInfo.Amount,
                        ProgressStatus = orderInfo.ProgressStatus,
                        TicketStatus = orderInfo.TicketStatus,
                        IssuseNumber = orderInfo.IssuseNumber,
                        BonusStatus = orderInfo.BonusStatus,
                        PreTaxBonusMoney = schemeInfo.TotalPreTaxBonusMoney,
                        AfterTaxBonusMoney = schemeInfo.TotalAfterTaxBonusMoney,
                        BetTime = orderInfo.BetTime,
                        GameType = orderInfo.GameType,
                        AddMoney = schemeInfo.TotalAddMoney,
                        RedBagAwardsMoney = schemeInfo.TotalRedBagAwardsMoney,
                        BonusAwardsMoney = schemeInfo.TotalBonusAwardsMoney,
                        StopAfterBonus = orderInfo.StopAfterBonus,

                        CodeList = codeList,
                        TogetherInfo = togetherInfo,
                        JoinTogetherUserList = joinTogetherUserList,
                        IssuseList = schemeInfo.OrderList
                    };
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询订单明细成功",
                        MsgId = entity.MsgId,
                        Value = result,
                    });
                }
                else
                {
                    Dictionary<string, object> param = new Dictionary<string, object>()
                    {
                        {"schemeId",schemeId },{ "userId",userId}
                    };
                    orderInfo = await _serviceProxyProvider.Invoke<MyOrderListInfo>(param, "api/Order/QueryMyOrderDetailInfo");
                    anteCodeList = await _serviceProxyProvider.Invoke<Sports_AnteCodeQueryInfoCollection>(param, "api/Order/QuerySportsOrderAnteCodeList");
                    if (schemeId.StartsWith("TSM"))
                    {
                        //合买
                        param.Add("pageIndex", 0);
                        param.Add("pageSize", 100);
                        togetherInfo = await _serviceProxyProvider.Invoke<Sports_TogetherSchemeQueryInfo>(param, "api/Order/QuerySportsTogetherDetail");
                        joinTogetherUserList = await _serviceProxyProvider.Invoke<Sports_TogetherJoinInfoCollection>(param, "api/Order/QuerySportsTogetherJoinList");
                    }
                    else
                    {
                        //普通
                    }

                    var codeList = await GetCodeList_GSAPP(_serviceProxyProvider, anteCodeList, orderInfo.GameCode, orderInfo.Amount);
                    var result = new
                    {
                        SchemeId = orderInfo.SchemeId,
                        GameCode = orderInfo.GameCode,
                        GameTypeName = orderInfo.GameTypeName,
                        SchemeType = orderInfo.SchemeType,
                        SchemeSource = orderInfo.SchemeSource,
                        SchemeBettingCategory = orderInfo.SchemeBettingCategory,
                        TotalMoney = orderInfo.TotalMoney,
                        Amount = orderInfo.Amount,
                        ProgressStatus = orderInfo.ProgressStatus,
                        TicketStatus = orderInfo.TicketStatus,
                        IssuseNumber = orderInfo.IssuseNumber,
                        BonusStatus = orderInfo.BonusStatus,
                        PreTaxBonusMoney = orderInfo.PreTaxBonusMoney,
                        AfterTaxBonusMoney = orderInfo.AfterTaxBonusMoney,
                        BetTime = orderInfo.BetTime,
                        GameType = orderInfo.GameType,
                        AddMoney = orderInfo.AddMoney,
                        RedBagAwardsMoney = orderInfo.RedBagAwardsMoney,
                        BonusAwardsMoney = orderInfo.BonusAwardsMoney,
                        StopAfterBonus = orderInfo.StopAfterBonus,

                        CodeList = codeList,
                        TogetherInfo = togetherInfo,
                        JoinTogetherUserList = joinTogetherUserList,
                    };

                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询订单明细成功",
                        MsgId = entity.MsgId,
                        Value = result,
                    });
                }
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询合买订单详细_203
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetBonus([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "count",20}
                };
                var bonus = await _serviceProxyProvider.Invoke<List<LotteryNewBonusInfo>>(param, "api/Order/QueryLotteryNewBonusInfoList");
                var bonuslist = bonus.OrderByDescending(p => p.AfterTaxBonusMoney).ToList();
                if (bonuslist == null) bonuslist = new List<LotteryNewBonusInfo>();
                //if (bonuslist != null && bonuslist.Count > 0)
                //{
                var list = bonuslist.Select(p => new
                {
                    GameName = ConvertHelper.GameName(p.GameCode, p.GameType),
                    CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                    UserDisplayName = ConvertHelper.GetBankCardNumberxxxString(p.UserDisplayName),
                    TotalMoney = p.AfterTaxBonusMoney,
                    SchemeId = p.SchemeId,
                    GameCode = p.GameCode,
                    GameType = p.GameType
                }).ToList();
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
                //}
                //return Json(new LotteryServiceResponse
                //{
                //    Code = ResponseCode.失败,
                //    Message = "暂无数据",
                //    MsgId = entity.MsgId,
                //    Value = "暂无数据",
                //});
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取成功" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取成功",
                });
            }
        }
        /// <summary>
        /// 开奖记录_220
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> LotteryRecord([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string type = p.GameCode;
                string term = p.PageIndex;
                int page = 0;
                int.TryParse(term, out page);
                var list = await GetHistory(_serviceProxyProvider, type, page);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取开奖记录成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询开奖历史
        /// </summary>
        public async Task<List<KaiJiangHistory>> GetHistory([FromServices]IServiceProxyProvider _serviceProxyProvider, string gameCode, int page)
        {
            gameCode = gameCode.ToUpper();
            var startTime = DateTime.Now.AddYears(-1);
            var endTime = DateTime.Now;
            List<KaiJiangHistory> list = new List<KaiJiangHistory>();
            string[] arr = { "T14C", "TR9", "T6BQC", "T4CJQ" };
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"startTime",startTime },{"endTime", endTime},{"pageIndex",page },{"pageSize",ConvertHelper.MaxIssuseCount("CTZQ") }
            };
            if (arr.Count(a => a == gameCode) == 1)
            {
                param.Add("gameCode", string.Format("{0}_{1}", "CTZQ", gameCode));
                var numberHistoryList = await _serviceProxyProvider.Invoke<GameWinNumber_InfoCollection>(param, "api/Order/QueryGameWinNumberByDate");
                foreach (var item in numberHistoryList.List)
                {
                    list.Add(new KaiJiangHistory()
                    {
                        result = item.WinNumber,
                        time = item.CreateTime.ToString("yyyy-MM-dd"),
                        prizepool = "",
                        term = item.IssuseNumber,
                        type = ConvertHelper.GetGameName(item.GameCode, item.GameType),
                        sale = ""
                    });
                }
            }
            else
            {
                param.Add("gameCode", gameCode);
                var numberHistoryList = await _serviceProxyProvider.Invoke<GameWinNumber_InfoCollection>(param, "api/Order/QueryGameWinNumberByDateDesc");
                foreach (var item in numberHistoryList.List)
                {
                    list.Add(new KaiJiangHistory()
                    {
                        result = item.WinNumber,
                        time = item.CreateTime.ToString("yyyy-MM-dd"),
                        prizepool = "",
                        term = item.IssuseNumber,
                        type = ConvertHelper.GetGameName(item.GameCode, item.GameType),
                        sale = ""
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 数字彩与传统足球最新开奖记录_221
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> RecordHistory_Normal([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var infos = await GetKaiJiang(_serviceProxyProvider);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取开奖记录成功",
                    MsgId = entity.MsgId,
                    Value = infos,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        public async Task<List<KaiJiang>> GetKaiJiang([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            try
            {
                var list = await GetRedisList(_serviceProxyProvider);
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<List<KaiJiang>> GetRedisList([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {

            var list = new List<KaiJiang>();
            string redisKey = EntityModel.Redis.RedisKeys.KaiJiang_Key;
            var entitys = RedisHelperEx.DB_Match.GetObj<GameWinNumber_InfoCollection>(redisKey);
            if (entitys == null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "gameString","JX11X5|GD11X5|SD11X5|CQSSC|SSQ|DLT|FC3D|PL3|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ|CTZQ_TR9"}
                };
                entitys = await _serviceProxyProvider.Invoke<GameWinNumber_InfoCollection>(param, "api/Order/QueryAllGameNewWinNumber");
            }
            foreach (var item in entitys.List)
            {
                //读取文件信息
                var poolInfo = BettingHelper.GetPoolInfo(item.GameCode, item.IssuseNumber);

                list.Add(new KaiJiang()
                {
                    result = item.WinNumber,
                    prizepool = poolInfo != null ? poolInfo.TotalPrizePoolMoney.ToString("###,##0.00") : "",
                    nums = ConvertHelper.Getnums(poolInfo),
                    name = item.GameCode.ToUpper() == "CTZQ" ? item.GameType : item.GameCode,
                    termNo = item.IssuseNumber,
                    ver = "1",
                    grades = ConvertHelper.Getgrades(poolInfo),
                    date = item.CreateTime.ToString("yyyy-MM-dd"),
                    type = ConvertHelper.GetGameName(item.GameCode, item.GameType),
                    sale = poolInfo != null ? poolInfo.TotalSellMoney.ToString("###,##0.00") : ""
                });
            }
            list[list.Count - 1].name = "TR9";
            list[list.Count - 1].type = "任选9";
            return list;
        }

        /// <summary>
        /// 开奖详情_222
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> RecordDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {

            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string type = p.GameCode;
                string term = p.IssuseNumber;
                var obj = await GetKaiJingInfo(_serviceProxyProvider, "Web", type, term);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取开奖详情成功",
                    MsgId = entity.MsgId,
                    Value = obj,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }

        }
        public async Task<PrizelevelInfo> GetKaiJingInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, string version, string type, string term)
        {
            type = type.ToUpper();
            //"JX11X5|CQSSC|SSQ|DLT|FC3D|PL3|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ|CTZQ_TR9"
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"byOfficial",true }
            };
            var entity = await _serviceProxyProvider.Invoke<List<LotteryIssuse_QueryInfo>>(param, "api/Order/QueryAllGameCurrentIssuse");
            var entitys = entity as List<LotteryIssuse_QueryInfo>;
            param.Clear();
            var bjdcentity = await _serviceProxyProvider.Invoke<BJDCIssuseInfo>(param, "api/Order/QueryBJDCCurrentIssuseInfo");//北单
            PrizelevelInfo info = new PrizelevelInfo();
            info.prizeLevel = new List<Prizelevel>();
            var gameCode = type;
            if (type.StartsWith("CTZQ"))
            {
                var strs = type.Split('_');
                gameCode = strs[0];
                //var gameType = strs[1];
                var poolInfo = BettingHelper.GetPoolInfo_CTZQ(type.ToUpper(), term);
                foreach (var item in poolInfo)
                {
                    info.prizeLevel.Add(new Prizelevel()
                    {
                        betnum = item.BonusCount.ToString(),
                        prize = item.BonusMoney.ToString("###,##0.00"),
                        name = item.BonusLevelDisplayName
                    });
                }
            }
            else
            {
                var poolInfo = BettingHelper.GetPoolInfo(type.ToUpper(), term);
                if (poolInfo.GradeList != null)
                {
                    foreach (var item in poolInfo.GradeList)
                    {
                        info.prizeLevel.Add(new Prizelevel()
                        {
                            betnum = item.BonusCount.ToString(),
                            prize = item.BonusMoney.ToString("###,##0.00"),
                            name = item.GradeName
                        });
                    }
                }
            }
            var model = entitys.Find(a => a.GameCode == gameCode);
            info.stopSendPrizeTime = model != null ? model.LocalStopTime.ToString() : DateTime.Now.ToString();

            return info;
        }
        /// <summary>
        /// 传统足球开奖比赛内容
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> RecordOpenMatch([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string type = p.GameType;
                string term = p.IssuseNumber;
                var list = await GetphoneOpenMatch(_serviceProxyProvider, "web", type, term);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取开奖比赛数据成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 3.4对阵详情：是在传统足球开奖详情页内请求
        /// </summary>
        public async Task<List<KaiJiangOpenMatch>> GetphoneOpenMatch([FromServices]IServiceProxyProvider _serviceProxyProvider, string version, string type, string term)
        {
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "gameType",type},{ "issuseNumber",term}
            };
            List<KaiJiangOpenMatch> list = new List<KaiJiangOpenMatch>();
            var match = await _serviceProxyProvider.Invoke<CTZQMatchInfo_Collection>(param, "api/Order/QueryCTZQMatchListByIssuseNumber");
            if (match == null || match.ListInfo == null)
                return list;
            int i = 1;
            foreach (var item in match.ListInfo)
            {
                list.Add(new KaiJiangOpenMatch()
                {
                    result = BettingHelper.GetResult(item.HomeTeamScore, item.GuestTeamScore),
                    match_point = -1,
                    whole_score = item.HomeTeamScore + ":" + item.GuestTeamScore,
                    match_name = item.MatchName,
                    away_team = item.GuestTeamName.Replace("\u0026nbsp;", "").Replace("&nbsp;", ""),
                    match_state = "已完成",
                    home_team = item.HomeTeamName.Replace("\u0026nbsp;", "").Replace("&nbsp;", ""),
                    half_score = item.HomeTeamHalfScore + ":" + item.GuestTeamHalfScore,
                    bout_index = (i++).ToString(),
                    match_time = ""
                });
            }
            return list;
        }
        private string AnalyticalCurrentSp(string currentSp, string code)
        {
            if (string.IsNullOrEmpty(currentSp) || string.IsNullOrEmpty(code))
                return string.Empty; ;
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

        #region 20180821新增三个接口（1.获取合买用户列表，2.获取自己的合买数据列表，3.仅根据订单号获取订单信息）
        /// <summary>
        /// 合买用户列表
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetQuerySportsTogetherJoinList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string schemeId = p.SchemeId;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                string userToken = p.UserToken;
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("schemeId", schemeId);
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                param.Add("userId", userId);
                var join = await _serviceProxyProvider.Invoke<Sports_TogetherJoinInfoCollection>(param, "api/Order/QuerySportsTogetherJoinList");
                var joinList = new List<object>();
                foreach (var item in join.List)
                {
                    if (item.JoinType != TogetherJoinType.Guarantees && item.JoinType != TogetherJoinType.SystemGuarantees)
                    {
                        joinList.Add(new
                        {
                            UserId = item.UserId,
                            UserDisplayName = ConvertHelper.HideUserName(item.UserDisplayName, item.HideDisplayNameCount),
                            HideDisplayNameCount = item.HideDisplayNameCount,
                            SchemeId = item.SchemeId,
                            RealBuyCount = item.RealBuyCount,
                            Price = item.Price,
                            JoinType = ConvertHelper.FomartJoinType(item.JoinType),
                            JoinId = item.JoinId,
                            JoinDateTime = item.JoinDateTime,
                            IsSucess = item.IsSucess,
                            BuyCount = item.BuyCount,
                            BonusMoney = item.BonusMoney,
                        });
                    }

                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = new { joinList = joinList, totalCount = join.TotalCount },
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        public async Task<IActionResult> GetMySportsTogetherListBySchemeId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string schemeId = p.SchemeId;
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("用户未登录");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param.Add("userToken", userToken);
                //var userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");
                //if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId)) throw new Exception("找不到相关用户");
                //param.Clear();
                param.Add("schemeId", schemeId);
                param.Add("userId", userId);
                var join = await _serviceProxyProvider.Invoke<List<Sports_TogetherJoinInfo>>(param, "api/Order/QueryMySportsTogetherListBySchemeId");
                var joinList = new List<object>();
                foreach (var item in join)
                {
                    if (item.JoinType != TogetherJoinType.Guarantees && item.JoinType != TogetherJoinType.SystemGuarantees)
                    {
                        joinList.Add(new
                        {
                            UserId = item.UserId,
                            UserDisplayName = ConvertHelper.HideUserName(item.UserDisplayName, item.HideDisplayNameCount),
                            HideDisplayNameCount = item.HideDisplayNameCount,
                            SchemeId = item.SchemeId,
                            RealBuyCount = item.RealBuyCount,
                            Price = item.Price,
                            JoinType = ConvertHelper.FomartJoinType(item.JoinType),
                            JoinId = item.JoinId,
                            JoinDateTime = item.JoinDateTime,
                            IsSucess = item.IsSucess,
                            BuyCount = item.BuyCount,
                            BonusMoney = item.BonusMoney,
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = joinList,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        public async Task<IActionResult> QueryNewOrderDetailBySchemeId_Share([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string schemeId = p.SchemeId;
                if (string.IsNullOrEmpty(schemeId))
                    throw new LogicException("订单号不能为空！");

                if (schemeId.StartsWith("CHASE"))
                {
                    return await QueryCHASEOrderDetail(_serviceProxyProvider, entity, schemeId);
                }
                else if (schemeId.StartsWith("TSM"))
                {
                    return await QueryTMSOrderDetail(_serviceProxyProvider, entity, schemeId);
                }
                else
                {
                    return await QueryGeneralOrderDetail(_serviceProxyProvider, entity, schemeId);
                }
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }

        }

        #endregion

    }
}
