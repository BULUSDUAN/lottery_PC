using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.PayModel
{ /// <summary>
  /// 资金收入/支出明细
  /// </summary>
    public class PayDetail
    {
        public AccountType AccountType { get; set; }
        public EntityModel.Enum.PayType PayType { get; set; }
        public decimal PayMoney { get; set; }
    }
}
