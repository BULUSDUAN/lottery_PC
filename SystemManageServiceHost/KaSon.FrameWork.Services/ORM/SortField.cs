using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    /// 排序字段对象
    /// </summary>
    public class SortField
    {
        /// <summary>
        /// 排序字段对象
        /// </summary>
        public SortField()
        {
            this.IsASC = true;
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 是否按从小到大的顺序
        /// </summary>
        public bool IsASC { get; set; }
    }
}
