using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    /// <summary>
    /// IQuery的扩展方法
    /// </summary>
    public static class QueryExtend
    {
        public static bool HasCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return true;
        }

        public static bool HasCount<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression }));
            return true;
        }

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
