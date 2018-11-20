using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using GameBiz.Core;
using Common.Business;
using FacaiExternal.Business;
using GameBiz.Business;
using Common;
using FacaiActivity.Domain.Entities;
using FacaiActivity.Domain.Managers;
using NHibernate.Criterion;
using FacaiExternal.Domain.Managers.AppendBonusConfig;

//各个彩种的规则加奖+彩豆
namespace FacaiActivity.Business
{
    public class A20120101_通用加奖 : IPrizeOrder_AfterTranCommit
    {
        public void PrizeOrder_AfterTranCommit(PrizeResult result)
        {
            if (!result.IsBonus)
            {
                // 未中奖
                return;
            }
            if (result.SchemeType == SchemeType.TogetherBetting)
            {
                // 合买订单不参与加奖活动
                return;
            }
            var appendMoney = 0M;
            var appendRatio = 0M;
            var appendMultiple = 1;
            var appendTotalMoney = 0M;

            var startIssueNumber = 0;
            var endIssueNumber = 500;
            var bonusMoneyStartMultiple = 0;

            ArrayList arrGameType = new ArrayList();
            ArrayList arrBonusMoneyStartMultiple = new ArrayList();
            ArrayList arrTotalBonusMoney = new ArrayList();
            ArrayList arrAppendMoney = new ArrayList();

            foreach (var item in result.BonusInfo)
            {
                //获取配置
                var config = new AppendBonusConfigManager().GetAppendBonusConfig(result.GameCode, item.GameTypeCode);

                if (config == null)
                {
                    continue;
                }

                appendMoney = config.AppendBonusMoney;
                appendRatio = config.AppendRatio;
                appendMultiple = config.StartMultiple;

                startIssueNumber = config.StartIssueNumber;
                endIssueNumber = config.EndIssueNumber;
                bonusMoneyStartMultiple = config.BonusMoneyStartMultiple;

                if (appendMoney < 0 || appendRatio < 0 || appendMultiple < 1 ||
                    startIssueNumber < 0 || endIssueNumber < 0 || bonusMoneyStartMultiple < 0)
                {
                    //配置错误直接返回
                    continue;
                }

                var currentIssueNumber = result.IssuseNumber;
                currentIssueNumber = currentIssueNumber.Substring(currentIssueNumber.IndexOf("-") + 1);

                if (Convert.ToInt32(currentIssueNumber) >= startIssueNumber && Convert.ToInt32(currentIssueNumber) <= endIssueNumber)
                {
                    if (bonusMoneyStartMultiple > 0)
                    {
                        //订单每中xx元加奖xx元              
                        int index = arrGameType.IndexOf(item.GameTypeCode);
                        if (index < 0)
                        {
                            arrGameType.Add(item.GameTypeCode);
                            arrBonusMoneyStartMultiple.Add(bonusMoneyStartMultiple);
                            arrTotalBonusMoney.Add(item.TotalMoney);
                            arrAppendMoney.Add(appendMoney);
                        }
                        else
                        {
                            var totalBonusMoney = Convert.ToDecimal(arrTotalBonusMoney[index]);
                            totalBonusMoney += item.TotalMoney;

                            arrTotalBonusMoney[index] = totalBonusMoney;
                        }
                    }
                    else
                    {
                        //每中xx倍加奖xx元
                        appendTotalMoney += item.TotalMoney * (appendRatio / 100.00M) + (appendMoney * (item.Amount / appendMultiple) * item.Count);
                    }
                }
            }

            //订单每中xx元加奖xx元
            for (int i = 0; i < arrGameType.Count; i++)
            {
                appendTotalMoney += Convert.ToDecimal(arrAppendMoney[i].ToString()) * (Convert.ToInt32(Convert.ToDecimal(arrTotalBonusMoney[i].ToString())) / Convert.ToInt32(arrBonusMoneyStartMultiple[i].ToString()));
            }

            if (appendTotalMoney <= 0M)
            {
                //没的任何加奖的，直接退出
                return;
            }

            var totalMoney = result.AfterTaxBonusMoney + appendTotalMoney;

            string displayName = Common.Business.LotteryHelper.GetDisplayNameFromGameCode(result.GameCode);

            string desc = string.Format("{0} 第{1}期 中奖:￥{2:N2}", displayName, result.IssuseNumber, result.AfterTaxBonusMoney, appendTotalMoney);

            if (appendTotalMoney > 0)
            {
                desc += string.Format("【加奖：￥{0:N2}】", appendTotalMoney);
            }

            lock (Common.Utilities.UsefullHelper.moneyLocker)
            {
                using (var biz = new FacaiActivityBusinessManagement())
                {
                    biz.BeginTran();

                    if (appendTotalMoney > 0)
                    {
                        // 添加加奖记录
                        var record = new A20120101_通用加奖_AppendBonus
                        {
                            GameCode = result.GameCode,
                            GameType = result.GameTypeList,
                            SchemeId = result.OrderId,
                            SchemeType = result.SchemeType,
                            UserId = result.UserId,
                            IssueMoney = result.IssuseMoney,
                            BonusMoney = result.AfterTaxBonusMoney,
                            AppendBonusMoney = appendTotalMoney,
                            AppendRatio = appendRatio,
                            TotalBonusMoney = totalMoney,
                            Desc = desc
                        };

                        var recordManager = new A20120101_通用加奖Manager();
                        recordManager.AddAppendBonusRecord(record);

                        // 添加加奖资金明细
                        BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_ActivityGiveMoney, result.OrderId, result.OrderId, false, "", "", result.UserId, AccountType.Bonus, appendTotalMoney
                            , string.Format("{0} 方案加奖，中奖：￥{0:N2}，加奖：￥{1:N2}", result.GameDisplayName, result.TotalBonusMoney, appendTotalMoney));
                    }

                    biz.CommitTran();
                }
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            var paraList = inputParam as object[];
            switch (type)
            {
                case "IPrizeOrder_AfterTranCommit":
                    PrizeOrder_AfterTranCommit((PrizeResult)paraList[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
            }
            return null;
        }
    }
}
