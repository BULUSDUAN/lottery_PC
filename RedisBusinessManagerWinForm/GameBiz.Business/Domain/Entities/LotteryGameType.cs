using System;
using System.Collections;
using GameBiz.Core;
using Common;

namespace GameBiz.Domain.Entities
{
	public class LotteryGameType
	{
        //GameCode|GameType
		public virtual string GameTypeId { get; set; }
		public virtual LotteryGame Game { get; set; }
		public virtual string GameType { get; set; }
		public virtual string DisplayName { get; set; }
		public virtual EnableStatus EnableStatus { get; set; }
        public virtual int TicketCount { get; set; }
    }
}
