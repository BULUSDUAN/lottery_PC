using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    public abstract class JingCai_Odds : I_JingCai_Odds
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public virtual string MatchId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        public abstract decimal GetOdds(string result);
        public abstract bool CheckIsValidate();
        public abstract void SetOdds(I_JingCai_Odds odds);
        public abstract bool Equals(I_JingCai_Odds odds);
        public abstract string GetOddsString();

    }
}
