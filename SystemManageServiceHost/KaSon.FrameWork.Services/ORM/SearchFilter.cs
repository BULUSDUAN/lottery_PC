using KaSon.FrameWork.Services.Enum;
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
    public class SearchFilter
    {
        public string Field { get; set; }

        public string Value { get; set; }

        public WhereType WhereType { get; set; }
    }
}
