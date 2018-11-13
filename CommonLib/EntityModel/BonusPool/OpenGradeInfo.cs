using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.BonusPool
{
    public class OpenGradeInfo
    {
        /// <summary>
        /// 奖等索引
        /// </summary>
        public int GradeIndex { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 奖等名称
        /// </summary>
        public string GradeName { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public string Attr { get; set; }
        /// <summary>
        /// 中奖个数
        /// </summary>
        public int BonusCount { get; set; }
        /// <summary>
        /// 单注奖金
        /// </summary>
        public decimal BonusMoney { get; set; }
    }
}
