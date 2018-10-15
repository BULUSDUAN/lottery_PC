using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.ORM
{
    public class QuerySearchModel
    {

        public QuerySearchModel()
        {
    
        }



        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sorts { get; set; }



        #region 分页 参数
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
  
        #endregion

    }
}
