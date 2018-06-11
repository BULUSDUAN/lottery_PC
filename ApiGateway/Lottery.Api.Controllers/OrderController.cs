﻿using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.RequestModel;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Helper;
using Lottery.ApiGateway.Model.Enum;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
                
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                if (string.IsNullOrEmpty(p.GameCode))
                    throw new Exception("彩种不能为空");
                //param.userToken = p.UserToken;
                param.Add("gameCode", p.GameCode.ToUpper());
                param.Add("gameType", p.GameType.ToUpper());
                param.Add("pageIndex", p.PageIndex);
                param.Add("pageSize", p.PageSize);
                param.Add("key",p.KeyWord);
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
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                if (string.IsNullOrEmpty(p.gameCode))
                    throw new Exception("传入彩种不能为空");
                param.Add("gameCode", p.GameCode);
                param.Add("issuseNumber", p.IssuseNumber);
                param.Add("gameType", p.GameType);
                DateTime startTime = string.IsNullOrEmpty(p.StartTime) ? DateTime.Now : Convert.ToDateTime(p.StartTime);
                DateTime endTime = string.IsNullOrEmpty(p.EndTime) ? DateTime.Now : Convert.ToDateTime(p.EndTime);
                param.Add("pageIndex", p.PageIndex == null ? 0 : Convert.ToInt32(p.PageIndex));
                param.Add("pageSize", p.PageSize == null ? 0 : Convert.ToInt32(p.PageSize));
                var list = new List<object>();
                if (p.gameCode.ToUpper() == "JCZQ")
                {
                    //读取json
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var jczqFileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}_{1}.json", p.gameCode, endTime.ToString("yyyyMMdd")));
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
                    var jczqFileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}_{1}.json", p.gameCode, endTime.ToString("yyyyMMdd")));
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
                Dictionary<string, object> param = new Dictionary<string, object>();

                var p = WebHelper.Decode(entity.Param);
                if (string.IsNullOrEmpty(p.userToken))
                    throw new ArgumentException("您还未登陆");

                param.Add("viewType", p.ViewType);
                param.Add("userToken", p.UserToken);
                DateTime startTime = p.StartTime == null ? DateTime.Now : Convert.ToDateTime(p.StartTime);
                int days = p.Days;
                startTime = startTime.AddDays(-days).Date;
                param.Add("startTime", startTime);
                param.Add("endTime", p.EndTime == null ? DateTime.Now : Convert.ToDateTime(p.EndTime));
                param.Add("pageIndex", p.PageIndex ?? 0);
                param.Add("pageSize", p.PageSize ?? 1);
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
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("userId", p.UserId);
                param.Add("state", p.BonusStatus == -1 ? null : (BonusStatus?)p.BonusStatus);
                //int schemeType = p.SchemeType;
                param.Add("days", p.ViewDay);
                param.Add("startTime", DateTime.Now.AddDays(-p.ViewDay));
                param.Add("endTime", DateTime.Now);
                param.Add("pageIndex", p.PageIndex);
                param.Add("pageSize", p.PageSize);
                param.Add("userToken", p.UserToken);
                int orderQueryType = p.OrderQueryType;
                if (string.IsNullOrEmpty(p.UserId) || string.IsNullOrEmpty(p.UserToken))
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

        public async Task<LotteryServiceResponse> QuerySportsTogetherList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("gameCode",p.GameCode);
                param.Add("gameType",p.GameType);
                param.Add("pageIndex",p.Pageindex);
                param.Add("PageSize",p.PageSize);
                param.Add("orderBy",p.orderBy);
                param.Add("sortType",p.sortType);
                param.Add("userToken",p.UserToken);

                string userId = string.Empty;
                if (!string.IsNullOrEmpty(p.userToken))
                {
                    param.Add("userToken", p.userToken);
                    var userInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/User/LoginByUserToken");


                    if (userInfo != null)
                        userId = userInfo.UserId;
                }

                if (p.orderBy == "Progress")
                    p.orderBy = "ManYuan desc, Progress " + p.sortType + ",TotalMoney DESC,ISTOP DESC";
                else if (p.orderBy == "TotalMoney")
                    p.orderBy = "ManYuan desc,TotalMoney " + p.sortType + ", Progress DESC,ISTOP DESC";

                var list = new List<object>();
                param.Add("key", "");
                param.Add("issuseNumber", "");
                param.Add("TogetherSchemeSecurity", null);
                param.Add("betCategory",null);
                param.Add("TogetherSchemeProgress", null);
                param.Add("minMoney", -1);
                param.Add("maxMoney", -1);
                param.Add("minProgress", -1);
                param.Add("maxProgress", -1);
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
    }
}
