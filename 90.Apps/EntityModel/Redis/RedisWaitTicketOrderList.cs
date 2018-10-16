using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Redis
{
    /// <summary>
    /// Redis等待出票订单列表（普通投注方式，追号方式）
    /// </summary>
    public class RedisWaitTicketOrderList
    {
        public RedisWaitTicketOrderList()
        {
            OrderList = new List<RedisWaitTicketOrder>();
        }

        public string KeyLine { get; set; }
        public bool StopAfterBonus { get; set; }

        public List<RedisWaitTicketOrder> OrderList { get; set; }
    }
}
