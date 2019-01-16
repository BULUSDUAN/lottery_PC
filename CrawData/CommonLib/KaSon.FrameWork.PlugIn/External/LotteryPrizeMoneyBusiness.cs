using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.ORM.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.PlugIn.External
{
    /// <summary>
    /// 部分数字彩 直接派钱
    /// </summary>
    public class LotteryPrizeMoneyBusiness :DBbase, IOrderPrize_AfterTranCommit
    {
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            //if (isVirtualOrder) return;
            //if (!isBonus) return;
            //if (afterTaxBonusMoney <= 0) return;
            //if (afterTaxBonusMoney >= 10000M) return;

            //开启事务
            DB.Begin();
            try
            {                
                var manager = new Sports_Manager();
                var order = manager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new LogicException(string.Format("自动派钱，没有找到订单{0}", schemeId));
                if (order.IsPrizeMoney)
                    throw new LogicException(string.Format("订单{0}已派奖", schemeId));

                order.IsPrizeMoney = true;
                manager.UpdateSports_Order_Complate(order);

                if (!order.IsVirtualOrder)
                {
                    if (order.SchemeType == (int)SchemeType.GeneralBetting || order.SchemeType == (int)SchemeType.ChaseBetting || order.SchemeType == (int)SchemeType.SingleCopy)
                    {
                        #region 普通、追号、抄单
                        if (order.AfterTaxBonusMoney > 0)
                        {
                            if (order.SchemeType == (int)SchemeType.SingleCopy)//抄单订单，派奖时需减去奖金提成的金额
                            {
                                var bdfxManager = new BDFXManager();
                                var bdfxRecorSingleEntity = bdfxManager.QueryBDFXRecordSingleCopyBySchemeId(schemeId);
                                var realBonusMoney = order.AfterTaxBonusMoney;
                                var commissionMoney = 0M;
                                if (bdfxRecorSingleEntity != null)
                                {
                                    var BDFXEntity = bdfxManager.QueryTotalSingleTreasureBySchemeId(bdfxRecorSingleEntity.BDXFSchemeId);
                                    if (BDFXEntity != null)
                                    {
                                        //计算提成金额
                                        if ((order.AfterTaxBonusMoney - order.TotalMoney) > 0)
                                        {
                                            commissionMoney = (order.AfterTaxBonusMoney - order.TotalMoney) * BDFXEntity.Commission / 100M;
                                            commissionMoney = Math.Truncate(commissionMoney * 100) / 100M;
                                            realBonusMoney = order.AfterTaxBonusMoney - commissionMoney;
                                            //返提成
                                            if (commissionMoney > 0)
                                            {
                                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_BDFXCommissionMoney, BDFXEntity.UserId, schemeId, commissionMoney,
                                            string.Format("抄单订单{0}中奖{1:N2}元,提成{2:N0}%,获得奖金盈利提成金额{3:N2}元.", schemeId, order.AfterTaxBonusMoney, BDFXEntity.Commission, commissionMoney));
                                            }
                                        }
                                    }
                                }
                                //返奖金
                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, realBonusMoney,
                                    string.Format("抄单订单{0}中奖{1:N2}元,扣除奖金盈利提成金额{2:N2}元,实得奖金{3:N2}元.", schemeId, order.AfterTaxBonusMoney, commissionMoney, realBonusMoney));
                            }
                            else
                            {
                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, order.AfterTaxBonusMoney,
                                    string.Format("中奖奖金{0:N2}元.", order.AfterTaxBonusMoney));
                            }
                        }

                        if (order.AddMoney > 0)
                            BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, order.AddMoney,
                                                        string.Format("订单{0}活动赠送{1:N2}元.", schemeId, order.AddMoney));
                        #endregion
                    }
                    if (order.SchemeType == (int)SchemeType.TogetherBetting)
                    {
                        #region 合买
                        var main = manager.QuerySports_Together(schemeId);
                        if (order.AfterTaxBonusMoney > 0)
                        {
                            //提成
                            var deductMoney = 0M;
                            if (order.AfterTaxBonusMoney > main.TotalMoney)
                                deductMoney = (order.AfterTaxBonusMoney - main.TotalMoney) * main.BonusDeduct / 100;
                            //提成金额，只能给合买发起者
                            if (deductMoney > 0M)
                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Deduct, order.UserId, schemeId, deductMoney,
                                    string.Format("订单{0}， 合买奖金盈利提成金额{1:N2}元。", schemeId, deductMoney));

                            //中奖金额,分发到所有参与合买的用户的奖金账户
                            var bonusMoney = order.AfterTaxBonusMoney - deductMoney;
                            var singleMoney = bonusMoney / main.TotalCount;
                            foreach (var join in manager.QuerySports_TogetherSucessJoin(schemeId))
                            {
                                //if (join.JoinType == TogetherJoinType.SystemGuarantees) continue;//20151015屏蔽原代码，如果合买有系统保底，依然将奖金返还到系统保底账上
                                //发参与奖金
                                if (join.RealBuyCount <= 0)
                                    continue;
                                var joinMoney = join.RealBuyCount * singleMoney;
                                //派钱
                                if (joinMoney > 0M)
                                    BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, join.JoinUserId, schemeId, joinMoney,
                                        string.Format("中奖分成，奖金￥{0:N2}元。", joinMoney));
                            }
                        }
                        if (order.AddMoney > 0M)
                        {
                            //加奖金额分配给发起者
                            if (order.DistributionWay == (int)AddMoneyDistributionWay.CreaterOnly)
                            {
                                //加奖
                                if (order.AddMoney > 0)
                                    BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Activity, order.UserId, schemeId, order.AddMoney,
                                        string.Format("订单{0}活动赠送{1:N2}元。", schemeId, order.AddMoney), RedBagCategory.Activity);
                            }
                            //处理加奖
                            if (order.DistributionWay == (int)AddMoneyDistributionWay.Average)
                            {
                                var addMonesinglePrice = order.AddMoney / main.TotalCount;
                                foreach (var join in manager.QuerySports_TogetherSucessJoin(schemeId))
                                {
                                    if (join.JoinType == (int)TogetherJoinType.SystemGuarantees) continue;

                                    if (join.RealBuyCount <= 0)
                                        continue;
                                    //发参与奖金
                                    var joinMoney = join.RealBuyCount * addMonesinglePrice;
                                    //派钱
                                    if (joinMoney > 0M)
                                        BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Activity, join.JoinUserId, schemeId, joinMoney,
                                            string.Format("订单{0}活动赠送{1:N2}元。", schemeId, joinMoney), RedBagCategory.Activity);
                                }
                            }
                            //加奖金额分配给发起者
                            if (order.DistributionWay == (int)AddMoneyDistributionWay.JoinerOnly)
                            {
                                //订单发起者没有加奖
                                var joinList = manager.QuerySports_TogetherSucessJoin(schemeId);
                                var createrList = joinList.Where(p => p.JoinUserId == order.UserId).ToList();
                                var createJoinCount = createrList.Count == 0 ? 0 : createrList.Sum(p => p.RealBuyCount);
                                var addMonesinglePrice = order.AddMoney / (main.TotalCount - createJoinCount);
                                foreach (var join in joinList)
                                {
                                    if (join.JoinType == (int)TogetherJoinType.SystemGuarantees) continue;
                                    if (join.JoinUserId == order.UserId) continue;

                                    if (join.RealBuyCount <= 0)
                                        continue;
                                    //发参与奖金
                                    var joinMoney = join.RealBuyCount * addMonesinglePrice;
                                    //派钱
                                    if (joinMoney > 0M)
                                        BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Activity, join.JoinUserId, schemeId, joinMoney,
                                            string.Format("订单{0}活动赠送{1:N2}元。", schemeId, joinMoney), RedBagCategory.Activity);
                                }
                            }
                        }
                        #endregion
                    }

                    //添加最新中奖记录表
                    if (afterTaxBonusMoney >= 2000M)
                    {
                        var userInfo = new UserBalanceManager().QueryUserRegister(order.UserId);
                        if (userInfo != null)
                        {
                            new SiteActivityManager().AddLotteryNewBonus(new E_LotteryNewBonus
                            {
                                AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                                Amount = order.Amount,
                                CreateTime = order.ComplateDateTime,
                                GameCode = order.GameCode,
                                GameType = order.GameType,
                                IssuseNumber = order.IssuseNumber,
                                PlayType = order.PlayType,
                                PreTaxBonusMoney = order.PreTaxBonusMoney,
                                SchemeId = order.SchemeId,
                                TotalMoney = order.TotalMoney,
                                UserDisplayName = userInfo.DisplayName,
                                HideUserDisplayNameCount = userInfo.HideDisplayNameCount
                            });
                        }
                    }
                }

                DB.Commit();


                //刷新用户在Redis中的余额
                if (afterTaxBonusMoney > 0M)
                    BusinessHelper.RefreshRedisUserBalance(userId);
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
              
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("EXEC_Plugin_LotteryPrizeMoneyBusiness_Error_", type, ex);
            }
            return null;
        }
    }
}
