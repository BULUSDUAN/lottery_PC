using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChinaRailway.Common;
using ChinaRailway.Payment.Models;
using ChinaRailway.Payment.Models.Pay;

namespace ChinaRailway.Payment.Tasks
{
    public class PayTask : BaseTask
    {
        public PayTask(ulong appid, DataSerializer serializer, DataSignature signer,string gateway_url) : base(appid, serializer, signer, gateway_url)
        {

        }

        public ServiceRequestResult Pay(Pay request, out PayResult result)
        {
            return ServiceRequest(EventNames.Pay, request, out result);
        }

        public ServiceRequestResult PayBank(PayBank request, out PayBankResult result)
        {
            return ServiceRequest(EventNames.Pay, request, out result);
        }

        public ServiceRequestResult PayAlipayTransfer(PayAlipayTransfer request, out PayAlipayTransferResult result)
        {
            return ServiceRequest(EventNames.Pay, request, out result);
        }

        public ServiceRequestResult PayAlipayEnvelope(PayAlipayEnvelope request, out PayAlipayEnvelopeResult result)
        {
            return ServiceRequest(EventNames.Pay, request, out result);
        }

        public ServiceRequestResult PayWepayTransfer(PayWepayTransfer request, out PayWepayTransferResult result)
        {
            return ServiceRequest(EventNames.Pay, request, out result);
        }

        public ServiceRequestResult PayWepayEnvelope(PayWepayEnvelope request, out PayWepayEnvelopeResult result)
        {
            return ServiceRequest(EventNames.Pay, request, out result);
        }

        public ServiceRequestResult QueryPay(QueryPay request, out QueryPayResult result)
        {
            return ServiceRequest(EventNames.Pay_Query, request, out result);
        }

        public ServiceRequestResult RefundPay(RefundPay request, out RefundPayResult result)
        {
            return ServiceRequest(EventNames.Pay_Refund, request, out result);
        }

        public ServiceRequestResult QueryRefundPay(QueryRefundPay request, out QueryRefundPayResult result)
        {
            return ServiceRequest(EventNames.Pay_Refund_Query, request, out result);
        }
    }
}
