using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Enum;
namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    public class KJGameIssuse
    {
        /// <summary>
        /// GameCode|IssuseNumber
        /// </summary>
        public virtual string GameCode_IssuseNumber { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime LocalStopTime { get; set; }
        public virtual DateTime GatewayStopTime { get; set; }
        public virtual DateTime OfficialStopTime { get; set; }
        public virtual IssuseStatus Status { get; set; }
        public virtual string WinNumber { get; set; }
        public virtual DateTime? AwardTime { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
