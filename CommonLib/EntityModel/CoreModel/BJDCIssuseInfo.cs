using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class BJDCIssuseInfo
    {
        public BJDCIssuseInfo()
        { }

        public string IssuseNumber { get; set; }
        public string MinLocalStopTime { get; set; }
        public string MinMatchStartTime { get; set; }
    }
}
