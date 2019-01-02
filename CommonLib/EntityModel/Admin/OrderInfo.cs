﻿using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
   public class OrderInfo
    {
       
        public Sports_SchemeQueryInfoCollection OrdersSearchResult;
      
        //public IEnumerable<IGrouping<DateTime, Sports_SchemeQueryInfo>> GroupOrderList;
        
    }

    public class BettingInfo
    {
        public BettingOrderInfoCollection OrdersSearchResult;
        //public IEnumerable<IGrouping<DateTime, BettingOrderInfo>> GroupOrderList;
    }


    public class OrderDetailInfo
    {
        public BettingOrderInfoCollection DetailResult;
        public BettingOrderInfo FirstItem;
        public BettingAnteCodeInfoCollection FirstAnteCode;
        public UserQueryInfo UserResult;
        public string UserKey;
        public Sports_SchemeQueryInfo OrderDetail;
        public Sports_AnteCodeQueryInfoCollection CodeList;
    }

 
}