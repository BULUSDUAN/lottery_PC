using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    /// <summary>
    /// 排序
    /// </summary>
  
    public class OrderByInfo
    {
        public string PropertyName { get; set; }
        //public Order Order { get; set; }
    }
  
    public class OrderByInfoList : List<OrderByInfo>
    {
    }

  
    public class TicketBatchPrizeInfo
    {
        //public long Id { get; set; }
        public string TicketId { get; set; }
        public BonusStatus BonusStatus { get; set; }
        public decimal PreMoney { get; set; }
        public decimal AfterMoney { get; set; }
    }

    public class TicketBatchPrizeInfo_Comparer : IEqualityComparer<TicketBatchPrizeInfo>
    {
        public bool Equals(TicketBatchPrizeInfo x, TicketBatchPrizeInfo y)
        {
            if (x == null)
                return y == null;
            return x.TicketId == y.TicketId;
        }

        public int GetHashCode(TicketBatchPrizeInfo obj)
        {
            if (obj == null)
                return 0;
            return obj.TicketId.GetHashCode();
        }
    }


    //public enum Order
    //{
    //    Asc = 0,
    //    Desc = 1,
    //}
}
