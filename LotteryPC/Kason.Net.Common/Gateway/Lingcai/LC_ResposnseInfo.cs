namespace Common.Lottery.Gateway.Liangcai
{
    /// <summary>
    ///     量彩返回结果
    /// </summary>
    public class LC_ResposnseInfo
    {
        //<?xml version="1.0" encoding="gb2312"?>
        //<ActionResult>
        //<xMsgID>20140901093908</xMsgID>
        //<xCode>0</xCode>
        //<xMessage>`</xMessage>
        //<xSign>dbb05b9ea19d65fd4879528ef93e4588</xSign>
        //<xValue>2014082_01 02 03 04 05 06+07</xValue>
        //</ActionResult>

        public string MsgId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Sign { get; set; }
        public string Value { get; set; }
    }
}