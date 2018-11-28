using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using External.Domain.Managers.SMSLog;

namespace External.Business
{
    /// <summary>
    /// 出票失败 发短信
    /// </summary>
    public class UpdateTicketStateBusiness : IComplateTicket
    {
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            return;
            if (string.IsNullOrEmpty(schemeId)) return;
            if (totalErrorMoney <= 0M) return;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var keyLine = string.Format("{0}{1}{2}", schemeId, totalMoney, totalErrorMoney);
                var smsManager = new SMSSendLogManager();
                var log = smsManager.QuerySMSSendLog(keyLine);
                if (log != null)
                {
                    var manager = new SchemeManager();
                    var orderDetail = manager.QueryOrderDetail(schemeId);
                    if (orderDetail != null)
                    {
                        var entity = new External.Domain.Managers.Authentication.UserMobileManager().GetUserMobile(orderDetail.UserId);
                        if (entity != null && !string.IsNullOrEmpty(entity.Mobile))
                        {
                            var content = string.Format("用户您好，由于系统繁忙，您投注的订单{0}总金额{1:N2}元，其中{2:N2}元出票失败。请登录网站查看", schemeId, totalMoney, totalErrorMoney);
                            smsManager.AddSMSSendLog(new External.Domain.Entities.SMSLog.SMSSendLog
                            {
                                CreateTime = DateTime.Now,
                                KeyLine = keyLine,
                                UserId = orderDetail.UserId,
                                Mobile = entity.Mobile,
                                Content = content,
                            });

                            var smsmres = Common.Net.SMS.SMSSenderFactory.SendSMS(entity.Mobile, content);
                        }
                    }
                }

                biz.CommitTran();
            }
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
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_Error_", type, ex);
            }
            return null;
        }
    }
}
