using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class TransferTask : BaseTask
    {
        public TransferTask(ulong appid, DataSerializer serializer, DataSignature signer, string gateway_url) : base(appid, serializer, signer, gateway_url)
        {

        }

        public ServiceRequestResult Transfer(Transfer request, out TransferResult result)
        {
            return ServiceRequest(EventNames.Transfer, request, out result);
        }

        public ServiceRequestResult QueryTransfer(QueryTransfer request, out QueryTransferResult result)
        {
            return ServiceRequest(EventNames.Transfer_Query, request, out result);
        }

        public ServiceRequestResult RefundTransfer(RefundTransfer request, out RefundTransferResult result)
        {
            return ServiceRequest(EventNames.Transfer_Refund, request, out result);
        }

        public ServiceRequestResult QueryRefundTransfer(QueryRefundTransfer request, out QueryRefundTransferResult result)
        {
            return ServiceRequest(EventNames.Transfer_Refund_Query, request, out result);
        }
    }
}
