using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common.Communication;
using External.Business.Domain.Managers.Celebritys;
using GameBiz.Business;
using External.Core;
using External.Domain.Entities.Celebritys;

namespace External.Business
{
    public class HandleWinnerModelCycleBusiness : IOrderPrize_AfterTranCommit, IComplateTicket
    {
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            #region 方案一(new)

            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var celebrityManager = new CelebrityManager();
                var modelCycleEntityList = celebrityManager.QueryWinnerModelCycleBySchemeId(schemeId);
                if (modelCycleEntityList != null && modelCycleEntityList.Count > 0)
                {
                    foreach (var modelCycle in modelCycleEntityList)
                    {
                        try
                        {
                            //修改每期方案信息
                            modelCycle.CurrBonusMoney = afterTaxBonusMoney;
                            modelCycle.ModelProgressStatus = isBonus ? ModelProgressStatus.Winning : ModelProgressStatus.NoBonus;
                            modelCycle.IsComplete = true;
                            modelCycle.CompleteTime = DateTime.Now;
                            celebrityManager.EditWinnerModelCycle(modelCycle);

                            //修改模型
                            var tempWinnerModel = celebrityManager.QueryWinnerModelByModelId(modelCycle.ModelId);
                            if (tempWinnerModel == null)
                                throw new Exception("未查询到模型" + modelCycle.ModelId + "");
                            tempWinnerModel.TotalBonusIssuseCount += isBonus ? 1 : 0;//统计累计中奖期数
                            tempWinnerModel.TotalModelBonusMoney += isBonus ? afterTaxBonusMoney : 0;
                            //winnerModel.TotalNotBonusIssuseCount += isBonus ? 0 : 1;//统计累计未中奖期数
                            //tempWinnerModel.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
                            //if (isBonus)//计算累计盈利金额
                            //{
                            //    var profitMoney = afterTaxBonusMoney - modelCycle.CurrBettingMoney;
                            //    if (profitMoney > 0M)
                            //        tempWinnerModel.TotalProfitMoney += profitMoney;
                            //}
                            if (tempWinnerModel.TotalModelBettingMoney > 0M)//计算总回报率
                            {
                                var reportRatio = Math.Truncate((tempWinnerModel.TotalModelBonusMoney / tempWinnerModel.TotalModelBettingMoney) * 100) / 100;
                                tempWinnerModel.TotalReportRatio += reportRatio;
                            }
                            if (tempWinnerModel.TotalBettingIssuseCount > 0)//计算中奖频率
                            {
                                decimal bonusFrequency = Math.Truncate((tempWinnerModel.TotalBonusIssuseCount / tempWinnerModel.TotalBettingIssuseCount) * 100M) / 100;
                                tempWinnerModel.BonusFrequency += bonusFrequency;
                            }
                            celebrityManager.EditWinnerModel(tempWinnerModel);
                        }
                        catch { }
                    }
                }
                else
                {
                    var modelSchemeDetailEntity = celebrityManager.QueryWinnerModelSchemeDetailBySchemeId(schemeId);
                    if (modelSchemeDetailEntity == null)
                        return;
                    var modelSchemeEntity = celebrityManager.QueryWinnerModelSchemeByKeyLine(modelSchemeDetailEntity.ModelKeyLine);
                    if (modelSchemeEntity == null)
                        throw new Exception("未查询到追号计划");

                    var winnerModel = celebrityManager.QueryWinnerModelByModelId(modelSchemeEntity.ModelId);
                    if (winnerModel == null)
                        throw new Exception("未查询到模型信息");
                    winnerModel.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
                    var profitMoneyYL = afterTaxBonusMoney - modelSchemeDetailEntity.CurrBettingMoney;
                    if (isBonus)//计算累计盈利金额
                    {
                        if (profitMoneyYL > 0M)
                            winnerModel.TotalProfitMoney += profitMoneyYL;
                    }
                    celebrityManager.EditWinnerModel(winnerModel);

                    //修改追号计划
                    var detailList = new List<WinnerModelSchemeDetail>();
                    //modelSchemeEntity.SchemeProgressStatus = SchemeProgressStatus.ModelStop;
                    modelSchemeEntity.CompleteIssuseCount += 1;
                    modelSchemeEntity.CompleteIssuseMoney += orderMoney;
                    modelSchemeEntity.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
                    var profiteRatio = Math.Truncate(((modelSchemeEntity.TotalBonusMoney - modelSchemeEntity.CompleteIssuseMoney) / modelSchemeEntity.CompleteIssuseMoney) * 100M) / 100;//计算盈利比率
                    if (modelSchemeEntity.IsProfitedStop && profitMoneyYL > 0M)//判断盈利后停止
                    {
                            modelSchemeEntity.IsStop = true;
                            modelSchemeEntity.StopTime = DateTime.Now;
                            modelSchemeEntity.StopDesc = "追号计划盈利，自动停止";
                            modelSchemeEntity.SchemeProgressStatus = SchemeProgressStatus.ModelStop;
                    }
                    else if (modelSchemeEntity.ProfiteRatio > 0M && profiteRatio>=modelSchemeEntity.ProfiteRatio)//判断当达到用户设置盈利比率后，停止追号计划
                    {
                        modelSchemeEntity.IsStop = true;
                        modelSchemeEntity.StopTime = DateTime.Now;
                        modelSchemeEntity.StopDesc = "追号计划达到盈利比率，自动停止";
                        modelSchemeEntity.SchemeProgressStatus = SchemeProgressStatus.ModelStop;
                    }

                    // 判断是否该退还模型作者先行赔付金额、提取佣金
                    if (modelSchemeEntity.TotalChaseIssuseCount == modelSchemeEntity.CompleteIssuseCount)//判断当前追号计划是否完成
                    {
                        if (modelSchemeEntity.CurrLossMoney > 0M)//判断是否有先行赔付金额
                        {
                            var profitMoney = modelSchemeEntity.TotalBonusMoney - (modelSchemeEntity.TotalMoney + modelSchemeEntity.CurrLossMoney);
                            if (profitMoney > 0)//追号计划盈利
                            {
                                //退还作者先行赔付金
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_WinnerModelPayBackFirstPayMoney, winnerModel.UserId, modelSchemeEntity.ModelKeyLine, modelSchemeEntity.CurrLossMoney, string.Format("追号计划{0}盈利,退还作者先行赔付金额{1}元", modelSchemeEntity.ModelKeyLine, modelSchemeEntity.CurrLossMoney));

                                //计算佣金
                                var money = modelSchemeEntity.TotalBonusMoney - modelSchemeEntity.CompleteIssuseMoney;
                                if (money > 0M)
                                {
                                    money = money * winnerModel.CommissionRitio;
                                    if (money >= 0.01M)
                                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_Commission, winnerModel.UserId, modelSchemeEntity.ModelKeyLine, money, string.Format("追号计划{0}盈利,模型作者先行赔付提取佣金{1}元", modelSchemeEntity.ModelKeyLine, money));
                                }
                            }
                        }
                    }

