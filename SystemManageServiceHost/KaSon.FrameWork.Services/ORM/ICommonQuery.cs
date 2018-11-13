using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    public interface ICommonQuery
    {
        /// <summary>
        /// 多表通用查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <typeparam name="TResult"></typeparam>
        QueryResult Query<TResult>(IQueryable<TResult> query, QueryArgs args); //where TResult : class, new();
    }
}
