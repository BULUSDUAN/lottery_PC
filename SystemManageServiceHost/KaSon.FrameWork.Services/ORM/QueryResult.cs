using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    /// 结果集
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// 查询结果
        /// </summary>
        public object Data { get; set; }

        public bool IsSuccess { get; set; }
        /// <summary>
        /// 总数据行数
        /// </summary>
        public int RowCount { get; set; }
    }
}
