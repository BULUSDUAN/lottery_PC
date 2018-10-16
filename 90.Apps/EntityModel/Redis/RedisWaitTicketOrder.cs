using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Redis
{
    /// <summary>
    /// Redis等待出票订单（普通投注方式）
    /// </summary>
    public class RedisWaitTicketOrder
    {
        public RedisWaitTicketOrder()
        {
            AnteCodeList = new List<C_Sports_AnteCode>();
        }

        public string KeyLine { get; set; }
        public SchemeType SchemeType { get; set; }
        public bool StopAfterBonus { get; set; }
        public C_Sports_Order_Running RunningOrder { get; set; }
        public List<C_Sports_AnteCode> AnteCodeList { get; set; }
    }
}
