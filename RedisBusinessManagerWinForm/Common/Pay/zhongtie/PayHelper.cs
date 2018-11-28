using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using Common.Cryptography;
using System.IO;
using System.Net;

namespace FancyOne.Payment
{
    public static class PayHelper
    {
        public static ulong LocalDateTimeToUnixTimeStamp(DateTime date)
        {
            DateTime UTCOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (ulong)Math.Floor((date.ToUniversalTime() - UTCOrigin).TotalMilliseconds);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string getmd5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
            //return Encipherment.MD5(string.Format("{0}", str));
        }


        public static string HttpPost(string POSTURL, string PostData)
        {
            //发送请求的数据
            //发送请求的数据
            WebRequest myHttpWebRequest = WebRequest.Create(POSTURL);
            myHttpWebRequest.Method = "POST";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(PostData);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            //发送成功后接收返回的XML信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            return lcHtml;
        }


        public static string GenerateRequest(string order, ulong bank, ushort currency, decimal amount, string product, string callback_front, string callback_push, string identity = null, string remark = null)
        {
            return GenerateRequest(order, (ulong)(ConvertDateTimeInt(DateTime.Now)), bank, currency, amount, product, callback_front, callback_push, identity, remark);
        }

        public static string GenerateRequest(string order, ulong time, ulong bank, ushort currency, decimal amount, string product, string callback_front, string callback_push, string identity = null, string remark = null)
        {
            var count = 8;
            var sb = new StringBuilder();

            sb.AppendFormat("|1={0}",order);
            sb.AppendFormat("|2={0}",time);
            sb.AppendFormat("|3={0}",bank);
            sb.AppendFormat("|4={0}",currency);
            sb.AppendFormat("|5={0}",amount);
            sb.AppendFormat("|6={0}",product);
            sb.AppendFormat("|7={0}",callback_front.Replace("|", "&or;").Replace("=", "&equal;"));
            sb.AppendFormat("|8={0}",callback_push.Replace("|", "&or;").Replace("=", "&equal;"));

            if (!string.IsNullOrEmpty(identity))
            {
                ++count;
                sb.AppendFormat("|9={0}",identity);
            }

            if (!string.IsNullOrEmpty(remark))
            {
                ++count;
                sb.AppendFormat("|10={0}",remark);
            }

            return string.Format("{0}{1}",count,sb);
        }

        public static string GenerateResponse(string order, ulong bank, ushort currency, decimal amount, ushort status, string identity = null, string remark = null)
        {
            var count = 5;
            var sb = new StringBuilder();

            sb.AppendFormat("|1={0}",order);
            sb.AppendFormat("|2={0}",bank);
            sb.AppendFormat("|3={0}",currency);
            sb.AppendFormat("|4={0}",amount);
            sb.AppendFormat("|5={0}",status);

            if (!string.IsNullOrEmpty(identity))
            {
                ++count;
                sb.AppendFormat("|6={0}",identity);
            }

            if (!string.IsNullOrEmpty(remark))
            {
                ++count;
                sb.AppendFormat("|7={0}",remark);
            }

            return string.Format("{0}{1}",count,sb);
        }

        public static string GenerateResponsePush(string pushid, string order, ulong bank, ushort currency, decimal amount, ushort status, string identity = null, string remark = null)
        {
            var count = 6;
            var sb = new StringBuilder();

            sb.AppendFormat("|1={0}",pushid);
            sb.AppendFormat("|2={0}",order);
            sb.AppendFormat("|3={0}",bank);
            sb.AppendFormat("|4={0}",currency);
            sb.AppendFormat("|5={0}",amount);
            sb.AppendFormat("|6={0}",status);

            if (!string.IsNullOrEmpty(identity))
            {
                ++count;
                sb.AppendFormat("|7={0}",identity);
            }

            if (!string.IsNullOrEmpty(remark))
            {
                ++count;
                sb.AppendFormat("|8={0}",remark);
            }

            return string.Format("{0}{1}",count,sb);
        }

