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
    public class QueryArgs
    {
        /// <summary>
        /// Ado数据查询参数
        /// </summary>
        public QueryArgs()
        {
            this.WhereFields = new List<WhereField>();
            this.QueryFields = new List<string>();
            this.SortFields = new List<SortField>();
        }

        /// <summary>
        /// 实体对象类型名称(带命名空间)
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 结果字段列表
        /// </summary>
        public List<string> QueryFields { get; set; }

        /// <summary>
        /// 排序字段列表
        /// </summary>
        public List<SortField> SortFields { get; set; }

        /// <summary>
        /// 条件字段列表
        /// </summary>
        public List<WhereField> WhereFields { get; set; }
    }
}
