using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20130808Info
    {
        public  long Id { get; set; }
        public  string UserId { get; set; }
        public  string RealName { get; set; }
        public  string Mobile { get; set; }
        public  decimal GiveMoney { get; set; }
        public  DateTime UpdateTime { get; set; }
    }
    [CommunicationObject]
    public class A20130808Info_Collection
    {
        public A20130808Info_Collection()
        {
            ActListInfo = new List<A20130808Info>();
        }
        public int TotalCount { get;set; }
        public decimal TotalGiveMoney { get; set; }
        public  List<A20130808Info> ActListInfo{get;set;}
    }
}
