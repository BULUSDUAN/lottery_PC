using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    public static class QueryExtend
    {
        /// <summary>
        /// 分页功能
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rowCount">每页的行数</param>
        /// <param name="pageNumber">第几页</param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int rowCount, int pageNumber)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(rowCount), Expression.Constant(pageNumber) }));
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="joins"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IQueryable<TSource> SetJoinMode<TSource>(this IQueryable<TSource> source, params JoinMode[] joins)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(joins) }));
        }
    }
}
