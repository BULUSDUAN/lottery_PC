using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Lottery.Gateway.HuAi
{
    /// <summary>
    /// 互爱 返回对象
    /// </summary>
    public class HuAi_ResponseInfo
    {
        //<?xml version="1.0" encoding="UTF-8"?> 
        //<ActionResult> 
        //    <reMsgID>98643738107324</reMsgID> 
        //    <reCode>0</reCode> 
        //    <reMessage>成功</reMessage> 
        //    <reSign>1f273f00098427516dd99792fdc046d9</reSign> 
        //<reValue>
        //  <Order OrderID="201406195001" LotID="2201" LotIssue="2014061904" Status="4">
        //    Ticket Seq="1" Status="N" Errmsg="未调度，已过期" TicketID="" 
        //      GroundID="" Odds=""/>
        //  </Order>
        //</reValue> 
        //</ActionResult>

        /// <summary>
        /// 交易消息序号，由代理商生成，服务器响应时将其原样返回给客户。
        /// </summary>
        public string reMsgID { get; set; }
        /// <summary>
        /// 处理结果码，0-成功，其他为投注失败
        /// </summary>
        public int reCode { get; set; }
        /// <summary>
        /// 处理结果提示信息，如果交易处理失败，可能返回错误提示信息。
        /// </summary>
        public string reMessage { get; set; }
        /// <summary>
        /// 交易处理结果，其格式为：参数值_参数值… 
        /// 多个参数值之间以下划线（_）连接。 
        /// </summary>
        public string reValue { get; set; }
        /// <summary>
        /// 服务端签名，（签名方式参照【加密方式】）。签名源字符串由：reMsgID+reCode+reValue+代理商密钥，按顺序连接而成。
        /// </summary>
        public string reSign { get; set; }

    }
}
