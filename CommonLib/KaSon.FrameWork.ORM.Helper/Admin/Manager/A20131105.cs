using System;
using System.Collections.Generic;
using KaSon.FrameWork.Common;
using EntityModel;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 优惠券相关
    /// </summary>
    public class A20131105 : DBbase
    {
        /// <summary>
        /// 生成优惠券
        /// </summary>
        public void BuildCoupons(string summary, decimal money, int count)
        {
            DB.Begin();

            var activityManager = new A20131105Manager();

            for (int i = 0; i < count; i++)
            {
                var md5 = Encipherment.MD5(Guid.NewGuid().ToString());
                var codeList = new List<string>();
                var t = 0;
                for (int j = 0; j < md5.Length; j++)
                {
                    if (t == 4 && codeList.Count < 19)
                    {
                        codeList.Add("-");
                        t = 0;
                    }
                    if (j % 2 == 0)
                    {
                        t++;
                        codeList.Add(md5[j].ToString());
                    }
                }
                var code = string.Join("", codeList.ToArray()).ToUpper();

                activityManager.AddA20131105_优惠券(new A20131105_优惠券
                {
                    CreateTime = DateTime.Now,
                    BelongUserId = string.Empty,
                    CanUsable = true,
                    Money = money,
                    Summary = summary,
                    Number = code,
                });
            }

            DB.Commit();
        }
        /// <summary>
        /// 使用优惠券
        /// </summary>
        public void ExchangeCoupons(string userId, string couponsNumber)
        {
            DB.Begin();

            var activityManager = new A20131105Manager();
            var entity = activityManager.QueryA20131105_优惠券(couponsNumber);
            if (entity == null)
                throw new Exception(string.Format("优惠券{0}不存在", couponsNumber));

            if (!entity.CanUsable)
                throw new Exception(string.Format("优惠券{0}已使用", couponsNumber));

            var old = activityManager.QueryA20131105_优惠券(entity.Summary, userId);
            if (old != null)
                throw new Exception("同一类型优惠券只能使用一张");

            entity.CanUsable = false;
            entity.BelongUserId = userId;
            activityManager.UpdateA20131105_优惠券(entity);

            //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_Activity, couponsNumber, couponsNumber, false, "", "", userId, AccountType.Common, entity.Money, string.Format("优惠券{0}兑换{1:N2}元", couponsNumber, entity.Money));
            BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, userId, couponsNumber, entity.Money,
                string.Format("优惠券{0}兑换{1:N2}元", couponsNumber, entity.Money), RedBagCategory.Activity);

            DB.Commit();
        }


        /// <summary>
        /// 查询优惠券
        /// </summary>
        public A20131105CouponsInfoCollection QueryCouponsList(string summary, bool? canUsable, string belongUserId, int pageIndex, int pageSize)
        {
            var result = new A20131105CouponsInfoCollection();
            var totalCount = 0;
            var activityManager = new A20131105Manager();
            var list = activityManager.QueryCouponsList(summary, canUsable, belongUserId, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            result.List.AddRange(list);

            return result;
        }
    }
}


