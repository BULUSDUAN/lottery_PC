using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChinaRailway.Common;
using ChinaRailway.Payment.Models;
using ChinaRailway.Payment.Models.Charge;
using ChinaRailway.Payment.Models.Charge.BankEasy;

namespace ChinaRailway.Payment.Tasks
{
    public class ChargeTask : BaseTask
    {
        public ChargeTask(ulong appid, DataSerializer serializer, DataSignature signer, string gateway_url) : base(appid, serializer, signer, gateway_url)
        {

        }

        public ServiceRequestResult Charge(Charge request, out ChargeResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeBank(ChargeBank request, out ChargeBankResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeBankEasy(ChargeBankEasy request, out ChargeBankEasyResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeAlipayMicro(ChargeAlipayMicro request, out ChargeAlipayMicroResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeAlipayNative(ChargeAlipayNative request, out ChargeAlipayNativeResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeAlipayApp(ChargeAlipayApp request, out ChargeAlipayAppResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeWepayMicro(ChargeWepayMicro request, out ChargeWepayMicroResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeWepayNative(ChargeWepayNative request, out ChargeWepayNativeResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeWepayApp(ChargeWepayApp request, out ChargeWepayAppResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeQQpayMicro(ChargeQQpayMicro request, out ChargeQQpayMicroResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeQQpayNative(ChargeQQpayNative request, out ChargeQQpayNativeResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }

        public ServiceRequestResult ChargeQQpayApp(ChargeQQpayApp request, out ChargeQQpayAppResult result)
        {
            return ServiceRequest(EventNames.Charge, request, out result);
        }


        public ServiceRequestResult QueryCharge(QueryCharge request, out QueryChargeResult result)
        {
            return ServiceRequest(EventNames.Charge_Query, request, out result);
        }

        public ServiceRequestResult RefundCharge(RefundCharge request, out RefundChargeResult result)
        {
            return ServiceRequest(EventNames.Charge_Refund, request, out result);
        }

        public ServiceRequestResult QueryRefundCharge(QueryRefundCharge request, out QueryRefundChargeResult result)
        {
            return ServiceRequest(EventNames.Charge_Refund_Query, request, out result);
        }

        public ServiceRequestResult EasyBankCardAdd(BankCardAdd request, out BankCardAddResult result)
        {
            return ServiceRequest(EventNames.Charge_Easy_BankCard_Add, request, out result);
        }

        public ServiceRequestResult EasyBankCardAddConfirm(BankCardAddConfirm request, out BankCardAddConfirmResult result)
        {
            return ServiceRequest(EventNames.Charge_Easy_BankCard_Add_Confirm, request, out result);
        }

        public ServiceRequestResult EasyBankCardQuery(BankCardQuery request, out BankCardQueryResult result)
        {
            return ServiceRequest(EventNames.Charge_Easy_BankCard_Query, request, out result);
        }

        public ServiceRequestResult EasyBankCardRemove(BankCardRemove request, out BankCardRemoveResult result)
        {
            return ServiceRequest(EventNames.Charge_Easy_BankCard_Remove, request, out result);
        }
    }
}
