using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Redis
{
    /// <summary>
    /// Redis等待出票订单（单式上传投注方式）
    /// </summary>
    public class RedisWaitTicketOrderSingle
    {
        public C_Sports_Order_Running RunningOrder { get; set; }
        public C_SingleScheme_AnteCode AnteCode { get; set; }

    }
}
