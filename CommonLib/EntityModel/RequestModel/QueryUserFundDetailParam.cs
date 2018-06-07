﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryUserFundDetailParam : Page
    {
        public QueryUserFundDetailParam()
        {
            this.keyLine = string.Empty;
        }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string accountTypeList { get; set; }
        public string categoryList { get; set; }
        public string userToken { get; set; }
        public string keyLine { get; set; }
    }
}
