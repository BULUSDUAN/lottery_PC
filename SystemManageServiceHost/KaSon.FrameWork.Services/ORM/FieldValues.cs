using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    /// 查询条件字段对象
    /// </summary>
    public class FieldValues
    {
        /// <summary>
        /// 查询字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 查询类型
        /// </summary>
       // public WhereType WhereType { get; set; }
    }
}
