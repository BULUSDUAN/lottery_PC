using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityModel.LotteryJsonInfo
{
    public class AppPay
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public List<Pay> pay { get; set; }
        public int[] amount { get; set; }
    }

    public class Pay
    {
        public string icon { get; set; }
        public string iconUrl { get; set; }
        public string[] desc { get; set; }
        public object[] bank { get; set; }
        public string title { get; set; }
        public string gateway { get; set; }
        public object openUrl { get; set; }
        public string webViewUrl { get; set; }
        public string actionUrl { get; set; }
        public object payType { get; set; }
    }
}