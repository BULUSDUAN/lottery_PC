using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
   public class Page 
    {
        /// <summary>
        /// 当前页数
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 允许最大数
        /// </summary>
        public int MaxPageSize = 200;
    }
}
