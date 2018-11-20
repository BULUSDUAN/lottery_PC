using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;

namespace Common.Net.SMS
{
    [ServiceContract]
    public class CSubmitState
    {
        public int State { get; set; }//提交结果值,返回值为0时，表示提交成功
        public string MsgID { get; set; }//提交结果ID，仅当提交成功后，此字段值才有意义
        public string MsgState { get; set; }//提交结果描述
        public int Reserve { get; set; }//保留，扩展用
    }
    [ServiceContract]
    public class CRemain
    {
        /// <summary>
        /// 状态返回值
        /// </summary>
        public int State { get; set; }//状态返回值
        /// <summary>
        /// 余额条数
        /// </summary>
        public int Remain { get; set; }//余额
    }
    [ServiceContract]
    interface ISMSService_10658
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        [OperationContract(Action = "http://tempuri.org/g_Submit")]
        CSubmitState g_Submit(string sname, string spwd, string scorpid, string sprdid, string sdst, string smsg);
        /// <summary>
        /// 接收回复XML格式 
        /// </summary>
        [OperationContract(Action = "http://tempuri.org/g_SubmitResponse")]
        string g_SubmitResponse(string g_SubmitResult);
        /// <summary>
        /// 取账号余额 
        /// </summary>
        [OperationContract(Action = "http://tempuri.org/Sm_GetRemain")]
        CRemain Sm_GetRemain(string sname, string spwd, string scorpid, string sprdid);
        /// <summary>
        /// 账号余额回复
        /// </summary>
        [OperationContract(Action = "http://tempuri.org/Sm_GetRemainResponse")]
        string Sm_GetRemainResponse(string Sm_GetRemainResult);
    }

    static class SMSServiceFactory_10658
    {
        //正式端口
        private static string ServiceURL = "http://chufa.lmobile.cn/submitdata/service.asmx";

        //临时端口
        //private static string ServiceURL = "http://dx.lmobile.cn:6003/submitdata/Service.asmx";

        public static ISMSService_10658 WebService()
        {
            return GetServiceChannel();
        }

        private static ISMSService_10658 GetServiceChannel()
        {
            System.ServiceModel.Channels.Binding bind = GetBinding();
            EndpointAddress address = new EndpointAddress(ServiceURL);
            ChannelFactory<ISMSService_10658> factory = new ChannelFactory<ISMSService_10658>(bind, address);
            ISMSService_10658 channel = factory.CreateChannel();
            return channel;
        }

        private static System.ServiceModel.Channels.Binding GetBinding()
        {
            BasicHttpBinding bind = new BasicHttpBinding(BasicHttpSecurityMode.None);
            bind.TransferMode = TransferMode.StreamedResponse;
            bind.MaxReceivedMessageSize = long.MaxValue;
            bind.MaxBufferSize = int.MaxValue;
            bind.ReaderQuotas.MaxArrayLength = int.MaxValue;
            bind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            bind.ReceiveTimeout = new TimeSpan(1, 0, 0);
            bind.SendTimeout = new TimeSpan(1, 0, 0);
            return bind;
        }

    }

    public static class SmsSenderFactory_10658
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        public static CSubmitState SendSMS(string mobiles, string content)
        {
            return new GeneralNoteSend_10658().SendSMS(mobiles, content);
        }
        /// <summary>
        /// 获取帐号余额
        /// </summary>
        public static int GetBalance()
        {
            return new GeneralNoteSend_10658().GetBalance();
        }
    }

    /// <summary>
    /// 接口
    /// </summary>
    interface ISMSSend_10658
    {
        CSubmitState SendSMS(string mobilephone, string content);
        int GetBalance();
    }

    abstract class SMSSend_10658
    {
        protected SMSSend_10658(string userName, string password, string scorpid, string sprdid)
        {
            sname = userName;
            spwd = password;
            Scorpid = scorpid;
            Sprdid = sprdid;
        }
        public string sname { get; set; }
        public string spwd { get; set; }
        public string Scorpid { get; set; }
        public string Sprdid { get; set; }

        protected List<string> MobilephoneSplit(string mobiles)
        {
            List<string> list = new List<string>();
            string[] arrStr = mobiles.Split(',');
            for (int i = 0; i < arrStr.Length; i += 100)
            {
                var str = arrStr.Skip(i).Take(100);
                list.Add(string.Join(",", str.ToArray()));
            }
            return list;
        }

        /// <summary>
        /// 是否还有足够余额
        /// </summary>
        /// <param name="mobiles">需要发送短信的手机个数</param>
        /// <returns>是否足够 是：true  否：false</returns>
        public bool isHaveBalance(string mobiles)
        {
            return GetBalance() >= mobiles.Split(',').Count();
            //return true;
        }

        /// <summary>
        /// 获取短信接口余额
        /// </summary>
        /// <returns></returns>
        public int GetBalance()
        {
            return Balance();//SMSServiceFactory_10658.WebService().Sm_GetRemain("lhsh0077", "87654321", "", "1012818");
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public int Balance()
        {
            var remain = 0;

            try
            {
                var balanceResult = PostManager.Post("http://chufa.lmobile.cn/submitdata/service.asmx/Sm_GetRemain", "sname=" + sname + "&spwd=" + spwd + "&scorpid=" + Scorpid + "&sprdid=" + Sprdid, Encoding.UTF8);

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(balanceResult);
                if (xdoc.GetElementsByTagName("State").Count > 0)
                {
                    var state = xdoc.GetElementsByTagName("State")[0].InnerText;
                    if (state == "0")
                    {
                        remain = int.Parse(xdoc.GetElementsByTagName("Remain")[0].InnerText);
                    }
                }
            }
            catch
            {                
            }
            
            return remain;
        }
    }

    class GeneralNoteSend_10658 : SMSSend_10658, ISMSSend_10658
    {
        public GeneralNoteSend_10658()
            : base("lhsh0077", "87654321", "", "1012818")
        {
            //Sprdid = "1012812"; //临时产品编号
        }
        public CSubmitState SendSMS(string mobiles, string content)
        {
            List<string> MobilephoneList = MobilephoneSplit(mobiles);
            CSubmitState sendResult = null;
            foreach (var item in MobilephoneList)
            {
                if (isHaveBalance(item))
                {
                    sendResult = SMSServiceFactory_10658.WebService().g_Submit(sname, spwd, Scorpid, Sprdid, mobiles, content);
                    continue;
                }
                else
                {
                    sendResult = new CSubmitState()
                    {
                        MsgState = "短信余额不足，请联系网站管理员及时充值短信。"
                    };
                    break;
                }
            }
            return sendResult;
        }
    }
}
