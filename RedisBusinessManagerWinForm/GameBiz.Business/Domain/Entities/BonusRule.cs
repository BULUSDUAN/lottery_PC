using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Auth.Domain.Entities;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 中奖规则
    /// </summary>
    public class BonusRule
    {
        public virtual int Id { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual int BonusGrade { get; set; }
        public virtual string BonusGradeDisplayName { get; set; }
        public virtual decimal BonusMoney { get; set; }
    }
}
