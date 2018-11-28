using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaRailway.Payment;
using System.IO;
using ChinaRailway.Payment.Models;
using ChinaRailway.Payment.Models.Charge;
using ChinaRailway.Payment.Models.Pay;


namespace Common.Pay.shunlifu
{
    public class SLF_PAY
    {
        private static SLF_PAY _instance = null;
        //private static ChinaRailwayApp _instance;
        private static readonly object obj = new object();
        private string url = "http://pay.shunli18.com/cashier?id={0}";
        public ulong AppId { get; set; }
        public string PrivateKey { get; set; }
        public string slf_payurl { get; set; }
        public string gateway_url { get; set; }
        static SLF_PAY()
        {
            // 创建SDK ChinaRailwayApp类
            // 设置商户应用ID和应用RS256私钥
        }

        //public string slf_payurl
        //{
        //    get
        //    {
        //        string defalutValue = "http://pay.shunli18.com/cashier?id={0}";
        //        //return defalutValue;
        //        try
        //        {
        //            var v = System.Configuration.ConfigurationManager.AppSettings["slf_payurl"].ToString();
        //            if (string.IsNullOrEmpty(v))
        //            {
        //                return defalutValue;
        //            }
        //            else
        //            {
        //                return v;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return defalutValue;
        //        }
        //    }
        //}


        public static SLF_PAY GetInstance()
        {
            if (null == _instance)
            {
                lock (obj)
                {
                    if (null == _instance)
                    {
                        _instance = new SLF_PAY();
                    }
                }
            }
            return _instance;
        }


