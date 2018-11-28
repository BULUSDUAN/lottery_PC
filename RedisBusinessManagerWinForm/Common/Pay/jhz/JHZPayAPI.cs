using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Gateway.WXPay;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Common.Pay.jhz
{
    public class JHZPayAPI
    {
        private static string url = "http://zf.szjhzxxkj.com/ownPay/pay";//请求地址

        //private static string merchantNo = "500006040903";//qcw商户号
        //private static string shprivate = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAIPMWsCwfeJXvUwCESwnzBY/r0GMsYfwzelq9Irqq1OZqhyeMNvzBDnQDgzDJZePDJFknHsVCnBISBYGaXP4rg7ydjx3I1eH4lnkDzosGpzA+j4X1oXg4uL1KN1To7+V4UfloN1sKyB0VpVWOnafRJyI5hz4n2XojA6fusmG0da1AgMBAAECgYBaV6rhqFkx3IiyYDxbMxBvM8ozOXi7wTG0TY9A5ej4jj2QDlNZgXQlGNt4ng/XmsO3qeqlZ9/W5YUcK9r/FrhgHqfUQNqSziOE3pmmpOiDtAtOTex+OiMWG7/nwEHsvjprlVz/C1I2bFnOkz/lbhFJFP1zcOLqI203JJ1WVtwxhQJBAOSz5Gi+doeaLeXLJe1AJopZ/LUiPv4T6M5yNKUdagnvMl7eRvGbok+4Zul6us8xKKMPHwXyJcELBoGmUbhZ3yMCQQCTh4GLTD7h05g/5l/9GIoNiHYrtqcBzTcCCAKFFglpdlVgDTh3a0puLLRqpjA/Y57Ol47W82r15qteDkvv8HxHAkAfqlCruANNTymfsXr02Hb9nOwCYFV8dGE9hE6JtgLikT3WKMyF01ir1QpatWV8HoBT41oWRqq3icFC3jZeYgMlAkB3BF+vCCDGwJRYILuZjJ17E3b6Bw2ud2cEPYAC6+dF7JAtwByowqa2QVx/mXjc+rpYQo6avJ+yp5fidjgu5tzrAkEAlZVKLy2QsUsJ3oMC999IY6byYeLOJow1mEYAQ/2NZUC2CyKjX4cu1UHaJR1en8M8jKQ15UWK8Of/nkYfW8/LvA==";//私钥
        //private static string publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCL4nMv6qK7Lt1MzfK20LrVd/0g0pXIvV281sT16s4xIWEg/Hfv0su0MHdbTobZfHcziyO/xdmItCzkcJOIIskuC3QukNrWnt7kf1wZ1OmIMWAcS5s9wnMd0QcpDpcyfZfJvlZgFDtgJtApXvCBBVIEX65W1FnmlZ7wccO3Ca+J8QIDAQAB";

        private static string merchantNo = "500009626796";//jiang商户号
        private static string shprivate = "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAKFKKcc3EFtYcMDoGeNGipqBJFkxJLkYPV7QNXUiBDececNVHH1u+ZLoM9OhvWEVbaaJuiJfVZNIxxroamwL5M4AC5grusl8YXd0UeVdnVIA1uJ1XrNQ2qWf81DpBCokAQX/SXdW6KXx+zNRJzAlcz2LiY3NGMhP/CY5p6jseuyDAgMBAAECgYACNSKnQGCv72DzsvmPu8pv3O6jbeHDysokPxNMPB/0puafvs16BgfSc+0pq2ANMR7kFaR/jfB86JUZEC9MtUj3ZzpcJitAMXjLsiCp/tHTuxy9KWab5RnBeAE3Qwd9FtT4oUGCxRoWHpbgAGkl68NXY2vTgSqNi7+eru3dlodQCQJBAOfa7lW20EuyXhAP7cXwROBPLmQm9R4WovsxbtndZHAN/3wi5ABcOccRkgdPMvL83OymhUs2hVE9VHnONFX8bX8CQQCyFgOVr/azZf1x2p5pNQcmrLoFS+vxy42zHf1jIQfSwTwaFbrVTiSgNjubI4Y2YhI0X3yqZMUMrrbD+UWrpEr9AkA1/z/oiHYEWxsbBIqswaNY1jgIrSYDBuSYOdAGP1Bn5Gqu33VYPCJcoVPwDdrgylEtcC608JKl8/GbmJGJwQtDAkBq2pCeES3qaKjQ1sc0AzBeQUeAhBR3SZalQbpW72u9RlqkoCMxd6i8RK4xIPiXyvJ3YE/yAXLGcFKTdg8atQKVAkAlyaw5CFwJEfnBly9IlRAFGvn+eg3eWOrlnwsSBf/Zy7qXm4URw/IXoA4iFjk4BGwtggEt/eTifhQ76czZpKyg";//私钥
        private static string publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCL4nMv6qK7Lt1MzfK20LrVd/0g0pXIvV281sT16s4xIWEg/Hfv0su0MHdbTobZfHcziyO/xdmItCzkcJOIIskuC3QukNrWnt7kf1wZ1OmIMWAcS5s9wnMd0QcpDpcyfZfJvlZgFDtgJtApXvCBBVIEX65W1FnmlZ7wccO3Ca+J8QIDAQAB";



        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="requestNo"></param>
        /// <param name="amount"></param>
        /// <param name="pageUrl"></param>
        /// <param name="backUrl"></param>
        /// <param name="payMethod"></param>
        /// <returns></returns>
        public static string GetPrePayUrl(string requestNo, string amount, string pageUrl, string backUrl, string payMethod)
        {
            string payDate = LocalDateTimeToUnixTimeStamp(DateTime.Now).ToString();
            string agencyCode = "0";
            string remark1 = "remark1";
            string remark2 = "remark2";
            string remark3 = "remark3";
            string data = merchantNo + "|" + requestNo + "|" + amount + "|" + pageUrl + "|" + backUrl + "|" + payDate + "|" + agencyCode + "|" + remark1 + "|" + remark2 + "|" + remark3;//拼接数据
            string privatekey = SafeUtil.RSAPrivateKeyJava2DotNet(shprivate);
            string signature = SafeUtil.Sign(data, privatekey);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("merchantNo", merchantNo);
            dic.Add("requestNo", requestNo);
            dic.Add("amount", amount);
            dic.Add("pageUrl", pageUrl);
            dic.Add("backUrl", backUrl);
            dic.Add("payDate", payDate);
            dic.Add("payMethod", payMethod);
            dic.Add("remark1", remark1);
            dic.Add("remark2", remark2);
            dic.Add("remark3", remark3);
            dic.Add("agencyCode", agencyCode);
            dic.Add("signature", signature);

            string xml = GetSignSource(dic);

            var content = new FormUrlEncodedContent(dic);
            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url, content).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().Result;
                    var json = JObject.Parse(response);
                    string backQrCodeUrl = json["backQrCodeUrl"].Value<string>();
                    string backOrderId = json["backOrderId"].Value<string>();
                    string sign = json["sign"].Value<string>();
                    string originalString = "{\"backQrCodeUrl\":\"" + backQrCodeUrl + "\",\"backOrderId\":\"" + backOrderId + "\"}";
                    string key = SafeUtil.RSAPublicKeyJava2DotNet(publickey);
                    var boo = SafeUtil.Verify(originalString, sign, key);
                    if (boo)
                    {
                        return backQrCodeUrl;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }




        }




        public static Dictionary<string, string> GetDict(string requestNo, string amount, string pageUrl, string backUrl, string payMethod, string bankType)
        {
            string payDate = LocalDateTimeToUnixTimeStamp(DateTime.Now).ToString();
            string agencyCode = "0";
            string remark1 = "remark1";
            string remark2 = "remark2";
            string remark3 = "remark3";
            string data = merchantNo + "|" + requestNo + "|" + amount + "|" + pageUrl + "|" + backUrl + "|" + payDate + "|" + agencyCode + "|" + remark1 + "|" + remark2 + "|" + remark3;//拼接数据
            string privatekey = SafeUtil.RSAPrivateKeyJava2DotNet(shprivate);
            string signature = SafeUtil.Sign(data, privatekey);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("merchantNo", merchantNo);
            dic.Add("requestNo", requestNo);
            dic.Add("amount", amount);
            dic.Add("pageUrl", pageUrl);
            dic.Add("backUrl", backUrl);
            dic.Add("payDate", payDate);
            dic.Add("payMethod", payMethod);
            dic.Add("remark1", remark1);
            dic.Add("remark2", remark2);
            dic.Add("remark3", remark3);
            dic.Add("agencyCode", agencyCode);
            dic.Add("signature", signature);
            dic.Add("cur", "CNY");
            dic.Add("bankType", bankType);
            dic.Add("bankAccountType", "11");
            dic.Add("timeout", "10");
            return dic;
        }


        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ret"></param>
        public static string[]  Verify(string msg, string ret, string sign)
        {
            string[] arr = new string[5];
            string content = ret + "|" + msg;
            arr[1] = content;
            string key = SafeUtil.RSAPublicKeyJava2DotNet(publickey);
            bool boo = SafeUtil.Verify(content, sign, key);
            if (boo)
            {
                var retjson = JObject.Parse(ret);
                var msgjson = JObject.Parse(msg);
                string code = retjson["code"].Value<string>();
                arr[0] = code;
                string money = msgjson["money"].Value<string>();
                string no = msgjson["no"].Value<string>();
                string payNo = msgjson["payNo"].Value<string>();
                if (code == "1000")
                {
                    arr[2] = money;
                    arr[3] = no;
                    arr[4] = payNo;
                }
            }
            else
            {
                arr[0] = "0";
                arr[1] = content + "验证签名失败";
            }
            return arr;
        }


        private static ulong LocalDateTimeToUnixTimeStamp(DateTime date)
        {
            DateTime UTCOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (ulong)Math.Floor((date.ToUniversalTime() - UTCOrigin).TotalSeconds);
        }


        private static string GetSignSource(Dictionary<string, string> dict)
        {
            List<string> list = new List<string>();
            foreach (var item in dict)
            {
                list.Add(string.Format("{0}=\"{1}\"", item.Key, item.Value));
            }

            return string.Join("&", list);
        }
    }
}
