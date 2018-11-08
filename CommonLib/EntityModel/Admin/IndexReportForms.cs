using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class IndexReportForms
    {
        public string Source { get; set; }
        public int RegisterCountByDay { get; set; }
        public int RegisterCountByMonth { get; set; }
        public int PayCount { get; set; }
        public int ReadNameCount { get; set; }
        public decimal RechargeMoneyByDay { get; set; }
        public decimal WithdrawalMoneyByDay { get; set; }
        public decimal RechargeMoneyByMonth { get; set; }
        public decimal WithdrawalMoneyByMonth { get; set; }
    }

    public class IndexReportForms_FromCount
    {
        public int TotalCount { get; set; }
        public string ComeFrom { get; set; }
    }

    public class IndexReportForms_FromTotalMoney
    {
        public decimal TotalMoney { get; set; }
        public string ComeFrom { get; set; }
    }
}
