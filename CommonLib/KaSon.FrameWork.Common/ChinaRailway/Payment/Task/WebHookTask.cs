using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class WebHookTask : BaseTask
    {
        public WebHookTask(ulong appid, DataSerializer serializer, DataSignature signer,string gateway_url) : base(appid, serializer, signer, gateway_url)
        {

        }

        public bool CheckSignature(IDictionary<string, string> header, string content, string sign)
        {
            var sign_packet = new DataPacket(header, content);
            return this.signer.Verify(sign_packet.SignaturePacket, sign);
        }
    }
}
