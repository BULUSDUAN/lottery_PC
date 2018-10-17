using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Communication;

namespace KaSon.FrameWork.ORM.Helper.Admin
{
   public class AdminPartService
    {
        public A20131105CouponsInfoCollection QueryCouponsList(string summary, bool? canUsable, string belongUserId, int pageIndex, int pageSize, string userToken)
        {
            var result = new A20131105CouponsInfoCollection();
            var totalCount = 0;
            var activityManager = new A20131105Manager();
            var list = activityManager.QueryCouponsList(summary, canUsable, belongUserId, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            result.List.AddRange(list);
            return result;
        }
        /// <summary>
        /// 生成优惠券
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="money"></param>
        /// <param name="count"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult BuildCoupons(string summary, decimal money, int count, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new A20131105().BuildCoupons(summary, money, count);
                return new CommonActionResult(true, "生成完成");
            }
            catch (Exception ex)
            {
                throw new Exception("生成优惠券出错 - " + ex.ToString());
            }
        }
    }
}
