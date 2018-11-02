using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class DataSignature
    {
        protected readonly RS256 rs256_signer;
        protected readonly RS256 rs256_verifier;
        protected readonly HS256 hs256_hash;

        public DataSignature(RS256 signer, RS256 verifier, HS256 hash)
        {
            this.rs256_signer = signer;
            this.rs256_verifier = verifier;
            this.hs256_hash = hash;
        }

        public string Sign(byte[] packet)
        {
            var packet_hash = hs256_hash.Hash(packet);
            var packet_hash_p2 = rs256_signer.SignData(packet_hash);

            return string.Format("{0}.{1}",packet_hash,packet_hash_p2);
        }

        public bool Verify(byte[] packet, string sign)
        {
            var sign_array = sign.Split('.');
            if(sign_array.Length != 2)
            {
                return false;
            }

            var packet_hash = hs256_hash.Hash(packet);
            if(sign_array[0] != packet_hash)
            {
                return false;
            }

            if(!rs256_verifier.VerifyData(packet_hash, sign_array[1]))
            {
                return false;
            }

            return true;
        }
    }

    public class DataPacket
    {
        public DataPacket(IDictionary<string, string> headers, string body)
        {
            var headers_dic = new SortedDictionary<string, string>();
            foreach(var header in headers)
            {
                switch(header.Key)
                {
                    case "Content-Type": // 通用
                        {
                            if (!string.IsNullOrEmpty(header.Value))
                            {
                                // bugfix

                                var value = header.Value;
                                if (value == "application/json; charset=utf-8")
                                {
                                    value = "application/json";
                                }

                                headers_dic.Add("Content-Type", value);
                            }
                        }
                        break;
                    case "Authorization": // 客户端API使用
                        {
                            if (!string.IsNullOrEmpty(header.Value))
                            {
                                headers_dic.Add("Authorization", header.Value);
                            }
                        }
                        break;
                    case "ChinaRailway-Application": // 通用
                        {
                            if(!string.IsNullOrEmpty(header.Value))
                            {
                                headers_dic.Add("Application", header.Value);
                            }
                        }
                        break;
                    case "ChinaRailway-Event": // 通用
                        {
                            if (!string.IsNullOrEmpty(header.Value))
                            {
                                headers_dic.Add("Event", header.Value);
                            }
                        }
                        break;
                    case "ChinaRailway-Delivery": // 回调通知使用
                        {
                            if (!string.IsNullOrEmpty(header.Value))
                            {
                                headers_dic.Add("Delivery", header.Value);
                            }
                        }
                        break;
                    case "ChinaRailway-Code": // API响应使用
                        {
                            if (!string.IsNullOrEmpty(header.Value))
                            {
                                headers_dic.Add("Code", header.Value);
                            }
                        }
                        break;
                }
            }

            var header_querystring = string.Join("&", headers_dic.Select(i => ("" + i.Key + "=" + Uri.EscapeDataString(i.Value) + "")));

            var data_p1 = Encoding.UTF8.GetBytes(header_querystring);
            var data_p2 = Encoding.UTF8.GetBytes(body);

            SignaturePacket = new byte[data_p1.Length + data_p2.Length];
            {
                Buffer.BlockCopy(data_p1, 0, SignaturePacket, 0, data_p1.Length);
                Buffer.BlockCopy(data_p2, 0, SignaturePacket, data_p1.Length, data_p2.Length);
            }
        }

        public byte[] SignaturePacket { get; protected set; }
    }
}
