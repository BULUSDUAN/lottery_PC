using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using Lottery.AdminApi.Model.HelpModel;

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
            return new A20131105CouponsInfoCollection();
        }
    }
}
