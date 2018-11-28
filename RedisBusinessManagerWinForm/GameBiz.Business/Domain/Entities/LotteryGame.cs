using System;
using System.Collections;
using GameBiz.Core;
using Common;

namespace GameBiz.Domain.Entities
{
	public class LotteryGame
	{
		public virtual string GameCode { get; set; }
		public virtual string DisplayName { get; set; }
		public virtual EnableStatus EnableStatus { get; set; }
	}
}
