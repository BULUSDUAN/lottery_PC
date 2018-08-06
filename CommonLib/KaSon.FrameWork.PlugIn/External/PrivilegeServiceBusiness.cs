using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.ORM.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.PlugIn.External
{
    /// <summary>
    /// 特权与服务
    /// </summary>
    public class PrivilegeServiceBusiness :DBbase, IComplateTicket
    {
        /// <summary>
        /// 每购彩100元增加成长值
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            DB.Begin();
            try
            {
               

                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                if (orderDetail == null || orderDetail.SchemeType == (int)SchemeType.SaveScheme || orderDetail.SchemeType == (int)SchemeType.SingleTreasure || orderDetail.IsVirtualOrder)
                    return;

                //查询用户信息
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(orderDetail.UserId);
                //赠送成长值
                var growthValue = GrowthValue(user.VipLevel, totalMoney);

                var currGrowth = manager.GetCurrDayGiveGrowth(userId);
                if (currGrowth >= 100)//每天赠送的成长值不能大于100
                    return;
                var sumGrowth = growthValue + currGrowth;
                if (sumGrowth > 100)
                    growthValue = 100 - currGrowth;

                if (growthValue > 0M)
                    BusinessHelper.Payin_UserGrowth(BusinessHelper.FundCategory_BuyGrowthValue, schemeId, user.UserId, (int)Math.Round(growthValue), string.Format("购彩赠送成长值:{0}", (int)Math.Round(growthValue)));

                //赠送豆豆
                var doudou = GetDoudou(user.VipLevel, totalMoney);
                if (doudou > 0M)
                    BusinessHelper.Payin_OCDouDou(BusinessHelper.FundCategory_BuyDouDou, schemeId, user.UserId, (int)Math.Round(doudou), string.Format("购彩赠送豆豆:{0}", (int)Math.Round(doudou)));

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
             
            
        }

        /// <summary>
        /// 得到成长值
        /// </summary>
        private decimal GrowthValue(int vipLevel, decimal totalMoney)
        {
            decimal growthValue = 0;
            switch (vipLevel)
            {
                case 1:
                    growthValue = totalMoney * 20 / 100;
                    break;
                case 2:
                    growthValue = totalMoney * 30 / 100;
                    break;
                case 3:
                    growthValue = totalMoney * 40 / 100;
                    break;
                case 4:
                    growthValue = totalMoney * 50 / 100;
                    break;
                case 5:
                    growthValue = totalMoney * 60 / 100;
                    break;
                case 6:
                    growthValue = totalMoney * 80 / 100;
                    break;
                case 7:
                    growthValue = totalMoney * 100 / 100;
                    break;
                case 8:
                    growthValue = totalMoney * 100 / 100;
                    break;
                case 10:
                    growthValue = totalMoney * 100 / 100;
                    break;
            }
            return growthValue;
        }

        /// <summary>
        /// 得到豆豆
        /// </summary>
        private decimal GetDoudou(int vipLevel, decimal totalMoney)
        {
            decimal doudou = 0;
            switch (vipLevel)
            {
                case 1:
                case 2:
                    doudou = 0;
                    break;
                case 3:
                case 4:
                    doudou = totalMoney * 25 / 100;
                    break;
                case 5:
                case 6:
                    doudou = totalMoney * 50 / 100;
                    break;
                case 7:
                case 8:
                    doudou = totalMoney * 75 / 100;
                    break;
                case 9:
                    doudou = totalMoney * 100 / 100;
                    break;
            }
            return doudou;
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (Exception ex)
            {
               
                //("EXEC_Plugin_Error_", type, ex);
            }
            return null;
        }
    }
}
