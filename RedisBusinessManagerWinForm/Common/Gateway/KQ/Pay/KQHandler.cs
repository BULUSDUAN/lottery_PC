using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace Common.Gateway.KQ.Pay
{
    /// <summary>
    /// 快钱支付 共公接口函数
    /// </summary>
    public class KQHandler
    {
        #region 功能函数
        /// <summary>
        /// 请求签名加密并返回加密签名串
        /// </summary>
        /// <param name="OriginalString">被加密字符串</param>
        /// <param name="prikey_path">商户私钥证书路径</param>
        /// <param name="CertificatePW">商户私钥密钥</param>
        /// <param name="SignType"></param>
        /// <returns></returns>
        public static string CerRSASignature(string OriginalString, string prikey_path, string CertificatePW, int SignType)
        {
            byte[] OriginalByte = Encoding.UTF8.GetBytes(OriginalString);
            // X509Certificate2 x509_Cer1 = new X509Certificate2(prikey_path, CertificatePW);
            //X509Certificate2 cert = new X509Certificate2(HttpContext.Current.Server.MapPath("tester-rsa.pfx"), "abby_7796", X509KeyStorageFlags.MachineKeySet);
            X509Certificate2 cert = new X509Certificate2(prikey_path, CertificatePW, X509KeyStorageFlags.MachineKeySet);
            // RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)x509_Cer1.PrivateKey;
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            switch (SignType)
            {
                case 1:
                    f.SetHashAlgorithm("MD5");//摘要算法MD5
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    result = md5.ComputeHash(OriginalByte);//摘要值
                    break;
                default:
                    f.SetHashAlgorithm("SHA1");//摘要算法SHA1
                    SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                    result = sha.ComputeHash(OriginalByte);//摘要值
                    break;
            }
            string SignData = System.Convert.ToBase64String(f.CreateSignature(result)).ToString();

            return SignData;
        }

        /// <summary>
        /// 引用证书非对称加/解密RSA-公钥验签【OriginalString：原文；SignatureString：签名字符；pubkey_path：证书路径；CertificatePW：证书密码；SignType：签名摘要类型（1：MD5，2：SHA1）】
        /// </summary>
        public static bool CerRSAVerifySignature(string OriginalString, string SignatureString, string pubkey_path, string CertificatePW, int SignType)
        {
            byte[] OriginalByte = System.Text.Encoding.UTF8.GetBytes(OriginalString);
            byte[] SignatureByte = Convert.FromBase64String(SignatureString);
            X509Certificate2 x509_Cer1 = new X509Certificate2(pubkey_path, CertificatePW);
            RSACryptoServiceProvider rsapub = (RSACryptoServiceProvider)x509_Cer1.PublicKey.Key;
            rsapub.ImportCspBlob(rsapub.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapub);
            byte[] HashData;
            switch (SignType)
            {
                case 1:
                    f.SetHashAlgorithm("MD5");//摘要算法MD5
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    HashData = md5.ComputeHash(OriginalByte);
                    break;
                default:
                    f.SetHashAlgorithm("SHA1");//摘要算法SHA1
                    SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                    HashData = sha.ComputeHash(OriginalByte);
                    break;
            }
            if (f.VerifySignature(HashData, SignatureByte))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证签名是否正确 - MD5加密校验
        /// </summary>
        public static bool MD5VerfitySignatur(string merchantSignMsgVal, string signMsg)
        {
            try
            {
                String merchantSignMsg = GetMD5(merchantSignMsgVal, "utf-8");
                return signMsg.ToUpper() == merchantSignMsg.ToUpper();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 功能函数。将变量值不为空的参数组成字符串
        /// </summary>
        /// <param name="returnStr"></param>
        /// <param name="paramId"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string appendParam(string returnStr, string paramId, string paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                {
                    returnStr += "&" + paramId + "=" + paramValue;
                }
            }
            else
            {
                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }
            return returnStr;
        }

        //功能函数。将字符串进行编码格式转换，并进行MD5加密，然后返回。开始
        private static string GetMD5(string dataStr, string codeType)
        {
            dataStr = dataStr + "&key=" + KQConfig.CertificatePW;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(codeType).GetBytes(dataStr));
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 设置商户信息
        /// </summary>
        /// <param name="merchantAcctId">人民币网关账户号</param>
        /// <param name="certificatePW">商户私钥密钥</param>
        public static void SetKQConfig(string merchantAcctId, string certificatePW)
        {
            KQConfig.MerchantAcctId = merchantAcctId;
            KQConfig.CertificatePW = certificatePW;
        }
        #endregion

        #region 获取TR3回传数据，发送TR4数据方法及验签函数
        private static object lockobj = new object();
        public static string mg = ""; //定义公共消息变量

        #region 公共接口
        /// <summary>
        /// TR4发送数据
        /// </summary>
        public void SendTR4(string reqXml)
        {
            lock (lockobj)
            {
                try
                {
                    //将TR4数据response到MAS
                    byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(reqXml);
                    HttpResponse hr = HttpContext.Current.Response;
                    hr.Clear();
                    hr.OutputStream.Write(bytes, 0, bytes.Length);
                    hr.OutputStream.Close();
                }
                catch
                {
                }

            }
        }
        /// <summary>
        /// 获取TR3返回的xml数据
        /// </summary>
        public string GetTR3()
        {
            lock (lockobj)
            {
                try
                {
                    //获取Proxy转发的TR3数据
                    Stream s = HttpContext.Current.Request.InputStream;
                    byte[] buffer = new byte[1024];//无符号 8 位整数数组
                    int count = 0;
                    StringBuilder builder = new StringBuilder();//表示可变字符字符串。

                    while ((count = s.Read(buffer, 0, 1024)) > 0)//每次读取1024个字节
                    {
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                    }
                    string sXML = builder.ToString();
                    return sXML;
                }
                catch (Exception e)
                {
                    string sStr = e.Message.ToString();
                    return sStr; //返回错误
                }
            }
        }
        #endregion

        #region 验签函数
        ///<summary>
        ///功能函数。证书验签：返回值是bool型，true表示成功，false表示失败（参数1：要验证的字符串，2：要验证的签名，3：证书文件路径，4：证书密码）
        ///<summary>
        public static bool SHA1Verify(string str_VerifySign, string signMsg, string pubkey_path, string cer_pw)
        {
            //对数据进行验签
            X509Certificate2 x509_Cer2 = new X509Certificate2(pubkey_path, cer_pw);
            RSACryptoServiceProvider rsapub = (RSACryptoServiceProvider)x509_Cer2.PublicKey.Key;
            rsapub.ImportCspBlob(rsapub.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapub);
            f.SetHashAlgorithm("SHA1");
            byte[] key = Convert.FromBase64String(signMsg);
            SHA1Managed sha = new SHA1Managed();
            byte[] name = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str_VerifySign));
            if (f.VerifySignature(name, key))//验签成功
            {
                return true;
            }
            else//验签失败
            {
                return false;
            }
        }
        //功能函数。证书验签结束
        #endregion

        #region 3DES加密/解密函数
        ///<summary>
        ///功能函数。
        /// 3DES加密
        ///<summary>
        public static string Encrypt3DES(string a_strString, string a_strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey);
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(a_strString);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        ///<summary>
        ///功能函数。
        /// 3DES解密
        ///<summary>
        public static string Decrypt3DES(string a_strString, string a_strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey);
            DES.Mode = CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            string result = "";
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            try
            {
                byte[] Buffer = Convert.FromBase64String(a_strString);
                result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch
            {

            }
            return result;
        }
        #endregion
        #endregion

        /// <summary>
        /// 快钱 - 构造请求字符串
        /// </summary>
        /// <param name="prikey_path">商户私钥证书路径</param>
        /// <param name="payerName">支付人姓名，可为中文或英文字符ID</param>
        /// <param name="payerContact">支付人联系方式，只能选择Email或手机号</param>
        /// <param name="out_trade_no">商户订单ID</param>
        /// <param name="productId">商品代码.可为字符或者数字ID</param>
        /// <param name="productName">商品名称，可为中文或英文字符</param>
        /// <param name="productDesc">商品描述</param>
        /// <param name="total_fee">订单金额，以分为单位，必须是整型数字。比方2，代表0.02元</param>
        /// <param name="return_url">服务器接受支付结果的后台地址.与[pageUrl]不能同时为空。必须是绝对地址。快钱通过服务器连接的方式将交易结果发送到[bgUrl]对应的页面地址，在商户处理完成后输出的<result>如果为1，页面会转向到<redirecturl>对应的地址。如果快钱未接收到<redirecturl>对应的地址，快钱将把支付结果post到[pageUrl]对应的页面。</param>
        /// <param name="notify_url">接受支付结果的页面地址.与[bgUrl]不能同时为空。必须是绝对地址。如果[bgUrl]为空，快钱将支付结果Post到[pageUrl]对应的地址。如果[bgUrl]不为空，并且[bgUrl]页面指定的<redirecturl>地址不为空，则转向到<redirecturl>对应的地址</param>
        /// <param name="orderTime">订单提交时间，14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]。如：20080101010101</param>
        /// <param name="payType">支付方式.固定选择值，只能选择00、10、11、12、13、14。00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）</param>
        /// <param name="redoFlag">同一订单禁止重复提交标志。固定选择值： 1、0。1代表同一订单号只允许提交1次；0表示同一订单号在没有支付成功的前提下可重复提交多次。默认为0建议实物购物车结算类商户采用0；虚拟产品类商户采用1</param>
        /// <param name="bankId">银行代码.实现直接跳转到银行页面去支付,只在payType=10时才需设置参数。具体代码参见 接口文档银行代码列表</param>
        /// <param name="pid">快钱的合作伙伴的账户号。如未和快钱签订代理合作协议，不需要填写本参数。</param>
        /// <param name="ext1">扩展字段1.在支付结束后原样返回给商户ID</param>
        /// <param name="ext2">扩展字段2.在支付结束后原样返回给商户ID</param>
        /// <param name="payerContactType">支付人联系方式类型.固定选择值，只能选择1，1代表Email</param>
        /// <param name="productNum">商品数量,可为空，非空时必须为数字</param>
        /// <param name="isTest">表示是否为测试状态，测试状态调用的测试网关接口</param>
        /// <returns></returns>
        public static string GreateRequestUrl(string prikey_path, string payerName, string payerContact, string out_trade_no, string productId, string productName, string productDesc, string total_fee, string return_url, string notify_url, string orderTime, string payType = "00", string redoFlag = "1", string bankId = "", string pid = "", string ext1 = "", string ext2 = "", string payerContactType = "1", string productNum = "", bool isTest = false)
        {
            //生成加密签名串
            //请务必按照如下顺序和规则组成加密串！
            string signMsgVal = "";
            signMsgVal = appendParam(signMsgVal, "inputCharset", KQConfig.InputCharset);
            signMsgVal = appendParam(signMsgVal, "pageUrl", return_url);
            signMsgVal = appendParam(signMsgVal, "bgUrl", notify_url);
            signMsgVal = appendParam(signMsgVal, "version", KQConfig.Version);
            signMsgVal = appendParam(signMsgVal, "language", KQConfig.Language);
            signMsgVal = appendParam(signMsgVal, "signType", KQConfig.SignType);
            signMsgVal = appendParam(signMsgVal, "merchantAcctId", KQConfig.MerchantAcctId);
            signMsgVal = appendParam(signMsgVal, "payerName", payerName);
            signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
            signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
            signMsgVal = appendParam(signMsgVal, "orderId", out_trade_no);
            signMsgVal = appendParam(signMsgVal, "orderAmount", total_fee);
            signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
            signMsgVal = appendParam(signMsgVal, "productName", productName);
            signMsgVal = appendParam(signMsgVal, "productNum", productNum);
            signMsgVal = appendParam(signMsgVal, "productId", productId);
            signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
            signMsgVal = appendParam(signMsgVal, "ext1", ext1);
            signMsgVal = appendParam(signMsgVal, "ext2", ext2);
            signMsgVal = appendParam(signMsgVal, "payType", payType);
            signMsgVal = appendParam(signMsgVal, "bankId", bankId);
            signMsgVal = appendParam(signMsgVal, "redoFlag", redoFlag);
            signMsgVal = appendParam(signMsgVal, "pid", pid);


            string signMsg = CerRSASignature(signMsgVal, prikey_path, KQConfig.CertificatePW, 2);

            #region 参数编码组成访问字符串
            string urlParam = "";
            foreach (var item in signMsgVal.Split('&'))
            {
                var paraItem = item.Split('=');
                if (urlParam == "")
                {
                    urlParam = paraItem[0] + "=" + HttpUtility.UrlEncode(paraItem[1], Encoding.UTF8);
                }
                else
                {
                    urlParam = urlParam + "&" + paraItem[0] + "=" + HttpUtility.UrlEncode(paraItem[1], Encoding.UTF8);
                }
            }
            #endregion

            string url = KQConfig.Gateway_Request(isTest) + "?" + urlParam + "&signMsg=" + HttpUtility.UrlEncode(signMsg, Encoding.UTF8);
            return url;
        }

        /// <summary>
        /// 快钱 - 构造请求字符串 - MD5加密方式
        /// </summary>
        /// <param name="payerName">支付人姓名，可为中文或英文字符ID</param>
        /// <param name="payerContact">支付人联系方式，只能选择Email或手机号</param>
        /// <param name="out_trade_no">商户订单ID</param>
        /// <param name="productId">商品代码.可为字符或者数字ID</param>
        /// <param name="productName">商品名称，可为中文或英文字符</param>
        /// <param name="productDesc">商品描述</param>
        /// <param name="total_fee">订单金额，以分为单位，必须是整型数字。比方2，代表0.02元</param>
        /// <param name="return_url">服务器接受支付结果的后台地址.与[pageUrl]不能同时为空。必须是绝对地址。快钱通过服务器连接的方式将交易结果发送到[bgUrl]对应的页面地址，在商户处理完成后输出的<result>如果为1，页面会转向到<redirecturl>对应的地址。如果快钱未接收到<redirecturl>对应的地址，快钱将把支付结果post到[pageUrl]对应的页面。</param>
        /// <param name="notify_url">接受支付结果的页面地址.与[bgUrl]不能同时为空。必须是绝对地址。如果[bgUrl]为空，快钱将支付结果Post到[pageUrl]对应的地址。如果[bgUrl]不为空，并且[bgUrl]页面指定的<redirecturl>地址不为空，则转向到<redirecturl>对应的地址</param>
        /// <param name="orderTime">订单提交时间，14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]。如：20080101010101</param>
        /// <param name="payType">支付方式.固定选择值，只能选择00、10、11、12、13、14。00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）</param>
        /// <param name="redoFlag">同一订单禁止重复提交标志。固定选择值： 1、0。1代表同一订单号只允许提交1次；0表示同一订单号在没有支付成功的前提下可重复提交多次。默认为0建议实物购物车结算类商户采用0；虚拟产品类商户采用1</param>
        /// <param name="bankId">银行代码.实现直接跳转到银行页面去支付,只在payType=10时才需设置参数。具体代码参见 接口文档银行代码列表</param>
        /// <param name="pid">快钱的合作伙伴的账户号。如未和快钱签订代理合作协议，不需要填写本参数。</param>
        /// <param name="ext1">扩展字段1.在支付结束后原样返回给商户ID</param>
        /// <param name="ext2">扩展字段2.在支付结束后原样返回给商户ID</param>
        /// <param name="payerContactType">支付人联系方式类型.固定选择值，只能选择1，1代表Email</param>
        /// <param name="productNum">商品数量,可为空，非空时必须为数字</param>
        /// <param name="isTest">表示是否为测试状态，测试状态调用的测试网关接口</param>
        /// <returns></returns>
        public static string GreateRequestUrl_MD5(string payerName, string payerContact, string out_trade_no, string productId, string productName, string productDesc, string total_fee, string return_url, string notify_url, string orderTime, string payType = "00", string redoFlag = "1", string bankId = "", string pid = "", string ext1 = "", string ext2 = "", string payerContactType = "1", string productNum = "", bool isTest = false)
        {
            //生成加密签名串
            //请务必按照如下顺序和规则组成加密串！
            string signMsgVal = "";
            signMsgVal = appendParam(signMsgVal, "inputCharset", KQConfig.InputCharset);
            signMsgVal = appendParam(signMsgVal, "pageUrl", return_url);
            signMsgVal = appendParam(signMsgVal, "bgUrl", notify_url);
            signMsgVal = appendParam(signMsgVal, "version", KQConfig.Version);
            signMsgVal = appendParam(signMsgVal, "language", KQConfig.Language);
            signMsgVal = appendParam(signMsgVal, "signType", KQConfig.SignType);
            signMsgVal = appendParam(signMsgVal, "merchantAcctId", KQConfig.MerchantAcctId);
            signMsgVal = appendParam(signMsgVal, "payerName", payerName);
            signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
            signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
            signMsgVal = appendParam(signMsgVal, "orderId", out_trade_no);
            signMsgVal = appendParam(signMsgVal, "orderAmount", total_fee);
            signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
            signMsgVal = appendParam(signMsgVal, "productName", productName);
            signMsgVal = appendParam(signMsgVal, "productNum", productNum);
            signMsgVal = appendParam(signMsgVal, "productId", productId);
            signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
            signMsgVal = appendParam(signMsgVal, "ext1", ext1);
            signMsgVal = appendParam(signMsgVal, "ext2", ext2);
            signMsgVal = appendParam(signMsgVal, "payType", payType);
            signMsgVal = appendParam(signMsgVal, "bankId", bankId);
            signMsgVal = appendParam(signMsgVal, "redoFlag", redoFlag);
            signMsgVal = appendParam(signMsgVal, "pid", pid);

            string signMsg = GetMD5(signMsgVal, "utf-8").ToUpper();

            #region 参数编码组成访问字符串
            string urlParam = "";
            foreach (var item in signMsgVal.Split('&'))
            {
                var paraItem = item.Split('=');
                if (urlParam == "")
                {
                    urlParam = paraItem[0] + "=" + HttpUtility.UrlEncode(paraItem[1], Encoding.UTF8);
                }
                else
                {
                    urlParam = urlParam + "&" + paraItem[0] + "=" + HttpUtility.UrlEncode(paraItem[1], Encoding.UTF8);
                }
            }
            #endregion

            string url = KQConfig.Gateway_Request(isTest) + "?" + urlParam + "&signMsg=" + HttpUtility.UrlEncode(signMsg, Encoding.UTF8);
            return url;
        }

        /// <summary>
        /// 验证充值响应状态 - 证书验证
        /// </summary>
        /// <param name="Request">响应消息体</param>
        /// <param name="pubkey_path">快钱公钥证书路径</param>
        /// <param name="CertificatePW">存放公钥的证书密码,一般没有密码</param>
        /// <returns>返回三个状态：成功 success 失败 fail 异常 error</returns>
        public static string VerifyResponse(HttpRequestBase Request, string pubkey_path, string CertificatePW = "")
        {
            #region 获取响应参数
            //获取人民币网关账户号
            string merchantAcctId = Request["merchantAcctId"].ToString().Trim();

            //获取网关版本.固定值
            ///快钱会根据版本号来调用对应的接口处理程序。
            ///本代码版本号固定为v2.0
            string version = Request["version"].ToString().Trim();

            //获取语言种类.固定选择值。
            ///只能选择1、2、3
            ///1代表中文；2代表英文
            ///默认值为1
            string language = Request["language"].ToString().Trim();

            //签名类型.固定值
            ///1代表MD5签名
            ///当前版本固定为1
            string signType = Request["signType"].ToString().Trim();

            //获取支付方式
            ///值为：10、11、12、13、14
            ///00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）
            string payType = Request["payType"].ToString().Trim();

            //获取银行代码
            ///参见银行代码列表
            string bankId = Request["bankId"].ToString().Trim();

            //获取商户订单号
            string orderId = Request["orderId"].ToString().Trim();

            //获取订单提交时间
            ///获取商户提交订单时的时间.14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]
            ///如：20080101010101
            string orderTime = Request["orderTime"].ToString().Trim();

            //获取原始订单金额
            ///订单提交到快钱时的金额，单位为分。
            ///比方2 ，代表0.02元
            string orderAmount = Request["orderAmount"].ToString().Trim();

            //获取快钱交易号
            ///获取该交易在快钱的交易号
            string dealId = Request["dealId"].ToString().Trim();

            //获取银行交易号
            ///如果使用银行卡支付时，在银行的交易号。如不是通过银行支付，则为空
            string bankDealId = Request["bankDealId"].ToString().Trim();

            //获取在快钱交易时间
            ///14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]
            ///如；20080101010101
            string dealTime = Request["dealTime"].ToString().Trim();

            //获取实际支付金额
            ///单位为分
            ///比方 2 ，代表0.02元
            string payAmount = Request["payAmount"].ToString().Trim();

            //获取交易手续费
            ///单位为分
            ///比方 2 ，代表0.02元
            string fee = Request["fee"].ToString().Trim();

            //获取扩展字段1
            string ext1 = Request["ext1"].ToString().Trim();

            //获取扩展字段2
            string ext2 = Request["ext2"].ToString().Trim();

            //获取处理结果
            ///10代表 成功; 11代表 失败
            string payResult = Request["payResult"].ToString().Trim();

            //获取错误代码
            ///详细见文档错误代码列表
            string errCode = Request["errCode"].ToString().Trim();

            //获取加密签名串
            string signMsg = Request["signMsg"].ToString().Trim();
            #endregion

            //生成加密串。必须保持如下顺序。
            string merchantSignMsgVal = "";
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "merchantAcctId", merchantAcctId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "version", version);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "language", language);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "signType", signType);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "payType", payType);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "bankId", bankId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "orderId", orderId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "orderTime", orderTime);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "orderAmount", orderAmount);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "dealId", dealId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "bankDealId", bankDealId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "dealTime", dealTime);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "payAmount", payAmount);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "fee", fee);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "ext1", ext1);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "ext2", ext2);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "payResult", payResult);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "errCode", errCode);


            //商家进行数据处理，并跳转会商家显示支付结果的页面
            ///首先进行签名字符串验证

            //string pubkey_path = HttpContext.Current.Server.MapPath("") + "\\certificate\\99bill[1].cert.rsa.20140728";//快钱公钥证书路径
            //string CertificatePW = "";//存放公钥的证书密码
            if (CerRSAVerifySignature(merchantSignMsgVal, signMsg, pubkey_path, CertificatePW, 2))
            {
                switch (payResult)
                {
                    case "10":
                        /*  
                         ' 商户网站逻辑处理，比方更新订单支付状态为成功
                        ' 特别注意：只有signMsg.ToUpper() == merchantSignMsg.ToUpper()，且payResult=10，才表示支付成功！
                         * 因为快钱会重复通知这个页面，首先判断订单是否已经更新，没有更新做更新有则不做更新，
                         * 同时将返回的付款金额payamount与提交订单前的订单金额进行对比校验,如果一致则更新订单。
                        */
                        //报告给快钱处理结果，并提供将要重定向的地址。
                        return "success";
                    default:
                        return "fail";
                }
            }
            else//验签失败
            {
                return "error";
            }
        }

        /// <summary>
        /// 验证充值响应状态 - MD5密钥校验
        /// </summary>
        /// <returns>返回三个状态：成功 success 失败 fail 异常 error</returns>
        public static string VerifyResponse_MD5(HttpRequestBase Request)
        {
            #region 获取响应参数
            //获取人民币网关账户号
            string merchantAcctId = Request["merchantAcctId"].ToString().Trim();

            //获取网关版本.固定值
            ///快钱会根据版本号来调用对应的接口处理程序。
            ///本代码版本号固定为v2.0
            string version = Request["version"].ToString().Trim();

            //获取语言种类.固定选择值。
            ///只能选择1、2、3
            ///1代表中文；2代表英文
            ///默认值为1
            string language = Request["language"].ToString().Trim();

            //签名类型.固定值
            ///1代表MD5签名
            ///当前版本固定为1
            string signType = Request["signType"].ToString().Trim();

            //获取支付方式
            ///值为：10、11、12、13、14
            ///00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）
            string payType = Request["payType"].ToString().Trim();

            //获取银行代码
            ///参见银行代码列表
            string bankId = Request["bankId"].ToString().Trim();

            //获取商户订单号
            string orderId = Request["orderId"].ToString().Trim();

            //获取订单提交时间
            ///获取商户提交订单时的时间.14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]
            ///如：20080101010101
            string orderTime = Request["orderTime"].ToString().Trim();

            //获取原始订单金额
            ///订单提交到快钱时的金额，单位为分。
            ///比方2 ，代表0.02元
            string orderAmount = Request["orderAmount"].ToString().Trim();

            //获取快钱交易号
            ///获取该交易在快钱的交易号
            string dealId = Request["dealId"].ToString().Trim();

            //获取银行交易号
            ///如果使用银行卡支付时，在银行的交易号。如不是通过银行支付，则为空
            string bankDealId = Request["bankDealId"].ToString().Trim();

            //获取在快钱交易时间
            ///14位数字。年[4位]月[2位]日[2位]时[2位]分[2位]秒[2位]
            ///如；20080101010101
            string dealTime = Request["dealTime"].ToString().Trim();

            //获取实际支付金额
            ///单位为分
            ///比方 2 ，代表0.02元
            string payAmount = Request["payAmount"].ToString().Trim();

            //获取交易手续费
            ///单位为分
            ///比方 2 ，代表0.02元
            string fee = Request["fee"].ToString().Trim();

            //获取扩展字段1
            string ext1 = Request["ext1"].ToString().Trim();

            //获取扩展字段2
            string ext2 = Request["ext2"].ToString().Trim();

            //获取处理结果
            ///10代表 成功; 11代表 失败
            string payResult = Request["payResult"].ToString().Trim();

            //获取错误代码
            ///详细见文档错误代码列表
            string errCode = Request["errCode"].ToString().Trim();

            //获取加密签名串
            string signMsg = Request["signMsg"].ToString().Trim();
            #endregion

            //生成加密串。必须保持如下顺序。
            string merchantSignMsgVal = "";
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "merchantAcctId", merchantAcctId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "version", version);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "language", language);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "signType", signType);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "payType", payType);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "bankId", bankId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "orderId", orderId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "orderTime", orderTime);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "orderAmount", orderAmount);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "dealId", dealId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "bankDealId", bankDealId);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "dealTime", dealTime);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "payAmount", payAmount);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "fee", fee);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "ext1", ext1);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "ext2", ext2);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "payResult", payResult);
            merchantSignMsgVal = appendParam(merchantSignMsgVal, "errCode", errCode);


            //商家进行数据处理，并跳转会商家显示支付结果的页面
            ///首先进行签名字符串验证

            if (MD5VerfitySignatur(merchantSignMsgVal, signMsg))
            {
                switch (payResult)
                {
                    case "10":
                        /*  
                         ' 商户网站逻辑处理，比方更新订单支付状态为成功
                        ' 特别注意：只有signMsg.ToUpper() == merchantSignMsg.ToUpper()，且payResult=10，才表示支付成功！
                         * 因为快钱会重复通知这个页面，首先判断订单是否已经更新，没有更新做更新有则不做更新，
                         * 同时将返回的付款金额payamount与提交订单前的订单金额进行对比校验,如果一致则更新订单。
                        */
                        //报告给快钱处理结果，并提供将要重定向的地址。
                        return "success";
                    default:
                        return "fail";
                }
            }
            else//验签失败
            {
                return "error";
            }
        }
    }
}
