using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ChinaRailway.Common;
using ChinaRailway.Payment.Tasks;

namespace ChinaRailway.Payment
{
    public class ChinaRailwayApp
    {
        private static ChinaRailwayApp _instance;
        private static readonly object obj = new object();
        public static ChinaRailwayApp GetInstance(ulong id, string key,string gateway_url)
        {
            if (null == _instance)
            {
                lock (obj)
                {
                    if (null == _instance)
                    {
                        _instance = new ChinaRailwayApp(id, key, gateway_url);
                    }
                }
            }
            return _instance;
        }

        protected readonly ulong app_id;

        protected readonly ChargeTask order_task;
        protected readonly PayTask payment_task;
        protected readonly TransferTask transfer_task;
        protected readonly WebHookTask webhook_task;
        protected readonly Serializer serializer;

        public ChargeTask Charge
        {
            get { return this.order_task; }
        }
        public PayTask Pay
        {
            get { return this.payment_task; }
        }
        public TransferTask Transfer
        {
            get { return this.transfer_task; }
        }
        public WebHookTask WebHook
        {
            get { return this.webhook_task; }
        }

        public Serializer Serializer
        {
            get { return this.serializer; }
        }

        static readonly RS256 data_verifier;
        static readonly HS256 data_hs256;

        static ChinaRailwayApp()
        {
            var platform_key = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCaB+w+RfL0iuRJT9y0+bgfi8jmatTpQY74vvLmtNa45Yaq2+rs7FdAyVomRralll411kKuYaCqNB3mIUGTT3tCq+c0PIkXk+aAPlUgehRy3FozcFuzO1i6ofq1xs+rJg5XtodX7G+A3rmpUMJ2vexv68rRovBvJKxRkDJsG7BvbQIDAQAB";
            {
                data_verifier = new RS256();
                data_verifier.SetPublicKey(platform_key);
            }

            var platform_hs_key = "xiZgYsxagQWmtW2cNPA2L9hZ299jGa7wpEiL2SV75OA=";
            {
                data_hs256 = new HS256(Convert.FromBase64String(platform_hs_key));
            }
        }

        public ChinaRailwayApp(ulong id, string key,string gateway_url)
        {
            this.app_id = id;

            {
                var data_serializer = new DataSerializer(new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new Smartunicom.Runtime.Serialization.ContractResolver.JsonSnakeCasePropertyNamesContractResolver()
                });

                var data_signer = new RS256();
                data_signer.SetPrivateKey(key);

                var data_signer_all = new DataSignature(data_signer, data_verifier, data_hs256);

                {
                    this.order_task = new ChargeTask(this.app_id, data_serializer, data_signer_all, gateway_url);
                    this.payment_task = new PayTask(this.app_id, data_serializer, data_signer_all, gateway_url);
                    this.transfer_task = new TransferTask(this.app_id, data_serializer, data_signer_all, gateway_url);
                    this.webhook_task = new WebHookTask(this.app_id, data_serializer, data_signer_all, gateway_url);
                    this.serializer = new Serializer(data_serializer);
                }
            }
        }
    }
}
