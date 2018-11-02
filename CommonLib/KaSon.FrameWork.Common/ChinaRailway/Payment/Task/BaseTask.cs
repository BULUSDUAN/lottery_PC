using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common.ChinaRailway;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class ServiceRequestResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class BaseTask
    {
        protected readonly ulong appid;

        protected readonly DataSerializer serializer;
        protected readonly DataSignature signer;
        protected readonly string gateway_url;
        //protected static readonly string gateway_url = "http://pay.shunli18.com/gateway";//zhuanyefu
        //protected static readonly string gateway_url = "http://pay.zhuanyefu.com/gateway";

        public BaseTask(ulong appid, DataSerializer serializer, DataSignature signer, string gateway_url)
        {
            this.appid = appid;
            this.serializer = serializer;
            this.signer = signer;
            this.gateway_url = gateway_url;
        }

        protected ServiceRequestResult ServiceRequest<T, TResult>(string event_name, T request, out TResult result)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request_data = this.serializer.SerializeString<T>(request);

                    client.DefaultRequestHeaders.TryAddWithoutValidation("ChinaRailway-Application", this.appid.ToString());
                    client.DefaultRequestHeaders.TryAddWithoutValidation("ChinaRailway-Event", event_name);

                    var sign_packet_header = new Dictionary<string, string>();
                    sign_packet_header.Add("Content-Type", "application/json");
                    sign_packet_header.Add("ChinaRailway-Application", this.appid.ToString());
                    sign_packet_header.Add("ChinaRailway-Event", event_name);

                    var sign_packet = new DataPacket(sign_packet_header, request_data);
                    var sign = this.signer.Sign(sign_packet.SignaturePacket);

                    client.DefaultRequestHeaders.TryAddWithoutValidation("ChinaRailway-Signature", sign);

                    var response = client.PostAsync(gateway_url, new StringContent(request_data, Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var response_content = response.Content.ReadAsStringAsync().Result;

                        var result_header_Code = response.Headers.GetValues("ChinaRailway-Code").First();
                        if (result_header_Code == "0")
                        {
                            var result_header_content_type = response.Content.Headers.ContentType.MediaType;
                            var result_header_application = response.Headers.GetValues("ChinaRailway-Application").First();
                            var result_header_event = response.Headers.GetValues("ChinaRailway-Event").First();
                            var result_header_signature = response.Headers.GetValues("ChinaRailway-Signature").First();

                            if (result_header_application != this.appid.ToString())
                            {
                                result = default(TResult);
                                return new ServiceRequestResult() { IsSuccess = false, Message = "application error." };
                            }

                            if (result_header_event != event_name)
                            {
                                result = default(TResult);
                                return new ServiceRequestResult() { IsSuccess = false, Message = "event error." };
                            }

                            var result_packet_header = new Dictionary<string, string>();
                            result_packet_header.Add("Content-Type", result_header_content_type);
                            result_packet_header.Add("ChinaRailway-Application", result_header_application);
                            result_packet_header.Add("ChinaRailway-Event", result_header_event);
                            result_packet_header.Add("ChinaRailway-Code", result_header_Code);

                            var result_packet = new DataPacket(result_packet_header, response_content);
                            if (!this.signer.Verify(result_packet.SignaturePacket, result_header_signature))
                            {
                                result = default(TResult);
                                return new ServiceRequestResult() { IsSuccess = false, Message = "signature error." };
                            }

                            result = this.serializer.DeserializeString<TResult>(response_content);
                            return new ServiceRequestResult() { IsSuccess = true };
                        }
                        else
                        {
                            var result_header_Message = this.serializer.DeserializeString<ErrorResult>((string)response_content);

                            result = default(TResult);
                            return new ServiceRequestResult() { IsSuccess = false, Message = string.Format("gateway response:{0}.", result_header_Message.Message) };
                        }
                    }
                    else
                    {
                        result = default(TResult);
                        return new ServiceRequestResult() { IsSuccess = false, Message = string.Format("gateway response status code: {0}.", response.StatusCode) };
                    }
                }
            }
            catch (Exception ex)
            {
                result = default(TResult);
                return new ServiceRequestResult() { IsSuccess = false, Message = string.Format("exception: {0}.", ex.Message) };
            }
        }
    }
}