        public static uint GetRequest(string data, out DataRequest request)
        {
            var data_array = data.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            ushort data_array_count = 0;
            if (!ushort.TryParse(data_array[0], out data_array_count) || (data_array_count != data_array.Length - 1))
            {
                request = null;
                return 1;
            }

            var result = new DataRequest();

            for (int i = 1; i <= data_array_count; i++)
            {
                var request_item = data_array[i].Split('=');
                if (request_item.Length != 2)
                {
                    request = null;
                    return 2;
                }

                ushort request_item_id;
                if (!ushort.TryParse(request_item[0], out request_item_id))
                {
                    request = null;
                    return 3;
                }

                switch (request_item_id)
                {
                    case 1: // order
                        {
                            var order = request_item[1];
                            if (string.IsNullOrEmpty(order))
                            {
                                request = null;
                                return 5;
                            }

                            result.Order = order;
                        }
                        break;
                    case 2: // time
                        {
                            ulong timestamp;
                            if (!ulong.TryParse(request_item[1], out timestamp))
                            {
                                request = null;
                                return 6;
                            }

                            result.Time = GetTime((long)timestamp);
                        }
                        break;
                    case 3: // bank
                        {
                            ulong bank;
                            if (!ulong.TryParse(request_item[1], out bank))
                            {
                                request = null;
                                return 7;
                            }

                            result.Bank = bank;
                        }
                        break;
                    case 4: // currency
                        {
                            ushort currency;
                            if (!ushort.TryParse(request_item[1], out currency))
                            {
                                request = null;
                                return 8;
                            }

                            result.Currency = currency;
                        }
                        break;
                    case 5: // amount
                        {
                            decimal amount;
                            if (!decimal.TryParse(request_item[1], out amount))
                            {
                                request = null;
                                return 9;
                            }

                            result.Amount = Math.Round(amount, 2);
                        }
                        break;
                    case 6: // product
                        {
                            var product = request_item[1];
                            if (string.IsNullOrEmpty(product))
                            {
                                request = null;
                                return 10;
                            }

                            result.Product = product;
                        }
                        break;
                    case 7: // callback_front
                        {
                            var callback_front = request_item[1];
                            if (string.IsNullOrEmpty(callback_front))
                            {
                                request = null;
                                return 11;
                            }

                            result.CallbackFront = callback_front.Replace("&or;", "|").Replace("&equal;", "=");
                        }
                        break;
                    case 8: // callback_push
                        {
                            var callback_push = request_item[1];
                            if (string.IsNullOrEmpty(callback_push))
                            {
                                request = null;
                                return 12;
                            }

                            result.CallbackPush = callback_push.Replace("&or;", "|").Replace("&equal;", "=");
                        }
                        break;
                    case 9: // identity
                        {
                            result.Identity = request_item[1];
                        }
                        break;
                    case 10: // remark
                        {
                            result.Remark = request_item[1];
                        }
                        break;
                    default:
                        {
                            request = null;
                            return 4;
                        }
                }
            }

            if (result.Order == null || result.Time == null || result.Bank == null || result.Currency == null || result.Amount == null || result.Product == null || result.CallbackFront == null || result.CallbackPush == null)
            {
                request = null;
                return 13;
            }

            request = result;
            return 0;
        }

        public static uint GetResponse(string data, out DataResponse response)
        {
            var data_array = data.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            ushort data_array_count = 0;
            if (!ushort.TryParse(data_array[0], out data_array_count) || (data_array_count != data_array.Length - 1))
            {
                response = null;
                return 1;
            }

            var result = new DataResponse();

            for (int i = 1; i <= data_array_count; i++)
            {
                var request_item = data_array[i].Split('=');
                if (request_item.Length != 2)
                {
                    response = null;
                    return 2;
                }

                ushort request_item_id;
                if (!ushort.TryParse(request_item[0], out request_item_id))
                {
                    response = null;
                    return 3;
                }

                switch (request_item_id)
                {
                    case 1: // order
                        {
                            var order = request_item[1];
                            if (string.IsNullOrEmpty(order))
                            {
                                response = null;
                                return 5;
                            }

                            result.Order = order;
                        }
                        break;
                    case 2: // bank
                        {
                            ulong bank;
                            if (!ulong.TryParse(request_item[1], out bank))
                            {
                                response = null;
                                return 6;
                            }

                            result.Bank = bank;
                        }
                        break;
                    case 3: // currency
                        {
                            ushort currency;
                            if (!ushort.TryParse(request_item[1], out currency))
                            {
                                response = null;
                                return 7;
                            }

                            result.Currency = currency;
                        }
                        break;
                    case 4: // amount
                        {
                            decimal amount;
                            if (!decimal.TryParse(request_item[1], out amount))
                            {
                                response = null;
                                return 8;
                            }

                            result.Amount = Math.Round(amount, 2);
                        }
                        break;
                    case 5: // status
                        {
                            ushort status;
                            if (!ushort.TryParse(request_item[1], out status))
                            {
                                response = null;
                                return 9;
                            }

                            result.Status = status;
                        }
                        break;
                    case 6: // identity
                        {
                            result.Identity = request_item[1];
                        }
                        break;
                    case 7: // remark
                        {
                            result.Remark = request_item[1];
                        }
                        break;
                    default:
                        {
                            response = null;
                            return 4;
                        }
                }
            }

            if (result.Order == null || result.Bank == null || result.Currency == null || result.Amount == null || result.Status == null)
            {
                response = null;
                return 10;
            }

            response = result;
            return 0;
        }

