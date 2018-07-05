using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Lottery.ApiGateway.Model.HelpModel
{
   public class LotteryServiceRequest
    {
        /// <summary>
        /// 请求来源
        /// </summary>
        public SchemeSource SourceCode { get; set; }     
        /// <summary>
        /// 消息序号（服务器原样返回）
        /// </summary>
        public string MsgId { get; set; }
        /// <summary>
        /// 业务参数
        /// </summary>
        public string Param { get; set; }
        /// <summary>
        /// 加密串
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 检查客户端与服务端MD5值
        /// </summary>
        /// <returns></returns>
        public bool CheckSign()
        { 
            //HttpContent.Current.Server.UrlDecode("");
            //todo:
            string sourceString = "caibb" + MsgId + (int)SourceCode + Param;
            return Sign.ToUpper() == Encipherment.MD5(sourceString, Encoding.UTF8).ToUpper();
        }

        public static LotteryServiceRequest RequestToEntity(NameValueCollection dic)
        {

            //if (!dic.AllKeys.Contains("SourceCode"))
            //    throw new ArgumentException("未传入SourceCode");
            //if (!dic.AllKeys.Contains("Action"))
            //    throw new ArgumentException("未传入Action");
            //if (!dic.AllKeys.Contains("MsgId"))
            //    throw new ArgumentException("未传入MsgId");
            if (!dic.AllKeys.Contains("Param"))
                throw new ArgumentException("未传入Param");
            //if (!dic.AllKeys.Contains("Sign"))
            //    throw new ArgumentException("未传入Sign");

            return new LotteryServiceRequest()
            {
                SourceCode = (SchemeSource)103,
                //MsgId = dic["MsgId"],
                MsgId = "",
                Param = dic["Param"],
                //Sign = dic["Sign"],
                Sign = "",
            };
        }
    }
}
