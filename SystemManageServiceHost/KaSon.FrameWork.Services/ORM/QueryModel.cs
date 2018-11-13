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
    public class QueryModel
    {
        public const char Separator = ';';
       
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Filters { get; set; }

        public string Values { get; set; }
        public string Sorts { get; set; }
        public int page { get; set; }

        public int limit { get; set; }

        public string Model { get; set; }
        public string Table { get; set; }
    }

  
}
