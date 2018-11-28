using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Business;
using Activity.Domain.Managers;
using External.Core.Authentication;
using GameBiz.Domain.Managers;
using Common.Communication;
using Activity.Domain.Entities;
using Activity.Core;

namespace Activity.Business
{
    public class A20140731_DouDouBusiness
    {

        /// <summary>
        /// 豆豆兑换奖品
        /// </summary>
        public void ExchangeDouDou(string userId, int doudou, string password)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var userBalance = new UserBalanceManager().QueryUserBalance(userId);
                if (userBalance.CurrentDouDou < doudou)
                    throw new Exception(string.Format("当前豆豆不足:{0}，请继续努力！", doudou));

                var order = Guid.NewGuid().ToString("N");
                var manager = new A20140731_DouDouManager();
                switch (doudou)
                {
                    case 1000:
                        //减去现金45元，加上奖金账户50元。
                        BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 45M, string.Format("用户{0}兑换红包", userId),
                            "ExchangeDouDou", password);
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆1000", userId));

                        //赠送50红包
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 50M, string.Format("用户{0}兑换50元红包", userId));

                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                        {
                            UserId = userId,
                            DouDou = doudou,
                            Money = 45M,
                            ActivityMoney = 50M,
                            Prize = "购彩金",
                            PrizeMoney = 5M,
                            IsGive = true,
                            CreateTime = DateTime.Now,
                        });
                        break;
                    case 1800:
                        //减去现金90元，加上奖金账户100元。
                        BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 90M, string.Format("用户{0}兑换红包", userId), "ExchangeDouDou", password);
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆1800", userId));

                        //赠送100红包
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 100M, string.Format("用户{0}兑换100元红包", userId));

                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                          {
                              UserId = userId,
                              DouDou = doudou,
                              Money = 90M,
                              ActivityMoney = 100M,
                              Prize = "购彩金",
                              PrizeMoney = 10M,
                              IsGive = true,
                              CreateTime = DateTime.Now,
                          });
                        break;
                    case 15000:
                        //减去现金900元，加上奖金账户1000元红包。
                        BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 900M, string.Format("用户{0}兑换红包", userId), "ExchangeDouDou", password);
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆15000", userId));
                        //赠送1000红包
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 1000M, string.Format("用户{0}兑换1000元红包", userId));

                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                          {
                              UserId = userId,
                              DouDou = doudou,
                              Money = 900M,
                              ActivityMoney = 1000M,
                              Prize = "购彩金",
                              PrizeMoney = 100M,
                              IsGive = true,
                              CreateTime = DateTime.Now,
                          });
                        break;
                    case 150000:
                        //加上奖金账户1000元红元。
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 1000M, string.Format("用户{0}兑换1000元红包", userId));
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆150000", userId));

                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                          {
                              UserId = userId,
                              DouDou = doudou,
                              Money = 0M,
                              ActivityMoney = 1000M,
                              Prize = "购彩金",
                              PrizeMoney = 1000M,
                              IsGive = true,
                              CreateTime = DateTime.Now,
                          });
                        break;
                    case 280000:
                        //加上奖金账户2000元红包。
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_ExchangeDouDou, userId, order, 2000M, string.Format("用户{0}兑换2000元红包", userId));
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆280000", userId));

                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                          {
                              UserId = userId,
                              DouDou = doudou,
                              Money = 0M,
                              ActivityMoney = 2000M,
                              Prize = "购彩金",
                              PrizeMoney = 2000M,
                              IsGive = true,
                              CreateTime = DateTime.Now,
                          });
                        break;
                    case 500000:
                        //IPAD
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆500000", userId));
                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                        {
                            UserId = userId,
                            DouDou = doudou,
                            Money = 0M,
                            ActivityMoney = 0M,
                            Prize = "IPAD",
                            PrizeMoney = 0M,
                            IsGive = false,
                            CreateTime = DateTime.Now,
                        });
                        break;
                    case 1000000:
                        //Mac Book Air。
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆1000000", userId));
                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                        {
                            UserId = userId,
                            DouDou = doudou,
                            Money = 0M,
                            ActivityMoney = 0M,
                            Prize = "Mac Book Air",
                            PrizeMoney = 0M,
                            IsGive = false,
                            CreateTime = DateTime.Now,
                        });

                        break;
                    case 5000000:
                        //奇瑞QQ。
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆5000000", userId));
                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                        {
                            UserId = userId,
                            DouDou = doudou,
                            Money = 0M,
                            ActivityMoney = 0M,
                            Prize = "奇瑞QQ",
                            PrizeMoney = 0M,
                            IsGive = false,
                            CreateTime = DateTime.Now,
                        });

                        break;
                    case 10000000:
                        //比亚迪。
                        //扣除豆豆
                        BusinessHelper.Payout_OCDouDou(BusinessHelper.FundCategory_ExchangeDouDou, order, userId, doudou, string.Format("用户{0}兑换红包，消除豆豆10000000", userId));
                        manager.AddA20140731_DouDou(new A20140731_DouDou()
                        {
                            UserId = userId,
                            DouDou = doudou,
                            Money = 0M,
                            ActivityMoney = 0M,
                            Prize = "比亚迪",
                            PrizeMoney = 0M,
                            IsGive = false,
                            CreateTime = DateTime.Now,
                        });

                        break;

                    default:
                        break;
                }


                biz.CommitTran();
            }
        }

        /// <summary>
        /// 查询豆豆列表
        /// </summary>
        public A20140731_DouDouInfoCollection QueryDouDouList(bool isGive, int pageIndex, int pageSize)
        {
            var result = new A20140731_DouDouInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new A20140731_DouDouManager().QueryDouDouList(isGive, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 修改领取奖品状态
        /// </summary>
        public void ToGetThePrize(int id, string userId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new A20140731_DouDouManager();
                var entity = manager.QueryDouDouById(id);
                if (entity == null)
                {
                    throw new Exception("修改信息未被查询到");
                }
                if (entity.IsGive == true)
                    throw new Exception("该奖品已领取");
                entity.IsGive = true;
                manager.UpdateA20140731_DouDou(entity);

                biz.CommitTran();
            }
        }

    }


}