        public string ChargeAlipay(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeAlipayNative()
                   {
                       Type = type,
                       Channel = channel,
                       Order = order,
                       Currency = 1,
                       Amount = amount,
                       Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                       {
                           Ip = ip,
                           Ua = Ua
                       },
                       Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                       {
                           Subject = subject,
                           Body = body,
                       },
                       Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                       Timeout = 60 * 10,
                       Remark = remark
                   };

                ChargeAlipayNativeResult response;
                var result = ChinaRailway.Payment.ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeAlipayNative(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    if (response.Credential.Type == 2)
                    {
                        return string.Format("{0}|{1}|{2}", "true", 2, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }

                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }

        public string ChargeWeixin(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeWepayNative()
                {
                    Type = type,
                    Channel = channel,
                    Order = order,
                    Currency = 1,
                    Amount = amount,
                    Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                    {
                        Ip = ip,
                        Ua = Ua
                    },
                    Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                    {
                        Subject = subject,
                        Body = body,
                    },
                    Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                    Timeout = 60 * 10,
                    Remark = remark
                };

                ChargeWepayNativeResult response;
                var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeWepayNative(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    if (response.Credential.Type == 2)
                    {
                        return string.Format("{0}|{1}|{2}", "true", 2, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }

                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }

        /// <summary>
        /// 20171201增加手机H5专用通道
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <param name="order"></param>
        /// <param name="amount"></param>
        /// <param name="ip"></param>
        /// <param name="Ua"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public string ChargeWeixinH5(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeWepayNative()
                {
                    Type = type,
                    Channel = channel,
                    Order = order,
                    Currency = 1,
                    Amount = amount,
                    Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                    {
                        Ip = ip,
                        Ua = Ua
                    },
                    Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                    {
                        Subject = subject,
                        Body = body,
                    },
                    Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                    Timeout = 60 * 10,
                    Remark = remark
                };

                ChargeWepayNativeResult response;
                var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeWepayNative(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    //if (response.Credential.Type == 2)
                    if (response.Credential.Type == 0)//手机微信H5专用通道
                    {
                        return string.Format("{0}|{1}|{2}", "true", 2, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }

                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }


        public string ChargeQQ(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeQQpayNative()
                {
                    Type = type,
                    Channel = channel,
                    Order = order,
                    Currency = 1,
                    Amount = amount,
                    Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                    {
                        Ip = ip,
                        Ua = Ua
                    },
                    Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                    {
                        Subject = subject,
                        Body = body,
                    },
                    Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                    Timeout = 60 * 10,
                    Remark = remark
                };

                ChargeQQpayNativeResult response;
                var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeQQpayNative(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    if (response.Credential.Type == 3)
                    {
                        return string.Format("{0}|{1}|{2}", "true", 3, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }

                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }

        public string ChargeUpay(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeAlipayNative()
                {
                    Type = type,
                    Channel = channel,
                    Order = order,
                    Currency = 1,
                    Amount = amount,
                    Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                    {
                        Ip = ip,
                        Ua = Ua
                    },
                    Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                    {
                        Subject = subject,
                        Body = body,
                    },
                    Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                    Timeout = 60 * 10,
                    Remark = remark
                };

                ChargeAlipayNativeResult response;
                var result = ChinaRailway.Payment.ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeAlipayNative(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    if (response.Credential.Type == 3)
                    {
                        return string.Format("{0}|{1}|{2}", "true", 3, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }

                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }

        public string Chargebank(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark, string resultUrl)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeBank()
                {
                    //Type = type,
                    //Channel = channel,
                    Order = order,
                    Currency = 1,
                    Amount = amount,
                    Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                    {
                        Ip = ip,
                        Ua = Ua
                    },
                    Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                    {
                        Subject = subject,
                        Body = body,
                    },
                    Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                    Timeout = 60 * 10,
                    Remark = remark,
                    ResultUrl = resultUrl
                };
                ChargeBankResult response;
                var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeBank(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    if (response.Credential.Type == 2)
                    {
                        return string.Format("{0}|{1}|{2}", "true", 2, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }
                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }

        /// <summary>
        /// 现在新的快捷需要上传name 开户人 cert_id 证件号  cert_type 证件类型 1 身份证
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <param name="order"></param>
        /// <param name="amount"></param>
        /// <param name="ip"></param>
        /// <param name="Ua"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="remark"></param>
        /// <param name="resultUrl"></param>
        /// <param name="no"></param>
        /// <param name="name"></param>
        /// <param name="cert_id"></param>
        /// <param name="cert_type"></param>
        /// <returns></returns>
        public string Chargeexpress(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark, string resultUrl, string no, string name, string cert_id, int cert_type, string mobile)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Charge.ChargeBankEasy(channel, ChinaRailway.Payment.Models.Charge.BankAccountType.DebitCard, no)
                {
                    //Type = type,
                    //Channel = channel,
                    Order = order,
                    Currency = 1,
                    Amount = amount,
                    Client = new ChinaRailway.Payment.Models.Charge.ChargeClient()
                    {
                        Ip = ip,
                        Ua = Ua
                    },
                    Product = new ChinaRailway.Payment.Models.Charge.ChargeProduct()
                    {
                        Subject = subject,
                        Body = body,
                    },
                    Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                    Timeout = 60 * 10,
                    Remark = remark,
                    ResultUrl = resultUrl,
                    name = name,
                    cert_id = cert_id,
                    cert_type = cert_type,
                    mobile = mobile
                };
                ChargeBankEasyResult response;
                var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Charge.ChargeBankEasy(request, out response); // 调用接口生成订单

                if (result.IsSuccess)
                {
                    if (response.Credential.Type == 2)
                    {
                        return string.Format("{0}|{1}|{2}", "true", 2, response.Credential.Content);
                    }
                    else
                    {
                        return string.Format("{0}|{1}|{2}", "true", 1, response.Credential.Content);
                        //return string.Format("{0}|{1}|{2}", "true", 1, string.Format(slf_payurl, response.Transaction));//response.Credential.Content
                    }
                    //return string.Format("{0}|{1}", "true", response.Credential.Content);
                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }

        public string ChargePay(ushort type, ushort channel, string order, decimal amount, string ip, string Ua, string subject, string body, string remark, string name, string no)
        {
            try
            {
                var request = new ChinaRailway.Payment.Models.Pay.PayBank(channel, ChinaRailway.Payment.Models.Pay.BankAccountType.DebitCard, name, no) // 指定银行编号，卡类型开户人和银行卡号
                    {
                        Order = order,
                        Currency = 1,
                        Amount = amount,
                        Product = new ChinaRailway.Payment.Models.Pay.PayProduct()
                        {
                            Subject = subject,
                            Body = body,
                        },
                        Time = Smartunicom.DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),
                        Timeout = 60 * 10,
                        Client = new ChinaRailway.Payment.Models.Pay.PayClient()
                        {
                            Ip = ip,
                            Ua = Ua
                        },
                        Remark = remark
                    };

                PayBankResult response;
                //var app = new ChinaRailway.Payment.ChinaRailwayApp(AppId, PrivateKey);
                //var result = app.Pay.PayBank(request, out response); // 调用接口订单信息
                var result = ChinaRailway.Payment.ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Pay.PayBank(request, out response); // 调用接口订单信息
                if (result.IsSuccess)
                {
                    return string.Format("{0}|{1}", "true", response.Status);
                }
                else
                {
                    return string.Format("{0}|{1}", "false", result.Message);
                }
            }
            catch (Exception exp)
            {
                return string.Format("{0}|{1}", "false", exp.Message);
            }
        }


        public int CheckSignature(Dictionary<string, string> header, string request_content, string request_header_signature, string request_header_event, out decimal money, out string orderid, out string outorderid, out string remark)
        {

            money = 0;
            orderid = string.Empty;
            outorderid = string.Empty;
            remark = string.Empty;
            if (!ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).WebHook.CheckSignature(header, request_content, request_header_signature))
            {

                return 500;
            }

            int id = 0;
            switch (request_header_event)
            {
                case EventNames.Charge_Succeeded: // 支付成功通知
                    {
                        var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Serializer.Deserialize<ChargeSucceeded>(request_content);
                        if (result.Status != 1)
                        {
                            return 500;
                        }

                        //todo: 获取到订单信息 转由商户系统处理
                        // 注意金额变动时的并发问题
                        money = result.Amount;
                        orderid = result.Order;
                        outorderid = result.Transaction;
                        remark = result.Remark;
                    }
                    return 204;
                case EventNames.Pay_Succeeded: // 代付 - 成功通知
                    {
                        var result = ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url).Serializer.Deserialize<PaySucceeded>(request_content);
                        if (result.Status != 1)
                        {
                            return 500;
                        }

                        //todo: 获取到订单信息 转由商户系统处理
                        // 注意金额变动时的并发问题
                        money = result.Amount;
                        orderid = result.Order;
                        outorderid = result.Transaction;
                        remark = result.Remark;
                    }
                    return 204;
                default:
                    {
                        return 404;
                    }
            }
        }

        /// <summary>
        /// 代付订单查询
        /// </summary>
        public string QueryPay(string orderId)
        {
            var request = new ChinaRailway.Payment.Models.Pay.QueryPay()
            {
                Order = orderId
            };
            QueryPayResult response;
            var app=ChinaRailway.Payment.ChinaRailwayApp.GetInstance(AppId, PrivateKey, gateway_url);
            var result = app.Pay.QueryPay(request, out response); // 调用接口订单信息
            if (result.IsSuccess)
            {
                return string.Format("true|{0}",response.Status);
            }
            else
            {
                return "false|查询失败";
            }
        }

        public string GetBody(Stream body)
        {
            return body.ReadAsString();
        }


        public string GetBankNo(string BankName)
        {
            string no = "0";
            switch (BankName)
            {
                case "中国工商银行":
                case "工商银行":
                    no = "10001";
                    break;
                case "中国农业银行":
                case "农业银行":
                    no = "10002";
                    break;
                case "中国银行":
                    no = "10003";
                    break;
                case "中国建设银行":
                case "建设银行":
                    no = "10004";
                    break;
                case "交通银行":
                case "中国交通银行":
                    no = "10005";
                    break;
                case "邮政储蓄银行":
                case "中国邮政储蓄银行":
                    no = "10006";
                    break;
                case "中信银行":
                case "中国中信银行":
                    no = "11001";
                    break;
                case "中国光大银行":
                case "光大银行":
                    no = "11002";
                    break;
                case "华夏银行":
                case "中国华夏银行":
                    no = "11003";
                    break;
                case "中国民生银行":
                case "民生银行":
                    no = "11004";
                    break;
                case "中国招商银行":
                case "招商银行":
                    no = "11005";
                    break;
                case "兴业银行":
                case "中国兴业银行":
                    no = "11006";
                    break;
                case "广发银行":
                case "中国广发银行":
                    no = "11007";
                    break;
                case "平安银行":
                case "中国平安银行":
                    no = "11008";
                    break;
                case "上海浦东发展银行":
                    no = "11009";
                    break;
                case "恒丰银行":
                case "中国恒丰银行":
                    no = "11010";
                    break;
                case "浙商银行":
                    no = "11011";
                    break;
                case "渤海银行":
                    no = "11012";
                    break;
                case "北京银行":
                    no = "12001";
                    break;
                case "上海银行":
                    no = "12040";
                    break;
                case "南京银行":
                    no = "12042";
                    break;
                case "杭州银行":
                    no = "12045";
                    break;
                case "温州银行":
                    no = "12046";
                    break;
                case "宁波银行":
                    no = "12055";
                    break;
                case "北京农村商业银行":
                    no = "13001";
                    break;
                case "上海农商银行":
                    no = "13018";
                    break;
                case "河北银行":
                    no = "12003";
                    break;
                case "宁夏银行":
                    no = "12120";
                    break;
                case "厦门银行":
                    no = "12130";
                    break;
                case "青岛银行":
                    no = "12132";
                    break;
                case "江苏银行":
                    no = "12041";
                    break;
                case "苏州银行":
                    no = "12044";
                    break;
                case "徽商银行":
                    no = "12056";
                    break;
                case "九江银行":
                    no = "12060";
                    break;
                case "上饶银行":
                    no = "12062";
                    break;
                case "齐鲁银行":
                    no = "12064";
                    break;
                case "齐商银行":
                    no = "12065";
                    break;
                case "日照银行":
                    no = "12070";
                    break;
                case "汉口银行":
                    no = "12083";
                    break;
                case "广州银行":
                    no = "12086";
                    break;
                case "东莞银行":
                    no = "12089";
                    break;
                case "重庆农村商业银行":
                    no = "13125";
                    break;
                case "深圳农村商业银行":
                    no = "13137";
                    break;
            }
            return no;
        }



    }
}
