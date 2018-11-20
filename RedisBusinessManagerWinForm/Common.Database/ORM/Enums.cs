using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.ORM
{
    /// <summary>
    /// 排序方向
    /// </summary>
    public enum SortDirection : int
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,
        /// <summary>
        /// 降序
        /// </summary>
        Desc
    }
    /// <summary>
    /// 删除类型
    /// </summary>
    public enum DeleteType : int
    {
        UnKnow = 0,
        Delete,
        DeleteByCriteria
    }
    /// <summary>
    /// 查询类型
    /// </summary>
    public enum SelectType : int
    {
        Unkown = 0,
        GetOneByKeyValue,
        GetOneByEntityKey,
        GetAll,
        GetList,
        GetCount,
        GetAllCount,
        GetListByExpression
    }
}
