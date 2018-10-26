using System;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 代理积分业务
    /// </summary>
    public class ExternalIntegralBusiness:DBbase
    {
        /// <summary>
        /// 完成提现
        /// 修改提现记录，清理冻结资金
        /// </summary>
        public void CompleteWithdraw(string orderId, string responseMsg, string opUserId)
        {
                DB.Begin();
                var fundManager = new FundManager();
                var entity = fundManager.QueryWithdraw(orderId);
                if (entity.Status !=(int)WithdrawStatus.Requesting)
                {
                    throw new Exception("该条信息提取状态不能进行完成操作 - " + entity.Status);
                }
                entity.Status = (int)WithdrawStatus.Success;
                entity.ResponseTime = DateTime.Now;
                //entity.ResponseMoney = entity.RequestMoney;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = responseMsg;
                entity.ResponseUserId = opUserId;
                fundManager.UpdateWithdraw(entity);
                //清理冻结
                 BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_IntegralCompleteWithdraw, entity.UserId, orderId
                    , string.Format("完成积分提取，积分{0:N2}：{1}", entity.RequestMoney, responseMsg), entity.RequestMoney);
                DB.Commit();
        }

        /// <summary>
        /// 拒绝提现
        /// 修改提现记录，退还冻结资金
        /// </summary>
        public void RefusedWithdraw(string orderId, string refusedMsg, string userId)
        {
            DB.Begin();
            var fundManager = new FundManager(); ;
            var entity = fundManager.QueryWithdraw(orderId);
            if (entity.Status != (int)WithdrawStatus.Requesting)
            {
                throw new Exception("该条信息提现状态不能进行拒绝操作 - " + entity.Status);
            }
            entity.Status = (int)WithdrawStatus.Refused;
            entity.ResponseMoney = 0M;
            entity.ResponseTime = DateTime.Now;
            entity.ResponseMessage = refusedMsg;
            entity.ResponseUserId = userId;
            fundManager.UpdateWithdraw(entity);
            // 返还资金
            BusinessHelper.Payin_FrozenBack(BusinessHelper.FundCategory_IntegralRefusedWithdraw, entity.UserId, orderId, entity.RequestMoney, string.Format("拒绝提取积分，返还积分{0:N2}：{1}", entity.RequestMoney, refusedMsg));
            DB.Commit();
        }
    }
}
