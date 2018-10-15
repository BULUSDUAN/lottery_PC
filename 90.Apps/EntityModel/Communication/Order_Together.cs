using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Communication
{
    /// <summary>
    /// 订单组合模型
    /// </summary>
    [Serializable]
    public  class Order_Together
    {
        /// <summary>
        /// 
        /// </summary>
        public C_Sports_Order_Running Order_Running { get; set; }


        /// <summary>
        /// 订单详情
        /// </summary>
        public C_OrderDetail OrderDetail { get; set; }
    }
}
