using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    
        public class Bank
        {
            public string name { get; set; }
            public string code { get; set; }
        }
        public class WebPayItem
        {
            /// <summary>
            /// 图标资源
            /// </summary>
            public string icon { get; set; }


            /// <summary>
            /// 图标URL，icon为空时使用
            /// </summary>
            public string iconUrl { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            public List<string> desc { get; set; }

            public List<Bank> bank { get; set; }

            /// <summary>
            /// 标题
            /// </summary>
            public string title { get; set; }


            /// <summary>
            /// 
            /// </summary>
            public string gateway { get; set; }







            /// <summary>
            /// 系统默认浏览器URL
            /// </summary>
            public string openUrl { get; set; }

            /// <summary>
            /// 内置WebViewURL,openUrl为空时使用
            /// </summary>
            public string webViewUrl { get; set; }


            public string actionUrl { get; set; }
            public string payType { get; set; }

        public List<int> amounts { get; set; }


    }


    public class CallBackParam
    {
        public string BankCardNo { get; set; }
        public string CurrentUser { get; set; }
        public string payAmount { get; set; }
        public string CurrentDomain { get; set; }
        public string HdpayType { get; set; }
        public string ActionUrl { get; set; }

    }
}
