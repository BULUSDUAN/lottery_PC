using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Pay
{
    public static class PaymentLock
    {
        public static readonly object BYPayLock = new object();

        public static readonly object YFPayLock = new object();

        public static readonly object WHPayLock = new object();
    }
}
