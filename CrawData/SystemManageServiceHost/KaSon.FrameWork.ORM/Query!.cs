using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class Query<T> : IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable
    {
        private Provider.DbProvider _provider;
        /// <summary>
        /// 表达式树
        /// </summary>
        private readonly System.Linq.Expressions.Expression _expression;
      
        public Query(Provider.DbProvider provider,bool isLLink=false)
        {
            // TODO: Complete member initialization
            Type type = typeof(T);
            var Context = provider.QueryContext;
            //添加查询字段

            Context.SafeAddQueryParameter(type);
            Context.IsLLink = isLLink;

            this._provider = provider;
            this._expression = System.Linq.Expressions.Expression.Constant(this);
        }

        public Query(ORM.Provider.DbProvider dbProvider, System.Linq.Expressions.Expression expression)
        {
            // TODO: Complete member initialization
            this._provider = dbProvider;
            this._expression = expression;
        }

        #region IEnumerable 迭代接口
        /// <summary>
        /// IEnumerable 迭代接口
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region IQueryable 接口
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            //QueryProvider ExecuteQuery
            return this._provider.QueryContext.QueryProvider.ExecuteQuery<T>(this._expression).GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

     

        public IQueryProvider Provider
        {
            get
            {
                return this._provider.QueryContext.QueryProvider;
            }
        }
        #endregion

        #region IQueryable 接口
        public System.Linq.Expressions.Expression Expression
        {
            get { return this._expression; }
        }
        #endregion
    }
}
