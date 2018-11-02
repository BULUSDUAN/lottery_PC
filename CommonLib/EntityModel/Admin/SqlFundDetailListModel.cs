using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class Sql_FundDetail_PayIn
    {
        public int PayInCount { get; set; }
        public decimal TotalPayInMoney { get; set; }
    }

    public class Sql_FundDetail_PayOut
    {
        public int PayOutCount { get; set; }
        public decimal TotalPayOutMoney { get; set; }
    }
}