                    //修改追号详情
                    modelSchemeDetailEntity.AfterTaxBonusMoney = afterTaxBonusMoney;
                    modelSchemeDetailEntity.IsComplete = true;
                    detailList.Add(modelSchemeDetailEntity);

                    if (modelSchemeEntity.IsStop)//判断当追号计划停止后，修改完成标识、完成期数、完成金额
                    {
                        var modelSchemeDetailList = celebrityManager.QueryWinnerModelSchemeDetailByKeyLine(modelSchemeEntity.ModelKeyLine);
                        foreach (var item in modelSchemeDetailList)
                        {
                            item.IsComplete = true;
                            detailList.Add(item);
                        }
                        modelSchemeEntity.CompleteIssuseCount = modelSchemeEntity.TotalChaseIssuseCount;
                        modelSchemeEntity.CompleteIssuseMoney = modelSchemeEntity.TotalMoney;
                    }

                    celebrityManager.UpdateWinnerModelScheme(modelSchemeEntity);
                    celebrityManager.UpdateWinnerModelSchemeDetail(detailList.ToArray());
                }

                biz.CommitTran();
            }

            #endregion

            #region 方案一(old)

            //using (var biz = new GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            //    var celebrityManager = new CelebrityManager();
            //    //修改追号详情
            //    var modelSchemeDetailEntity = celebrityManager.QueryWinnerModelSchemeDetailBySchemeId(schemeId);
            //    if (modelSchemeDetailEntity == null || string.IsNullOrEmpty(modelSchemeDetailEntity.ModelKeyLine))//当追号详情为空时，检查每期方案表，当前期是否被购买，如果没有任何购买记录则需要修改相应字段数据
            //    {
            //        var modelCycleList = celebrityManager.QueryWinnerModelCycleBySchemeId(schemeId);
            //        if (modelCycleList != null && modelCycleList.Count > 0)
            //        {
            //            foreach (var item in modelCycleList)
            //            {
            //                var tempWinnerModelScheme = celebrityManager.QueryWinnerModelSchemeListByModelId(item.ModelId);
            //                if (tempWinnerModelScheme == null||tempWinnerModelScheme.Count<=0)
            //                {
            //                    //修改每期方案信息
            //                    item.CurrBonusMoney = afterTaxBonusMoney;
            //                    item.ModelProgressStatus = isBonus ? ModelProgressStatus.Winning : ModelProgressStatus.NoBonus;
            //                    item.IsComplete = true;
            //                    item.CompleteTime = DateTime.Now;
            //                    celebrityManager.EditWinnerModelCycle(item);

            //                    var tempWinnerModel = celebrityManager.QueryWinnerModelByModelId(item.ModelId);
            //                    //修改模型
            //                    tempWinnerModel.TotalBonusIssuseCount += isBonus ? 1 : 0;//统计累计中奖期数
            //                    //winnerModel.TotalNotBonusIssuseCount += isBonus ? 0 : 1;//统计累计未中奖期数
            //                    tempWinnerModel.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
            //                    if (isBonus)//计算累计盈利金额
            //                    {
            //                        var profitMoney = afterTaxBonusMoney - item.CurrBettingMoney;
            //                        if (profitMoney > 0M)
            //                            tempWinnerModel.TotalProfitMoney += profitMoney;
            //                    }
            //                    if (tempWinnerModel.TotalSaleMoney > 0M)//计算总回报率
            //                    {
            //                        var reportRatio = Math.Truncate((tempWinnerModel.TotalBonusMoney / tempWinnerModel.TotalSaleMoney) * 100) / 100;
            //                        tempWinnerModel.TotalReportRatio += reportRatio;
            //                    }
            //                    if (tempWinnerModel.TotalBettingIssuseCount > 0)//计算中奖频率
            //                    {
            //                        decimal bonusFrequency = Math.Truncate((tempWinnerModel.TotalBonusIssuseCount / tempWinnerModel.TotalBettingIssuseCount) * 100M) / 100;
            //                        tempWinnerModel.BonusFrequency += bonusFrequency;
            //                    }
            //                    celebrityManager.EditWinnerModel(tempWinnerModel);
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var modelSchemeEntity = celebrityManager.QueryWinnerModelSchemeByKeyLine(modelSchemeDetailEntity.ModelKeyLine);
            //        if (modelSchemeEntity == null)
            //            throw new Exception("未查询到追号计划");

            //        var winnerModel = celebrityManager.QueryWinnerModelByModelId(modelSchemeEntity.ModelId);
            //        if (winnerModel == null)
            //            throw new Exception("未查询到模型信息");

            //        var modelCycl = celebrityManager.QueryWinnerModelCycleById(modelSchemeDetailEntity.ModelCycleId);
            //        if (modelCycl == null)
            //            throw new Exception("未查询到每期方案信息");

            //        //修改模型
            //        winnerModel.TotalBonusIssuseCount += isBonus ? 1 : 0;//统计累计中奖期数
            //        //winnerModel.TotalNotBonusIssuseCount += isBonus ? 0 : 1;//统计累计未中奖期数
            //        winnerModel.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
            //        if (isBonus)//计算累计盈利金额
            //        {
            //            var profitMoney = afterTaxBonusMoney - modelCycl.CurrBettingMoney;
            //            if (profitMoney > 0M)
            //                winnerModel.TotalProfitMoney += profitMoney;
            //        }
            //        if (winnerModel.TotalSaleMoney > 0M)//计算总回报率
            //        {
            //            var reportRatio = Math.Truncate((winnerModel.TotalBonusMoney / winnerModel.TotalSaleMoney) * 100) / 100;
            //            winnerModel.TotalReportRatio = reportRatio;
            //        }
            //        if (winnerModel.TotalBettingIssuseCount > 0)//计算中奖频率
            //        {
            //            decimal bonusFrequency = Math.Truncate((winnerModel.TotalBonusIssuseCount / winnerModel.TotalBettingIssuseCount) * 100M) / 100;
            //            winnerModel.BonusFrequency = bonusFrequency;
            //        }
            //        celebrityManager.EditWinnerModel(winnerModel);

            //        //修改每期方案信息
            //        modelCycl.CurrBonusMoney = afterTaxBonusMoney;
            //        modelCycl.ModelProgressStatus = isBonus ? ModelProgressStatus.Winning : ModelProgressStatus.NoBonus;
            //        modelCycl.IsComplete = true;
            //        modelCycl.CompleteTime = DateTime.Now;
            //        celebrityManager.EditWinnerModelCycle(modelCycl);

            //        //修改追号计划
            //        var detailList = new List<WinnerModelSchemeDetail>();
            //        modelSchemeEntity.SchemeProgressStatus = SchemeProgressStatus.ModelStop;
            //        modelSchemeEntity.CompleteIssuseCount += 1;
            //        modelSchemeEntity.CompleteIssuseMoney += orderMoney;
            //        modelSchemeEntity.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
            //        if (modelSchemeEntity.IsProfitedStop)//判断盈利后停止
            //        {
            //            var profiteRatio = Math.Truncate(((modelSchemeEntity.TotalBonusMoney - modelSchemeEntity.TotalMoney) / modelSchemeEntity.TotalMoney) * 100M) / 100;
            //            if (profiteRatio >= modelSchemeEntity.ProfiteRatio)
            //            {
            //                modelSchemeEntity.IsStop = true;
            //                modelSchemeEntity.StopTime = DateTime.Now;
            //                modelSchemeEntity.StopDesc = "追号计划达到盈利比率，自动停止";
            //            }
            //        }

            //        // 判断是否该退还模型作者先行赔付金额、提取佣金
            //        if (modelSchemeEntity.TotalChaseIssuseCount == modelSchemeEntity.CompleteIssuseCount)//判断当前追号计划是否完成
            //        {
            //            if (modelSchemeEntity.CurrLossMoney > 0M)//判断是否有先行赔付金额
            //            {
            //                var profitMoney = modelSchemeEntity.TotalBonusMoney - (modelSchemeEntity.TotalMoney + modelSchemeEntity.CurrLossMoney);
            //                if (profitMoney > 0)//追号计划盈利
            //                {
            //                    //退还作者先行赔付金
            //                    BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_WinnerModelPayBackFirstPayMoney, winnerModel.UserId, modelSchemeEntity.ModelKeyLine, modelSchemeEntity.CurrLossMoney, string.Format("追号计划{0}盈利,退还作者先行赔付金额{1}元", modelSchemeEntity.ModelKeyLine, modelSchemeEntity.CurrLossMoney));

            //                    //计算佣金
            //                    var money = modelSchemeEntity.TotalBonusMoney - modelSchemeEntity.TotalMoney;
            //                    if (money > 0M)
            //                    {
            //                        money = money * winnerModel.CommissionRitio;
            //                        if (money >= 0.01M)
            //                            BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_Commission, winnerModel.UserId, modelSchemeEntity.ModelKeyLine, money, string.Format("追号计划{0}盈利,模型作者先行赔付提取佣金{1}元", modelSchemeEntity.ModelKeyLine, money));
            //                    }
            //                }
            //            }
            //        }

            //        //修改追号详情
            //        modelSchemeDetailEntity.AfterTaxBonusMoney = afterTaxBonusMoney;
            //        modelSchemeDetailEntity.IsComplete = true;
            //        detailList.Add(modelSchemeDetailEntity);

            //        if (modelSchemeEntity.IsStop)//判断当追号计划停止后，修改完成标识、完成期数、完成金额
            //        {
            //            var modelSchemeDetailList = celebrityManager.QueryWinnerModelSchemeDetailByKeyLine(modelSchemeEntity.ModelKeyLine);
            //            foreach (var item in modelSchemeDetailList)
            //            {
            //                item.IsComplete = true;
            //                detailList.Add(item);
            //            }
            //            modelSchemeEntity.CompleteIssuseCount = modelSchemeEntity.TotalChaseIssuseCount;
            //            modelSchemeEntity.CompleteIssuseMoney = modelSchemeEntity.TotalMoney;
            //        }

            //        celebrityManager.UpdateWinnerModelScheme(modelSchemeEntity);
            //        celebrityManager.UpdateWinnerModelSchemeDetail(detailList.ToArray());
            //    }

            //    biz.CommitTran();

            //}

            #endregion

            #region 方案二(注:奖金不正确)

            //using (var biz = new GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            //    var celebrityManager = new CelebrityManager();
            //    var modelCycleList = celebrityManager.QueryWinnerModelCycleBySchemeId(schemeId);
            //    if (modelCycleList == null || modelCycleList.Count <= 0)
            //        return;
            //    //修改每期方案表数据
            //    foreach (var item in modelCycleList)
            //    {
            //        item.CurrBonusMoney = afterTaxBonusMoney;
            //        item.ModelProgressStatus = isBonus ? ModelProgressStatus.Winning : ModelProgressStatus.NoBonus;
            //        item.IsComplete = true;
            //        item.CompleteTime = DateTime.Now;
            //        celebrityManager.EditWinnerModelCycle(item);

            //        //修改模型
            //        var winnerModel = celebrityManager.QueryWinnerModelByModelId(item.ModelId);
            //        if (winnerModel == null)
            //            throw new Exception("未查询到模型信息");
            //        winnerModel.TotalBonusIssuseCount += isBonus ? 1 : 0;//统计累计中奖期数
            //        //winnerModel.TotalNotBonusIssuseCount += isBonus ? 0 : 1;//统计累计未中奖期数
            //        winnerModel.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
            //        if (isBonus)//计算累计盈利金额
            //        {
            //            var profitMoney = item.CurrBonusMoney - item.CurrBettingMoney;
            //            if (profitMoney > 0M)
            //                winnerModel.TotalProfitMoney += profitMoney;
            //        }
            //        if (winnerModel.TotalSaleMoney > 0M)//计算总回报率
            //        {
            //            var reportRatio = Math.Truncate((winnerModel.TotalBonusMoney / winnerModel.TotalSaleMoney) * 100) / 100;
            //            winnerModel.TotalReportRatio += reportRatio;
            //        }
            //        if (winnerModel.TotalBettingIssuseCount > 0)//计算中奖频率
            //        {
            //            decimal bonusFrequency = Math.Truncate((winnerModel.TotalBonusIssuseCount / winnerModel.TotalBettingIssuseCount) * 100M) / 100;
            //            winnerModel.BonusFrequency += bonusFrequency;
            //        }
            //        celebrityManager.EditWinnerModel(winnerModel);

            //        //修改追号计划
            //        var modelSchemeList = celebrityManager.QueryNotStopWinnerModelSchemeListByModelId(item.ModelId);//查询购买当前每期方案的追号计划
            //        foreach (var scheme in modelSchemeList)
            //        {
            //            //修改追号计划
            //            scheme.SchemeProgressStatus = SchemeProgressStatus.ModelStop;
            //            scheme.CompleteIssuseCount += 1;
            //            scheme.CompleteIssuseMoney += orderMoney;
            //            scheme.TotalBonusMoney += isBonus ? afterTaxBonusMoney : 0;
            //            if (scheme.IsProfitedStop)//判断盈利后停止
            //            {
            //                var profiteRatio = Math.Truncate(((scheme.TotalBonusMoney - scheme.TotalMoney) / scheme.TotalMoney) * 100M) / 100;
            //                if (profiteRatio >= scheme.ProfiteRatio)
            //                {
            //                    scheme.IsStop = true;
            //                    scheme.StopTime = DateTime.Now;
            //                    scheme.StopDesc = "追号计划达到盈利比率，自动停止";
            //                }
            //            }

            //            // 判断是否该退还模型作者先行赔付金额、提取佣金
            //            if (scheme.TotalChaseIssuseCount == scheme.CompleteIssuseCount)//判断当前追号计划是否完成
            //            {
            //                if (scheme.CurrLossMoney > 0M)//判断是否有先行赔付金额
            //                {
            //                    var profitMoney = scheme.TotalBonusMoney - (scheme.TotalMoney + scheme.CurrLossMoney);
            //                    if (profitMoney > 0)//追号计划盈利
            //                    {
            //                        //退还作者先行赔付金
            //                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_WinnerModelPayBackFirstPayMoney, winnerModel.UserId, scheme.ModelKeyLine, scheme.CurrLossMoney, string.Format("追号计划{0}盈利,退还作者先行赔付金额{1}元", scheme.ModelKeyLine, scheme.CurrLossMoney));

            //                        //计算佣金
            //                        var money = scheme.TotalBonusMoney - scheme.TotalMoney;
            //                        if (money > 0M)
            //                        {
            //                            money = money * winnerModel.CommissionRitio;
            //                            if (money >= 0.01M)
            //                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_Commission, winnerModel.UserId, scheme.ModelKeyLine, money, string.Format("追号计划{0}盈利,模型作者先行赔付提取佣金{1}元", scheme.ModelKeyLine, money));
            //                        }
            //                    }

            //                }
            //            }

            //            //修改追号详情
            //            //var modelSchemeDetailEntity = celebrityManager.QueryWinnerModelSchemeDetailBySchemeId(schemeId, scheme.ModelKeyLine);
            //            //if (modelSchemeDetailEntity == null)
            //            //    throw new Exception("未查询到追号计划详情");
            //            var modelSchemeDetailEntity = celebrityManager.QueryWinnerModelSchemeDetailByKeyLine(scheme.ModelKeyLine).FirstOrDefault(s => s.ModelCycleId == item.ModelCycleId && s.IsComplete == false);
            //            if (modelSchemeDetailEntity.PayStatus == PayStatus.WaitingPay)
            //                modelSchemeDetailEntity.PayStatus = PayStatus.FailPay;
            //            modelSchemeDetailEntity.AfterTaxBonusMoney = afterTaxBonusMoney;
            //            modelSchemeDetailEntity.IsComplete = true;

            //            celebrityManager.UpdateWinnerModelScheme(scheme);
            //            celebrityManager.UpdateWinnerModelSchemeDetail(modelSchemeDetailEntity);

            //        }

            //    }

            //    biz.CommitTran();
            //}

            #endregion
        }

        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var celebrityManager = new CelebrityManager();

            var modelCycleList = celebrityManager.QueryWinnerModelCycleBySchemeId(schemeId);
            if (modelCycleList != null && modelCycleList.Count() > 0)//统计模型投注总期数、投注总金额
            {
                foreach (var item in modelCycleList)
                {
                    try
                    {
                        var winnerModel = celebrityManager.QueryWinnerModelByModelId(item.ModelId);
                        if (winnerModel == null)
                            throw new Exception("未查询到模型");
                        winnerModel.TotalBettingIssuseCount += 1;
                        winnerModel.TotalModelBettingMoney += totalMoney;
                        celebrityManager.EditWinnerModel(winnerModel);
                    }
                    catch { }
                }
            }
            else//统计模型购买人数、销售金额
            {
                var modelSchemeDetail = celebrityManager.QueryWinnerModelSchemeDetailBySchemeId(schemeId);
                if (modelSchemeDetail == null)
                    return;
                var modelScheme = celebrityManager.QueryWinnerModelSchemeByKeyLine(modelSchemeDetail.ModelKeyLine);
                if (modelScheme == null)
                    return;
                var winnerModel = celebrityManager.QueryWinnerModelByModelId(modelScheme.ModelId);
                if (winnerModel == null)
                    throw new Exception("未查询到模型");

                winnerModel.TotalBuyCount += 1;
                winnerModel.TotalSaleMoney += totalMoney;
                celebrityManager.EditWinnerModel(winnerModel);
            }

            //var modelCycleList = celebrityManager.QueryWinnerModelCycleBySchemeId(schemeId);
            //if (modelCycleList == null || modelCycleList.Count <= 0)
            //    return;
            //foreach (var item in modelCycleList)
            //{
            //    try
            //    {
            //        //修改模型
            //        var winnerModel = celebrityManager.QueryWinnerModelByModelId(item.ModelId);
            //        if (winnerModel == null)
            //            throw new Exception("未查询到模型信息");
            //        winnerModel.TotalBuyCount += 1;
            //        winnerModel.TotalSaleMoney += totalMoney;
            //        celebrityManager.EditWinnerModel(winnerModel);
            //    }
            //    catch { }
            //}
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IHandleWinnerModelCycle_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
                        break;
                         case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_HandleWinnerModelCycleBusiness_Error_", type, ex);
            }
            return null;
        }
    }
}