        public static uint GetResponsePush(string data, out DataResponsePush response)
        {
            var data_array = data.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            ushort data_array_count = 0;
            if (!ushort.TryParse(data_array[0], out data_array_count) || (data_array_count != data_array.Length - 1))
            {
                response = null;
                return 1;
            }

            var result = new DataResponsePush();

            for (int i = 1; i <= data_array_count; i++)
            {
                var request_item = data_array[i].Split('=');
                if (request_item.Length != 2)
                {
                    response = null;
                    return 2;
                }

                ushort request_item_id;
                if (!ushort.TryParse(request_item[0], out request_item_id))
                {
                    response = null;
                    return 3;
                }

                switch (request_item_id)
                {
                    case 1: // pushid
                        {
                            var pushid = request_item[1];
                            if (string.IsNullOrEmpty(pushid))
                            {
                                response = null;
                                return 5;
                            }

                            result.PushId = pushid;
                        }
                        break;
                    case 2: // order
                        {
                            var order = request_item[1];
                            if (string.IsNullOrEmpty(order))
                            {
                                response = null;
                                return 6;
                            }

                            result.Order = order;
                        }
                        break;
                    case 3: // bank
                        {
                            ulong bank;
                            if (!ulong.TryParse(request_item[1], out bank))
                            {
                                response = null;
                                return 7;
                            }

                            result.Bank = bank;
                        }
                        break;
                    case 4: // currency
                        {
                            ushort currency;
                            if (!ushort.TryParse(request_item[1], out currency))
                            {
                                response = null;
                                return 8;
                            }

                            result.Currency = currency;
                        }
                        break;
                    case 5: // amount
                        {
                            decimal amount;
                            if (!decimal.TryParse(request_item[1], out amount))
                            {
                                response = null;
                                return 9;
                            }

                            result.Amount = Math.Round(amount, 2);
                        }
                        break;
                    case 6: // status
                        {
                            ushort status;
                            if (!ushort.TryParse(request_item[1], out status))
                            {
                                response = null;
                                return 10;
                            }

                            result.Status = status;
                        }
                        break;
                    case 7: // identity
                        {
                            result.Identity = request_item[1];
                        }
                        break;
                    case 8: // remark
                        {
                            result.Remark = request_item[1];
                        }
                        break;
                    default:
                        {
                            response = null;
                            return 4;
                        }
                }
            }

            if (result.PushId == null || result.Order == null || result.Bank == null || result.Currency == null || result.Amount == null || result.Status == null)
            {
                response = null;
                return 11;
            }

            response = result;
            return 0;
        }

        public static uint HashData(ushort encryption, ulong vendor, string data, string hashkey, out string sign)
        {
            switch (encryption)
            {
                case 1:
                case 2:
                    {
                        sign = HashDataMD5(vendor, data, hashkey);
                        return 0;
                    }
                default:
                    sign = null;
                    return 1;
            }
        }

        public static string HashDataMD5(ulong vendor, string data, string hashkey)
        {
            //return DigestHelper.MD5($"{vendor}&{data}&{hashkey}");
            return Encipherment.MD5(string.Format("{0}&{1}&{2}", vendor, data, hashkey));
        }

        public static uint EncryptData(ushort encryption, ulong vendor, string data, string hashkey, string enckey, out string sign, out string encdata)
        {
            switch (encryption)
            {
                case 1:
                    {
                        if (string.IsNullOrEmpty(hashkey))
                        {
                            sign = null;
                            encdata = null;
                            return 1;
                        }

                        encdata = DataEncryptHelper.Encode(data);
                        sign = HashDataMD5(vendor, encdata, hashkey);
                        return 0;
                    }
                default:
                    {
                        sign = null;
                        encdata = null;
                        return 3;
                    }
            }
        }

        public static uint DecryptData(ushort encryption, ulong vendor, string data, string hashkey, string enckey, string sign, out string decdata)
        {
            switch (encryption)
            {
                case 1:
                    {
                        if (string.IsNullOrEmpty(hashkey))
                        {
                            decdata = null;
                            return 1;
                        }

                        var sign_verify = HashDataMD5(vendor, data, hashkey);
                        if (!sign_verify.Equals(sign, StringComparison.OrdinalIgnoreCase))
                        {
                            decdata = null;
                            return 4;
                        }

                        decdata = DataEncryptHelper.Decode(data);
                        return 0;
                    }
                default:
                    {
                        decdata = null;
                        return 3;
                    }
            }
        }


         /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int ConvertDateTimeInt(System.DateTime time)  
        {  
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));  
            return (int)(time - startTime).TotalSeconds;  
        }  


        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(long timeStamp)  
        {  
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));  
            //long lTime = long.Parse(timeStamp + "0000000");  
            TimeSpan toNow = new TimeSpan(timeStamp);  
            return dtStart.Add(toNow);  
        }  
    }
}
